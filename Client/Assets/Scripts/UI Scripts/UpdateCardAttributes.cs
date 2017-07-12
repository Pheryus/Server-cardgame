using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCardAttributes : MonoBehaviour {
    private Control control;

    const string attackGO_name = "attack";
    const string lifeGO_name = "life";
    const string cooldownGO_name = "cooldown";

    private void getControlReference() {
        GameObject go = GameObject.FindGameObjectWithTag("Control");
        control = go.GetComponent<Serverino>().control;

    }

    private void updateAttributes() {
        Card card = GetComponent<CardInstance>().card;

        if (card.getActualDmg() != card.getDmg()) {
            transform.Find(attackGO_name).GetComponent<Text>().enabled = true;
            transform.Find(attackGO_name).GetComponent<Text>().text = card.getActualDmg().ToString();
        }
        else
            transform.Find(attackGO_name).GetComponent<Text>().enabled = false;


        if (card.getActualLife() != card.getLife()) {
            transform.Find(lifeGO_name).GetComponent<Text>().enabled = true;
            transform.Find(lifeGO_name).GetComponent<Text>().text = card.getActualLife().ToString();
        }
        else
            transform.Find(lifeGO_name).GetComponent<Text>().enabled = false;

        if (card.getActualCooldown() != 0) {
            transform.Find(cooldownGO_name).GetComponent<Text>().enabled = true;
            transform.Find(cooldownGO_name).GetComponent<Text>().text = card.getActualCooldown().ToString();
        }
        else
            transform.Find(cooldownGO_name).GetComponent<Text>().enabled = false;


    }

    void Update () {
	    if (control == null) {
            getControlReference();
        }	    
        else {
            updateAttributes();

        }
	}
}
