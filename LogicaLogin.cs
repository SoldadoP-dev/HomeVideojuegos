using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;

// El namespace debe coincidir con el definido en tu archivo XAML
namespace HomeVideojuegos
{
    // Es crucial que esta clase sea 'partial' y se llame 'LoginWindow'
    // para que se vincule con el archivo LoginWindow.xaml.
    public partial class LoginWindow : Window
    {
        // 1. DICCIONARIO DE USUARIOS VÁLIDOS (Usuario, Contraseña)
        private readonly Dictionary<string, string> validUsers = new Dictionary<string, string>
        {
            {"admin", "admin1234"}, // Usuario admin con la contraseña requerida
            {"test", "pass"},
            // Añade aquí cualquier otro usuario: {"usuario", "contraseña"}
        };

        // Constructor necesario para inicializar la ventana y sus componentes XAML.
        public LoginWindow()
        {
            InitializeComponent();

            // Asegurarse de que el campo de error (TxtError) esté limpio al inicio
            if (TxtError != null)
            {
                TxtError.Text = string.Empty;
            }
        }

        // Manejador de evento vinculado al botón LOGIN en el XAML
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // 1. Obtener credenciales
            string user = TxtUser.Text.Trim();
            string pass = TxtPass.Password;

            // Limpiar mensaje de error previo antes de la validación
            TxtError.Text = string.Empty;

            // 2. VALIDACIÓN DE CAMPOS VACÍOS
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                TxtError.Text = "Error: Debe introducir Usuario y Contraseña.";
                TxtError.Foreground = Brushes.Red;
                return;
            }

            // 3. VALIDACIÓN USANDO EL DICCIONARIO
            // Verifica si el usuario existe Y si la contraseña coincide
            if (validUsers.TryGetValue(user, out string storedPassword) && storedPassword == pass)
            {
                // Inicia sesión exitosa
                TxtError.Text = $"¡Bienvenido, {user}! Acceso concedido.";
                TxtError.Foreground = Brushes.LightGreen;

                // *** LÓGICA DE NAVEGACIÓN EXITOSA (CORREGIDA) ***
                // Abre la ventana principal (MainWindow)
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close(); // Cierra la ventana de login
            }
            else
            {
                // Credenciales incorrectas
                TxtError.Text = "Error: Usuario o Contraseña incorrectos.";
                TxtError.Foreground = Brushes.Red;
            }
        }

        // Manejador de evento vinculado al botón NUEVO en el XAML
        private void NewUser_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Funcionalidad de registro de nuevo usuario aún no implementada.", "Nuevo Usuario", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Manejador de evento vinculado al botón SALIR en el XAML
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}