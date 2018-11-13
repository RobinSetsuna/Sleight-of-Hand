using System;
using System.Collections;
using System.Collections.Generic;

public class StatusEffect : IAttributeGetter, IComparable
{
    private readonly AttributeSet attributes;

    public StatusEffectData Data { get; private set; }

    public int Id { get; private set; }

    public int NumStacks { get; private set; }
    public int EndRound { get; private set; }
    
    public int MaxNumStacks
    {
        get
        {
            return Data.MaxNumStacks;
        }
    }

    public float this[int id]
    {
        get
        {
            return attributes[id] * NumStacks;
        }
    }

    public float this[AttributeType attribute]
    {
        get
        {
            return attributes[(int)attribute] * NumStacks;
        }
    }

    private StatusEffect()
    {
    }

    public StatusEffect(int id, int duration, int numStacks = 1)
    {
        Id = id;
        EndRound = duration == int.MaxValue ? int.MaxValue : LevelManager.Instance.RoundNumber + duration;
        NumStacks = numStacks;

        Data = TableDataManager.Singleton.GetStatusEffectData(id);

        attributes = AttributeSet.Parse(Data.Attributes);
    }

    public bool ReachMaxNumStacks()
    {
        return NumStacks == MaxNumStacks;
    }

    internal void Stack(StatusEffect other)
    {
        if (NumStacks != MaxNumStacks)
            NumStacks = Math.Min(NumStacks + other.NumStacks, MaxNumStacks);

        if (other.EndRound > EndRound)
            EndRound = other.EndRound;
    }

    public int CompareTo(StatusEffect other)
    {
        return EndRound.CompareTo(other.EndRound);
    }

    public int CompareTo(object obj)
    {
        return CompareTo((StatusEffect)obj);
    }

    public IEnumerator<KeyValuePair<int, float>> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new Enumerator(this);
    }

    public override string ToString()
    {
        return string.Format("[{0}][x{1}/{2}] Id:{3} Name:{4} Attributes:{5}", EndRound, NumStacks, MaxNumStacks, Id, Data.Name, attributes);
    }

    public class Enumerator : IEnumerator<KeyValuePair<int, float>>
    {
        private int numStacks;
        private IEnumerator<KeyValuePair<int, float>> attributeSetEnumerator;

        public Enumerator(StatusEffect statusEffect)
        {
            numStacks = statusEffect.NumStacks;
            attributeSetEnumerator = statusEffect.attributes.GetEnumerator();
        }

        public KeyValuePair<int, float> Current
        {
            get
            {
                return new KeyValuePair<int, float>(attributeSetEnumerator.Current.Key, attributeSetEnumerator.Current.Value * numStacks);
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public void Dispose() {}

        public bool MoveNext()
        {
            return attributeSetEnumerator.MoveNext();
        }

        public void Reset()
        {
            attributeSetEnumerator.Reset();
        }
    }
}
