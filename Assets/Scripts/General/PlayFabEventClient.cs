using System;
using System.Collections.Generic;
using UnityEngine;

public enum MicrogameEvents : SByte
{
    MicrogameStarted = 0,
    MicrogameFinished,
    UserGainedItem,
    UserMadeUseOfItem,
    UserAttackedByFoe,
    UserLoggedOn,
    UserLoggedOff,
    UserRegisteredLoginCredentials,
    UserDeregisterdLoginCredentials,
    UserGainedScore,
    UserLostScore
}

public class PlayFabEventClient : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Posts a PlayFab PlayStream event. Instead of using this, the intended behavior for posting events is to call a event-specific method (if it exists).
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="data"></param>
    public bool PostPlayStreamEvent(object eventType, Dictionary<string, string> data)
    {
        if (eventType == null || data == null  || !(eventType is SByte))
        {
            Debug.LogError("Invalid argument submitted");
            return false;
        }

        // call PlayFab method here

        return true;
    }

    /// <summary>
    /// Posts a PlayFab telemetry event. Instead of using this, the intended behavior for posting events is to call a event-specific method (if it exists).
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="data"></param>
    public bool PostTelemetryEvent(object eventType, Dictionary<string, string> data)
    {
        if (eventType == null || data == null || !(eventType is SByte))
        {
            Debug.LogError("Invalid argument submitted");
            return false;
        }

        // call PlayFab method here

        return true;
    }
 


}
