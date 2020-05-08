using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
[System.Serializable]
public class LocalizationData
{
    public LocalizationItem[] items;
}
[System.Serializable]
public class LocalizationDataLang:LocalizationData
{
    public SystemLanguage SystemLanguage;
}
[System.Serializable]
public class LocalizationItem
{
    public string key;
    public string value;
}

[System.Serializable]
public class SystemLanguageFile
{
    public string FileName;
    public SystemLanguage SystemLanguage;
}


public class LocalizationManager : MonoBehaviour
{
    static SystemLanguage[] possibleLanguages = new SystemLanguage[] {
        SystemLanguage.English, SystemLanguage.Russian
    };

    public int CurrentLanguageIndex { get { return currentLanguage; } }
    public SystemLanguage CurrentLanguage { get { return CurrentSystemLanguage; } }
    int currentLanguage = 0;
    static SystemLanguage DefaultSystemLanguage = SystemLanguage.English;
    static SystemLanguage CurrentSystemLanguage;
    public static bool LocalTextLoaded = false;
    public event System.Action OnLocalizedTextLoaded;




    public string LanguagesFolder = "Lang2";
    public static LocalizationManager Instance;
    public static Dictionary<string, string> LocalUIText = new Dictionary<string, string>();
    public static Dictionary<SystemLanguage, Dictionary<string, string>> LocalUITextDynamic = new Dictionary<SystemLanguage, Dictionary<string, string>>();
    public static Dictionary<SystemLanguage, string> SystemLanguageFiles = new Dictionary<SystemLanguage, string>();


    public SystemLanguageFile[] languageFiles;

    public LocalizationDataLang[] SpecialUIEementsContent;
   
    bool loadStart = true;

    private void Awake()
    {


        if (Instance == null)
        {
            Instance = this;
            for (int i = 0; i < languageFiles.Length; i++)
            {

                SystemLanguageFiles.Add(languageFiles[i].SystemLanguage, languageFiles[i].FileName);
            }
           

            for (int i = 0; i < SpecialUIEementsContent.Length; i++)
            {
                LocalUITextDynamic.Add(SpecialUIEementsContent[i].SystemLanguage, new Dictionary<string, string>());
                for (int j = 0; j < SpecialUIEementsContent[i].items.Length; j++)
                {
                    string key = SpecialUIEementsContent[i].items[j].key;
                    string val = SpecialUIEementsContent[i].items[j].value;

                    LocalUITextDynamic[SpecialUIEementsContent[i].SystemLanguage].Add(key, val);
                }
            }
        }
        else
        {

            Destroy(gameObject);
        }
        DontDestroyOnLoad(Instance);






    }


    public void LoadSavedLanguage()
    {
        if (PlayerPrefs.HasKey("LangIndex"))
        {
            currentLanguage = PlayerPrefs.GetInt("LangIndex");
            CurrentSystemLanguage = possibleLanguages[currentLanguage];

        }
        else
        {

            CurrentSystemLanguage = DefaultSystemLanguage;
        }

        string fileName = SystemLanguageFiles[DefaultSystemLanguage];
        if (SystemLanguageFiles.ContainsKey(CurrentSystemLanguage))
            fileName = SystemLanguageFiles[CurrentSystemLanguage];
        StartCoroutine(LoadLocalizedText(fileName, currentLanguage, CurrentSystemLanguage));


    }
    public void SwitchLanguage()
    {
        int lang = currentLanguage;
        lang++;
        if (lang == possibleLanguages.Length)
            lang = 0;

        SystemLanguage language = possibleLanguages[lang];


        string fileName = SystemLanguageFiles[DefaultSystemLanguage];
        if (SystemLanguageFiles.ContainsKey(language))
        {
            fileName = SystemLanguageFiles[language];
        }
        StartCoroutine(LoadLocalizedText(fileName, lang, language));


    }





    IEnumerator LoadLocalizedText(string fileName, int langIndex, SystemLanguage lang)
    {

        LocalUIText = new Dictionary<string, string>();
        fileName = LanguagesFolder + "/" + fileName + ".json";
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

     
        LocalUIText.Clear();

        string dataAsJson = "";


#if UNITY_ANDROID || UNITY_WEBGL

        UnityWebRequest unityWebRequest = UnityWebRequest.Get(filePath);
        yield return unityWebRequest.SendWebRequest();
        dataAsJson = unityWebRequest.downloadHandler.text;
        

#else
        if (File.Exists(filePath))
        {
        dataAsJson = File.ReadAllText(filePath);
            yield return null;
        }
#endif
      
        if (dataAsJson != "" && dataAsJson != null)
        {
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                LocalUIText.Add(loadedData.items[i].key, loadedData.items[i].value);

            }

            LocalTextLoaded = true;
            CurrentSystemLanguage = lang;
            currentLanguage = langIndex;
            PlayerPrefs.SetInt("LangIndex", currentLanguage);
        }
        else
        {
            LocalTextLoaded = false;

        }

        if (OnLocalizedTextLoaded != null)// && LocalTextLoaded
        {
            OnLocalizedTextLoaded();
        }

        if (!LocalTextLoaded && loadStart)
        {

            currentLanguage = 0;
            CurrentSystemLanguage = DefaultSystemLanguage;
        }


       
    }


    public string GetLocalizedValue(string uiElementName)
    {
        // Debug.Log("fsdjnfskdjnflk"+ uiElementName);
        string result = null;

        if (LocalUIText.ContainsKey(uiElementName))
        {
            result = LocalUIText[uiElementName];
        }

        return result;

    }
    public string GetLocalizedSpecialValue(string element)
    {
      //  Debug.Log(LocalUITextDynamic.Count + "       ☺-☺  " + LocalUITextDynamic.ContainsKey(CurrentLanguage)+"   "+CurrentLanguage);
        //foreach (var item in LocalUITextDynamic)
        //{
        //    foreach (var el in item.Value)
        //    {
        //        Debug.Log(el.Key +"     "+el.Value);
        //    }
        //}
        string result = null;
        if (LocalUITextDynamic.ContainsKey(CurrentLanguage))
        {
     
            if (LocalUITextDynamic[CurrentLanguage].ContainsKey(element))
            {
            result = LocalUITextDynamic[CurrentLanguage][element];
            }
        }
        return result;

    }

    public void ClearUIControllerSubscribes()
    {

        OnLocalizedTextLoaded = null;
    }

}
