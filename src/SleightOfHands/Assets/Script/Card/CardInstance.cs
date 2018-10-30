using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardInstance : MonoBehaviour
{


    private string cardName;
    private string intro;
    private string type;
    private string effect;
    private int cost;
    private int ID;

    public void InitialCard(Card card)
    {
        cardName = card.cardName;
        intro = card.intro;
        type = card.type;
        effect = card.effect;
        ID = card.ID;

    }

    void WriteCardInfo()
    {
        this.gameObject.transform.GetChild(0);
    }

    public void Magnify()
    {
        //transform.localScale *= 1.1f;
        Debug.Log(cardName + " on enter");
        GetComponent<RectTransform>().localScale = Vector3.zero;
    }
}
