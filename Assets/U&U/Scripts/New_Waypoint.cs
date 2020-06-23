using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class New_Waypoint : MonoBehaviour
    {
        [SerializeField] protected float conectivityRad = 50f; //Radio de conectividad; donde se detectan entre si los waypoints
        [SerializeField] protected float debugDrawRadius = 0.5f; //Radio de dibujo del waypoint (para ubicacion)

        List<New_Waypoint> connections; //Se declara la lista de new_waypoints

        public void Start()
        {
            GameObject[] allWaypoints = GameObject.FindGameObjectsWithTag("Waypoint"); //Coloca todos los game objects con tag "Waypoint" en un array

            connections = new List<New_Waypoint>(); //Se inicializa la lista de new_waypoints

            for (int i = 0; i < allWaypoints.Length; i++) //Por cada objecto del array...
            {
                New_Waypoint nextWaypoint = allWaypoints[i].GetComponent<New_Waypoint>(); //Establece a nextWaypoint el componente new_waypoint del objeto del array

                if(nextWaypoint != null) //Revisa que contenga algo
                {
                    if(Vector3.Distance(this.transform.position, nextWaypoint.transform.position) <= conectivityRad && nextWaypoint != this) //Revisa si la distancia entre este objecto y nextWaypoint es <= al radio de conectividad Y nextwaypoint no se si mismo
                    {
                        connections.Add(nextWaypoint); //Se agrega a la lista
                    }
                }
            }
        }

        public void OnDrawGizmos() //Dibuja el radio de localizacion y de influencia de los waypoints
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, debugDrawRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, conectivityRad);
        }

        public New_Waypoint NextWaypoint (New_Waypoint previousWaypoint)
        {
            if(connections.Count == 0) //Revisa que la lista contenga objetos
            {
                Debug.LogError("Insufficient waypoint count"); //No hay waypoints, avisa de error y devuelve nulo
                return null;
            }
            else if(connections.Count == 1 && connections.Contains(previousWaypoint)) //Revisa si la lista solo tiene 1 elemento y si es el mismo
            {
                Debug.LogWarning("Only one waypoint to move to..."); //Solo existe un waypoint, avisa de error y regresa el mismo
                return previousWaypoint;
            }
            else //Ninguna de las anteriores, encuentra un waypoint aleatorio que no sea el pasado
            {
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

