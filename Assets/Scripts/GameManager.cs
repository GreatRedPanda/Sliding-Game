using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
[System.Serializable]
public class  LevelsContainerData
{

    public string Key;
    public LevelsContainer LevelsContainer;

    public static Dictionary<string, LevelsContainer> ListToDictionary(List<LevelsContainerData> list)
    {
     
        Dictionary<string, LevelsContainer> LevelsContainersDatas = new Dictionary<string, LevelsContainer>();
        foreach (var item in list)
        {
            LevelsContainersDatas.Add(item.Key, item.LevelsContainer);
        }

        return LevelsContainersDatas;
    }
}

[System.Serializable]
public class LevelsContainerButtonData
{

    public string Key;
    public Button Button;

    public static Dictionary<string, Button> ListToDictionary(List<LevelsContainerButtonData> list)
    {
       
        Dictionary<string, Button> LevelsButtons = new Dictionary<string, Button>();
        foreach (var item in list)
        {
            LevelsButtons.Add(item.Key, item.Button);
        }

        return LevelsButtons;
    }
}
public class GameManager : MonoBehaviour
{
    public AsyncOperation AsyncSceneLoad;
    public static int Sound = 0;
    public static bool FirstTime = true;
    public static bool FastStoneMove = false;
    public static bool FastStoneBackMove = false;

    public LevelsContainer SimpleLevels;

    LevelsContainer CurrentContainer;
    LevelDescription CurrentLevel;


    public static GameManager Instance;
    public SaveController saveController;
    public RecordLevelSave RecordLevelSave;

    public List<LevelsContainerData> LevelsContainers = new List<LevelsContainerData>();

    Dictionary<string, LevelsContainer> LevelsGroups = new Dictionary<string, LevelsContainer>();

    public string savedLevelsKey= "savedLevels";
    public string RecordsPrefSave = "";
    private void Awake()
    {
        if (Instance == null)
        {
           
            Instance = this;
            DontDestroyOnLoad(gameObject);
            loadPrefs();
            saveController.LoadData();

          //  StartCoroutine(saveController.Load());


            RecordLevelSave.LoadRecord(RecordsPrefSave);

            LevelsGroups = LevelsContainerData.ListToDictionary(LevelsContainers);
        LocalizationManager.Instance.LoadSavedLanguage();

        }
        else
            Destroy(gameObject);
    }
    void Start()
    {      
      
       // LoadLevels();
          
    }
    void loadPrefs()
    {
        if (PlayerPrefs.HasKey(nameof(RecordsPrefSave)))
        {

            RecordsPrefSave = PlayerPrefs.GetString(nameof(RecordsPrefSave));
        }
        if (PlayerPrefs.HasKey(nameof(FirstTime)))
        {
            FirstTime = !(PlayerPrefs.GetInt(nameof(FirstTime)) == 1);
    

        }
        if (PlayerPrefs.HasKey("Sound"))
        {
            Sound = PlayerPrefs.GetInt("Sound");

        }
        else
        {
            PlayerPrefs.SetInt("Sound", Sound);
        }


        if (PlayerPrefs.HasKey(nameof(FastStoneMove)))
        {
            FastStoneMove = (PlayerPrefs.GetInt(nameof(FastStoneMove)) == 1);

        }

        if (PlayerPrefs.HasKey(nameof(FastStoneBackMove)))
        {
            FastStoneBackMove = (PlayerPrefs.GetInt(nameof(FastStoneBackMove)) == 1);

        }






    }
 public   void LoadLevels(MainMenuController mainMenuController)
    {

    
        if (mainMenuController != null)
        {

            foreach (var item in LevelsGroups)
            {
                Button btn = mainMenuController.GetButtonByKey(item.Key);
                if (btn!= null)
                {
                    mainMenuController.MakeLevelButton(btn, item.Value, item.Key);
                }
            }
            if (SaveController.saves.Count != 0)
                mainMenuController.MakeLevelButton(mainMenuController.SavedLevelsBtn, LevelsGroups.Values.ToList(), SaveController.saves, savedLevelsKey);
            else
                mainMenuController.SavedLevelsBtn.gameObject.SetActive(false);
        }
    }



    public LevelsContainer GetContainerByLEvel(LevelDescription level)

    {

        foreach (var item in LevelsGroups)
        {
            if (item.Value.Levels.Contains(level))
                return item.Value;
        }
        return null;
    }
    public void LoadLevel(LevelDescription level)
    {
        CurrentLevel = level;

        //  SceneManager.LoadScene(1);
        AsyncSceneLoading(1);
    }

    public void LoadLevel(LevelDescription level, LevelsContainer container)
    {
        CurrentLevel = level;
        CurrentContainer = container;
        //SceneManager.LoadScene(1);
        AsyncSceneLoading(1);
    }
    public void NextLevel()
    {
        int index = CurrentContainer.Levels.IndexOf(CurrentLevel);
        index++;
        if (index>0 && index != CurrentContainer.Levels.Count)
        {
            CurrentLevel = CurrentContainer.Levels[index];
            LoadLevel(CurrentLevel);
        }
        if (index == CurrentContainer.Levels.Count)
        {

            KeyValuePair<string, LevelsContainer> keyValuePair=    LevelsGroups.First(x => x.Value == CurrentContainer);
            List<string> keys=      LevelsGroups.Keys.ToList<string>();
            int nextKey = keys.IndexOf(keyValuePair.Key)+1;

            if (nextKey != keys.Count)
            {
                CurrentContainer = LevelsGroups[keys[nextKey]];
                CurrentLevel = CurrentContainer.Levels[0];
                LoadLevel(CurrentLevel);
            }
            else
                SceneManager.LoadScene(0);
        }
    }
    public void ToMainMenu()
    {

        SceneManager.LoadScene(0);

    }

    private void OnLevelWasLoaded(int level)
    {
        if (Instance == this)
        {
            if (level == 1)
            {
                FindObjectOfType<GameController>().CurrentLevel = CurrentLevel;
            }

#if UNITY_WEBGL
            else
            if (level == 0)
            {

                saving();

            }

#endif
        }
    }




    public void AutoSaving()
    {

        saving();
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            saving();
        }
    }

    private void OnApplicationQuit()
    {
        saving();
    }


    void saving()
    {

        PlayerPrefs.SetString(nameof(RecordsPrefSave), RecordLevelSave.SaveRecords());
        SaveInLevel();
        saveController.SaveData();

    }
    void SaveInLevel()
    {
        GameController gc = FindObjectOfType<GameController>();
        if (gc != null)
            gc.AddSave();
    }

    public void SwitchSound()
    {
        if (Sound == 0)
            Sound = 1;
        else
            Sound = 0;
        PlayerPrefs.SetInt("Sound", Sound);
    }

    public void SetFastMove()
    {
        FastStoneMove = !FastStoneMove;
        PlayerPrefs.SetInt(nameof(FastStoneMove), FastStoneMove?1:0);
    }

    public void SetFastBackMove()
    {
        FastStoneBackMove = !FastStoneBackMove;
        PlayerPrefs.SetInt(nameof(FastStoneBackMove), FastStoneBackMove ? 1 : 0);
    }


    public void TutorialPassed()
    {
       
        FirstTime = false;
        PlayerPrefs.SetInt(nameof(FirstTime), 1);
    }


    void AsyncSceneLoading(int index)
    {
        AsyncSceneLoad = SceneManager.LoadSceneAsync(index);
        FindObjectOfType<UIController>().UILoadScene();

    }

}
