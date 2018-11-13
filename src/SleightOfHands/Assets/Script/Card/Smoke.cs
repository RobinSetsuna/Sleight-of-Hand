using UnityEngine;

public class Smoke : MonoBehaviour
{
    [SerializeField] private int duration = 4;
    [SerializeField] private int detectionRange = 3;

    //private Vector2Int center;
    //private HashSet<Enemy> enemies = new HashSet<Enemy>();

	void Start ()
    {
        //center = GridManager.Instance.GetGridPosition(transform.position);

        LevelManager.Instance.onRoundNumberChange.AddListener(HandleRoundNumberChange);

        //GridManager.Instance.onUnitMove.AddListener(HandleUnitMove);
        //CameraManager.Instance.FocusAt(transform.position);
    }

	// Update is called once per frame
	//void Update ()
 //   {
	//	if(GameObject.FindGameObjectsWithTag("Enemy") != null)
 //       {
 //           var enemies = GameObject.FindGameObjectsWithTag("Enemy");
 //           foreach(GameObject obj in enemies)
 //           {
 //               Enemy enemy = obj.GetComponent<Enemy>();
 //               if (CheckEnemy(obj))
 //               {
 //                   //StartCoroutine(Question(obj));
 //                   if (enemy.CurrentDetectionState == EnemyDetectionState.Found)
 //                   {
 //                       EnemyManager.Instance.QuestionPop(obj.transform);

 //                       enemy.Statistics.AddStatusEffect(new StatusEffect(4, duration));
 //                       enemy.SetDetectionState(EnemyDetectionState.Normal);
 //                   }
 //               }
 //               else
 //               {
 //                       enemy.DetectionRange = 3;
 //               }
 //           }
 //       }
	//}

    //private bool CheckEnemy(GameObject obj)
    //{
    //    Tile enemyTile = GridManager.Instance.GetTile(obj.transform.position);
    //    Tile smokeTile = GridManager.Instance.GetTile(transform.position);
    //    if (MathUtility.ManhattanDistance(enemyTile.x, enemyTile.y, smokeTile.x, smokeTile.y) <= detectionRange)
    //        return true;
    //    else
    //        return false;
    //}

    void HandleRoundNumberChange(int turn)
    {
        if(--duration == 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Enemy>())
        {
            Enemy enemy = other.GetComponent<Enemy>();

            EnemyManager.Instance.QuestionPop(enemy.transform);

            enemy.Statistics.AddStatusEffect(new StatusEffect(3, 2));
            enemy.Statistics.AddStatusEffect(new StatusEffect(4, duration));
            enemy.SetDetectionState(EnemyDetectionState.Normal);
        }
        else if (other.GetComponent<player>())
        {
            player Player = other.GetComponent<player>();

            Player.Statistics.AddStatusEffect(new StatusEffect(5, duration));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy)
                enemy.SetDetectionState(EnemyDetectionState.Normal);
            enemy.Statistics.RemoveStatusEffect(4);
        }
    }

    //private void HandleUnitMove(Unit unit, Vector2Int previousGridPosition, Vector2Int currentGridPosition)
    //{
    //    Enemy enemy = unit.GetComponent<Enemy>();

    //    if (enemy)
    //    {
    //        if (MathUtility.ChebyshevDistance(center.x, center.y, currentGridPosition.x, currentGridPosition.y) <= 1)
    //        {
    //            if (!enemies.Contains(enemy))
    //            {
    //                enemies.Add(enemy);

    //                EnemyManager.Instance.QuestionPop(enemy.transform);

    //                enemy.Statistics.AddStatusEffect(new StatusEffect(4, duration));
    //                enemy.SetDetectionState(EnemyDetectionState.Normal);
    //            }
    //        }
    //        else if (enemies.Contains(enemy))
    //        {
    //            enemies.Remove(enemy);

    //            enemy.Statistics.RemoveStatusEffect(4);
    //        }
    //    }
    //}
}
