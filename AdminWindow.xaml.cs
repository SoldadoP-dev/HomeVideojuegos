using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using MySql.Data.MySqlClient;

namespace HomeVideojuegos
{
    public partial class AdminWindow : Window
    {
        private string connStr = "Server=localhost;Port=3306;Database=home_videojuegos_db;Uid=root;Pwd=301206;";
        private byte[] _imagenBytes;
        private int _idJuegoSel = -1;
        private int _idUserSel = -1;

        public AdminWindow()
        {
            InitializeComponent();
            CargarTablaJuegos();
            CargarTablaUsuarios();
        }

        // --- GESTIÓN DE JUEGOS ---
        private void CargarTablaJuegos()
        {
            var lista = new ObservableCollection<dynamic>();
            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT * FROM videojuegos", conn);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lista.Add(new
                            {
                                Id = rdr["id_juego"],
                                Title = rdr["titulo"],
                                Anio = rdr["anio"],
                                Price = rdr["precio"],
                                Fabricante = rdr["fabricante"],
                                Desc = rdr["descripcion"],
                                Img = rdr["imagen_blob"]
                            });
                        }
                    }
                }
                DgJuegos.ItemsSource = lista;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void DgJuegos_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DgJuegos.SelectedItem == null) return;
            dynamic sel = DgJuegos.SelectedItem;
            _idJuegoSel = (int)sel.Id;
            TxtTitulo.Text = sel.Title;
            TxtAnio.Text = sel.Anio.ToString();
            TxtPrecio.Text = sel.Price.ToString();
            TxtFabricante.Text = sel.Fabricante;
            TxtDescripcion.Text = sel.Desc;
            if (!Convert.IsDBNull(sel.Img))
            {
                _imagenBytes = (byte[])sel.Img;
                ImgPreview.Source = BytesToImg(_imagenBytes);
            }
            else { ImgPreview.Source = null; _imagenBytes = null; }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string sql = _idJuegoSel == -1
                        ? "INSERT INTO videojuegos (titulo, anio, precio, fabricante, descripcion, imagen_blob) VALUES (@t,@a,@p,@f,@d,@i)"
                        : "UPDATE videojuegos SET titulo=@t, anio=@a, precio=@p, fabricante=@f, descripcion=@d, imagen_blob=@i WHERE id_juego=@id";

                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@t", TxtTitulo.Text);
                    cmd.Parameters.AddWithValue("@a", TxtAnio.Text);
                    cmd.Parameters.AddWithValue("@p", TxtPrecio.Text.Replace(",", "."));
                    cmd.Parameters.AddWithValue("@f", TxtFabricante.Text);
                    cmd.Parameters.AddWithValue("@d", TxtDescripcion.Text);
                    cmd.Parameters.AddWithValue("@i", (object)_imagenBytes ?? DBNull.Value);
                    if (_idJuegoSel != -1) cmd.Parameters.AddWithValue("@id", _idJuegoSel);
                    cmd.ExecuteNonQuery();
                }
                CargarTablaJuegos();
                LimpiarJuego();
                MessageBox.Show("Juego guardado correctamente");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e) => LimpiarJuego();
        private void LimpiarJuego()
        {
            _idJuegoSel = -1; TxtTitulo.Clear(); TxtAnio.Clear(); TxtPrecio.Clear();
            TxtFabricante.Clear(); TxtDescripcion.Clear(); ImgPreview.Source = null; _imagenBytes = null;
        }

        // --- GESTIÓN DE USUARIOS (ACTUALIZADO) ---
        private void CargarTablaUsuarios()
        {
            var lista = new ObservableCollection<dynamic>();
            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT id, NombreUsuario, email, rol, estado FROM usuarios", conn);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lista.Add(new { id = rdr["id"], NombreUsuario = rdr["NombreUsuario"], email = rdr["email"], rol = rdr["rol"], estado = rdr["estado"] });
                        }
                    }
                }
                DgUsuarios.ItemsSource = lista;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void DgUsuarios_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DgUsuarios.SelectedItem == null) return;
            dynamic sel = DgUsuarios.SelectedItem;
            _idUserSel = (int)sel.id;
            TxtUserNombre.Text = sel.NombreUsuario;
            TxtUserEmail.Text = sel.email;
            CbUserRol.Text = sel.rol;
            CbUserEstado.Text = sel.estado;
            TxtUserPass.Clear(); // Por seguridad, no mostramos la contraseña actual
        }

        private void BtnActualizarUsuario_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string pass = TxtUserPass.Password;

                    // IMPORTANTE: Se usa 'Password' para coincidir con el nombre de columna SQL estándar
                    string sql = _idUserSel == -1
                        ? "INSERT INTO usuarios (NombreUsuario, email, rol, estado, Password) VALUES (@n,@e,@r,@s,@p)"
                        : "UPDATE usuarios SET NombreUsuario=@n, email=@e, rol=@r, estado=@s" +
                          (!string.IsNullOrEmpty(pass) ? ", Password=@p " : " ") + "WHERE id=@id";

                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@n", TxtUserNombre.Text);
                    cmd.Parameters.AddWithValue("@e", TxtUserEmail.Text);
                    cmd.Parameters.AddWithValue("@r", CbUserRol.Text);
                    cmd.Parameters.AddWithValue("@s", CbUserEstado.Text);

                    if (!string.IsNullOrEmpty(pass) || _idUserSel == -1)
                        cmd.Parameters.AddWithValue("@p", pass);

                    if (_idUserSel != -1) cmd.Parameters.AddWithValue("@id", _idUserSel);
                    cmd.ExecuteNonQuery();
                }
                CargarTablaUsuarios();
                LimpiarUser();
                MessageBox.Show("Usuario procesado correctamente");
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void BtnLimpiarUser_Click(object sender, RoutedEventArgs e) => LimpiarUser();
        private void LimpiarUser()
        {
            _idUserSel = -1; TxtUserNombre.Clear(); TxtUserEmail.Clear(); TxtUserPass.Clear();
            CbUserRol.SelectedIndex = -1; CbUserEstado.SelectedIndex = -1;
        }

        // --- APOYO ---
        private void Logout_Click(object sender, RoutedEventArgs e) { new LoginWindow().Show(); this.Close(); }

        private void BtnSeleccionarImagen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            if (op.ShowDialog() == true)
            {
                _imagenBytes = File.ReadAllBytes(op.FileName);
                ImgPreview.Source = new BitmapImage(new Uri(op.FileName));
            }
        }

        private BitmapImage BytesToImg(byte[] b)
        {
            if (b == null) return null;
            var img = new BitmapImage();
            using (var ms = new MemoryStream(b))
            {
                img.BeginInit(); img.CacheOption = BitmapCacheOption.OnLoad; img.StreamSource = ms; img.EndInit();
            }
            return img;
        }
    }
}