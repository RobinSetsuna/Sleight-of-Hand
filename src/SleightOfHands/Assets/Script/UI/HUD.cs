using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Text turn;

    private void HandleTurnUpdate(int n)
    {
        turn.text = n.ToString();
    }
}
