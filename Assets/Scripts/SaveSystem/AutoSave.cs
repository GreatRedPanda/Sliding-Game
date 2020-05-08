using UnityEngine;
using System.Collections;

public class AutoSave : MonoBehaviour
{
    public Animator SaveAnimation;
    public int TimeInMinutes = 5;
    float timeInSeconds;
    float currentTime = 0;

     GameManager GameMan;



    private void Start()
    {
        timeInSeconds = TimeInMinutes * 60;
        GameMan = FindObjectOfType<GameManager>();
    }
    private void Update()
    {


        currentTime += Time.deltaTime;

        if (currentTime >= timeInSeconds)
        {
            Save();
        }
    }

    public void Save()
    {
        if (GameMan != null)
            GameMan.AutoSaving();
        currentTime = 0;

        SaveAnimation.SetTrigger("Saving");
    }

}
