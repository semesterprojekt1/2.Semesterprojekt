using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_1_Load : LevelChanger
{
    // Start is called before the first frame update
    void Start()
    {
        InitStart("L1S1");
    }

    // Update is called once per frame
    void Update()
    {
        if ((gm.StoryNumber == 0.09f && inventory.getGotTeddy()) || gm.StoryNumber == 0.10f) {
            gameObject.GetComponent<hedgehole_dialog>().enabled = false;    
            StartCoroutine(ChangeLevel(2f));          
        }
        
    }
}
