using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disposer : MonoBehaviour
{
    //[SerializeField] Animator animator;

    [HideInInspector] public GameObject disposerHeldCell;

    public GameObject dropPoint;

    private bool disposing;
    private float disposeTime = 2f;

    // Start is called before the first frame update
    void Start()
    {
        PlayerActions.Instance.OnInteractPerformed += Instance_OnInteractPerformed;
    }

    private void Instance_OnInteractPerformed(object sender, System.EventArgs e)
    {
        if (!disposing && //is the disposer not busy disposing?
            PlayerActions.Instance.heldCell != null &&
            disposerHeldCell == null &&
            Vector3.Distance(this.transform.position, PlayerActions.Instance.heldCell.transform.position) < 1f)
        {
            disposerHeldCell = PlayerActions.Instance.heldCell;
            disposerHeldCell.transform.parent = dropPoint.transform;
            disposerHeldCell.transform.localPosition = Vector3.zero;
            PlayerActions.Instance.heldCell = null;
            disposing = true; //We enter the disposing state instead of destroying the cell immediately
            //animator.SetBool("disposeCell", true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (disposing)
        {
            disposeTime -= Time.deltaTime; //this is a countdown from 2 seconds to (eventually) 0
            if (disposeTime < 0f) //at the end of the countdown we reset the animation (to be added later) and reset the disposing state and timer
            {
                disposing = false;
                //animator.SetBool("disposeCell", false);
                disposeTime = 2f;
            }
            else if (disposeTime < 1f) //halfway through the countdown (1s) we destroy the cell and free the reference to it
            {
                Destroy(disposerHeldCell);
                disposerHeldCell = null;

            }
        }
    }
}
