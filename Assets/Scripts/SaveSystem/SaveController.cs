using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class SaveData
{
    public int LevelID;
    public string LevelName;
    public int[] MapContainerLeft;
    public int[] MapContainerUp;
    public int[,] Map;
    public int MovesCount;
}






public class SaveController : MonoBehaviour
{

#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void SyncFiles();

    [DllImport("__Internal")]
    private static extern void WindowAlert(string message);
#endif
    public string FileName = "saves.svs";
    public static List<SaveData> saves = new List<SaveData>();


    public void DeleteSave(int levelId)
    {

        int index = saves.FindIndex(x => x.LevelID == levelId);
       
        if (index != -1)
            saves.RemoveAt(index);
       

    }
    public SaveData GetSave(int levelId, string levelName)
    {
        int index = saves.FindIndex(x => x.LevelID == levelId);

        if (index == -1)
            return null;
        else
            return saves[index];

    }
    public void CreateSave(int levelId, string levelName, Cell[,] map, Cell[] leftContainers, Cell[] upContainer, int movesCount)
    {
        SaveData newSD = new SaveData();
        newSD.LevelID = levelId;
        newSD.LevelName = levelName;

        newSD.MovesCount = movesCount;
       newSD.MapContainerLeft = new int[leftContainers.Length];
        for (int i = 0; i < newSD.MapContainerLeft.Length; i++)
        {
            newSD.MapContainerLeft[i] = leftContainers[i].GetCellStoneCount();
        }

      
        newSD.MapContainerUp = new int[upContainer.Length];
        for (int i = 0; i < newSD.MapContainerUp.Length; i++)
        {
            newSD.MapContainerUp[i] = upContainer[i].GetCellStoneCount();
        }

        newSD.Map = new int[map.GetLength(0), map.GetLength(1)];
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                newSD.Map[i, j] = map[i, j].GetCellStoneCount();
            }
        }
        int index = saves.FindIndex(x => x.LevelID == levelId);

        if (index != -1)
        {

            saves.RemoveAt(index);
        }
        
        {

            saves.Add(newSD);
        }


    }

    public void SaveData()
    {

        BinaryFormatter bf = new BinaryFormatter();


//#if !UNITY_WEBGL
        string path = getFilePath();
        if (File.Exists(path))
        {
            FileStream file = File.OpenWrite(path);
            bf.Serialize(file, saves); 
            file.Close();
        }
        else
        {
            FileStream file = File.Create(path);
            bf.Serialize(file, saves);
            file.Close();
        }

#if UNITY_WEBGL && !UNITY_EDITOR
            SyncFiles();
#endif
        //#else
        //        MemoryStream stream = new MemoryStream();
        //        bf.Serialize(stream, saves);
        //        string data = System.Convert.ToBase64String(stream.GetBuffer());
        //        PlayerPrefs.SetString(FileName, data);
        //        PlayerPrefs.Save();

        //        //string data = PlayerPrefs.GetString(FileName);
        //        //byte[] bytes = Encoding.ASCII.GetBytes(data);
        //        //MemoryStream stream = new MemoryStream(bytes);      
        //        //List<SaveData> saves1= (List<SaveData>)bf.Deserialize(stream);


        //#endif

    }



    public void LoadData()
    {

   BinaryFormatter bf = new BinaryFormatter();

//#if !UNITY_WEBGL

        string path = getFilePath();
        if (File.Exists(path))
        {
            FileStream file = File.Open(path, FileMode.Open);
            saves = (List<SaveData>)bf.Deserialize(file);
         
            file.Close();         
        }


#if UNITY_WEBGL && !UNITY_EDITOR
            SyncFiles();
#endif
        //#else

        //        if (PlayerPrefs.HasKey(FileName))
        //        {
        //            try { 
        //            string data = PlayerPrefs.GetString(FileName);

        //            byte[] bytes = System.Convert.FromBase64String(data);
        //            MemoryStream stream = new MemoryStream(bytes);
        //            saves = (List<SaveData>)bf.Deserialize(stream);
        //             }
        //            catch
        //            { }
        //        }

        //#endif
    }




    string getFilePath()
    {
//        string path = Application.dataPath + "\\" + FileName;
//#if UNITY_ANDROID
//        path = "jar:file://" + Application.dataPath + "!/assets/" + FileName;

//#endif

        string filePath = Path.Combine(Application.persistentDataPath, FileName);
        return filePath;

    }

    //public IEnumerator Load()
    //{

    //    string filePath = getFilePath();
    //    UnityWebRequest unityWebRequest = UnityWebRequest.Get(filePath);
    //    yield return unityWebRequest.SendWebRequest();

    //   string data = unityWebRequest.downloadHandler.text;

    //    byte[] data1 = unityWebRequest.downloadHandler.data ;


    //    Debug.Log(data1.Length +"   "+unityWebRequest.uri);
    //    //string data1 = unityWebRequest.downloadHandler.text;

    //    //if (data != null && !(data.Length < 0))
    //    //{
    //        BinaryFormatter bf = new BinaryFormatter();
    //        var stream = new MemoryStream(data1);
    //   // var ds = new DeflateStream(stream, CompressionMode.Decompress, false);

    //    List<SaveData> saves1= (List<SaveData>)bf.Deserialize(stream);

    //    Debug.Log(data1.Length+"   "+saves1.Count +"  sa"+saves.Count);
    //    //    saves = (List<SaveData>)bf.Deserialize(stream);
    //    //}

    //}


    //public IEnumerator Save()
    //{



    //    string path = "file://" + Path.Combine(Application.persistentDataPath, "saves1.svs");


    //   BinaryFormatter bf = new BinaryFormatter();
    //    MemoryStream stream = new MemoryStream();
    //    bf.Serialize(stream, saves);
    //    byte[] data = stream.ToArray();



    //    UnityWebRequest unityWebRequest = new UnityWebRequest(path, "POST");
    //    unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer() ;
    //    unityWebRequest.uploadHandler = (UploadHandler) new UploadHandlerRaw(data);

    //    //unityWebRequest.SetRequestHeader("Content-typw", "application/svs");
    //    yield return unityWebRequest.SendWebRequest();

    //    Debug.Log(unityWebRequest.error + $"   {   unityWebRequest.uri}  \n  "+ unityWebRequest.method);




    //}

}
