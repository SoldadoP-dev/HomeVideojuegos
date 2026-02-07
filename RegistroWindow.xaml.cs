using System;
using System.Windows;

namespace HomeVideojuegos
{
    public partial class RegistroWindow : Window
    {
        private Window _parent;
        private LogicalLogin _loginLogic;

        public RegistroWindow(Window parent, LogicalLogin loginLogic)
        {
            InitializeComponent();
            _parent = parent;
            _loginLogic = loginLogic;
        }

        private void Registrar_Click(object sender, RoutedEventArgs e)
        {
            string user = TxtRegistroUser.Text.Trim();
            string email = TxtRegistroEmail.Text.Trim();
            string pass = TxtRegistroPass.Password;
            string confirm = TxtRegistroPassConfirm.Password;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(email))
            {
                TxtRegistroError.Text = "Todos los campos son obligatorios.";
                return;
            }

            if (pass != confirm)
            {
                TxtRegistroError.Text = "Las contraseñas no coinciden.";
                return;
            }

            string error = _loginLogic.TryRegisterUser(user, pass, email);

            if (error == null)
            {
                MessageBox.Show("¡Cuenta creada con éxito!", "Registro", MessageBoxButton.OK, MessageBoxImage.Information);
                VolverAlLogin();
            }
            else
            {
                TxtRegistroError.Text = error;
            }
        }

        private void Volver_Click(object sender, RoutedEventArgs e) => VolverAlLogin();
        private void VolverAlLogin() { _parent.Show(); this.Close(); }
    }
}