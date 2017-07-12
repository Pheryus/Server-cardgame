using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeControl {

    private Control control;
    private int[] life;
    const int initial_life = 6;
    private GameObject[] lifeUI;

    GameObject victory, lose, canvas;

    private int player_id, opponent_id;

	public LifeControl(Control control) {
        this.control = control;
        life = new int[2];
        lifeUI = new GameObject[2];
        life[0] = initial_life;
        life[1] = initial_life;

        this.victory = Resources.Load("win") as GameObject;
        this.lose = Resources.Load("lose") as GameObject;
        this.canvas = GameObject.FindGameObjectWithTag("Canvas");

        this.player_id = control.getPlayerId();
        lifeUI[player_id] = GameObject.FindGameObjectWithTag("playerLifeUI");
        this.opponent_id = control.getOpponentId();
        lifeUI[opponent_id] = GameObject.FindGameObjectWithTag("opponentLifeUI");
    }

    public void changeLife(int life, int side) {
        this.life[side] -= life;

        this.lifeUI[side].GetComponent<Text>().text = this.life[side].ToString();
        this.checkDeath();
    }

    private void checkDeath() {
        if (this.life[player_id] == 0) {
            GameObject.Instantiate(this.victory, canvas.transform);
            Debug.Log("Você perdeu!");
        }
        else if (this.life[opponent_id] == 0) {
            GameObject.Instantiate(this.lose, canvas.transform);
            Debug.Log("Você ganhou!");
        }
    }

}
