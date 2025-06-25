package com.sn.SrvFinance.server;

import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.mysql.cj.xdevapi.Client;
import com.sn.SrvFinance.mySQL.SqlConnection;
import com.sn.SrvFinance.mySQL.SqlResultSet;
import com.sn.SrvFinance.network.handler.IMessageReceiver;
import com.sn.SrvFinance.network.io.Message;
import com.sn.SrvFinance.network.session.ISession;
import com.sn.SrvFinance.network.session.Session;
import com.sn.SrvFinance.server.io.MySession;
import com.sn.SrvFinance.services.Service;
import com.sn.SrvFinance.user.User;
import org.mindrot.jbcrypt.BCrypt;

import javax.swing.filechooser.FileSystemView;
import java.io.File;
import java.io.FileOutputStream;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

public class Controller implements IMessageReceiver {
    private  static Controller instance;
    public static Controller gI() {
        if (instance == null) {
            return new Controller();
        }
        return instance;
    }
    @Override
    public void onMessage(ISession s, Message msg) {
        if(s==null) {
            System.out.println("Session is null");
            return;
        }
        if(msg==null) {
            System.out.println("msg is null");
            return;
        }
        MySession session = (MySession) s;
        byte cmd = msg.cmd;
        byte subCmd = msg.subCmd;
        if(cmd == 6){
            /* cmd 6 == Check Key. và xử lý login
            + Server mở cổng = public, phòng thủ chổ này, Client chứa 1 key , check Key Nếu key sai chạy cmd block ip
            + Đăng nhập thành công, Add Session vào User để có thể chạy đa nền tảng.
            +
             */
            if(subCmd==0){
                String key = msg.readUTF();
               // System.out.println("key:"+key);
                if (key.equals(ServerManager.keyClient)){
                    session.isActive = true;
                   // System.out.println("CHUYÊN CỜ:");
                } else{
                    ClientManager.gI().kickSession(session);
                   // System.out.println("XÓA SESSION 1");
                }
            }else{
                if(!session.isActive){
                    ClientManager.gI().kickSession(session);
                  //  System.out.println("XÓA SESSION 2");
                    return;
                }
                switch(subCmd){
                    case 1 -> {
                        /*Login, User và Password gửi là hàng read, để trong ram (để trong buffer, ko lấy biến tạm ra dễ bị trích xuất)
                        *  User Đọc UTF check trong SQL xong rồi lấy has password, has này là has BCrypt, Vì sao dùng has này vì mấy thằng md5... hiện tạo ko an toàn
                        * Nghe nói thế, chứ ko hiểu nó dò kiểu gì ảo ma. Mà hiện tại thấy mấy web kia họ toàn xài BCrypt, nên xài.
                        * Password sẽ đưa vào 1 hàm để case has so sánh luôn, nếu so sánh chuẩn, thì đăng nhập dc, ko thì ko đăng nhập dc
                        * Làm vậy để làm gì? Vì Mục đích làm Server này là nơi chứa dữ liệu trên SQL< ngay cả khi SQL bị leak thì vẫn ko thể có được data thật.
                        * SQL lưu passwork hash, Client gửi pass real qua check. Như v giả định có dc hash pass trong SQL cũng ko thể đăng nhập từ client khác dc.
                        * Sau đó có thể dùng passwork làm key SSE luôn, và mọi raw gửi sẽ lưu header là id user và raw là đã mã hóa SSE
                        * Kết hợp vs sinh cái gì đấy mỗi lần gửi raw mã hóa SSE và lưu cái đó lại thì ko bị trích xuất là search cái gì để có thêm thông tin dò.
                        * */
                        DoLogin(session,msg);
                    }
                    case 2 -> {
                        DoRegister(session,msg);
                    }
                }
            }

        }
        if(!session.isActive){
            ClientManager.gI().kickSession(session);
        }

        User user = session.user;
        if(user==null) {
            System.out.println("user is null");
           // ClientManager.gI().kickSession(session);
        }
        switch (cmd){
//            case 7 -> { //Gói tin nhận FirtToken +  file
//                switch (subCmd){ //có payloadSize là 2 byte (max) 64kb
//                    case 0 -> { // Gửi file + Token Search đầu tiên
//                        String tokenFirst = toHexString(msg.readBytes(32));
//                        int FileIdLength = msg.readUnsignedByte();
//                        String FileID = toHexString(msg.readBytes(FileIdLength));
//                        int rawLength =msg.readUnsignedShort();
//                        byte[] raw = msg.readBytes(rawLength);
//
//                    }
//                    case 1 -> {// Gửi fileID + List Token Search
//                        String tokenFirst = toHexString(msg.readBytes(32));
//                        int FileIdLength = msg.readUnsignedByte();
//                        String FileID = toHexString(msg.readBytes(FileIdLength));
//                        int rawLength =msg.readUnsigned24();
//                        byte[] raw = msg.readBytes(rawLength);
//                        AddFileFirstToken(user,tokenFirst,FileID,raw);
//                    }
//                }
//            }
//            case 8 -> { // Gửi fileID + list Token
//                switch (subCmd) {
//                    case 0 -> {
//                        int FileIdLength = msg.readUnsignedByte();
//                        String FileID = toHexString(msg.readBytes(FileIdLength));
//                        int rawLength =msg.readUnsignedShort();
//                        List<String> listToken= new ArrayList<>();
//                        for(int i=0;i<rawLength;i++){
//                            listToken.add(toHexString(msg.readBytes(32)));
//                        }
//                        AddListToken(user,FileID,listToken);
//                    }
//
//                }
//            }
            case 7 -> { // Gói tin nhận FirstToken + file
                switch (subCmd) {
                    case 0 -> { // Gửi file + Token Search đầu tiên (64KB)
                        String tokenFirst = toHexString(msg.readBytes(32));
                        int FileIdLength = msg.readUnsignedByte();
                        String FileID = toHexString(msg.readBytes(FileIdLength));
                        int rawLength = msg.readUnsignedShort();
                        byte[] raw = msg.readBytes(rawLength);

                        System.out.println("📦 [CMD 7 - subCmd 0] Nhận File + FirstToken");
                        System.out.println("🔑 TokenFirst: " + tokenFirst);
                        System.out.println("🆔 FileID: " + FileID);
                        System.out.println("📄 rawLength: " + rawLength);
                        System.out.println("📄 rawBytes: " + raw.length);
                        AddFileFirstToken(user, tokenFirst, FileID, raw);
                    }
                    case 1 -> { // Gửi file + FirstToken (16MB)
                        String tokenFirst = toHexString(msg.readBytes(32));
                        int FileIdLength = msg.readUnsignedByte();
                        String FileID = toHexString(msg.readBytes(FileIdLength));
                        int rawLength = msg.readUnsigned24();
                        byte[] raw = msg.readBytes(rawLength);

                        System.out.println("📦 [CMD 7 - subCmd 1] Nhận File + FirstToken (16MB)");
                        System.out.println("🔑 TokenFirst: " + tokenFirst);
                        System.out.println("🆔 FileID: " + FileID);
                        System.out.println("📄 rawLength: " + rawLength);
                        System.out.println("📄 rawBytes: " + raw.length);

                        AddFileFirstToken(user, tokenFirst, FileID, raw);
                    }
                }
            }
            case 8 -> { // Gửi fileID + list Token
                switch (subCmd) {
                    case 0 -> {
                        int FileIdLength = msg.readUnsignedByte();
                        String FileID = toHexString(msg.readBytes(FileIdLength));
                        int tokenCount = msg.readUnsignedShort();

                        List<String> listToken = new ArrayList<>();
                        System.out.println("📦 [CMD 8 - subCmd 0] Nhận List Token cho File");
                        System.out.println("🆔 FileID: " + FileID);
                        System.out.println("🔢 TokenCount: " + tokenCount);

                        for (int i = 0; i < tokenCount; i++) {
                            String token = toHexString(msg.readBytes(32));
                            listToken.add(token);
                            System.out.println("   🔹 Token[" + i + "]: " + token);
                        }
                        AddListToken(user, FileID, listToken);
                    }
                    case 1 ->{
                        int FileIdLength = msg.readUnsignedByte();
                        String FileID = toHexString(msg.readBytes(FileIdLength));
                        int tokenCount = msg.readUnsignedShort();
                        List<String> listToken = new ArrayList<>();
                        for (int i = 0; i < tokenCount; i++) {
                            String token = toHexString(msg.readBytes(32));
                            listToken.add(token);
                        }
                        userDownFile(user, FileID,listToken);
                    }
                }
            }
            case 9 ->{
                switch (subCmd) {
                    case 0 -> {
                        String token = toHexString(msg.readBytes(32));
                        userSearchToken(user, token);
                    }
                }
            }
        }
    }

