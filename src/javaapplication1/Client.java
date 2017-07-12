
package javaapplication1;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import java.util.UUID;
import java.util.logging.Level;
import java.util.logging.Logger;
import org.json.JSONException;

/**
 * 
 * @author Pedro
 */
public class Client {

    private final Socket client;
    private final ReceiveListener listener;
    private final InputStream inputStream;
    private final OutputStream outputStream;
    private final String id;
    
    private Game game;
    public int gameid;
    
    /**
     * 
     * @param client
     * @param listener
     * @throws IOException
     * @throws JSONException 
     */
    public Client(Socket client, ReceiveListener listener) throws IOException, JSONException {
        this.id = UUID.randomUUID().toString();
        this.client = client;
        this.listener = listener;
        this.inputStream = client.getInputStream();
        this.outputStream = client.getOutputStream();
        this.game = null;
        new ReadThread().start();
        sendStart();
    }
    
    /**
     * 
     * @return 
     */
    public int getId(){
        return Integer.parseInt(id);
    }
    
    public void setGame(Game game){
        this.game = game;
    }
    
    public void sendDeckToClient(Deck deck) throws JSONException{
         sendToClient(deck.deckJSON().toString());
    }
    
    public void sendStartGameAck(int clientid){
        String id = Integer.toString(clientid);
        gameid = clientid;
        sendToClient(id + " ack start game");
    }
    
    
    //Chamada quando conexão é estabelecida
    private void sendStart() throws JSONException{
        sendToClient("Conexao estabelecida - by server");
    }
    
    public void sendToClient(String message){
        System.out.println("Message sent: " + message);
        try{
            byte[] bytes = message.getBytes();
            byte[] bytesSize = intToByteArray(message.length());
            outputStream.write(bytesSize, 0, 4);
            outputStream.write(bytes, 0, bytes.length);
        }
        catch (IOException e){
            e.printStackTrace();
        }
    }


    private class ReadThread extends Thread {

        @Override
        public void run(){
            super.run();
            byte[] bytes = new byte[1024];
            
            while (!client.isClosed()){
                try {
                    //recebe mensagem do cliente
                    int data = inputStream.read(bytes);
                    if (data > 1){
                        String string = new String(bytes, 0, data);
                        if (game == null){
                            listener.dataReceive(Client.this, string);
                        }
                        else{
                            game.clientSentMessage(string, gameid);
                        }
                    }
                }
                catch (IOException e){
                    e.printStackTrace();
                    break;
                    
                } catch (JSONException ex) {
                    Logger.getLogger(Client.class.getName()).log(Level.SEVERE, null, ex);
                }
            }
            System.out.println("Close");
        }
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
 