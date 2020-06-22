using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Code
{
    public class New_ConnectedPatrol : MonoBehaviour
    {
        enum EnemyState { Patrol, Chase };

        [SerializeField] bool patrolWaiting; //Decide si el agente espera en cada nodo o no

        [SerializeField] float totalWaitTime = 3f; //Tiempo total de espera en el nodo

        [SerializeField] float switchProb = 0.2f; //Probabilidad de cambio de direccion

        [SerializeField] EnemyState currentState; //Estado del enemigo

        NavMeshAgent navMeshAgent;                                                                                  //Variables de comportamiento
        New_Waypoint currentWaypoint;                                                                               //
        New_Waypoint previousWaypoint;                                                                              //
                                                                                                                    //
        bool isTraveling;                                                                                           //
        bool isWaiting;                                                                                             //
        float waitTimer;                                                                                            //
        int waypointsVisited;                                                                                       //

        public void Start()
        {
            navMeshAgent = this.GetComponent<NavMeshAgent>();  //Se asigna el componente "NavMeshAgent" a la variable

            if (navMeshAgent == null) //Revisa en caso que no tenga el componente
            {
                Debug.LogError("The nav mesh agent component is not ettached to " + gameObject.name);
            }
            else
            {
                if(currentWaypoint == null) //Revisa si la variable es nula
                {
                    GameObject[] allWaypoints = GameObject.FindGameObjectsWithTag("Waypoint"); //Guarda todos los objetos con tag "Waypoint" en un array

                    if (allWaypoints.Length > 0) // Si el array contiene objetos
                    {
                        while (currentWaypoint == null) //Mientras la variable sea nula
                        {
                            int random = UnityEngine.Random.Range(0, allWaypoints.Length); //Variable local: Numero aleatorio entre 0 y longitud de array
                            New_Waypoint startingWaypoint = allWaypoints[random].GetComponent<New_Waypoint>(); //Establece la variable startingWaypoint como un objeto random del array "allWaypoints"

                            if(startingWaypoint != null) //Revisa si startingWaypoint no sea nula
                            {
                                currentWaypoint = startingWaypoint; //Obvio
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to find any waypoints for use in scene"); //Si no encuentra ningun objeto con el tag
                    }
                }

                SetDestination(); //Ejecuta el metodo
            }

            currentState = EnemyState.Patrol; //Cambia el estado a patrullaje
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


