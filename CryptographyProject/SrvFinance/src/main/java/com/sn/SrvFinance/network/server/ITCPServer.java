package com.sn.SrvFinance.network.server;

import com.sn.SrvFinance.network.handler.IServerClose;

public interface ITCPServer extends Runnable {
    ITCPServer init();

    ITCPServer start(int paramInt) throws Exception;

    ITCPServer setAcceptHandler(ISessionAcceptHandler paramISessionAcceptHandler);

    ITCPServer close();

    ITCPServer dispose();

    ITCPServer randomKey(boolean paramBoolean);

    ITCPServer setDoSomeThingWhenClose(IServerClose paramIServerClose);

    ITCPServer setTypeSessionClone(Class paramClass) throws Exception;

    ISessionAcceptHandler getAcceptHandler() throws Exception;

    boolean isRandomKey();

    void stopConnect();
}
