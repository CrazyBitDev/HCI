using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerActions : MonoBehaviour
{
    public static PlayerActions Instance { get; private set; }

    private PlayerInput playerActionSystem;
    [SerializeField] private float movSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float playerHeight;
    [SerializeField] private float playerRadius;
    [SerializeField] private float movDist;

    public event EventHandler OnInteractPerformed;
    public event EventHandler OnPausePerformed;

    public GameObject holdPoint;
    [HideInInspector] public GameObject heldCell;

    private void Awake()
    {
        Instance = this;
        playerActionSystem = new PlayerInput();
        playerActionSystem.PlayerActions.Enable();
        playerActionSystem.PlayerActions.Interact.performed += Interact_performed;
        playerActionSystem.PlayerActions.Pause.performed += Pause_performed;

    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPausePerformed?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractPerformed?.Invoke(this, EventArgs.Empty);
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

        //getting the orientation of the camera
        Quaternion cameraRot = Camera.main.transform.rotation;

        float cameraAngle;
        Vector3 cameraAxis;

        //getting the angle represnetation of cameraRot (quaternion -> euler angles)
        cameraRot.ToAngleAxis(out cameraAngle, out cameraAxis);

        //using the euler angles version to obtain a "flattened" version of the orientation
        Quaternion cameraRotFlat = Quaternion.Euler(0, cameraAngle * cameraAxis.y, 0);

        //remapping our vector with movement given by WASD keys to its rotated version (rotated by cameraROtFlat)
        //in other words: the movVec is no longer aligned with X and Z axis of the world
        //                the movVec is now aligned with the direction the camera is looking at
        movVec = cameraRotFlat * movVec;


        //float movDist = movSpeed * Time.deltaTime;

        bool canMove = ShootCapsule(movVec); //Check for collisions along the movement direction


        if (!canMove) //something is blocking the way so let's check along X direction
        {
            Vector3 movVecX = new Vector3(movVec.x, 0, 0).normalized; //get just the X of movVec
            canMove = ShootCapsule(movVecX); //check along movVecX
            if (canMove)
            {
                movVec = movVecX; //the new movVec is now the movVec on X axis
            }

        }


        //Applying movement
        if (canMove)
        {
            this.transform.position += movVec * movSpeed * Time.deltaTime;
        }

        this.transform.forward = Vector3.Slerp(this.transform.forward, movVec, Time.deltaTime * turnSpeed);



        //Animation
        if (movVec.magnitude == 0)
        {
            playerAnimator.SetBool("isWalking", false);
        }
        else
        {
            playerAnimator.SetBool("isWalking", true);
        }



    }

    private bool ShootCapsule(Vector3 toVec)
    {
        return !Physics.CapsuleCast(this.transform.position,
                                            this.transform.position + Vector3.up * playerHeight,
                                            playerRadius,
                                            toVec,
                                            movDist
                                            );

    }

    private void OnDisable()
    {
        playerActionSystem.PlayerActions.Interact.performed -= Interact_performed;
        playerActionSystem.PlayerActions.Pause.performed -= Pause_performed;

    }
}
