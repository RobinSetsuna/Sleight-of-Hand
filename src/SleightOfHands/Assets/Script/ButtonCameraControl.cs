using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCameraControl : MonoBehaviour {

    public GameObject player;
    public GameObject playerCamera;

    public float rotationDegree = 90.0f;
    public float cameraSensi = 3.0f;

    private Vector3 playerPos;
    private Vector3 playerRota;
    private Vector3 cameraPos;
    private Vector3 cameraRota;

    public void CameraLeft() {
        playerCamera.transform.Translate(-Vector3.right * Time.deltaTime * cameraSensi);
    }

    public void CameraRight() {
        playerCamera.transform.Translate(Vector3.right * Time.deltaTime * cameraSensi);
    }

    public void CameraUp() {
        playerCamera.transform.Translate(Vector3.up * Time.deltaTime * cameraSensi);
    }

    public void CameraDown() {
        playerCamera.transform.Translate(-Vector3.up * Time.deltaTime * cameraSensi);
    }
   
    public void CameraCW() {
        playerCamera.transform.Rotate(Vector3.up * rotationDegree);
    }
    public void CameraCCW() {
        playerCamera.transform.Rotate(-Vector3.up * rotationDegree);
    }
    public void CameraReset() {
        playerPos = player.transform.position;
        playerRota = player.transform.eulerAngles;
        playerCamera.transform.eulerAngles = playerRota;
        playerCamera.transform.position = playerPos;
    }



}
