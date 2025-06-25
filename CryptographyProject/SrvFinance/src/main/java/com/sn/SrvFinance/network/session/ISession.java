package com.sn.SrvFinance.network.session;

import com.sn.SrvFinance.network.handler.IMessageReceiver;
import com.sn.SrvFinance.network.handler.IMessageIO;
import com.sn.SrvFinance.network.io.Message;
import com.sn.SrvFinance.network.io.ScBuf;

import java.util.Queue;

public interface ISession {
    enum SessionState {
        PreLogin,
        Login
    }
    TypeSession getTypeSession();

    ISession setMessageIO(IMessageIO paramIMessageIO);

    ISession setMessageReceiver(IMessageReceiver paramIMessageHandler);

    IMessageReceiver getMessageReceiver();

    ISession startSend();

    ISession startCollect();

    ISession start();

    ISession setReconnect(boolean paramBoolean);

    void writeToBuffer(Message paramMessage);

    ISession SetBufferSize(int paramInt);

    void setSessionState(SessionState param);

    SessionState getSessionState();

    ScBuf getBufferReader();

    ScBuf getBufferWriter();

    Queue<Message> getMessageQueue();

    void setIsHandlerRead(boolean paramBoolean);

    boolean getIsHandlerRead();

    void setIsHandlerWrite(boolean paramBoolean);
    boolean getIsHandlerWrite();

    void initThreadSession();

    void reconnect();

    String getIP();

    boolean isConnected();

    long getID();

    void addMessage(Message paramMessage);

    void doSendMessage(Message paramMessage) throws Exception;

    void disconnect();

    void dispose();

    int getNumMessages();

}
