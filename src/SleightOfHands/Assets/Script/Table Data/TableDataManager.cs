using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TableDataManager
{
    private static readonly TableDataManager singleton = new TableDataManager();

    /// <summary>
    /// The unique instance
    /// </summary>
    public static TableDataManager Singleton
    {
        get
        {
            return singleton;
        }
    }

    private string path;
    private Dictionary<string, TableDataPage> dataPages;

    private TableDataManager()
    {
        path = Path.Combine(Application.streamingAssetsPath, "Tables\\");
        dataPages = new Dictionary<string, TableDataPage>();
    }

    /// <summary>
    /// Read all data from tables
    /// </summary>
    public void Initialize()
    {
        AddDataPageFromCSV<CardData>("Card");
    }

    /// <summary>
    /// Get data at a specific index in the given data page
    /// </summary>
    /// <typeparam name="T"> The type of the data </typeparam>
    /// <param name="pageName"> The name of the data page containing the desired data entry </param>
    /// <param name="index"> The index of the data entry to get </param>
    /// <returns></returns>
    public T GetData<T>(string pageName, int index)
    {
        return (T)dataPages[pageName][index];
    }

    private void AddDataPageFromCSV<T>(string pageName) where T : ITableDataEntry, new()
    {
#if UNITY_EDITOR
        Debug.Log(LogUtility.MakeLogString("TableDataManager", "Add " + pageName + " data"));
#endif

        TableDataPage dataPage = new TableDataPage();

        using (StreamReader file = new StreamReader(path + pageName + ".csv"))
        {
            int row = 0;

            for (string line = file.ReadLine(); line != null; line = file.ReadLine())
            {
                if (row > 0)
                {
                    T dataEntry = new T();

                    dataPage.AddEntry(dataEntry.Initialize(row, line.Split(',')), dataEntry);
                }

                row++;
            }
        }

        dataPages.Add(pageName, dataPage);
    }

    /// <summary>
    /// Get the data of a card with a given id
    /// </summary>
    /// <param name="id"> The id of the card to concern </param>
    /// <returns> A CardInfo containing the information of the card </returns>
    public CardData GetCardData(int id)
    {
        return GetData<CardData>("Card", id);
    }
}