    private void DoLogin(MySession s, Message msg) {
        SqlResultSet rs = null;
        try {
            String username = msg.readUTF();
            String password = msg.readUTF();
            rs = SqlConnection.executeQuery("select * from users where username = ?", username);
            if (rs.first()) {
                String passHash = rs.getString("password");
                boolean result = BCrypt.checkpw(password, passHash);
                if (result) {
                    long id = rs.getLong("id");
                    if(ClientManager.gI().hasUser(id)){
                        s.user.setId(id);
                        ClientManager.gI().getUser(id).addSession(s);
                    } else {
                        User us = new User();
                        us.setId(id);
                        us.setUsername(username);
                        us.addSession(s);
                        if(s.user!=null){
                            s.user = null;
                        }
                        s.user = us;
                        ClientManager.gI().addUser(id, us);
                    }
                    Service.gI().sendMessLogin(s,true);
                  //  System.out.println("✅ Đăng nhập thành công: " + username + " id: " + id);
                } else {
                    System.out.println("Gửi msg Login");
                    Service.gI().sendMessLogin(s,false);
                  //  System.out.println("❌ Sai mật khẩu (checkpw false)");
                }
            } else {
                Service.gI().sendMessLogin(s,false);
              //  System.out.println("❌ Không tìm thấy user: " + username);
            }
        } catch (Exception e) {
            System.out.println("‼️ Exception trong DoLogin:");
            e.printStackTrace();
        }
    }
    public void DoRegister(MySession s, Message msg) {
        SqlResultSet rs = null;
        try {
            String username = msg.readUTF();
            String password = msg.readUTF();
            rs = SqlConnection.executeQuery("select * from users where username = ?", username);
            if (rs.first()) {
                Service.gI().sendMessRegister(s, false);
                return;
            } else
            {
                String hashedPassword = BCrypt.hashpw(password, BCrypt.gensalt());
                SqlConnection.executeInsert(
                        "INSERT INTO users (username, password) VALUES (?, ?)",
                        username, hashedPassword
                );
                Service.gI().sendMessRegister(s, true);
            }
        } catch (Exception e) {
            System.out.println("‼️ Exception trong DoLogin:");
            e.printStackTrace();
        }
    }


