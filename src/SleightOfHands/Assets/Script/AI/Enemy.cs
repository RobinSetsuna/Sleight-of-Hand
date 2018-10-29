using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class Enemy : Unit
{

	[SerializeField]private int detection_range;
	private GameObject player;


	private int counter = 0;

	private bool signed = false;
	// Use this for initialization
	 private void Awake () {
		
	}
// Update is called once per frame
	private void LateUpdate() {
		
		#region DEMO CODE
		if (!signed)
		{
			player = GameObject.FindGameObjectWithTag("Player");
			player.GetComponent<player>().OnPositionUpdateForUnit.AddListener(HandleMouseDrag);
			signed = true;
		}
		#endregion

	}

	public void Initiate()
	{
		setInitialPos(GridManager.Instance.TileFromWorldPoint(transform.position).gridPosition);
		player = GameObject.FindGameObjectWithTag("Player");
		player.GetComponent<player>().OnPositionUpdateForUnit.AddListener(HandleMouseDrag);
	}

	private void HandleMouseDrag(Vector2Int pos)
	{
		Tile current_tile = GridManager.Instance.TileFromWorldPoint(transform.position);
		var rangeList =ProjectileManager.Instance.getProjectileRange(current_tile,detection_range);

		#region DEMO CODE
		GridManager.Instance.DehighlightAll();
		foreach (Tile tile in rangeList)
		{
			tile.Highlight(Tile.HighlightColor.Red);
		}
		#endregion

		if(rangeList.Contains(GridManager.Instance.getTile(pos.x,pos.y))){
			//detected
			// add some operation here
		}
	}
	
}
