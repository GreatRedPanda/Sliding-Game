using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;



interface IStoneTargetCell
    {

    bool IsFilled { get;  }
         Image TargetImage { get; set; }
         Color StoneOnCell { get; set; }
         Color CellWithoutStone { get; set; }
    }

