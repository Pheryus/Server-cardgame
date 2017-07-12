using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateManaUI : MonoBehaviour {

    Control control;
    public string player = "player_mana";
    void GetControlReference() {
        GameObject go = GameObject.FindGameObjectWithTag("Control");
        control = go.GetComponent<Serverino>().control;
    }

	void FixedUpdate () {
		if (control == null) {
            GetControlReference();
        }
        else {
            if (player == "player_mana")
                GetComponentInChildren<Text>().text = control.getMana().getPlayerMana().ToString();
            else
                GetComponentInChildren<Text>().text = control.getMana().getOpponentMana().ToString();
        }
	}
}
