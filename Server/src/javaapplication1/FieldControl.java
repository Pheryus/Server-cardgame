package javaapplication1;

import java.util.concurrent.Callable;

/**
 *
 * @author Pedro
 */
public class FieldControl {
    private Field[] players_field;
    
    public FieldControl(){
        players_field = new Field[2];
        players_field[0] = new Field(0);
        players_field[1] = new Field(1);
    }
    
    public Field[] getPlayersField(){
        return players_field;
    }
    
    public void cardsCanAttack(int clientid){
        players_field[clientid].cardsCanAttack();
    }
    
    public boolean haveTarget(Position position){
        return players_field[position.side].haveTarget(position.line, position.column);
    }
    
    public void damageToAllCards(int dmg){
        for (int i = 0; i < 2; i++){
            for (int j = 0; j < 2; j++){
                for (int k = 0; k < 3; j++){
                    Card card = players_field[i].getCard(j, k);
                    if (card != null)
                        card.takeDamage(dmg);
                }
            }
        }
    }
    
    public void damageToPlayerCards(int dmg, int player_id){
        for (int j = 0; j < 2; j++){
                for (int k = 0; k < 3; j++){
                    Card card = players_field[player_id].getCard(j, k);
                    if (card != null)
                        card.takeDamage(dmg);
                }
            }
    }
        
    
    Card getCardFromField(Position position){
        return players_field[position.side].getCard(position.line, position.column);
    }
}
