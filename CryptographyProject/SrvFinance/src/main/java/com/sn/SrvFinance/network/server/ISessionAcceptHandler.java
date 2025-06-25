package com.sn.SrvFinance.network.server;

import com.sn.SrvFinance.network.session.ISession;

public interface ISessionAcceptHandler {
    void sessionInit(ISession paramISession);
    void sessionDisconnect(ISession paramISession);
}
