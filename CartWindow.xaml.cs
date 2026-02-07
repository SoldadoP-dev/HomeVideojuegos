using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace HomeVideojuegos
{
    public partial class CartWindow : Window
    {
        private dynamic _usuario;
        private string connStr = "Server=localhost;Port=3306;Database=home_videojuegos_db;Uid=root;Pwd=301206;";

        public CartWindow(dynamic usuario)
        {
            InitializeComponent();
            this._usuario = usuario;
            CargarCarrito();
        }

        private void CargarCarrito()
        {
            lbCarrito.Items.Clear();
            decimal total = 0;
            object userId = (_usuario is string) ? _usuario : _usuario.Id;

            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    // Añadimos imagen_blob a la consulta
                    string sql = @"SELECT v.id_juego, v.titulo, v.precio, v.imagen_blob 
                                   FROM carrito c 
                                   JOIN videojuegos v ON c.id_juego = v.id_juego 
                                   WHERE c.id_usuario = @uid";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            BitmapImage img = null;
                            if (rdr["imagen_blob"] != DBNull.Value)
                            {
                                byte[] data = (byte[])rdr["imagen_blob"];
                                using (var ms = new MemoryStream(data))
                                {
                                    img = new BitmapImage();
                                    img.BeginInit();
                                    img.StreamSource = ms;
                                    img.CacheOption = BitmapCacheOption.OnLoad;
                                    img.EndInit();
                                }
                            }

                            lbCarrito.Items.Add(new
                            {
                                id_juego = rdr["id_juego"],
                                titulo = rdr["titulo"].ToString(),
                                precio = Convert.ToDecimal(rdr["precio"]),
                                imagen = img
                            });
                            total += Convert.ToDecimal(rdr["precio"]);
                        }
                    }
                }
                lblTotal.Text = $"{total:N2} €";
            }
            catch (Exception ex) { MessageBox.Show("Error al cargar carrito: " + ex.Message); }
        }

        private void EliminarDelCarrito_Click(object sender, RoutedEventArgs e)
        {
            // Como usamos un botón dentro de un DataTemplate, usamos el Tag para saber qué ID borrar
            Button btn = (Button)sender;
            object idJuego = btn.Tag;
            object userId = (_usuario is string) ? _usuario : _usuario.Id;

            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string sql = "DELETE FROM carrito WHERE id_usuario = @uid AND id_juego = @jid LIMIT 1";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@jid", idJuego);
                    cmd.ExecuteNonQuery();
                }
                CargarCarrito(); // Refrescar
            }
            catch (Exception ex) { MessageBox.Show("Error al eliminar: " + ex.Message); }
        }

        private void Pagar_Click(object sender, RoutedEventArgs e) => MessageBox.Show("¡Gracias por tu compra!");

        private void Volver_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left) DragMove();
        }
    }
}