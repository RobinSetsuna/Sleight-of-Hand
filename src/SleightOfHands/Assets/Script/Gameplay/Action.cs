public class Action
{
    public long TimeCreated { get; private set; }

    protected System.Action<System.Action> Callback { get; set; }

    public Action()
    {
        TimeCreated = TimeUtility.localTimeInMilisecond;
    }

    public Action(System.Action<System.Action> callback) : this()
    {
        Callback = callback;
    }

    public void Execute(System.Action callback)
    {
        if (Callback != null)
            Callback.Invoke(callback);
    }
}

public class Movement : Action
{
    public readonly Unit unit;
    public readonly Tile destination;

    public Movement(Unit unit, Tile destination) : base()
    {
        Callback = MakeUnitMove;

        this.unit = unit;
        this.destination = destination;
    }

    private void MakeUnitMove(System.Action callback)
    {
        unit.MoveTo(destination, callback);
    }

    override public string ToString()
    {
        return string.Format("[{0}] Moved to {1}.", unit, destination);
    }
}
