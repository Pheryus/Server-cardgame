using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaControl {
    const int initial_mana = 3;
    const int second_initial_mana = 5;

    const int mana_max_per_turn = 7;
    private int mana_per_turn = 3;

    

    private Control control;

    public ManaControl(Control control) {
        this.control = control;
        players_mana = new int[2];

        gainInitialMana();
    }

    private int[] players_mana;

    public void setPlayerMana(int mana) {
        players_mana[control.getPlayerId()] = mana;
    }

    public void setOpponentMana(int mana) {
        players_mana[control.getOpponentId()] = mana;
    }

    private void gainInitialMana() {
        players_mana[0] = initial_mana;
        players_mana[1] = second_initial_mana;
    }

    public bool checkIfItsPlayable(Card c) {
        return players_mana[control.getPlayerId()] >= c.getCost();
    }


    /// <summary>
    /// O jogador algo gasta mana equivalente ao custo da carta.
    /// </summary>
    /// <param name="cost">custo da carta jogada</param>
    public void spendMana(int cost) {
        players_mana[control.getPlayerId()] -= cost;
    }

    public int getPlayerMana() {
        return players_mana[control.getPlayerId()];
    }

    public int getOpponentMana() {
        return players_mana[control.getOpponentId()];
    }

    public void gainMana() {
        players_mana[control.getPlayerTurn()] += mana_per_turn;
        checkAdditionalMana();
    }

    private void checkAdditionalMana() {
        //se o novo turno voltou ao player principal
        if (control.getPlayerTurn() == 1 && mana_per_turn < mana_max_per_turn) {
            Debug.Log("ganha + mana");
            mana_per_turn++;
        }
    }


}
