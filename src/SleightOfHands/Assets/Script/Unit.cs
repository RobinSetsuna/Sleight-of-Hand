using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///	<summary/>
/// Unit
/// abstract class for all walkable unit in map
/// when heading, move to heading, facing heading direction.
/// </summary>
public abstract class Unit : MonoBehaviour
{

	[SerializeField] private float movementSpeed;
	protected Transform heading;
	// Use this for initialization
	public void Awake ()
	{
	}
	
	// Update is called once per frame
	protected void FixedUpdate(){
		if ( heading!= null && heading.position != Vector3.zero)
		{			
			// heading detected
			Vector3 desiredPosition = new Vector3(heading.position.x, transform.position.y, heading.position.z);
			Facing(desiredPosition);
			transform.position = Vector3.MoveTowards(transform.position, desiredPosition, movementSpeed * Time.deltaTime);
			if ((transform.position - heading.position).sqrMagnitude < 1.05)
			{
				// arrived heading tile
				heading = null;
			}
		}
	}

    // change direction of unit facing, sync with heading
	void Facing(Vector3 pos)
	{
		transform.LookAt(pos);
	}

	public void setHeading(Tile tile)
	{
		//heading is the next tile unit will move to.
		heading = tile.transform;
	}
}
