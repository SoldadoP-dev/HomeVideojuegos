using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using MySql.Data.MySqlClient;

namespace HomeVideojuegos
{
    public partial class DetalleJuegoWindow : Window
    {
        private dynamic _juego;
        private dynamic _usuario;
        private string connStr = "Server=localhost;Port=3306;Database=home_videojuegos_db;Uid=root;Pwd=301206;";

        public DetalleJuegoWindow(dynamic juegoSeleccionado, dynamic usuarioActual)
        {
            InitializeComponent();
            this._juego = juegoSeleccionado;
            this._usuario = usuarioActual;
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                LblTitulo.Text = _juego.Title;
                LblFabricante.Text = _juego.Fabricante;
                LblDescripcion.Text = _juego.Descripcion; // Sincronizado
                LblPrecio.Text = $"{_juego.Price:N2} €";

                if (_juego.Img != null && !Convert.IsDBNull(_juego.Img))
                {
                    using (var ms = new MemoryStream((byte[])_juego.Img))
                    {
                        var img = new BitmapImage();
                        img.BeginInit(); img.StreamSource = ms; img.CacheOption = BitmapCacheOption.OnLoad; img.EndInit();
                        ImgDetalle.Source = img;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void BtnComprar_Click(object sender, RoutedEventArgs e) => EjecutarInsercion("biblioteca");
        private void BtnCarrito_Click(object sender, RoutedEventArgs e) => EjecutarInsercion("carrito");
        private void BtnWishlist_Click(object sender, RoutedEventArgs e) => EjecutarInsercion("wishlist");
        private void BtnCerrar_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) { if (e.ChangedButton == System.Windows.Input.MouseButton.Left) DragMove(); }

        private void EjecutarInsercion(string tabla)
        {
            try
            {
                if (_usuario == null) { MessageBox.Show("Inicia sesión."); return; }
                object userId = (_usuario is string) ? _usuario : _usuario.Id;

                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string sql = $"INSERT INTO {tabla} (id_usuario, id_juego) VALUES (@uid, @jid)";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@jid", _juego.Id);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show(tabla == "biblioteca" ? "¡Comprado!" : "Añadido.");
                }
            }
            catch { MessageBox.Show("Ya lo tienes en tu lista."); }
        }
    }
}