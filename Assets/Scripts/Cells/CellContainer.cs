using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;
using TMPro;
public class CellContainer: Cell
{

   public    TextMeshProUGUI countText;
    List<Stone> reserveStone;



    protected override void Start()
    {
        base.Start();
        countText = GetComponentInChildren<TextMeshProUGUI>();
        if (reserveStone == null || reserveStone.Count<=0)
        countText.gameObject.SetActive(false);
        countText.transform.SetAsLastSibling();
    }


    public override int GetCellStoneCount()
    {
        if (reserveStone == null)
            return 0;
        return reserveStone.Count;
    }



    public void GetStoneFromReserve()
    { }

    public override void StoneGotToCell()
    {
        base.StoneGotToCell();
       // if (gameStart)
        {
            if (!countText.gameObject.activeSelf && reserveStone.Count > 0)
                countText.gameObject.SetActive(true);
            countText.text = reserveStone.Count.ToString();
            countText.transform.SetAsLastSibling();
        }
    }
    public override void SetStone(Stone s, bool gameStart = false)
    {
            if (reserveStone == null)
               reserveStone = new List<Stone>();

        if (reserveStone.Count > 0)
        {
            if (gameStart)
                reserveStone[reserveStone.Count - 1].gameObject.SetActive(false);
            else
                reserveStone[reserveStone.Count - 1].Appear(false);
        }
        reserveStone.Add(s);
        CurrentStone = reserveStone[reserveStone.Count - 1];
        CurrentStone.SetInteractable(true);

        if (gameStart)
        {
            if (!countText.gameObject.activeSelf && reserveStone.Count > 0)
                countText.gameObject.SetActive(true);
            countText.text = reserveStone.Count.ToString();
            countText.transform.SetAsLastSibling();
        }
    }

    public override void TakeStone()
    {
        Stone s = CurrentStone;
        reserveStone.Remove(CurrentStone);
        if (reserveStone.Count > 0)
        {

         
            CurrentStone = reserveStone[reserveStone.Count - 1];
            CurrentStone.Appear(true);
        }
        else
            CurrentStone = null;
        if (reserveStone.Count<=0)
            countText.gameObject.SetActive(false);
        else
        countText.text = reserveStone.Count.ToString();
        countText.transform.SetAsLastSibling();
    }

    public override void Clear()
    {
        if(reserveStone!=null)
        reserveStone.Clear();
    }
}

