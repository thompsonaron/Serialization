using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using UnityEngine;

public class Tester : MonoBehaviour
{
    void Start()
    {
        Item item = new Item();
        item.amount = 2;
        item.name = "DItem";


        Serializator ser = new Serializator();

        Item item1 = new Item();
        item1.amount = 2;
        item1.name = "corn";
        Item item2 = new Item();
        item2.amount = 3;
        item2.name = "bread";
        Item item3 = new Item();
        item3.amount = 4;
        item3.name = "tro";
        Item item4 = new Item();
        item4.amount = 2;
        item4.name = "32";
        Item item5 = new Item();
        item5.amount = 41;
        item5.name = "fff";
        Inventory inv = new Inventory();
        inv.items = new Item[1];
        inv.items[0] = item1;
        inv.items2 = new List<Item>();
        inv.items2.Add(item2);
        inv.items2.Add(item3);
        inv.items2.Add(item4);
        inv.items2.Add(item5);

        var stream = ser.serialize(inv);

        Inventory resolved = ser.DeserializeInventory(stream);

        foreach (var obj in resolved.items2)
        {
            Debug.Log(obj.name+ " " + obj.amount);
        }



    }


}
