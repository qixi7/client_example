using System;
using System.IO;
using System.Text;
using Google.Protobuf;
using Public.Log;
using Public.Net;

namespace example_client
{
    class Program
    {
        public static Connection conn; // 连接实例

        // step_1: 定义 连接服务器成功委托
        private static void ConnectSuccessEvent(Connection conn)
        {
            LogHelper.DebugF("<Client> ConnectSuccessEvent with type={0}, ip={1}, port={2}",
                conn.InitParam.connType.ToString(), conn.InitParam.IP, conn.InitParam.Port);

            Pb.C2S_SyncRoleInfo pbMsg = new Pb.C2S_SyncRoleInfo();
            pbMsg.RoleID = 1001;
            pbMsg.Name = "hehe";

            GameNetPack pack = new GameNetPack();
            pack.msgID = (ushort)Pb.Game_Msg.MsgSyncRoleInfo;
            pack.body = pbMsg.ToByteArray();
            conn.SendData(pack);
        }

        // step_2: 定义 连接服务器失败委托
        private static void ConnectFailedEvent(Connection conn, ConnectionError error)
        {
            LogHelper.ErrorF("<Client> ConnectFailedEvent with type={0}, ip={1}, port={2}, err={3}",
                conn.InitParam.connType.ToString(), conn.InitParam.IP, conn.InitParam.Port, error.ToString());
        }

        // step_3: 定义 接受消息委托
        private static void RecvMsgEvent(Connection conn, GameNetPack pack)
        {
            LogHelper.DebugF("<Client> RecvMsgEvent Called. msgID={0}, msg={1}",
                pack.msgID, Encoding.UTF8.GetString(pack.body));
        }

        // run example
        public static void RunExample()
        {
            // --------- 测试准备工作. 忽略 Begin-------
            string ip = "127.0.0.1";
            int port = 17000;
            ConnectionType connType = ConnectionType.TCP;
            // --------- 测试准备工作. 忽略 End---------

            // step_4: 创建连接
            ConnectionParam param = new ConnectionParam(ip, port, connType);
            conn = new Connection(param);
            conn.ConnectedEvent = ConnectSuccessEvent;
            conn.ConnectErrorEvent = ConnectFailedEvent;
            conn.RecvMsgEvent = RecvMsgEvent;
            conn.Connect();
            LogHelper.DebugF("<Client> Begin Connect, ip={0}, port={1}, Type={2}",
                ip, port, connType.ToString());

            // 模拟unity Update
            while (true)
            {
                conn.Update();
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            RunExample();
        }
    }
}