using System.Collections.Generic;
using System.IO;
using System;
using System.Diagnostics;

public class Serializator
{
    public byte[] serialize(Datas datas)
    {
        var s = new MemoryStream();
        var bW = new BinaryWriter(s);
        bW.Write(datas.health);
        return s.ToArray();
    }
    public byte[] serialize(Inventory inventory)
    {
        var s = new MemoryStream();
        var bW = new BinaryWriter(s);
        bW.Write(inventory.items.Length);
        foreach (var item in inventory.items)
        {
            bW.Write(serialize(item));
        }
        bW.Write(inventory.items2.Count);
        foreach (var item in inventory.items2)
        {
            bW.Write(serialize(item));
        }
        s.Close();
        return s.ToArray();
    }
    public byte[] serialize(Item item)
    {
        var s = new MemoryStream();
        var bW = new BinaryWriter(s);
        bW.Write(item.name);
        bW.Write(item.amount);
        return s.ToArray();
    }
    public byte[] serialize(Player player)
    {
        var s = new MemoryStream();
        var bW = new BinaryWriter(s);
        bW.Write(player.hp);
        bW.Write(player.mana);
        bW.Write(player.arr);
        bW.Write(player.alive);
        bW.Write(serialize(player.inventory));
        bW.Write(player.randomThings.Count);
        foreach (var item in player.randomThings)
        {
            bW.Write(item);
        }
        bW.Write(player.rppl.Count);
        foreach (var item in player.rppl)
        {
            bW.Write(serialize(item));
        }
        bW.Write(player.intrrppl.Length);
        foreach (var item in player.intrrppl)
        {
            bW.Write(item);
        }
        return s.ToArray();
    }
    public byte[] serialize(Serializator serializator)
    {
        var s = new MemoryStream();
        var bW = new BinaryWriter(s);
        return s.ToArray();
    }
    public byte[] serialize(Tester tester)
    {
        var s = new MemoryStream();
        var bW = new BinaryWriter(s);
        return s.ToArray();
    }

    public Datas DeserializeDatas(byte[] b)
    {
        var s = new MemoryStream(b);
        var bR = new BinaryReader(s);
        var obj = new Datas();
        obj.health = bR.ReadInt32();
        return obj;
    }

    public Inventory DeserializeInventory(byte[] b)
    {
        var s = new MemoryStream(b);
        var bR = new BinaryReader(s);
        var obj = new Inventory();
        int itemsArraySize = bR.ReadInt32();
        obj.items = new Item[itemsArraySize];
        for (int i = 0; i < itemsArraySize; i++)
        {
            obj.items[i] = DeserializeItem(ref b, ref s,ref bR);
        }
        obj.items2 = new List<Item>();
        int items2ListSize = bR.ReadInt32();
        for (int i = 0; i < items2ListSize; i++)
        {
            obj.items2.Add(DeserializeItem(ref b, ref s, ref bR));
        }
        s.Close();
        bR.Close();
        return obj;
    }

    public Item DeserializeItem(byte[] b)
    {
        var s = new MemoryStream(b);
        var bR = new BinaryReader(s);
        var obj = new Item();
        obj.name = bR.ReadString();
        obj.amount = bR.ReadInt32();

        return obj;
    }

    public Item DeserializeItem(ref byte[] b, ref MemoryStream s, ref BinaryReader bR)
    {
        //s = new MemoryStream(b);
        //var bR = new BinaryReader(s);
        var obj = new Item();
        obj.name = bR.ReadString();
        obj.amount = bR.ReadInt32();

        return obj;
    }

    public Player DeserializePlayer(byte[] b)
    {
        var s = new MemoryStream(b);
        var bR = new BinaryReader(s);
        var obj = new Player();
        obj.hp = bR.ReadInt32();
        obj.mana = bR.ReadInt32();
        obj.arr = bR.ReadChar();
        obj.alive = bR.ReadBoolean();
        obj.inventory = DeserializeInventory(b);
        obj.randomThings = new List<Int32>();
        int randomThingsListSize = bR.ReadInt32();
        for (int i = 0; i < randomThingsListSize; i++)
        {
            obj.randomThings.Add(bR.ReadInt32());
        }
        obj.rppl = new List<Player>();
        int rpplListSize = bR.ReadInt32();
        for (int i = 0; i < rpplListSize; i++)
        {
            obj.rppl.Add(DeserializePlayer(b));
        }
        int intrrpplArraySize = bR.ReadInt32();
        obj.intrrppl = new Int32[intrrpplArraySize];
        for (int i = 0; i < intrrpplArraySize; i++)
        {
            obj.intrrppl[i] = bR.ReadInt32();
        }
        return obj;
    }

    public Serializator DeserializeSerializator(byte[] b)
    {
        var s = new MemoryStream(b);
        var bR = new BinaryReader(s);
        var obj = new Serializator();
        return obj;
    }

    public Tester DeserializeTester(byte[] b)
    {
        var s = new MemoryStream(b);
        var bR = new BinaryReader(s);
        var obj = new Tester();
        return obj;
    }

}
