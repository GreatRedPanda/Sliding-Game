using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = " New Level", menuName = "Level")]
public class LevelDescription : ScriptableObject
{
   
    public int ID;
    [SerializeField]
    public string Name;
    //public int MovesRecord;
    public string difficulty;

    public int W;
    public int H;
    [TextArea(3,10)]
    public string Map;
    [TextArea(3, 10)]
    public string MapStoneUpReserve;
    [TextArea(3, 10)]
    public string MapStoneLeftReserve;
    [TextArea(3, 10)]
    public string MapColor;



    int[,] mapInt;
    Color[,] mapColor;
    int[] leftReserve;
    int[] upReserve;

    public int[] GetUpReserve()
    {
    
        if (upReserve != null && upReserve.Length>0)
        {    
            return upReserve;
        }
        else
        {
            upReserve= GetReserveMap(MapStoneUpReserve);

            return upReserve;
        }
    }
    public int[] GetLeftReserve()
    {
        if (leftReserve != null && leftReserve.Length > 0)
        {
            return leftReserve;
        }
        else
        {
            leftReserve = GetReserveMap(MapStoneLeftReserve);
            return leftReserve;
        }
    }


    public int[] GetReserveMap(string mapAsStr)
    {
        

        int[] mapAsArray = null;
        try
        {
            mapAsArray = (JsonUtility.FromJson<SerializableArray<int>>(mapAsStr).Array);
        }
        catch
        {
            
        }


        int[] map = null;
        if (mapAsArray != null)
        {
            map = new int[mapAsArray.Length];
            for (int i = 0; i < mapAsArray.Length; i++)
            {
                map[i] =   mapAsArray[i];                   
                
            }
        }

    
        return map;

    }
    public int[,] GetMap()
    {
      
        if (mapInt != null)
        {
            return mapInt;
        }
        int[] mapAsArray = null;
        try
        {

            mapAsArray = (JsonUtility.FromJson<SerializableArray<int>>(Map).Array) ;
           

        }
        catch {
           

        }

      
        int[,] map = null;
        if (mapAsArray != null)
        {
          
            map = new int[W, H];
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    int num = mapAsArray[i * H + j] ;
                    map[i, j] = num;
                }
            }
        }

        mapInt = map;
        return map;

    }


    public Color[,] GetColorMap()
    {

        if (mapColor != null)
        {
            return mapColor;
        }
        string[] mapAsArray = null;
        try
        {

            mapAsArray = (JsonUtility.FromJson<SerializableArray<string>>(MapColor).Array);
           

        }
        catch
        {
           

        }


        Color[,] map = null;
        if (mapAsArray != null)
        {
           
            map = new Color[W, H];
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    Color c = new Color();
                    
                     bool success=   ColorUtility.TryParseHtmlString("#" + mapAsArray[i * H + j], out c);
                    if(success)
                        map[i, j]=c;
                
                }
            }
        }

        mapColor = map;
        return map;

    }
}


