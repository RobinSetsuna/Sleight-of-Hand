using UnityEngine;

public class Glue : MonoBehaviour
{
    [SerializeField] private int duration = 4;

    void Start ()
    {
        LevelManager.Instance.onRoundNumberChange.AddListener(HandleRoundNumberChange);

        //CameraManager.Instance.FocusAt(transform.position);
        //GridManager.Instance.GetTile(this.transform.position).walkable = false;
    }

    void HandleRoundNumberChange(int round)
    {
        if (--duration == 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();

        if (enemy)
        {
            enemy.Statistics.AddStatusEffect(new StatusEffect(3, duration));
            EnemyManager.Instance.QuestionPop(other.gameObject.transform);
        }  
    }

    private void OnTriggerExit(Collider other)
    {
    }
}
