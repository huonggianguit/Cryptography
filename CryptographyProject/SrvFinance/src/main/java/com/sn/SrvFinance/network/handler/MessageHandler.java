package com.sn.SrvFinance.network.handler;

import com.sn.SrvFinance.network.io.Message;
import com.sn.SrvFinance.network.session.ISession;

public class MessageHandler implements IMessageReceiver {
    public void onMessage(ISession paramISession, Message paramMessage) throws Exception {
        paramMessage.close();
    }
}
