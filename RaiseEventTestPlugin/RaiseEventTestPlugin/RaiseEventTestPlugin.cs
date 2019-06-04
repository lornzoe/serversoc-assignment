using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Hive;
using MySql.Data;
using MySql.Data.MySqlClient;


using Photon.Hive.Plugin;
namespace TestPlugin
{
    public class RaiseEventTestPlugin : PluginBase
    {

        private string connStr;
        private MySqlConnection conn;

        public string ServerString
        {
            get;
            private set;
        }
        public int CallsCount
        {
            get;
            private set;
        }
        public RaiseEventTestPlugin()
        {
            this.UseStrictMode = true;
            this.ServerString = "ServerMessage";
            this.CallsCount = 0;

            ConnectToMySQL();

        }
        public override string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public void ConnectToMySQL()
        {
            // Connect to MySQL
            connStr =
           "server=localhost;user=root;database=accountdb;port=3306;password=password";
            conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public void DisconnectFromMySQL()
        {
            conn.Close();
        }

        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            
            try
            {
                base.OnRaiseEvent(info);
            }
            catch (Exception e)
            {
                this.PluginHost.BroadcastErrorInfoEvent(e.ToString(), info);
                return;
            }
            if (info.Request.EvCode == 1)
            {
               

                string RecvdMessage = Encoding.UTF8.GetString((byte[])info.Request.Data);


                string sql = "INSERT INTO login_account (idlogin_account, login_name, password) VALUES ('6','aassdd', 'asdasd')";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();


                //this.PluginHost.LogDebug(RecvdMessage);    


                //string playerName = GetStringDataFromMessage("PlayerName"); // playerName == Guest3721
                //string playerPassword = GetStringDataFromMessage("Password"); // playerPassword == 1234


                /*
                ++this.CallsCount;
                int cnt = this.CallsCount;
                string ReturnMessage = info.Nickname + " clicked the button. Now the count is " + cnt.ToString();
            this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
            senderActor: 0,
            targetGroup: 0,
            data: new Dictionary<byte, object>() { { (byte)245, ReturnMessage }},
            evCode: info.Request.EvCode,
            cacheOp: 0 );*/
            }
        }
    }
}
