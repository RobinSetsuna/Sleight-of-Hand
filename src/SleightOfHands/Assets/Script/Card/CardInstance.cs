using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardInstance : MonoBehaviour{


    private string cardName;
    private string intro;
    private string type;
    private string effect;
    private int ID;

    public CardInstance(string _cardName, string _intro, string _type, string _effect, int _ID)
    {
        cardName = _cardName;
        intro = _intro;
        type = _type;
        effect = _effect;
        ID = _ID;
    }

    void WriteCardInfo()
    {
        this.gameObject.transform.GetChild(0);
    }
}
