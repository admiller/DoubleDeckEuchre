using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable()]
public class Bid 
{
    public int suitNumber;
    public int trickNumber;
    public int seatNumber;
    public string suitName;

    private Bid(int _trickNumber, int _suitNumber, string _suitName, int _seatNumber)
    {
        this.trickNumber = _trickNumber;
        this.suitNumber = _suitNumber;
        this.suitName = _suitName;
        this.seatNumber = _seatNumber;
    }

    public static Bid Construct(int _number, string _suitName, int _seatNumber)
    {
        int suitNum = -1;

        if (_suitName.Equals("Spades"))
        {
            suitNum = Constants.Spades;
        }
        else if (_suitName.Equals("Hearts"))
        {
            suitNum = Constants.Hearts;
        }
        else if (_suitName.Equals("Clubs"))
        {
            suitNum = Constants.Clubs;
        }
        else if (_suitName.Equals("Diamonds"))
        {
            suitNum = Constants.Diamonds;
        }
        else if (_suitName.Equals("High"))
        {
            suitNum = Constants.High;
        }
        else if (_suitName.Equals("Low"))
        {
            suitNum = Constants.Low;
        }
        else if (_suitName.Equals("Pass"))
        {
            suitNum = Constants.Pass;
        }

        return new Bid(_number, suitNum, _suitName, _seatNumber);
    }
}
