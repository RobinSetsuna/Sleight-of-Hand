using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour {

    
    private List<Attribute> attList = new List<Attribute>();
	// Use this for initialization

    public void AddEffect(Card card)
    {
        Attribute att = new Attribute(card);
        attList.Add(att);
    }
	void Awake () {
        CardManager.Instance.onAttributeTimeOut.AddListener(HandleTimeOut);
        LevelManager.Instance.onCurrentTurnChange.AddListener(HandleTurnChange);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void HandleTimeOut(Attribute att)
    {
        attList.Remove(att);
        LevelManager.Instance.onNewTurnUpdateAttribute.Invoke(this);
    }

    private void HandleTurnChange(int curr)
    {
        LevelManager.Instance.onNewTurnUpdateAttribute.Invoke(this);
    }

    public float GetAP_c()
    {
        float totalAPc = 0;
        foreach (Attribute att in attList)
            totalAPc += att.GetDic(Attributes.AP_c);
        return totalAPc;
    }

    public float GetAP_f()
    {
        float totalAPf = 0;
        foreach (Attribute att in attList)
            totalAPf += att.GetDic(Attributes.AP_f);
        return totalAPf;
    }
    public float GetHP_c()
    {
        float totalHPc = 0;
        foreach (Attribute att in attList)
            totalHPc += att.GetDic(Attributes.HP_c);
        return totalHPc;
    }
    public float GetHP_f()
    {
        float totalHPf = 0;
        foreach (Attribute att in attList)
            totalHPf += att.GetDic(Attributes.HP_f);
        return totalHPf;
    }

    public int CurrentAP_c()
    {
        return (int)attList[attList.Count - 1].GetDic(Attributes.AP_c);
    }

    public int CurrentAP_f()
    {
        return (int)attList[attList.Count - 1].GetDic(Attributes.AP_f);
    }
    public float CurrentHP_c()
    {
        return attList[attList.Count - 1].GetDic(Attributes.AP_c);
    }

    public float CurrentHP_f()
    {
        return attList[attList.Count - 1].GetDic(Attributes.AP_f);
    }
}
