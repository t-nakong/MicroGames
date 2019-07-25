using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    public Sprite clearTexture;
    public Sprite defaultTexture;
    public Sprite boomTexture;

    // Start is called before the first frame update
    void Start()
    {
        PlayField.status = "Empty";
    }
    
    // Update is called once per frame
    void Update()
    {
        if (PlayField.status.Equals("Clear!"))
        {
            GetComponent<SpriteRenderer>().sprite = clearTexture;

        }
        else if (PlayField.status.Equals("Empty"))
        {
            GetComponent<SpriteRenderer>().sprite = defaultTexture;
        }
        else if (PlayField.status.Equals("Boom!"))
        {
            GetComponent<SpriteRenderer>().sprite = boomTexture;
        }
    }
}
