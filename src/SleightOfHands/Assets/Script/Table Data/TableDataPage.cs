using System.Collections.Generic;

public class TableDataPage
{
    private Dictionary<int, object> entries;

    /// <summary>
    /// Get the data at a specific index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public object this[int index]
    {
        get
        {
            return entries[index];
        }
    }

    /// <summary>
    /// Construct an empty data page
    /// </summary>
    internal TableDataPage()
    {
        entries = new Dictionary<int, object>();
    }

    /// <summary>
    /// Add a entry at a given index
    /// </summary>
    /// <param name="index"> The index where to add the given data entry </param>
    /// <param name="entry"> The data entry to add </param>
    internal void AddEntry(int index, object entry)
    {
        entries.Add(index, entry);
    }
}
