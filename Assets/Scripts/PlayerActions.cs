using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{

    public static PlayerActions Instance { get; private set; }

    private PlayerInput playerActionSystem;
    [SerializeField] private float movSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private Animator playerAnimator;

    //separator on the inspector
    [Space(10)]

    [SerializeField] private float playerHeight;
    [SerializeField] private float playerRadius;
    [SerializeField] private float movDist;

    private void Awake()
    {
        Instance = this;
        playerActionSystem = new PlayerInput();
        playerActionSystem.PlayerActions.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 readVec = playerActionSystem.PlayerActions.Movement.ReadValue<Vector2>();
        Vector3 movVec = new Vector3(readVec.x, 0, readVec.y);
        
        bool canMove = ShootCapsule(movVec);

        if (!canMove)
        {
            Vector3 movVecX = new Vector3(readVec.x, 0, 0).normalized;

            canMove = ShootCapsule(movVecX);
            if (canMove)
            {
                movVec = movVecX;
            } else
            {
                Vector3 movVecZ = new Vector3(0, 0, readVec.y).normalized;

                canMove = ShootCapsule(movVecZ);
                if (canMove)
                {
                    movVec = movVecZ;
                }
            }
        }

        if (canMove)
        {
            this.transform.position += movVec * movSpeed * Time.deltaTime;
        }

        this.transform.forward = Vector3.Slerp(this.transform.forward, movVec, Time.deltaTime * turnSpeed);

        playerAnimator.SetBool("isWalking", movVec.magnitude != 0);
    }

    private bool ShootCapsule(Vector3 toVec)
    {
        return !Physics.CapsuleCast(this.transform.position, this.transform.position + Vector3.up * playerHeight, playerRadius, toVec, movDist);
    }

}
