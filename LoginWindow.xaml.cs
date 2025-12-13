using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace HomeVideojuegos
{
    public partial class LoginWindow : Window
    {
        // 1. Instancia de la clase de lógica centralizada (usa MySQL a través de DataContext)
        private readonly LogicalLogin _loginLogic = new LogicalLogin();

        // --- Variables de Control de Bloqueo ---
        private const int MaxFailedAttempts = 3;
        private const int BlockTimeSeconds = 50;
        private int failedAttempts = 0;
        private DispatcherTimer blockTimer;
        private DateTime blockEnds;

        public LoginWindow()
        {
            InitializeComponent();
            InitializeBlockTimer();

            // Inicialización de la Interfaz de Usuario
            if (TxtError != null)
            {
                TxtError.Text = string.Empty;
            }
            if (this.FindName("TxtVisiblePass") is TextBox visiblePass)
            {
                visiblePass.Visibility = Visibility.Collapsed;
            }
        }

        // ---------------------------------------------------------------------
        // MANEJADORES DE EVENTOS Y LÓGICA DE LOGIN
        // ---------------------------------------------------------------------

        private void InitializeBlockTimer()
        {
            blockTimer = new DispatcherTimer();
            blockTimer.Interval = TimeSpan.FromSeconds(1);
            blockTimer.Tick += BlockTimer_Tick;
        }

        // --- Manejador de evento vinculado al botón LOGIN ---
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (blockTimer.IsEnabled)
            {
                TimeSpan remaining = blockEnds - DateTime.Now;
                TxtError.Foreground = Brushes.Red;
                TxtError.Text = $"🚫 Cuenta bloqueada. Intente de nuevo en {Math.Ceiling(remaining.TotalSeconds)} segundos.";
                return;
            }

            string user = TxtUser.Text.Trim();
            string pass = TxtPass.Password;

            // Lógica para capturar la contraseña visible si está activa
            if (this.FindName("TxtVisiblePass") is TextBox visiblePass && visiblePass.Visibility == Visibility.Visible)
            {
                pass = visiblePass.Text;
            }

            TxtError.Text = string.Empty;

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                TxtError.Text = "Error: Debe introducir Usuario y Contraseña.";
                TxtError.Foreground = Brushes.Red;
                return;
            }

            // 2. Uso de _loginLogic para la validación
            if (_loginLogic.ValidUsers.TryGetValue(user, out string storedPassword) && storedPassword == pass)
            {
                failedAttempts = 0;
                TxtError.Text = $"¡Bienvenido, {user}! Acceso concedido.";
                TxtError.Foreground = Brushes.LightGreen;

                // Redirección a la ventana principal
                MainWindow mainWindow = new MainWindow(user);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                failedAttempts++;
                string errorMessage;

                if (_loginLogic.ValidUsers.ContainsKey(user))
                {
                    errorMessage = "Error: La contraseña es incorrecta.";
                }
                else
                {
                    errorMessage = $"Error: El usuario '{user}' no está registrado.";
                }

                if (failedAttempts >= MaxFailedAttempts)
                {
                    StartBlockoutTimer();
                }
                else
                {
                    TxtError.Text = $"{errorMessage} Intentos restantes: {MaxFailedAttempts - failedAttempts}";
                    TxtError.Foreground = Brushes.Red;
                }
            }
        }


        // --- Abre la ventana de Registro ---
        private void NewUser_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            // Pasamos la referencia de esta ventana y la instancia de la Lógica central
            RegistroWindow registroWindow = new RegistroWindow(this, _loginLogic);

            registroWindow.Show();
        }

        // --- Manejador de evento vinculado al botón SALIR ---
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // ---------------------------------------------------------------------
        // LÓGICA DE VISIBILIDAD DE CONTRASEÑA (RESUELVE CS1061)
        // ---------------------------------------------------------------------

        // Evento que se dispara al cambiar el texto en el PasswordBox (TxtPass)
        private void TxtPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Resuelve CS1061 de TxtPass_PasswordChanged
            // Solo actualiza si el TextBox visible está oculto
            if (this.FindName("TxtVisiblePass") is TextBox TxtVisiblePass && TxtVisiblePass.Visibility == Visibility.Collapsed)
            {
                TxtVisiblePass.Text = TxtPass.Password;
            }
        }

        // Evento que se dispara al cambiar el texto en el TextBox visible (TxtVisiblePass)
        private void TxtVisiblePass_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Resuelve CS1061 de TxtVisiblePass_TextChanged
            // Solo actualiza si el PasswordBox está oculto
            if (TxtPass.Visibility == Visibility.Collapsed)
            {
                TxtPass.Password = ((TextBox)sender).Text;
            }
        }

        // Manejador del botón de alternar visibilidad
        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            // Resuelve CS1061 de TogglePasswordVisibility_Click
            if (this.FindName("TxtVisiblePass") is TextBox TxtVisiblePass)
            {
                if (TxtPass.Visibility == Visibility.Visible)
                {
                    // Ocultar PasswordBox, mostrar TextBox
                    TxtVisiblePass.Text = TxtPass.Password;
                    TxtVisiblePass.Visibility = Visibility.Visible;
                    TxtPass.Visibility = Visibility.Collapsed;
                    TxtVisiblePass.Focus();
                }
                else
                {
                    // Ocultar TextBox, mostrar PasswordBox
                    TxtPass.Password = TxtVisiblePass.Text;
                    TxtPass.Visibility = Visibility.Visible;
                    TxtVisiblePass.Visibility = Visibility.Collapsed;
                    TxtPass.Focus();
                }
            }
        }

        // ---------------------------------------------------------------------
        // LÓGICA DE BLOQUEO DE SEGURIDAD (RESUELVE CS0103)
        // ---------------------------------------------------------------------

        private void StartBlockoutTimer()
        {
            // Resuelve CS0103 de StartBlockoutTimer
            blockEnds = DateTime.Now.AddSeconds(BlockTimeSeconds);
            blockTimer.Start();
            TxtError.Text = $"🚨 Bloqueo de seguridad: {BlockTimeSeconds} segundos. Demasiados intentos fallidos.";
            TxtError.Foreground = Brushes.OrangeRed;

            // Deshabilitar campos y botones
            TxtUser.IsEnabled = false;
            TxtPass.IsEnabled = false;
            if (this.FindName("TxtVisiblePass") is TextBox visiblePass)
            {
                visiblePass.IsEnabled = false;
            }
            if (this.FindName("TogglePassButton") is Button toggleButton)
            {
                toggleButton.IsEnabled = false;
            }
        }

        private void BlockTimer_Tick(object sender, EventArgs e)
        {
            // Resuelve CS0103 de BlockTimer_Tick
            TimeSpan remaining = blockEnds - DateTime.Now;
            if (remaining.TotalSeconds <= 0)
            {
                // Fin del bloqueo
                blockTimer.Stop();
                failedAttempts = 0;
                TxtError.Text = "🔓 Bloqueo finalizado. Puede intentar iniciar sesión.";
                TxtError.Foreground = Brushes.LightGreen;

                // Habilitar campos y botones
                TxtUser.IsEnabled = true;
                TxtPass.IsEnabled = true;
                if (this.FindName("TxtVisiblePass") is TextBox visiblePass)
                {
                    visiblePass.IsEnabled = true;
                }
                if (this.FindName("TogglePassButton") is Button toggleButton)
                {
                    toggleButton.IsEnabled = true;
                }
            }
            else
            {
                // Actualizar contador
                TxtError.Text = $"🚫 Cuenta bloqueada. Intente de nuevo en {Math.Ceiling(remaining.TotalSeconds)} segundos.";
            }
        }
    }
}