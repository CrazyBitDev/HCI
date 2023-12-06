using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{

    [SerializeField] private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = PlayerActions.Instance.transform.position + offset;

        this.transform.LookAt(PlayerActions.Instance.transform);
    }
}
