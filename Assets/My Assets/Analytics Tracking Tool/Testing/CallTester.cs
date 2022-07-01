using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GameAnalyticsSDK;

public class CallTester : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI outputText;
    [SerializeField]
    private string[] remoteConfigKeys;

    void Awake()
    {
        GameAnalytics.Initialize();
    }

    void OnRemoteConfigUpdate()
    {
        print("Remote config ready: " + GameAnalytics.IsRemoteConfigsReady());

        outputText.text = "Remote config ready: " + GameAnalytics.IsRemoteConfigsReady() + "\nAB testing id: " + GameAnalytics.GetABTestingId() + "\nAB variant testing id: " + GameAnalytics.GetABTestingVariantId();
    }

    /// <summary>
    /// Sends a random test event to the Game Analytics Server.
    /// </summary>
    public void SendGATestEvent()
    {
        TM_EventManager.SendLevelEvent(GAProgressionStatus.Start, "TestLevel", UnityEngine.Random.Range(0, 100));
    }

    public void CheckRemoteConfig()
    {
        if(GameAnalytics.IsRemoteConfigsReady())
        {
            outputText.text = "Remote config is ready.";
        } else {
            outputText.text = "Remote config is not ready.";
        }
    }

    public void GetConfigKeys()
    {
        int counter = 0;
        outputText.text = "";
        foreach (string key in remoteConfigKeys)
        {
            outputText.text += $"GA config key #{counter}: " + GameAnalytics.GetRemoteConfigsValueAsString(key) + "\n";
            counter ++;
        }
    }

    public void GetABTestingID()
    {
        outputText.text = "AB testing id: " + GameAnalytics.GetABTestingId();
    }

    public void GetABVariantID()
    {
        outputText.text = "AB variant testing id: " + GameAnalytics.GetABTestingVariantId();
    }

    void OnEnable()
    {
        GameAnalytics.OnRemoteConfigsUpdatedEvent += OnRemoteConfigUpdate;
    }

    void OnDisable()
    {
        GameAnalytics.OnRemoteConfigsUpdatedEvent += OnRemoteConfigUpdate;
    }
}
