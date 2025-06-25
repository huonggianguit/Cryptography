package com.sn.SrvFinance.network.io;

import com.sn.SrvFinance.network.session.ISession;

import java.io.IOException;
import java.nio.ByteBuffer;
import java.nio.channels.SelectionKey;
import java.nio.channels.Selector;
import java.nio.channels.SocketChannel;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class ChannelOutboundHandler {
    private static final ChannelOutboundHandler instance = new ChannelOutboundHandler();

    public static ChannelOutboundHandler gI() {
        return instance;
    }

    private static final ExecutorService writeThreadPool = Executors.newFixedThreadPool(4);

    public void handleWrite(ISession session, SelectionKey key) {
        writeThreadPool.execute(() -> {
            try {
                if (session.getBufferWriter().hasPendingWrite()) {
                    session.getBufferWriter().writeToSocket();
                } else {
                    Message msg = session.getMessageQueue().peek();
                    if(msg != null) {
                        session.writeToBuffer(msg);
                    }
                }
                if(session.getMessageQueue().isEmpty() && !session.getBufferWriter().hasRemaining()){
                    cancelWrite(key);
                }
                session.setIsHandlerWrite(false);
            } catch (IOException e) {
                e.printStackTrace();
//                try {
//                    //client.close();
//                } catch (IOException ignored) {
//                }
            }
        });
    }
    private void cancelWrite(SelectionKey key) {
        if (key != null && key.isValid()) {
            key.interestOps(key.interestOps() & ~SelectionKey.OP_WRITE);
        }
    }
}

