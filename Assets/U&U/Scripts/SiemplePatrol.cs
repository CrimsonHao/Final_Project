using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SiemplePatrol : MonoBehaviour
{
    // Dictates whether the agent waits on node
    [SerializeField] bool patrolWaiting;

    //The time it waits at each node
    [SerializeField] float totalWaitTime = 3f;

    //Probability to switch direction
    [SerializeField] float switchPorb = 0.2f;

    //List of all nodes to visit
    [SerializeField] List<Waypoints> patrolPoints;

    //Variables for base behaviour
    NavMeshAgent navMeshAgent;
    int currentPatrolIndex;
    bool isTraveling;
    bool isWaiting;
    bool patrolForward;
    float waitTimer;

    public void Start()
    {
        navMeshAgent = this.GetComponent<NavMeshAgent>();

        if(navMeshAgent == null)
        {
            Debug.LogError("Tne navmesh agent component is not attached to " + gameObject.name);
        }
        else
        {
            if(patrolPoints != null && patrolPoints.Count >= 2)
            {
                currentPatrolIndex = 0;
                SetDestination();
            }
            else
            {
                Debug.Log("Insufficient patrol points for basic behaviour");
            }
        }
    }

    private void Update()
    {
        //Check if we are close to destination
        if(isTraveling && navMeshAgent.remainingDistance <= 1.0f)
        {
            isTraveling = false;

            //If we are going to wait, wait
            if(isWaiting)
            {
                isWaiting = true;
                waitTimer = 0f;
            }
            else
            {
                ChangePatrolPoint();
                SetDestination();
            }
        }

        //If we are waiting
        if(isWaiting)
        {
            waitTimer += Time.deltaTime;

            if(waitTimer >= totalWaitTime)
            {
                isWaiting = false;

                ChangePatrolPoint();
                SetDestination();
            }
        }
    }

    private void SetDestination()
    {
        if(patrolPoints != null)
        {
            Vector3 targetVector = patrolPoints[currentPatrolIndex].transform.position;
            navMeshAgent.SetDestination(targetVector);
            isTraveling = true;
        }
    }

    ///Selects a new patrol point in the aviable list
    ///with a small probability for us to move fwd or bkwd
    private void ChangePatrolPoint()
    {
        if(UnityEngine.Random.Range(0f, 1f) <= switchPorb)
        {
            patrolForward = !patrolForward;
        }

        if(patrolForward)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        }
        else
        {
            if(--currentPatrolIndex < 0)
            {
                currentPatrolIndex = patrolPoints.Count - 1;
            }
        }
    }
}
