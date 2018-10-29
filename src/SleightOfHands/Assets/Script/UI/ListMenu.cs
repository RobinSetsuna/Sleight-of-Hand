using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ListMenu : UserInterface
{
    [SerializeField] private Transform list;
    private Dictionary<Button, UnityAction> callbacks;
    
    public override void OnOpen(params object[] args)
    {
        int numArgs = args.Length;

        if (numArgs == 1 || numArgs % 2 == 0)
            throw new ArgumentException("Invalid number of arguments");

        Vector3 position = (Vector3)args[0];

        Button newListItem = list.GetChild(0).GetComponent<Button>();
        int numExistedListItems = list.childCount;

        for (int i = 1; i < args.Length; i += 2)
        {
            string label = (string)args[i];
            UnityAction callback = (UnityAction)args[i + 1];

            int index = i / 2;

            Button listItem;

            if (index < numExistedListItems)
                listItem = list.GetChild(index).GetComponent<Button>();
            else
                listItem = Instantiate(newListItem, list);

            Transform listItemTransform = listItem.transform;
            listItemTransform.GetChild(0).GetComponent<Text>().text = label;
            // TODO: Reposition

            listItem.onClick.RemoveAllListeners();
            listItem.onClick.AddListener(callback);

            callbacks.Add(listItem, callback);
        }

        // TODO: Resizing background
    }
}
