using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Pro.ServerConnect
{
    public interface IMessageHandler
    {
        void onMessage(Message message);

        void onConnectionFail(bool isMain);

        void onDisconnected(bool isMain);

        void onConnectOK(bool isMain);
    }

}
