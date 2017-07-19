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
        int dmg = -target.getActualAttack();
        
        if (attacker_take_damage)
            attacker.changeActualLife(dmg);
        int attacker_dmg = -attacker.getActualAttack();

        target.changeActualLife(attacker_dmg);
    }

    public bool canAttackPlayer(Card attacker) {

        if (attacker.getSiegeDmg() == 0)
            return false;

        Position attacker_position = attacker.getPosition();

        if (attacker.isRange() && attacker_position.line == 0)
            return true;
        else if (!attacker.isRange() && attacker_position.line == 1)
            return false;

        Position front_creature = new Position(2 - attacker_position.column, 0, 1 - attacker_position.side);

        if (field.getCardByPosition(front_creature) == null) {
            return true;
        }
        return false;
    }

    public bool canAttackCharacter(Card attacker, Card target) {
        Position attacker_position = attacker.getPosition();
        Position target_position = target.getPosition();

        if (attacker.getActualAttack() == 0)
            return false;
        else if (attacker.isRange() && attacker_position.line == 0)
            return true;
        else if (!attacker.isRange() && attacker_position.line == 1)
            return false;

        if (target_position.line == 0)
            return true;
        else {
            Position front_creature = new Position(target_position.column, 0, target_position.side);
            if (target_position.column == attacker_position.column && field.getCardByPosition(front_creature) == null)
                return true;
        }
        return false;

    }


    public void directAttack(Card attacker, int target_side) {
        int life_lost = attacker.getSiegeDmg();
        control.getLifeControl().changeLife(life_lost, target_side);
        attacker.setCanAct(false);
    }

    public void cardAttackCard(Card attacker, Card target) {
        calculateBattle(attacker, target);
        field.check_death(attacker);
        field.check_death(target);
        attacker.setCanAct(false);
    }

    

    public void creaturesCanAttack() {
        for (int i = 0; i < 2; i++) {
            GameObject line = field.getFieldGO().transform.GetChild(i).gameObject;
            for (int j = 0; j < 3; j++) {
                GameObject creature = line.transform.GetChild(j).gameObject;
                if (creature.transform.childCount > 0) {
                    creature = creature.transform.GetChild(0).gameObject;
                    if (creature != null) {
                        creature.GetComponent<CardGOInstance>().card.setCanAct(true);
                    }
                }
            }
        }
    }

}
