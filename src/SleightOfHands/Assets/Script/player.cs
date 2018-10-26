///	<summary/>
/// Player - derived class of Unit
/// Active movement range, dragging path action, set heading
///
/// </summary>
public class player : Unit
{
	private int path_index;
	private bool action;

    public int ActionPoint { get; private set; }

    // Use this for initialization
    void Start()
    {
        ActionPoint = 5;
        action = false;
    }

    // Update is called once per frame
    //void Update()
    //{
    //if (Input.GetButtonDown("Jump"))
    //{
    //    // press space to show movement range and ready to draw the path
    //    movementEnable();
    //}
    //if (Input.GetMouseButtonDown(1))
    //{
    //    // right click disable the movement mode
    //    movementDisable();
    //}

    //if (Input.GetButtonDown("Fire1") && !action)
    //{
    //    //script for player movement
    //    GridManager.Instance.ok_to_drag = false;
    //    path_index = 0;
    //    GridManager.Instance.TileFromWorldPoint(transform.position).Wipe();
    //    action = true;
    //}

    // checking target tile during movement
    //if (action && heading == null)
    //{
    //    if (path_index > action_point || GridManager.Instance.generatedPath[path_index] == null)
    //    {
    //        GridManager.Instance.wipeTiles();
    //        action = false;
    //    }
    //    else
    //    {
    //        // one tile move finished, assign new heading tile to unit
    //        var temp = GridManager.Instance.generatedPath[path_index];
    //        setHeading(temp);
    //        temp.Wipe();
    //        path_index++;
    //    }
    //}

    //if (GridManager.Instance.checktimes > action_point)
    //{
    //    // exceed the action point limit
    //    // disable dragging
    //    GridManager.Instance.ok_to_drag = false;
    //}
    //}

 //   public void movementEnable()
	//{
	//	// enable movement mode, ok for highlight, selected tiles in map.
	//	GridManager.Instance.wipeTiles();
	//	Tile tile_stand = GridManager.Instance.TileFromWorldPoint(transform.position);
	//	GridManager.Instance.Highlight(tile_stand,ActionPoint,3);
	//	tile_stand.setSelected();
	//	GridManager.Instance.ok_to_drag = true;
	//}

	//public void movementDisable()
	//{
	//	// disable movement mode, wipe highlight, selected tiles in map.
	//	GridManager.Instance.wipeTiles();
	//	GridManager.Instance.ok_to_drag = false;
	//}

	//public void setActionPoints(int _Action_point)
	//{
	//	//set Action points
	//	ActionPoint = _Action_point;
	//}

    //public void Move()
    //{
    //    // disable drag action
    //    GridManager.Instance.ok_to_drag = false;
    //    // set path to start point
    //    path_index = 0;
    //    // wipe start tile
    //    GridManager.Instance.TileFromWorldPoint(transform.position).Wipe();

    //    action = true; // trigger heading pushing
    //}
}
