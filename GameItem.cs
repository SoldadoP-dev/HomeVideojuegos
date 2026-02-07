using System.Windows.Media;

namespace HomeVideojuegos
{
    public class GameItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Price { get; set; }
        public string Fabricante { get; set; }
        public ImageSource ImageSource { get; set; } // Crucial para mostrar la foto de la BD
    }
}