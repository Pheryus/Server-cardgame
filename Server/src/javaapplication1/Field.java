
package javaapplication1;
import java.util.*;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

/**
 *
 * @author Pedro
 */
public class Field {
    
    Card[] frontline;
    Card[] backline;
    
    private final int id;
    
    public Field(int player_id){
        this.id = player_id;
        frontline = new Card[3];
        backline = new Card[3];
    }
    
    public Card getCard(int line, int column){
        if (line == 0 )
            return frontline[column];
        else
            return backline[column];
    } 
    
    public Card removeCard(int line, int column){
        Card card;
        if (line == 0){
            card = frontline[column];
            frontline[column] = null;
        }
        else{
            card = backline[column];
            backline[column] = null;
        }
        return card;
    }
    
    boolean haveTarget(int line, int column){
        if (line == 0){
            if (frontline[column] != null)
                return true;
        }
        
        else if (backline[column] != null)
            return true;
        
        return false;
    }
    
    JSONObject fieldJSON() throws JSONException{
        JSONObject json1 = new JSONObject();
        JSONObject json2 = new JSONObject();
        JSONArray jsonArray = new JSONArray();
        
        for (int i = 0; i < frontline.length; i++){
            JSONObject card_list = new JSONObject();
            int id = frontline[i].getId();
            card_list.put("id", id);
            card_list.put("type", frontline[i].getType());
            card_list.put("cost", frontline[i].getCost());
            card_list.put("dmg", frontline[i].getActual_attack());
            card_list.put("life", frontline[i].getActual_life());
            jsonArray.put(card_list);
        }
        
        json1.put("hand", jsonArray);  
        
        for (int i = 0; i < backline.length; i++){
            JSONObject card_list = new JSONObject();
            int id = backline[i].getId();
            card_list.put("id", id);
            card_list.put("type", backline[i].getType());
            card_list.put("cost", backline[i].getCost());
            card_list.put("dmg", backline[i].getActual_attack());
            card_list.put("life", backline[i].getActual_life());
            jsonArray.put(card_list);
        }
        
        json2.put("hand", jsonArray);  
        
        JSONObject jsonField = new JSONObject();
        jsonField.put("frontline", json1);
        jsonField.put("backline", json2);
        
        return jsonField;
    }
    
    public void cardsCanAttack(){
        for (int i=0; i < frontline.length; i++){
            if (frontline[i] != null){
                frontline[i].setAttack(true);
            }
        }
        for (int i=0; i < backline.length; i++){
            if (backline[i] != null){
                backline[i].setAttack(true);
            }
        }
    }
    
    public void summonCreature(Card card, int line, int column){
        if (line == 0)
            frontline[column] = card;
        else
            backline[column] = card;
        
        printField();
    }
    
    private void printField(){
        System.out.println("-----Frontline---");
        for (int i=0; i < 3; i++){
            System.out.println("i: " + i + " - " + frontline[i]);
        }
    }
    
}
