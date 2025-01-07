using Microsoft.EntityFrameworkCore;
using Portafolio.Entities;
using System.IO;

namespace Portafolio.Services
{
    //TECNOLOGIA DE CONEXION: ENTITY FRAMEWORK
    //OBSERVACION: LOS NOMBRES DE LOS ATRIBUTOS DEL MODELO DEBEN COINCIDIR CON LOS CAMPOS EN LA BASE DE DATOS
    public class StorageService : DbContext
    {
        //SERVICIO DE CONEXION
        private readonly ConnectionService _connectionService;
        //SERVICIO DE GESTION DE ARCHIVOS
        private readonly FileManagerService _fileManagerService;
        //RUTA DE LA CARPETA DE IMAGENES
        private readonly string Images = "img04/";
      
        //CONSTRUCTOR
        
        //FileManagerService: SERVICIO QUE GESTIONA EL MANEJO DE ARCHIVOS
        public StorageService(DbContextOptions options, FileManagerService fileManagerService, ConnectionService connectionService) : base(options)
        {
            _fileManagerService = fileManagerService;
            _connectionService = connectionService;
        }

        //OnConfiguring: CONFIGURA LA CONEXION CON LA BASE DE DATOS usando ConnectionService
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _connectionService.GetConnection("DesignA");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        //MAPEO DE TABLAS
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Clasificacion> Clasificaciones { get; set; }

        //CONFIGURACION DE MODELO PARA EF
        private void EFSettings<TEntity>(ModelBuilder modelBuilder) where TEntity : class
        {
            modelBuilder.Entity<TEntity>(e =>
            {
                e.ToTable(typeof(TEntity).Name);
                e.HasKey("Id");
                e.Property("Id").ValueGeneratedOnAdd();
            });
        }

        //CONSTRUCTOR DE ENTIDADES EF
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            EFSettings<Pelicula>(modelBuilder);
            EFSettings<Genero>(modelBuilder);
            EFSettings<Clasificacion>(modelBuilder);
        }
        //LISTADOS
        public List<Pelicula> ListarPeliculas()
        {
            return Peliculas.ToList();
        }
        public List<Genero> ListarGeneros()
        {
            return Generos.ToList();
        }
        public List<Clasificacion> ListarClasificaciones()
        {
            return Clasificaciones.ToList();
        }
        public Pelicula BuscarPelicula(int id)
        {
            return Peliculas.FirstOrDefault(p => p.Id == id);
        }

        //CAMBIAR ESTADO (INCLUYE SOFT DELETE)
        public bool? CambiarEstado(int id)
        {
            var pelicula = Peliculas.FirstOrDefault(p => p.Id == id);

            if (pelicula != null)
            {
                pelicula.Estado = !pelicula.Estado;
                SaveChanges();
                return pelicula.Estado;
            }

            return null; // Retorna null si la película no se encuentra
        }

        //CREAR
        public int CrearPelicula(Pelicula pelicula)
        {           
                var latest = new Pelicula
                {
                    Nombre = pelicula.Nombre,
                    Genero = pelicula.Genero,
                    Clasificacion = pelicula.Clasificacion,
                    Version = pelicula.Version,
                    Path = "",
                    Estado = true,
                };
            Peliculas.Add(latest);
            SaveChanges();

            return latest.Id;

        }
        //ACTUALIZAR
        public int EditarPelicula(Pelicula pelicula)
        {
            var peliculaExistente = Peliculas.FirstOrDefault(p => p.Id == pelicula.Id);
            if (peliculaExistente != null)
            {
                peliculaExistente.Nombre = pelicula.Nombre;
                peliculaExistente.Genero = pelicula.Genero;
                peliculaExistente.Clasificacion = pelicula.Clasificacion;
                peliculaExistente.Version = pelicula.Version;
                SaveChanges();

                return 0;//LOGRO EDITARLA
            }
            return -1;//NO LOGRO EDITARLA
        }

        public bool ActualizarCover(int id, IFormFile file) {

            var pelicula = Peliculas.FirstOrDefault(p => p.Id == id);
            if (pelicula == null)
            {
                return false; //PELICULA NO EXISTE
            }
            if (_fileManagerService.CheckFileTypeExtension(file) == 1)//1 = IMAGEN
            {
                _fileManagerService.RemoveFile(Images, id);
                string DBpath = _fileManagerService.GetDBpath(Images, id, file);
                string APIpath = _fileManagerService.GetAPIpath(DBpath);
                pelicula.Path = DBpath;
                SaveChanges();
                _fileManagerService.SaveFile(APIpath, file);
                return true;
            }
            
            return false; //EXTENSION NO VALIDA
        }

        //METODOS POSIBLES DE USO
        //FromSqlRaw: PARA EJECUTAR CONSULTAS SIN PARAMETROS
        //FromSqlInterpolated: PARA EJECUTAR CONSULTAS CON PARAMETROS

    }
}
