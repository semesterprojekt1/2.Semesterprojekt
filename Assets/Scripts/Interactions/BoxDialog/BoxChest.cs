using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxChest : BoxDialog
{

    // Start is called before the first frame update
    void Start()
    {
        initStart(false);
        newDialogLine("Cool, a sword!", 0);
        newDialogLine("It looks pointy, be careful with it", 1);
    }

    // Update is called once per frame
    void Update()
    {
        initUpdate();
    }
}
