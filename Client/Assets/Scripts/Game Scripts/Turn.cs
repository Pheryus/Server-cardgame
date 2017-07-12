using UnityEngine;
using System.Collections;

public class Turn {

    public string phase="";
    public Control creference;
    private bool is_player_turn = false;
    public GameObject card;

    public Turn(Control creference) {
        this.creference = creference;
    }

    public void BeginTurn() {
        ManaPhase();
    }

    private void ManaPhase() {
        phase = "mana";
    }
    




}
