using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class GameController : MonoBehaviour
{
    public event System.Action<int> OnMove;
    public event System.Action<int, int> OnMapChanged;
    public event System.Action<int, int, bool> OnVictory;
  public event System.Action<int> OnStepMade;


    public Vector2 Gap;
    public GameObject Parent;
    public Cell CellPrefab;
    public CellContainer CellContainerPrefab;
    public CellPit CellPitPrefab;

    public StoneCell CellSolvePartPrefab;
    public StoneCellPit CellPitSolvePartPrefab;
    public Stone StonePrefab;
    public LevelDescription CurrentLevel;

    public float StoneDeltaTime = 5f;


    Stone SelectedStone;

    bool interactable = true;
    public static GameController Instance;

    Cell[,] map;// p=new Cell[,];

    Cell[] upReserve;
    Cell[] leftReserve;

    bool victory = false;
    int movesCount = 0;
    int targetCellsCount = 0;
    int currentCellsCount = 0;




    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    SaveController saveController;
    RecordLevelSave RecordLevelSave;
    private void Start()
    {

        RecordLevelSave = FindObjectOfType<RecordLevelSave>();
        LevelUIController levelUIController = FindObjectOfType<LevelUIController>();
        if (levelUIController != null)
        {
            OnMapChanged += levelUIController.SetStoneCells;
            OnMove += levelUIController.SetMovesCount;
            OnVictory += levelUIController.Win;
            OnStepMade += levelUIController.SetBack;
            levelUIController.SetLevelName(CurrentLevel.Name);
            if(RecordLevelSave!=null)
            levelUIController.SetMovesRecord(RecordLevelSave.GetRecordByLevelID(CurrentLevel));
        }

        saveController = FindObjectOfType<SaveController>();
        SaveData sd = null;
        if (saveController != null)
        {
            sd = saveController.GetSave(CurrentLevel.ID, CurrentLevel.Name);
            if (sd != null)
            {
                movesCount = sd.MovesCount;
            }
            OnMove?.Invoke(movesCount);
        }

        GenUtil.SetPrefabs(CellPrefab, StonePrefab, CellContainerPrefab, CellPitPrefab, CellSolvePartPrefab, CellPitSolvePartPrefab);
        GenUtil.CountParameters(Parent, CurrentLevel.W, CurrentLevel.H, Gap, StoneDeltaTime);
         map=  GenUtil.GenerateLevel(Parent,CurrentLevel,  sd);

        upReserve = GenUtil.GenerateReserveUp(Parent, CurrentLevel,  sd);
        leftReserve = GenUtil.GenerateReserveLeft(Parent, CurrentLevel,  sd);

        GetTargetCellsCount();
        if (levelUIController != null)
        {
            levelUIController.SetStonesTotal(GenUtil.AllStones.Count);
        }
    }

    public int GetTargetCellsCount()
    {


        int c = 0;

        IEnumerable<IStoneTargetCell> res =    map.OfType<IStoneTargetCell>();
        IStoneTargetCell[] res1= res.ToArray();
        c = res1.Length;
        targetCellsCount = c;

        IEnumerable<IStoneTargetCell> res2 = res.Where(x=>x.IsFilled);
        IStoneTargetCell[] res21 = res2.ToArray();
      
        currentCellsCount = res21.Length;
        OnMapChanged?.Invoke(res21.Length, c);

        return c;

    }
    public void SubscribeToCellEvent(Cell c)
    {

        c.OnCellClick += onCellClick;
        c.OnStoneOnCell += onStoneOnCell;
    }

    public void SubscribeToStoneEvent(Stone s)
    {
        s.OnStoneClick += onStoneClick;
        s.OnStoneMoving += onStoneMove;

    }




    void onStoneOnCell(Cell c)
    {
     
        IEnumerable<IStoneTargetCell> res2 = map.OfType<IStoneTargetCell>().Where(x => x.IsFilled);
        IStoneTargetCell[] res21 = res2.ToArray();
        currentCellsCount = res21.Length;

        if (currentCellsCount == targetCellsCount)
            Victory();
     
       OnMapChanged?.Invoke(currentCellsCount, targetCellsCount);
    }
    void onStoneMove(Stone s, bool isMoving)
    {
        interactable = !isMoving;

    }
    void onStoneClick(Stone s)
    {

        


       
        if (SelectedStone != null)
        {
            SelectedStone.Selected(false);

            if (SelectedStone == s)
            {
                SelectedStone = null;
                return;
            }
        }
    
        SelectedStone = s;
        SelectedStone.Selected(true);
    }

    void onCellClick(Cell c)
    {

        Debug.Log("OnCellClick");
        if (c.GetType() == typeof(CellContainer))
            return;
        if (SelectedStone != null)
        {
            if (!SelectedStone.Interactable  || !interactable)
                return;
            Cell stoneCell = SelectedStone.CellParent;


            int side = -1;

            Vector2Int diff = stoneCell.Position - c.Position;
    

            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            {
                if (diff.x > 0)
                    side = 0;
                else
                    side = 1;
            }
            else
            {
                if (diff.y > 0)
                    side = 2;
                else
                    side = 3;
            }

            if (stoneCell.GetType() == typeof(CellContainer))
            {
                if (stoneCell.Position.x < 0)
                    side = 1;
                else
                    side = 2;
            }

            Cell targetCell = GetTargetCell(stoneCell.Position, side);
           // Debug.Log("GetTargetCell");
            SetCellToStone(stoneCell, targetCell, SelectedStone);
           // Debug.Log("SetCellToStone");
            StepSaver.AddStep(new StepData() { PrevCell = stoneCell, Stone = SelectedStone, NewCell = targetCell });
            OnStepMade?.Invoke(1);
            //SelectedStone.Selected(false);
            //SelectedStone = null;
            // Debug.Log("cell cick finish");

           // "".Replace("", "d");
        }



    }

    void SetCellToStone(Cell prevCel, Cell newCell, Stone s, bool isBack=false)
    {


        if (newCell != null)
        {
            if (!isBack)
                movesCount++;
            else
                movesCount--;

   OnMove?.Invoke(movesCount);
            bool inPit = !s.Interactable;


            newCell.SetStone(s);

            prevCel.TakeStone();

            Debug.Log(" s.StartMove start");
            s.StartMove(newCell.transform, inPit,isBack);
            Debug.Log(" s.StartMove finish");




        }
    }


    public void Back()
    {
        int step = 0;
        StepData stepData = StepSaver.BackStep(ref step);
        if (stepData != null)
        {

            SetCellToStone(stepData.NewCell, stepData.PrevCell, stepData.Stone, true);


        }
        if(step==0)
            OnStepMade?.Invoke(0);
        //{
        //    if (b != null)
        //    {
        //        GameObject sender = b as GameObject;
        //        if (sender != null)
        //        {
        //            UIElemSwitchValue uIElemSwitch = sender.GetComponent<UIElemSwitchValue>();
        //            uIElemSwitch.Switch(false);
        //        }

        //    }
        //}


    }
    Cell GetTargetCell(Vector2Int currentPos, int side)
    {

        Cell c = null;
        bool hasObstacle = false;
   
        if (side == 0)
        {

            for (int i = currentPos.x-1; i >= 0; i--)
            {
                if (i < 0 || currentPos.y<0)
                    continue;
                c = map[i, currentPos.y].CheckCell( c, out hasObstacle);
                if (hasObstacle )
                    break;
            }
        }
        else if (side == 1)
        {

            for (int i = currentPos.x + 1; i < map.GetLength(0); i++)
            {
                if (i < 0 || currentPos.y < 0)
                    continue;
                c = map[i, currentPos.y].CheckCell(c, out hasObstacle);
                if (hasObstacle)
                    break;
            }
          
        }
        else if (side == 2)
        {

            for (int j = currentPos.y - 1; j >= 0; j--)
            {
                if (j < 0 || currentPos.x < 0)
                    continue;
                c = map[currentPos.x, j].CheckCell(c, out hasObstacle);
                if (hasObstacle)
                    break;
            }
        }
        else if (side == 3)
        {

            for (int j = currentPos.y + 1; j < map.GetLength(1); j++)
            {
                if (j < 0 || currentPos.x < 0)
                    continue;
                c = map[currentPos.x, j].CheckCell(c, out hasObstacle);
                if (hasObstacle)
                    break;
            }
         
        }


        return c;
    }




    public void Resize()
    {



        GenUtil.Resize(map, leftReserve, upReserve, Parent, CurrentLevel.W, CurrentLevel.H, Gap, StoneDeltaTime);
    }

    public void Restart()
    {
        if (saveController != null)
            saveController.DeleteSave(CurrentLevel.ID);
        GenUtil.Replay(CurrentLevel, map, leftReserve, upReserve);
        movesCount = 0;
        OnMove?.Invoke(0);
        OnMapChanged?.Invoke(0, targetCellsCount);
        victory = false;
    }

    public void QuitLevel()
    {
        AddSave();
      
        GameManager gm = GameManager.Instance;
        if (gm != null)
        {
            gm.ToMainMenu();
        }
    }


    public void AddSave()
    {
       // if (saveController != null && movesCount !=0)
            if (saveController != null &&!victory )
                saveController.CreateSave(CurrentLevel.ID, CurrentLevel.Name, map, leftReserve, upReserve, movesCount);
    }

   
    void Victory()
    {

        // if (CurrentLevel.MovesRecord==0 || movesCount < CurrentLevel.MovesRecord)
        //CurrentLevel.MovesRecord = movesCount;
        bool isNewRecord = false;
        RecordLevelSave.AddRecord(CurrentLevel, movesCount, out isNewRecord);
        victory = true;
        OnVictory?.Invoke(movesCount, RecordLevelSave.GetRecordByLevelID(CurrentLevel), isNewRecord);
        movesCount = 0;
        saveController.DeleteSave(CurrentLevel.ID);
    }
    //private void OnApplicationPause(bool pause)
    //{
    //    if (pause)
    //    {
    //        saveController.CreateSave(CurrentLevel.ID, CurrentLevel.Name, map, leftReserve, upReserve);
    //        //StartCoroutine(saveController.Save());
    //        saveController.SaveData();
    //    }
    //}

    //private void OnApplicationQuit()
    //{
    //    saveController.CreateSave(CurrentLevel.ID, CurrentLevel.Name, map, leftReserve, upReserve);
    //    //StartCoroutine(saveController.Save());
    //    saveController.SaveData();
    //}
}
