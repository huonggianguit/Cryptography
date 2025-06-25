package com.sn.SrvFinance.server;

public class Manager {
    private static Manager instance;
    public static Manager gI(){
        if(instance == null){
            instance = new Manager();
        }
        return instance;
    }
    private Manager() {
        ServerManager.gI().run();
        // Constructor private để không cho new bên ngoài
    }
}
