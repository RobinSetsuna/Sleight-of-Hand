using UnityEngine;
///	<summary/>
/// Unit
/// abstract class for all walkable unit in map
/// when heading, move to heading, facing heading direction.
/// </summary>
public abstract class Unit : MonoBehaviour
{
	[SerializeField] private float maxSpeed;
    public float MaxSpeed
    {
        get
        {
            return maxSpeed;
        }
    }

    private float speed;
    private Vector3 destination;
    private System.Action moveCallback;

    private void FixedUpdate()
    {
        if (speed > 0)
        {
            Vector3 position = transform.position;

            // End move
            if (MathUtility.ManhattanDistance(destination.x, destination.z, position.x, position.z) < 0.05)
            {
                speed = 0;

                if (moveCallback != null)
                {
                    System.Action callbackToBeExecuted = moveCallback;
                    moveCallback = null;

                    callbackToBeExecuted.Invoke();
                }
            }

            Vector3 orientation = destination - position;
            orientation.y = 0;

            transform.forward = orientation.normalized;
            transform.Translate(0, 0, Mathf.Min(speed * Time.deltaTime, orientation.magnitude));
        }
    }

    public void MoveTo(Vector3 destination, System.Action callback)
    {
        Debug.LogFormat("[Unit.cs] MoveTo({0}, {1})", destination, callback);

        this.destination = destination;
        moveCallback = callback;

        speed = maxSpeed;
    }

    public void MoveTo(Tile tile, System.Action callback)
    {
        MoveTo(GridManager.Instance.GetWorldPosition(tile.x, tile.y), callback);
    }

    //protected Transform heading;

    //// Update is called once per frame
    //protected void FixedUpdate(){
    //	if ( heading!= null && heading.position != Vector3.zero)
    //	{			
    //		// heading detected
    //		Vector3 desiredPosition = new Vector3(heading.position.x, transform.position.y, heading.position.z);
    //		Facing(desiredPosition);
    //		transform.position = Vector3.MoveTowards(transform.position, desiredPosition, movementSpeed * Time.deltaTime);
    //		if ((transform.position - heading.position).sqrMagnitude < 1.05)
    //		{
    //			// arrived heading tile
    //			heading = null;
    //		}
    //	}
    //}

    //   // change direction of unit facing, sync with heading
    //void Facing(Vector3 pos)
    //{
    //	transform.LookAt(pos);
    //}

    //public void setHeading(Tile tile)
    //{
    //	//heading is the next tile unit will move to.
    //	heading = tile.transform;
    //}
}
