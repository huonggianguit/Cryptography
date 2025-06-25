package com.sn.SrvFinance.network.server;

import com.sn.SrvFinance.network.handler.IServerClose;
import com.sn.SrvFinance.network.io.ChannelInboundHandler;
import com.sn.SrvFinance.network.io.ChannelOutboundHandler;
import com.sn.SrvFinance.network.session.ISession;
import com.sn.SrvFinance.network.session.Session;
import com.sn.SrvFinance.network.session.SessionFactory;

import java.nio.channels.SocketChannel;
import java.util.Set;
import java.util.Iterator;

import java.io.IOException;
import java.net.InetSocketAddress;
import java.util.logging.Level;
import java.util.logging.Logger;
import java.nio.channels.Selector;
import java.nio.channels.ServerSocketChannel;
import java.nio.channels.SelectionKey;

public class TCPServer implements ITCPServer {
    private static TCPServer instance;
    private int port = -1;
    ServerSocketChannel gateServerListen;
    private Class<Session> sessionClone = Session.class;
    private boolean start;
    private boolean randomKey;
    private IServerClose serverClose;
    private ISessionAcceptHandler acceptHandler;
    private Thread AcceptThread;
    private Selector selector;
    public static TCPServer gI() {
        if (instance == null) {
            instance = new TCPServer();
        }
        return instance;
    }

    public ITCPServer init() {
        this.AcceptThread = new Thread(this);
        return this;
    }

    public ITCPServer start(int port) throws Exception {
        if (port < 0) {
            throw new Exception("Vui lòng khởi tạo port server!");
        }
        if (this.acceptHandler == null) {
            throw new Exception("AcceptHandler chưa được khởi tạo!");
        }
        if (!ISession.class.isAssignableFrom(this.sessionClone)) {
            throw new Exception("Type session clone không hợp lệ!");
        }
        try {
            this.port = port;
            this.gateServerListen = ServerSocketChannel.open();
            this.gateServerListen.bind(new InetSocketAddress(port));
            this.gateServerListen.configureBlocking(false); // bắt buộc cho NIO non-blocking
            selector = Selector.open();
            this.gateServerListen.register(selector, SelectionKey.OP_ACCEPT);
        } catch (IOException iOException) {
            System.out.println("Lỗi khởi tạo server tại port " + port);
            System.exit(0);
        }
        this.start = true;
        this.AcceptThread.start();
        System.out.println("Server chạy port: " + this.port);
        return this;
    }

    public ITCPServer close() {
        this.start = false;
        if (this.gateServerListen != null) {
            try {
                this.gateServerListen.close();
            } catch (IOException iOException) {
                iOException.printStackTrace();
            }
        }
        if (this.serverClose != null) {
            this.serverClose.serverClose();
        }
        System.out.println("Server đã đóng!");
        return this;
    }

    public ITCPServer dispose() {
        this.acceptHandler = null;
        this.AcceptThread = null;
        this.gateServerListen = null;
        return this;
    }

    public ITCPServer setAcceptHandler(ISessionAcceptHandler paramISessionAcceptHandler) {
        this.acceptHandler = paramISessionAcceptHandler;
        return this;
    }

