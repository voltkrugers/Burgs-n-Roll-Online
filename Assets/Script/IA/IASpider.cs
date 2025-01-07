using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class IASpider : MonoBehaviour
{
    public Transform[] waypoints;
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;
    
    private NavMeshAgent navMeshAgent;
    private int currentWaypointIndex = -1;
    private bool isWaiting;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        StartCoroutine(MoveToNextWaypoint());
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.CompareTag("Player"))
        {
            CharacterController playerController = other.GetComponent<CharacterController>();
        
            if (playerController != null)
            {
                playerController.StunCharacter(3);
            }
            else
            {
                Debug.LogError("CharacterController is missing on the Player!");
            }
        }
    }


    private IEnumerator MoveToNextWaypoint()
    {
        while (true)
        {
            if (!isWaiting && !navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
            {
                isWaiting = true; // Commence à attendre
                float waitTime = Random.Range(minWaitTime, maxWaitTime);
                yield return new WaitForSeconds(waitTime); // Temps d'attente aléatoire

                SetNextWaypoint();
                isWaiting = false; // Arrête d'attendre
            }
            yield return null;
        }
    }

    private void SetNextWaypoint()
    {
        if (waypoints.Length == 0)
            return;

        int newWaypointIndex;
        do
        {
            newWaypointIndex = Random.Range(0, waypoints.Length);
        } while (newWaypointIndex == currentWaypointIndex);

        currentWaypointIndex = newWaypointIndex;
        navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
    }
}
