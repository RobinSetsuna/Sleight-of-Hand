using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum AttributeType : int
{
    Hp_i = 10,
    Hp_f = 11,
    Hp_p = 12,
    Atk_f = 1017,
    Dmg_f = 1018,
    Dmg = 19,

    Ap_i = 20,
    Ap_f = 21,
    Ap_p = 22,
    Ftg_pm = 1027,
    Ftg_p = 1028,
    Ftg = 29,

    Dr_i = 30,
    Dr_f = 31,

    Ar_i = 40,
    Ar_f = 41,

    Vr_i = 50,
    Vr_f = 51,

    Evsn_i = 60,
    Evsn_f = 61,
}

public enum Statistic : int
{
    Hp = 1,
    Ap = 2,
    DetectionRange = 3,
    AttackRange = 4,
    VisibleRange = 5,
    Evasion = 6,
}

public enum FatigueType : int
{
    General = 0,
    Movement,
}

public class StatisticSystem
{
    public class EventOnStatisticChange : UnityEvent<Statistic, float, float> { }

    /// <summary>
    /// An event triggered whenever a certain statistic in this system changes 
    /// </summary>
    public EventOnStatisticChange onStatisticChange = new EventOnStatisticChange();

    public EventOnDataChange3<StatusEffect> onStatusEffectChange = new EventOnDataChange3<StatusEffect>(); 

    /// <summary>
    /// 
    /// </summary>
    private Dictionary<Statistic, float> statistics = new Dictionary<Statistic, float>();

    /// <summary>
    /// Attributes that will never change over time
    /// </summary>
    private readonly AttributeSet talents;

    /// <summary>
    /// All status effects applied to this system in time order
    /// </summary>
    private StatusEffectQueue statusEffects = new StatusEffectQueue();

