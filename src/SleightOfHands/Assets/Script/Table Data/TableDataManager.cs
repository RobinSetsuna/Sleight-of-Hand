using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TableDataManager
{
    private static readonly TableDataManager singleton = new TableDataManager();

    public static TableDataManager Singleton
    {
        get
        {
            return singleton;
        }
    }

    private string path;
    private Dictionary<string, TableDataPage> dataPages;

    public int NumDataPages
    {
        get
        {
            return dataPages.Count;
        }
    }

    private TableDataManager()
    {
        path = Path.Combine(Application.streamingAssetsPath, "Tables\\");
        dataPages = new Dictionary<string, TableDataPage>();

        AddDataPageFromCSV<CardInfo>("Card");
    }

    public void Initialize() { }

    public T GetData<T>(string pageName, int id)
    {
        return (T)dataPages[pageName][id];
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

                    dataPage.AddEntry(dataEntry.Set(row, line.Split(',')), dataEntry);
                }

                row++;
            }
        }

        dataPages.Add(pageName, dataPage);
    }
}
