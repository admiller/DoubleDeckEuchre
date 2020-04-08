using System;

public class Constants
{
    public const int Spades = 0;
    public const int Hearts = 1;
    public const int Clubs = 2;
    public const int Diamonds = 3;
    public const int High = 4;
    public const int Low = 5;
    public const int Pass = 6;

    public static string GetSuitText(int suit)
    {
        string ret = "";

        switch (suit)
        {
            case Spades:
                ret = "Spades";
                break;

            case Hearts:
                ret = "Hearts";
                break;

            case Clubs:
                ret = "Clubs";
                break;

            case Diamonds:
                ret = "Diamonds";
                break;
        }

        return ret;
    }

    public static string GetCardText(int cardNumber)
    {
        string ret = "";

        switch (cardNumber)
        {
            case 9:
                ret = "9";
                break;

            case 10:
                ret = "10";
                break;

            case 11:
                ret = "Jack";
                break;

            case 12:
                ret = "Queen";
                break;

            case 13:
                ret = "King";
                break;

            case 14:
                ret = "Ace";
                break;
                
        }

        return ret;
    }

    public static string GetTrumpText(int trump)
    {
        // If trump is one of the 4 suits, just use our other helper method
        string ret = GetSuitText(trump);

        // Trump is high or low
        if (string.IsNullOrEmpty(ret))
        {
            switch (trump)
            {
                case High:
                    ret = "High";
                    break;

                case Low:
                    ret = "Low";
                    break;
            }
        }

        return ret;
    }
}
