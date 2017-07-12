using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour {

    bool is_clicked = false;

    public void ButtonClick() {
        if (!is_clicked) { 
            Serverino server = GameObject.FindGameObjectWithTag("Control").GetComponent<Serverino>();
            server.setState(server.STATE_STARTGAME);
            is_clicked = true;
        }

    }

}
