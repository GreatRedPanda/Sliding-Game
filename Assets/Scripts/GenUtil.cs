using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GenUtil
{
    static Cell cellPrefab;
    static Cell cellSolvePartPrefab;
    static Stone stonePrefab;
    static Cell containerPrefab;
    static CellPit cellPit;
    static CellPit cellPitSolvePartPrefab;

    public static Vector2 CellSize;
    public static Vector2 CellOffset;
    public static Vector2 StartPosition;

    public static Vector2 ParentSize;
    public static Queue<Stone> AllStones =new Queue<Stone>();


    public static float StoneSpeed;
    public static GameObject Parent;


    public static Vector3 Scale { get { return Parent.transform.localScale; } }
    public static void SetPrefabs(Cell cellPref,Stone stonePref,CellContainer containerPref, CellPit cellPitPref, Cell cellSolvePartPref, CellPit cellPitSolvePartPref)
    {

        cellSolvePartPrefab = cellSolvePartPref;
        cellPitSolvePartPrefab = cellPitSolvePartPref;
           cellPrefab = cellPref;
        stonePrefab = stonePref;
        containerPrefab = containerPref;
        cellPit = cellPitPref;
        Stone.PrefabSize = stonePref.GetComponent<RectTransform>().rect.size;


    }
    public static void CountParameters(GameObject parent, int W, int H, Vector2 gapPercent, float  stoneDeltaTime )
    {
        

        RectTransform rt = parent.GetComponent<RectTransform>();
        RectTransform rtParent=parent.transform.parent.GetComponent<RectTransform>();

       
        rt.sizeDelta = rtParent.rect.size;
        rt.anchoredPosition = Vector2.zero;

        Vector2 size = rt.rect.size;


        gapPercent /= 100;
        Vector2 gap = new Vector2(gapPercent.x * size.x, gapPercent.y * size.y) ;
        size.x -= gap.x * 2;
        size.y -= gap.y * 2;

        ParentSize = size;



            float parentWidth = size.x / (W + 1);
            float parentHeight = size.y / (H + 1);

            float min = (parentHeight < parentWidth) ? parentHeight : parentWidth;
           CellSize= new Vector2((min), (min));

       // Stone.Speed = min / stoneDeltaTime;
        Parent = parent;
        StoneSpeed = min / stoneDeltaTime;
        Vector2 startPos = size / 2;
        Vector2 actualSize = new Vector2(W + 1, H + 1) * min;
        Vector2 left = size - actualSize;
        left /= 2;
        StartPosition = left / size+gapPercent;
        rt.pivot = StartPosition;




    }

 
    public static Cell[,] GenerateLevel(GameObject parent,
        LevelDescription level,  SaveData saveData=null)
    {
    
       AllStones.Clear();
        Color[,] colorMap = level.GetColorMap();
        int[,] mapData = level.GetMap();
        if (mapData == null)
            return null;
        Cell[,]  map = new Cell[mapData.GetLength(0), mapData.GetLength(1)];

        for (int i = 0; i < mapData.GetLength(0); i++)
        {
            for (int j = 0;j < mapData.GetLength(1); j++)
            {
            
                int needStone = 0;
                int hasStone = 0;
                int type = 1;

                GetCellSaveData(mapData[i, j], out type, out hasStone, out needStone);

                if (saveData != null)
                    hasStone = saveData.Map[i, j];



                Cell prefab = GetCellPrefabByType(type);
                Cell cell = InstantiateCell(parent, prefab, new Vector2Int(i, j), new Vector2((i + 1) , j ));     
                map[i, j] = cell;
            
                for (int k = 0; k < hasStone; k++)
                {
                   InstantiateStone(cell, stonePrefab);
                }
                if (colorMap != null)
                    cell.SetColor(colorMap[i, j]);
                    
            }
        }


     


        return map;
    }

public static Cell[] GenerateReserveUp(GameObject parent,
        LevelDescription level, SaveData saveData = null)
    {

        int[] reserve = level.GetUpReserve();
        
        if (reserve == null)
            return null;
        Cell[] upReserve = new Cell[level.W];
        for (int i = 0; i < level.W; i++)
        {
            int count = reserve[i];
            if (saveData != null)
                count = saveData.MapContainerUp[i];
            CellContainer cell = InstantiateCell(parent, containerPrefab, new Vector2Int(i, level.H), new Vector2((i + 1), level.H)) as CellContainer;
            upReserve[i] = cell;
            for (int k = 0; k < count; k++)
            {
                InstantiateStone(cell, stonePrefab);
            }
        }
        return upReserve;
    }

 public static Cell[] GenerateReserveLeft(GameObject parent,
       LevelDescription level, SaveData saveData = null)
    {

        int[] reserve = level.GetLeftReserve();
        if (reserve == null)
            return null;
        Cell[] leftReserve = new Cell[level.H];

        for (int j = 0; j < level.H; j++)
        {
            int count = reserve[j];
            if (saveData != null)
                count = saveData.MapContainerLeft[j];
            CellContainer cell = InstantiateCell(parent, containerPrefab, new Vector2Int(-1, j), new Vector2((0), j)) as CellContainer;
            leftReserve[j] = cell;
            for (int k = 0; k < count; k++)
            {
                InstantiateStone(cell, stonePrefab);
            }
        }

        return leftReserve;

    }

  static Cell GetCellPrefabByType(int type)
    {

        CellType typeC = (CellType)type;

        switch (typeC)
        {
            case CellType.ordinary: return cellPrefab;
            case CellType.pit: return cellPit;
            case CellType.picPart: return cellSolvePartPrefab;
            case CellType.picPit: return cellPitSolvePartPrefab;

        }
        return cellPrefab;
    }

 public   static CellType GetCellType( Cell cell)
    {

        CellType typeC = CellType.ordinary;


        if (cell.NeedStone)
            typeC = CellType.picPart;

        if (cell.GetType() == typeof(CellPit))
        {

            typeC = CellType.pit;
            if (cell.NeedStone)
                typeC = CellType.picPit;
        }
        if (cell.GetType() == typeof(CellContainer))
        {

            typeC = CellType.container;
           
        }

        return typeC;
    }


    static Cell InstantiateCell(GameObject parent, Cell prefab, Vector2Int mapPos, Vector2 worldPos)
    {
        Cell cell = GameObject.Instantiate(prefab);
        cell.transform.SetParent(parent.transform);
        RectTransform rect = cell.GetComponent<RectTransform>();
        cell.transform.localScale = Vector3.one;
        cell.transform.localPosition = worldPos * CellSize;
        rect.sizeDelta = CellSize;

        //rect.anchorMin = worldPos * (CellSize / ParentSize);
        //rect.anchorMax = (worldPos + Vector2.one) * (CellSize / ParentSize);
        //rect.offsetMin = Vector2.zero;
        //rect.offsetMax = Vector2.zero;


        cell.Position = mapPos;

        return cell;
    }
    static Stone InstantiateStone(Cell c, Stone sPref)
    {
        Stone s = GameObject.Instantiate(sPref);
        
       
        s.transform.SetParent(c.transform);
        RectTransform rect = s.GetComponent<RectTransform>();
        s.transform.localScale = Vector3.one;
        s.transform.localPosition = Vector2.zero;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        c.SetStone(s, true);
        AllStones.Enqueue(s);
        return s;
    }

    static Stone SetStone(Cell c)
    {
       
        Stone s = AllStones.Dequeue();

       
        s.gameObject.SetActive(true);
        s.transform.SetParent(c.transform);
        RectTransform rect = s.GetComponent<RectTransform>();
        s.transform.localScale = Vector3.one;
        s.transform.localPosition = Vector2.zero;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
       c.SetStone(s, true);
        AllStones.Enqueue(s);
        return s;
    }


    public static void Replay(LevelDescription level, Cell[,] map, Cell[] left, Cell[] up)
    {
        int[,] mapData = level.GetMap();
        if (mapData == null)
            return;
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                map[i, j].Clear();

               
                int needStone = 0;
                int hasStone = 0;
                int type = 1;

                GetCellSaveData(mapData[i, j], out type, out hasStone, out needStone);
                for (int k = 0; k < hasStone; k++)
                {
                    SetStone(map[i, j]);
                }

            }
        }
        int[] reserveLeft = level.GetLeftReserve();
        if (reserveLeft == null)
            return;
        int[] reserveUp = level.GetUpReserve();
        if (reserveLeft == null)
            return;
        for (int i = 0; i < left.Length; i++)
        {
            left[i].Clear();

            int count = reserveLeft[i];
            for (int k = 0; k < count; k++)
            {
                SetStone(left[i]);
            }
        }
        for (int i = 0; i < up.Length; i++)
        {
            up[i].Clear();
            int count = reserveUp[i];
            for (int k = 0; k < count; k++)
            {
                SetStone(up[i]);
            }
        }
    }




    public static void GetCellSaveData(int data, out int type, out int stoneCount, out int needStone)
    {
        needStone = 0;
        stoneCount = 0;
        type = 1;
        needStone = data / 100;
        stoneCount = (data % 100) / 10;
        type = data % 10;


    }




    public static void Resize(Cell[,] map, Cell[] left, Cell[] up, GameObject parent, int W, int H, Vector2 gapPercent, float stoneDeltaTime)
    {

        CountParameters(parent, W, H, gapPercent, stoneDeltaTime);

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {

                RectTransform rect = map[i,j].GetComponent<RectTransform>();

                map[i, j].transform.localPosition = (map[i, j].Position+Vector2.right) * CellSize;
                rect.sizeDelta = CellSize;
           

            }
        }

        for (int i = 0; i < left.Length; i++)
        {

            RectTransform rect = left[i].GetComponent<RectTransform>();

            left[i].transform.localPosition = (left[i].Position + Vector2.right) * CellSize;
            rect.sizeDelta = CellSize;
        }
        for (int i = 0; i < up.Length; i++)
        {

            RectTransform rect = up[i].GetComponent<RectTransform>();

            up[i].transform.localPosition = (up[i].Position + Vector2.right) * CellSize;
            rect.sizeDelta = CellSize;
        }


    }


}

