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
public class ActionsControl {
    
    private final Game game;
    
    private final Deck[] deck_players;
    private final ManaControl manaControl;
    private final FieldControl fieldControl;
    private final LifeControl lifeControl;
    private JSONController jsonController;
    private final Client[] clients;
    
    public ActionsControl (Game game){
        this.game = game;
        this.fieldControl = game.getFieldControl();
        this.manaControl = game.getManaControl();
        this.lifeControl = game.getLifeControl();
        this.clients = game.getClients();
        this.deck_players = game.getPlayerDecks();
    }
    
    private void playSpell(Card card, int client_id, JSONObject spellJSON) throws JSONException{
        
        Position target_position = null;
        
        boolean valid_card = true;
        
        
        try {
            JSONObject json_position = spellJSON.getJSONObject("position");
            target_position = this.jsonToPosition(json_position);
        }
        catch (JSONException e){
            
        }
        Card target_card = null;
        
        if (target_position != null){
            target_card = this.getCardFromField(target_position);
            if (target_card == null)
                valid_card = false;
        }

        if (valid_card == true){
            card.onPlayEffect.execute(card);
            manaControl.spendMana(card, client_id);
            sendToClient("Play spell" + Integer.toString(client_id) + " ack", client_id);

            //cria a mensagem do monstro criado + a mana atual do jogador que jogou ela
            String new_message = JSONController.enemyPlayMagic(card.getId(), card, target_position).toString();

            //pega o id do oponente
            int opponentid = (client_id + 1) % 2;

            sendToClient(new_message, opponentid);
        }
    }
    
    
    public void clientCastSpell(String message, int clientid, JSONObject spellJSON, int players_turn) throws JSONException{
        
        int card_id = Integer.parseInt(spellJSON.optString("id"));
        
        Card card = deck_players[players_turn].cardIsInDeck(card_id);
        
        //se carta existir na mão do player
        if (card != null){
            if (manaControl.checkAvaliableMana(card, players_turn) && isPlayerTurn(clientid, players_turn)){                
                this.playSpell(card, players_turn, spellJSON);
            }
            else
                System.out.println("sem mana suficiente");
        }
        else {
            System.out.println("carta nao existe");
        }

    }
    
  
    
    
    void clientAttack (int clientid, JSONObject attackMSG, int players_turn) throws JSONException{   
        
        //checa se é turno do jogador
        if (clientid != players_turn)
            return;
        
        int attacker_id = attackMSG.optInt("attacker_id");
        int target_id = attackMSG.optInt("target_id");
        
        //pega posição do personagem aliado
        JSONObject json_position = attackMSG.getJSONObject("attacker_position");
        Position position = this.jsonToPosition(json_position);
        
        //atacou direto
        if (target_id == -1){
            if (creatureCanAttackDirectly(position)){
                confirmedDirectAttack(position, attackMSG);
            }
        }
        
        else {
            JSONObject target_json_position = attackMSG.getJSONObject("target_position");
            Position target_position = this.jsonToPosition(target_json_position);
            
            if (creatureCanAttack(position, target_position)){
               confirmedAttack(position, target_position, attackMSG);
            }
        }
    }
    
    void confirmedAttack(Position attacker_position, Position target_position, JSONObject attackMSG){
        int client_id = attacker_position.side;
        clients[client_id].sendToClient("Attack" + Integer.toString(client_id) + " ack");

        Card attacker = getCardFromField(attacker_position);
        int opponentid = game.getOpponentID(client_id);
        
        Card target = getCardFromField(target_position);

        if (attacker != null && target != null){
           calculateBattle(attacker, target);
           attacker.setAttack(false);
           String attack_message = jsonController.enemyAttackCharacterJSON(attackMSG).toString();
           sendToClient(attack_message, opponentid);
           checkDeaths();
        }
         
    }
    
    private void checkDeaths(){
        Field[] fields = fieldControl.getPlayersField();
        for (int i=0; i < 2; i++){
            for (int j=0; j < 2; j++){
                for (int k=0; k < 3; k++){
                    Card card = fields[i].getCard(j, k);
                    if (card != null && card.isDead()){
                        card = fields[i].removeCard(j, k);
                        this.returnCardToHand(card);
                        
                    }
                }
            }
        }
    }
    
    private void returnCardToHand(Card card){
        int player_controller = card.getPlayerId();
        
        card.reset();
        
        deck_players[player_controller].addCardToHand(card);
    }
            
    
    /**
     * 
     * @param attacker
     * @param target 
     */
    private void calculateBattle(Card attacker, Card target){
        //atacante toma dano se for ranged e inimigo não for ranged.
        boolean attacker_take_damage = ! (attacker.isRanged() == true && target.isRanged() == false);
        /*
            AQUI OCORRERÃO CHAMADAS DE FUNÇÕES ENVOLVENDO BATALHAS
        */
        
        if (attacker_take_damage)
            attacker.takeDamage(target.getAttack());
        
        target.takeDamage(attacker.getAttack());
        
    }
    
    private boolean isPlayerTurn(int clientid, int player_turn){
        return clientid == player_turn;
    }
    
