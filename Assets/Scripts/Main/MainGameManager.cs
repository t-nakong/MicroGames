using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.MicroGames
{
    public class MainGameManager : MonoBehaviourPunCallbacks
    {
        [Header("Load Panel")]
        public GameObject LoadPanel;

        [Header("Team Selection Panel")]
        public GameObject TeamSelectionPanel;

        [Header("Home Panel")]
        public GameObject HomePanel;
        public Button PlayButton;

        [Header("Game Selection Panel")]
        public GameObject GameSelectionPanel;

        [Header("Lobby Panel")]
        public GameObject LobbyPanel;
        public Text GameNameText;
        public Image PlayerRedImage;
        public Text PlayerRedText;
        public Image PlayerBlueImage;
        public Text PlayerBlueText;
        public GameObject SearchingRedText;
        public GameObject SearchingBlueText;
        public GameObject BackButton;
        public Button StartGameButton;

        [Header("Games")]
        public string[] MiniGames;

        [Header("Player")]
        public string PlayerName;
        public string PlayerTeam = "red";

        private Dictionary<int, GameObject> _playerListEntries;
        private Sprite _playerSprite;
        private Sprite _opponentSprite;
        private string _specificGame = "";

        #region UNITY

        #region PUBLIC

        public void SetActivePanel(string activePanel)
        {
            LoadPanel.SetActive(activePanel.Equals(LoadPanel.name));
            TeamSelectionPanel.SetActive(activePanel.Equals(TeamSelectionPanel.name));
            HomePanel.SetActive(activePanel.Equals(HomePanel.name));
            GameSelectionPanel.SetActive(activePanel.Equals(GameSelectionPanel.name));
            LobbyPanel.SetActive(activePanel.Equals(LobbyPanel.name));
        }

        public void SetPlayerName(string name)
        {
            Debug.Log("set name to " + name);

            PlayerName = name;
        }

        #endregion

        #region PRIVATE

        private bool CheckForPlayers()
        {
            return PhotonNetwork.CurrentRoom.PlayerCount == 2;
        }

        private void ResetLobby()
        {
            PlayerRedImage.gameObject.SetActive(false);
            PlayerBlueImage.gameObject.SetActive(false);
            PlayerRedText.gameObject.SetActive(true);
            PlayerBlueText.gameObject.SetActive(true);
            SearchingRedText.SetActive(true);
            SearchingBlueText.SetActive(true);
            BackButton.SetActive(true);
            StartGameButton.interactable = false;
        }

        #endregion

        void Start()
        {
            PlayButton.interactable = false;
            SetActivePanel(LoadPanel.name);
            ResetLobby();
        }

        void Awake()
        {
            PlayerName = "Player" + Random.Range(1000, 10000);

            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LocalPlayer.NickName = PlayerName;
            PhotonNetwork.ConnectUsingSettings();
        }

        #endregion

        #region PUN CALLBACKS

        public override void OnConnectedToMaster()
        {
            Debug.Log("connected to master");
            PlayButton.interactable = true;
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("failed to join random");
            if (_specificGame == "")
                CreateRandomRoom();
            else
                CreateSpecificRoom(_specificGame);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("joined room: " +PhotonNetwork.CurrentRoom.Name);
            Debug.Log(PhotonNetwork.PlayerList.Length + " players in the room");

            if (CheckForPlayers())
            {
                Debug.Log("enough players have joined");
                if (PlayerTeam.Equals("red"))
                {
                    
                    PlayerRedText.text = PlayerName;
                    //PlayerRedImage.sprite = _playerSprite;
                    PlayerBlueText.text = PhotonNetwork.PlayerList[0].NickName;
                    //PlayerBlueImage.sprite = _opponentSprite;
                }
                else
                {
                    PlayerBlueText.text = PlayerName;
                    //PlayerBlueImage.sprite = _playerSprite;
                    PlayerRedText.text = PhotonNetwork.PlayerList[0].NickName;
                    //PlayerRedImage.sprite = _opponentSprite;
                }
                SearchingRedText.SetActive(false);
                SearchingBlueText.SetActive(false);
                PlayerRedImage.gameObject.SetActive(true);
                PlayerBlueImage.gameObject.SetActive(true);

                StartGameButton.interactable = true;
            }
            else
            {
                Debug.Log("not enough players have joined");

                if (PlayerTeam.Equals("red"))
                {
                    PlayerRedText.text = PlayerName;
                    //PlayerRedImage.sprite = _playerSprite;
                    SearchingRedText.SetActive(false);
                    PlayerRedImage.gameObject.SetActive(true);
                }
                else
                {
                    PlayerBlueText.text = PlayerName;
                    //PlayerBlueImage.sprite = _playerSprite;
                    SearchingBlueText.SetActive(false);
                    PlayerBlueImage.gameObject.SetActive(true);
                }
            }
            GameNameText.text = PhotonNetwork.CurrentRoom.Name.Split(new char[] { ' ' })[0].ToUpper();
            SetActivePanel(LobbyPanel.name);
        }

        public override void OnLeftRoom()
        {
            Debug.Log("left room");

            SetActivePanel(GameSelectionPanel.name);
            ResetLobby();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PlayerTeam.Equals("red"))
            {
                PlayerBlueText.text = PhotonNetwork.PlayerList[1].NickName;
                //PlayerBlueImage.sprite = _opponentSprite;
                SearchingBlueText.SetActive(false);
                PlayerBlueImage.gameObject.SetActive(true);
            }
            else
            {
                PlayerRedText.text = PhotonNetwork.PlayerList[1].NickName;
                //PlayerRedImage.sprite = _opponentSprite;
                SearchingRedText.SetActive(false);
                PlayerRedImage.gameObject.SetActive(true);
            }
            BackButton.SetActive(false);
            StartGameButton.interactable = true;
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PlayerTeam.Equals("red"))
            {
                SearchingBlueText.SetActive(true);
                PlayerBlueImage.gameObject.SetActive(false);
            }
            else
            {
                SearchingRedText.SetActive(true);
                PlayerRedImage.gameObject.SetActive(false);
            }
            BackButton.SetActive(true);
            StartGameButton.interactable = false;
        }

        #endregion

        #region UI CALLBACKS

        public void JoinSpecificRoom(string game_name)
        {
            Debug.Log("attempting to join " +game_name +" room");

            string other_team = PlayerTeam.Equals("red") ? "blue" : "red";
            Hashtable expectedRoomProperties = new Hashtable() { { "game_name", game_name }, { "host_team", other_team } };

            PhotonNetwork.JoinRandomRoom(expectedRoomProperties, 2);
        }

        public void JoinRandomRoom()
        {
            Debug.Log("attempting to join random room");

            string other_team = PlayerTeam.Equals("red") ? "blue" : "red";
            Hashtable expectedRoomProperties = new Hashtable() { { "host_team", other_team } };

            PhotonNetwork.JoinRandomRoom(expectedRoomProperties, 2);
        }

        public void CreateSpecificRoom(string game_name)
        {
            Debug.Log("creating " + game_name + " room");

            string roomName = game_name + " " + PlayerName +Random.Range(1000, 10000);
            RoomOptions roomOptions = new RoomOptions();
            string[] roomProperties = { "game_name", "host_team" };
            roomOptions.CustomRoomPropertiesForLobby = roomProperties;
            roomOptions.CustomRoomProperties = new Hashtable() { { "game_name", game_name }, { "host_team", PlayerTeam } };
            roomOptions.MaxPlayers = 2;

            _specificGame = game_name;
            PhotonNetwork.CreateRoom(roomName, roomOptions, null);
        }

        public void CreateRandomRoom()
        {
            Debug.Log("creating random room");

            string game_name = MiniGames[Random.Range(0, MiniGames.Length)];
            string room_name = game_name + " " + PlayerName + Random.Range(1000, 10000);
            RoomOptions roomOptions = new RoomOptions();
            string[] roomProperties = { "game_name", "host_team" };
            roomOptions.CustomRoomPropertiesForLobby = roomProperties;
            roomOptions.CustomRoomProperties = new Hashtable() { { "game_name", game_name }, { "host_team", PlayerTeam } };
            roomOptions.MaxPlayers = 2;

            _specificGame = "";
            PhotonNetwork.CreateRoom(room_name, roomOptions, null);
        }

        public void BacktoSelection()
        {
            Debug.Log("back to selection");
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
                _specificGame = "";
            }

            SetActivePanel(GameSelectionPanel.name);
        }

        public void StartGame()
        {
            Debug.Log("Starting Game");

            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            string miniGame = PhotonNetwork.CurrentRoom.Name.Split(new char[] { ' ' })[0];
            Debug.Log(miniGame);
            PhotonNetwork.LoadLevel(miniGame);
        }

        #endregion
    }
}