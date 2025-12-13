using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Windows; // Necesario para MessageBox

namespace HomeVideojuegos
{
    // Clase simple para contener el usuario (debe existir en el namespace)
    public class Usuario
    {
        public string NombreUsuario { get; set; }
        public string Contrasena { get; set; }
    }

    public class LogicalLogin
    {
        // 1. CADENA DE CONEXIÓN PURA (Usando SslMode=0 para compatibilidad máxima con MySql.Data)
        // Por favor, prueba ESTA cadena de conexión:
        private const string ConnectionString =
            "Server=localhost;Port=3306;Database=home_videojuegos_db;Uid=root;Pwd=301206;SslMode=0;";

        // Si la anterior con SslMode=0 falla, regresa a la versión None:
        // private const string ConnectionString =
        //    "Server=localhost;Port=3306;Database=home_videojuegos_db;Uid=root;Pwd=301206;SslMode=None;";


        public Dictionary<string, string> ValidUsers { get; private set; } = new Dictionary<string, string>();

        public LogicalLogin()
        {
            LoadUsersFromDatabase();
        }

        private void LoadUsersFromDatabase()
        {
            ValidUsers.Clear();

            // Usamos la conexión pura de MySql.Data
            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT NombreUsuario, Contrasena FROM usuarios";

                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string user = reader.GetString("NombreUsuario");
                            string pass = reader.GetString("Contrasena");
                            ValidUsers[user] = pass;
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    // Manejar errores específicos de la base de datos (Ej: Conexión denegada)
                    MessageBox.Show($"Error al conectar o leer la base de datos: {ex.Message}", "Error Crítico de BD");
                    // Opcional: Agregar usuarios de prueba si falla la conexión
                    // ValidUsers["admin"] = "admin1234";
                }
                catch (System.Exception ex)
                {
                    // Manejar otros errores
                    MessageBox.Show($"Error general al cargar usuarios: {ex.Message}", "Error");
                }
            }
        }

        /// <summary>
        /// Intenta registrar un nuevo usuario en la base de datos.
        /// Retorna null si es exitoso, o un mensaje de error si falla.
        /// </summary>
        public string TryRegisterUser(string newUser, string newPass, string confirmPass)
        {
            // --- 1. Validaciones en memoria ---
            if (string.IsNullOrWhiteSpace(newUser) || string.IsNullOrWhiteSpace(newPass) || string.IsNullOrWhiteSpace(confirmPass))
            {
                return "Todos los campos son obligatorios.";
            }

            if (newPass != confirmPass)
            {
                return "Las contraseñas no coinciden.";
            }

            if (ValidUsers.ContainsKey(newUser))
            {
                return $"El usuario '{newUser}' ya está registrado.";
            }

            // --- 2. Inserción en la base de datos ---
            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO usuarios (NombreUsuario, Contrasena) VALUES (@User, @Pass)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@User", newUser);
                        command.Parameters.AddWithValue("@Pass", newPass);
                        command.ExecuteNonQuery();
                    }

                    // Actualizar el diccionario en memoria después del éxito en BD
                    ValidUsers[newUser] = newPass;
                    return null; // Registro exitoso
                }
                catch (MySqlException ex)
                {
                    // Manejar errores de SQL durante la inserción
                    return $"Error al insertar el usuario en BD: {ex.Message}";
                }
            }
        }
    }
}