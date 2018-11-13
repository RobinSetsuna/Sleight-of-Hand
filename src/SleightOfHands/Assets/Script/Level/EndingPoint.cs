using UnityEngine;

public class EndingPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<player>())
            LevelManager.Instance.NotifySuccess();
    }
}
