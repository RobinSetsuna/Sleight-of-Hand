internal interface ITableDataEntry
{
    /// <summary>
    /// Initialize the data entry according to the parsed string data
    /// </summary>
    /// <param name="row"> The row of this entry in the original table </param>
    /// <param name="stringData"> The parsed string data used for initialization </param>
    /// <returns> The index of this entry in the accordant data page </returns>
    int Initialize(int row, string[] stringData);
}
