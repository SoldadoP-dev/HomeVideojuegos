// Models/Juego.cs
public class Juego
{
    public string ImagenRuta { get; set; } // Ejemplo: /Resources/GTAV.jpg
    public string Nombre { get; set; }

    // **NUEVA PROPIEDAD para el texto del carrusel**
    public string NombreGenero { get; set; }
}