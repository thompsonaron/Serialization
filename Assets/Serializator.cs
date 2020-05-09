using System.Collections.Generic;
using System.IO;


public class Serializator
{
    byte[] serialize(Datas datas)
    {
        var s = new MemoryStream();
        var bW = new BinaryWriter(s);
        bW.Write(datas.health);
        return s.ToArray();
    }
    byte[] serialize(Inventory inventory)
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
        return s.ToArray();
    }
    byte[] serialize(Item item)
    {
        var s = new MemoryStream();
        var bW = new BinaryWriter(s);
        bW.Write(item.name);
        bW.Write(item.amount);
        return s.ToArray();
    }
    byte[] serialize(Player player)
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
        return s.ToArray();
    }
}
