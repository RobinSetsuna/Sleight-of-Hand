public class CardData : ITableDataEntry
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Template { get; private set; }
    public string Illustration { get; private set; }
    public string Description { get; private set; }
    public int Range { get; private set; }
    public int Duration { get; private set; }
    public string Effect { get; private set; }
    public int EffectType { get; private set; }

    int ITableDataEntry.Initialize(int row, string[] stringData)
    {
        Id = int.Parse(stringData[0]);
        Name = stringData[1];
        Template = stringData[2];
        Illustration = stringData[3];
        Description = stringData[4];
        Range = int.Parse(stringData[5]);
        Duration = int.Parse(stringData[6]);
        Effect = stringData[7];
        EffectType = int.Parse(stringData[8]);

        return Id;
    }

    public string[] ParseAttribute()
    {
        return null;
    }

    public override string ToString()
    {
        return string.Format("Id: {0}\nName: {1}\nTemplate: {2}\nIllustration: {3}\nDescription: {4}\nRange: {5}\nDuration: {6}\nEffect: {7}\nType:{8}", Id, Name, Template, Illustration, Description, Range, Duration, Effect, EffectType);
    }
}
