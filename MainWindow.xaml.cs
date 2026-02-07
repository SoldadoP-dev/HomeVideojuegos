using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;

namespace HomeVideojuegos
{
    public partial class MainWindow : Window
    {
        private string connStr = "Server=localhost;Port=3306;Database=home_videojuegos_db;Uid=root;Pwd=301206;";
        public dynamic UsuarioActual { get; set; }

        public MainWindow(dynamic usuario)
        {
            InitializeComponent();
            this.UsuarioActual = usuario;

            string nombre = (usuario is string) ? usuario : usuario.NombreUsuario;
            TxtBienvenida.Text = $"Hola, {nombre}";

            CargarJuegosHome();
        }

        private void CargarJuegosHome(string filtro = "")
        {
            ContenedorJuegos.Children.Clear();
            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    // 1. MEJORA: Añadimos 'descripcion' y 'fabricante' a la consulta SQL
                    string sql = "SELECT id_juego, titulo, precio, imagen_blob, fabricante, descripcion FROM videojuegos WHERE visible_home = 1";

                    if (!string.IsNullOrEmpty(filtro))
                        sql += " AND titulo LIKE @filtro";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    if (!string.IsNullOrEmpty(filtro))
                        cmd.Parameters.AddWithValue("@filtro", "%" + filtro + "%");

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            // 2. CORRECCIÓN CLAVE: El objeto debe tener TODAS las propiedades que pide la ventana de detalles
                            var juego = new
                            {
                                Id = rdr["id_juego"],
                                Title = rdr["titulo"].ToString(),
                                Price = Convert.ToDecimal(rdr["precio"]),
                                Img = rdr["imagen_blob"],
                                // Aseguramos que Fabricante y Descripcion existan en el objeto
                                Fabricante = rdr["fabricante"] != DBNull.Value ? rdr["fabricante"].ToString() : "Editor Independiente",
                                Descripcion = rdr["descripcion"] != DBNull.Value ? rdr["descripcion"].ToString() : "Sin descripción disponible."
                            };
                            CrearTarjetaJuego(juego);
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error en la carga: " + ex.Message); }
        }

        private void CrearTarjetaJuego(dynamic juego)
        {
            // Contenedor con animación
            Border card = new Border
            {
                Height = 270,
                Margin = new Thickness(12),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#161A24")),
                CornerRadius = new CornerRadius(15),
                ClipToBounds = true,
                Style = (Style)this.Resources["GameCardStyle"]
            };

            // Glow al pasar el ratón
            DropShadowEffect shadow = new DropShadowEffect { BlurRadius = 20, Opacity = 0.1, Color = Colors.Black, ShadowDepth = 0 };
            card.Effect = shadow;

            card.MouseEnter += (s, e) => { shadow.Color = (Color)ColorConverter.ConvertFromString("#00D1D1"); shadow.Opacity = 0.3; };
            card.MouseLeave += (s, e) => { shadow.Color = Colors.Black; shadow.Opacity = 0.1; };

            // Evento Click: Ahora 'juego' tiene todas las propiedades necesarias
            card.MouseLeftButtonUp += (s, e) => {
                try
                {
                    new DetalleJuegoWindow(juego, UsuarioActual).ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al abrir detalles: " + ex.Message);
                }
            };

            StackPanel stack = new StackPanel();

            // Imagen
            Grid imgArea = new Grid { Height = 180, Background = Brushes.Black };
            Image img = new Image { Stretch = Stretch.UniformToFill };

            if (juego.Img != null && !Convert.IsDBNull(juego.Img))
            {
                using (var ms = new MemoryStream((byte[])juego.Img))
                {
                    var bmp = new BitmapImage();
                    bmp.BeginInit(); bmp.StreamSource = ms; bmp.CacheOption = BitmapCacheOption.OnLoad; bmp.EndInit();
                    img.Source = bmp;
                }
            }
            imgArea.Children.Add(img);

            // Información (Título y Precio)
            Border infoBorder = new Border { Padding = new Thickness(15, 12, 15, 12) };
            StackPanel txtStack = new StackPanel();

            txtStack.Children.Add(new TextBlock
            {
                Text = juego.Title.ToUpper(),
                Foreground = Brushes.White,
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                TextTrimming = TextTrimming.CharacterEllipsis
            });

            txtStack.Children.Add(new TextBlock
            {
                Text = $"{juego.Price:N2} €",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00D1D1")),
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 5, 0, 0)
            });

            infoBorder.Child = txtStack;
            stack.Children.Add(imgArea);
            stack.Children.Add(infoBorder);
            card.Child = stack;

            ContenedorJuegos.Children.Add(card);
        }

        // --- Navegación y Eventos ---
        private void TxtBusqueda_TextChanged(object sender, TextChangedEventArgs e) => CargarJuegosHome(TxtBusqueda.Text);
        private void BtnExplorar_Click(object sender, RoutedEventArgs e) { TxtBusqueda.Text = ""; CargarJuegosHome(); }
        private void BtnBiblioteca_Click(object sender, RoutedEventArgs e) => new LibraryWindow(UsuarioActual).ShowDialog();
        private void BtnCarrito_Click(object sender, RoutedEventArgs e) => new CartWindow(UsuarioActual).ShowDialog();
        private void BtnWishlist_Click(object sender, RoutedEventArgs e) => new WishlistWindow(UsuarioActual).ShowDialog();
        private void Logout_Click(object sender, RoutedEventArgs e) { new LoginWindow().Show(); this.Close(); }
    }
}