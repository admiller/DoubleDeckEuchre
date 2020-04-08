using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Serializer : MonoBehaviour
{
    public static byte[] SerializeArrayList(ArrayList arrayList)
    {
        BinaryFormatter binaryF = new BinaryFormatter();
        using (MemoryStream memoryStream = new MemoryStream())
        {
            binaryF.Serialize(memoryStream, arrayList);

            return memoryStream.ToArray();
        }
    }

    public static ArrayList DeserializeArrayList(byte[] dataStream)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            BinaryFormatter binaryF = new BinaryFormatter();

            memoryStream.Write(dataStream, 0, dataStream.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return (ArrayList)binaryF.Deserialize(memoryStream);
        }
    }

    public static byte[] SerializeCard(Card card)
    {
        BinaryFormatter binaryF = new BinaryFormatter();
        using (MemoryStream memoryStream = new MemoryStream())
        {
            binaryF.Serialize(memoryStream, card);

            return memoryStream.ToArray();
        }
    }

    public static Card DeserializeCard(byte[] dataStream)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            BinaryFormatter binaryF = new BinaryFormatter();

            memoryStream.Write(dataStream, 0, dataStream.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return (Card)binaryF.Deserialize(memoryStream);
        }
    }

    public static byte[] SerializeDeck(Deck deck)
    {
        BinaryFormatter binaryF = new BinaryFormatter();
        using (MemoryStream memoryStream = new MemoryStream())
        {
            binaryF.Serialize(memoryStream, deck);

            return memoryStream.ToArray();
        }
    }

    public static Deck DeserializeDeck(byte[] dataStream)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            BinaryFormatter binaryF = new BinaryFormatter();

            memoryStream.Write(dataStream, 0, dataStream.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return (Deck)binaryF.Deserialize(memoryStream);
        }
    }

    public static byte[] SerializeTrick(Trick trick)
    {
        BinaryFormatter binaryF = new BinaryFormatter();
        using (MemoryStream memoryStream = new MemoryStream())
        {
            binaryF.Serialize(memoryStream, trick);

            return memoryStream.ToArray();
        }
    }

    public static Trick DeserializeTrick(byte[] dataStream)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            BinaryFormatter binaryF = new BinaryFormatter();

            memoryStream.Write(dataStream, 0, dataStream.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return (Trick)binaryF.Deserialize(memoryStream);
        }
    }

    public static byte[] SerializeBid(Bid bid)
    {
        BinaryFormatter binaryF = new BinaryFormatter();
        using (MemoryStream memoryStream = new MemoryStream())
        {
            binaryF.Serialize(memoryStream, bid);

            return memoryStream.ToArray();
        }
    }

    public static Bid DeserializeBid(byte[] dataStream)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            BinaryFormatter binaryF = new BinaryFormatter();

            memoryStream.Write(dataStream, 0, dataStream.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return (Bid)binaryF.Deserialize(memoryStream);
        }
    }

    public static byte[] SerializeBidList(BidList bidList)
    {
        BinaryFormatter binaryF = new BinaryFormatter();
        using (MemoryStream memoryStream = new MemoryStream())
        {
            binaryF.Serialize(memoryStream, bidList);

            return memoryStream.ToArray();
        }
    }

    public static BidList DeserializeBidList(byte[] dataStream)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            BinaryFormatter binaryF = new BinaryFormatter();

            memoryStream.Write(dataStream, 0, dataStream.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return (BidList)binaryF.Deserialize(memoryStream);
        }
    }
}
