using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;

public class TM_EventManager : MonoBehaviour
{
    //Current options: level start, end, or fail + level name + player score.
    public static void SendLevelEvent(GAProgressionStatus levelStatus, string levelName, int playerScore)
    {
        GameAnalytics.NewProgressionEvent(levelStatus, levelName, playerScore); //Sends the event to GA SDK
    }
}