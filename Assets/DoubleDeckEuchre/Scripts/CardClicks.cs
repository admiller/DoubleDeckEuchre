using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ADM.DoubleDeckEuchre;

public class CardClicks : MonoBehaviour
{
    public GameObject card;
    public int index;
    float doubleClickStart = 0;

    void OnMouseUp()
    {
        if ((Time.time - doubleClickStart) < 0.4f)
        {
            this.OnDoubleClick();
            doubleClickStart = -1;
        }
        else
        {
            doubleClickStart = Time.time;
        }
    }

    void OnDoubleClick()
    {
        GameObject gameObject = GameObject.Find("GameManager");
        GameManager gameManager = gameObject.GetComponent<GameManager>();
        gameManager.OnCardDoubleClicked(index, gameObject);
    }
}
