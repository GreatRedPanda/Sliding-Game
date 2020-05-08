#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Linq;


[CustomEditor(typeof(LevelsContainer))]
[CanEditMultipleObjects]
public class LevelsListEditor : Editor
{
    ReorderableList m_LevelsList;
    private SerializedProperty m_levelsData;

    void OnEnable()
    {
        m_levelsData = serializedObject.FindProperty("Levels");
         m_LevelsList = new ReorderableList(serializedObject, m_levelsData, true, true, true, true);
        m_LevelsList.drawHeaderCallback = DrawHeaderCallback;

        //Set up the method callback to draw each element in our reorderable list
        m_LevelsList.drawElementCallback = DrawElementCallback;

        //Set the height of each element.
        m_LevelsList.elementHeightCallback += ElementHeightCallback;

        //Set up the method callback to define what should happen when we add a new object to our list.
        m_LevelsList.onAddCallback += OnAddCallback;
    }


    /// <summary>
    /// Draws the header for the reorderable list
    /// </summary>
    /// <param name="rect"></param>
    private void DrawHeaderCallback(Rect rect)
    {
        EditorGUI.LabelField(rect, "Shortcuts");
    }

    /// <summary>
    /// This methods decides how to draw each element in the list
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="index"></param>
    /// <param name="isactive"></param>
    /// <param name="isfocused"></param>
    private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
    {
        
        //Get the element we want to draw from the list.
        SerializedProperty element = m_LevelsList.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;

        //We get the name property of our element so we can display this in our list.
        //SerializedProperty elementName = element.FindPropertyRelative("m_Name");
        //string elementTitle = string.IsNullOrEmpty(elementName.stringValue)
        //    ? "New level"
        //    : $"{elementName.stringValue}";

        //Draw the list item as a property field, just like Unity does internally.
     
        EditorGUI.PropertyField(position:
            new Rect(rect.x += 10, rect.y, Screen.width * .5f, height: EditorGUIUtility.singleLineHeight), property:
            element, label: new GUIContent(element.displayName), includeChildren: true);
    }

    /// <summary>
    /// Calculates the height of a single element in the list.
    /// This is extremely useful when displaying list-items with nested data.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private float ElementHeightCallback(int index)
    {
        //Gets the height of the element. This also accounts for properties that can be expanded, like structs.
        float propertyHeight =
            EditorGUI.GetPropertyHeight(m_LevelsList.serializedProperty.GetArrayElementAtIndex(index), true);

        float spacing = EditorGUIUtility.singleLineHeight / 2;

        return propertyHeight + spacing;
    }

    /// <summary>
    /// Defines how a new list element should be created and added to our list.
    /// </summary>
    /// <param name="list"></param>
    private void OnAddCallback(ReorderableList list)
    {
        var index = list.serializedProperty.arraySize;
        list.serializedProperty.arraySize++;
        list.index = index;
        var element = list.serializedProperty.GetArrayElementAtIndex(index);
    }

    /// <summary>
    /// Draw the Inspector Window
    /// </summary>
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        m_LevelsList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
    //public override void OnInspectorGUI()
    //{
    //    EditorGUI.BeginChangeCheck();


    //    serializedObject.Update();


    //    var fields = target.GetType().GetFields();
    //    foreach (var field in fields)
    //        EditorGUILayout.PropertyField(serializedObject.FindProperty(field.Name), true);

    //    m_LevelsList.DoLayoutList();

    //    serializedObject.ApplyModifiedProperties();
    //    if (EditorGUI.EndChangeCheck())
    //        EditorUtility.SetDirty(target);
    //}
}
#endif