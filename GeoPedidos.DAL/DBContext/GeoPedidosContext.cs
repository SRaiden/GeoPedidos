using System;
using System.Collections.Generic;
using GeoPedidos.Entity;
using Microsoft.EntityFrameworkCore;

namespace GeoPedidos.DAL.DBContext;

public partial class GeoPedidosContext : DbContext
{
    public GeoPedidosContext()
    {
    }

    public GeoPedidosContext(DbContextOptions<GeoPedidosContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Configuracion> Configuracion { get; set; }

    public virtual DbSet<FabricaGusto> FabricaGustos { get; set; }

    public virtual DbSet<FabricaInsumo> FabricaInsumos { get; set; }

    public virtual DbSet<FabricaPasteleria> FabricaPasteleria { get; set; }

    public virtual DbSet<FabricaPedido> FabricaPedidos { get; set; }

    public virtual DbSet<FabricaPedidosDetalle> FabricaPedidosDetalles { get; set; }

    public virtual DbSet<FabricaPedidosRemito> FabricaPedidosRemitos { get; set; }

    public virtual DbSet<FabricaProducto> FabricaProductos { get; set; }

    public virtual DbSet<FabricaUsuario> FabricaUsuarios { get; set; }

    public virtual DbSet<GeneralEmpresa> GeneralEmpresas { get; set; }

    public virtual DbSet<GeneralSucursales> GeneralSucursales { get; set; }

    public virtual DbSet<VistaCierresCaja> VistaCierresCajas { get; set; }

    public virtual DbSet<VistaFichadasReporte> VistaFichadasReportes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { 
    
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("geopedidos")
            .UseCollation("SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.Entity<FabricaGusto>(entity =>
        {
            entity.ToTable("fabrica_gustos", "geosoft");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Categoria)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.IdEmpresa).HasColumnName("Id_Empresa");
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Configuracion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Configuracion");
            entity.ToTable("Configuracion");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Propiedad)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("propiedad");
            entity.Property(e => e.Recurso)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("recurso");
            entity.Property(e => e.Valor)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("valor");
        });

        modelBuilder.Entity<FabricaInsumo>(entity =>
        {
            entity.ToTable("fabrica_insumos", "geosoft");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Categoria)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.IdEmpresa)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("Id_Empresa");
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FabricaPasteleria>(entity =>
        {
            entity.ToTable("fabrica_pasteleria", "geosoft");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Categoria)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.IdEmpresa).HasColumnName("Id_Empresa");
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FabricaPedido>(entity =>
        {
            entity.ToTable("fabrica_pedidos", "geosoft");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.Estado)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.FechaAnulado)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_Anulado");
            entity.Property(e => e.FechaConfirmado)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_Confirmado");
            entity.Property(e => e.FechaLeido)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_Leido");
            entity.Property(e => e.FechaRemitido)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_Remitido");
            entity.Property(e => e.FechaEntrega)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_Entrega");
            entity.Property(e => e.IdSucursal).HasColumnName("Id_Sucursal");
            entity.Property(e => e.IdUsuario).HasColumnName("Id_Usuario");
            entity.Property(e => e.LeidoCaja).HasColumnName("Leido_Caja");
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.Nota)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Comentario)
               .HasMaxLength(200)
               .IsUnicode(false);
            entity.Property(e => e.NumeroPedido).HasColumnName("Numero_Pedido");
            entity.Property(e => e.Tipo)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FabricaPedidosDetalle>(entity =>
        {
            entity.ToTable("fabrica_pedidos_detalles", "geosoft");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.IdPedido).HasColumnName("Id_Pedido");
            entity.Property(e => e.Kilo).HasColumnType("decimal(11, 3)");
            entity.Property(e => e.Modified).HasColumnType("datetime");
        });

        modelBuilder.Entity<FabricaPedidosRemito>(entity =>
        {
            entity.ToTable("fabrica_pedidos_remito", "geosoft");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CodBarra)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Cod_Barra");
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.IdPedido).HasColumnName("Id_Pedido");
            entity.Property(e => e.Kilos).HasColumnType("decimal(11, 3)");
            entity.Property(e => e.Modified).HasColumnType("datetime");
        });

        modelBuilder.Entity<FabricaProducto>(entity =>
        {
            entity.ToTable("fabrica_productos", "geosoft");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Categoria)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.IdEmpresa).HasColumnName("Id_Empresa");
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FabricaUsuario>(entity =>
        {
            entity.ToTable("fabrica_usuarios", "geosoft");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Contraseña)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IdEmpresa).HasColumnName("Id_Empresa");
            entity.Property(e => e.IdSucursal)
                .HasComment("Numero de sucursal, no ID")
                .HasColumnName("Id_Sucursal");
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.OkLogin).HasColumnName("OK_Login");
            entity.Property(e => e.Rol)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<GeneralEmpresa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_empresas");

            entity.ToTable("general_empresas", "geosoft");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Alias)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ColorFondo).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ColorFuente).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Estado)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MailAvisoPedido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Mail_Aviso_Pedido");
            entity.Property(e => e.NombreEmpresa)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Nombre_Empresa");
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RgbFondo)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("rgbFondo");
            entity.Property(e => e.RgbFuente)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("rgbFuente");
        });

        modelBuilder.Entity<GeneralSucursales>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_sucursales");

            entity.ToTable("general_sucursales", "geosoft");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Alias)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Ciudad)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CodigoArea)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CostoEnvio).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.Domicilio)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmpresaId).HasColumnName("Empresa_Id");
            entity.Property(e => e.FechaLimiteConexionesVencidas).HasColumnType("datetime");
            entity.Property(e => e.FechaVencimiento).HasColumnType("datetime");
            entity.Property(e => e.HorarioId).HasColumnName("Horario_Id");
            entity.Property(e => e.ListaPreciosId).HasColumnName("ListaPrecios_Id");
            entity.Property(e => e.Localidad)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NombreSucursal)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Nombre_Sucursal");
            entity.Property(e => e.NumeroSucursal).HasColumnName("Numero_Sucursal");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UltimoPago).HasColumnType("datetime");
            entity.Property(e => e.Whatsapp)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VistaCierresCaja>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vistaCierresCaja", "geosoft");

            entity.Property(e => e.EmpresaId).HasColumnName("Empresa_Id");
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.Hora).HasColumnType("datetime");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NombreEmpresa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NombreSucursal)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SucursalId).HasColumnName("Sucursal_Id");
            entity.Property(e => e.TotalCtaCte).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalEfectivo).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalEgreso).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalIngreso).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalTarjeta).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalVentas).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Usuario)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VistaFichadasReporte>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vista_Fichadas_Reporte", "geosoft");

            entity.Property(e => e.ApellidoNombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("date");
            entity.Property(e => e.FechaOperativa).HasColumnType("date");
            entity.Property(e => e.Hora).HasColumnType("datetime");
            entity.Property(e => e.HorasNetas).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Motivo)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
