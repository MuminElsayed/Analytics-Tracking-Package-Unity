using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameAnalyticsSDK;

[CustomEditor(typeof(TrackingManager))]
[ExecuteInEditMode]
public class TM_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        //Draw the default inspector fields
        DrawDefaultInspector();

        //Add a custom sync button
        GUILayout.Space(20);

        if(GUILayout.Button("Sync Settings", GUILayout.Height(50)))
        {
            TrackingManager.instance.PushSettings();
            Debug.Log("Settings saved!");
        }
    }
}