    private Position jsonToPosition(JSONObject position){
        System.out.println("leitao");
        if (position == null)
            return null;
        
        int line = position.optInt("line");
        int column = position.optInt("column");
        int side = position.optInt("side");
        
        Position pos = new Position(line, column, side);
        return pos;
    }
    
    private boolean positionInFieldIsEmpty(Position pos){
        Field field = fieldControl.getPlayersField()[pos.side];
        
        return field.haveTarget(pos.line, pos.column) == false;
    }
    
    private void playCreature (Deck deck, int hand_id, Card creature_played, Position card_position, int clientid, JSONObject creature) throws JSONException{
            //gasta carta do "deck"
            deck.playCardByIndex(hand_id);
            //gasta mana de acordo com o custo
            manaControl.spendMana(creature_played, clientid);
            
            //recebe mana do jogador que jogou a carta
            int player_mana = manaControl.getPlayerMana(clientid);
            
            if (clientid == 0)
                System.out.println("Cliente " + clientid + " jogou carta carta " +  creature_played.getName() + " na posição (" + card_position.line + " , " + card_position.column);
            //envia mensagem que personagem foi jogado
            sendToClient("PlayCharacter" + Integer.toString(clientid) + " ack", clientid);
            
            //atualiza o field
            fieldControl.getPlayersField()[clientid].summonCreature(creature_played, card_position.line, card_position.column);
            
            //cria a mensagem do monstro criado + a mana atual do jogador que jogou ela
            String message = JSONController.EnemyPlayedCreatureJSON(clientid, creature_played, player_mana, creature).toString();
            //pega o id do oponente
            int opponentid = (clientid+1)%2;

            sendToClient(message, opponentid);
    }
    
    
    void clientPlayCreature(String message, int clientid, JSONObject creature, int players_turn) throws JSONException{
        
        if (this.isPlayerTurn(clientid, players_turn) == false){
            System.out.println("Não é turno do jogador");
            return;
        }
        
        int hand_id = creature.optInt("hand_id");
        Deck deck = deck_players[players_turn];
        Card creature_played = deck.cardIsInDeckByIndex(hand_id);

        if (creature_played != null){
            if (manaControl.checkAvaliableMana(creature_played, players_turn)){
                
                JSONObject json_position = creature.optJSONObject("position");
                
                Position position = this.jsonToPosition(json_position);
                
                if (position != null && this.positionInFieldIsEmpty(position) == true){
                    if (creature_played.canPlay())
                        this.playCreature(deck, hand_id, creature_played, position, clientid, creature);
                }
                else{
                    System.out.println("posicao invalida");
                }
            }
            else
                System.out.println("sem mana suficiente");
        }
        else {
            System.out.println("Criatura não existe!!!");
        }
        
        
    }

    

    
    /**
     * 
     * @param clientid
     * @param line
     * @param column
     * @return 
     */
    private boolean creatureCanAttackDirectly(Position position){
        
        Card card = getCardFromField(position);
        int opponent_id = game.getOpponentID(position.side);
        if (card != null && card.isAble_to_attack()){
            
            Position front_creature = new Position (0, 2 - position.column, opponent_id);
            
            if (card.isRanged() == true && position.line == 0)
                return true;
            else if (card.isRanged() == false && position.line == 1)
                return false;
            //se não tiver ninguem na mesma coluna dele, pode atacar direto
            else if (getCardFromField(front_creature) == null)
                return true;
        }
        return false;
    }
    
    private Card getCardFromField(Position position){
        return fieldControl.getCardFromField(position);
    }
    
    //retorna true se a carta pode atacar
    private boolean creatureCanAttack(Position position, Position target_position){
        Card card = getCardFromField(position);
        int opponent_id = game.getOpponentID(position.side);
        
        
        Card target = getCardFromField(target_position);
        
        System.out.println("Target " + target.getName());
        
        if (card != null && card.isAble_to_attack() && target != null){
            //se estiver atras e nao for ranged, nao pode atacar
            if (!card.isRanged() && position.line == 1)
                return false;
            
            //se for ranged e estiver na frente, ataca quaqluer um
            else if (card.isRanged() && position.line == 0)
                    return true;
            
            else {
                
                //se o alvo estiver na frente, pode atacar
                if (target_position.line == 0)
                    return true;
                //se o alvo estiver na backline, sem ninguem na frente pra protege-lo
                else{
                    Position front_creature = new Position (0, target_position.column, opponent_id);
                    return target_position.column == position.column && getCardFromField(front_creature) == null;
                }
            }
        }
            
        return false;
    }
    
    private void sendToClient(String message,  int client){
        this.clients[client].sendToClient(message);
    }
    
    private void confirmedDirectAttack(Position position, JSONObject message){
        int opponent_id = game.getOpponentID(position.side);
        Card attacker = getCardFromField(position);
        if (attacker != null){
            attacker.setAttack(false);
            this.lifeControl.damagePlayer(attacker, opponent_id);
            int player_life = this.lifeControl.getPlayerLife(opponent_id);
            
            sendToClient("Attack" + Integer.toString(position.side) + " ack", position.side);
            
            String attack_message = JSONController.enemyAttackPlayerJSON(message, player_life).toString();
            sendToClient(attack_message, opponent_id);
        }
    }
    
    
    
}
