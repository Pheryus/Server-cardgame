﻿using UnityEngine;
using System.Collections;


public class Control {
    private int dsize = 40;

    private int player_id = 0;
    private int opponent_id = 1;
    private int player_turn = 0;
    private bool find_hand = false;

    //array of cards 
    private Hand hand;
    private Field field;
    private LifeControl lifeControl;
    private ManaControl mana;
    private Serverino serverino;
    private BattleControl battleControl;

    private int player1_turns = 0;
    private int player2_turns = 0;

    public LifeControl getLifeControl() {
        return lifeControl;
    }

    public void setOpponentMana(int mana) {
        this.mana.setOpponentMana(mana);
    }

    public ManaControl getMana() {
        return mana;
    }

    public Hand getHand() {
        return hand;
    }

    public int getPlayerTurn() {
        return player_turn;
    }

    public int getPlayerId() {
        return player_id;
    }

    public int getOpponentId() {
        return opponent_id;
    }
    
    public BattleControl getBattleControl() {
        return battleControl;
    }

    public void setPlayerId(int id) {
        player_id = id;
        opponent_id = (player_id + 1) % 2;
    }

    public void spendMana(int manacost) {
        this.mana.spendMana(manacost);
    }

    public Field getField() {
        return field;
    }

	public Control (Serverino server, int player_id) {
        this.setPlayerId(player_id);
        this.serverino = server;
        this.field = new Field(this);
        this.battleControl = new BattleControl(this, field);
        this.lifeControl = new LifeControl(this);
        this.mana = new ManaControl(this);
        this.hand = GameObject.FindGameObjectWithTag("Hand").GetComponent<Hand>();
    }

    public void createCard(int id, int cost, int dmg, int life, Position pos) {

        field.createCard(id, cost, dmg, life, pos, pos.side);
    }

    public void addCardToHand(int id, string type, int cost, int dmg, int life) {
        Card card = new Card(id, type, cost, dmg, life, player_id);
        hand.AddCard(card);
    }

    public void returnCardToHand(Card card) {
        card.resetCard();
        hand.AddCard(card);
    }

    public bool isPlayerTurn() {
        return player_turn == player_id;
    }


    /// <summary>
    /// Muda o turno do jogador atual, e incrementa o número de turnos daquele jogador.
    /// </summary>
    private void changeTurn() {
        player_turn = (player_turn + 1) % 2;
        if (isPlayerTurn()) { 
            this.player2_turns++;
            if (this.player1_turns > 0)
                mana.gainMana();
        }
        else { 
            this.player1_turns++;
            if (this.player2_turns > 0)
                mana.gainMana();
        }

    }

    public void endTurn() {
        if (this.isPlayerTurn()) {
            this.hand.endTurn();
        }
    }
        
    public void newTurn() {
        changeTurn();
        if (this.isPlayerTurn())
            battleControl.creaturesCanAttack();

    }


}


