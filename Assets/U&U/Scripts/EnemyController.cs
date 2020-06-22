using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float AIMoveSpeed;
    [SerializeField] float damping = 6.0f;

    NavMeshAgent agent;
    public Transform[] navPoint;
    public LayerMask playerMask;
    public Transform enem;
    public Transform playerToFollow;
    public float playerDistance;
    public int destPoint = 0;
    public bool playerOnSight;
    public float distance = 100.0f;
    //public int numOfSights;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enem = GetComponent<Transform>();
        agent.autoBraking = false;
        //numOfSights=0;
    }

    // Update is called once per frame
    void Update()
    {

        RaycastHit hit;
        Ray playerDetection = new Ray(enem.position,enem.forward);
        
        playerDistance = Vector3.Distance(player.position, transform.position);

        Debug.DrawRay(playerDetection.origin, playerDetection.direction * distance, Color.blue);

        if(Physics.Raycast(playerDetection, out hit, distance, playerMask) )
        {
            LookAtPlayer();
            Debug.Log("Its hitting the player");
          
            playerOnSight = true;
        }

        if(playerOnSight)
        {
           
            if(playerDistance < 20f)
            {
                Chase();
                agent.destination = playerToFollow.position;
            
            }
            else if(playerDistance > 70f)
            {
                playerOnSight = false;
                GotoNextPoint();
            }
        }

        if(agent.remainingDistance < 0.5f)
        {
            GotoNextPoint();
        }
    }

    void LookAtPlayer()
    {
        transform.LookAt(player);
      
    }

    void Chase()
    {
        transform.Translate(Vector3.forward * AIMoveSpeed * Time.deltaTime);
    }

    void GotoNextPoint()
    {
        if (navPoint.Length == 0)
            return;
        agent.destination = navPoint[destPoint].position;
        destPoint = (destPoint + 1) % navPoint.Length;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        numOfSights += 1;
    //    }
    //}
}
