using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(1)]
public class PlayerHandler : MonoBehaviour
{
    [SerializeField] PhysicsHand rightHand;
    [SerializeField] PhysicsHand leftHand;

    [SerializeField] LayerMask layerToIgnoreForce;
    [SerializeField] AudioClip dieClip;
    [SerializeField] AudioClip stepClip;
    [SerializeField] ParticleSystem ressurectFX;
    [SerializeField] float ressurrectEffectTime = 1.2f;
    [SerializeField] float dieSpeedThreshold = 10f;
    public UnityEvent onDie;
    bool isDead;

    Rigidbody rb;
    AudioSource audioSource;

    float prevSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        prevSpeed = rb.velocity.magnitude;

        if (nextForce == Vector3.zero) return;

        rb.AddForce(nextForce, ForceMode.Acceleration);
        rb.AddForce(nextDrag, ForceMode.Acceleration);
        nextForce = Vector3.zero;
        nextDrag = Vector3.zero;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if ((layerToIgnoreForce & (1 << collision.gameObject.layer)) != 0) return;

        if (isDead) return;

        if (Vector3.Dot(rb.velocity, Vector3.up) > 0f) return;

        if (prevSpeed > dieSpeedThreshold)
        {
            StartCoroutine(HandleDeath());
        }
    }

    private IEnumerator HandleDeath()
    {
        audioSource.PlayOneShot(dieClip);
        onDie.Invoke();

        isDead = true;
        rightHand.IsDisabled = true;
        leftHand.IsDisabled = true;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        yield return new WaitForSeconds(ressurrectEffectTime);

        ressurectFX.Play();
        ressurectFX.gameObject.GetComponent<AudioSource>().Play();

        rb.isKinematic = false;
        isDead = false;
        rightHand.IsDisabled = false;
        leftHand.IsDisabled = false;
    }


    Vector3 nextForce;
    Vector3 nextDrag;
    public void AddNextForce(Vector3 force, Vector3 drag)
    {
        nextForce += force;
        if (Vector3.Dot(nextForce, Vector3.up) < 0f) return;
        nextDrag += drag;
    }

    public void PlayStep()
    {
        audioSource.PlayOneShot(stepClip);
    }

    public AudioClip GetStepClip()
    {
        return stepClip;
    }
}
