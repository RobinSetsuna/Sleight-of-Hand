using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Attributes : int
{
    HP_c = 0,
    HP_f,
    AP_c,
    AP_f,
    ATK_c,
    ATK_f
}

public class Attribute{

    private int turnCount = 0;
    private int duration;
    private Dictionary<Attributes, float> attDic;
    private Card card;

    private Attribute()
    {
        attDic = new Dictionary<Attributes, float>();
        attDic.Add(Attributes.HP_c, 0);
        attDic.Add(Attributes.AP_c, 0);
        attDic.Add(Attributes.ATK_c, 0);
        attDic.Add(Attributes.HP_f, 0);
        attDic.Add(Attributes.AP_f, 0);
        attDic.Add(Attributes.ATK_f, 0);
        duration = 0;
    }

    public Attribute(Card card) : this()
    {
        LoadAttributeFromCard(card);
        LevelManager.Instance.onCurrentTurnChange.AddListener(HandleTurnChange);
    }

    private void LoadAttributeFromCard(Card card)
    {
        this.card = card;
        attDic[Attributes.HP_c] += card.HP_c;
        attDic[Attributes.AP_c] += card.AP_c;
        attDic[Attributes.ATK_c] += card.ATK_c;
        attDic[Attributes.HP_f] += card.HP_f;
        attDic[Attributes.AP_f] += card.AP_f;
        attDic[Attributes.ATK_f] += card.ATK_f;
        duration = card.duration;
    }

    private void HandleTurnChange(int currentTurn)
    {
        turnCount++;
        if(turnCount == duration)
        {
            LogUtility.PrintLogFormat("Attribute", "Card Effect {0} Time Out .", card.cardName);
            CardManager.Instance.onAttributeTimeOut.Invoke(this);
        }
        
    }

    public float GetDic(Attributes att)
    {
        return attDic[att];
    }
}
