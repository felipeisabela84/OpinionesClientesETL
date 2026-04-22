using OpinionesClientesETL.DATA.Entities.Dwh.Dimensions;
using OpinionesClientesETL.DATA.Entities.Dwh.Facts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace OpinionesClientesETL.DATA.Persisitence
{
    public class DWHInventoryContext : DbContext
    {
        public DWHInventoryContext(DbContextOptions<DWHInventoryContext> options)
            : base(options)
        {

        }
        public DbSet<DimCategoria> DimCategorias { get; set; }
        public DbSet<DimClasificacion> DimClasificaciones { get; set; }
        public DbSet<DimClientes> DimClientes { get; set; }
        public DbSet<DimFecha> DimFechas { get; set; }
        public DbSet<DimFuentes> DimFuentes { get; set; }
        public DbSet<DimProductos> DimProductos { get; set; }
        public DbSet<DimRating> DimRatings { get; set; }

        public DbSet<FactOpiniones> FactOpiniones { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DimCategoria>()
            .HasKey(x => x.IDCATEGORIA);
            modelBuilder.Entity<DimClasificacion>()
            .HasKey(x => x.IDCLASIFICACION);

            modelBuilder.Entity<DimClientes>()
                            .HasKey(x => x.IDCLIENTE);
            modelBuilder.Entity<DimFecha>()
                            .HasKey(x => x.IDFECHA);
            modelBuilder.Entity<DimFecha>()
             .Property(x => x.FECHA_DT)
              .HasColumnName("FECHA");
            modelBuilder.Entity<DimFuentes>()
                            .HasKey(x => x.IDFUENTE);

            modelBuilder.Entity<DimProductos>()
                            .HasKey(x => x.IDPRODUCTO); 
            modelBuilder.Entity<DimRating>()
                            .HasKey(x => x.IDRATING);
            modelBuilder.Entity<DimClasificacion>()
                .Property(x => x.IDCLASIFICACION)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<FactOpiniones>()
                .HasKey(x => x.IDOPINION);

            modelBuilder.Entity<FactOpiniones>()
             .Property(x => x.IDOPINION)
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<DimProductos>()
            .Property(x => x.IDPRODUCTO)
            .ValueGeneratedNever();

            modelBuilder.Entity<DimClientes>()
                .Property(x => x.IDCLIENTE)
                .ValueGeneratedNever();

            modelBuilder.Entity<DimProductos>()
             .ToTable("PRODUCTOS", "DIM");

            modelBuilder.Entity<DimClientes>()
                .ToTable("CLIENTES", "DIM");

            modelBuilder.Entity<DimCategoria>()
                .ToTable("CATEGORIA", "DIM");

            modelBuilder.Entity<DimFuentes>()
                .ToTable("FUENTES", "DIM");

            modelBuilder.Entity<DimRating>()
                .ToTable("RATING", "DIM");

            modelBuilder.Entity<DimFecha>()
                .ToTable("FECHA", "DIM");

            modelBuilder.Entity<DimClasificacion>()
                .ToTable("CLASIFICACION", "DIM");

            modelBuilder.Entity<FactOpiniones>()
            .ToTable("OPINIONES", "FACT");



        }
    }
}
