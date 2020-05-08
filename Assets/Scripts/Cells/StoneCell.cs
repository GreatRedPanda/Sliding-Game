using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;

public  class StoneCell: Cell, IStoneTargetCell
    {
    [SerializeField]
    private Image targetImage;
    [SerializeField]
    Color stoneOnCell;
    [SerializeField]
    Color cellWithoutStone;

    public bool IsFilled { get => CurrentStone != null; }
    public Image TargetImage { get => targetImage; set => targetImage=value; }
    public Color StoneOnCell { get => stoneOnCell; set => stoneOnCell = value; }
    public Color CellWithoutStone { get => cellWithoutStone; set => cellWithoutStone = value; }



    protected override void Start()
    {
        base.Start();
        if(CurrentStone==null)
        TargetImage.color = CellWithoutStone;
        else
            TargetImage.color = StoneOnCell;
    }
    public override void SetStone(Stone s, bool gameStart = false)
    {
        base.SetStone(s, gameStart);
        TargetImage.color = StoneOnCell;

    }

    public override void TakeStone()
    {
        base.TakeStone();
        TargetImage.color = CellWithoutStone;
    }
    public override void Clear()
    {
        base.Clear();
        TargetImage.color = CellWithoutStone;
    }
}