    public void AddFileFirstToken(User uc, String TokenFirst, String fileID, byte[] rawFile) {
        try {
            ObjectMapper mapper = new ObjectMapper();
            System.out.println("📥 AddFileFirstToken:");
            System.out.println("🔑 TokenFirst: " + TokenFirst);
            System.out.println("🆔 FileID: " + fileID);
            System.out.println("👤 UserID: " + uc.getId());
            System.out.println("📄 rawFile.length: " + rawFile.length);

            SqlResultSet rs = SqlConnection.executeQuery(
                    "SELECT * FROM encryptedindex WHERE token = ?", TokenFirst
            );

            if (!rs.first()) {
                System.out.println("🆕 Token chưa có trong DB → tạo mới");
                String jsonArray = mapper.writeValueAsString(List.of(fileID));
                SqlConnection.executeInsert(
                        "INSERT INTO encryptedindex (token, file_id, user_id) VALUES (?, ?, ?)",
                        TokenFirst, jsonArray, uc.getId()
                );
            } else {
                System.out.println("✅ Token đã có trong DB → kiểm tra cập nhật fileID");
                String fileIdJson = rs.getString("file_id");
                List<String> fileList;

                if (fileIdJson != null && !fileIdJson.isEmpty()) {
                    fileList = mapper.readValue(fileIdJson, new TypeReference<List<String>>() {});
                } else {
                    fileList = new ArrayList<>();
                }

                if (!fileList.contains(fileID)) {
                    fileList.add(fileID);
                    String updatedJson = mapper.writeValueAsString(fileList);
                    SqlConnection.executeUpdate(
                            "UPDATE encryptedindex SET file_id = ? WHERE token = ?",
                            updatedJson, TokenFirst
                    );
                    System.out.println("📝 Đã thêm mới fileID vào danh sách token");
                } else {
                    System.out.println("⚠️ fileID đã tồn tại trong danh sách → không thêm lại");
                }
            }

            File desktopDir = FileSystemView.getFileSystemView().getHomeDirectory();
            String desktopPath = desktopDir.getAbsolutePath();

            File folder = new File(desktopDir, "DataServerDoAn");
            if (!folder.exists()) {
                folder.mkdirs();
                System.out.println("📁 Đã tạo thư mục: " + folder.getAbsolutePath());
            }

            File outFile = new File(folder, fileID);
            try (FileOutputStream fos = new FileOutputStream(outFile)) {
                fos.write(rawFile);
                System.out.println("✅ Ghi file thành công: " + outFile.getAbsolutePath());
            }

        } catch (Exception e) {
            System.err.println("‼️ Lỗi trong AddFileFirstToken:");
            e.printStackTrace();
        }
    }
    public void AddListToken(User uc, String fileID, List<String> tokenList) {
        try {
            ObjectMapper mapper = new ObjectMapper();
            System.out.println("📥 AddListToken:");
            System.out.println("🆔 FileID: " + fileID);
            System.out.println("👤 UserID: " + uc.getId());
            System.out.println("🔢 Tổng số token: " + tokenList.size());
            for (String token : tokenList) {
                SqlResultSet rs = SqlConnection.executeQuery(
                        "SELECT * FROM encryptedindex WHERE token = ?", token
                );
                if (!rs.first()) {
                    System.out.println("🆕 Token mới → tạo mới: " + token);
                    String jsonArray = mapper.writeValueAsString(List.of(fileID));
                    SqlConnection.executeInsert(
                            "INSERT INTO encryptedindex (token, file_id, user_id) VALUES (?, ?, ?)",
                            token, jsonArray, uc.getId()
                    );
                } else {
                    String fileIdJson = rs.getString("file_id");
                    List<String> fileList;

                    if (fileIdJson != null && !fileIdJson.isEmpty()) {
                        fileList = mapper.readValue(fileIdJson, new TypeReference<List<String>>() {});
                    } else {
                        fileList = new ArrayList<>();
                    }
                    if (!fileList.contains(fileID)) {
                        fileList.add(fileID);
                        String updatedJson = mapper.writeValueAsString(fileList);
                        SqlConnection.executeUpdate(
                                "UPDATE encryptedindex SET file_id = ? WHERE token = ?",
                                updatedJson, token
                        );
                        System.out.println("📝 Đã cập nhật thêm fileID cho token: " + token);
                    } else {
                        System.out.println("⚠️ Token đã chứa fileID → bỏ qua: " + token);
                    }
                }
            }
        } catch (Exception e) {
            System.err.println("‼️ Lỗi trong AddListToken:");
            e.printStackTrace();
        }
    }

