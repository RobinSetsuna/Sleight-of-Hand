public class CardInfo : ITableDataEntry
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Template { get; private set; }
    public string Illustration { get; private set; }

    int ITableDataEntry.Set(int row, string[] stringData)
    {
        Id = int.Parse(stringData[0]);
        Name = stringData[1];
        Description = stringData[2];
        Template = stringData[3];
        Illustration = stringData[4];

        return Id;
    }
}
