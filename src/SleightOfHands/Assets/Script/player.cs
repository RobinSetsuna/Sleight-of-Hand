///	<summary/>
/// Player - derived class of Unit
/// Active movement range, dragging path action, set heading
///
/// </summary>
public class player : Unit
{
    protected override void Awake()
    {
        Statistics = new StatisticSystem(new AttributeSet(AttributeType.Ap_i, (float)initialActionPoint,
                                                          AttributeType.Hp_i, (float)initialHealth,
                                                          AttributeType.Vr_i, 1000f));

        onStatisticChange = Statistics.onStatisticChange;
        onStatusEffectChange = Statistics.onStatusEffectChange;

        base.Awake();
    }
}
