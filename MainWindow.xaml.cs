using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using HomeVideojuegos;

namespace HomeVideojuegos
{
    public partial class MainWindow : Window
    {
        private List<GameItem> _originalSellers;
        public ObservableCollection<GameItem> TopSellers { get; set; }

        // --- VARIABLES DE SCROLL CONTINUO ---
        private DispatcherTimer _autoScrollTimer;
        private TranslateTransform _carouselTransform;
        private double _itemWidth = 208; // Ancho del elemento + Margen

        // Constantes de Velocidad
        private const double NORMAL_RATE = 0.7;
        private const double ACCELERATED_RATE = 6.0;

        private double _currentScrollRate = NORMAL_RATE;
        private int _scrollDirection = 1;
        // ------------------------------------

        // *** CONSTRUCTOR SIN ARGUMENTOS ***
        public MainWindow()
        {
            InitializeComponent();
            InitializeTopSellers();
            this.DataContext = this;

            // Inicializamos la Transformación de traslación
            // **IMPORTANTE**: Si el RenderTransform es nulo, lo creamos.
            if (TopSellersItemsControl.RenderTransform is TranslateTransform tt)
            {
                _carouselTransform = tt;
            }
            else
            {
                _carouselTransform = new TranslateTransform();
                TopSellersItemsControl.RenderTransform = _carouselTransform;
            }

            SetupContinuousScroll();
        }

        // *** CONSTRUCTOR CON ARGUMENTO ***
        public MainWindow(string username) : this()
        {
            if (this.FindName("TxtUsernameDisplay") is TextBlock usernameDisplay)
            {
                usernameDisplay.Text = username;
            }
        }

        private void InitializeTopSellers()
        {
            // --- Rutas de imágenes (mantenidas) ---
            _originalSellers = new List<GameItem>
            {
                new GameItem { ImagePath = "pack://application:,,,/HomeVideojuegos;component/Images/GTA5.jpg", Title = "Grand Theft Auto V", Price = "$29.99" },
                new GameItem { ImagePath = "pack://application:,,,/HomeVideojuegos;component/Images/DiabloIV.jpg", Title = "Diablo IV", Price = "$69.99" },
                new GameItem { ImagePath = "pack://application:,,,/HomeVideojuegos;component/Images/Minecraft.jpg", Title = "Minecraft", Price = "$24.99" },
                new GameItem { ImagePath = "pack://application:,,,/HomeVideojuegos;component/Images/Hades.jpg", Title = "Hades", Price = "$12.49" },
                new GameItem { ImagePath = "pack://application:,,,/HomeVideojuegos;component/Images/ResidentEvil.jpg", Title = "Resident Evil 4 Remake", Price = "$49.99" },
                new GameItem { ImagePath = "pack://application:,,,/HomeVideojuegos;component/Images/GhostOfTsushima.jpg", Title = "Ghost of Tsushima", Price = "$59.99" },
                new GameItem { ImagePath = "pack://application:,,,/HomeVideojuegos;component/Images/Witcher3.jpg", Title = "The Witcher 3: Wild Hunt", Price = "$9.99" },
                new GameItem { ImagePath = "pack://application:,,,/HomeVideojuegos;component/Images/GodOfWar.jpg", Title = "God of War", Price = "$29.99" },
                new GameItem { ImagePath = "pack://application:,,,/HomeVideojuegos;component/Images/ReFantazio.jpg", Title = "Metaphor: ReFantazio", Price = "$69.99" },
                new GameItem { ImagePath = "pack://application:,,,/HomeVideojuegos;component/Images/GD.jpg", Title = "Geometry Dash", Price = "$4.99" },
                new GameItem { ImagePath = "pack://application:,,,/HomeVideojuegos;component/Images/Silksong.jpg", Title = "Hollow Knight: Silksong", Price = "$39.99" },
                new GameItem { ImagePath = "pack://application:,,,/HomeVideojuegos;component/Images/Inazuma.jpg", Title = "Inazuma Eleven: Victory Road", Price = "$79.99" },
                new GameItem { ImagePath = "pack://application:,,,/HomeVideojuegos;component/Images/Ori.jpg", Title = "Ori and the Will of the Wisps", Price = "$14.99" },
                new GameItem { ImagePath = "pack://application:,,,/HomeVideojuegos;component/Images/FC26.jpg", Title = "FC 26", Price = "$69.99" },
                new GameItem { ImagePath = "pack://application:,,,/HomeVideojuegos;component/Images/Balatro.jpg", Title = "Balatro", Price = "$13.99" },
            };

            TopSellers = new ObservableCollection<GameItem>(_originalSellers);

            // Duplicar la lista para el efecto de scroll infinito
            for (int i = 0; i < 100; i++)
            {
                foreach (var item in _originalSellers)
                {
                    TopSellers.Add(item);
                }
            }

            // (Esta línea es redundante si se hace en el constructor, pero no hace daño)
            // TopSellersItemsControl.RenderTransform = new TranslateTransform();
        }

