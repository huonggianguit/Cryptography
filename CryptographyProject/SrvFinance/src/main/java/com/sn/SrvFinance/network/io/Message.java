package com.sn.SrvFinance.network.io;

import javax.imageio.ImageIO;
import java.awt.image.BufferedImage;
import java.io.*;
import java.util.Arrays;

public class Message implements AutoCloseable{
    public byte cmd;
    public byte subCmd;
    public byte[] data;
    public Message(int paramCmd,int paramSubCmd) {
        this((byte) paramCmd, (byte) paramSubCmd);
    }
    private ByteArrayOutputStream os;
    private DataOutputStream dos;
    private ByteArrayInputStream is;
    private DataInputStream dis;

    public Message(byte paramCmd,byte paramSubCmd) {
        this.cmd = paramCmd;
        this.subCmd = paramSubCmd;
        this.os = new ByteArrayOutputStream();
        this.dos = new DataOutputStream(this.os);
    }

    public Message(byte paramCmd,byte paramSubCmd, byte[] paramArrayOfByte) {
        this.cmd = paramCmd;
        this.subCmd = paramSubCmd;
        this.is = new ByteArrayInputStream(paramArrayOfByte);
        this.dis = new DataInputStream(this.is);
      //  System.out.println("Nhận dc cmd " + this.cmd + " subcmd " + this.subCmd);
    }


    @Override
    public void close() {
        // Sử dụng Close để try-with-resources auto Close
        try {
            if (this.dis != null) {
                this.dis.close();
            }
            if (this.dos != null) {
                this.dos.close();
            }
            if (this.is != null) {
                this.is.close();
            }
            if (this.os != null) {
                this.os.close();
            }
        } catch (IOException e) {
            // log hoặc bỏ qua nếu cần
        }
    }
    public void dispose() {
        close();
        this.dis = null;
        this.is = null;
        this.dos = null;
        this.os = null;
    }
    public void prepareToRead(){
        try {
            data = os.toByteArray(); // lẩy raw payload

            int len = data.length; // size payload

            os.reset(); // xóa raw

            // điền lại thành 1 gói
            dos.writeByte(cmd);
            dos.writeByte(subCmd);
            if(cmd!=9&&cmd!=10){
                dos.writeShort(len);
                dos.write(data);
            }else{
                dos.write(data);
            }

            dos.flush();
            //Chuyển sang stream đọc để đọc gửi vào buf
            this.data = os.toByteArray();
            this.is = new ByteArrayInputStream(data);
            this.dis = new DataInputStream(is);
            //System.out.println("[DEBUG] Data chuyển đổi msg: " + Arrays.toString(data));
        } catch (IOException e) {
            e.printStackTrace(); // hoặc xử lý lỗi
        }
    }

    public int availableRead(){
        return is.available();
    }
    public int read() {
        try {
            return dis.read();
        } catch (IOException e) {
            e.printStackTrace();
            return -1;
        }
    }

    public int read(byte[] paramArrayOfByte) {
        try {
            return dis.read(paramArrayOfByte);
        } catch (IOException e) {
            e.printStackTrace();
            return -1;
        }
    }

    public int read(byte[] paramArrayOfByte, int paramInt1, int paramInt2) {
        try {
            return dis.read(paramArrayOfByte, paramInt1, paramInt2);
        } catch (IOException e) {
            e.printStackTrace();
            return -1;
        }
    }

    public boolean readBoolean() {
        try {
            return dis.readBoolean();
        } catch (IOException e) {
            e.printStackTrace();
            return false;
        }
    }

    public byte readByte() {
        try {
            return dis.readByte();
        } catch (IOException e) {
            e.printStackTrace();
            return 0;
        }
    }


    public byte[] readBytes(int maxToRead) {
        try {
            int available = dis.available();
            int toRead = Math.min(maxToRead, available);

            if (toRead <= 0) {
                System.out.println("[readBytes] Không có gì để đọc (available = " + available + ")");
                return new byte[0];
            }
            byte[] result = new byte[toRead];
            int actual = dis.read(result);
            System.out.println("[readBytes] Yêu cầu: " + maxToRead + ", có: " + available + " → đọc: " + actual);
            return actual > 0 ? result : new byte[0];
        } catch (IOException e) {
            System.err.println("[readBytes] Lỗi khi đọc: " + e.getMessage());
            return new byte[0];
        }
    }


    public short readShort() {
        try {
            return dis.readShort();
        } catch (IOException e) {
            e.printStackTrace();
            return 0;
        }
    }

    public int readInt() {
        try {
            return dis.readInt();
        } catch (IOException e) {
            e.printStackTrace();
            return 0;
        }
    }

