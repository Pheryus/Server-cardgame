using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public interface ICommand {
    void execute(object param);
}

public class Card {

    private const string characterType = "Personagem";

    private int id, cost, overcost = 0, cooldown = 1, actual_cooldown;
    private int? attack, life;
    public int original_cost;   

    public Effects onActivateEffect;
    
    private string name, type;
    
    private int actual_damage, actual_life, siege_dmg;


    private bool immuny, invisible, can_act, ranged, charge;

    private Position position;

    private int player_controller;


    public Card (int id, string name, string type, int cost, int damage, int life, int cooldown, int overcost) {
        this.id = id;
        this.cost = cost;
        this.original_cost = cost;
        this.actual_cooldown = 0;
        this.name = name;

        this.cooldown = cooldown;
        this.overcost = overcost;

        this.attack = damage;
        this.life = life;
        this.immuny = false;
        this.invisible = false;
        this.actual_damage = this.attack.GetValueOrDefault();
        this.actual_life = this.life.GetValueOrDefault();
        if (this.attack > 0)
            this.siege_dmg = 1;
        else
            this.siege_dmg = 0;

        this.type = type;
     
    }

    public void setPlayerId(int player_id) {
        this.player_controller = player_id;
    }

    public bool isMagic() {
        return this.type == "Magia";
    }

    public void setSiegeDmg(int siege_dmg) {
        this.siege_dmg = siege_dmg;
    }

    public bool belongsToPlayer(int player_id) {
        return player_id == this.player_controller;
    }

    public void setCharge(bool charge) {
        this.charge = charge;
    }

    public bool hasCharge() {
        return this.charge;
    }

    /// <summary>
    /// Efeito de carta que acontece quando uma criatura entra.
    /// </summary>
    public void onPlayEnter() {
        if (this.hasCharge())
            this.can_act = true;
    }


    public bool canAct() {
        return can_act;
    }
    public Position getPosition() {
        return position;
    }

    public void setRanged(bool range) {
        this.ranged = true;
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

    
    /// <summary>
    /// Função chamada se a carta se encontra na mão do player.
    /// </summary>
    public void endOfTurnInHand() {
        if (this.actual_cooldown > 0) {
            this.actual_cooldown--; 
        }
    }

    public int getActualSiegeDmg() {
        return this.siege_dmg;
    }

    public bool canPlay() {
        return this.actual_cooldown == 0;
    }

    private void resetStatus() {
        if (isCreature()) {
            this.actual_damage = (int)this.attack;
            this.actual_life = (int)this.life;
            this.can_act = false;
        }
        this.actual_cooldown = this.cooldown;
    }

    public void increaseCost() {
        this.cost += this.overcost;
    }

    public void setPosition(Position position) {
        this.position = position;
    }

    public void setCanAct (bool can_act) {
        this.can_act = can_act;
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

    public int? getAttack() {
        return this.attack;
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

    public int getActualAttack() {
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
