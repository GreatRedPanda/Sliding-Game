using UnityEngine;
using System.Collections;
//[System.Serializable]
[CreateAssetMenu(fileName = " New language", menuName = "LocalizationText")]
public class LanguageTextJsonContainer : ScriptableObject
{

   [TextArea(3, 50)]
    public string JsonText;
}
