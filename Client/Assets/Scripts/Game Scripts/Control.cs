using UnityEngine;
using System.Collections;


public class Control {

    private int player_id = 0;
    private int opponent_id = 1;
    private int player_turn = 0;

    private Hand hand;
    private Field field;
    private LifeControl lifeControl;
    private ManaControl mana;

    private EffectsControl effects_control;

    private BattleControl battleControl;

    private int player1_turns = 0;
    private int player2_turns = 0;

	public Control (int player_id) {
        this.setPlayersIds(player_id);
        this.field = new Field(this);
        this.battleControl = new BattleControl(this, field);
        this.lifeControl = new LifeControl(player_id, opponent_id);
        this.mana = new ManaControl(this);
        this.effects_control = new EffectsControl(this);

        this.hand = GameObject.FindGameObjectWithTag("Hand").GetComponent<Hand>();
    }

    /// <summary>
    /// Checa se o player possui mana para jogar a carta
    /// </summary>
    public bool checkMana (Card card) {
        return this.mana.checkIfItsPlayable(card);
    }

    public void createCard(int id, int cost, Position pos) {
        Card card = CardInstance.createCard(id);
        field.createCard(card, pos, opponent_id);
    }

    public void moveCreature(Position start_position, Position end_position) {
        field.moveCreature(start_position, end_position);
    }

    /// <summary>
    /// Cria uma instância de uma carta nova, e adiciona-a a mão.
    /// </summary>
    public void addCardToHand(int id) {
        Card card = CardInstance.createCard(id);
        card.setPlayerId(this.player_id);
        hand.addCard(card);
    }


    /// <summary>
    /// Ativa efeitos de reset da carta, e retorna-a para a mão.
    /// </summary>
    /// <param name="card"></param>
    public void returnCardToHand(Card card) {
        card.resetCard();
        hand.addCard(card);
    }

    /// <summary>
    /// Checa se é o turno do jogador.
    /// </summary>
    public bool isPlayerTurn() {
        return player_turn == player_id;
    }


    /// <summary>
    /// Muda o turno do jogador atual, e incrementa o número de turnos daquele jogador.
    /// </summary>
    private void changeTurn() {
        player_turn = (player_turn + 1) % 2;
        if (isPlayerTurn()) { 
            this.player2_turns++;
            if (this.player1_turns > 0)
                mana.gainMana();
        }
        else { 
            this.player1_turns++;
            if (this.player2_turns > 0)
                mana.gainMana();
        }

    }
    
    /// <summary>
    /// Atualiza o turno.
    /// Efeitos no inicio do turno são ativados
    /// </summary>
    public void newTurn() {
        changeTurn();
        if (this.isPlayerTurn())
            battleControl.creaturesCanAttack();

    }

    /// <summary>
    /// Retorna uma instância da carta a partir de seu id.
    /// </summary>
    public Card getCardById(int id) {
        Card card = CardInstance.createCard(id);
        card.setPlayerId(player_id);
        return card;
    }



    /// <summary>
    /// Ativa efeito da magia, de acordo com seu efeito ativo.
    /// Existem vários efeitos ativos diferentes. Alguns precisam de um alvo, por isso a necessidade do parametro position.
    /// </summary>
    public void playMagic(Card card, Position position = null) {

        if (card.onActivateEffect == null)
            return;

        ActiveEffects card_effect = card.onActivateEffect;

        int id = card_effect.id;
        switch (id) {
            case 1: {
                    effects_control.damageToTargetPosition(position, card_effect.dmg);
                    break;
                }
            case 2: {
                    effects_control.damageToTargetPlayerField(player_turn, card_effect.dmg);
                    break;
                }
            case 3: 
                effects_control.damageToField(card_effect.dmg);
                break;
            case 4:
                effects_control.summonGoblins(player_turn);
                break; 
        }
        this.field.check_board();
        mana.spendMana(card.getCost());
        card.resetCard();
        
    }


    public void reduceOpponentMana(int cost) {
        this.mana.reduceOpponentMana(cost);
    }

    public void spendMana(int manacost) {
        this.mana.spendMana(manacost);
    }

    public void endTurn() {
        if (this.isPlayerTurn()) {
            this.hand.endTurn();
        }
    }


    // 
    // Encapsulamento das variáveis
    //

    public ManaControl getMana() {
        return mana;
    }

    public EffectsControl getEffectsControl() {
        return effects_control;
    }

    public Hand getHand() {
        return hand;
    }

    public Field getField() {
        return field;
    }

    public LifeControl getLifeControl() {
        return lifeControl;
    }

    public int getPlayerTurn() {
        return player_turn;
    }

    public int getPlayerId() {
        return player_id;
    }

    public void setPlayersIds(int id) {
        player_id = id;
        opponent_id = (player_id + 1) % 2;
    }

    public int getOpponentId() {
        return opponent_id;
    }

    public BattleControl getBattleControl() {
        return battleControl;
    }

}


