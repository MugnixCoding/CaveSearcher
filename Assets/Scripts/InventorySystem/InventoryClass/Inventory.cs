using InventorySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public bool GetTresureYet;
    [SerializeField] public int maxSlotNumber = 4;

    public event EventHandler OnItemListChanged;
    private List<InventorySlot> itemList = new List<InventorySlot>();
    private void Start()
    {
        GetTresureYet = false;
    }

    public List<InventorySlot> GetItemList()
    {
        return itemList;
    }
    public ItemScriptableObject GetItem(int itemNumber)
    {
        return itemList[itemNumber].item;
    }
    public void AddItem(ItemScriptableObject item, int amount)
    {
        if (itemList.Count >= maxSlotNumber)
        {
            return;
        }
        if (item is IStackable)
        {
            //Debug.Log("堆疊道具");
            AddStackableItem((IStackable)item, amount);
        }
        else
        {
            //Debug.Log("增加道具");
            AddIndependentItem(item, amount);
        }
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    private void AddStackableItem(IStackable item, int amount)
    {
        bool itemAlreadyExist = false;
        int AddItemAmount = amount;
        foreach (InventorySlot InventoryItem in itemList)
        {
            if (!(InventoryItem.item is IStackable))
            {
                continue;
            }
            if (((ItemScriptableObject)item).id == InventoryItem.item.id)
            {
                Debug.Log("重複");
                if (item.stackLimit != 0)
                {
                    if (InventoryItem.amount + AddItemAmount > item.stackLimit)
                    {
                        InventoryItem.amount = item.stackLimit;
                        AddItemAmount = InventoryItem.amount + AddItemAmount - item.stackLimit;
                    }
                    else
                    {
                        InventoryItem.AddAmount(AddItemAmount);
                        itemAlreadyExist = true;
                        break;
                    }
                }
                else
                {
                    InventoryItem.AddAmount(AddItemAmount);
                    itemAlreadyExist = true;
                    break;
                }
            }
        }
        if (!itemAlreadyExist)
        {
            itemList.Add(new InventorySlot((ItemScriptableObject)item, AddItemAmount));
        }
    }

    private void AddIndependentItem(ItemScriptableObject item, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            itemList.Add(new InventorySlot(item, amount));
        }
    }

    public void SubItem(ItemScriptableObject item)
    {
        //TODO discard item
    }
}
public class InventorySlot
{
    public ItemScriptableObject item;
    public int amount;
    public InventorySlot(ItemScriptableObject item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
    public void AddAmount(int value)
    {
        amount += value;
    }
}