        // --- LÓGICA DE SCROLL CONTINUO Y ACELERACIÓN ---

        private void SetupContinuousScroll()
        {
            if (_autoScrollTimer == null)
            {
                _autoScrollTimer = new DispatcherTimer(DispatcherPriority.Normal);
                _autoScrollTimer.Interval = TimeSpan.FromMilliseconds(16);
                _autoScrollTimer.Tick += AutoScrollTimer_Tick;
            }

            _currentScrollRate = NORMAL_RATE;
            _scrollDirection = 1;

            _autoScrollTimer.Start();
        }

        private void AutoScrollTimer_Tick(object sender, EventArgs e)
        {
            if (_carouselTransform == null) return; // Chequeo de seguridad

            // 1. Calcular el desplazamiento usando la velocidad y la dirección actuales
            double scrollDelta = _currentScrollRate * _scrollDirection;

            // 2. Aplicar el desplazamiento
            _carouselTransform.X -= scrollDelta;

            // 3. Lógica de "Scroll Infinito"
            double fullListWidth = _originalSellers.Count * _itemWidth;

            if (_carouselTransform.X <= -fullListWidth)
            {
                _carouselTransform.X += fullListWidth;
            }
            else if (_carouselTransform.X > 0)
            {
                _carouselTransform.X -= fullListWidth;
            }
        }

        // --- LÓGICA DE BOTONES (ACELERACIÓN) ---

        // Flecha izquierda: Acelera y cambia la dirección a la inversa (-1)
        private void PreviousButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _currentScrollRate = ACCELERATED_RATE;
            _scrollDirection = -1;
            e.Handled = true; // Consumir el evento
        }

        // Flecha derecha: Acelera y mantiene la dirección normal (+1)
        private void NextButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _currentScrollRate = ACCELERATED_RATE;
            _scrollDirection = 1;
            e.Handled = true; // Consumir el evento
        }

        // MouseUp (para ambas flechas): Restaura la velocidad automática y la dirección predeterminada
        private void NextPreviousButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Restablece la velocidad a la normal (lenta)
            _currentScrollRate = NORMAL_RATE;

            // Restablece la dirección a la predeterminada (hacia la derecha/avanzar)
            _scrollDirection = 1;
            e.Handled = true; // Consumir el evento
        }

        // El resto de métodos de navegación se mantienen sin cambios

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Cerrar Sesión. ¡Adiós!");
        }

        private void ExitApplication_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ExploreAllGames_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Navegando a la página de Exploración de Juegos.");
        }

        private void TopNavButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            MessageBox.Show($"Navegando a: {btn.Content}");
        }

        private void LibraryButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Navegando a la Biblioteca (Librería).");
        }

        private void WishlistButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Navegando a la Lista de Deseos.");
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Navegando al Carrito.");
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Navegando a Configuración.");
        }
    }
}