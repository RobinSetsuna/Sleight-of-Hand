using System.Collections.Generic;

public class TableDataPage
{
    private Dictionary<int, object> entries;

    public object this[int id]
    {
        get
        {
            return entries[id];
        }
    }

    internal TableDataPage()
    {
        entries = new Dictionary<int, object>();
    }

    internal void AddEntry(int id, object entry)
    {
        entries.Add(id, entry);
    }
}
