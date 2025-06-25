package com.sn.SrvFinance.network.session;

import com.sn.SrvFinance.network.handler.IMessageReceiver;
import com.sn.SrvFinance.network.handler.IMessageIO;
import com.sn.SrvFinance.network.io.Message;
import com.sn.SrvFinance.network.io.ScBuf;

import java.io.IOException;
import java.nio.ByteBuffer;
import java.nio.channels.SelectionKey;
import java.nio.channels.Selector;
import java.nio.channels.SocketChannel;
import java.util.LinkedList;
import java.util.Queue;
import java.util.concurrent.ConcurrentLinkedQueue;

public class Session implements ISession {
    public final SocketChannel channel;
    private final Selector selector;
    public final Queue<ByteBuffer> writeQueue = new LinkedList<>();

    private SessionState sessionState = SessionState.PreLogin;

    private final Queue<Message> messageQueue = new ConcurrentLinkedQueue<>();

    /* Concurrent có cơ chế  lock-free, dùng CAS (Compare-And-Swap) để đảm bảo thread safety mà không cần khóa (lock), cơ chế : head: trỏ tới node đầu (để đọc: poll()),tail: trỏ tới node cuối (để ghi: offer())
    *Không khóa (synchronized)
    * boolean CAS(address, expectedValue, newValue),Nếu *address == expectedValue → thì gán newValue,Ngược lại → KHÔNG làm gì, return false → retry


    +Dùng CAS (Compare-And-Swap) – 1 loại thao tác nguyên tử ở CPU

    +Cho phép nhiều thread cùng ghi/đọc mà không chặn nhau

    + Tưởng: Nó như cái băng chuyền, đọc là lấy đầu, ghi là nó đẩy vào cuối (khác synchronized (Block)
    * Khác chổ này mọi theard sẽ dc OS nó quản lý trong 1 vùng nào đấy. Khi nó block thì cần chờ loop back lại để mở block, và đây sẽ là chi phí swich, tốn cost chổ này)
    *  Sun.misc.Unsafe, Ra từ: Java 1.4 (2002).  Dùng nội bộ cho JVM/HotSpot → sau đó cộng đồng hack ra dùng luôn,Không public API → phải "bẻ khóa" (getDeclaredField("theUnsafe")…)
    * AtomicXXX, AtomicInteger, AtomicReference,JVM JIT inline CAS Ra từ: Java 1.5 (2004),Là API chính thức đầu tiên cho lập trình lock-free,Thuộc java.util.concurrent.atomic, Dùng Unsafe bên dưới để làm CAS
    * VarHandle,  Ra từ: Java 9 (2017), Public API, thay thế Unsafe chính thức, Có hỗ trợ compareAndSet, getAndSet, weakCompareAndSet, atomic full,Hướng đến modularity + clean code + JIT-friendly
    * API của java mới cho nó connect xuống sanbox real luôn ( điều này giúp nó đỡ chuyển chi phí qua các sandbox, Unsafe và VarHandle — "lối tắt" từ JVM xuống CPU)
    * Trong trường hợp này hình như thằng JVM nó qua mặt cả OS, nó gửi xuống CPU sửa cấp nguyên tử, và CPU sẽ báo lại cho nó sửa dc hay ko, Và nếu theard thua(ko sửa dc ) thì jvm retry lại
    (Không tốn chi phí swiching)
    * Khi 2 theard cùng ghi (bất kể 1 theard thắng hay cùng lúc, Cpu ép 1 thằng thắng), nó sẽ đọc con trõ cuối. Check next xem có null ko? nếu null thì nó cho ghi, thằng thắng dc ghi, thằng thua retry(while)
    *
     * JVM trong trường hợp dùng trò này nó chỉ warp native thôi, bản chất vẫn là sandbox địa chỉ nó gửi là va(virtual address), cpu tự call mmu của OS để lấy address real
    Quy trình: OS nó tùy theo ram nó setup ram rồi có 1 bảng Page Table(bảng ánh xạ ram real), Sandbox như py,c#,jv nó đều gửi va xuống cpu (mmu, 1 vùng vật lý setup check địa chỉ, người đọc bản đồ)
     xong cpu nó sẽ tra catch (nó có catch lại địa chỉ dùng gần đây) hoặc nó tra trên PA để tìm dc địa chỉ trong ram) nếu lỗi gọi OS xử lý (page fault)
     +Trong Page Table Entry (PTE) chứa permission flags (Readable,Writable,Executable,User/Supervisor), MMU tra PTE ,Nếu permission không hợp lệ, MMU gây trap về OS:

     */
    @Override
    public Queue<Message> getMessageQueue() {
        return messageQueue;
    }

    private IMessageReceiver messageReceiver;

    private ScBuf bufferReader;
    private ScBuf bufferWriter;
    private int sizeBuffer;
    /* Khác với IO lúc trước mỗi Session phải có 1 tới 2 Theard để xử lý khi nhận raw từ Socket và khi đẩy raw xuống Socket
    + Giờ dùng 1 kênh tổng gI() để handler cho mọi Seesion khi nhận và ghi.
    + Đoạn này gán 2 thành phần này dạng Interface để có thể tùy chỉnh Class xử lý trong trường hợp toàn bộ Network dc gói thành lib
    */
    public ScBuf getBufferReader() {
        return bufferReader;
    }

