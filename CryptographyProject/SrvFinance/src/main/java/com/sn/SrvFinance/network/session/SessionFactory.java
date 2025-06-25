package com.sn.SrvFinance.network.session;

import java.nio.channels.Selector;
import java.nio.channels.SocketChannel;

public class SessionFactory {
    private static SessionFactory I;

    public static SessionFactory gI() {
        if (I == null) {
            I = new SessionFactory();
        }
        return I;
    }

    public <T extends ISession> T cloneSession(Class<T> paramClass, SocketChannel paramSocket, Selector selector) throws Exception {
        return paramClass
                .getConstructor(SocketChannel.class, Selector.class)
                .newInstance(paramSocket, selector);
    }
}
