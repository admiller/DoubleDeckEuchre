using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable()]
public class BidList
{
    public List<Bid> bidList;

    public BidList()
    {
        bidList = new List<Bid>();
    }

    public void AddBid(Bid bid)
    {
        bidList.Add(bid);
    }

    public List<Bid> GetBids()
    {
        return bidList;
    }
}
