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

    void HandleRoundNumberChange(int turn)
    {
        if(--duration == 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //other.SendMessage("ApplyStatusEffect", new StatusEffect(3, 2));
        //other.SendMessage("ApplyStatusEffect", new StatusEffect(4, duration));
        //other.SendMessage("ApplyStatusEffect", new StatusEffect(5, duration));

        if (other.GetComponent<Enemy>())
        {
            Enemy enemy = other.GetComponent<Enemy>();

            EnemyManager.Instance.QuestionPop(enemy.transform);

            enemy.ApplyStatusEffect(new StatusEffect(3, 2));
            enemy.ApplyStatusEffect(new StatusEffect(4, duration));
            enemy.GetComponent<EnemyController>().SetDetectionState(EnemyMode.Dazzled);
        }
        else if (other.GetComponent<player>())
        {
            player Player = other.GetComponent<player>();

            Player.ApplyStatusEffect(new StatusEffect(5, duration));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Enemy>())
            other.GetComponent<Enemy>().RemoveStatusEffect(4);
        else if (other.GetComponent<player>())
            other.GetComponent<player>().RemoveStatusEffect(5);
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
