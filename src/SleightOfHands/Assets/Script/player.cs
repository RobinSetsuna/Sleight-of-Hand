using UnityEngine;

///	<summary/>
/// Player - derived class of Unit
/// Active movement range, dragging path action, set heading
///
/// </summary>
public class player : Unit
{
    public int VisibleRange
    {
        get
        {
            return Mathf.RoundToInt(Statistics[StatisticType.VisibleRange]);
        }
    }

    protected override void Awake()
    {
        Statistics = new StatisticSystem(new AttributeSet(AttributeType.Ap_i, (float)initialActionPoint,
                                                          AttributeType.Hp_i, (float)initialHealth,
                                                          AttributeType.Vr_i, 1000f));

        onAttributeChange = Statistics.onStatisticChange;

        base.Awake();
    }
}
