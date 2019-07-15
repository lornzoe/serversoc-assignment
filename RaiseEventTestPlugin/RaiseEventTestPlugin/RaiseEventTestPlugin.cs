using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Hive;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;

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

                    reader.Close();
                    sql = "SELECT * FROM accountdb.login_account WHERE login_name = '" + playerName + "' AND password = '" + playerPassword + "'";
                    cmd = new MySqlCommand(sql, conn);
                    reader = cmd.ExecuteReader();

                    if (!reader.HasRows) // password doesnt match lol
                    {
                        ResponseMessage = "wrongPassword";
                    }
                    else
                    {
                        reader.Read();

                        ResponseMessage = "correctPassword";

                        object[] objArray1 = new object[4];

                        objArray1[0] = "playerName=";
                        objArray1[1] = reader[0];
                        objArray1[2] = ",position=";
                        objArray1[3] = reader[3];

                        ResponseMessage = string.Concat(objArray1);
                    }
                    reader.Close();

                }

                else // if the account DOESNT exists
                {
                    reader.Close();

                    this.PluginHost.LogDebug("asdf");
                    // we'll make a new account.
                    sql = "INSERT INTO `accountdb`.`login_account` (`login_name`, `password`) VALUES('" + playerName + "', '" + playerPassword + "')";
                    cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    ResponseMessage = "newPlayer";
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
                reader.Dispose();

                Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
                dictionary.Add(250, ResponseMessage);
                SendParameters parameters = new SendParameters();
                base.PluginHost.BroadcastEvent(0, 0, 0, 250, dictionary, 0, parameters);


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

        //private byte[] SerializeCustomPluginType(object o)
        //{
        //    CustomPluginType customObject = o as CustomPluginType; // cast it first
        //    if (customObject == null)
        //        return null;

        //    using (var s = new MemoryStream())
        //    {
        //        using (var bw = new BinaryWriter(s))
        //        {
        //            bw.Write(customObject.intField);
        //            bw.Write(customObject.byteField);
        //            bw.Write(customObject.stringField);

        //            return s.ToArray();
        //        }
        //    }
        //}

        //private object DeserializeCustomPluginType(byte[] bytes)
        //{
        //    CustomPluginType customObject = new CustomPluginType();
        //    using (var s = new MemoryStream(bytes))
        //    {
        //        using (var br = new BinaryReader(s))
        //        {
        //            customObject.intField = br.ReadInt32();
        //            customObject.byteField = br.ReadByte();
        //            customObject.stringField = br.ReadString();
        //        }
        //    }
        //    return customObject;
        //}

        //public override bool SetupInstance(IPluginHost host, Dictionary<string, string> config, out string errorMsg)
        //{
        //    PluginHost.LogDebug("Setting up overrided instance from RaiseEventTestPlugin.cs!");
        //    host.TryRegisterType(typeof(CustomPluginType), 1,
        //        SerializeCustomPluginType,
        //        DeserializeCustomPluginType);
        //    PluginHost.LogDebug("Finished setting up overrided instance from RaiseEventTestPlugin.cs, now returning!");
        //    PluginHost.LogDebug(" REEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
        //    return base.SetupInstance(host, config, out errorMsg);
        //}
    }
}
