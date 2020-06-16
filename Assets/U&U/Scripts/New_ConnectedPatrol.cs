using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Code
{
    public class New_ConnectedPatrol : MonoBehaviour
    {
        //Dictates whether the agent waits on each node
        [SerializeField] bool patrolWaiting;

        //Total time to wait at each node
        [SerializeField] float totalWaitTime = 3f;

        //Probability to change direction
        [SerializeField] float switchProb = 0.2f;

        //Variables to base behaviour
        NavMeshAgent navMeshAgent;
        New_Waypoint currentWaypoint;
        New_Waypoint previousWaypoint;

        bool isTraveling;
        bool isWaiting;
        float waitTimer;
        int waypointsVisited;

        public void Start()
        {
            navMeshAgent = this.GetComponent<NavMeshAgent>();

            if (navMeshAgent == null)
            {
                Debug.LogError("The nav mesh agent component is not ettached to " + gameObject.name);
            }
            else
            {
                if(currentWaypoint == null)
                {
                    //Set it random and grab all waypoints objects in scene
                    GameObject[] allWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");

                    if(allWaypoints.Length > 0)
                    {
                        while (currentWaypoint == null)
                        {
                            int random = UnityEngine.Random.Range(0, allWaypoints.Length);
                            New_Waypoint startingWaypoint = allWaypoints[random].GetComponent<New_Waypoint>();

                            if(startingWaypoint != null)
                            {
                                currentWaypoint = startingWaypoint;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to find any waypoints for use in scene");
                    }
                }

                SetDestination();
            }
            
        }

        public void Update()
        {
            //Check if it is close to the destination
            if(isTraveling && navMeshAgent.remainingDistance <= 1.0f)
            {
                isTraveling = false;
                waypointsVisited++;

                //Check if we are waiting, then wait
                if(patrolWaiting)
                {
                    isWaiting = true;
                    waitTimer = 0f;
                }
                else
                {
                    SetDestination();
                }
            }

            // If you are waiting
            if(isWaiting)
            {
                waitTimer += Time.deltaTime;
                if(waitTimer >= totalWaitTime)
                {
                    isWaiting = false;
                }
            }
        }

        private void SetDestination()
        {
            if(waypointsVisited > 0)
            {
                New_Waypoint nextWaypoint = currentWaypoint.NextWaypoint(previousWaypoint);
                previousWaypoint = currentWaypoint;
                currentWaypoint = nextWaypoint;
            }

            Vector3 targetVector = currentWaypoint.transform.position;
            navMeshAgent.SetDestination(targetVector);
            isTraveling = true;
        }
    }
}


