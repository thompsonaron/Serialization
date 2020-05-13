using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using UnityEngine;

public class Tester : MonoBehaviour
{
    const string fileName = "AppSettings.dat";
    // Start is called before the first frame update
    void Start()
    {
        //BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create));
        Item item = new Item();
        item.amount = 2;
        item.name = "DItem";
        //writer.Write(item.name);
        //writer.Write(item.amount);
        //writer.Close();

        //  var s = new MemoryStream(10000);
        //  BinaryWriter binaryWriter = new BinaryWriter(s);
        //  string two = "two";
        //  binaryWriter.Write(two);
        ////  var blabla = new MemoryStream();
        //  //    s.CopyTo(blabla);
        //  ////binaryWriter.Write(item.amount);
        //  //binaryWriter.Close();
        //  //s.Close();

        //  BinaryReader binaryReader = new BinaryReader(s);
        //  string twoRead = binaryReader.ReadString();
        //  Debug.Log(twoRead);
        //  // Debug.Log(binaryReader.ReadInt32());
        //  //binaryReader.Close();

        //BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open));
        //string name = reader.ReadString();
        //int num = reader.ReadInt32();

        //Debug.Log(name);
        //Debug.Log(num);

        //Serializator ser = new Serializator();
        //var t = ser.serialize(item);


        //MemoryStream mm = new MemoryStream(t);

        //BinaryReader rr = new BinaryReader(mm);
        //string ff = rr.ReadString();
        //int aa = rr.ReadInt32();
        //Debug.Log(ff);
        //Debug.Log(aa);
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
            Debug.Log(obj.name);
        }

        //Debug.Log(resolved.amount + resolved.name);
        //Item sol = new Item();
        //var s = new MemoryStream(stream);
        //var bR = new BinaryReader(s);
        //var obj = new Item();
        //sol.name = bR.ReadString();
        //sol.amount = bR.ReadInt32();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
