using System.ServiceModel;
using System;
using MySql.Data.MySqlClient;
using ChatLibrary;

namespace ChatServer
{
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Single,
        InstanceContextMode = InstanceContextMode.Single)]
    internal class MemberService : IMemberContract
    {
        private Database _Database;

        public MemberService(Database db)
        {
            _Database = db;
        }

        public void RegisterMember(string username, string password, Member.Gender orientation, DateTime birthday)
        {
            MySqlCommand cmd = _Database.CreateCommand(
                "INSERT INTO Members(Username, Password, Gender, Birthday)" +
                "VALUES(@Username, @Password, @Gender, @Birthday)");
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Password", password);
            cmd.Parameters.AddWithValue("@Gender", orientation.ToString());
            cmd.Parameters.AddWithValue("@Birthday", birthday.ToString("yyyy-MM-dd"));

            try
            {
                Int32 result = cmd.ExecuteNonQuery();

                if (result == 0)
                {
                    throw new FaultException("Failed to register account.", new FaultCode("Client"));
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                {
                    throw new FaultException("Account already exists.", new FaultCode("Client"));
                }
                else
                { 
                    throw new FaultException(ex.Message, new FaultCode("Server"));
                }
            }
        }
    }
}
