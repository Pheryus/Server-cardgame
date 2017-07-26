using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsControl {


    public Control control;
    private Field field;
    public EffectsControl(Control control) {
        this.control = control;
        this.field = control.getField();
    }

    /// <summary>
    /// Causa dano em todo personagens no campo dos dois jogadores
    /// </summary>
    /// <param name="dmg">Dano causado a todos personagens</param>
    public void damageToField(int dmg) {
        for (int i = 0; i < 2; i++) 
            for (int j = 0; j < 3; j++) 
                for (int k = 0; k < 2; k++) { 
                    Position position = new Position(k, j, i);
                    this.field.damageToPosition(position, dmg);
                }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player_id"></param>
    /// <param name="dmg"></param>
    public void damageToTargetPlayerField (int player_id, int dmg) {
        for (int i = 0; i < 2; i++)
            for (int j = 0; j < 3; j++) {
                Position position = new Position(j, i, player_id);
                this.field.damageToPosition(position, dmg);
            }
    }


    /// <summary>
    /// Aqui podem existir efeitos que interajam com o dano causado, como aumentar o dano com dano magico, etc
    /// </summary>
    /// <param name="position"></param>
    /// <param name="dmg"></param>
    public void damageToTargetPosition(Position position, int dmg) {
        this.field.damageToPosition(position, dmg);

    }

    public void summonGoblins(int player_turn) {
        for (int i = 0; i < 3; i++) {
            Position goblin_position = new Position(0, i, player_turn);
            Card goblin_card = TokenCards.getMeleeGoblin();
            this.field.createCard(goblin_card, goblin_position, player_turn);
        }

        for (int i = 0; i < 3; i++) {
            Position goblin_position = new Position(1, i, player_turn);
            Card goblin_card = TokenCards.getRangedGoblin();
            this.field.createCard(goblin_card, goblin_position, player_turn);
        }
    }

   

}
