package com.sn.SrvFinance.network.io;

import com.sn.SrvFinance.network.session.ISession;
import com.sn.SrvFinance.network.session.Session;
import com.sn.SrvFinance.server.ClientManager;
import lombok.Data;
import lombok.Getter;
import lombok.Setter;

import java.io.*;
import java.nio.ByteBuffer;
import java.nio.channels.SocketChannel;


public class ScBuf {
    @Getter
    private final SocketChannel socket;
    private final ByteBuffer buf; // Dùng để đọc raw từ socket channel
    private Session session;
    private final ByteArrayOutputStream os;
    private final DataOutputStream dos;
    private ByteArrayInputStream is;
    private DataInputStream dis;
    private byte[] Payload;

    public ScBuf(SocketChannel socket,Session s) {
        this.socket = socket;
        session= s;
        this.buf = ByteBuffer.allocate(8192);
        this.os = new ByteArrayOutputStream();
        this.dos = new DataOutputStream(this.os);
    }

    // ========== SOCKET INTERACTION ==========
    public void readFromSocket() {
        try {
            buf.clear(); // reset buffer để ghi mới
            int read = socket.read(buf); // ⛔ chỗ này có thể ném SocketException

            if (read == -1) {
                ClientManager.gI().kickSession(session);
                System.out.println("Đã Kick Session");
                return;
            }

            if (read == 0) {
                return;
            }

            buf.flip();
            byte[] raw = new byte[buf.remaining()];
            buf.get(raw);
            dos.write(raw);
            byte[] data = os.toByteArray();

            // xử lý data nếu cần
        } catch (IOException e) {
            // 🔴 Bắt cả Connection reset
            System.err.println("❌ Socket lỗi (client đóng đột ngột): " + e.getMessage());
            ClientManager.gI().kickSession(session);
            try {

                socket.close();
            } catch (IOException ex) {
                ex.printStackTrace();
            }
        }
    }

    public Message getMessageFormPayload() {
        byte[] Payload = os.toByteArray();
        if (Payload.length < 2) {
            return null;
        }
        byte cmd = Payload[0];
        byte subCmd = Payload[1];

        int endIndex = -1;
        if (cmd == 7) {
            int indexToken = 2;
            int indexLenName = indexToken + 32;

            if (Payload.length <= indexLenName) {
                return null;
            }

            int lenName = Payload[indexLenName] & 0xFF;
            int indexRawFileID = indexLenName + 1;
            int indexRawLength = indexRawFileID + lenName;

            int rawLength, indexRawData;
            if (subCmd == 0) {
                if (Payload.length < indexRawLength + 2) {
                    return null;
                }
                rawLength = ((Payload[indexRawLength] & 0xFF) << 8) | (Payload[indexRawLength + 1] & 0xFF);
                indexRawData = indexRawLength + 2;
            } else if (subCmd == 1) {
                if (Payload.length < indexRawLength + 3) {
                    return null;
                }
                rawLength = ((Payload[indexRawLength] & 0xFF) << 16)
                        | ((Payload[indexRawLength + 1] & 0xFF) << 8)
                        |  (Payload[indexRawLength + 2] & 0xFF);
                indexRawData = indexRawLength + 3;
            } else {
                return null;
            }

            endIndex = indexRawData + rawLength;
            if (Payload.length < endIndex) {
                return null;
            }
//            byte[] realPayload = new byte[endIndex - 2]; // bỏ cmd+subcmd
//            System.arraycopy(Payload, 2, realPayload, 0, realPayload.length);
//            // giữ lại phần thừa
//            byte[] remaining = new byte[Payload.length - endIndex];
//            System.arraycopy(Payload, endIndex, remaining, 0, remaining.length);
//            os.reset();
//            os.write(remaining, 0, remaining.length);

            byte[] realPayload = new byte[endIndex - 2];
            System.arraycopy(Payload, 2, realPayload, 0, realPayload.length);

            // giữ lại phần còn dư để lần sau đọc tiếp
            if (Payload.length > endIndex) {
                byte[] remaining = new byte[Payload.length - endIndex];
                System.arraycopy(Payload, endIndex, remaining, 0, remaining.length);
                os.reset();
                os.write(remaining, 0, remaining.length);
            } else {
                os.reset(); // hết gói
            }


            return new Message(cmd, subCmd, realPayload);
        }
        if (cmd == 8) {

            int indexLenName = 2;
            if (Payload.length <= indexLenName) {

                return null;
            }

            int lenName = Payload[indexLenName] & 0xFF;
            int indexRawFileID = indexLenName + 1;
            int indexTokenCount = indexRawFileID + lenName;

            if (Payload.length < indexTokenCount + 2) {

                return null;
            }

            int tokenCount = ((Payload[indexTokenCount] & 0xFF) << 8) | (Payload[indexTokenCount + 1] & 0xFF);
            endIndex = indexTokenCount + 2 + tokenCount * 32;

            if (Payload.length < endIndex) {
                return null;
            }
            byte[] realPayload = new byte[endIndex - 2];
            System.arraycopy(Payload, 2, realPayload, 0, realPayload.length);

            byte[] remaining = new byte[Payload.length - endIndex];
            System.arraycopy(Payload, endIndex, remaining, 0, remaining.length);
            os.reset();
            os.write(remaining, 0, remaining.length);

            return new Message(cmd, subCmd, realPayload);
        }
        if (Payload.length < 4) {
            return null;
        }
        int size = ((Payload[2] & 0xFF) << 8) | (Payload[3] & 0xFF);
        if (size < 0 || size > 32767) {
            os.reset();
            return null;
        }
        endIndex = 4 + size;
        if (Payload.length < endIndex) {
            return null;
        }
        byte[] realPayload = new byte[size];
        System.arraycopy(Payload, 4, realPayload, 0, size);
        byte[] remaining = new byte[Payload.length - endIndex];
        System.arraycopy(Payload, endIndex, remaining, 0, remaining.length);
        os.reset();
        os.write(remaining, 0, remaining.length);
        return new Message(cmd, subCmd, realPayload);
    }

    public void writeToSocket() throws IOException {
        System.out.println("[writeToSocket] Bắt đầu ghi socket...");
        buf.flip(); // chuyển sang read mode
        System.out.println("[writeToSocket] Trạng thái sau flip - limit: " + buf.limit() + ", position: " + buf.position());
        while (buf.hasRemaining()) {
            int written = socket.write(buf);
            System.out.println("[writeToSocket] Đã ghi được " + written + " byte");
            if (written == 0) {
                System.out.println("[writeToSocket] Ghi non-blocking bị nghẽn (written = 0), dừng lại.");
                break;
            }
        }
        buf.compact(); // chuyển về write mode
        System.out.println("[writeToSocket] Sau compact - position: " + buf.position() + ", remaining: " + buf.remaining());
    }


    public int writeBytes(byte[] data) {
        int writable = Math.min(buf.remaining(), data.length);
        if (writable <= 0) return 0;

        buf.put(data, 0, writable);
        return writable;
    }


    public void reset() {

        buf.clear();
    }


    public boolean hasRemaining() {
        return buf.hasRemaining();
    }

    public int remaining() {
        return buf.remaining();
    }

    public boolean hasPendingWrite() {
        return buf.position() > 0; // nghĩa là buffer có dữ liệu cần gửi (sau flip)
    }

}
