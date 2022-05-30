using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    [SerializeField] HUDManager hudManager;
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject pathGO;
    [SerializeField] float secondsToLand = 5f;

    void Start()
    {
        playerTransform.position = spawnPoint.position;
        StartCoroutine(DoStarterThing());
    }

    private IEnumerator DoStarterThing()
    {
        pathGO.SetActive(false);

        yield return new WaitForSeconds(secondsToLand);

        pathGO.SetActive(true);
    }
}
