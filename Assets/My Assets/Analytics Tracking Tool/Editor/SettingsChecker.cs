using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class SettingsChecker : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildReport report) //Called before building
    {
        CheckBuildStatus();
    }
    
    void CheckBuildStatus()
    {
        if(!TrackingManager.instance.canBuild)
        {
            EditorUtility.DisplayDialog("Error", "Please add and sync your game settings before build.", "Ok");
            throw new BuildFailedException("Please add and sync your game settings before building.");
        }
    }
}