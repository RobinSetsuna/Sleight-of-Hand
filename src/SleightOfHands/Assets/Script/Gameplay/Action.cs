using UnityEngine;

/// <summary>
/// The base class for all in-game actions that units can have (e.g. movement, attack)
/// </summary>
public class Action
{
    /// <summary>
    /// [read-only] The time in milisecond when the action was created
    /// </summary>
    public readonly long timeCreated;

    protected System.Action<System.Action> actionDelegate;

    protected Action()
    {
        timeCreated = TimeUtility.localTimeInMilisecond;
    }

    /// <summary>
    /// Construct a new Action
    /// </summary>
    /// <param name="actionDelegate"> The delegate function to be executed when the action is being executed </param>
    public Action(System.Action<System.Action> actionDelegate) : this()
    {
        this.actionDelegate = actionDelegate;
    }

    /// <summary>
    /// Execute the action
    /// </summary>
    /// <param name="callback"> The callback function to be executed after the execution of this action </param>
    public void Execute(System.Action callback)
    {
        if (actionDelegate != null)
            actionDelegate.Invoke(callback);
    }
}

/// <summary>
/// A inherited class of Action for the movement of all units
/// </summary>
public class Movement : Action
{
    /// <summary>
    /// [read-only] The unit that is going to move
    /// </summary>
    public readonly Unit unit;

    /// <summary>
    /// [read-only] The destination of the movement
    /// </summary>
    public readonly Tile destination;

    /// <summary>
    /// Construct a new movement action
    /// </summary>
    /// <param name="unit"> The unit that is going to move </param>
    /// <param name="destination"> The destination of the movement </param>
    public Movement(Unit unit, Tile destination) : base()
    {
        actionDelegate = MakeUnitMove;

        this.unit = unit;
        this.destination = destination;
    }

    /// <summary>
    /// The specialized callback function for this kind of action
    /// </summary>
    /// <param name="callback"> The callback function to be executed after the execution of this movement </param>
    private void MakeUnitMove(System.Action callback)
    {
        unit.MoveTo(destination, callback);
    }

    override public string ToString()
    {
        return string.Format("[{0}] Move to {1}.", unit, destination);
    }
}

public class CardUsage : Action
{
    public readonly Unit initiator;
    public readonly Card card;
    public readonly Tile targetTile;

    public CardUsage(Unit initiator, Card card, Tile targetTile)
    {
        actionDelegate = UseCard;

        this.initiator = initiator;
        this.card = card;
        this.targetTile = targetTile;
    }

    private void UseCard(System.Action callback)
    {
        initiator.UseCard(card, targetTile, callback);
    }

    override public string ToString()
    {
        return string.Format("[{0}] Use a card \"{1}\" at {2}.", initiator, card.Data.Name, targetTile);
    }
}

public class StatusEffectApplication : Action
{
    public readonly StatusEffect statusEffect;
    public readonly StatisticSystem target;

    public StatusEffectApplication(StatusEffect statusEffect, StatisticSystem target)
    {
        actionDelegate = ApplyStatusEffect;

        this.statusEffect = statusEffect;
        this.target = target;
    }

    private void ApplyStatusEffect(System.Action callback)
    {
        target.AddStatusEffect(statusEffect);
        callback.Invoke();
    }

    override public string ToString()
    {
        return string.Format("Apply a status effect \"{0}\" on {1}.", statusEffect, target);
    }
}

public class Casting : Action
{
    public readonly GameObject obj;
    public readonly Tile targetTile;

    public Casting(GameObject obj, Tile targetTile)
    {
        actionDelegate = Cast;

        this.obj = obj;
        this.targetTile = targetTile;
    }

    private void Cast(System.Action callback)
    {
        GridManager.Instance.Spawn(obj, targetTile);
        callback.Invoke();
    }

    override public string ToString()
    {
        return string.Format("Cast a \"{0}\" on {1}.", obj, targetTile);
    }
}
