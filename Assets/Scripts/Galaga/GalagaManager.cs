using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

namespace Photon.Pun.MicroGames
{
    public class GalagaManager : MonoBehaviourPunCallbacks
    {
        public string Team = "red";
        public GalagaPlayer[] Players;

        [Header("UI")]
        public GameObject InstructionPanel;
        public Button StartGameButton;

        private GalagaPlayer _player;
        private bool _gameStarted;

        public void StartGame()
        {
            _gameStarted = true;
            InstructionPanel.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("started game\n\n");
            Debug.Log(PhotonNetwork.CurrentRoom.Name);

            StartGameButton.interactable = false;
            InstructionPanel.SetActive(true);

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

        // Update is called once per frame
        void Update()
        {
            if (_gameStarted)
            {
                float h = Input.GetAxis("Horizontal");

                Vector3 tempVect = new Vector3(h, 0, 0);
                tempVect = tempVect.normalized * _player.Speed * Time.deltaTime;

                _player.gameObject.GetComponent<RectTransform>().localPosition += tempVect;
            }
        }

        #region PUN CALLBACKS

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel("Main");
        }

        #endregion
    }
}
