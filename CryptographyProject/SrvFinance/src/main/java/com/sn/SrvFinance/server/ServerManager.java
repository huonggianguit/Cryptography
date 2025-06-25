package com.sn.SrvFinance.server;

import com.sn.SrvFinance.network.handler.IServerClose;
import com.sn.SrvFinance.network.server.ISessionAcceptHandler;
import com.sn.SrvFinance.network.server.TCPServer;
import com.sn.SrvFinance.network.session.ISession;
import com.sn.SrvFinance.server.io.MySession;
import org.mindrot.jbcrypt.BCrypt;

import java.util.HashMap;
import java.util.Map;

public class ServerManager {
    public static ServerManager instance;
    public boolean isRunning;
    public static int PORT = 12345;
    public static String keyClient ="DontDoThat";
    public static final Map CLIENTS = new HashMap();
    public void init(){
        Manager.gI();
    }
    public static ServerManager gI(){
        if (instance ==null){
            instance = new ServerManager();
            instance.init();
        }
        return instance;
    }
    public static void main(String[] args){
        ServerManager.gI();
//        String password = "helloworld";
//        String hashed = BCrypt.hashpw(password, BCrypt.gensalt(8));
//        System.out.println("✅ Hash chính xác của 'helloworld': " + hashed);

        // ✅ Hook giải phóng port khi JVM tắt (Stop, Ctrl+C, kill)
        Runtime.getRuntime().addShutdownHook(new Thread(() -> {
            try {

                TCPServer.gI().stop(); // hoặc close(), tùy hàm m cung cấp
            } catch (Exception e) {
                e.printStackTrace();
            }
        }));
    }
    public  void run(){
        isRunning=true;
        activeServerSocket();
    }
    private  void activeServerSocket(){
        try {
            TCPServer.gI().init().setAcceptHandler(new ISessionAcceptHandler() {
                @Override
                public void sessionInit(ISession s) {
                    s.setMessageReceiver(Controller.gI()).SetBufferSize(512);
                }
                @Override
                public void sessionDisconnect(ISession paramISession) {
                    ClientManager.gI().kickSession((MySession) paramISession);
                }
            }).setTypeSessionClone(MySession.class).setDoSomeThingWhenClose(new IServerClose() {
                @Override
                public void serverClose() {
                    System.out.println("server close");
                    System.exit(0);
                }
            }).start(PORT);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
    private boolean canConnectWithIp(String ipAddress) {
//        Object o = CLIENTS.get(ipAddress);
//        if (o == null) {
//            CLIENTS.put(ipAddress, 1);
//            return true;
//        } else {
//            int n = Integer.parseInt(String.valueOf(o));
//            if (n < Manager.MAX_PER_IP) {
//                n++;
//                CLIENTS.put(ipAddress, n);
//                return true;
//            } else {
//                return false;
//            }
//        }
        return true;
    }

    public void disconnect(MySession session) {
//        Object o = CLIENTS.get(session.getIP());
//        if (o != null) {
//            int n = Integer.parseInt(String.valueOf(o));
//            n--;
//            if (n < 0) {
//                n = 0;
//            }
//            CLIENTS.put(session.getIP(), n);
//        }
    }
}
