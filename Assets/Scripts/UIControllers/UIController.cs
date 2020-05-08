using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UIElement
{

    public string ParentPanelName;
    public RectTransform Panel;
    // public Text ChildTextComponent;
}
public class UIController : MonoBehaviour
{
    protected bool localized = false;
    public RectTransform RootPanel;

    public UIElemSwitchValue FastMoveBtn;
    public UIElemSwitchValue FastBackMoveBtn;

    public Scrollbar LoadBar;
    public RectTransform LoadingPanel;
    //  protected Dictionary<string, UIElement> Panels = new Dictionary<string, UIElement>();
    protected virtual void Awake()
    {

        LocalizationManager.Instance.ClearUIControllerSubscribes();
        LocalizationManager.Instance.OnLocalizedTextLoaded += SetLocalization;

    }
    protected virtual void Start()
    {

        if (LocalizationManager.LocalTextLoaded && !localized)
            SetLocalization();
       
    }
    public virtual void SetLocalization()
    {

        localized = true;

        if (FastMoveBtn != null)
            FastMoveBtn.Switch(GameManager.FastStoneMove);
        if (FastBackMoveBtn != null)
        {

            if (GameManager.FastStoneMove)
            {

                FastBackMoveBtn.Switch(true);

            }

            else
            {

                FastBackMoveBtn.Switch(GameManager.FastStoneBackMove);
            }
        }


        TextMeshProUGUI[] children = RootPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var t in children)
            {
                if (t != null)
                {

                    string newText = null;

                if (t.GetType() == typeof(LocalizableText))
                {
                    LocalizableText lc = t as LocalizableText;
                 
                    newText = getLocal(lc.Key, lc.text);
                }
                else
                {
                    newText = getLocal(t.name, t.text);

                }

                if (t.transform.GetComponentInParent<TextSwitching>() != null)
                {
                    t.transform.GetComponentInParent<TextSwitching>().Localize();
                    continue;

                }
                    if (newText != null)
                        t.text = newText;
                }
            }


       

    }


    public void SwitchLanguage(Object b)
    {
        LocalizationManager.Instance.SwitchLanguage();
        if (b != null)
        {
            GameObject sender = b as GameObject;
            if (sender != null)
            {
                UIElemSwitchValue uIElemSwitch = sender.GetComponent<UIElemSwitchValue>();
                uIElemSwitch.Switch();
            }

        }
    }

    public void FastStoneMove(Object b)
    {
     
        GameManager.Instance.SetFastMove();

        if (b != null)
        {
            GameObject sender = b as GameObject;
            if (sender != null)
            {
                UIElemSwitchValue uIElemSwitch = sender.GetComponent<UIElemSwitchValue>();
                uIElemSwitch.Switch(GameManager.FastStoneMove);
            }

            if (GameManager.FastStoneMove)
            {

                FastBackMoveBtn.Switch(true);
                
            }

            else
            {
               
                    FastBackMoveBtn.Switch(GameManager.FastStoneBackMove);
            }
        }
    }

    public void FastStoneBackMove(Object b)
    {

        if (GameManager.FastStoneMove)
            return;
        GameManager.Instance.SetFastBackMove();
        if (b != null)
        {
            GameObject sender = b as GameObject;
            if (sender != null)
            {
                UIElemSwitchValue uIElemSwitch = sender.GetComponent<UIElemSwitchValue>();
                uIElemSwitch.Switch(GameManager.FastStoneBackMove);
            }
           
        }
    }



    public virtual void UILoadScene()
    {

        LoadingPanel.gameObject.SetActive(true);
        StartCoroutine(LoadingScene());



    }


    public static string getLocal(string key, string defStr)
    {

        string local = LocalizationManager.Instance.GetLocalizedValue(key);
        if (string.IsNullOrEmpty(local))
            local = defStr;
        return local;
    }
    IEnumerator LoadingScene()
    {
        while (!GameManager.Instance.AsyncSceneLoad.isDone)
        {
            float progress = Mathf.Clamp01(GameManager.Instance.AsyncSceneLoad.progress / 0.9f);

            LoadBar.value = progress;
            yield return null;
        }

    }
    //public void ClosePanel(GameObject panelToClose)
    //{

    //    panelToClose.SetActive(false);
    //}

    //public void OpenPanel(GameObject panelToOpen)
    //{

    //    panelToOpen.SetActive(true);
    //}
}
