using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Portafolio.Entities
{
    public class Pelicula
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public byte Genero { get; set; }
        public byte Clasificacion { get; set; }
        public byte Version { get; set; }
        public string Path { get; set; }
        public bool Estado { get; set; }
    }
}
