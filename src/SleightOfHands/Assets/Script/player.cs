using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : Unit {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Jump"))
		{
			Tile tile_stand = GridManager._instance.TileFromWorldPoint(transform.position);
			GridManager._instance.Highlight(tile_stand,5,1);
		}
	   }
	
	
}
