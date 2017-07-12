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
    
    void clientCastSpell(String message, int clientid, JSONObject spell, int players_turn){
        
        int card_id = Integer.parseInt(spell.optString("id"));
        Card card = deck_players[players_turn].cardIsInDeck(card_id);
        //se carta existir na mão do player
        if (card != null){
            if (manaControl.checkAvaliableMana(card, players_turn)){
                JSONObject position = spell.optJSONObject("position"); 
                if (position != null){
                    int line = Integer.parseInt(position.optString("line"));
                    int column = Integer.parseInt(position.optString("column"));
                    int side = Integer.parseInt(position.optString("side"));
                    if (fieldControl.haveTarget(side, line, column)){

                    }
                    else{
                        System.out.println("posicao invalida");
                        return;
                    }
                }
                manaControl.spendMana(card, players_turn);

                //ativa carta
                //envia ack pro cliente
            }
            else
                System.out.println("sem mana suficiente");
        }
        else {
            System.out.println("carta nao existe");
        }

    }
    
    void clientAttack (int clientid, JSONObject attackMSG, int players_turn){   
        
        //checa se é turno do jogador
        if (clientid != players_turn)
            return;
        
        int attacker_id = attackMSG.optInt("attacker_id");
        int target_id = attackMSG.optInt("target_id");
        
        //pega posição do personagem aliado
        JSONObject position = attackMSG.optJSONObject("attacker_position");
        int line = position.optInt("line");
        int column = position.optInt("column");
        
        //atacou direto
        if (target_id == -1){
            if (creatureCanAttackDirectly(clientid, line, column)){
                confirmedDirectAttack(clientid, line, column, attackMSG);
            }
        }
        
        else {
            //pega posição do inimigo
            JSONObject target_position = attackMSG.optJSONObject("target_position");
            System.out.println(attackMSG);
            int target_line = target_position.optInt("line");
            int target_column = target_position.optInt("column");
            if (creatureCanAttack(clientid, line, column, target_line, target_column)){
                System.out.println("pode atacar, comandante!");
               confirmedAttack(clientid, line, column, target_line, target_column, attackMSG);
            }
        }
    }
    
    void confirmedAttack(int clientid, int line, int column, int target_line, int target_column, JSONObject attackMSG){
         clients[clientid].sendToClient("Attack" + Integer.toString(clientid) + " ack");
         Card attacker = getCardFromField(clientid, line, column);
         int opponentid = game.getOpponentID(clientid);
         Card target = getCardFromField(opponentid, target_line, target_column);
         
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
            attacker.reduceActual_life(target.getDamage());
        
        target.reduceActual_life(attacker.getDamage());
        
    }
    
    private boolean isPlayerTurn(int clientid, int player_turn){
        return clientid == player_turn;
    }
    
    private Position jsonToPosition(JSONObject position){
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
    private boolean creatureCanAttackDirectly(int clientid, int line, int column){
        Card card = getCardFromField(clientid, line, column);
        int opponent_id = game.getOpponentID(clientid);
        if (card != null && card.isAble_to_attack()){
            if (card.isRanged() == true && line == 0)
                return true;
            else if (card.isRanged() == false && line == 1)
                return false;
            //se não tiver ninguem na mesma coluna dele, pode atacar direto
            else if (getCardFromField(opponent_id, 0, 2 - column) == null)
                return true;
        }
        return false;
    }
    
    private Card getCardFromField(int clientid, int line, int column){
        return fieldControl.getCardFromField(clientid, line, column);
    }
    
    //retorna true se a carta pode atacar
    private boolean creatureCanAttack(int clientid, int line, int column, int targetline, int targetcolumn){
        Card card = getCardFromField(clientid, line, column);
        int opponent_id = game.getOpponentID(clientid);
        System.out.println("Attacker " + card.getName());
        Card target = getCardFromField(opponent_id, targetline, targetcolumn);
        System.out.println("Target " + target.getName());
        if (card != null && card.isAble_to_attack() && target != null){
            //se estiver atras e nao for ranged, nao pode atacar
            if (!card.isRanged() && line == 1)
                return false;
            
            //se for ranged e estiver na frente, ataca quaqluer um
            else if (card.isRanged() && line == 0)
                    return true;
            
            else {
                //se o alvo estiver na frente, pode atacar
                if (targetline == 0)
                    return true;
                //se o alvo estiver na backline, sem ninguem na frente pra protege-lo
                else return targetcolumn == column && getCardFromField(opponent_id, 0, targetcolumn) == null;
            }
        }
            
        return false;
    }
    
    private void sendToClient(String message,  int client){
        this.clients[client].sendToClient(message);
    }
    
    private void confirmedDirectAttack(int clientid, int line, int column, JSONObject message){
        int opponent_id = game.getOpponentID(clientid);
        Card attacker = getCardFromField(clientid, line, column);
        if (attacker != null){
            attacker.setAttack(false);
            System.out.println("Atacou diretamente!");
            this.lifeControl.damagePlayer(attacker, opponent_id);
            int player_life = this.lifeControl.getPlayerLife(opponent_id);
            
            sendToClient("Attack" + Integer.toString(clientid) + " ack", clientid);
            
            String attack_message = JSONController.enemyAttackPlayerJSON(message, player_life).toString();
            sendToClient(attack_message, opponent_id);
        }
    }
    
    
    
}
