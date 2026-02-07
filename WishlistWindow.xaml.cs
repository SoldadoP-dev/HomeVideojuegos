using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MySql.Data.MySqlClient;

namespace HomeVideojuegos
{
    public partial class WishlistWindow : Window
    {
        private dynamic _usuario;
        private string connStr = "Server=localhost;Port=3306;Database=home_videojuegos_db;Uid=root;Pwd=301206;";

        public WishlistWindow(dynamic usuario)
        {
            InitializeComponent();
            this._usuario = usuario;
            CargarWishlist();
        }

        private void CargarWishlist()
        {
            try
            {
                object userId = (_usuario is string) ? _usuario : _usuario.Id;

                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    // Seleccionamos id, titulo, precio e imagen_blob de la tabla videojuegos
                    string sql = @"SELECT v.id_juego, v.titulo, v.precio, v.imagen_blob 
                                   FROM wishlist w 
                                   JOIN videojuegos v ON w.id_juego = v.id_juego 
                                   WHERE w.id_usuario = @uid";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    var lista = new List<object>();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            BitmapImage imgSource = null;
                            if (rdr["imagen_blob"] != DBNull.Value)
                            {
                                byte[] binaryData = (byte[])rdr["imagen_blob"];
                                imgSource = new BitmapImage();
                                imgSource.BeginInit();
                                imgSource.StreamSource = new MemoryStream(binaryData);
                                imgSource.CacheOption = BitmapCacheOption.OnLoad;
                                imgSource.EndInit();
                            }

                            lista.Add(new
                            {
                                id_juego = rdr["id_juego"],
                                titulo = rdr["titulo"].ToString(),
                                precio_formateado = $"{rdr["precio"]:N2} €",
                                imagen = imgSource
                            });
                        }
                    }
                    icWishlist.ItemsSource = lista;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la lista de deseos: " + ex.Message);
            }
        }

        private void RemoveWishlist_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as System.Windows.Controls.Button;
            var idJuego = btn.Tag;
            object userId = (_usuario is string) ? _usuario : _usuario.Id;

            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string sql = "DELETE FROM wishlist WHERE id_usuario = @uid AND id_juego = @jid";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@jid", idJuego);
                    cmd.ExecuteNonQuery();
                }
                CargarWishlist(); // Recarga visualmente la lista
            }
            catch (Exception ex) { MessageBox.Show("Error al eliminar: " + ex.Message); }
        }

        private void AddFromWishlist_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as System.Windows.Controls.Button;
            var idJuego = btn.Tag;
            object userId = (_usuario is string) ? _usuario : _usuario.Id;

            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    // 1. Insertar en carrito (IGNORAR si ya existe)
                    string sqlCarrito = "INSERT IGNORE INTO carrito (id_usuario, id_juego) VALUES (@uid, @jid)";
                    MySqlCommand cmdCarrito = new MySqlCommand(sqlCarrito, conn);
                    cmdCarrito.Parameters.AddWithValue("@uid", userId);
                    cmdCarrito.Parameters.AddWithValue("@jid", idJuego);
                    cmdCarrito.ExecuteNonQuery();

                    // 2. Eliminar de la lista de deseos inmediatamente
                    string sqlWishlist = "DELETE FROM wishlist WHERE id_usuario = @uid AND id_juego = @jid";
                    MySqlCommand cmdWishlist = new MySqlCommand(sqlWishlist, conn);
                    cmdWishlist.Parameters.AddWithValue("@uid", userId);
                    cmdWishlist.Parameters.AddWithValue("@jid", idJuego);
                    cmdWishlist.ExecuteNonQuery();

                    MessageBox.Show("¡Movido al carrito exitosamente!", "Tienda", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                CargarWishlist(); // Actualizar interfaz
            }
            catch (Exception ex) { MessageBox.Show("Error al procesar: " + ex.Message); }
        }

        private void Volver_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }
    }
}