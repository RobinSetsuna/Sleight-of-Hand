using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{

	[SerializeField]private int detection_range;
	private GameObject player;

	private int counter = 0;

    private bool detection_highlighted = false;
    private HashSet<Tile> rangeList;

    protected override void Awake()
    {
        base.Awake();

        GridManager.Instance.onUnitMove.AddListener(HandleDetection);
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
            Tile current_tile = GridManager.Instance.GetTile(transform.position);
            rangeList = ProjectileManager.Instance.getProjectileRange(current_tile, detection_range);

            GridManager.Instance.DehighlightAll();
            foreach (Tile tile in rangeList)
            {
                tile.Highlight(Tile.HighlightColor.Red);
            }
            detection_highlighted = true;
        }
    }

	private void HandleDetection(Unit unit, Vector2Int previousPos, Vector2Int pos)
	{
        if (unit.tag == "Player")
        {
            Tile current_tile = GridManager.Instance.GetTile(transform.position);
            rangeList = ProjectileManager.Instance.getProjectileRange(current_tile, detection_range);

            if (rangeList.Contains(GridManager.Instance.GetTile(pos)))
            {
                //detected
                // add some operation here
            }
        }
	}
}
