using MySql.Data.MySqlClient;
using System;
using System.Net;

namespace ChatServer
{
    internal class Database
    {
        private Boolean _Ready;

        private MySqlConnection _Connection;
        
        public Database(IPAddress address, String username, String password, String database)
        {
            String connnStr;

            connnStr = "server=" + address.ToString() + ";";
            connnStr += "uid=" + username + ";";
            connnStr += "pwd=" + password + ";";
            connnStr += "database=" + database + ";";

            _Connection = new MySqlConnection(connnStr);

            _Ready = false;
        }

        public void Open()
        {
            if (_Ready)
            {
                throw new InvalidOperationException("The database connection can't be re-initialised because it was initialised before.");
            }

            try
            {
                _Connection.Open();
            }
            catch (MySqlException ex)
            {
                throw new ApplicationException("Failed to connect to database", ex);
            }

            _Ready = !_Ready;
        }

        public bool IsReady()
        {
            return _Ready;
        }

        public MySqlCommand CreateCommand(String query)
        {
            if (!_Ready)
            {
                throw new InvalidOperationException("The database connection was not initialised yet.");
            }

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _Connection;
            cmd.CommandText = query;
            cmd.Prepare();

            return cmd;
        }

        public void Close()
        {
            if (!_Ready)
            {
                throw new InvalidOperationException("The database connection can't be terminated because was never initialised.");
            }

            if (_Connection != null)
            {
                _Connection.Close();
            }

            _Ready = !_Ready;
        }
    }
}
