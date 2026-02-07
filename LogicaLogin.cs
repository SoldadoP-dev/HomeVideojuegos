using System;
using MySql.Data.MySqlClient;

namespace HomeVideojuegos
{
    public class LogicalLogin
    {
        private string connStr = "Server=localhost;Port=3306;Database=home_videojuegos_db;Uid=root;Pwd=301206;SslMode=0;AllowPublicKeyRetrieval=True;";

        public string TryRegisterUser(string user, string pass, string email)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    // SQL corregido: usamos 'Password'
                    string sql = "INSERT INTO usuarios (NombreUsuario, Password, email, rol, estado) VALUES (@u, @p, @e, 'Nominal', 'Activo')";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", user);
                        cmd.Parameters.AddWithValue("@p", pass);
                        cmd.Parameters.AddWithValue("@e", email);
                        cmd.ExecuteNonQuery();
                    }
                    return null;
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062) return "El usuario ya existe.";
                return "Error: " + ex.Message;
            }
        }
    }
}