using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Komoditos B4 Kulitos :3
namespace Assets.Code
{
    public class New_ConnectedPatrol : MonoBehaviour
    {
        enum EnemyState { Patrol, Initializing, Chase };

        [SerializeField] bool patrolWaiting; //Decide si el agente espera en cada nodo o no
        [SerializeField] float totalWaitTime = 3f; //Tiempo total de espera en el nodo
        [SerializeField] float switchProb = 0.2f; //Probabilidad de cambio de direccion
        [SerializeField] EnemyState currentState; //Estado actual del enemigo
        [SerializeField] EnemyState previousState; //Estado previo del enemigo
        [SerializeField] Transform player; //Jugador
        [SerializeField] float rayDistance; //Distancia del raycast
        [SerializeField] bool isLookingAtPlayer; //Si el enemigo esta viendo al jugador
        [SerializeField] LayerMask playerMask; //Layer que interactuara con el ray

        NavMeshAgent navMeshAgent;
        New_Waypoint currentWaypoint;
        New_Waypoint previousWaypoint;

        Ray detectionRay;
        RaycastHit hit;

        private bool isTraveling;  //Si esta viajando
        private bool isWaiting; //Si esta esperando
        private float waitTimer; //Se almacena el tiempo a comparar para la espera
        private int waypointsVisited; // Cuantos waypoints hemos visitado
        private float playerDistance; //Distanca al jugador
        private bool isPreparing; //Si esta en espera de cambiar de estado

        public void Start()
        {
            currentState = EnemyState.Patrol; //En un inicio el enemigo esta en el estado de patrulla
            navMeshAgent = this.GetComponent<NavMeshAgent>();  //Se asigna el componente "NavMeshAgent" a la variable

            if (navMeshAgent == null) //Revisa en caso que no tenga el componente
            {
                Debug.LogError("The nav mesh agent component is not attached to " + gameObject.name);
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
            playerDistance = Vector3.Distance(this.transform.position, player.transform.position); //Establece la distancia al jugador EN.TODO.MOMENTO.
            detectionRay = new Ray(this.transform.position, this.transform.forward); //Se crea el Ray
            Debug.DrawRay(detectionRay.origin, detectionRay.direction * rayDistance, Color.red); //Se dibuja el ray

            if(Physics.Raycast(detectionRay, out hit, rayDistance, playerMask) && currentState == EnemyState.Patrol) //Si detecta al jugador y no esta en estado de chase
            {
                navMeshAgent.isStopped = true;

                transform.LookAt(player);
                isLookingAtPlayer = true;
                Debug.LogWarning("Player is in VISION RANGE");

                currentState = EnemyState.Initializing;
                previousState = EnemyState.Patrol;
                isPreparing = true;
            }
            else if(Physics.Raycast(detectionRay, out hit, rayDistance, playerMask) && currentState == EnemyState.Chase)
            {
                transform.LookAt(player);
                isLookingAtPlayer = true;
                Debug.LogWarning("Player is being chased");
            }
            else
            {
                currentState = EnemyState.Patrol;
                previousState = EnemyState.Patrol;
            }

            if (currentState == EnemyState.Initializing && isPreparing == true)
            {
                StartCoroutine(ChaseTransition());
                isPreparing = false;
            }
            else if(currentState == EnemyState.Patrol) //Revisa si el estado del enemigo es Patrullaje
            {
                if (isTraveling && navMeshAgent.remainingDistance <= 1.0f) //Revisa si esta cerca del destino
                {
                    isTraveling = false;
                    waypointsVisited++;

                    if (patrolWaiting) //Si se tiene que esperar, espera
                    {
                        isWaiting = true;
                        waitTimer = 0f;
                    }
                    else
                    {
                        SetDestination(); //Obvio
                    }
                }

                if (isWaiting) //Si se esta esperando
                {
                    waitTimer += Time.deltaTime;
                    if (waitTimer >= totalWaitTime)
                    {
                        isWaiting = false;
                        patrolWaiting = false;
                        SetDestination(); //Obvio
                    }
                }
            }
            else if(currentState == EnemyState.Chase)
            {
                if(isLookingAtPlayer)
                {
                    previousState = EnemyState.Chase;

                    if(navMeshAgent.remainingDistance <= 0.8f)
                    {
                        currentState = EnemyState.Initializing;
                        isPreparing = true;
                        Debug.LogWarning("Ataque");
                    }
                }
                else
                {
                    currentState = EnemyState.Initializing;
                    isPreparing = true;
                    Debug.LogWarning("El jugador escapo");
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

            float rand = UnityEngine.Random.Range(0.0f, 1.0f);
            if (rand <= switchProb)
            {
                patrolWaiting = true;
            }
        }

        private void SetChaseUp()
        {
            navMeshAgent.SetDestination(player.position);
        }

        IEnumerator ChaseTransition()
        {
            navMeshAgent.isStopped = true;
            yield return new WaitForSeconds(2.5f);

            if(previousState == EnemyState.Patrol && currentState == EnemyState.Initializing)
            {
                currentState = EnemyState.Chase;
                previousState = EnemyState.Initializing;
                navMeshAgent.isStopped = false;
                SetChaseUp();
            }
            else if(previousState == EnemyState.Chase && currentState == EnemyState.Initializing)
            {
                currentState = EnemyState.Patrol;
                previousState = EnemyState.Initializing;
                navMeshAgent.isStopped = false;
                SetDestination();
            }
        }
    }
}


