public class CardInfo : ITableDataEntry
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Template { get; private set; }
    public string Illustration { get; private set; }
    public string Description { get; private set; }
    public int Range { get; private set; }
    public int Duration { get; private set; }

    int ITableDataEntry.Initialize(int row, string[] stringData)
    {
        Id = int.Parse(stringData[0]);
        Name = stringData[1];
        Template = stringData[2];
        Illustration = stringData[3];
        Description = stringData[4];
        Range = int.Parse(stringData[5]);
        Duration = int.Parse(stringData[6]);

        return Id;
    }

    public override string ToString()
    {
        return string.Format("Id: {0}\nName: {1}\nTemplate: {2}\nIllustration: {3}\nDescription: {4}\nRange: {5}\nDuration: {6}", Id, Name, Template, Illustration, Description, Range, Duration);
    }
}