    public static String toHexString(byte[] bytes) {
        StringBuilder sb = new StringBuilder(bytes.length * 2);
        for (byte b : bytes) {
            sb.append(String.format("%02x", b));
        }
        return sb.toString();
    }
    public static byte[] fromHexString(String hex) {
        int len = hex.length();
        if (len % 2 != 0) {
            throw new IllegalArgumentException("Chuỗi hex không hợp lệ");
        }

        byte[] data = new byte[len / 2];
        for (int i = 0; i < len; i += 2) {
            data[i / 2] = (byte) ((Character.digit(hex.charAt(i), 16) << 4)
                    + Character.digit(hex.charAt(i + 1), 16));
        }
        return data;
    }

    private void userSearchToken(User uc, String token) {
        try {
            ObjectMapper mapper = new ObjectMapper();
            Set<String> uniqueFileIDs = new HashSet<>(); // dùng Set để loại trùng
            SqlResultSet rs = SqlConnection.executeQuery(
                    "SELECT * FROM encryptedindex WHERE token = ?", token
            );
            while (rs.next()) {
                long userId = rs.getLong("user_id");
                if (userId != uc.getId()) {
                    continue;
                }
                String fileIDJson = rs.getString("file_id");

                // Parse JSON array: ví dụ ["abc", "def", "ghi"]
                List<String> fileIDs = mapper.readValue(fileIDJson, new TypeReference<List<String>>() {});

                uniqueFileIDs.addAll(fileIDs); // tự loại trùng

            }
            if (!uniqueFileIDs.isEmpty()) {
               Service.gI().sendListFileSearch(uc,uniqueFileIDs);
            } else{
                Service.gI().sendMessNotFoundFile(uc);
            }
        } catch (Exception e) {
            System.err.println("‼️ Lỗi trong AddListToken:");
            e.printStackTrace();
        }
    }

    private void userDownFile(User uc, String fileID, List<String> tokenList) {
        try {
            System.out.println("📥 userDownFile - fileID: " + fileID);
            System.out.println("📥 userDownFile - token count: " + tokenList.size());
            ObjectMapper mapper = new ObjectMapper();
            for (String token : tokenList) {
                System.out.println("🔎 Đang kiểm tra token: " + token);
                SqlResultSet rs = SqlConnection.executeQuery(
                        "SELECT * FROM encryptedindex WHERE token = ?", token
                );
                boolean flag = false;
                while (rs.next() && !flag) {
                    long userId = rs.getLong("user_id");
                    System.out.println("  ↪ user_id trong DB: " + userId);
                    if (userId != uc.getId()) {
                        System.out.println("  ❌ userId không khớp, bỏ qua");
                        continue;
                    }
                    String fileIDJson = rs.getString("file_id");
                    System.out.println("  ✅ userId khớp. file_id JSON: " + fileIDJson);
                    List<String> fileIDs = mapper.readValue(fileIDJson, new TypeReference<List<String>>() {});
                    System.out.println("  🔎 Kiểm tra xem chứa fileID: " + fileID + " ?");
                    if (fileIDs.contains(fileID)) {
                        System.out.println("  ✅ Tìm thấy fileID trong danh sách, gửi file!");
                        flag = true;
                        Service.gI().sendFile(uc, fileID);
                        return;
                    } else {
                        System.out.println("  ❌ Không chứa fileID");
                    }
                }
            }
        } catch (Exception e) {
            System.err.println("‼️ Lỗi trong userDownFile:");
            e.printStackTrace();
        }
    }

}
