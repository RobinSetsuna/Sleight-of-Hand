using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{

	[SerializeField]private int detection_range;
	private GameObject player;


	private int counter = 0;

	private bool signed = false;
    private bool detection_highlighted = false;
    private HashSet<Tile> rangeList;

	private void LateUpdate()
    {
		#region DEMO CODE
		if (!signed)
		{
			player = GameObject.FindGameObjectWithTag("Player");
			player.GetComponent<player>().onGridPositionChange.AddListener(HandleDetection);
			signed = true;
		}
		#endregion
	}

	public void Initiate()
	{
		setInitialPos(GridManager.Instance.TileFromWorldPoint(transform.position).gridPosition);
		player = GameObject.FindGameObjectWithTag("Player");
		player.GetComponent<player>().onGridPositionChange.AddListener(HandleDetection);
	}

    public void hightlightDetection()
    {
        // show the range to be detected
        if (detection_highlighted)
        {
            foreach (Tile tile in rangeList)
            {
                tile.Dehighlight();
            }
            detection_highlighted = false;
        }
        else
        {
            Tile current_tile = GridManager.Instance.TileFromWorldPoint(transform.position);
            rangeList = ProjectileManager.Instance.getProjectileRange(current_tile, detection_range);
            GridManager.Instance.DehighlightAll();
            foreach (Tile tile in rangeList)
            {
                tile.Highlight(Tile.HighlightColor.Red);
            }
            detection_highlighted = true;
        }
    }

	private void HandleDetection(Vector2Int previousPos, Vector2Int pos)
	{
		Tile current_tile = GridManager.Instance.TileFromWorldPoint(transform.position);

		rangeList = ProjectileManager.Instance.getProjectileRange(current_tile,detection_range);
		if(rangeList.Contains(GridManager.Instance.getTile(pos.x,pos.y)))
        {
			//detected
			// add some operation here
		}
	}
	
}
