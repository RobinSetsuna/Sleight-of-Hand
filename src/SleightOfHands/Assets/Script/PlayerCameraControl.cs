using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraControl : MonoBehaviour
{

    public GameObject player;
    public GameObject playerCamera;
    //public Vector3 initialPosition;
    //public Vector3 initialRotation;
    public float rotationDegree = 90.0f;
    public float cameraSensi = 3.0f;

    private Vector3 playerPos;
    private Vector3 playerRota;
    private Vector3 cameraPos;
    private Vector3 cameraRota;
    // Use this for initialization
    void Start()
    {
        
        playerPos = player.transform.position;
        playerRota = player.transform.eulerAngles;

        // initialize the position and the rotation of the player camera
        cameraPos = new Vector3(playerPos.x, playerPos.y, playerPos.z);
        cameraRota = new Vector3(playerRota.x, playerRota.y, playerRota.z);
        // initialPosition = new Vector3(0, 0, 0);
        // initialRotation = new Vector3(0, 0, 0);
        playerCamera.transform.position = cameraPos;
        playerCamera.transform.eulerAngles = cameraRota;
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = player.transform.position;
        playerRota = player.transform.eulerAngles;

        // the horizontal movement of the camera
        if (Input.GetKey(KeyCode.A)) {
            playerCamera.transform.Translate(-Vector3.right * Time.deltaTime * cameraSensi);
        }
        if (Input.GetKey(KeyCode.D))
        {
            playerCamera.transform.Translate(Vector3.right * Time.deltaTime * cameraSensi);
        }

        // the vertical movement of the camera
        if (Input.GetKey(KeyCode.W))
        {
            playerCamera.transform.Translate(Vector3.up * Time.deltaTime * cameraSensi);
        }
        if (Input.GetKey(KeyCode.S))
        {
            playerCamera.transform.Translate(-Vector3.up * Time.deltaTime * cameraSensi);
        }

        // the camera rotation by certain degrees 
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            playerCamera.transform.Rotate(Vector3.up * rotationDegree);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            playerCamera.transform.Rotate(-Vector3.up * rotationDegree);
        }

        // reset the player camera according to the position of the player
        if (Input.GetKeyDown(KeyCode.R))
        {
            playerCamera.transform.eulerAngles = playerRota;
            playerCamera.transform.position = playerPos;
        }
    }
}
