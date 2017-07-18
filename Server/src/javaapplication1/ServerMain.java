
package javaapplication1;

import java.io.IOException;
import java.net.Inet4Address;
import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.Socket;
import org.json.JSONException;

public class ServerMain {
    
    public static void main(String[] args) throws JSONException{
        try {
            Clients clients = new Clients();
            ServerSocket serverSocket = new ServerSocket(16000);
            while (true){
                Socket socket = serverSocket.accept();
                Client client = new Client(socket, clients);
                clients.addClient(client);
            }
        }
        catch (IOException e){
            e.printStackTrace();
        }
    }
}
