using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MainMenuController : UIController
{
    public RectTransform LevelsParentl;
    public LevelsPanel LevelsPanelPrefab;


    public Button SavedLevelsBtn;
    public List<LevelsContainerButtonData> LevelsContainerButtons = new List<LevelsContainerButtonData>();

    Dictionary<string, Button> LevelsButtons = new Dictionary<string, Button>();
    List<LevelsPanel> LevelsPanels = new List<LevelsPanel>();

    bool btnsLocalized = false;
    protected override void Awake()
    {
        LevelsButtons = LevelsContainerButtonData.ListToDictionary(LevelsContainerButtons);
        base.Awake();

    }
    protected override void Start()
    {

        GameManager.Instance.LoadLevels(this);
        base.Start();
        if (LocalizationManager.LocalTextLoaded  && !btnsLocalized)
            SetLevelsLocalization();


    }

    public override void SetLocalization()
    {
        base.SetLocalization();

        Debug.Log("GETTING NAME ");
        if (LevelsPanels.Count!=0)
            SetLevelsLocalization();
    }
    public void SetLevelsLocalization()
    {
        btnsLocalized = true;
        Debug.Log("GETTING NAME " );
        foreach (var levelsPanel in LevelsPanels)
        {

          
            if (string.IsNullOrEmpty(levelsPanel.ButtonKey ))
            {

                string name = getLocal(levelsPanel.gameObject.name, levelsPanel.ButtonKey);
                levelsPanel.SetName(name);

            }
           // else
            {

                string name =getLocal(levelsPanel.ButtonKey, levelsPanel.ButtonKey);

                Debug.Log("GETTING NAME "+name);
                levelsPanel.SetName(name);

            }
        }

    }

    public Button GetButtonByKey(string key)
    {
        if (LevelsButtons.ContainsKey(key))
            return LevelsButtons[key];
        return null;
    }
    public void MakeLevelButton(Button btn, LevelsContainer levelsContainer, string key = null)
    {

        LevelsPanel levelsPanel = InstantiateBtn(btn, key);
        levelsPanel.AddLevels(levelsContainer);


    }

    public void MakeLevelButton(Button btn, List<LevelsContainer> levelsContainers, List<SaveData> saves, string key = null)
    {
        LevelsPanel levelsPanel = InstantiateBtn(btn, key);     
        levelsPanel.AddLevels(levelsContainers, saves);
       
    }


    LevelsPanel InstantiateBtn(Button btn, string key = null)
    {
        btn.onClick.RemoveAllListeners();
        LevelsPanel levelsPanel = Instantiate(LevelsPanelPrefab);
        LevelsPanels.Add(levelsPanel);

        levelsPanel.ButtonKey = key;
        levelsPanel.transform.SetParent(LevelsParentl);
        levelsPanel.transform.localScale = Vector3.one;
        levelsPanel.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        levelsPanel.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        levelsPanel.gameObject.SetActive(false);
        btn.onClick.AddListener(() => { btn.transform.parent.parent.gameObject.SetActive(false); levelsPanel.gameObject.SetActive(true); });

        return levelsPanel;
    }
    public void CloseLevelsPanel(RectTransform sender)
    {
        LevelsPanel activePanel=sender.GetComponentInChildren<LevelsPanel>();
        if (activePanel != null)
        activePanel.gameObject.SetActive(false);

    }
}
