using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
public class LevelUIController : UIController
{

    public TextMeshProUGUI LevelName;

    public TextMeshProUGUI StonesTotal;
    public TextMeshProUGUI StonesOnCells;

    public TextMeshProUGUI MovesRecord;

    public TextMeshProUGUI MovesCount;

    string movesCountTxt;

    public RectTransform WinPanel;


    public TextMeshProUGUI WinText;
    public TextMeshProUGUI MovesTxt;
    public TextMeshProUGUI OldRecordTxt;
    public Image NewRecordImg;


    public ImageSwitching BackBtn;
    int totalStones = 0;
    int currStones = 0;
    int totalCells = 0;
    int record = 0;
    int movesCount = 0;

    protected override void Start()
    {
        base.Start();
        SetBack(0);
    }
    public override void SetLocalization()
    {
        base.SetLocalization();
      

        SetStonesTotal(totalStones);
        SetStoneCells(currStones, totalCells);
        SetMovesRecord(record);
        SetMovesCount(movesCount);

    }

    public void SetLevelName(string name)
    {

        LevelName.text = name;

    }

    public void SetStonesTotal(int stones)
    {
        totalStones = stones;

        StonesTotal.text = getLocal("StonesTotal", "Stones Total") + "\n" + stones;


    }
    public void SetStoneCells(int stones, int cells)
    {
        currStones = stones;
        totalCells = cells;

        StonesOnCells.text = getLocal("StonesOnCells", "Stones On Cells") + "\n" + stones.ToString()+"/"+cells.ToString();
    
    }
    public void SetMovesRecord(int record)
    {
        this.record = record;
        string recordStra = record.ToString();
        if (record == 0)
            recordStra = "-";
        MovesRecord.text = getLocal("MovesRecord", "Moves Record") + "\n" + recordStra;

    }

    public void SetBack(int hasBackMoves)
    {
        if (BackBtn != null)
        {

            if (hasBackMoves == 0)
            { BackBtn.Switch(false); }
            else
            { BackBtn.Switch(true); }

        }
    }
    public void SetMovesCount(int count)
    {
        movesCount = count;
        MovesCount.text = getLocal("MovesCount", "Moves Count") + "\n"+ count.ToString();
      

    }


    public void Win(int movesCount, int record, bool isNewRecord)
    {
        WinPanel.gameObject.SetActive(true);
        WinText.text = getLocal("Victory", "Victory");

        if (isNewRecord)
        {
            OldRecordTxt.transform.parent.gameObject.SetActive(false);
            MovesTxt.text = getLocal("WinMovesNewRecord", "Win Moves New Record") + ": " + movesCount.ToString();
            if (NewRecordImg != null)
            {
                NewRecordImg.gameObject.SetActive(true);
            }
        }
        else
        {
            MovesTxt.text = getLocal("WinMovesCount", "Win Moves Count") + ": " + movesCount.ToString();

            OldRecordTxt.gameObject.SetActive(true);
            OldRecordTxt.text = getLocal("WinRecord", "Win Record") + ": " + movesCount.ToString();
            if (NewRecordImg != null)
            {
                NewRecordImg.gameObject.SetActive(false);
            }

        }

    }



    public void NextLevel()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.NextLevel();
    }
}
