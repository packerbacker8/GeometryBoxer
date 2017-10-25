using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 


public class WorldInteraction : MonoBehaviour
{
    private NavMeshAgent playerAgent; 

    public GameObject activePlayer;

    private Animator animator;

    private int speedId;
    private int rotateId;

    public Vector3 moveTarget;

    public bool isInteractable;
    public bool pathReached;
    public bool canMove;

    public Quaternion rot; 

    public GameObject currentInteractable;

    void OnDrawGizmosSelected()
    {

        var nav = GetComponent<NavMeshAgent>();
        if (nav == null || nav.path == null)
            return;

        var line = this.GetComponent<LineRenderer>();
        if (line == null)
        {
            line = this.gameObject.AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Sprites/Default")) { color = Color.yellow };
            line.SetWidth(0.5f, 0.5f);
            line.SetColors(Color.yellow, Color.yellow);
        }

        var path = nav.path;

        line.SetVertexCount(path.corners.Length);

        for (int i = 0; i < path.corners.Length; i++)
        {
            line.SetPosition(i, path.corners[i]);
        }

    }

    public enum MoveFSM
    {
        findPosition,
        move,
        turnToFace,
        interact
    }

    public MoveFSM moveFSM;
    
    public enum TurnFSM
    {
        Empty,
        TriggerTurn,
        WaitForTurnEnd
    }

    public TurnFSM turnFSM; 


    private void Start()
    {
        animator = this.GetComponent<Animator>(); 

        speedId = Animator.StringToHash("Speed");
        rotateId = Animator.StringToHash("Angle");

        playerAgent = this.GetComponent<NavMeshAgent>(); 
        canMove = true; 
        pathReached = false; 
        activePlayer = this.gameObject; 
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            GetInteraction();
        }

        MoveStates();
    }


    public void MoveStates()
    {
        switch(moveFSM)
        {
            case MoveFSM.findPosition:

                break;
            case MoveFSM.move:
                Move();
                break; 
            case MoveFSM.turnToFace:
                TurnToFace();
                break;
            case MoveFSM.interact:
               // if(currentInteractable != null)
                    //currentInteractable.GetComponent<Interactable>().Interact(this.gameObject);
                break; 
        }
    }

    public void Move()
    {
         if(!playerAgent.pathPending)
        {
            if (playerAgent.remainingDistance <= playerAgent.stoppingDistance)
            {
                animator.SetFloat(speedId, 0f);

                pathReached = true;

                moveFSM = MoveFSM.turnToFace;
            }
        }

    }

    public void TurnToFace()
    {
        if (currentInteractable != null)
        {
            if (pathReached == true)
            {
                Vector3 dir = currentInteractable.transform.position - transform.position;
                dir.y = 0;
                rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, 5f * Time.deltaTime);

                if ((rot.eulerAngles - transform.rotation.eulerAngles).sqrMagnitude < .01)
                {
                    pathReached = false;
                    animator.SetBool("Turning", false);
                    turnFSM = TurnFSM.Empty;
                    moveFSM = MoveFSM.interact;
                }
            }
        }
        else if (currentInteractable == null)
        {
            moveFSM = MoveFSM.findPosition;
        }
    }

    private void GetInteraction()
    {
        if(canMove)
        {
            Ray interactionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit interactionInfo;
            if (Physics.Raycast(interactionRay, out interactionInfo, Mathf.Infinity))
            {
                    
                if (interactionInfo.collider.tag == "Interactable")
                {
                    currentInteractable = interactionInfo.collider.gameObject;
                    isInteractable = true;
                    currentInteractable = interactionInfo.collider.gameObject;

                    playerAgent.destination = 
                        currentInteractable.GetComponent<Interactable>().interactionPoint.transform.position;
                    //currentInteractable.GetComponent<Interactable>().isClicked = true;

                    moveTarget = playerAgent.destination; 

                    animator.SetFloat(speedId, 3f);            

                    pathReached = false;
                    moveFSM = MoveFSM.move;

                }
                else
                {
                    if (currentInteractable != null)
                    {
                        //currentInteractable.GetComponent<Interactable>().isClicked = false;
                        currentInteractable = null;
                    }
                    isInteractable = false;

                    moveTarget = interactionInfo.point;
                    playerAgent.destination = interactionInfo.point;


                    animator.SetFloat(speedId, 3f);

                    pathReached = false;
                    moveFSM = MoveFSM.move;
                }
            }
        }
    }
}