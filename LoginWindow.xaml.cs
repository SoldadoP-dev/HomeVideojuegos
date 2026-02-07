using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using MySql.Data.MySqlClient;

namespace HomeVideojuegos
{
    public partial class LoginWindow : Window
    {
        private readonly LogicalLogin _loginLogic = new LogicalLogin();

        // Control de Bloqueo
        private const int MaxFailedAttempts = 3;
        private const int BlockTimeSeconds = 50;
        private int failedAttempts = 0;
        private DispatcherTimer blockTimer;
        private DateTime blockEnds;

        // Cadena de conexión
        private string connectionString = "Server=localhost;Database=home_videojuegos_db;Uid=root;Pwd=301206;";

        public LoginWindow()
        {
            InitializeComponent();
            InitializeBlockTimer();
            TxtError.Text = string.Empty;
        }

        private void InitializeBlockTimer()
        {
            blockTimer = new DispatcherTimer();
            blockTimer.Interval = TimeSpan.FromSeconds(1);
            blockTimer.Tick += BlockTimer_Tick;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (blockTimer.IsEnabled) return;

            string user = TxtUser.Text.Trim();
            string pass = (TxtVisiblePass.Visibility == Visibility.Visible) ? TxtVisiblePass.Text : TxtPass.Password;

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                TxtError.Text = "Rellene todos los campos.";
                return;
            }

            // Validamos contra la BBDD y obtenemos el rol en un solo paso
            string rolRecuperado = ValidarUsuarioYObtenerRol(user, pass);

            if (rolRecuperado != null)
            {
                failedAttempts = 0;

                if (rolRecuperado.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    new AdminWindow().Show();
                }
                else
                {
                    new MainWindow(user).Show();
                }
                this.Close();
            }
            else
            {
                failedAttempts++;
                ManejarErrorLogin();
            }
        }

        private string ValidarUsuarioYObtenerRol(string username, string password)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    // Buscamos que coincidan ambos campos
                    string query = "SELECT rol FROM usuarios WHERE NombreUsuario = @user AND Password = @pass";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", username);
                        cmd.Parameters.AddWithValue("@pass", password);
                        object result = cmd.ExecuteScalar();
                        return result?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message);
                return null;
            }
        }

        private void ManejarErrorLogin()
        {
            if (failedAttempts >= MaxFailedAttempts)
            {
                StartBlockoutTimer();
            }
            else
            {
                TxtError.Text = "Usuario o contraseña incorrectos.";
                TxtError.Foreground = Brushes.Red;
            }
        }

        private void StartBlockoutTimer()
        {
            blockEnds = DateTime.Now.AddSeconds(BlockTimeSeconds);
            blockTimer.Start();
            TxtUser.IsEnabled = TxtPass.IsEnabled = false;
        }

        private void BlockTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan remaining = blockEnds - DateTime.Now;
            if (remaining.TotalSeconds <= 0)
            {
                blockTimer.Stop();
                failedAttempts = 0;
                TxtUser.IsEnabled = TxtPass.IsEnabled = true;
                TxtError.Text = "🔓 Intente de nuevo.";
            }
            else
            {
                TxtError.Text = $"🚫 Bloqueado: {Math.Ceiling(remaining.TotalSeconds)}s.";
            }
        }

        // --- Eventos de UI requeridos por el XAML ---
        private void NewUser_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new RegistroWindow(this, _loginLogic).Show();
        }

        private void Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void TxtPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (TxtVisiblePass.Visibility == Visibility.Collapsed) TxtVisiblePass.Text = TxtPass.Password;
        }

        private void TxtVisiblePass_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtPass.Visibility == Visibility.Collapsed) TxtPass.Password = TxtVisiblePass.Text;
        }

        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            if (TxtPass.Visibility == Visibility.Visible)
            {
                TxtVisiblePass.Text = TxtPass.Password;
                TxtVisiblePass.Visibility = Visibility.Visible;
                TxtPass.Visibility = Visibility.Collapsed;
            }
            else
            {
                TxtPass.Password = TxtVisiblePass.Text;
                TxtPass.Visibility = Visibility.Visible;
                TxtVisiblePass.Visibility = Visibility.Collapsed;
            }
        }
    }
}