using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//funções de cálculos de batalha
public class BattleControl {

    private Control control;
    private Field field;

    public BattleControl(Control control, Field field) {
        this.control = control;
        this.field = field;
    }

    private void calculateBattle(Card attacker, Card target) {
        //atacante toma dano se for ranged e inimigo não for ranged.
        bool attacker_take_damage = !(attacker.isRange() && !target.isRange());
        /*
            AQUI OCORRERÃO CHAMADAS DE FUNÇÕES ENVOLVENDO BATALHAS
        */
        int dmg = -target.getActualDmg();
        
        if (attacker_take_damage)
            attacker.changeActualLife(dmg);
        int attacker_dmg = -attacker.getActualDmg();

        target.changeActualLife(attacker_dmg);
    }

    public void directAttack(Card attacker, int target_side) {
        int life_lost = attacker.getSiegeDmg();
        control.getLifeControl().changeLife(life_lost, target_side);
        attacker.setCanAttack(false);
    }

    public void cardAttackCard(Card attacker, Card target) {
        calculateBattle(attacker, target);
        checkDeath(attacker);
        checkDeath(target);
        attacker.setCanAttack(false);
    }

    public void checkDeath(Card card) {
        if (card.getActualLife() <= 0) {
            Position position = card.getPosition();
            field.removeCard(position);
            field.removeCardFromGameObject(position);
            int player_id = this.control.getPlayerId();
            if (card.belongsToPlayer(player_id)) {
                control.returnCardToHand(card);
            }

        }
    }

    public void creaturesCanAttack() {
        for (int i = 0; i < 2; i++) {
            GameObject line = field.getFieldGO().transform.GetChild(i).gameObject;
            for (int j = 0; j < 3; j++) {
                GameObject creature = line.transform.GetChild(j).gameObject;
                if (creature.transform.childCount > 0) {
                    creature = creature.transform.GetChild(0).gameObject;
                    if (creature != null) {
                        creature.GetComponent<CardInstance>().card.setCanAttack(true);
                    }
                }
            }
        }
    }

}
