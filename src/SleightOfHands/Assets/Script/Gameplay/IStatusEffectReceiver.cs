public interface IStatusEffectReceiver
{
    bool ApplyStatusEffect(StatusEffect statusEffect);
    StatusEffect RemoveStatusEffect(int id);
}
