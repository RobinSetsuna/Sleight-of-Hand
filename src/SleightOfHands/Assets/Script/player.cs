using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : Unit {

	// Use this for initialization
	private int action_point;
	void Start ()
	{
		action_point = 5;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Jump"))
		{
			// press space to show movement range and ready to draw the path
			Tile tile_stand = GridManager._instance.TileFromWorldPoint(transform.position);
			GridManager._instance.Highlight(tile_stand,action_point,3);
			tile_stand.set_selected();
			GridManager._instance.ok_to_drag = true;
		}

		if (Input.GetMouseButtonDown(1))
		{
			// right click cancel the path
			GridManager._instance.wipeTiles();
			Tile tile_stand = GridManager._instance.TileFromWorldPoint(transform.position);
			GridManager._instance.Highlight(tile_stand,action_point,3);
			tile_stand.set_selected();
			GridManager._instance.ok_to_drag = true;
		}
		
		if (GridManager._instance.checktimes >= action_point)
		{
			// exceed the action point limit
			GridManager._instance.ok_to_drag = false;
		}
	}
}
