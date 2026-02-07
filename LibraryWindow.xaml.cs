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
    public partial class LibraryWindow : Window
    {
        private string connStr = "Server=localhost;Port=3306;Database=home_videojuegos_db;Uid=root;Pwd=301206;";
        private dynamic _usuario;

        public LibraryWindow(dynamic usuarioActual)
        {
            InitializeComponent();
            this._usuario = usuarioActual;
            CargarBiblioteca();
        }

        private void CargarBiblioteca()
        {
            ContenedorBiblioteca.Children.Clear();
            object userId = (_usuario is string) ? _usuario : _usuario.Id;

            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string sql = @"SELECT v.* FROM videojuegos v 
                                 INNER JOIN biblioteca b ON v.id_juego = b.id_juego 
                                 WHERE b.id_usuario = @uid";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var juego = new
                            {
                                Title = rdr["titulo"].ToString(),
                                Img = rdr["imagen_blob"]
                            };
                            CrearTarjetaBiblioteca(juego);
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error en biblioteca: " + ex.Message); }
        }

        private void CrearTarjetaBiblioteca(dynamic juego)
        {
            // Usamos un Width fijo para asegurar el centrado en el UniformGrid del XAML
            Border card = new Border
            {
                Width = 240,
                Height = 300,
                Margin = new Thickness(15),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#161A24")),
                CornerRadius = new CornerRadius(12),
                ClipToBounds = true
            };

            DropShadowEffect glow = new DropShadowEffect { BlurRadius = 15, Opacity = 0.2, ShadowDepth = 0, Color = Colors.Black };
            card.Effect = glow;

            card.MouseEnter += (s, e) => { glow.Color = (Color)ColorConverter.ConvertFromString("#00D1D1"); glow.Opacity = 0.5; };
            card.MouseLeave += (s, e) => { glow.Color = Colors.Black; glow.Opacity = 0.2; };

            StackPanel mainStack = new StackPanel();

            // Imagen centrada
            Grid imgGrid = new Grid { Height = 180, Background = Brushes.Black };
            Image img = new Image
            {
                Stretch = Stretch.UniformToFill,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            if (juego.Img != null && !Convert.IsDBNull(juego.Img))
            {
                using (var ms = new MemoryStream((byte[])juego.Img))
                {
                    var bmp = new BitmapImage();
                    bmp.BeginInit(); bmp.StreamSource = ms; bmp.CacheOption = BitmapCacheOption.OnLoad; bmp.EndInit();
                    img.Source = bmp;
                }
            }
            imgGrid.Children.Add(img);

            Border infoBorder = new Border { Padding = new Thickness(12) };
            StackPanel infoStack = new StackPanel();

            infoStack.Children.Add(new TextBlock
            {
                Text = juego.Title.ToUpper(),
                Foreground = Brushes.White,
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis,
                Margin = new Thickness(0, 0, 0, 10)
            });

            Button btnJugar = new Button
            {
                Content = "JUGAR",
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00D1D1")),
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Bold,
                Height = 35,
                Cursor = System.Windows.Input.Cursors.Hand,
                BorderThickness = new Thickness(0)
            };

            infoBorder.Child = infoStack;
            infoStack.Children.Add(btnJugar);

            mainStack.Children.Add(imgGrid);
            mainStack.Children.Add(infoBorder);
            card.Child = mainStack;

            ContenedorBiblioteca.Children.Add(card);
        }
    }
}