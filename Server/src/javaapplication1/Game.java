/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package javaapplication1;

import org.json.JSONException;
import org.json.JSONObject;

/**
 *
 * @author Pedro
 */
public class Game {
    
    private final Deck[] deck_players;
    
    private final JSONController jsonController;

    public PlayersCards players_cards;
    private int players_turn;
        
    private final FieldControl fieldControl;
    private final LifeControl lifeControl;
    private final ManaControl manaControl;
    private int turn_number = 0;
    private final Client[] clients;
    private final ActionsControl actionsControl;

    public FieldControl getFieldControl() {
        return fieldControl;
    }


    public LifeControl getLifeControl(){
        return lifeControl;
    }
    
    public ManaControl getManaControl() {
        return manaControl;
    }
    
    public JSONController getJsonController(){
        return jsonController;
    }
    
    public Deck[] getPlayerDecks(){
        return deck_players;
    }
 
    public Game (){
        lifeControl = new LifeControl();
        manaControl = new ManaControl();
        players_cards = new PlayersCards();
        clients = new Client[2];
        jsonController = new JSONController();
        deck_players = new Deck[2];
        deck_players[0] = new Deck(0, this);
        deck_players[1] = new Deck(1, this);
   
        fieldControl = new FieldControl();
        actionsControl = new ActionsControl(this);
    }
    
    public Client[]  getClients(){
        return clients;
    }
    
    public int getOpponentID(int clientid){
        return (clientid + 1) % 2;
    }
    
    public void setClients(Client client, Client client2){
        this.clients[0] = client;
        this.clients[1] = client2;
    }
    

    private boolean isPlayerTurn(int playerid){
        return players_turn == playerid;
    }
    
    private void clientSentString(String message, int clientid) throws JSONException{
        switch (message){
            case "send deck" : {
                System.out.println("id do player: " + clientid);
                Deck player_deck = this.deck_players[clientid];
                clients[clientid].sendDeckToClient(player_deck);
                break;
            }

            case "end turn" : {
                if (players_turn == clientid){
                    this.endTurn();
                    
                }
                break;
            }
        }
    }
    
    void clientSentMessage(String message, int clientid) throws JSONException{
        System.out.println("Message receive: " + message);
        try {
            JSONObject json = new JSONObject(message);
            clientSentJSON(json, message, clientid);
        }
        
        catch (JSONException a){
            clientSentString(message, clientid);
        }
    }
    
    private void clientSentJSON(JSONObject json, String message, int clientid) throws JSONException{
        if (!isPlayerTurn(clientid))
            return;
        
        //checa se é uma magia
        JSONObject card = json.optJSONObject("playspell");
        if (card != null){
            actionsControl.clientCastSpell(message, clientid, card, players_turn);
        }

        card = json.optJSONObject("attack");
        if (card != null){
            actionsControl.clientAttack(clientid, card, players_turn);
        }  

        card = json.optJSONObject("playcreature");
        if (card != null){
            actionsControl.clientPlayCreature(message, clientid, card, players_turn);
        }
    }
    
    private void endTurnEffects(){
        for (Object c : this.deck_players[players_turn].cards){
            Card card = (Card)c;
            card.endTurnInHand();
        }
    }
    
    
    private void endTurn(){
        this.turn_number ++;
        
        this.endTurnEffects();
        
        players_turn = (players_turn + 1 ) % 2;
        fieldControl.cardsCanAttack(players_turn);
        
        if (this.turn_number > 1){
            manaControl.increaseMana(players_turn, this.turn_number);
        }

        clients[0].sendToClient("end turn");
        clients[1].sendToClient("end turn");
        
    }
    

    public static byte[] intToByteArray(int a){
        byte[] ret = new byte[4];
        ret[0] = (byte) (a & 0xFF);
        ret[1] = (byte) ((a >> 8) & 0xFF);
        ret[2] = (byte) ((a >> 16) & 0xFF);
        ret[3] = (byte) ((a >> 24) & 0xFF);
        return ret;
    }
    
    
   
    
}
