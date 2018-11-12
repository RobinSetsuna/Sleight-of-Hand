using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public bool isOpen = false;
    public string cardName = "Haste";

    private void FixedUpdate()
    {
        if(isOpen == true)
        {
            //1.get card
            // CardManager.Instance.GetCard(cardName);

            //2.play the animation
        }
    }
}
