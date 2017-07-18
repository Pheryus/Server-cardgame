using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UpdateAttributes {

    const string attackGO_name = "attack";
    const string lifeGO_name = "life";
    const string cooldownGO_name = "cooldown";
    const string newcostGO_name = "new_cost";


    public static void updateAttributes(Card card, Transform transform) {

        if (card.getActualAttack() != card.getAttack()) {
            transform.FindChild(attackGO_name).GetComponent<Text>().enabled = true;
            transform.FindChild(attackGO_name).GetComponent<Text>().text = card.getActualAttack().ToString();
        }
        else
            transform.FindChild(attackGO_name).GetComponent<Text>().enabled = false;


        if (card.getActualLife() != card.getLife()) {
            transform.FindChild(lifeGO_name).GetComponent<Text>().enabled = true;
            transform.FindChild(lifeGO_name).GetComponent<Text>().text = card.getActualLife().ToString();
        }
        else
            transform.FindChild(lifeGO_name).GetComponent<Text>().enabled = false;

        if (card.getActualCooldown() != 0) {
            transform.FindChild(cooldownGO_name).GetComponent<Text>().enabled = true;
            transform.FindChild(cooldownGO_name).GetComponent<Text>().text = card.getActualCooldown().ToString();
        }
        else
            transform.FindChild(cooldownGO_name).GetComponent<Text>().enabled = false;

        if (card.getCost() != card.original_cost) {
            transform.FindChild(newcostGO_name).GetComponent<Text>().enabled = true;
            transform.FindChild(newcostGO_name).GetComponent<Text>().text = card.getCost().ToString();
        }
    }
}


public class UpdateCardAttributes : MonoBehaviour {
    private Control control;



    private void getControlReference() {
        GameObject go = GameObject.FindGameObjectWithTag("Control");
        control = go.GetComponent<Serverino>().control;
    }


    void Update () {
	    if (control == null) {
            getControlReference();
        }	    
        else {
            UpdateAttributes.updateAttributes(GetComponent<CardGOInstance>().card, transform);

        }
	}
}
