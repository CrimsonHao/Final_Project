using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class New_Waypoint : Waypoints
    {
        [SerializeField] protected float conectivityRad = 50f;

        List<New_Waypoint> connections;

        public void Start()
        {
            //Grab all waypoints in scene
            GameObject[] allWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");

            //Create a list of such waypoints
            connections = new List<New_Waypoint>();

            //Check if they are a connected waypoint
            for(int i = 0; i < allWaypoints.Length; i++)
            {
                New_Waypoint nextWaypoint = allWaypoints[i].GetComponent<New_Waypoint>();

                if(nextWaypoint != null)
                {
                    if(Vector3.Distance(this.transform.position, nextWaypoint.transform.position) <= conectivityRad && nextWaypoint != this)
                    {
                        connections.Add(nextWaypoint);
                    }
                }
            }
        }

        public override void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, debugDrawRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, conectivityRad);
        }

        public New_Waypoint NextWaypoint (New_Waypoint previousWaypoint)
        {
            if(connections.Count == 0)
            {
                //No waypoints, return null and warn
                Debug.LogError("Insufficient waypoint count");
                return null;
            }
            else if(connections.Count == 1 && connections.Contains(previousWaypoint))
            {
                //Only one waypoint and its the previous one
                return previousWaypoint;
            }
            else
            {
                //Find a random waypoint that is not the previous one
                New_Waypoint nextWaypoint;
                int nextIndex = 0;

                do
                {
                    nextIndex = UnityEngine.Random.Range(0, connections.Count);
                    nextWaypoint = connections[nextIndex];
                } while (nextWaypoint == previousWaypoint);

                return nextWaypoint;
            }
        }
    }
}

