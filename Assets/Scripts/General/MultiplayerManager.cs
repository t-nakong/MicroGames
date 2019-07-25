using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

namespace Photon.Pun.MicroGames
{
    public abstract class MultiplayerManager : MonoBehaviourPunCallbacks
    {
        public string Team = "red";
        public MultiPlayer[] Players;

        [Header("UI")]
        public GameObject InstructionPanel;
        public Button StartGameButton;

        protected MultiPlayer _player;
        private protected bool _gameStarted;

        public void StartGame()
        {
            _gameStarted = true;
            InstructionPanel.SetActive(false);
        }

        private void GetTeam()
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest
            {
                Keys = new List<string> { "Team" }
            }, (request) =>
            {
                Debug.Log("retreived player data");
                if (request.Data.ContainsKey("Team"))
                {
                    Team = request.Data["Team"].Value;
                    Debug.Log("player is on " + Team + " team");
                    if (Team.Equals("red"))
                    {
                        _player = Players[0];
                    }
                    else
                    {
                        _player = Players[1];
                    }
                    StartGameButton.interactable = true;
                }
                else
                {
                    Debug.Log("does not contain teams key");
                }
            }, null);
        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("started game\n\n");

            if (PhotonNetwork.IsConnected)
            {
                Debug.Log(PhotonNetwork.CurrentRoom.Name);
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
            }

            if (PlayFabClientAPI.IsClientLoggedIn())
            {
                Debug.Log("client is logged in");
                GetTeam();
            }
            else
            {
                Debug.Log("client not logged in");
            }

            StartGameButton.interactable = false;
            InstructionPanel.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        #region PlayFab

        private void LoginWithDeviceId()
        {
            // LoginWithAndroidDeviceId, then if user doesn't have account, prompt them to create one (SHOULD IMPLEMENT LATER)

            PlayFabClientAPI.LoginWithAndroidDeviceID(
                request: new LoginWithAndroidDeviceIDRequest
                {
                    AndroidDevice = SystemInfo.deviceModel,
                    AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
                    CreateAccount = true,
                    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
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
                    TitleId = PlayFabSettings.TitleId
                },
                resultCallback: OnLoginSuccess,
                errorCallback: OnLoginFailure
                );

        }

        #endregion

        #region PUN CALLBACKS

        public override void OnConnectedToMaster()
        {
            Debug.Log("connected to master");
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel("Main");
        }

        #endregion

        #region PLAYFAB CALLBACKS

        public void OnLoginSuccess(LoginResult result)
        {
            Debug.Log("logged into PlayFab");
            GetTeam();
        }

        public void OnLoginFailure(PlayFabError error)
        {
            Debug.Log("log in to PlayFab failed");
        }

        #endregion
    }
}
