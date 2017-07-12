/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package javaapplication1;

/**
 *
 * @author Pedro
 */
public class LifeControl {
    
    private int[] players_life;
    private final int initial_life = 7;
    
    public LifeControl(){
        players_life = new int[2];
        players_life[0] = players_life[1] = initial_life;
    }
    
    public int getPlayerLife(int player){
        return players_life[player];
    }
    
    //causa dano no player de acordo com o ataque siege
    public void damagePlayer(Card card, int player_id){
        players_life[player_id] -= card.getActual_siegedmg();
    }
    
}
