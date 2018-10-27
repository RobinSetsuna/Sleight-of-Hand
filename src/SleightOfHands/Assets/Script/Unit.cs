using System.Collections;
using UnityEngine;

///	<summary/>
/// Unit
/// abstract class for all walkable unit in map
/// when heading, move to heading, facing heading direction.
/// </summary>
public abstract class Unit : InLevelObject
{
    [SerializeField] private float maxSpeed;
    public float MaxSpeed
    {
        get
        {
            return maxSpeed;
        }
    }

    [SerializeField] private int initialActionPoint;
    public int InitialActionPoint
    {
        get
        {
            return initialActionPoint;
        }
    }

    public int ActionPoint { get; protected set; }

    private float speed;
    private Vector3 start;
    private Vector3 destination;

    public void MoveTo(Vector3 destination, System.Action callback)
    {
        start = transform.position;
        this.destination = destination;

        speed = maxSpeed;

        StartCoroutine(Move(callback));
    }

    public void MoveTo(Tile tile, System.Action callback)
    {
        MoveTo(GridManager.Instance.GetWorldPosition(tile.x, tile.y), callback);
    }

    private void Start()
    {
        ActionPoint = initialActionPoint;
    }

    private IEnumerator Move(System.Action callback)
    {
        while (speed > 0)
        {
            Vector3 position = transform.position;

            // End move
            if (MathUtility.ManhattanDistance(destination.x, destination.z, position.x, position.z) < 0.05)
                speed = 0;

            Vector3 orientation = destination - position;
            orientation.y = 0;

            transform.forward = orientation.normalized;
            transform.Translate(0, 0, Mathf.Min(speed * Time.deltaTime, orientation.magnitude));

            yield return null;
        }

        ActionPoint--;

        if (callback != null)
            callback.Invoke();

        yield return null;
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