    public ScBuf getBufferWriter() {
        return bufferWriter;
    }

    public ISession SetBufferSize(int size) {
        sizeBuffer = size;
        return this;
    }

    public TypeSession typeSession;
    private boolean isHandlerRead;
    private boolean isHandlerWrite;

    public Session(SocketChannel channel, Selector selector) {
        this.channel = channel;
        this.selector = selector;
        this.bufferReader= new ScBuf(channel,this);
        this.bufferWriter= new ScBuf(channel,this);
    }

    public TypeSession getTypeSession() {
        return this.typeSession;
    }

    public void initThreadSession() {

    }

    public ISession setMessageIO(IMessageIO messageIO) {
        return this;
    }

    public ISession setMessageReceiver(IMessageReceiver param) {
        messageReceiver = param;
        return this;
    }

    public void setSessionState(SessionState param) {
        sessionState = param;
    }
    public SessionState getSessionState(){
        return sessionState;
    }

    public IMessageReceiver getMessageReceiver() {
        return messageReceiver;
    }

    public ISession setMessageHandler() {
        return this;
    }

    public ISession startSend() {
        return this;
    }

    public ISession startCollect() {
        return this;
    }

    public ISession start() {
        return this;
    }

    public ISession setReconnect(boolean flag) {
        return this;
    }

    public void reconnect() {

    }

    public boolean getIsHandlerRead() {
        return isHandlerRead;
    }

    public void setIsHandlerRead(boolean flag) {
        isHandlerRead = flag;
    }

    public boolean getIsHandlerWrite() {
        return isHandlerWrite;
    }

    public void setIsHandlerWrite(boolean flag) {
        isHandlerWrite = flag;
    }

    public String getIP() {
        return "";
    }

    public boolean isConnected() {
        return false;
    }

    public long getID() {
        return 0;
    }

    public void addMessage(Message msg) {
        msg.prepareToRead();
        messageQueue.add(msg);
        SelectionKey key = channel.keyFor(selector); // 👈 lấy SelectionKey đang liên kết socket
        if (key != null && key.isValid()) {
            int interestOps = key.interestOps();
            if ((interestOps & SelectionKey.OP_WRITE) == 0) {
                // 👇 Thêm OP_WRITE nếu chưa có
                key.interestOps(interestOps | SelectionKey.OP_WRITE);
                selector.wakeup(); // 👈 đánh thức selector nếu đang block
            }
        }
    }

    public void writeToBuffer(Message msg) {
        try {
            int maxToRead = bufferWriter.remaining();
         //   System.out.println("[writeToBuffer] bufferWriter.remaining(): " + maxToRead);

            byte[] temp = msg.readBytes(maxToRead);
          //  System.out.println("[writeToBuffer] Đọc " + temp.length + " byte từ message");

            if (temp.length > 0) {
                int written = bufferWriter.writeBytes(temp);
             //   System.out.println("[writeToBuffer] Đã ghi " + written + " byte vào bufferWriter");
            }

            int left = msg.availableRead();
          //  System.out.println("[writeToBuffer] Dữ liệu còn lại trong message: " + left + " byte");

            if (left == 0) {
                messageQueue.poll();
                msg.close();
            //    System.out.println("[writeToBuffer] Đã đọc xong message, đóng và loại khỏi hàng đợi");
            }
        } catch (Exception e) {
            System.err.println("[writeToBuffer] Lỗi: " + e.getMessage());
            e.printStackTrace();
        }
    }

    public void doSendMessage(Message paramMessage) {
    }

    public void disconnect() {
        try {
            SelectionKey key = channel.keyFor(selector);
            if (key != null) {
                key.cancel(); // Hủy lắng nghe selector cho session này
            }
            if (channel.isOpen()) {
                channel.close(); // 👈 huỷ socket kernel → client bị disconnect thật
            }


            dispose(); // Dọn dẹp bộ nhớ

        } catch (IOException e) {
            e.printStackTrace(); // Hoặc log lỗi
        }
    }

    public void dispose() {
        messageQueue.clear();       // Xóa hàng đợi tin nhắn
        writeQueue.clear();         // Xóa hàng đợi ghi
        bufferReader = null;        // Dọn buffer
        bufferWriter = null;
        messageReceiver = null;     // Xóa handler
        typeSession = null;
        sessionState = null;

        // Cờ trạng thái
        isHandlerRead = false;
        isHandlerWrite = false;
    }

    public int getNumMessages() {
        return 0;
    }

    // Gửi message: đẩy vào queue và gọi flush
    public void send(ByteBuffer buffer, SelectionKey key) throws IOException {
        writeQueue.add(buffer);
        tryFlush(key);
    }

    // Flush queue → ghi tuần tự, không được trộn
    public void tryFlush(SelectionKey key) throws IOException {
        while (!writeQueue.isEmpty()) {
            ByteBuffer buf = writeQueue.peek();
            int written = channel.write(buf);

            if (buf.hasRemaining()) {
                // Buffer ghi chưa hết, chờ OP_WRITE lần sau
                key.interestOps(key.interestOps() | SelectionKey.OP_WRITE);
                return;
            }

            writeQueue.poll(); // Ghi xong thì xóa khỏi queue
        }

        // Ghi hết rồi → bỏ OP_WRITE để không spam select()
        key.interestOps(key.interestOps() & ~SelectionKey.OP_WRITE);
    }
}
