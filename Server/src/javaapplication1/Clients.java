    /*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package javaapplication1;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.lang.String;
import java.util.Iterator;
import java.util.Random;
import java.util.logging.Level;
import java.util.logging.Logger;
import org.json.JSONException;

/**
 *
 * @author Vinicius
 */
public class Clients implements ReceiveListener {
    private List<Client> clients;
    
    private ArrayList<Client> startGameQueue;
    private int min_num_to_start_game = 2;
    
    public Clients(){
        this.clients = new ArrayList<>();
        clients = new ArrayList<>();
        this.startGameQueue  = new ArrayList<>();
    }
    
    public void addClient(Client client){
        clients.add(client);
    }
    
    
    /**
     * 
     * @param client 
     */
    void ClientStartGame(Client client){
        startGameQueue.add(client);
        System.out.println("Adicionou cliente a queue");
        tryToStartGame();
    }
    
    
    @Override
    public void dataReceive(Client client, String data){
        if (data.equals("start game")){
            ClientStartGame(client);
        }
    }
    
    public void sendBroadcast(Client client, String data){
        for (Client item : clients){
            if (item != client)
                item.sendToClient(data);
        }
    }
    
    private void startGame(Client client, Client client2){
        Game game = new Game();
        game.setClients(client, client2);
        client.setGame(game);
        client2.setGame(game);
    }
    
    private void tryToStartGame(){
    
        if (startGameQueue.size() >= min_num_to_start_game){
            Client client = startGameQueue.remove(0);
            
            Client client2 = startGameQueue.remove(0);

            Random rand = new Random();
            int randvalue = rand.nextInt(2);
            if (randvalue == 0){
                client.sendStartGameAck(0);   
                client2.sendStartGameAck(1);
                startGame(client, client2);
            }
            else {
                client.sendStartGameAck(1);   
                client2.sendStartGameAck(0);
                startGame(client2, client);
            }
            System.out.println("Game started");
        }

    }
}
