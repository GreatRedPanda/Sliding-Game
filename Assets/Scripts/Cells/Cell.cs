using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;
public class Cell : MonoBehaviour, IPointerClickHandler
{
    public event System.Action<Cell> OnCellClick;
    public event System.Action<Cell> OnStoneOnCell;
    public Vector2Int Position;
    public Stone CurrentStone;
    public bool NeedStone;

    public Color OriginalColor;
    public Color CurrentColor;

    
    protected virtual void Start()
    {
        GameController.Instance.SubscribeToCellEvent(this);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        OnCellClick?.Invoke(this);
    }


    public virtual void SetStone(Stone s, bool gameStart=false)
    {

        CurrentStone = s;
        s.SetInteractable(true);
      
    }
    public virtual void TakeStone()
    {

        CurrentStone = null;
    }



    public virtual int GetCellStoneCount()
    {
        int res = 0;
        //res = (int)GenUtil.GetCellType(this);

        if(CurrentStone!=null)
        res +=  1;

        //if (NeedStone)
        //    res += 100;
        return res;

    }


    public virtual void Clear()
    {
        if (CurrentStone != null)
        {
            CurrentStone.ResetAnimation();
            CurrentStone = null;
        }
        //вернуть исходный цвет
    }


    public void SetColor(Color c)
    {
        if (c != Color.clear)
        {
            OriginalColor = c;
            GetComponent<Image>().color = OriginalColor;
        }
    }


    public virtual void StoneGotToCell()
    {
        OnStoneOnCell?.Invoke(this);
    }

    public virtual Cell CheckCell(Cell c, out bool hasObstacle)
    {
        hasObstacle = false;
   
        //if (cell.GetType() == typeof(CellPit))
        //{

        //    CellPit pit = map[x, y] as CellPit;
        //    if (pit.BottomStone == null)
        //    {
                
        //        hasObstacle = true;
        //        c = pit;
        //        return c;
        //    }
        //}
        if (CurrentStone == null)
            c = this;
        else
            hasObstacle = true;

        return c;
    }


}
