using UnityEngine;
using System.Collections;

public class CellPit : Cell
{

    public Stone BottomStone;

  
    public override void SetStone(Stone s, bool gameStart=false)
    {
        if (BottomStone == null)
        {
            BottomStone = s;
            s.SetInteractable(false);
        }
        else
        {
            CurrentStone = s;
            s.SetInteractable(true);
        }
    }

    public override void TakeStone()
    {

        if (CurrentStone != null)
            CurrentStone = null;
        else if (BottomStone != null)
            BottomStone = null;
         
    }


    public override int GetCellStoneCount()
    {
        int res = 0;
     //   res = (int)GenUtil.GetCellType(this);

        if (BottomStone != null)
            res += 1;
        if (CurrentStone != null)
            res += 1;

     //   if (NeedStone)
           // res += 100;
        return res;
    }
    public override void Clear()
    {
        base.Clear();
        if (BottomStone != null)
        {
            BottomStone.SetInteractable(true);
          BottomStone.ResetAnimation();
        }
        BottomStone = null;
    }



    public override Cell CheckCell(Cell c, out bool hasObstacle)
    {
        hasObstacle = false;
                 
            if (BottomStone == null)
            {

                hasObstacle = true;
                c = this;
                return c;
            }
        
        if (CurrentStone == null)
            c = this;
        else
            hasObstacle = true;

        return c;
    }
}
