using System.Windows;

namespace HomeVideojuegos
{
    /// <summary>
    /// Lógica de interacción para App.xaml.
    /// Esta clase define la configuración de inicio de la aplicación.
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Mostrar la ventana de Login al inicio
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}