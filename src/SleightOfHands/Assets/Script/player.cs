using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///	<summary/>
/// Player - derived class of Unit
/// Active movement range, dragging path action, set heading
///
/// </summary>
public class player : Unit {

	// Use this for initialization
	private int action_point;
	private int path_index;
	private bool action;
	void Start ()
	{
		action_point = 5;
		action = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Jump"))
		{
			// press space to show movement range and ready to draw the path
			movementEnable();
		}
		if (Input.GetMouseButtonDown(1))
		{
			// right click disable the movement mode
			movementDisable();
		}
		
		if (Input.GetButtonDown("Fire1") && !action)
		{
			//script for player movement
			GridManager._instance.ok_to_drag = false;
			path_index = 0;
			GridManager._instance.TileFromWorldPoint(transform.position).Wipe();
			action = true;
		}
		
		// checking target tile during movement
		if (action && heading == null)
		{
			if (path_index > action_point ||GridManager._instance.generatedPath[path_index] == null)
			{
				GridManager._instance.wipeTiles();
				action = false;
			}
			else
			{
				// one tile move finished, assign new heading tile to unit
				var temp = GridManager._instance.generatedPath[path_index];
				setHeading(temp);
				temp.Wipe();
				path_index++;
			}
		}
		
		if (GridManager._instance.checktimes > action_point)
		{
			// exceed the action point limit
			// disable dragging
			GridManager._instance.ok_to_drag = false;
		}
	}

	public void movementEnable()
	{
		// enable movement mode, ok for highlight, selected tiles in map.
		GridManager._instance.wipeTiles();
		Tile tile_stand = GridManager._instance.TileFromWorldPoint(transform.position);
		GridManager._instance.Highlight(tile_stand,action_point,3);
		tile_stand.setSelected();
		GridManager._instance.ok_to_drag = true;
	}

	public void movementDisable()
	{
		// disable movement mode, wipe highlight, selected tiles in map.
		GridManager._instance.wipeTiles();
		GridManager._instance.ok_to_drag = false;
	}

	public void setActionPoints(int _Action_point)
	{
		//set Action points
		action_point = _Action_point;
	}

	public void Move()
	{
		// disable drag action
		GridManager._instance.ok_to_drag = false;
		// set path to start point
		path_index = 0;
		// wipe start tile
		GridManager._instance.TileFromWorldPoint(transform.position).Wipe();
		
		action = true; // trigger heading pushing
	}
}
