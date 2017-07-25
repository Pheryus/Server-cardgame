using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;

public class Serverino : MonoBehaviour {

    private NetworkStream stream;
    private StreamWriter writer;

    private TcpClient client;
    public Control control;
    private int state = 0;

    public int STATE_STARTGAME = 1;
    public int STATE_WAITINGGAME = 2;
    int STATE_LOADINGSCENE = 3;
    int STATE_SENDMEDECK = 4;
    int STATE_WAITINGDECK = 5;
    public int STATE_INGAME = 6;
    public GameObject prefab;
    bool isConnected = false;
    private int temp_player_id;

    private const int max_connection_attempts = 5;

    public int getState() {
        return state;
    }

    public void setState(int state) {
        this.state = state;
    }


    private void tryConnection(int attempts) {
        client = new TcpClient();

        var result = client.BeginConnect("192.168.1.8", 16000, null, null);
        var sucess = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
        
        if (!sucess) {
            if (attempts < max_connection_attempts) { 
                print("Failed to connect");
                tryConnection(attempts + 1);
            }
            else {
                Application.Quit();
            }
        }
        isConnected = true;
        stream = client.GetStream();
        stream.ReadTimeout = 10;
    }


	void Start () {
        DontDestroyOnLoad(this.gameObject);
        tryConnection(0);
        createStream();
 
	}

    private void createStream() {
        if (stream.CanRead) {
            writer = new StreamWriter(stream);
            string message = readData();
            stream.Flush();
            parseData(message);
        }
        else
            Destroy(gameObject);
    }


    public void send(string message) {
        writer.Write(message);
        writer.Flush();
    }


    private void Update() {

        if (Input.GetKey(KeyCode.P)) {
            Debug.Break();
        }

        if (isConnected) {
            readInput();
            parseData(readData());
        }
        if (state == STATE_LOADINGSCENE && SceneManager.GetActiveScene().name == "Game") {
            gameSceneLoaded();
        }
    }

    private void gameSceneLoaded() {
        //quer receber cartas da mao
        state = STATE_SENDMEDECK;
        control = new Control(temp_player_id);
        temp_player_id = -1;
    }

    void readInput() {
        if (state == STATE_STARTGAME) {
            send("start game");
            state = STATE_WAITINGGAME;
        }
        else if (state == STATE_SENDMEDECK) {
            send("send deck");
            state = STATE_WAITINGDECK;
        }
    }
        
    public void playerPlayCreature(Card card, Position position) {
        card.onPlayEnter();
        control.spendMana(card.getCost());
    }

    //play a 1 size character on the line/position on the battlefield
    public bool tryPlayCharacter(Card card, Position pos, int hand_index) {
        int id = control.getPlayerId();
        JSONObject message = JSONWriter.playCharacterJSON(card, id, hand_index, pos);
        send(message.ToString());
        return readAck("PlayCharacter" + control.getPlayerId() + " ack");
    }

    /// <summary>
    /// joga magia sem alvo - Exemplo: Nevasca (cause 2 de dano a todos personagens)
    /// </summary>
    /// <param name="magic_id"></param>
    /// <returns></returns>
    public bool tryPlayMagicWithoutTarget(int magic_id) {
        JSONObject json = JSONWriter.tryPlayMagic(magic_id);
        send(json.ToString());
        return readAck("Play spell" + control.getPlayerId() + " ack");
    }

    /// <summary>
    /// joga magia com alvo - Exemplo: Bola de Fogo (cause 5 de dano a um personagem)
    /// </summary>
    /// <param name="magic_id"></param>
    /// <param name="pos"></param>
    /// <param name="side"></param>
    /// <returns> True se a carta foi aceita no server.</returns>
    public bool tryPlayMagicWithTarget(int magic_id, Position pos) {
        JSONObject json = JSONWriter.tryPlayMagic(magic_id);
        
        JSONObject spell = json.GetField("playspell");
        spell.AddField("position", JSONWriter.setTarget(pos));

        send(json.ToString());

        return readAck("Play spell" + control.getPlayerId() + " ack");
    }

    public bool tryAttackCharacter(int attacker_id, int target_id, Position attacker_pos, Position target_pos) {
        JSONObject message = JSONWriter.AttackCharacterJSON(attacker_id, target_id, attacker_pos, target_pos);
        send(message.ToString());
        return readAck("Attack" + control.getPlayerId() + " ack");
    }

    public bool tryAttackPlayer(int attacker_id, Position attacker_pos) {
        JSONObject message = JSONWriter.attackPlayerJSON(attacker_id, attacker_pos);
        send(message.ToString());
        return readAck("Attack" + control.getPlayerId() + " ack");
    }



    bool readAck(string ackExpected) {
        if (stream.CanRead) {
            string message = readData();
            stream.Flush();
            if (message == ackExpected)
                return true;
            else
                return false;
            
        }
        else
            return false;


    }


    string readData() {

        if (stream.CanRead) {
            try {
                byte[] blen = new Byte[4];
                int data = stream.Read(blen, 0, 4);

                if (data > 0) {
                    int len = BitConverter.ToInt32(blen, 0);
                    
                    Byte[] buff = new byte[1024];
                    data = stream.Read(buff, 0, len);
                    if (data > 0) {
                        string result = Encoding.ASCII.GetString(buff, 0, data);
                        return result;
                    }
                }
            }
            catch (Exception ex) {
                //Debug.Log("Erro: " + ex);
            }
        }
        return "";
    }





