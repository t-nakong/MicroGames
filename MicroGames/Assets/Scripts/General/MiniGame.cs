using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGame : MonoBehaviour
{
    public string name;
    public byte numPlayers;

    MiniGame(string _name, byte num_players)
    {
        name = _name;
        numPlayers = num_players;
    }

    public string GetName
    {
        get { return name;  }
    }
    public string SetName
    {
        set { name = value; }
    }

    public byte GetNumPlayers
    {
        get { return numPlayers; }
    }
    public byte SetNumPlayers
    {
        set { numPlayers = value; }
    }
}
