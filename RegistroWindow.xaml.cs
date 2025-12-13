using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HomeVideojuegos
{
    public partial class RegistroWindow : Window
    {
        private readonly LoginWindow _loginWindow;
        private readonly LogicalLogin _loginLogic;

        /// <summary>
        /// Constructor que recibe la ventana de Login y la clase de Lógica.
        /// </summary>
        public RegistroWindow(LoginWindow loginWindow, LogicalLogin loginLogic)
        {
            InitializeComponent();
            _loginWindow = loginWindow;
            _loginLogic = loginLogic; // Referencia a la lógica central
        }

        // Manejador del botón REGISTRAR
        private void Registrar_Click(object sender, RoutedEventArgs e)
        {
            string newUser = TxtRegistroUser.Text.Trim();
            string newPass = TxtRegistroPass.Password;
            string confirmPass = TxtRegistroPassConfirm.Password;

            TxtRegistroError.Text = string.Empty;

            // Delegamos toda la validación y guardado a LogicalLogin
            string errorMessage = _loginLogic.TryRegisterUser(newUser, newPass, confirmPass);

            if (errorMessage != null)
            {
                // Si hay un error, mostramos el mensaje devuelto por la lógica
                TxtRegistroError.Text = errorMessage;
                TxtRegistroError.Foreground = Brushes.Red;
                return;
            }

            // Registro exitoso
            MessageBox.Show($"¡Usuario '{newUser}' creado exitosamente! Ahora puede iniciar sesión.",
                            "Registro Completado", MessageBoxButton.OK, MessageBoxImage.Information);

            // Regresar a la ventana de Login
            if (_loginWindow.FindName("TxtUser") is TextBox loginUserTextBox)
            {
                loginUserTextBox.Text = newUser;
            }

            _loginWindow.Show();
            this.Close();
        }

        // Manejador del botón VOLVER A LOGIN
        private void Volver_Click(object sender, RoutedEventArgs e)
        {
            _loginWindow.Show();
            this.Close();
        }
    }
}