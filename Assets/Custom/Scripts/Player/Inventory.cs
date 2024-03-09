
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private Dictionary<string, ItemInfo> items = new();
    

    public void AddItem(IItem _item)
    {
        if (!items.ContainsKey(_item.Name))
        {
            items[_item.Name] = new ItemInfo
            {
                item = _item,
                count = 1
            };
        }
    }
    
    public ItemInfo GetItem(string _name)
    {
        return items[_name];
    }
}

public struct ItemInfo
{
    public IItem item;
    public int count;
}
