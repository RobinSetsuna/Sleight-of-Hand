using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class CameraManager : MonoBehaviour
{
	private HashSet<UnitType> typeWhiteList = new HashSet<UnitType>(
	new UnitType[]{
		UnitType.Default,
		UnitType.Player,
		UnitType.UI,
		UnitType.Enemy,
		UnitType.Item
	}); // hold all the mouse Interactable UnitType will not cause camera dragging.
	private Vector3 destination;
	[SerializeField]private float smoothTimeY;
	[SerializeField]private float smoothTimeX;

	public bool Bounds; // check if the camera hit the boundary
	public Vector3 minCameraPos;
	public Vector3 maxCanmeraPos;
	[SerializeField]private float rotate_speed;
	[SerializeField]private float CameraDistance; // the distance between camera and target, include fallow, focus

    [SerializeField] private float smoothTime = 2f;

	
	private bool following;
	private Vector3 velocity;
	private bool shaking;
	private float shakeMagnitude;

	private Vector3 defaultPosition;
	private Quaternion defaultRotation;
	private Queue<Vector3> focusQueue;
    private Dictionary<Vector3, System.Action> focusCallbacks = new Dictionary<Vector3, System.Action>();
	private bool allocated;

	private static CameraManager instance;
	public static CameraManager Instance
	{
		get
		{
			if (instance == null) instance = GameObject.Find("CameraManager").GetComponent<CameraManager>();
			return instance;
		}
	}

	public bool isContainedInWhiteList(MouseInteractable obj)
	{
		return typeWhiteList.Contains(obj.Type);
	}

    public void EnableZoom()
    {
        GetComponent<CameraDragging>().isZoomEnabled = true;
    }

    public void DisableZoom()
    {
        GetComponent<CameraDragging>().isZoomEnabled = false;
    }

    public void CameraZoomIn()
	{
		StartCoroutine(ZoomIn());
	}

	public void CameraZoomOut()
	{
		StartCoroutine(ZoomOut());
	}

	public void Finished()
	{
        allocated = false;

        if (focusCallbacks.ContainsKey(destination))
        {
            focusCallbacks[destination].Invoke();
            focusCallbacks.Remove(destination);
        }

		if (focusQueue.Count != 0)
		{
			destination = focusQueue.Dequeue();
			StartCoroutine(Focus());
		}
//		else
//		{
//			StartCoroutine(ZoomOut());
//		}
	}

	public void setDefaultPos()
	{
		// set the default pos for reset function
		// make a function call when Initiate
		defaultPosition = transform.position;
		defaultRotation = transform.rotation;
		focusQueue = new Queue<Vector3>();
	}

	public void ResetPos()
	{
		// reset the position back to default
		following = false;
		destination = defaultPosition;
		transform.rotation = defaultRotation;
		StartCoroutine(Moveto(1));
	}

	public bool IsBoundedForFallow()
	{
		// check the Camera is fallowing
		return following;
	}

	public void BoundCameraFollow(Transform unit)
	{
		// bound Camera fallow to Transform, Camera will fallow the target until it release
		//FocusAt(unit.transform.position, null);
		following = true;
		StartCoroutine(CameraFallow(unit,true));
	}

	public void UnboundCameraFollow()
	{
		// release Camera
		following = false;
	}

	public void FocusAt(Vector3 destination, System.Action callback = null)
	{
		// Make a one time focus
		// may add a camera action queue for camera action
		UnboundCameraFollow();

        if (callback != null)
            focusCallbacks.Add(destination, callback);

        if (allocated)
			focusQueue.Enqueue(destination);
        else
		{
			allocated = true;
			this.destination = destination;
			StartCoroutine(Focus());
		}
	}

	public void Shaking(float Duration,float Magnitude)
	{
		// may add a camera action queue for camera action
		// shaking the screen for effect, like earthquake, explosion
		StartCoroutine(Shake(Duration,Magnitude));
	}

    private void Start()
    {
        LevelManager.Instance.onGameEnd.AddListener(StopAllCoroutines);
    }

    private IEnumerator CameraFallow(Transform unit,bool chasing)
	{
		Vector3 temp = unit.position;
		while (following)
		{
				// smooth camera movement
				float posy = transform.position.z+(unit.position.z - temp.z);
				float posx = transform.position.x+(unit.position.x - temp.x);
				transform.position = new Vector3(posx, transform.position.y, posy);
				if (Bounds)
				{
					// if there is a bounds for camera movement
					transform.position = new Vector3(
						Mathf.Clamp(transform.position.x, minCameraPos.x, maxCanmeraPos.x),
						transform.position.y, Mathf.Clamp(transform.position.z, minCameraPos.z, maxCanmeraPos.z)
					);
				}
		    temp = unit.position;
			yield return null;
		}
		yield return null;
	}

//		while (fallowing)
//		{
//			// two type of chasing:
//			// if chasing is false, camera won't change the position, just keep looking at target
//			// if chasing is true, camera will change position too.
//			if (chasing)
//			{
//				destination = unit.transform.position;
//			}
//			if (MathUtility.ManhattanDistance(destination.x, destination.z, transform.position.x,
//				    transform.position.z) > CameraDistance)
//			{

//			}
//			Vector3 dirFromAtoB = (unit.position - transform.position).normalized;
//			var dotProd = Vector3.Dot(dirFromAtoB, transform.forward); // check if the camera already rotate to right angle
//			if (dotProd > 0.1)
//			{
//				// no need for sharp pointing in fallow,0.3 is good enough
//				//smooth camera rotation
//				var rotation = Quaternion.LookRotation(unit.position - transform.position);
//				print(rotation);
//				transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotate_speed);
//			}


	private IEnumerator Shake(float Duration,float Magnitude)
	{
		var start_time = Time.fixedUnscaledTime;
		var temp = transform.position;
		while (Time.fixedUnscaledTime - start_time < Duration)
		{
			float x = Random.Range(-1f, 1f) * Magnitude;
			float y = Random.Range(-1f, 1f) * Magnitude;
			// shaking
			transform.position = new Vector3(transform.position.x+x,transform.position.y,transform.position.z+y);
			if (Bounds)
			{
				//bounds shaking
				transform.position = new Vector3(Mathf.Clamp(transform.position.x, minCameraPos.x, maxCanmeraPos.x)+x,
					transform.position.y,Mathf.Clamp(transform.position.z, minCameraPos.z, maxCanmeraPos.z)+y
				);
			}
			yield return null;
		}

		transform.position = temp;
		yield return null;
	}

	private IEnumerator Rotateback()
	{
		Vector3 dirFromAtoB = (-destination - transform.position).normalized;
		var dotProd = Vector3.Dot(dirFromAtoB, transform.forward);
		while (dotProd > 0.9)
		{
			print(dotProd);
			// smooth rotation
			var desiredRotQ = defaultRotation;
			transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotQ, Time.deltaTime * rotate_speed);
			dirFromAtoB = (-destination - transform.position).normalized;
			dotProd = Vector3.Dot(dirFromAtoB, transform.forward);
			yield return null;
		}
		yield return null;
	}

	private IEnumerator Moveto(float Distance)
	{
		while(MathUtility.ManhattanDistance(destination.x, destination.z, transform.position.x, transform.position.z) > Distance){
			    // smooth movement
				float x = 0;
				float y = 0;
				float posy = Mathf.SmoothDamp(transform.position.z, destination.z, ref velocity.z, smoothTimeY);
				float posx = Mathf.SmoothDamp(transform.position.x, destination.x, ref velocity.x, smoothTimeX);

				transform.position = new Vector3(posx + x, transform.position.y, posy + y);
				if (Bounds)
				{
					transform.position = new Vector3(
						Mathf.Clamp(transform.position.x, minCameraPos.x, maxCanmeraPos.x) + x,
						transform.position.y, Mathf.Clamp(transform.position.z, minCameraPos.z, maxCanmeraPos.z) + y
					);
				}
			yield return null;
		}
		yield return null;
	}

	private IEnumerator Focus()
	{
        Vector3 cameraForward = Camera.main.transform.forward;

        Vector3 cameraPosition = transform.position;
        Vector3 tilePosition = GridManager.Instance.GetTile(destination).transform.position;
        Vector3 targetPosition = tilePosition - Mathf.Abs((cameraPosition.y - tilePosition.y) / cameraForward.y) * cameraForward;

        targetPosition.y = cameraPosition.y;

        float time = smoothTime;

        while (MathUtility.EuclideanDistance(targetPosition.x, targetPosition.z, cameraPosition.x, cameraPosition.z) > 0.1f)
        {
            transform.position = Vector3.SmoothDamp(cameraPosition, targetPosition, ref velocity, time);
            cameraPosition = transform.position;

            yield return null;
        }

        StartCoroutine(ZoomIn());

        yield break;
	}

	private IEnumerator ZoomOut()
	{
		//allocated = true;
		Camera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			//Debug.Log("I'm looking at " + hit.transform.name);
			Tile temp = GridManager.Instance.GetTile(hit.transform.position);
			Tile desTile = GridManager.Instance.GetTile(destination);
			if (desTile.x != temp.x || desTile.y != temp.y){
				while (Camera.main.orthographicSize < 8)
				{
					Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize,11,ref velocity.x, smoothTimeX/2);
					yield return null;
				}
			}
		}
		//StartCoroutine(Focus());
		//Finished();
		yield return null;
	}
	private IEnumerator ZoomIn()
	{
		//allocated = true;
		while (Camera.main.orthographicSize > 4){
					Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize,3,ref velocity.x, smoothTimeX/2);
					yield return null;
		}
		Finished();
		yield return null;
	}
}
