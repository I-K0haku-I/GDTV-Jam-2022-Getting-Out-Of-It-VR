using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoingOutsideTrigger : MonoBehaviour
{
    [SerializeField] float darknessDelay;
    [SerializeField] HUDManager hudManager;
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject[] enableObjects;
    [SerializeField] GameObject gameObjectToChange;
    private bool isTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        isTriggered = true;

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            gameObjectToChange.layer = 0;
            audioSource.Play();
            foreach (var go in enableObjects)
            {
                go.SetActive(true);
            }
            StartCoroutine(EnterDarknessLater());
        }
    }

    private IEnumerator EnterDarknessLater()
    {
        yield return new WaitForSeconds(darknessDelay);
        hudManager.EnterDarknessAndKeepIt();
    }
}
