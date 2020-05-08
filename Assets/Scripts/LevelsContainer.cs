using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = " New LevelContainer", menuName = "LevelContainer")]
public class LevelsContainer  : ScriptableObject
{
   //[HideInInspector]
    public List<LevelDescription> Levels;


   



}
    //[System.Serializable]
    //public class Description
    //{
    //    //[SerializeField]
    //    //string m_Name;


    //    [SerializeField]
    //    public LevelDescription LevelDescription;
    //    //[SerializeField]
    //    // string Name => (LevelDescription==null)?"":LevelDescription.Name;
    //}