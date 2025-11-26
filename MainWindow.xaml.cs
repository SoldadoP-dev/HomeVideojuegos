using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HomeVideojuegos
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Inicializa los componentes definidos en MainWindow.xaml
            InitializeComponent();
        }

        // ===============================================
        // MANEJADORES DE EVENTOS PARA LA BARRA SUPERIOR (TOP NAV)
        // ===============================================

        private void TopNavButton_Click(object sender, RoutedEventArgs e)
        {
            // Este manejador de clics genérico se puede usar para los botones HOME, GAMES, DLC & ADD-ONS, etc.
            if (sender is Button button)
            {
                MessageBox.Show($"Navegando a: {button.Content}", "Top Navigation");
            }
        }

        // ===============================================
        // MANEJADORES DE EVENTOS PARA LA BARRA LATERAL (SIDEBAR)
        // ===============================================

        private void LibraryButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Abriendo la Biblioteca de Juegos", "Sidebar Navigation");
            // Aquí iría la lógica para cargar el UserControl de la Biblioteca en el área principal.
        }

        private void WishlistButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Mostrando la Lista de Deseados", "Sidebar Navigation");
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Revisando el Carrito de Compras", "Sidebar Navigation");
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Abriendo Configuración de la Aplicación", "Sidebar Navigation");
        }

        // ===============================================
        // MANEJADOR DE EVENTOS PARA EL BOTÓN DE ACCIÓN (CALL TO ACTION)
        // ===============================================

        private void ExploreAllGames_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Explorando todo el catálogo de juegos disponibles", "Call To Action");
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            // Aquí puedes agregar la lógica para cerrar sesión, por ejemplo:
            MessageBox.Show("Sesión cerrada correctamente.");
            // Opcional: cerrar la ventana o redirigir al login
            // this.Close();
        }
    }
}