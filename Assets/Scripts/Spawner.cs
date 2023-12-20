using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public enum SpawnerState
    {
        Waiting,
        Spawning,
        Spawned
    }

    private float spawnTimer;

    SpawnerState spawnerState;

    [SerializeField] private GameObject[] cellPrefabs;
    [SerializeField] private GameObject spawnPoint;
    [HideInInspector] public GameObject spawnedCell; //Reference to the cell we are spawning

    public bool isSpawned;

    [SerializeField] private float pickupDistance = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        PlayerActions.Instance.OnInteractPerformed += Instance_OnInteractPerformed;
        spawnTimer = UnityEngine.Random.Range(2f, 10f);
    }

    private void Instance_OnInteractPerformed(object sender, System.EventArgs e)
    {
        //Stuff we need to do to hand over the energy cell to the player
        if (CanPickup(spawnerState, PlayerActions.Instance.transform.position, pickupDistance, PlayerActions.Instance.heldCell)) //player isn't holding any cell
        {
            //animator.SetBool("isSpawned", false);
            spawnedCell.transform.parent = PlayerActions.Instance.holdPoint.transform;
            spawnedCell.transform.localPosition = Vector3.zero;
            PlayerActions.Instance.heldCell = spawnedCell;
            spawnedCell = null;
            spawnTimer = UnityEngine.Random.Range(2f, 10f);
            spawnerState = SpawnerState.Waiting;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnerState == SpawnerState.Waiting) //What to do in Waiting state
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer < 0) //Countdown is over
            {
                spawnerState = SpawnerState.Spawning;
                spawnTimer = 1; //The time we want to give the spawner to perform the spawning
                if (spawnedCell == null)
                {
                    spawnedCell = Instantiate(cellPrefabs[UnityEngine.Random.Range(0, 4)]);
                    //We position energy cell on the spawn point
                    spawnedCell.transform.parent = spawnPoint.transform;
                    spawnedCell.transform.localPosition = Vector3.zero;
                    //animator.SetBool("isSpawned", true);

                }
            }
        }
        else if (spawnerState == SpawnerState.Spawning)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer < 0)
            {
                isSpawned = true;
                spawnerState = SpawnerState.Spawned;
            }
        }
    }

    private void OnDisable()
    {
        PlayerActions.Instance.OnInteractPerformed -= Instance_OnInteractPerformed;
    }

    private bool CanPickup(SpawnerState spawnerState, Vector3 playerPosition, float distThresh, GameObject targetGO)
    {
        if (spawnerState == SpawnerState.Spawned &&
            Vector3.Distance(this.transform.position, playerPosition) < distThresh &&
            targetGO == null)
        {
            return true;
        }
        return false;
    }
}
