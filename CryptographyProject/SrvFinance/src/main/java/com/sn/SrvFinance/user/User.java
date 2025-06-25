package com.sn.SrvFinance.user;

import com.sn.SrvFinance.network.io.Message;
import com.sn.SrvFinance.network.session.ISession;
import com.sn.SrvFinance.server.io.MySession;

import java.util.ArrayList;
import java.util.List;
import lombok.Getter;
import lombok.Setter;
@Getter
@Setter
public class User {
    private long id;
    private String username;
    private List<ISession> sessions = new ArrayList<>();

    public void addSession(ISession session) {
        sessions.add(session);
    }

    public void removeSession(ISession session) {
        sessions.remove(session);
    }


    public void addMessage(Message msg){
        for(ISession session : sessions){
            if(session!=null){
                session.addMessage(msg);
            }
        }
    }
    public boolean findSession(ISession session) {
        return sessions.contains(session);
    }
    public void dispose() {
        // Xóa toàn bộ session và giải phóng tài nguyên liên quan
        for (ISession session : sessions) {
            if (session != null) {
                session.dispose(); // nếu ISession có hàm dispose
            }
        }
        sessions.clear(); // dọn sạch danh sách session

    }
    public boolean hasNoSession() {
        return sessions.isEmpty();
    }

}
