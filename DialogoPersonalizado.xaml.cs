using System.Windows;
using System.Windows.Media.Effects;

namespace HomeVideojuegos
{
    public partial class DialogoPersonalizado : Window
    {
        public DialogoPersonalizado(string mensaje)
        {
            InitializeComponent();
            TxtMensaje.Text = mensaje;
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        // El método DEBE verse así para aceptar los dos argumentos (mensaje y this)
        public static void Mostrar(string mensaje, Window parent = null)
        {
            var ventana = new DialogoPersonalizado(mensaje);

            if (parent != null)
            {
                ventana.Owner = parent; // Centra el mensaje sobre la ventana principal
                parent.Effect = new BlurEffect { Radius = 8 }; // Aplica el desenfoque
            }

            ventana.ShowDialog();

            if (parent != null)
            {
                parent.Effect = null; // Quita el desenfoque al cerrar
            }
        }
    }
}