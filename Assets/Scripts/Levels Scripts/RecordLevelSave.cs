using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecordData
{

    public int LevelID;

   
    public int MovesRecord;
}

public class RecordLevelSave: MonoBehaviour
{


    public static List<RecordData> recordsSaves = new List<RecordData>();


    public  void LoadRecord(string jsonRecordsData)
    {
     
        try
        {
            SerializableArray<RecordData> sr = JsonUtility.FromJson<SerializableArray<RecordData>>(jsonRecordsData);
            if (sr != null)
            {
                RecordData[] recordLevelSaves = sr.Array;

                recordsSaves = new List<RecordData>(recordLevelSaves);
            }
        }
        catch
        {
            recordsSaves = new List<RecordData>();
        }
        if (recordsSaves == null)
            recordsSaves = new List<RecordData>();
       // Debug.Log("RECORDS LOADING CALLLED"+recordsSaves +"sdf"+ new List<RecordLevelSave>());
    }

    public  string SaveRecords()
    {

        string data = JsonUtility.ToJson(new SerializableArray<RecordData>() { Array = recordsSaves.ToArray() });
   
        return data;
    }
    public  void AddRecord(LevelDescription levelDescription, int record, out bool isNewRecord)
    {
        isNewRecord = false;
        if (recordsSaves == null || recordsSaves.Count == 0)
        {
            isNewRecord = true;
            recordsSaves.Add(new RecordData()
            { LevelID = levelDescription.ID, MovesRecord = record }
          );
        }
        RecordData recordLevelSave = recordsSaves.Find(x => x.LevelID == levelDescription.ID);

        if (recordLevelSave != null)
        {
            if (recordLevelSave.MovesRecord > record)
            {
                recordLevelSave.MovesRecord = record;
                isNewRecord = true;
            }
        }
        else
        {
            isNewRecord = true;
            recordsSaves.Add(new RecordData()
            { LevelID = levelDescription.ID, MovesRecord = record }
            );
        }


  
    }


    public  int GetRecordByLevelID(LevelDescription levelDescription)
    {
        if (recordsSaves == null)
            return 0;
        RecordData recordLevelSave = recordsSaves.Find(x => x.LevelID == levelDescription.ID);

        if (recordLevelSave != null)
            return recordLevelSave.MovesRecord;

        else
            return 0;


    }

}
