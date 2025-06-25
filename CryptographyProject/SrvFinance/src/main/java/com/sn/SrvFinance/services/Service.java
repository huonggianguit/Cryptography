package com.sn.SrvFinance.services;

import com.sn.SrvFinance.network.io.Message;
import com.sn.SrvFinance.server.Controller;
import com.sn.SrvFinance.server.io.MySession;
import com.sn.SrvFinance.user.User;

import javax.swing.filechooser.FileSystemView;
import java.io.File;
import java.util.ArrayList;
import java.util.List;
import java.util.Set;
import java.nio.file.Files;
import java.io.IOException;
public class Service {
    private static Service instance;

    public static Service gI() {
        if (instance == null) {
            instance = new Service();
        }
        return instance;
    }

    public void sendMessLogin(MySession s, boolean flag) {
        try {
            Message msg = new Message(6, 1);
            msg.writeByte(flag ? 1 : 0);
            System.out.println("[sendMessLogin] Đã tạo message với flag = " + (flag ? 1 : 0));

            s.addMessage(msg);
            System.out.println("[sendMessLogin] Đã Add message login tới session");
        } catch (Exception e) {
            System.err.println("[sendMessLogin] Lỗi khi gửi message login: " + e.getMessage());
            e.printStackTrace();

        }
    }
    public void sendMessRegister(MySession s, boolean flag) {
        try {
            Message msg = new Message(6, 2);
            msg.writeByte(flag ? 1 : 0);
            s.addMessage(msg);
        } catch (Exception e) {
            System.err.println("[sendMessLogin] Lỗi khi gửi message Register: " + e.getMessage());
            e.printStackTrace();
        }
    }
    public void sendListFileSearch(User uc, Set<String> list ) {
        try {
            List<byte[]> listFileID = new ArrayList<>();
            for (String fileName : list) {
                listFileID.add(Controller.fromHexString(fileName));
            }
            Message msg = new Message(7, 0);
            msg.writeUnsignedByte(listFileID.size());
            for(byte[] fileID : listFileID) {
                msg.writeUnsignedByte(fileID.length);
                msg.write(fileID);
            }
            uc.addMessage(msg);
        } catch (Exception e) {
            System.err.println("[sendMessLogin] Lỗi khi gửi message Register: " + e.getMessage());
            e.printStackTrace();
        }
    }
    public void sendMessNotFoundFile(User uc) {
        Message msg = new Message(7, 0);
        msg.writeUnsignedByte(0);
    }
    public void sendFile(User uc, String fileName) {
        System.out.println("Send file: ");
        File desktopDir = FileSystemView.getFileSystemView().getHomeDirectory();
        File folder = new File(desktopDir, "DataServerDoAn");
        File targetFile = new File(folder, fileName);

        // Kiểm tra file tồn tại
        if (!targetFile.exists()) {
            System.err.println("❌ File không tồn tại: " + targetFile.getAbsolutePath());
            return;
        }
        try {
            byte[] filename = Controller.fromHexString(fileName);
            byte[] rawFile = Files.readAllBytes(targetFile.toPath());
            // Kiểm tra trong phạm vi 2 byte không dấu
            if (rawFile.length <= 0xFFFF) {
               Message msg = new Message(9,0);
               msg.writeUnsignedByte(filename.length);
               msg.write(filename);
               msg.writeUnsignedShort(rawFile.length);
               msg.write(rawFile);
               uc.addMessage(msg);
            } else if (rawFile.length <= 0xFFFFFF) {
                Message msg = new Message(10,0);
                msg.writeUnsignedByte(filename.length);
                msg.write(filename);
                msg.writeUInt24(rawFile.length);
                msg.write(rawFile);
                uc.addMessage(msg);
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}