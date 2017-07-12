package javaapplication1;

import java.util.logging.Level;
import java.util.logging.Logger;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

/**
 *
 * @author Pedro
 */
public final class JSONController {
    
        /*
    	"attack" : {
		"attacker_id" : attacker_id,
		"target_id" : target_id,
		"attacker_position" : {
            "side" : side,
			"line" : line,
			"column" : column
		},
		"target_position" : {
			"line" : line,
			"column" : column
		}	
	}
    */
    static JSONObject playerslifeJSON(int[] players_life) throws JSONException{
        JSONObject playerlifes = new JSONObject();
        JSONArray playerlifeArray = new JSONArray();
        
        JSONObject player1life = new JSONObject();
        player1life.put("life1", players_life[0]);
        
        JSONObject player2life = new JSONObject();
        player1life.put("life2", players_life[1]);
        
        playerlifeArray.put(player1life);
        playerlifeArray.put(player2life);
        playerlifes.put("playerslife:", playerlifeArray);
        return playerlifes;
    }
    
    static JSONObject playershandsizeJSON(Deck[] deck_players) throws JSONException {
        
        JSONObject playershandsize = new JSONObject();
        
        JSONArray playershandsizeArray = new JSONArray();
        
        JSONObject player1handsize = new JSONObject();
        JSONObject player2handsize = new JSONObject();
        
        player1handsize.put("hand_size1", deck_players[0].cards.size());
        player1handsize.put("hand_size2", deck_players[1].cards.size());
        
        playershandsizeArray.put(player1handsize);
        playershandsizeArray.put(player2handsize);
        
        playershandsize.put("playersHandSize", playershandsizeArray);
        return playershandsize;
    }
    
    
    //retorna o numero de mana dos players seguindo o padrão JSON
    /*    
    {
    playersmana: {
        "mana1" : mana1,
        "mana2" : mana2
        }
    }
    */
    
    static JSONObject playersmanaJSON(int[] players_mana) throws JSONException {
        JSONObject playersmana = new JSONObject();
        JSONArray playermanaArray = new JSONArray();
        
        JSONObject player1mana = new JSONObject();
        player1mana.put("mana1", players_mana[0]);
        
        JSONObject player2mana = new JSONObject();
        player1mana.put("mana2", players_mana[1]);
        
        playermanaArray.put(player1mana);
        playermanaArray.put(player2mana);
        playersmana.put("playersmana:", playermanaArray);
        
        return playersmana;
        
    }
    
    
    static JSONObject enemyAttackPlayerJSON(JSONObject json, int playerlife){
        JSONObject attackJSON = new JSONObject();
        try {
            json.put("player_life", playerlife);
            attackJSON.put("attack", json);   
        } catch (JSONException ex) {
            Logger.getLogger(Game.class.getName()).log(Level.SEVERE, null, ex);
        }
        return attackJSON;
    }
    
    static JSONObject enemyAttackCharacterJSON(JSONObject json){
        JSONObject attackJSON = new JSONObject();
        
        try {
            attackJSON.put("attack", json);
        }catch (JSONException e){
        }
        return attackJSON;
    }
    
   
    static JSONObject EnemyPlayedCreatureJSON(int playerid, Card card_played, int mana, JSONObject json) throws JSONException{
        JSONObject creature_playedJSON = new JSONObject();
        
        json.put("cost", card_played.getCost());
        
        json.put("enemymana", mana);
        json.put("dmg", card_played.getDamage());
        json.put("life", card_played.getLife());
        
        creature_playedJSON.put("playcreature", json);
        return creature_playedJSON;
    }
    
    
}