    public float this[Statistic statistic]
    {
        get
        {
            return statistics.ContainsKey(statistic) ? CalculateStatistic(statistic) : 0;
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
                onStatisticChange.Invoke(statistic, originalValue, statistics[statistic]);
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

    public int ApplyFatigue(int rFtg, FatigueType type = FatigueType.General)
    {
        int eFtg = CalculateEffectiveFatigue(rFtg, type);

        AddStatusEffect(new StatusEffect(1, 2, eFtg));

        return eFtg;
    }

    public int ApplyDamage(int rDmg)
    {
        if (Random.Range(0, 100) > CalculateStatistic(Statistic.Evasion))
        {
            int eDmg = rDmg;

            AddStatusEffect(new StatusEffect(0, int.MaxValue, eDmg));

            return eDmg;
        }

        return 0;
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

    public float CalculateStatistic(Statistic statistic)
    {
        return CalculateStatistic(statistic, talents, statusEffects);
    }

    public static float CalculateStatistic(Statistic statistic, params IAttributeCollection[] attributeSets)
    {
        switch (statistic)
        {
            case Statistic.Hp: // Hp = (∑Hp_i + ∑Hp_f) * (1 + ∑Hp_p) - ∑Dmg
                return (AttributeSet.Sum(AttributeType.Hp_i, attributeSets) + AttributeSet.Sum(AttributeType.Hp_f, attributeSets)) * (1 + AttributeSet.Sum(AttributeType.Hp_p, attributeSets)) - AttributeSet.Sum(AttributeType.Dmg, attributeSets);

            case Statistic.Ap: // Ap = MAX(0, (∑Ap_i + ∑Ap_f) * (1 + ∑Ap_p) - ∑Ftg)
                return Mathf.Max(0, (AttributeSet.Sum(AttributeType.Ap_i, attributeSets) + AttributeSet.Sum(AttributeType.Ap_f, attributeSets)) * (1 + AttributeSet.Sum(AttributeType.Ap_p, attributeSets)) - AttributeSet.Sum(AttributeType.Ftg, attributeSets));

            case Statistic.DetectionRange: // DetectionRange = MAX(-1, ∑Dr_i + ∑Dr_f)
                return Mathf.Max(-1, AttributeSet.Sum(AttributeType.Dr_i, attributeSets) + AttributeSet.Sum(AttributeType.Dr_f, attributeSets));

            case Statistic.AttackRange: // AttackRange = MAX(0, ∑Ar_i + ∑Ar_f)
                return Mathf.Max(0, AttributeSet.Sum(AttributeType.Ar_i, attributeSets) + AttributeSet.Sum(AttributeType.Ar_f, attributeSets));

            case Statistic.VisibleRange: // VisibleRange = MAX(-1, ∑Ar_i + ∑Ar_f)
                return Mathf.Max(-1, AttributeSet.Sum(AttributeType.Vr_i, attributeSets) + AttributeSet.Sum(AttributeType.Vr_f, attributeSets));

            case Statistic.Evasion: // Evasion = Evsn_i + Evsn_f
                return AttributeSet.Sum(AttributeType.Evsn_i, attributeSets) + AttributeSet.Sum(AttributeType.Evsn_f, attributeSets);

            default:
                return 0;
        }
    }

    public int CalculateEffectiveFatigue(int rFtg, FatigueType type = FatigueType.General)
    {
        return CalculateEffectiveFatigue(rFtg, type, talents, statusEffects);
    }

    public static int CalculateEffectiveFatigue(int rFtg, FatigueType type, params IAttributeCollection[] attributeSets)
    {
        switch (type)
        {
            case FatigueType.Movement: // eFtg = ROUND(rFtg * (1 + Ftg_pm) * (1 + Ftg_p))
                return Mathf.RoundToInt(rFtg * (1 + AttributeSet.Sum(AttributeType.Ftg_pm, attributeSets)) * (1 + AttributeSet.Sum(AttributeType.Ftg_p, attributeSets)));

            default: // eFtg = ROUND(rFtg * (1 + Ftg_p))
                return Mathf.RoundToInt(rFtg * (1 + AttributeSet.Sum(AttributeType.Ftg_p, attributeSets)));
        }
    }

    public int CalculateDamageOutput(int rAtk)
    {
        return CalculateDamageOutput(rAtk, talents, statusEffects);
    }

    public static int CalculateDamageOutput(int rAtk, params IAttributeCollection[] attributeSets)
    {
        return Mathf.RoundToInt(Mathf.Max(0, rAtk * (1 + AttributeSet.Sum(AttributeType.Atk_f, attributeSets)))); // rDmg = ROUND(MAX(0, rAtk * (1 + Atk_f)))
    }

    public bool AddStatusEffect(StatusEffect statusEffect)
    {
        bool isExisted = statusEffects.Contains(statusEffect);

        if (statusEffects.Push(statusEffect))
        {
#if UNITY_EDITOR
            Debug.Log(LogUtility.MakeLogString("StatisticSystem", "Add " + statusEffect + "\n" + ToString()));
#endif

            UpdateChangedStatistics(statusEffect);
            onStatusEffectChange.Invoke(isExisted ? ChangeType.Updating : ChangeType.Incremental, statusEffect);

            return true;
        }

        return false;
    }

    public StatusEffect RemoveStatusEffect(int id)
    {
        StatusEffect statusEffect = statusEffects.Remove(id);

        if (statusEffect != null)
        {
#if UNITY_EDITOR
            Debug.Log(LogUtility.MakeLogString("StatisticSystem", "Remove " + statusEffect + "\n" + ToString()));
#endif

            UpdateChangedStatistics(statusEffect);
            onStatusEffectChange.Invoke(ChangeType.Decremental, statusEffect);
        }

        return statusEffect;
    }

    private void HandleRoundNumberChange(int round)
    {
        List<StatusEffect> pastStatusEffects = new List<StatusEffect>();
        while (statusEffects.Top() != null && statusEffects.Top().EndRound == round)
            pastStatusEffects.Add(statusEffects.Pop());

        UpdateChangedStatistics(pastStatusEffects);
    }

    private void UpdateChangedStatistics(IAttributeCollection attributes)
    {
        HashSet<int> changedStatistics = new HashSet<int>();
        foreach (KeyValuePair<int, float> attribute in attributes)
            changedStatistics.Add(attribute.Key / 10);

        foreach (int id in changedStatistics)
        {
            Statistic statistic = (Statistic)id;
            this[statistic] = CalculateStatistic(statistic);
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
            Statistic statistic = (Statistic)id;
            this[statistic] = CalculateStatistic(statistic);
        }
    }

    public override string ToString()
    {
        string s = "";

        foreach (KeyValuePair<Statistic, float> statistic in statistics)
            s += ";" + statistic.Key + ":" + statistic.Value;

        return string.Format("Stat: {0}\nTalent: {1}\n\n{2}", s.Substring(1), talents, statusEffects);
    }
}
