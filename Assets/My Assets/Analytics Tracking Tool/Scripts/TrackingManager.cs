using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;
using System;
// using Facebook.Unity;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

[ExecuteInEditMode]
public class TrackingManager : MonoBehaviour
{
    public static TrackingManager instance;
    [Header("Game Analytics Settings")]
    [SerializeField]
    private string androidKey;
    [SerializeField]
    private string androidSecretKey;
    [SerializeField]
    private string IosKey;
    [SerializeField]
    private string IosSecretKey;
    [SerializeField]
    private RemoteConfigKey[] remoteConfigKeys;
    private Dictionary<string, string> RCKeys;

    [Header("Facebook Settings")]
    [SerializeField]
    private string facebookAppID;
    // [HideInInspector]
    [SerializeField]
    public bool canBuild;
    private bool settingsSynced;

    void Awake()
    {
        if(Application.isPlaying)
        {
            //Initialize the SDKs
            GameAnalytics.Initialize();
            // FB.Init(); //Facebook Unity SDK init crashes on start, not even working in an empty project

            //Update Remote Config Key values
            IEnumerator coro = CheckRemoteConfigReady();
            StartCoroutine(coro);
        }
    }

    // RUNTIME SETTINGS //
    //
    //

    /// <summary>
    /// Returns the Remote Config key value if found, and default value if not. (Make sure it's added on the Game Analytics Console as well).
    /// </summary>
    public string GetKeyValue(string keyName, string defaultValue)
    {
        if(RCKeys.ContainsKey(keyName))
        {
            return RCKeys[keyName];
        } else {
            Debug.Log($"Key \"{keyName}\" not found! Returning default value.");
            return defaultValue;
        }
    }

    void UpdateRemoteConfigKeys() //Updates all key values from Game Analytics
    {
        RCKeys = new Dictionary<string, string>();
        foreach (var key in remoteConfigKeys)
        {
            RCKeys.Add(key.name, GameAnalytics.GetRemoteConfigsValueAsString(key.name, key.keyValue));
        }
    }

    private IEnumerator CheckRemoteConfigReady() //Checks recursively if remote config is ready, then updates values
    {
        yield return new WaitForSeconds(1.0f);
        if(!GameAnalytics.IsRemoteConfigsReady() && Time.realtimeSinceStartup < 60.0f) //If config is not ready for the first 1 minute
        {
            IEnumerator coro = CheckRemoteConfigReady();
            StartCoroutine(coro); //Check again
        } else {
            UpdateRemoteConfigKeys(); //Update config keys' values
        }
    }

    void SetSingleton()
    {
        if(Application.isPlaying)
        {
            if (instance == null)
            {
                instance = this;
            } else {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        } else {
            instance = this;
        }
    }

    void OnEnable()
    {
        SetSingleton();
        
        CheckSettings();
        //Subscribe to Remote Config Update Event
        GameAnalytics.OnRemoteConfigsUpdatedEvent += UpdateRemoteConfigKeys;

        //Load sync status from last session
        JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("TM_Settings"), this); //Set the saved values 
    }
    
    void OnDisable()
    {
        //Unsubscribe from Remote Config Update Event
        GameAnalytics.OnRemoteConfigsUpdatedEvent += UpdateRemoteConfigKeys;
    }

    // EDITOR SETTINGS //
    //
    //

    public void PushSettings()
    {
        //Push game keys to their respective platforms into Game Analytics (GA) settings
        if(!androidKey.Equals(""))
        {
            AddGameKeys(RuntimePlatform.Android, androidKey, androidSecretKey);
        } else if (!IosKey.Equals(""))
        {
            AddGameKeys(RuntimePlatform.IPhonePlayer, IosKey, IosSecretKey);
        } else {
            Debug.Log("Please add game keys before syncing!");
        }
        AddGameAnalyticsObj(); //Add Game Analytics Object to the scene

        //Push Facebook SDK settings
        // SetFBGameID();

        settingsSynced = true;
        CheckSettings();

        //Save settings locally
        string jsonData = JsonUtility.ToJson(this, false);
        PlayerPrefs.SetString("TM_Settings", jsonData);
        PlayerPrefs.Save();
    }

    public void CheckSettings() //Checks if settings are okay for build.
    {
        if(!androidKey.Equals("") && !androidSecretKey.Equals("") || !IosKey.Equals("") && !IosSecretKey.Equals("")) //Check game keys
        {
            if(settingsSynced)
            {
                canBuild = true; //Android or IOS values aren't empty, and latest values have been synced
            } else {
                canBuild = false;
            }
        } else {
            canBuild = false;
        }
    }

    void OnValidate() //Called when a value is changed in the inspector
    {
        if(JsonUtility.ToJson(this, false) != PlayerPrefs.GetString("TM_Settings")) //If settings have changed
        {
            settingsSynced = false;
            CheckSettings();
        }
    }

    //
    //Game Analytics SDK Handler
    //

    //Checks if platform is not added to GA settings, and adds it
    void AddMissingPlatform(RuntimePlatform platform)
    {
        if(!GameAnalyticsSDK.GameAnalytics.SettingsGA.Platforms.Contains(platform)) //Check if the platform is not already added
        {
            GameAnalyticsSDK.GameAnalytics.SettingsGA.AddPlatform(platform); //Add the platform
        }
    }

    //Add game keys to the specified platform
    void AddGameKeys(RuntimePlatform platform, string gameKey, string secretKey)
    {
        int platformIndex = GameAnalyticsSDK.GameAnalytics.SettingsGA.Platforms.FindIndex(x => x == platform); //Get index of current platform

        if(platformIndex == -1) //If the platform is not added to the GA settings
        {
            //Add the platform, then assign its keys
            AddMissingPlatform(platform);
            AddGameKeys(platform, gameKey, secretKey);
        } else {
            //Add keys to GA settings for the specified platform
            GameAnalyticsSDK.GameAnalytics.SettingsGA.UpdateGameKey(platformIndex, gameKey);
            GameAnalyticsSDK.GameAnalytics.SettingsGA.UpdateSecretKey(platformIndex, secretKey);
            GameAnalyticsSDK.GameAnalytics.SettingsGA.Build[platformIndex] = Application.version; //Sync build version with unity's
        }
    }

    public static void AddGameAnalyticsObj()
    {
        if (UnityEngine.Object.FindObjectOfType (typeof(GameAnalytics)) == null) //If object is not in scene
            {
                GameObject go = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(GameAnalytics.WhereIs("GameAnalytics.prefab", "Prefab"), typeof(GameObject))) as GameObject;
                go.name = "GameAnalytics";
                Selection.activeObject = go;
                Undo.RegisterCreatedObjectUndo(go, "Created GameAnalytics Object");
            }
    }

    // 
    //Facebook SDK handler
    // 
    // void SetFBGameID()
    // {
    //     //No method to add multiple platforms by script, so just setting the current platform to FB settings
    //     Facebook.Unity.Settings.FacebookSettings.AppIds = new List<string>{facebookAppID};
    //     Facebook.Unity.Settings.FacebookSettings.AppLabels = new List<string>{Application.productName};
    // }
}

[System.Serializable]
public class RemoteConfigKey //A holder for all remote config keys
{
    public string name;
    public string keyValue;
}