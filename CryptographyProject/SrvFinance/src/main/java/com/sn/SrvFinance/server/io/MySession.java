package com.sn.SrvFinance.server.io;

import com.sn.SrvFinance.network.session.Session;
import com.sn.SrvFinance.user.User;

import java.nio.channels.Selector;
import java.nio.channels.SocketChannel;

public class MySession extends Session {
    public User user = null;
    public boolean isActive;
    public MySession(SocketChannel channel, Selector selector) {
        super(channel,selector);
    }
}
