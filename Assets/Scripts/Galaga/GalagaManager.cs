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
        public const string PLAYER_TEAM = "red";
        public string Team;
        public GalagaPlayer[] Players;

        private GalagaPlayer _player;

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("started game\n\n");
            Debug.Log(PhotonNetwork.CurrentRoom.Name);

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
                }
                else
                {
                    Debug.Log("does not contain teams key");
                }
            }, null);

            //Debug.Log(PLAYER_TEAM);
            if (Team.Equals("red"))
            {
                _player = Players[0];
            }
            else
            {
                _player = Players[1];
            }
        }

        // Update is called once per frame
        void Update()
        {
            /*
            Touch touch = Input.GetTouch(0);
            _player.Position = touch.position;
            */

            float h = Input.GetAxis("Horizontal");

            Vector3 tempVect = new Vector3(h, 0, 0);
            tempVect = tempVect.normalized * _player.Speed * Time.deltaTime;

            _player.gameObject.GetComponent<RectTransform>().localPosition += tempVect;
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
