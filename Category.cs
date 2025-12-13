// Contenido del archivo Category.cs
using System;

namespace HomeVideojuegos
{
    // La clase debe ser pública para que el motor de WPF pueda verla
    public class Category
    {
        // Las propiedades deben ser públicas para el Data Binding
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
    }
}