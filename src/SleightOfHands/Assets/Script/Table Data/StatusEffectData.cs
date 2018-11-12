public class StatusEffectData : ITableDataEntry
{
    public int Id { get; private set; }
    public int Type { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Attributes { get; private set; }
    public int MaxNumStacks { get; private set; }

    public int Initialize(int row, string[] stringData)
    {
        Id = int.Parse(stringData[0]);
        Type = int.Parse(stringData[1]);
        Name = stringData[2];
        Description = stringData[3];
        Attributes = stringData[4];
        MaxNumStacks = int.Parse(stringData[5]);

        return Id;
    }
}
