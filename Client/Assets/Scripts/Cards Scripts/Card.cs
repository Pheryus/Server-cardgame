using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public interface ICommand {
    void execute(object param);
}

public class Card {

    private const string characterType = "Personagem";

    private int id, cost, overcost = 0, cooldown = 1, actual_cooldown;
    private int? damage, life;

    public ICommand onPlayEffect;
    

    private string name, type;
    
    //actual status
    private int actual_damage, actual_life, siege_dmg;


    private bool immuny, invisible, can_attack, ranged;

    private Position position;

    private int player_controller;

    //construct
    public Card (int id, string name, string type, int cost, int damage, int life, int cooldown, int overcost) {
        this.id = id;
        this.cost = cost;
        this.actual_cooldown = 0;
        this.name = name;

        this.cooldown = cooldown;
        this.overcost = overcost;

        this.damage = damage;
        this.life = life;
        this.immuny = false;
        this.invisible = false;
        this.actual_damage = this.damage.GetValueOrDefault();
        this.actual_life = this.life.GetValueOrDefault();
        this.siege_dmg = 1;

        this.type = type;

        //getting effects
        //eff = CardEffects.getEffect(id, type);
     
    }

    public void setPlayerId(int player_id) {
        this.player_controller = player_id;
    }

    public bool isMagic() {
        return this.type == "Magia";
    }

    public bool belongsToPlayer(int player_id) {
        return player_id == this.player_controller;
    }

    public bool canAttack() {
        return can_attack;
    }
    public Position getPosition() {
        return position;
    }

    public int getPlayerId() {
        return this.player_controller;
    }
	
    public bool isCreature() {
        return this.type == characterType;
    }

    public void resetCard() {
        this.resetStatus();
        this.increaseCost();
    }

    public void endOfTurnInHand() {
        if (this.actual_cooldown > 0) {
            this.actual_cooldown--; 
        }
    }

    public bool canPlay() {
        return this.actual_cooldown == 0;
    }


    private void resetStatus() {
        if (isCreature()) {
            this.actual_damage = (int)this.damage;
            this.actual_life = (int)this.life;
            this.can_attack = false;
        }
        this.actual_cooldown = this.cooldown;
    }

    public void increaseCost() {
        this.cost += this.overcost;
    }



    public void setPosition(Position position) {
        this.position = position;
    }

    public void setCanAttack (bool canattack) {
        can_attack = canattack;
    }

    public int getSiegeDmg() {
        return siege_dmg;
    }

    public int getID(){
        return id;
    }

    public bool isDead() {
        return this.actual_life <= 0;
    }

    public bool isRange() {
        return ranged;
    }

    public bool isImmuny() {
        return immuny;
    }

    public bool isInvisible() {
        return invisible;
    }

    public int? getDmg() {
        return this.damage;
    }

    public int? getLife() {
        return this.life;
    }

    public int getCooldown() {
        return this.cooldown;
    }

    public int getActualCooldown() {
        return this.actual_cooldown;
    }

    public int getActualDmg() {
        return actual_damage;
    }

    public int getActualLife() {
        return actual_life;
    }

    public void changeActualLife(int life) {
        actual_life += life;
        if (actual_life > this.life)
            actual_life = life;
    }

    public void dealDamage(int dmg) {
        this.actual_life -= dmg;
    }


    public int getCost() {
        return cost;
    }

}