    void parseData(string data) {
        JSONObject json = new JSONObject(data);
        if (json.IsNull) {
            switch (data) {

                case "Conexao estabelecida - by server":  {
                        break;
                    }

                case "0 ack start game": {
                        if (state == STATE_WAITINGGAME) {
                            temp_player_id = 0;
                            state = STATE_LOADINGSCENE;
                            SceneManager.LoadScene("Game");
                        }
                        break;
                    }
                case "1 ack start game": {
                        if (state == STATE_WAITINGGAME) {
                            temp_player_id = 1;
                            state = STATE_LOADINGSCENE;
                            SceneManager.LoadScene("Game");
                        }
                        break;
                    }
                case "end turn": {
                        control.endTurn();

                        control.newTurn();
                        break;
                    }
                
            }
        }


        else if (!json.IsNull) {
            switch ((string)json.keys[0]) {

                case "hand": {
                        if (state == STATE_WAITINGDECK) {
                            state = STATE_INGAME;
                            this.setHand((JSONObject)json.list[0]);
                        }
                        break;
                    }
                case "playcreature" : {
                        this.enemyPlayCharacter((JSONObject)json.list[0]);
                        break;
                    }
                case "attack": {
                        this.enemyAttack((JSONObject)json.list[0]);
                        break;
                    }
                case "playspell": {
                        this.enemyPlayMagic((JSONObject)json.list[0]);
                        break;
                    }
                case "movecreature": {
                        this.enemyMoveCharacter((JSONObject)json.list[0]);
                        break;
                    }
            }
        }
    }

    public void endTurn() {
        send("end turn");
    }


    /// <summary>
    /// Trata o json recebido quando o oponente joga uma magia.
    /// </summary>
    /// <param name="json"></param>
    private void enemyPlayMagic(JSONObject json) {
        int id = (int)json.GetField("id").n;
        Card card = control.getCardById(id);
        card.setPlayerId(control.getOpponentId());
        JSONObject positionJSON = json.GetField("position");
        Position position = null;

        if (positionJSON != null) {
             position = JSONReader.getPositionInstance(positionJSON);
        }
        this.control.playMagic(card, position);
    }

    private void enemyAttack (JSONObject json) {

        int target_id = (int)json.GetField("target_id").n;
        JSONObject position = json.GetField("attacker_position");
        Position attacker_position = JSONReader.getPositionInstance(position);

        Card attacker_card = control.getField().getCardByPosition(attacker_position);
        BattleControl battleControl = control.getBattleControl();

        //atacou direto
        if (target_id == -1) {
            battleControl.directAttack(attacker_card, control.getPlayerId());
        }
        else {
            JSONObject target_positionJSON = json.GetField("target_position");
            Position target_position = JSONReader.getPositionInstance(target_positionJSON);

            Card target_card = control.getField().getCardByPosition(target_position);
            battleControl.cardAttackCard(attacker_card, target_card);  
        }

    }

    private void enemyMoveCharacter(JSONObject json) {
        JSONObject start_position = json.GetField("start_position");
        Position start_pos = JSONReader.getPositionInstance(start_position);

        JSONObject end_position = json.GetField("end_position");
        Position end_pos = JSONReader.getPositionInstance(end_position);

        control.moveCreature(start_pos, end_pos);


    }

    /// <summary>
    /// Função ainda em alfa.
    /// </summary>
    /// <param name="json"></param>
    private void enemyPlayCharacter(JSONObject json) {
        int id = (int)json.GetField("id").n;

        JSONObject position = json.GetField("position");
        Position pos = JSONReader.getPositionInstance(position);

        int cost = (int)json.GetField("cost").n;
        this.opponentPlayCreature(id, cost, pos);

    }



    /*
    void enemyPlayCharacter(JSONObject json) {
        int id = (int)json.GetField("id").n;
        int player_id = (int)json.GetField("player_id").n;
        int hand_id = (int)json.GetField("hand_id").n;

        JSONObject position = json.GetField("position");
        Position pos = JSONReader.getPositionInstance(position);

        int cost = (int)json.GetField("cost").n;
        int dmg = (int)json.GetField("dmg").n;
        int life = (int)json.GetField("life").n;

        int mana = (int)json.GetField("enemymana").n;
        opponentPlayCreature(mana, id, cost, dmg, life, pos);
    }
    */

    private void opponentPlayCreature(int id, int cost, Position pos) {
        control.reduceOpponentMana(cost);
        control.createCard(id, cost, pos);
    }

    public bool tryMoveCharacter(Card card, Position position) {
        JSONObject message = JSONWriter.moveCharacterJSON(card, position);
        send(message.ToString());
        return readAck("Move" + control.getPlayerId() + " ack");
    }


    void setHand(JSONObject obj) {

        int[] hand = { 0, 1, 2, 3, 10, 12, 8, 9, 13, 14 };

        foreach (int card in hand)
            control.addCardToHand(card);

        /*
        for (int i = 0; i < obj.list.Count; i++) {
            JSONObject card = (JSONObject)obj.list[i];
            
            int id = (int)card.GetField("id").n;
            
            string type = card.GetField("type").str;
            int cost = (int)card.GetField("cost").n;
            int dmg = -1;
            int life = -1;

            if (type.Equals("Personagem")) {
                JSONObject dmgJSON = card.GetField("dmg");
                if (dmgJSON) 
                    dmg = (int)dmgJSON.n;
                
                JSONObject lifeJSON = card.GetField("life");
                if (lifeJSON)
                    life = (int)lifeJSON.n;
            }
            control.addCardToHand(id);
            
            
        }
        */
    }


}
