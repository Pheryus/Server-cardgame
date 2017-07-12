package javaapplication1;

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
    
    public boolean haveTarget(int side, int line, int column){
        return players_field[side].haveTarget(line, column);
    }
    
    Card getCardFromField(int clientid, int line, int column){
        return players_field[clientid].getCard(line, column);
    }
}
