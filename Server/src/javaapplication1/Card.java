
package javaapplication1;

/**
 *
 * @author Pedro
 */
public class Card {
    
    ICommand onPlayEffect;
    ICommand onDeathEffect;
    
    private Game game_instance;

    private int id;
    private int attack, life, cost;
    
    private int actual_attack, actual_life, actual_siegedmg;
    
    private boolean able_to_attack, ranged;
    
    private String name, type, text;

    public Card (int id, String name, String type,  int cost, int attack, int life, int cooldown, int overcost){
        this.id = id;
        this.type = type;
        this.name = name;
        this.cooldown = 1;
        this.attack = attack;
        this.life = life;
        this.actual_attack = attack;
        this.actual_cooldown = 0;
        this.cost = cost;
        this.actual_life = life;
        this.actual_siegedmg = 1;
        this.ranged = false;
        this.able_to_attack = false;
    }
    
    
    
    public void setGame(Game game){
        this.game_instance = game;
    }
    
    
    public int getAttack() {
        return attack;
    }

    public void setAttack(int attack) {
        this.attack = attack;
    }

    public int getLife() {
        return life;
    }

    public Game getGameInstance(){
        return this.game_instance;
    }
    
    public FieldControl getFieldControl(){
        return this.game_instance.getFieldControl();
    }
    
    public void setLife(int life) {
        this.life = life;
    }
    
    public void takeDamage(int dmg){
        this.actual_life -= dmg;
        if (this.actual_life <= 0){
            if (this.onDeathEffect != null)
                this.onDeathEffect.execute(null);
        }
    }
    
    public boolean isDead(){
        return this.actual_life <= 0;
    }

    public int getCost() {
        return cost;
    }

    public boolean canPlay(){
        return this.actual_cooldown == 0;
    }
    
    public void endTurnInHand(){
        if (this.actual_cooldown > 0)
            this.actual_cooldown -= 1;
    }
    
    
    public void setCost(int cost) {
        this.cost = cost;
    }

    public int getActual_attack() {
        return actual_attack;
    }

    public void setActual_attack(int actual_attack) {
        this.actual_attack = actual_attack;
    }

    public int getActual_life() {
        return actual_life;
    }

    public void setActual_life(int actual_life) {
        this.actual_life = actual_life;
    }

    public int getActual_siegedmg() {
        return actual_siegedmg;
    }

    public void setActual_siegedmg(int actual_siegedmg) {
        this.actual_siegedmg = actual_siegedmg;
    }

    public boolean isAble_to_attack() {
        return able_to_attack;
    }

    public void setAble_to_attack(boolean able_to_attack) {
        this.able_to_attack = able_to_attack;
    }

    public boolean isRanged() {
        return ranged;
    }

    public void setRanged(boolean ranged) {
        this.ranged = ranged;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }

    public String getText() {
        return text;
    }

    public void setText(String text) {
        this.text = text;
    }

    public int getPlayer_id() {
        return player_id;
    }

    public void setPlayer_id(int player_id) {
        this.player_id = player_id;
    }

    public int getCooldown() {
        return cooldown;
    }

    public void setCooldown(int cooldown) {
        this.cooldown = cooldown;
    }

    public int getActual_cooldown() {
        return actual_cooldown;
    }

    public void setActual_cooldown(int actual_cooldown) {
        this.actual_cooldown = actual_cooldown;
    }
    
    private int player_id, cooldown, actual_cooldown;
    

    
    public boolean canAttack(){
        return this.able_to_attack;
    }
    
    public void setAttack(boolean can_attack){
        this.able_to_attack = can_attack;
    }
    
    public int getPlayerId(){
        return this.player_id;
    }
    
    
    public void onPlayed(){
        if (this.onPlayEffect != null)
            onPlayEffect.execute(null);
    }
    
    
    
    public void reset(){
        this.actual_attack = this.attack;
        this.actual_life = this.life;
        this.able_to_attack = false;
        this.actual_cooldown = this.cooldown;
    }
    
    public int getId(){
        return id;
    }
    
    
}