    public void run() {
        try {
            while (this.start) {
              //  System.out.println("Selector Block");
                selector.select();
               // System.out.println("Selector Accept");
                /*
                - Khi sử dụng Io Sử dụng Socket:
                + JVM sẽ gọi syscall xuống OS. OS sẽ block tại đây và khi có Socket vào OS đánh thức theard và tiếp tục thực thi tiếp
                , sau đó quay lại vòng lặp tiếp tục gọi syscall -> lãng phí(syscall). Khó scale.
                + Java gọi epoll_create() → Kernel trả về 1 fd (gọi là epollfd): epoll (Linux), kqueue (BSD/macOS), IOCP (Windows). Logic như nhau
                + Về cơ bản với NIO JVM vẫn đẩy theard này xuống (Như IO) CPU scheduler(gắn flag TASK_UNINTERRUPTIBLE (theard Wait/Sleep chờ I/O cụ thể socket wake)
                + "Tưởng": Hiện tại OS nó chuyển IO sang 1 cái kho, nó có cái kho chứa khi Socket vào (trong kennel). nó có 1 loop riêng liên tục view kho (event hook + interrupt-driven)
                + JVM đăng ký 1 vs OS nó nhúng vào kennel liên tục view. Khi xuất hiện 1 socket hoặc socket có chứa raw thì nó đẩy lên (Callback)
                + TCP SYN tới -> Kernel thấy -> Đẩy connection đó vào accept backlog queue (hàng đợi của server socket), Đánh dấu server_fd là EPOLLIN,Kernel wake thread đang selector.select(),
                Java → quay về từ selector.select(), Tới đây, JVM báo key.isAcceptable() == true
                 */
                Set<SelectionKey> keys = selector.selectedKeys(); //selector  object đại diện cho "event multiplexer" của OS.
                Iterator<SelectionKey> iter = keys.iterator();

                while (iter.hasNext()) {
                    SelectionKey key = iter.next();
                    iter.remove();
                    if (!key.isValid()) continue;
                    if (key.isAcceptable()) {
                        /* khi nào isAcceptable true? Khi client gửi đầy đủ 3 gói TCP handshake (SYN → SYN-ACK → ACK)
                        + Kernel cho socket vào backlog queue
                        + Đánh dấu EPOLLIN trên server_fd (Gọi poll() callback của socket,Kiểm tra queue, Nếu pass → gắn EPOLLIN, đưa event vào ready list,Wake thread đang ngủ trong epoll_wait()
                        + selector.select() Vào tiến trình tại con trỏ đang ngừng.
                        + 3 bước này tự động. OS tự xử lý thông qua Sanbox JVM
                        + OS nó có 1 table quản lý các socket vào, bao gồm trang thái của socket.
                        key.isAcceptable() trả về true khi đủ 3 bước và đối vs trường hợp chưa accept. Đã accept là flase
                        */

                        ServerSocketChannel server = (ServerSocketChannel) key.channel();
                        SocketChannel client = server.accept();
                        /*
                        + JVM gọi xuống sys_accept() (native syscall)
                        +  Kernel lấy phần tử đầu hàng trong accept queue
                        + Tự gỡ nó khỏi queue,Trả client socket fd cho JVM,JVM wrap thành SocketChannel-> Trả về 1 file descriptor mới (fd khác với server_fd)
                        - >Socket này là 1 struct sock *sk hoàn toàn mới, tách biệt với server_fd -> Gắn vào file table của tiến trình Java ->Socket con trở thành một socket riêng biệt, không còn nằm trong queue


                        + "Tưởng": vào kho lấy 1 socket gói lại thành SocketChannel đẩy lên theard này xong clear (hút 1 socket ra khỏi backlog).  ,,
                        */
                        client.configureBlocking(false);
                        client.register(selector, SelectionKey.OP_READ);
                        /*
                        + Gọi read() khi chưa có data,Trả về ngay (0 hoặc WouldBlock) chứ không chờ
                        + Gọi write() khi buffer đầy,Trả về ngay, phải xử lý lại
                        + Gọi accept() khi chưa có client,Trả về null, không block

                        * Với IO Mỗi socket cần vài theard (2 hay tùy) để Loop đợi khi socket có raw là thực thi, đồng nghĩa nhiều theard
                        + NIO nó ko những có loop view kho liên tục mà nó còn view cả raw trong mỗi socket để đẩy lên.
                        + lệnh này chặn blocking tức là nó ko cần chờ raw tới để đẩy, mà nó đẩy liên tục, nếu ko có raw thì trả về 0.
                        + Vì sao cần chơi kiểu này, Vì giảm theard. Dùng 1 pool theard xong đẩy sang các Theard rãnh để liên tục xử lý read,write. Giảm theard khi Scale.
                        * Thay vì 1 socket 2 theard thì giờ 1 pool, 1 loop quản lý pool là read write hết.
                        */
                        ISession iSession = SessionFactory.gI().cloneSession(this.sessionClone, client,selector);
                        this.acceptHandler.sessionInit(iSession);
                        SessionManager.gI().putSession(client,iSession);
                       // System.out.println("Có socket vào ");
                    }
                    if (key.isReadable()) {
                        SocketChannel client = (SocketChannel) key.channel();
                        ISession session = SessionManager.gI().getSession(client);
                        if(!session.getIsHandlerRead()){
                            session.setIsHandlerRead(true);
                            ChannelInboundHandler.gI().handleRead(session);
                        }
                    }
                    if (key.isWritable()) {
                        SocketChannel client = (SocketChannel) key.channel();
                        ISession session = SessionManager.gI().getSession(client);
                        if(!session.getIsHandlerWrite()){
                            session.setIsHandlerWrite(true);
                            ChannelOutboundHandler.gI().handleWrite(session,key);
                        }
                    }
                }
            }
        } catch (IOException iOException) {
            iOException.printStackTrace();
        } catch (Exception exception) {
            Logger.getLogger(TCPServer.class.getName()).log(Level.SEVERE, null, exception);
        }
    }

    public ITCPServer setDoSomeThingWhenClose(IServerClose paramIServerClose) {
         this.serverClose = paramIServerClose;
        return this;
    }

    public ITCPServer randomKey(boolean paramBoolean) {
        this.randomKey = paramBoolean;
        return this;
    }


    public ITCPServer setTypeSessionClone(Class paramClass) throws Exception {
        this.sessionClone = paramClass;
        return this;
    }

    public ISessionAcceptHandler getAcceptHandler() throws Exception {
        if (this.acceptHandler == null) {
            throw new Exception("AcceptHandler chưa được khởi tạo!");
        }
        return this.acceptHandler;
    }

    public boolean isRandomKey() {
        return this.randomKey;
    }

    public void stopConnect() {
        this.start = false;
    }
    public void stop() {

        this.start = false;

        try {
            if (selector != null && selector.isOpen()) {
                selector.close();
            }
        } catch (IOException e) {
            e.printStackTrace();
        }

        try {
            if (gateServerListen != null && gateServerListen.isOpen()) {
                gateServerListen.close();
            }
        } catch (IOException e) {
            e.printStackTrace();
        }

        if (serverClose != null) {
            try {
                serverClose.serverClose();
            } catch (Exception e) {
                e.printStackTrace();
            }
        }

        System.out.println("🛑 TCPServer đã dừng và giải phóng tài nguyên.");
    }

}
