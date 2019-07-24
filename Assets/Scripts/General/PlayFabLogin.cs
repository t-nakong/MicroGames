using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabUser
{
    public string Id;
    public Dictionary<string, object> Data;
    public List<ItemInstance> Inventory;
    public Dictionary<string, int> VirtualCurrency;
}

/// <summary>
/// Script that handles PlayFab logins, to be attached to a main menu object/screen
/// </summary>
public class PlayFabLogin : MonoBehaviour
{
    /// when a user logs out, all of this info must be reset
    public string SessionTicket { get; private set; } = null;
    public EntityKey EntityKey { get; private set; } = null;
    public PlayFabUser StartingUser { get; private set; } = null;
    public Dictionary<string, string> StartingTitleData { get; private set; } = null;

    // Start is called before the first frame update
    void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            // set this based on what I saw in Nathan's project settings, so this could be wrong
            PlayFabSettings.TitleId = "A9088";
        }

        PromptUserToLogin(out bool shouldDoBlindLogin);
        if (shouldDoBlindLogin)
        {
            LoginWithDeviceId();
        }
        else
        {
            // would use PlayFabClientAPI.LoginWithPlayFabRequest(...) call to perform login, but it should probably be executed from the hosting user
        }

    }

    /// <summary>
    /// Theoretically would show UI buttons to user, and if they click the 'Play without an account' option, email address sign-up/login would be foregone (until later) 
    /// </summary>
    /// <param name="shouldDoBlindLogin">Is set <code>true</code> if user wants to start playing without logging in</param>
    void PromptUserToLogin(out bool shouldDoBlindLogin)
    {
        shouldDoBlindLogin = true;
    }

    /// <summary>
    /// Logs in the user, then if successful gets the EntityKey and---if the user previously had an account---the player data as well
    /// </summary>
    void LoginWithDeviceId()
    {
        // LoginWithAndroidDeviceId, then if user doesn't have account, prompt them to create one (SHOULD IMPLEMENT LATER)
        PlayFabClientAPI.LoginWithAndroidDeviceID(
            request: new LoginWithAndroidDeviceIDRequest
            {
                AndroidDevice = SystemInfo.deviceModel,
                AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true,
                InfoRequestParameters =
                {
                    GetCharacterInventories = false,
                    GetCharacterList = false,
                    GetPlayerProfile = false,
                    GetPlayerStatistics = false,
                    GetTitleData = true,
                    GetUserData = true,
                    GetUserInventory = true,
                    GetUserReadOnlyData = false,
                    GetUserVirtualCurrency = true
                },
                OS = SystemInfo.operatingSystem, // OS
                TitleId = "A9088" // TitleId, 
            },
            resultCallback: OnLoginSuccess,
            errorCallback: OnLoginFailure
            );

    }

    /// <summary>
    /// Resets all of the user specific login info. Intended to be used when the user logs out, but MAKE SURE TO UPDATE ALL RELEVANT DATA TO PLAYFAB FIRST
    /// </summary>
    void ClearLoginInfo()
    {
        SessionTicket = null;
        EntityKey = null;
        StartingUser = null;
        StartingTitleData = null;
    }

    // why do we need EntityToken AND SessionTicket?
    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log($"PLAYER {SystemInfo.deviceUniqueIdentifier} succesfully logged on");
        Debug.Log($"Player ID {result.PlayFabId} is device unique id");
        EntityKey = result.EntityToken.Entity;
        StartingUser = new PlayFabUser { Id = result.PlayFabId };
        StartingTitleData = result.InfoResultPayload.TitleData;

        // if existing player, fill out the latest player data
        if (!result.NewlyCreated)
        {
            foreach (var datum in result.InfoResultPayload.UserData)
            {
                StartingUser.Data.Add(datum.Key, datum.Value.Value);
            }

            StartingUser.VirtualCurrency = result.InfoResultPayload.UserVirtualCurrency;
            StartingUser.Inventory = result.InfoResultPayload.UserInventory;
        }

        // thought: we should probably have client update users' data after every game

        // could have UI display options right after this
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Debug info:");
        Debug.LogError(error.GenerateErrorReport());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
