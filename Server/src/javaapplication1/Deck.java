/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package javaapplication1;
import java.util.ArrayList;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

/**
 *
 * @author Pedro
 */
public class Deck {
    
    public ArrayList cards;
    private int player_id;
    
    private Game game_instance;
    
    public Deck (int player_id, Game game){
        cards = new ArrayList();
        this.game_instance = game;
        this.player_id = player_id;
        this.createDeck();
    }

    
    void createDeck(){
        cards.add(new Card(0, "Yeti", "Personagem", 4, 4, 5, 1, 0));
        cards.add(new Card(1, "Raptor", "Personagem", 2, 3, 2, 1, 0));

        cards.add(new Card(2, "Crocolisk", "Personagem", 2, 2, 3, 1, 0));
        
        cards.add(new Card(3, "Ogre", "Personagem", 6, 6, 7, 1, 0));        
        cards.add(new Card(4, "War Golem", "Personagem", 7, 7, 7, 1, 0));
        
        cards.add(CardInstance.fireball());
        cards.add(CardInstance.flamestrike());
        
        this.setPlayerIdDeck();
        
    }
    
    
    
    private void setPlayerIdDeck(){
        for (int i = 0; i < cards.size(); i++){
            Card card = (Card)cards.get(i);
            card.setPlayer_id(player_id);
            card.setGame(game_instance);
        }
    }

    /*
    "hand" : {
        "card1" : {
            "id"
        },
        "card2
    
    }
    */
    
    
    public JSONObject deckJSON() throws JSONException{
        JSONObject json = new JSONObject();
        JSONObject jsonArray = new JSONObject();
      
        for (int i = 0; i < cards.size(); i++){
            
            JSONObject card_list = new JSONObject();
            int id = ((Card)(cards.get(i))).getId();
            card_list.put("id", id);
            card_list.put("type", ((Card)(cards.get(i))).getType());
            card_list.put("cost", ((Card)(cards.get(i))).getCost());
            card_list.put("dmg", ((Card)(cards.get(i))).getActual_attack());
            card_list.put("life", ((Card)(cards.get(i))).getActual_life());
            
            jsonArray.put("card" + i, card_list);
        }
        
        json.put("hand", jsonArray);  
        
        return json;
    }
    
    public void addCardToHand(Card card){
        
        this.cards.add(card);
        this.cardsDeck();
    }
    
    
    //retorna carta pelo index da mão
    //retorna null se não existir
    public Card cardIsInDeckByIndex(int index){
        //this.cardsDeck();
        if (index < cards.size()){
            Card card = (Card)cards.get(index);
            System.out.println("carta da mao: " + card.getName());
            return card;
        }
        else
            return null;
    }
    
    public void cardsDeck(){
        for (int i=0; i < cards.size(); i++){
            //System.out.println("custo: " + ((Card)(cards.get(i))).cost);
        }
    }
    
    public void playCardByIndex (int index){
        cards.remove(index);
    }
    
    //retorna carta se ela estiver no deck
    //retorna falso se não estiver  
    public Card cardIsInDeck(int id){
        for (int i = 0; i < cards.size(); i++){
            if (((Card)(cards.get(i))).getId() == id){
                return (Card)cards.get(i);
            }
        }
        return null;
    }
   
    
}
