using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Security.Cryptography;

namespace SignUpLogin_by_MySql.Models
{
    public class DBLayer
    {
        MySqlConnection constr;
        public DBLayer(IConfiguration config)
        {
            constr = new MySqlConnection(config.GetConnectionString("constr"));
        }

        public int ExecuteQuery(string procname , MySqlParameter[] parameter)
        {
            MySqlCommand cmd = new MySqlCommand(procname, constr);
            cmd.CommandType = CommandType.StoredProcedure;
            if(parameter != null)
            {
                cmd.Parameters.AddRange(parameter);
            }
            cmd.Connection.Open();
            int res = cmd.ExecuteNonQuery();
            cmd.Connection.Clone();
            return res;
        }

        public DataTable table(string procname , MySqlParameter[] parameters)
        {
            MySqlCommand cmd = new MySqlCommand(procname,constr);
            cmd.CommandType = CommandType.StoredProcedure;
            if (parameters != null) 
            {
                cmd.Parameters.AddRange(parameters);
            }
            MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;
        }
    }
}
