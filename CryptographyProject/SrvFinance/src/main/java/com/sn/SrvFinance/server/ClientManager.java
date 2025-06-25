package com.sn.SrvFinance.server;
import com.sn.SrvFinance.network.session.ISession;
import com.sn.SrvFinance.server.io.MySession;
import com.sn.SrvFinance.user.User;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;
/*T đang viết CLient, Class này để làm gì?:
+ Nó để put Session vào Client.
+ Trong Client chứa list Users, Login, Check xem User có tồn tại ko. Nếu có thì New User, ko thì add thêm Session vào User.
+ Khi gửi Mess cần có hàm gửi All Session. Vào User tìm All Seesion để gửi.
*/
public class ClientManager {
    private static ClientManager instance;
    public static ClientManager gI(){
        if(instance == null){
            instance = new ClientManager();
        }
        return instance;
    }
    private ClientManager() {
       // new Thread(this).start();
    }
    public int id = 1_000_000_000;
    private final Map<Long, User> Users = new ConcurrentHashMap<>(); //ID Account User và Object User

    public void kickSession(ISession session) {
            for (User user : Users.values()) {
                if(user.hasNoSession()){
                    System.out.println("Kicking session...");
                    removeUser(user.getId());
                    user.dispose();
                    user = null;
                }
                if (user != null && user.findSession(session)) {

                    user.removeSession(session);
                    if(user.hasNoSession()){
                        removeUser(user.getId());
                        user.dispose();
                        user = null;
                    }
                    break; // dừng ngay khi đã xử lý xong
                }
            }
        if(session != null){
            session.disconnect();
        }
    }
    public boolean hasUser(long id) {
        return Users.containsKey(id);
    }
    public User getUser(long id) {
        return Users.get(id); // Trả về null nếu không tồn tại
    }
    public void addUser(long id, User user) {
        Users.put(id, user);
    }
    public void removeUser(long id) {
        System.out.println( "Removing user: " + id );
        Users.remove(id);
    }


}
