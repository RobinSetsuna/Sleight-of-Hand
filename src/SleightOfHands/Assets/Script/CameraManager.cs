using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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

	public bool bounds; // check if the camera hit the boundary
	public Vector3 minCameraPos;
	public Vector3 maxCanmeraPos;
	[SerializeField]private float rotate_speed;
	[SerializeField]private float CameraDistance; // the distance between camera and target, include fallow, focus

	
	public bool fallowing;
	private Vector3 velocity;
	private bool shaking;
	private float shake_magnitude;

	private Vector3 defaultPosition;
	private Quaternion defaultRotation;
	
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



	public void setDefaultPos()
	{
		// set the default pos for reset function
		// make a function call when Initiate
		defaultPosition = transform.position;
		defaultRotation = transform.rotation;
	}

	public void ResetPos(System.Action callback)
	{
		// reset the position back to default
		fallowing = false;
		destination = defaultPosition;
		transform.rotation = defaultRotation;
		StartCoroutine(Moveto(1,callback));
		//StartCoroutine(Rotateback(callback));
	}

	public bool isBoundedForFallow()
	{
		// check the Camera is fallowing
		return fallowing;
	}

	public void boundCameraFallow(Transform unit)
	{
		// bound Camera fallow to Transform, Camera will fallow the target until it release
		//FocusAt(unit.transform.position, null);
		fallowing = true;
		StartCoroutine(CameraFallow(unit,true,null));
	}
	
	public void unboundCameraFallow()
	{
		// release Camera
		fallowing = false;
	}

	public void FocusAt(Vector3 destination,System.Action callback)
	{
		// Make a one time focus 
		// may add a camera action queue for camera action
		unboundCameraFallow();
		this.destination = destination;
		StartCoroutine(Focus(callback));
		StartCoroutine(Moveto(CameraDistance,callback));
	}
		
	public void Shaking(float Duration,float Magnitude ,System.Action callback)
	{
		// may add a camera action queue for camera action
		// shaking the screen for effect, like earthquake, explosion
		StartCoroutine(Shake(Duration,Magnitude,callback));
	}
	
	private IEnumerator CameraFallow(Transform unit,bool chasing,System.Action callback)
	{
		Vector3 temp = unit.position;
		while (fallowing)
		{
				// smooth camera movement
				float posy = transform.position.z+(unit.position.z - temp.z);
				float posx = transform.position.x+(unit.position.x - temp.x);
				transform.position = new Vector3(posx, transform.position.y, posy);
				if (bounds)
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
		
		if (callback != null)
			callback.Invoke();
		// trigger callback event if have one
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


	private IEnumerator Shake(float Duration,float Magnitude,System.Action callback)
	{
		var start_time = Time.fixedUnscaledTime;
		var temp = transform.position;
		while (Time.fixedUnscaledTime - start_time < Duration)
		{
			float x = Random.Range(-1f, 1f) * Magnitude;
			float y = Random.Range(-1f, 1f) * Magnitude;
			// shaking
			transform.position = new Vector3(transform.position.x+x,transform.position.y,transform.position.z+y);
			if (bounds)
			{
				//bounds shaking
				transform.position = new Vector3(Mathf.Clamp(transform.position.x, minCameraPos.x, maxCanmeraPos.x)+x,
					transform.position.y,Mathf.Clamp(transform.position.z, minCameraPos.z, maxCanmeraPos.z)+y
				);
			}
			yield return null;
		}

		transform.position = temp;
		if (callback != null)
			callback.Invoke();
		yield return null;
	}
	
	private IEnumerator Rotateback(System.Action callback)
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

		if (callback != null)
			callback.Invoke();
		
		yield return null;
	}
	
	private IEnumerator Moveto(float Distance,System.Action callback)
	{
		while(MathUtility.ManhattanDistance(destination.x, destination.z, transform.position.x, transform.position.z) > Distance){
			    // smooth movement
				float x = 0;
				float y = 0;	
				float posy = Mathf.SmoothDamp(transform.position.z, destination.z, ref velocity.z, smoothTimeY);
				float posx = Mathf.SmoothDamp(transform.position.x, destination.x, ref velocity.x, smoothTimeX);

				transform.position = new Vector3(posx + x, transform.position.y, posy + y);
				if (bounds)
				{
					transform.position = new Vector3(
						Mathf.Clamp(transform.position.x, minCameraPos.x, maxCanmeraPos.x) + x,
						transform.position.y, Mathf.Clamp(transform.position.z, minCameraPos.z, maxCanmeraPos.z) + y
					);
				}
			yield return null;
		}


		if (callback != null)
			callback.Invoke();
		yield return null;
	}
	
	private IEnumerator Focus(System.Action callback)
	{
		Vector3 dirFromAtoB = (destination - transform.position).normalized;
		var dotProd = Vector3.Dot(dirFromAtoB, transform.forward);
		while (dotProd > 0.1)
		{
			// sharp pointing, also provide a little reverse rotation
			// smooth rotation
			var rotation = Quaternion.LookRotation(destination - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotate_speed);
			dirFromAtoB = (destination - transform.position).normalized;
			dotProd = Vector3.Dot(dirFromAtoB, transform.forward);
			yield return null;
		}

		if (callback != null)
			callback.Invoke();
		
		yield return null;
	}
 
}