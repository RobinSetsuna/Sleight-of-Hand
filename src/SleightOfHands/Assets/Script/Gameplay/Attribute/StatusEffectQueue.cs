﻿using System.Collections;
using System.Collections.Generic;

public class StatusEffectQueue : IAttributeGetter
{
    private AttributeSet sumAttributes;
    private Dictionary<int, StatusEffect> map;

    private List<StatusEffect> list;
    private int current;

    public float this[int id]
    {
        get
        {
            return sumAttributes[id];
        }
    }

    public float this[AttributeType attribute]
    {
        get
        {
            return this[(int)attribute];
        }
    }

    public StatusEffectQueue()
    {
        sumAttributes = new AttributeSet();

        map = new Dictionary<int, StatusEffect>();
        list = new List<StatusEffect>();
        current = 0;
    }

    public bool IsEmpty()
    {
        return current >= list.Count;
    }

    public void Push(StatusEffect statusEffect)
    {
        int id = statusEffect.Id;
        if (map.ContainsKey(id))
        {
            StatusEffect existedStatusEffect = map[id];

            bool needReposition = existedStatusEffect.EndRound != statusEffect.EndRound;

            if (!existedStatusEffect.ReachMaxNumStacks())
            {
                foreach (KeyValuePair<int, float> attribute in existedStatusEffect)
                    sumAttributes.Add(attribute.Key, -attribute.Value);

                existedStatusEffect.Stack(statusEffect);

                foreach (KeyValuePair<int, float> attribute in existedStatusEffect)
                    sumAttributes.Add(attribute.Key, attribute.Value);
            }

            if (needReposition)
            {
                list.Remove(existedStatusEffect);
                Add(existedStatusEffect, current, list.Count - 1);
            }
        }
        else
        {
            if (IsEmpty())
                list.Add(statusEffect);
            else
                Add(statusEffect, current, list.Count - 1);

            map.Add(id, statusEffect);

            foreach (KeyValuePair<int, float> attribute in statusEffect)
                sumAttributes.Add(attribute.Key, attribute.Value);
        }
    }

    public StatusEffect Top()
    {
        return IsEmpty() ? null : list[current];
    }

    public StatusEffect Pop()
    {
        if (IsEmpty())
            return null;

        StatusEffect statusEffect = list[current++];

        map.Remove(statusEffect.Id);

        foreach (KeyValuePair<int, float> attribute in statusEffect)
            sumAttributes.Add(attribute.Key, -attribute.Value);

        return statusEffect;
    }

    private void Add(StatusEffect statusEffect, int start, int end)
    {
        if (start == end)
        {
            list.Insert(start + 1, statusEffect);

            return;
        }

        int mid = (start + end + 1) / 2;

        int compare = statusEffect.CompareTo(list[mid]);

        if (compare < 0)
            Add(statusEffect, start, mid - 1);
        else
            Add(statusEffect, mid, end);
    }

    public IEnumerator<KeyValuePair<int, float>> GetEnumerator()
    {
        return sumAttributes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return sumAttributes.GetEnumerator();
    }

    public override string ToString()
    {
        string s = "";

        for (int i = 0; i < list.Count; i++)
            s += (i == current ? "->" : "  ") + list[i].ToString() + "\n";

        return s;
    }
}
