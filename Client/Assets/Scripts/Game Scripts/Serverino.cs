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
    bool waiting_ack = false;

    private const int max_connection_attempts = 5;

    public int getState() {
        return state;
    }

    public void setState(int state) {
        this.state = state;
    }


    private void TryConnection(int attempts) {
        client = new TcpClient();

        var result = client.BeginConnect("127.0.0.1", 16000, null, null);
        var sucess = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
        
        if (!sucess) {
            if (attempts < max_connection_attempts) { 
                print("Failed to connect");
                TryConnection(attempts + 1);
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
        TryConnection(0);
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
        control = new Control(this, temp_player_id);
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
        control.spendMana(card.getCost());
    }

    //play a 1 size character on the line/position on the battlefield
    public bool TryPlayCharacter(Card card, Position pos, int hand_index) {
        int id = control.getPlayerId();
        JSONObject message = JSONWriter.PlayCharacterJSON(card, id, hand_index, pos);
        send(message.ToString());
        return readAck("PlayCharacter" + control.getPlayerId() + " ack");
    }

    //joga magia sem alvo - Exemplo: Nevasca (cause 2 de dano a todos personagens)
    public void TryPlayMagicWithoutTarget(int magic_id) {
        JSONObject json = JSONWriter.TryPlayMagic(magic_id);
        send(json.ToString());
    }

    // joga magia com alvo - Exemplo: Bola de Fogo (cause 5 de dano a um personagem)
    public void TryPlayMagicWithTarget(int magic_id, Position pos, int side) {
        JSONObject json = JSONWriter.TryPlayMagic(magic_id);
        json.AddField("position", JSONWriter.SetTarget(pos, side));
        send(json.ToString());
    }

    public bool tryAttackCharacter(int attacker_id, int target_id, Position attacker_pos, Position target_pos) {
        JSONObject message = JSONWriter.AttackCharacterJSON(attacker_id, target_id, attacker_pos, target_pos);
        send(message.ToString());
        return readAck("Attack" + control.getPlayerId() + " ack");
    }

    public bool tryAttackPlayer(int attacker_id, Position attacker_pos) {
        JSONObject message = JSONWriter.AttackPlayerJSON(attacker_id, attacker_pos);
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

            }
        }
        return "";
    }





    void parseData(string data) {
        
        JSONObject json = new JSONObject(data);
        if (json.IsNull) {
            switch (data) {

                case "Conexao estabelecida - by server":  {
                        Debug.Log("Recebi mensagem que estou em conectado");
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
                            SetHand((JSONObject)json.list[0]);
                        }
                        break;
                    }
                case "playcreature" : {
                        EnemyPlayCharacter((JSONObject)json.list[0]);
                        break;
                    }
                case "attack": {
                        enemyAttack((JSONObject)json.list[0]);
                        break;
                    }
            }
        }
    }

    public void EndTurn() {
        send("end turn");
    }

    private void enemyAttack (JSONObject json) {

        int target_id = (int)json.GetField("target_id").n;
        JSONObject position = json.GetField("attacker_position");
        Position attacker_position = JSONReader.getPositionInstance(position, false);
        attacker_position.side = control.getOpponentId();

        Card attacker_card = control.getField().getCard(attacker_position);
        Debug.Log("id atacante " + attacker_card.getID());
        BattleControl battleControl = control.getBattleControl();

        //atacou direto
        if (target_id == -1) {
            Debug.Log("oponente me atacou diretamente");
            battleControl.directAttack(attacker_card, control.getPlayerId());
        }
        else {
            JSONObject target_positionJSON = json.GetField("target_position");
            Position target_position = JSONReader.getPositionInstance(target_positionJSON, false);
            target_position.side = control.getPlayerId();

            Card target_card = control.getField().getCard(target_position);
            Debug.Log("id target " + target_card.getID());
            battleControl.cardAttackCard(attacker_card, target_card);  
        }

    }



    void EnemyPlayCharacter(JSONObject json) {
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

    private void opponentPlayCreature(int mana, int id, int cost, int dmg, int life, Position pos) {
        control.setOpponentMana(mana);
        control.createCard(id, cost, dmg, life, pos);
    }

    void SetHand(JSONObject obj) {
        for (int i = 0; i < obj.list.Count; i++) {
            JSONObject card = (JSONObject)obj.list[i];
            
            int id = (int)card.GetField("id").n;
            
            string type = card.GetField("type").str;
            int cost = (int)card.GetField("cost").n;
            int dmg = -1;
            int life = -1;

            //testa se é um personagem ou herói (contém dmg e life)
            if (type.Equals("Personagem") || type.Equals("Herói")) {
                JSONObject dmgJSON = card.GetField("dmg");
                if (dmgJSON) 
                    dmg = (int)dmgJSON.n;
                
                JSONObject lifeJSON = card.GetField("life");
                if (lifeJSON)
                    life = (int)lifeJSON.n;
            }
            control.addCardToHand(id, type, cost, dmg, life);
            
            
        }
    }


}
