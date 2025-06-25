package com.sn.SrvFinance.network.server;

import com.sn.SrvFinance.network.session.ISession;

import java.nio.channels.SocketChannel;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;

public class SessionManager {
    private static SessionManager i;
    private final Map<SocketChannel, ISession> sessionMap;

    public static SessionManager gI() {
        if (i == null) {
            i = new SessionManager();
        }
        return i;
    }

    public SessionManager() {
        this.sessionMap = new ConcurrentHashMap<>();
    }

    public void putSession(SocketChannel channel, ISession session) {
        this.sessionMap.put(channel, session);
    }

    public void removeSession(SocketChannel channel) {
        this.sessionMap.remove(channel);
    }

    public ISession getSession(SocketChannel channel) {
        return this.sessionMap.get(channel);
    }

    public ISession findByID(long id) throws Exception {
        for (ISession session : sessionMap.values()) {
            if (session.getID() == id) {
                return session;
            }
        }
        throw new Exception("Session " + id + " không tồn tại");
    }

    public int getNumSession() {
        return this.sessionMap.size();
    }

    public Iterable<ISession> getAllSessions() {
        return sessionMap.values();
    }
}
