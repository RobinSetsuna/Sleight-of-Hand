﻿using UnityEngine;

public class CameraDragging : MonoBehaviour
{
	private Vector3 dragOrigin;
	[SerializeField] private float dragSpeed;

    [SerializeField] private float curZoomPos = 10;
    [SerializeField] private float zoomTo = 15; // curZoomPos will be the value
	[SerializeField] private float zoomFrom = 3;
    [SerializeField] private float zoomSpeed = 10;

    public bool isZoomEnabled = true;

    private void Start()
	{
		// demo test, delete when actual use
		CameraManager.Instance.setDefaultPos();

        MouseInputManager.Singleton.onMouseDrag.AddListener(HandleMouseDrag);
	}

    void Update()
    {
        #region ZOOM
        // If the wheel goes up it, decrement 5 from "zoomTo"
        if (isZoomEnabled)
        {
            curZoomPos = Camera.main.orthographicSize;
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + zoomSpeed * Time.deltaTime, zoomFrom, zoomTo);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - zoomSpeed * Time.deltaTime, zoomFrom, zoomTo);
            }
        }
        #endregion

        #region  DEMO_CODE
        // demo test, delete when actual use
        if (Input.GetKey(KeyCode.A))
        {
            if (!CameraManager.Instance.IsBoundedForFallow())
            {
                CameraManager.Instance.BoundCameraFollow(GameObject.FindGameObjectWithTag("Player").transform);
            }

        }
        if (Input.GetKey(KeyCode.S))
        {
            CameraManager.Instance.FocusAt(GameObject.FindGameObjectWithTag("Player").transform.position);
        }


        if (Input.GetKey(KeyCode.D))
        {
            CameraManager.Instance.Shaking(0.3f, 0.1f);
        }

        if (Input.GetKey(KeyCode.F))
        {
            CameraManager.Instance.ResetPos();
        }
        #endregion

        //#region DRAGGING_FUNCTION IMPLEMENTATION
        //// mouse left button to move the camera
        //if (Input.GetMouseButtonDown(0))
        //{
        //    dragOrigin = Input.mousePosition;
        //    return;
        //}
        //if (!Input.GetMouseButton(0)) return;
        //var currentClicked = MouseInputManager.Singleton.CurrentMouseClicked; // check if  the tag is in the white list
        //if (currentClicked == null || !CameraManager.Instance.isContainedInWhiteList(currentClicked))
        //{
        //    Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        //    Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);
        //    transform.Translate(move, Space.World);
        //    if (CameraManager.Instance.bounds)
        //    {
        //        // if there is a bounds for camera movement
        //        transform.position = new Vector3(
        //            Mathf.Clamp(transform.position.x, CameraManager.Instance.minCameraPos.x, CameraManager.Instance.maxCanmeraPos.x),
        //            transform.position.y, Mathf.Clamp(transform.position.z, CameraManager.Instance.minCameraPos.z, CameraManager.Instance.maxCanmeraPos.z)
        //        );
        //    }

        //}
        //#endregion
    }

    private void HandleMouseDrag(MouseInteractable obj)
    {
        if (LevelManager.Instance.playerController.CurrentPlayerState == PlayerState.Idle)
        {
            Vector3 d = Input.mousePosition - MouseInputManager.Singleton.MouseDownPosition;

            if (d.magnitude > 0)
            {
                Vector3 mouseMovement = Camera.main.ScreenToViewportPoint(d);
                Vector3 move = new Vector3(mouseMovement.x * dragSpeed * Time.deltaTime, 0, mouseMovement.y * dragSpeed * Time.deltaTime);

                transform.Translate(-move);

                if (CameraManager.Instance.Bounds)
                {
                    // if there is a bounds for camera movement
                    transform.position = new Vector3(
                        Mathf.Clamp(transform.position.x, CameraManager.Instance.minCameraPos.x, CameraManager.Instance.maxCanmeraPos.x),
                        transform.position.y,
                        Mathf.Clamp(transform.position.z, CameraManager.Instance.minCameraPos.z, CameraManager.Instance.maxCanmeraPos.z));
                }
            }
        }
    }
}
