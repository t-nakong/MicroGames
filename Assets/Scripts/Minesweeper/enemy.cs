using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public Sprite defaultTexture;
    public Sprite affectedTexture;

    // Start is called before the first frame update
    void Start()
    {
        PlayField.enemyStatus = "Default";
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayField.enemyStatus != null)
        {
            if (PlayField.enemyStatus.Equals("Default"))
            {
                GetComponent<SpriteRenderer>().sprite = defaultTexture;

            }
            else if (PlayField.enemyStatus.Equals("Affected"))
            {
                GetComponent<SpriteRenderer>().sprite = affectedTexture;
            }
        }
    }
}
