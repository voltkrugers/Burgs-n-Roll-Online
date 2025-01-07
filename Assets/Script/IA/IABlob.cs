using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class IABlob : MonoBehaviour
{
    private NavMeshAgent agent;
    public float radius = 1.0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;  // Permet de laisser l'agent continuer à chercher sans s'arrêter trop tôt
        StartCoroutine(BehaviourTree());
    }

    private IEnumerator BehaviourTree()
    {
        while (true)
        {
            SetRandomTargetPosition();
            yield return StartCoroutine(MoveToTarget());
            yield return new WaitForSeconds(3f);
        }
    }

    private void SetRandomTargetPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            Debug.Log("New target set: " + hit.position);
        }
        else
        {
            Debug.Log("Failed to find a valid NavMesh location");
        }
    }

    private IEnumerator MoveToTarget()
    {
        while (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        Debug.Log("AI reached target: " + agent.destination);
        yield return null;
    }
}
