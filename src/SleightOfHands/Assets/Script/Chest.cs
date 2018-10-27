using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour {

    public bool isOpen = false;
    string cardName = "Haste";

    void SetChestOpen()
    {
        isOpen = true;
    }
}
