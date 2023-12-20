using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CameraColtroller : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    //[SerializeField] private GameObject player;
    [SerializeField] private CinemachineDollyCart cameraDolly;
    [SerializeField] private float baseFOV;
    [SerializeField] private float FOVMulti;

    private float lengthScale; //scaling factor for the length of the track
    private float radius;

    // Start is called before the first frame update
    void Start()
    {
        lengthScale = cameraDolly.m_Path.PathLength;
        radius = new Vector3(virtualCamera.transform.position.x, 0, virtualCamera.transform.position.z).magnitude;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //flattened position of the player
        Vector2 player2DPos = new Vector2(PlayerActions.Instance.transform.position.x, PlayerActions.Instance.transform.position.z);

        //distance of the player from the center (reactor pos)
        float player2DPosMag = player2DPos.magnitude;

        //scaling the position of the player in a [0,1] range
        player2DPos = new Vector2(player2DPos.x / player2DPosMag, player2DPos.y / player2DPosMag);

        //getting the angle between the position of the player and some arbitrary vectory.
        float posLength = Vector2.SignedAngle(player2DPos, new Vector2(1, 1));

        //scaling the FOV with distance of the player from the camera
        float FOV = baseFOV + (PlayerActions.Instance.transform.position.magnitude * FOVMulti);

        //dealing with possibly negative signed angles
        if (posLength > 0) SetCameraPos(posLength / 360, FOV);
        else SetCameraPos((posLength + 360) / 360, FOV);
    }

    private void SetCameraPos(float pos, float scaledFOV)
    {
        //applying the FOV scale to the FOV field in the virtual camera
        virtualCamera.m_Lens.FieldOfView = scaledFOV;

        //applying the position in the camera dolly scriptW
        cameraDolly.m_Position = pos * lengthScale; //transform the pos from [0,1] range to [0,lengthScale]
    }
}
