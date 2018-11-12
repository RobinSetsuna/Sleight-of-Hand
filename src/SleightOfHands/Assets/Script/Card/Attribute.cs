using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Attributes : int
{
    AP_c,
    AP_f,
}

public class Attribute
{
    private int turnCount = 0;
    private int duration;
    private Dictionary<Attributes, float> attDic;
    private CardData card;

    private Attribute()
    {
        attDic = new Dictionary<Attributes, float>();
        attDic.Add(Attributes.AP_c, 0);
        attDic.Add(Attributes.AP_f, 0);
        duration = 0;
    }

    public Attribute(CardData card) : this()
    {
        LoadAttributeFromCard(card);
        LevelManager.Instance.onCurrentTurnChange.AddListener(HandleTurnChange);
    }

    private void LoadAttributeFromCard(CardData card)
    {
        this.card = card;
        string[] attributes = card.ParseAttribute();
        attDic[Attributes.AP_c] += int.Parse(attributes[0]);
        attDic[Attributes.AP_f] += int.Parse(attributes[1]);
        duration = card.Duration;
    }

    private void HandleTurnChange(int currentTurn)
    {
        turnCount++;
        if(turnCount == duration)
        {
            LogUtility.PrintLogFormat("Attribute", "Card Effect {0} Time Out .", card.Name);
            CardManager.Instance.onAttributeTimeOut.Invoke(this);
        }
        
    }

    public float GetDic(Attributes att)
    {
        return attDic[att];
    }
}
