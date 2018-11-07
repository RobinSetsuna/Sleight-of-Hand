public class CardInfo : ITableDataEntry
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Template { get; private set; }
    public string Illustration { get; private set; }
    public string Description { get; private set; }

    int ITableDataEntry.Set(int row, string[] stringData)
    {
        Id = int.Parse(stringData[0]);
        Name = stringData[1];
        Template = stringData[2];
        Illustration = stringData[3];
        Description = stringData[4];

        return Id;
    }

    public override string ToString()
    {
        return string.Format("[{0}] {1}: {2} (template: {3}; illustration: {4})", Id, Name, Description, Template, Illustration);
    }
}
