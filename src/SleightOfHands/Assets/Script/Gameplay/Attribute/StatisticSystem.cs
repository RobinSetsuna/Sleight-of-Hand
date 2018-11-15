using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    Dr_i = 30,
    Dr_f = 31,

    Ar_i = 40,
    Ar_f = 41,

    Vr_i = 50,
    Vr_f = 51,
}

public enum StatisticType : int
{
    Hp = 1,
    Ap = 2,
    DetectionRange = 3,
    AttackRange = 4,
    VisibleRange = 5,
}

public class StatisticSystem
{
    public class EventOnStatisticChange : UnityEvent<StatisticType, float, float> { }

    /// <summary>
    /// An event triggered whenever a certain statistic in this system changes 
    /// </summary>
    public EventOnStatisticChange onStatisticChange = new EventOnStatisticChange();

    /// <summary>
    /// 
    /// </summary>
    private Dictionary<StatisticType, float> statistics = new Dictionary<StatisticType, float>();

    /// <summary>
    /// Attributes that will never change over time
    /// </summary>
    private readonly AttributeSet talents;

    /// <summary>
    /// All status effects applied to this system in time order
    /// </summary>
    private StatusEffectQueue statusEffects = new StatusEffectQueue();

    public float this[StatisticType statistic]
    {
        get
        {
            return statistics.ContainsKey(statistic) ? statistics[statistic] : 0;
        }

        set
        {
            bool hasChange = true;
            float originalValue = 0;

            if (!statistics.ContainsKey(statistic))
                statistics.Add(statistic, value);
            else
            {
                originalValue = statistics[statistic];

                if (value != originalValue)
                    statistics[statistic] = value;
                else
                    hasChange = false;
            }

            if (hasChange)
                onStatisticChange.Invoke(statistic, originalValue, value);
        }
    }

    private StatisticSystem()
    {
        talents = new AttributeSet();
    }

    public StatisticSystem(AttributeSet talents)
    {
        this.talents = talents;

        UpdateChangedStatistics(this.talents);

        LevelManager.Instance.onRoundNumberChange.AddListener(HandleRoundNumberChange);
    }

    public int ApplyFatigue(int rawFatigue)
    {
        int fatigue = rawFatigue;

        AddStatusEffect(new StatusEffect(1, 2, fatigue));

        return fatigue;
    }

    public int ApplyDamage(int rawDamage)
    {
        int damage = rawDamage;

        AddStatusEffect(new StatusEffect(0, int.MaxValue, damage));

        return damage;
    }

    public float Sum(AttributeType attribute)
    {
        return AttributeSet.Sum(attribute, talents, statusEffects);
    }

    public float CalculateActualApCost(int rawCost)
    {
        return rawCost;
    }

    //~StatisticSystem()
    //{
    //    LevelManager.Instance.onRoundNumberChange.RemoveListener(HandleRoundNumberChange);
    //}

    public static float CalculateStatisticValue(StatisticType statistic, params IAttributeGetter[] attributeSets)
    {
        switch (statistic)
        {
            case StatisticType.Hp: // Hp = (∑Hp_i + ∑Hp_f) * (1 + ∑Hp_p) - ∑Hp_c
                return (AttributeSet.Sum(AttributeType.Hp_i, attributeSets) + AttributeSet.Sum(AttributeType.Hp_f, attributeSets)) * (1 + AttributeSet.Sum(AttributeType.Hp_p, attributeSets)) - AttributeSet.Sum(AttributeType.Hp_c, attributeSets);

            case StatisticType.Ap: // Ap = MAX(0, (∑Ap_i + ∑Ap_f) * (1 + ∑Ap_p) - ∑Ap_c)
                return Mathf.Max(0, (AttributeSet.Sum(AttributeType.Ap_i, attributeSets) + AttributeSet.Sum(AttributeType.Ap_f, attributeSets)) * (1 + AttributeSet.Sum(AttributeType.Ap_p, attributeSets)) - AttributeSet.Sum(AttributeType.Ap_c, attributeSets));

            case StatisticType.DetectionRange: // DetectionRange = MAX(0, ∑Dr_i + ∑Dr_f)
                return Mathf.Max(0, AttributeSet.Sum(AttributeType.Dr_i, attributeSets) + AttributeSet.Sum(AttributeType.Dr_f, attributeSets));

            case StatisticType.AttackRange: // AttackRange = MAX(0, ∑Ar_i + ∑Ar_f)
                return Mathf.Max(0, AttributeSet.Sum(AttributeType.Ar_i, attributeSets) + AttributeSet.Sum(AttributeType.Ar_f, attributeSets));

            case StatisticType.VisibleRange: // VisibleRange = MAX(0, ∑Ar_i + ∑Ar_f)
                return Mathf.Max(0, AttributeSet.Sum(AttributeType.Vr_i, attributeSets) + AttributeSet.Sum(AttributeType.Vr_f, attributeSets));

            default:
                return 0;
        }
    }

    public void AddStatusEffect(StatusEffect statusEffect)
    {
        if (statusEffects.Push(statusEffect))
            UpdateChangedStatistics(statusEffect);

#if UNITY_EDITOR
        Debug.Log(LogUtility.MakeLogString("StatisticSystem", "Add " + statusEffect + "\n" + ToString()));
#endif
    }

    public StatusEffect RemoveStatusEffect(int id)
    {
        StatusEffect statusEffect = statusEffects.Remove(id);

#if UNITY_EDITOR
        Debug.Log(LogUtility.MakeLogString("StatisticSystem", "Remove " + statusEffect + "\n" + ToString()));
#endif

        return statusEffect;
    }

    private void HandleRoundNumberChange(int round)
    {
        List<StatusEffect> pastStatusEffects = new List<StatusEffect>();
        while (statusEffects.Top() != null && statusEffects.Top().EndRound == round)
            pastStatusEffects.Add(statusEffects.Pop());

        UpdateChangedStatistics(pastStatusEffects);
    }

    private void UpdateChangedStatistics(IAttributeGetter attributes)
    {
        HashSet<int> changedStatistics = new HashSet<int>();
        foreach (KeyValuePair<int, float> attribute in attributes)
            changedStatistics.Add(attribute.Key / 10);

        foreach (int id in changedStatistics)
        {
            StatisticType statistic = (StatisticType)id;
            this[statistic] = CalculateStatisticValue(statistic, talents, statusEffects);
        }
    }

    private void UpdateChangedStatistics(List<StatusEffect> statusEffects)
    {
        HashSet<int> changedStatistics = new HashSet<int>();
        foreach (StatusEffect statusEffect in statusEffects)
            foreach (KeyValuePair<int, float> attribute in statusEffect)
                changedStatistics.Add(attribute.Key / 10);

        foreach (int id in changedStatistics)
        {
            StatisticType statistic = (StatisticType)id;
            this[statistic] = CalculateStatisticValue(statistic, talents, this.statusEffects);
        }
    }

    public override string ToString()
    {
        string s = "";

        foreach (KeyValuePair<StatisticType, float> statistic in statistics)
            s += ";" + statistic.Key + ":" + statistic.Value;

        return string.Format("Stat: {0}\nTalent: {1}\n\n{2}", s.Substring(1), talents, statusEffects);
    }
}
