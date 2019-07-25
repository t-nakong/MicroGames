using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun.MicroGames
{
    public class GalagaManager : MultiplayerManager
    {
  
        
        

        // Update is called once per frame
        void Update()
        {
            if (_gameStarted)
            {
                float h = Input.GetAxis("Horizontal");

                Vector3 tempVect = new Vector3(h, 0, 0);
                tempVect = tempVect.normalized * ((GalagaPlayer)_player).Speed * Time.deltaTime;

                _player.gameObject.GetComponent<RectTransform>().localPosition += tempVect;
            }
        }
    }
}