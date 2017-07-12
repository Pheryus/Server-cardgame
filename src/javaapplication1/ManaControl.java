
package javaapplication1;

/**
 *
 * @author Pedro
 */
public class ManaControl {
    private int[] players_mana;
    private final int mana_max_per_turn = 7;
    private int mana_per_turn = 3;
    
    private final int first_player_mana = 3;
    private final int second_player_mana = 5;
    
    public ManaControl(){
        players_mana = new int[2];
        players_mana[0] = this.first_player_mana;
        players_mana[1] = this.second_player_mana;
    }
    
    int getPlayerMana(int player){
        return players_mana[player];
    }
    
    void spendMana(Card card, int players_turn){
        players_mana[players_turn] -= card.getCost();
    }
       
    boolean checkAvaliableMana(Card card, int players_turn){
        return players_mana[players_turn] >= card.getCost();
    }
    
    void increaseMana (int players_turn, int turns){
        
        if (players_turn == 0 && mana_per_turn < mana_max_per_turn && turns > 3){
            mana_per_turn++;
        }
        
        players_mana[players_turn] += mana_per_turn;

    }
    
    
}
