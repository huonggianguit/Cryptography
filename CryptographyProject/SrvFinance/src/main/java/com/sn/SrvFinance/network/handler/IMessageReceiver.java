package com.sn.SrvFinance.network.handler;

import com.sn.SrvFinance.network.io.Message;
import com.sn.SrvFinance.network.session.ISession;

public interface IMessageReceiver {
    void onMessage(ISession paramISession, Message paramMessage) throws Exception;
}
