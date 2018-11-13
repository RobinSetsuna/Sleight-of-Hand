using System.Collections.Generic;
using UnityEngine.Events;

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

        RefreshChangedStatistics(this.talents);

        LevelManager.Instance.onRoundNumberChange.AddListener(HandleRoundNumberChange);
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

            case StatisticType.Ap: // Ap = (∑Ap_i + ∑Ap_f) * (1 + ∑Ap_p) - ∑Ap_c
                return (AttributeSet.Sum(AttributeType.Ap_i, attributeSets) + AttributeSet.Sum(AttributeType.Ap_f, attributeSets)) * (1 + AttributeSet.Sum(AttributeType.Ap_p, attributeSets)) - AttributeSet.Sum(AttributeType.Ap_c, attributeSets);

            default:
                return 0;
        }
    }

    public void AddStatusEffect(StatusEffect statusEffect)
    {
        statusEffects.Push(statusEffect);

        RefreshChangedStatistics(statusEffect);

#if UNITY_EDITOR
        UnityEngine.Debug.Log(LogUtility.MakeLogString("StatisticSystem", "Add " + statusEffect + "\n" + ToString()));
#endif
    }

    private void HandleRoundNumberChange(int round)
    {
        List<StatusEffect> pastStatusEffects = new List<StatusEffect>();
        while (statusEffects.Top() != null && statusEffects.Top().EndRound == round)
            pastStatusEffects.Add(statusEffects.Pop());

        RefreshChangedStatistics(pastStatusEffects);
    }

    private void RefreshChangedStatistics(IAttributeGetter attributes)
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

    private void RefreshChangedStatistics(List<StatusEffect> statusEffects)
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
