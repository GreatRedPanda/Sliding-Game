using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StepData
{
    public Cell PrevCell;
    public Stone Stone;

    public Cell NewCell;

}

public static class StepSaver
{



    public static List<StepData> PrevStates = new List<StepData>();
    public static int BackStepsLimit = 10;

    public static bool HasSteps { get { return PrevStates.Count > 0; } }


    public static void AddStep(StepData stepData)
    {




        PrevStates.Add(stepData);

        if (PrevStates.Count > BackStepsLimit)
            PrevStates.RemoveAt(0);



    }

    public static StepData BackStep(ref int steps)
    {

        StepData sd = null;
        
        if (PrevStates.Count != 0)
        {
            StepData data = PrevStates[PrevStates.Count - 1];
            PrevStates.RemoveAt(PrevStates.Count - 1);
           
            sd = data;
        }
        steps = PrevStates.Count;
        return sd;

    }

}
