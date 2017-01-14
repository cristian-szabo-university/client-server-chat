using MySql.Data.MySqlClient;
using System.IdentityModel.Selectors;
using System.ServiceModel;

namespace ChatServer
{
    internal class MemberValidator : UserNamePasswordValidator
    {
        private Database _Database;

        public MemberValidator(Database db)
        {
            _Database = db;
        }

        public override void Validate(string userName, string password)
        {
            MySqlCommand cmd = _Database.CreateCommand(
                "SELECT * FROM Members " + 
                "WHERE Username=@Username AND Password=@Password;");
            cmd.Parameters.AddWithValue("@Username", userName);
            cmd.Parameters.AddWithValue("@Password", password);

            try
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        throw new FaultException("Username or password is incorrect!", new FaultCode("Client"));
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw new FaultException(ex.Message, new FaultCode("Server"));
            }          
        }
    }
}
