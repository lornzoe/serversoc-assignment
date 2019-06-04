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

                string playerName = GetStringDataFromMessage("PlayerName", RecvdMessage); // playerName == Guest3721
                string playerPassword = GetStringDataFromMessage("Password", RecvdMessage); // playerPassword == 1234

                // string sql = "INSERT INTO login_account (login_name, password) VALUES ('aassdd', 'asdasd')";

                string sql = "SELECT * FROM accountdb.login_account WHERE login_name = '" + playerName + "'";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                //int check = // cmd.ExecuteNonQuery();'
                MySqlDataReader reader = cmd.ExecuteReader();

                //string sql = "INSERT INTO `accountdb`.`login_account` (`login_name`, `password`) VALUES('" + playerName + "', '" + playerPassword + "')";
                //MySqlCommand cmd = new MySqlCommand(sql, conn);
                //cmd.ExecuteNonQuery();

                string ResponseMessage;

                if (reader.HasRows) // if the account exists
                {
                    reader.Dispose();
                    sql = "SELECT * FROM accountdb.login_account WHERE login_name = '" + playerName + "' AND password = '" + playerPassword + "'";
                    cmd = new MySqlCommand(sql, conn);
                    reader = cmd.ExecuteReader();

                    if (!reader.HasRows) // password doesnt match lol
                    {
                        ResponseMessage = "Username's password does not match the database. Updating password.";
                    }
                    else
                    {
                        ResponseMessage = "Success. Logging you in now.";
                    }
                }

                else // if the account DOESNT exists
                {
                    // we'll make a new account.
                    sql = "INSERT INTO `accountdb`.`login_account` (`login_name`, `password`) VALUES('" + playerName + "', '" + playerPassword + "')";
                    cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    ResponseMessage = "Made a new entry.";
                }



                this.PluginHost.LogDebug(RecvdMessage);

                //++this.CallsCount;
                //int cnt = this.CallsCount;
                //string ReturnMessage = info.Nickname + " clicked the button. Now the count is " + cnt.ToString();

                //this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                //senderActor: 0,
                //targetGroup: 0,
                //data: new Dictionary<byte, object>() { { (byte)245, ResponseMessage } },
                //evCode: info.Request.EvCode,
                //cacheOp: 0);
            }
        }

        public string GetStringDataFromMessage(string key, string message)
        {
            string altkey = ' ' + key;

            var nameValuePairs = message.Split(new[] { ',' });
            var nameValuePair = nameValuePairs[0].Split(new[] { '=' });
            var nameValuePair1 = nameValuePairs[1].Split(new[] { '=' });

            if (nameValuePair[0] == key || nameValuePair[0] == altkey)
            {
                return nameValuePair[1];
            }

            //else if (nameValuePair1[0] == key || nameValuePair1[0] == altkey)
            //{
            //    return nameValuePair1[1];
            //}

            //else return null;
            
            else
                return nameValuePair1[1];

        }
    }
}
