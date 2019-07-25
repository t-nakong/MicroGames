using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun.MicroGames
{
    public class GalagaManager : MonoBehaviour
    {
        public const string PLAYER_TEAM = "red";
        public GalagaPlayer[] Players;

        private GalagaPlayer _player;

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("started game");
            Debug.Log(PLAYER_TEAM);
            if (PLAYER_TEAM.Equals("red"))
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
    }
}
