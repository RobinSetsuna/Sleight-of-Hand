﻿using System.Collections;
using System.Collections.Generic;

public enum AttributeType : int
{
    Hp_i = 10,
    Hp_f = 11,
    Hp_p = 12,
    Hp_c = 19,

    Ap_i = 20,
    Ap_f = 21,
    Ap_p = 22,
    Ap_c = 29,
}

public enum StatisticType : int
{
    Hp = 1,
    Ap = 2,
}

public class AttributeSet : IAttributeGetter
{
    private Dictionary<int, float> attributes;

    public float this[int id]
    {
        get
        {
            return attributes.ContainsKey(id) ? attributes[id] : 0;
        }
    }

    public float this[AttributeType attribute]
    {
        get
        {
            return this[(int)attribute];
        }
    }

    public AttributeSet()
    {
        attributes = new Dictionary<int, float>();
    }

    private AttributeSet(string s) : this()
    {
        foreach (string field in s.Split(';'))
        {
            string[] values = field.Split(':');

            Set(int.Parse(values[0]), float.Parse(values[1]));
        }
    }

    public static AttributeSet Parse(string s)
    {
        return new AttributeSet(s);
    }

    public static AttributeSet Sum(params IAttributeGetter[] attributeSets)
    {
        AttributeSet attributeSet = new AttributeSet();

        foreach (IAttributeGetter attributes in attributeSets)
            foreach (KeyValuePair<int, float> attribute in attributes)
                attributeSet.Add(attribute.Key, attribute.Value);

        return attributeSet;
    }

    public static float Sum(AttributeType attribute, params IAttributeGetter[] attributeSets)
    {
        float sum = 0;

        foreach (IAttributeGetter attributeSet in attributeSets)
            sum += attributeSet[attribute];

        return sum;
    }

    public float Add(int id, float value)
    {
        if (!attributes.ContainsKey(id))
            attributes.Add(id, value);
        else
            attributes[id] += value;

        return attributes[id];
    }

    public void Set(int id, float value)
    {
        if (!attributes.ContainsKey(id))
            attributes.Add(id, value);
        else
            attributes[id] = value;
    }

    public IEnumerator<KeyValuePair<int, float>> GetEnumerator()
    {
        return attributes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return attributes.GetEnumerator();
    }

    public override string ToString()
    {
        string s = "";

        foreach (KeyValuePair<int, float> attribute in attributes)
            s += ";" + (AttributeType)attribute.Key + ":" + attribute.Value;

        return s.Substring(1);
    }
}