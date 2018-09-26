using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class serverclick : MonoBehaviour {

    public void selectype(string scene)
    {
        Debug.Log(scene);
        Application.LoadLevel(scene);
    }

}
