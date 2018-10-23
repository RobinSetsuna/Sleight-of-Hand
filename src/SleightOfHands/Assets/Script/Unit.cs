using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract  class Unit : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void setStart_Pos(Tile tile)
	{
		Tile tile_node = GridManager._instance.TileFromWorldPoint(tile.transform.position);
		
	}
}
