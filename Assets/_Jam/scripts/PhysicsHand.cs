using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsHand : MonoBehaviour
{
    [SerializeField] PlayerHandler playerHandler;
    [SerializeField] LayerMask layerToIgnoreForce;
    [SerializeField] float hitSoundThreshold = 1f;
    [SerializeField] float hitSoundCooldown = 0.3f;

    [Header("PID")]
    [SerializeField] Rigidbody playerRb;
    [SerializeField] Transform target;
    [SerializeField] float frequency = 50f;
    [SerializeField] float damping = 1f;
    [SerializeField] float rotfrequency = 100f;
    [SerializeField] float rotDamping = 0.9f;

    [Space]
    [Header("Spring")]
    [SerializeField] float climbForce = 1000f;
    [SerializeField] float climbDrag = 500f;

    Rigidbody _rb;
    Vector3 _previousPosition;
    bool isColliding;

    private bool isDisabled;
    private bool isHitCooldown;

    public bool IsDisabled
    {
        get
        {
            return isDisabled;
        }
        set
        {
            _rb.isKinematic = value;
            isDisabled = value;
        }
    }

    private void Start()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
        _rb = GetComponent<Rigidbody>();
        _rb.maxAngularVelocity = float.PositiveInfinity;
        _previousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (IsDisabled) return;
        PIDMovement();
        PIDRotation();
        if (isColliding)
            HookesLaw();
    }

    private void HookesLaw()
    {
        Vector3 displacementFromResting = transform.position - target.position;
        Vector3 force = displacementFromResting * climbForce;
        float drag = GetDrag();

        playerRb.AddForce(force, ForceMode.Acceleration);
        playerRb.AddForce(drag * -playerRb.velocity * climbDrag, ForceMode.Acceleration);
    }

    private float GetDrag()
    {
        Vector3 handVelocity = (target.localPosition - _previousPosition) / Time.fixedDeltaTime;
        float drag = 1f / (handVelocity.magnitude + 0.01f);
        //drag = drag > 1 ? 1 : drag;
        //drag = drag < 0.03f ? 0.03f : drag;
        drag = Mathf.Clamp(drag, 0.03f, 1f);
        _previousPosition = transform.position;
        return drag;
    }

    private void PIDRotation()
    {
        float kp = (6f * frequency) * (6f * frequency) * 0.25f;
        float kd = 4.5f * frequency * damping;
        float g = 1f / (1f + kd * Time.fixedDeltaTime + kp * Time.fixedDeltaTime * Time.fixedDeltaTime);
        float ksg = kp * g;
        float kdg = (kd + kp * Time.fixedDeltaTime) * g;
        Vector3 force = (target.position - transform.position) * ksg + (playerRb.velocity - _rb.velocity) * kdg;
        _rb.AddForce(force, ForceMode.Acceleration);
    }

    private void PIDMovement()
    {
        float kp = (6f * rotfrequency) * (6f * rotfrequency) * 0.25f;
        float kd = 4.5f * rotfrequency * rotDamping;
        float g = 1 / (1 + kd * Time.fixedDeltaTime + kp * Time.fixedDeltaTime * Time.fixedDeltaTime);
        float ksg = kp * g;
        float kdg = (kd + kp * Time.fixedDeltaTime) * g;
        Quaternion q = target.rotation * Quaternion.Inverse(transform.rotation);
        if (q.w < 0)
        {
            q.x = -q.x;
            q.y = -q.y;
            q.z = -q.z;
            q.w = -q.w;
        }
        q.ToAngleAxis(out float angle, out Vector3 axis);
        axis.Normalize();
        axis *= Mathf.Deg2Rad;
        Vector3 torque = ksg * axis * angle + -_rb.angularVelocity * kdg;
        _rb.AddTorque(torque, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        playerHandler.OnCollisionEnter(collision);
        if ((layerToIgnoreForce & (1 << collision.gameObject.layer)) != 0) return;
        isColliding = true;
        if (!isHitCooldown && _rb.velocity.magnitude > hitSoundThreshold)
        {
            AudioSource.PlayClipAtPoint(playerHandler.GetStepClip(), collision.GetContact(0).point);
            StartCoroutine(PlayHitSound());
        }
    }

    private IEnumerator PlayHitSound()
    {
        isHitCooldown = true;
        yield return new WaitForSeconds(hitSoundCooldown);
        isHitCooldown = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((layerToIgnoreForce & (1 << collision.gameObject.layer)) != 0) return;
        isColliding = false;
    }
}
