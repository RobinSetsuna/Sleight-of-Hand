using System;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ListMenu : UserInterface
{
    [SerializeField] private UIBoxColliderScaler boxCollider;
    [SerializeField] private UIList list;
    // private Dictionary<Button, UnityAction> callbacks = new Dictionary<Button, UnityAction>();
    
    public override void OnOpen(params object[] args)
    {
        int numArgs = args.Length;

        if (numArgs == 1 || numArgs % 2 == 0)
            throw new ArgumentException("Invalid number of arguments");

        GetComponent<RectTransform>().localPosition = (Vector3)args[0];

        RectTransform listTransform = list.GetComponent<RectTransform>();

        Button newListItem = listTransform.GetChild(0).GetComponent<Button>();
        int numExistedListItems = listTransform.childCount;

        // callbacks.Clear();

        int index = 0;
        for (int i = 1; i < args.Length; i += 2)
        {
            string label = (string)args[i];
            UnityAction callback = (UnityAction)args[i + 1];

            index = i / 2;

            Button listItem;

            if (index < numExistedListItems)
                listItem = listTransform.GetChild(index).GetComponent<Button>();
            else
                listItem = Instantiate(newListItem, listTransform);

            listItem.gameObject.SetActive(true);

            Transform listItemTransform = listItem.transform;
            listItemTransform.GetChild(0).GetComponent<Text>().text = label;

            listItem.onClick.RemoveAllListeners();
            listItem.onClick.AddListener(callback);
            listItem.onClick.AddListener(Close);

            // callbacks.Add(listItem, callback);
        }

        for (int i = index; i < numExistedListItems; i++)
            listTransform.GetChild(i).gameObject.SetActive(false);

        list.Refresh();
        boxCollider.Refresh();
    }
}
