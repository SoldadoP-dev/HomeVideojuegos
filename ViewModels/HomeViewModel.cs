// ViewModels/HomeViewModel.cs (Extracto)
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System;

public class HomeViewModel : ViewModelBase
{
    public ObservableCollection<Juego> TopSellers { get; set; }
    private DispatcherTimer _timer;

    public HomeViewModel()
    {
        TopSellers = new ObservableCollection<Juego>
        {
            // Carga tus juegos aquí
            new Juego { ImagenRuta = "GTAV.jpg" },
            new Juego { ImagenRuta = "Minecraft.jpg" },
            // ... los demás juegos de Top Sellers
        };

        // Inicializa el temporizador para el movimiento automático
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(3); // Mueve cada 3 segundos
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        // En un carrusel real, la lógica aquí notificaría a la vista para
        // que desplace el ScrollViewer o cambie el índice del elemento visible.
        // Por la simplicidad de la implementación en XAML, esto lo delegaremos
        // al code-behind de la ventana (MainView.xaml.cs) por única vez,
        // usando el evento de Tick para provocar la acción de scroll.
    }
}