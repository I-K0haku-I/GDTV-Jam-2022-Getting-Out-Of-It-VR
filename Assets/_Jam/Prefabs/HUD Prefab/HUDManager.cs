using System;
using UnityEngine;


public class HUDManager : MonoBehaviour
{
    public Animator Anim;
    //public MeshRenderer DarknessRenderer;
    //public AnimationCurve DarknessCurve;
    public float TransitionDuration = 0.5f;

    //private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

    void Start()
    {
        Anim = GetComponent<Animator>();
    }

    void Update()
    {
        //TeleportToHead();
    }

    public void EnterDarkness()
    {
        Anim.SetTrigger("doTransition");
    }

    public void EnterDarknessAndKeepIt()
    {
        Anim.SetTrigger("doTransition2");
    }

    //public void TeleportToHead()
    //{
    //    transform.SetPositionAndRotation(trackingData.position, trackingData.rotation);
    //}
}
