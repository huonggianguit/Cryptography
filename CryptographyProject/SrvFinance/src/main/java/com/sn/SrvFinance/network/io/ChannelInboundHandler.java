package com.sn.SrvFinance.network.io;

import com.sn.SrvFinance.network.session.ISession;
import com.sn.SrvFinance.server.ClientManager;

import java.io.IOException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class ChannelInboundHandler {
    private static final ChannelInboundHandler instance = new ChannelInboundHandler();
    public static ChannelInboundHandler gI() {
        return instance;
    }

    private static final ExecutorService readThreadPool = Executors.newFixedThreadPool(8);
   // public void handleRead(ISession session) {
//        readThreadPool.execute(() -> {
//            try {
//                if(session!=null) {
//                    Message msg = session.getBufferReader().getMessageFormPayload();
//                    if (msg != null) {
//                        try {
//                            session.getMessageReceiver().onMessage(session, msg);
//                        } catch (Exception e) {
//                            e.printStackTrace();
//                        }
//                    }else{
//                        if(session!=null) {
//                            session.getBufferReader().readFromSocket();
//                            msg = session.getBufferReader().getMessageFormPayload();
//                            if (msg != null) {
//                                try {
//                                    session.getMessageReceiver().onMessage(session, msg);
//                                } catch (Exception e) {
//                                    ClientManager.gI().kickSession(session);
//                                }
//                            }
//
//                        }
//
//                    }
//                    session.setIsHandlerRead(false);
//                }
//
//            }  catch (Exception  e) {
//                ClientManager.gI().kickSession(session);
//        }
//        });
        public void handleRead(ISession session) {
            readThreadPool.execute(() -> {
                try {
                    if (session != null) {

                        // Rút hết message còn lại trong buffer
                        Message msg;
                        do {
                            msg = session.getBufferReader().getMessageFormPayload();
                            if (msg != null) {
                                try {
                                    session.getMessageReceiver().onMessage(session, msg);
                                } catch (Exception e) {
                                    e.printStackTrace();
                                    ClientManager.gI().kickSession(session);
                                    return;
                                }
                            }
                        } while (msg != null);

                        // Đọc thêm từ socket
                        session.getBufferReader().readFromSocket();

                        // Rút tiếp message mới sau khi đọc
                        do {
                            msg = session.getBufferReader().getMessageFormPayload();
                            if (msg != null) {
                                try {
                                    session.getMessageReceiver().onMessage(session, msg);
                                } catch (Exception e) {
                                    e.printStackTrace();
                                    ClientManager.gI().kickSession(session);
                                    return;
                                }
                            }
                        } while (msg != null);

                        session.setIsHandlerRead(false);
                    }
                } catch (Exception e) {
                    ClientManager.gI().kickSession(session);
                }
            });
        }

    //}

//    private void processMessage(SocketChannel client, Msg msg) {
//        // xử lý logic business ở đây
//    }
}
