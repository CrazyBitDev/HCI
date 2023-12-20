using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{

    [SerializeField] private float pickUpDistance = 1.5f;
    [SerializeField] private float dropOffDistance = 1.5f;
    [SerializeField] private float swapDistance = 1.5f;
    [HideInInspector] public GameObject tableHeldCell;

    public GameObject dropPoint;


    // Start is called before the first frame update
    void Start()
    {
        PlayerActions.Instance.OnInteractPerformed += Instance_OnInteractPerformed;
    }

    private void Instance_OnInteractPerformed(object sender, System.EventArgs e)
    {
        //Player wants to drop off the Energy Cell
        if (PlayerActions.Instance.heldCell != null && //is player holding a cell?
            tableHeldCell == null && //is the table free?
            Vector3.Distance(this.transform.position, PlayerActions.Instance.heldCell.transform.position) < dropOffDistance)
        {
            tableHeldCell = PlayerActions.Instance.heldCell; //cell reference: Player -> Table
            tableHeldCell.transform.parent = dropPoint.transform; //cell parent: Player -> Table
            tableHeldCell.transform.localPosition = Vector3.zero;
            PlayerActions.Instance.heldCell = null; //clearing reference on Player
        }
        else if (PlayerActions.Instance.heldCell == null && //is the player not holding any cell?
                 tableHeldCell != null && //is the table holding a cell?
                 Vector3.Distance(this.transform.position, PlayerActions.Instance.transform.position) < pickUpDistance) //Player wants to pick up the Energy Cell
        {
            PlayerActions.Instance.heldCell = tableHeldCell;//cell reference: Table -> Player
            PlayerActions.Instance.heldCell.transform.parent = PlayerActions.Instance.holdPoint.transform;//cell parent: Player->Table
            PlayerActions.Instance.heldCell.transform.localPosition = Vector3.zero;
            tableHeldCell = null; //clearing reference on Table
        }
        else if (PlayerActions.Instance.heldCell != null && //if player is holding a cell
            tableHeldCell != null && //and the table is holding a cell
            Vector3.Distance(this.transform.position, PlayerActions.Instance.transform.position) < swapDistance) //and the player is close enough to swap
        {
            GameObject temp = PlayerActions.Instance.heldCell; //create a temp variable to hold the player's cell, assign the player's cell to the variable
            PlayerActions.Instance.heldCell = tableHeldCell; //assign the table's cell to the player's cell
            tableHeldCell = temp; //assign the temp variable to the table's cell
            tableHeldCell.transform.parent = dropPoint.transform; //set the table's cell's parent to the table's drop point
            tableHeldCell.transform.localPosition = Vector3.zero; //set the table's cell's local position to zero
            PlayerActions.Instance.heldCell.transform.parent = PlayerActions.Instance.holdPoint.transform; //set the player's cell's parent to the player's hold point
            PlayerActions.Instance.heldCell.transform.localPosition = Vector3.zero; //set the player's cell's local position to zero
        }
        // else if 
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDisable()
    {
        PlayerActions.Instance.OnInteractPerformed -= Instance_OnInteractPerformed;
    }
}
