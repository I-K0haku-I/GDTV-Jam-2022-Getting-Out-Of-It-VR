using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHandler : MonoBehaviour
{
    [SerializeField] PhysicsHand rightHand;
    [SerializeField] PhysicsHand leftHand;

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
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;
        
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

    public void PlayStep()
    {
        audioSource.PlayOneShot(stepClip);
    }

    public AudioClip GetStepClip()
    {
        return stepClip;
    }
}