    public long readLong() {
        try {
            return dis.readLong();
        } catch (IOException e) {
            e.printStackTrace();
            return 0L;
        }
    }

    public float readFloat() {
        try {
            return dis.readFloat();
        } catch (IOException e) {
            e.printStackTrace();
            return 0.0f;
        }
    }

    public double readDouble() {
        try {
            return dis.readDouble();
        } catch (IOException e) {
            e.printStackTrace();
            return 0.0;
        }
    }

    public char readChar() {
        try {
            return dis.readChar();
        } catch (IOException e) {
            e.printStackTrace();
            return 0;
        }
    }

    public String readUTF() {
        try {
            return dis.readUTF();
        } catch (IOException e) {
            e.printStackTrace();
            return null;
        }
    }

    public void readFully(byte[] paramArrayOfByte) {
        try {
            dis.readFully(paramArrayOfByte);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void readFully(byte[] paramArrayOfByte, int paramInt1, int paramInt2) {
        try {
            dis.readFully(paramArrayOfByte, paramInt1, paramInt2);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public int readUnsignedByte() {
        try {
            return dis.readUnsignedByte();
        } catch (IOException e) {
            e.printStackTrace();
            return 0;
        }
    }
    public int readUnsigned24() {
        try {
            int b1 = dis.readUnsignedByte(); // byte cao
            int b2 = dis.readUnsignedByte(); // byte giữa
            int b3 = dis.readUnsignedByte(); // byte thấp

            return (b1 << 16) | (b2 << 8) | b3;
        } catch (IOException e) {
            e.printStackTrace();
            return -1;
        }
    }

    public int readUnsignedShort() {
        try {
            return dis.readUnsignedShort();
        } catch (IOException e) {
            e.printStackTrace();
            return 0;
        }
    }

// ------------------------ WRITE ----------------------------

    public void write(byte[] paramArrayOfByte) {
        try {
            dos.write(paramArrayOfByte);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void write(byte[] paramArrayOfByte, int paramInt1, int paramInt2) {
        try {
            dos.write(paramArrayOfByte, paramInt1, paramInt2);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void write(int paramInt) {
        try {
            dos.write(paramInt);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void writeBoolean(boolean paramBoolean) {
        try {
            dos.writeBoolean(paramBoolean);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void writeByte(int paramInt) {
        try {
            dos.writeByte(paramInt);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
    public void writeUnsignedByte(int value) {
        try {
            if (value < 0 || value > 255) {
                throw new IllegalArgumentException("Giá trị phải trong khoảng 0–255");
            }
            dos.writeByte(value & 0xFF);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
    public void writeBytes(String paramString) {
        try {
            dos.writeBytes(paramString);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void writeChar(int paramInt) {
        try {
            dos.writeChar(paramInt);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void writeChars(String paramString) {
        try {
            dos.writeChars(paramString);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void writeDouble(double paramDouble) {
        try {
            dos.writeDouble(paramDouble);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void writeFloat(float paramFloat) {
        try {
            dos.writeFloat(paramFloat);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void writeInt(int paramInt) {
        try {
            dos.writeInt(paramInt);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void writeLong(long paramLong) {
        try {
            dos.writeLong(paramLong);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void writeShort(int paramInt) {
        try {
            dos.writeShort(paramInt);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
    public void writeUnsignedShort(int value) {
        try {
            if (value < 0 || value > 0xFFFF) {
                throw new IllegalArgumentException("Giá trị phải nằm trong khoảng 0 đến 65535 (2 byte không dấu)");
            }
            dos.writeByte((value >>> 8) & 0xFF); // byte cao
            dos.writeByte(value & 0xFF);        // byte thấp
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
    public void writeUInt24(int value) {
        try {
            dos.writeByte((value >>> 16) & 0xFF); // byte cao nhất
            dos.writeByte((value >>> 8) & 0xFF);  // byte giữa
            dos.writeByte(value & 0xFF);          // byte thấp nhất
        } catch (IOException e) {
            e.printStackTrace();
        }
    }


    public void writeUTF(String paramString) {
        try {
            dos.writeUTF(paramString);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public BufferedImage readImage() {
        try {
            int i = readInt();
            byte[] arrayOfByte = new byte[i];
            read(arrayOfByte);
            return ImageIO.read(new ByteArrayInputStream(arrayOfByte));
        } catch (IOException e) {
            e.printStackTrace();
            return null;
        }
    }

    public void writeImage(BufferedImage paramBufferedImage, String paramString) {
        try {
            ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
            ImageIO.write(paramBufferedImage, paramString, byteArrayOutputStream);
            byte[] arrayOfByte = byteArrayOutputStream.toByteArray();
            writeInt(arrayOfByte.length);
            write(arrayOfByte);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
