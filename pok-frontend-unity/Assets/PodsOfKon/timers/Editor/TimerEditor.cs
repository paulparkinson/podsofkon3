using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;

[CustomEditor(typeof(CustomTimer))]
[CanEditMultipleObjects]
public class TimerEditor :  Editor
{
    //this texture is the banner image at top of script in inspector.
    Texture2D texture;

    void OnEnable() 
    {
        texture = Resources.Load("ctBanner") as Texture2D;
    }

    public override void OnInspectorGUI () 
    {
        //These 2 lines give us the image at the top of the CustomizableTimer script in the inspector.
        var space = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(90));
        GUI.Label(space, texture);

        DrawDefaultInspector();

        //The Horizontals and FlexibleSpaces center our elements in the inspector.
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        //The following creates the 3 buttons in the inspector. The foreach enables functionality for multi-object selecting.
        if (GUILayout.Button("Start Timer", GUILayout.Width(200), GUILayout.Height(23)))
        {
            foreach (var ct in targets.Cast<CustomTimer>())
            {             
            ct.StartTimer();
            }
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Pause Timer", GUILayout.Width(200), GUILayout.Height(23)))
        {
            foreach (var ct in targets.Cast<CustomTimer>())
            {
                ct.PauseTimer();
            }
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Reset Timer", GUILayout.Width(200), GUILayout.Height(23)))
        {
                foreach (var ct in targets.Cast<CustomTimer>())
                {
                    ct.ResetTimer();
                }
        }  

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }
}