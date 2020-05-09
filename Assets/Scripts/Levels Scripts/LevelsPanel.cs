using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelsPanel : MonoBehaviour
{

    public int LevelsOnPage;

    public RectTransform PagePrefab;
    public Button LevelButtonPrefab;


    public RectTransform PageParent;

    public string LevelRecordName;


    public TextMeshProUGUI CategoryName;
    public TextMeshProUGUI PagesCount;
    List<RectTransform> pages = new List<RectTransform>();

    public string ButtonKey;
    int currentPage;
    int pagesCount;

    public void SetName(string name)
    {
        CategoryName.text = name;
    }
    void CalculateInventoryPanelParameters(RectTransform panel, int row,int colomns=1)
    {
        GridLayoutGroup grid = panel.GetComponent<GridLayoutGroup>();


        Vector2 spacing = grid.spacing;
        float paddingX = grid.padding.left + grid.padding.right;

        float paddingY = grid.padding.top + grid.padding.bottom;

        float cellSizeX = (panel.rect.width - paddingX - spacing.x * (colomns - 1)) / colomns;



        float cellSizeY = (panel.rect.height - paddingY - spacing.y * (row - 1)) / row;



        grid.cellSize = new  Vector2(cellSizeX, cellSizeY);
    

        
        //float containerLength = (cellSizeY * row + spacing.y * (row - 1)) + paddingY;
        //panel.sizeDelta = new Vector2(panel.sizeDelta.x, containerLength);
      
    }


    public void AddLevels(LevelsContainer levelsContainer)
    {

     

        for (int i = 0; i < levelsContainer.Levels.Count; i++)
        {

            if (i == 0 || (i) % (LevelsOnPage) == 0)
            {
          
                RectTransform lvlPage = Instantiate(PagePrefab);
             
                pages.Add(lvlPage);
                lvlPage.transform.SetParent(PageParent);
                lvlPage.transform.localScale = Vector3.one;
                lvlPage.offsetMin = Vector2.zero;
                lvlPage.offsetMax = Vector2.zero;
                //CalculateInventoryPanelParameters(lvlPage, LevelsOnPage);

            }
            Button btn = Instantiate(LevelButtonPrefab);
            btn.transform.SetParent(pages[pages.Count - 1]);
            btn.transform.localScale = Vector3.one;
            btn.GetComponentInChildren<TextMeshProUGUI>().text = levelsContainer.Levels[i].Name;

       

            LevelDescription lvl = levelsContainer.Levels[i];
            RecordLevelSave recordLevelSave = FindObjectOfType<RecordLevelSave>();
            if (recordLevelSave != null)
            {
                int record = recordLevelSave.GetRecordByLevelID(lvl);
                if (record != 0)
                    SetPassedLevel(btn.transform, record.ToString());
            }
            btn.onClick.AddListener(() =>
            {

                GameManager.Instance.LoadLevel(lvl, levelsContainer);

            });

            
        }
        ScrollSnapRect scrollSnapRect = GetComponentInChildren<ScrollSnapRect>();

        if (scrollSnapRect != null)
            scrollSnapRect.OnPageSwitched += GetCurrentPage;
        GetCurrentPage(currentPage);

    }



    void SetPassedLevel(Transform level, string moves)
    {

        Transform t = level.Find(LevelRecordName);
        t.gameObject.SetActive(true);
        TextMeshProUGUI text = t.GetComponentInChildren<TextMeshProUGUI>();
        text.text = moves;
    }


    public void GetCurrentPage(int page)
    {
        if (pages.Count != 0)
        {
            currentPage = page + 1;
        }
            PagesCount.text = currentPage + "/" + pages.Count;
        
    }
    public void AddLevels(List<LevelsContainer> levelsContainer, List<SaveData> saves)
    {


        
        for (int i = saves.Count-1, j=0; i >=0; i--)
        {

            LevelsContainer container = null;
            LevelDescription lvl = null;
            foreach (var lc in levelsContainer)
            {
                lvl = lc.Levels.Find(x => x.ID == saves[i].LevelID);
                if (lvl != null)
                {
                    container = lc;
                    break;
                }
            }
            if (lvl != null)
            {
                if ( (j) % (LevelsOnPage) == 0)
                {

                    RectTransform lvlPage = Instantiate(PagePrefab);
                    pages.Add(lvlPage);
                  
                    lvlPage.transform.SetParent(PageParent);
                    lvlPage.transform.localScale = Vector3.one;
                    lvlPage.offsetMin = Vector2.zero;
                    lvlPage.offsetMax = Vector2.zero;
                   // CalculateInventoryPanelParameters(lvlPage, LevelsOnPage);

                }
                Button btn = Instantiate(LevelButtonPrefab);
                btn.transform.SetParent(pages[pages.Count - 1]);
                btn.transform.localScale = Vector3.one;
                btn.GetComponentInChildren<TextMeshProUGUI>().text = lvl.Name;


                RecordLevelSave recordLevelSave = FindObjectOfType<RecordLevelSave>();
                if (recordLevelSave != null)
                {
                    int record = recordLevelSave.GetRecordByLevelID(lvl);
                    if (record != 0)
                        SetPassedLevel(btn.transform, record.ToString());
                }
                btn.onClick.AddListener(() =>
                {

                    GameManager.Instance.LoadLevel(lvl, container);

                });
                j++;
            }
         
        }
        ScrollSnapRect scrollSnapRect = GetComponentInChildren<ScrollSnapRect>();

        if (scrollSnapRect != null)
            scrollSnapRect.OnPageSwitched += GetCurrentPage;
        GetCurrentPage(currentPage);
    }
}

