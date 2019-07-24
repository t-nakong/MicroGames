using System;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using Newtonsoft.Json.Linq;

public enum MicrogameEvents : SByte
{
    CompetitionStarted,
    CompetitionFinished,
    MicrogameStarted,
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

public enum PlayFabAuthorityLevel : byte
{
    Character,
    Player,
    User,
    Title
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
    public bool PostPlayStreamEvent(object eventType, in Dictionary<string, object> data, object authorityLevel)
    {
        if (eventType == null || data == null  || !(eventType is SByte) || 
            !(Enum.IsDefined(typeof(MicrogameEvents), eventType)) || !(authorityLevel is byte))
        {
            Debug.LogError("Invalid argument submitted");
            return false;
        }

        switch (authorityLevel)
        {
            case PlayFabAuthorityLevel.Player: case PlayFabAuthorityLevel.User:
                PlayFabClientAPI.WritePlayerEvent(
                    request: new WriteClientPlayerEventRequest
                    {
                        Body = data,
                        EventName = Enum.GetName(typeof(MicrogameEvents), eventType)
                    },
                    resultCallback: OnEventPostSuccess,
                    errorCallback: OnEventPostFailure
                );
                break;

            case PlayFabAuthorityLevel.Title:
                PlayFabClientAPI.WriteTitleEvent(
                    request: new WriteTitleEventRequest
                    {
                        Body = data,
                        EventName = Enum.GetName(typeof(MicrogameEvents), eventType)
                    },
                    resultCallback: OnEventPostSuccess,
                    errorCallback: OnEventPostFailure
                );
                break;
        }

        return true;
    }

    /// <summary>
    /// Posts a PlayFab telemetry event. Instead of using this, the intended behavior for posting events is to call a event-specific method (if it exists).
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="data"></param>
    public bool PostTelemetryEvent(object eventType, in Dictionary<string, object> data, object authorityLevel)
    {
        if (eventType == null || data == null || !(eventType is SByte) ||
            !(Enum.IsDefined(typeof(MicrogameEvents), eventType)) || !(authorityLevel is byte))
        {
            Debug.LogError("Invalid argument submitted");
            return false;
        }

        // call special PlayFab method here ... 

        return true;
    }
 
    /// <summary>
    /// Specialized method for writing the <code>MicrogameEvents.CompetitionStarted</code> PlayStream event
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="sessionId"></param>
    /// <param name="redTeam"></param>
    /// <param name="blueTeam"></param>
    public void PostCompetitionStartedEvent(in string mode, in string sessionId, in List<string> redTeam, in List<string> blueTeam)
    {
        if (!PostPlayStreamEvent(MicrogameEvents.CompetitionStarted, 
            new Dictionary<string, object>()
            {
                { "Mode", mode },
                { "SessionId", sessionId },
                { "RedTeam", new JArray(redTeam) },
                { "BlueTeam", new JArray(blueTeam) }
            },
            PlayFabAuthorityLevel.Title))
        {
            Debug.LogError($"Could not write {Enum.GetName(typeof(MicrogameEvents), MicrogameEvents.CompetitionStarted)} to PlayFab");
        }
    }

    public void PostCompetitionFinishedEvent(in string mode, in string sessionId, List<string> redTeam, in List<string> blueTeam)
    {
        if (!PostPlayStreamEvent(MicrogameEvents.CompetitionStarted, 
            new Dictionary<string, object>()
            {
                { "Mode", mode },
                { "SessionId", sessionId },
                { "RedTeam", new JArray(redTeam) },
                { "BlueTeam", new JArray(blueTeam) }
            },
            PlayFabAuthorityLevel.Title))
        {
            Debug.LogError($"Could not write {Enum.GetName(typeof(MicrogameEvents), MicrogameEvents.CompetitionFinished)} to PlayFab");
        }
    }

    private void OnEventPostSuccess(WriteEventResponse response) { 
    }

    private void OnEventPostFailure(PlayFabError error)
    {
        Debug.LogError("Debug info:");
        Debug.LogError(error.GenerateErrorReport());
    }

}
