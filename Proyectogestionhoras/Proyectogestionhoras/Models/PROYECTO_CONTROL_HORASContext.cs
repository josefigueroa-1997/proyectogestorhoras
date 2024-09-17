using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Proyectogestionhoras.Models
{
    public partial class PROYECTO_CONTROL_HORASContext : DbContext
    {
        public PROYECTO_CONTROL_HORASContext()
        {
        }

        public PROYECTO_CONTROL_HORASContext(DbContextOptions<PROYECTO_CONTROL_HORASContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ccosto> Ccostos { get; set; } = null!;
        public virtual DbSet<CcostoUnegocio> CcostoUnegocios { get; set; } = null!;
        public virtual DbSet<Cliente> Clientes { get; set; } = null!;
        public virtual DbSet<Contacto> Contactos { get; set; } = null!;
        public virtual DbSet<Empresa> Empresas { get; set; } = null!;
        public virtual DbSet<Factura> Facturas { get; set; } = null!;
        public virtual DbSet<FacturaProyecto> FacturaProyectos { get; set; } = null!;
        public virtual DbSet<Gasto> Gastos { get; set; } = null!;
        public virtual DbSet<Planilla> Planillas { get; set; } = null!;
        public virtual DbSet<PlanillaUsusarioProyecto> PlanillaUsusarioProyectos { get; set; } = null!;
        public virtual DbSet<Presupuesto> Presupuestos { get; set; } = null!;
        public virtual DbSet<Proyecto> Proyectos { get; set; } = null!;
        public virtual DbSet<ProyectoGasto> ProyectoGastos { get; set; } = null!;
        public virtual DbSet<Recurso> Recursos { get; set; } = null!;
        public virtual DbSet<Rol> Rols { get; set; } = null!;
        public virtual DbSet<SegmentoCosto> SegmentoCostos { get; set; } = null!;
        public virtual DbSet<Sucursal> Sucursals { get; set; } = null!;
        public virtual DbSet<SucursalCliente> SucursalClientes { get; set; } = null!;
        public virtual DbSet<Tipologium> Tipologia { get; set; } = null!;
        public virtual DbSet<Unegocio> Unegocios { get; set; } = null!;
        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;
        public virtual DbSet<UsuarioProyecto> UsuarioProyectos { get; set; } = null!;

       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ccosto>(entity =>
            {
                entity.ToTable("CCOSTO");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.TipoCcosto)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("TIPO_CCOSTO");
            });

            modelBuilder.Entity<CcostoUnegocio>(entity =>
            {
                entity.ToTable("CCOSTO_UNEGOCIO");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Codigo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CODIGO");

                entity.Property(e => e.IdCcosto).HasColumnName("ID_CCOSTO");

                entity.Property(e => e.IdUnegocio).HasColumnName("ID_UNEGOCIO");

                entity.HasOne(d => d.IdCcostoNavigation)
                    .WithMany(p => p.CcostoUnegocios)
                    .HasForeignKey(d => d.IdCcosto)
                    .HasConstraintName("ID_CCOSTO_FK");

                entity.HasOne(d => d.IdUnegocioNavigation)
                    .WithMany(p => p.CcostoUnegocios)
                    .HasForeignKey(d => d.IdUnegocio)
                    .HasConstraintName("ID_UNEGOCIO_FK");
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("CLIENTE");

                entity.HasIndex(e => e.IdCliente, "UQ__CLIENTE__677F38F40BCA749B")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Ciudad)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("CIUDAD");

                entity.Property(e => e.Direccion)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("DIRECCION");

                entity.Property(e => e.IdCliente)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("id_cliente");

                entity.Property(e => e.Instagram)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("INSTAGRAM");

                entity.Property(e => e.Linkedin)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("LINKEDIN");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE");

                entity.Property(e => e.PagWeb)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("PAG_WEB");

                entity.Property(e => e.Pais)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("PAIS");

                entity.Property(e => e.Telefono)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TELEFONO");
            });

            modelBuilder.Entity<Contacto>(entity =>
            {
                entity.ToTable("CONTACTO");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Cargo)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("CARGO");

                entity.Property(e => e.Desde)
                    .HasColumnType("date")
                    .HasColumnName("DESDE");

                entity.Property(e => e.Email)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.Hasta)
                    .HasColumnType("date")
                    .HasColumnName("HASTA");

                entity.Property(e => e.IdCliente).HasColumnName("ID_CLIENTE");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE");

                entity.Property(e => e.Telefono)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TELEFONO");

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.Contactos)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ID_CLIENTE_FK");
            });

            modelBuilder.Entity<Empresa>(entity =>
            {
                entity.ToTable("EMPRESA");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.TipoEmpresa)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("TIPO_EMPRESA");
            });

            modelBuilder.Entity<Factura>(entity =>
            {
                entity.ToTable("FACTURA");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Fecha)
                    .HasColumnType("datetime")
                    .HasColumnName("FECHA");

                entity.Property(e => e.Iva)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("IVA");

                entity.Property(e => e.Monto)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("MONTO");

                entity.Property(e => e.Total)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("TOTAL");
            });

            modelBuilder.Entity<FacturaProyecto>(entity =>
            {
                entity.ToTable("FACTURA_PROYECTO");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.IdFactura).HasColumnName("ID_FACTURA");

                entity.Property(e => e.IdProyecto).HasColumnName("ID_PROYECTO");

                entity.HasOne(d => d.IdFacturaNavigation)
                    .WithMany(p => p.FacturaProyectos)
                    .HasForeignKey(d => d.IdFactura)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ID_FACTURA_FK");

                entity.HasOne(d => d.IdProyectoNavigation)
                    .WithMany(p => p.FacturaProyectos)
                    .HasForeignKey(d => d.IdProyecto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ID_PROYECTOFACTURA_FK");
            });

            modelBuilder.Entity<Gasto>(entity =>
            {
                entity.ToTable("GASTOS");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Cuenta)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("CUENTA");

                entity.Property(e => e.IdCuenta).HasColumnName("ID_CUENTA");

                entity.Property(e => e.Monto)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("MONTO");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE");

                entity.Property(e => e.TipoServicio)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("TIPO_SERVICIO");
            });

            modelBuilder.Entity<Planilla>(entity =>
            {
                entity.ToTable("PLANILLA");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<PlanillaUsusarioProyecto>(entity =>
            {
                entity.ToTable("PLANILLA_USUSARIO_PROYECTO");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.IdPlanilla).HasColumnName("ID_PLANILLA");

                entity.Property(e => e.IdUsuProy).HasColumnName("ID_USU_PROY");

                entity.HasOne(d => d.IdPlanillaNavigation)
                    .WithMany(p => p.PlanillaUsusarioProyectos)
                    .HasForeignKey(d => d.IdPlanilla)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ID_PLANILLA_FK");

                entity.HasOne(d => d.IdUsuProyNavigation)
                    .WithMany(p => p.PlanillaUsusarioProyectos)
                    .HasForeignKey(d => d.IdUsuProy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ID_USU_PRO_FK");
            });

            modelBuilder.Entity<Presupuesto>(entity =>
            {
                entity.ToTable("PRESUPUESTO");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Afectaiva)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("AFECTAIVA");

                entity.Property(e => e.Moneda)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("MONEDA");

                entity.Property(e => e.Monto)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("MONTO");
            });

            modelBuilder.Entity<Proyecto>(entity =>
            {
                entity.ToTable("PROYECTO");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Fecha)
                    .HasColumnType("date")
                    .HasColumnName("FECHA");

                entity.Property(e => e.FechaInicio)
                    .HasColumnType("date")
                    .HasColumnName("FECHA_INICIO");

                entity.Property(e => e.FechaTermino)
                    .HasColumnType("date")
                    .HasColumnName("FECHA_TERMINO");

                entity.Property(e => e.IdCcostoUnegocio).HasColumnName("ID_CCOSTO_UNEGOCIO");

                entity.Property(e => e.IdCliente).HasColumnName("ID_CLIENTE");

                entity.Property(e => e.IdPresupuesto).HasColumnName("ID_PRESUPUESTO");

                entity.Property(e => e.IdTipologia).HasColumnName("ID_TIPOLOGIA");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE");

                entity.Property(e => e.NumProyecto)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("NUM_PROYECTO");

                entity.Property(e => e.Plazo).HasColumnName("PLAZO");

                entity.Property(e => e.TipoEmpresa).HasColumnName("TIPO_EMPRESA");

                entity.Property(e => e.TipoStatus)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("TIPO_STATUS");

                entity.HasOne(d => d.IdCcostoUnegocioNavigation)
                    .WithMany(p => p.Proyectos)
                    .HasForeignKey(d => d.IdCcostoUnegocio)
                    .HasConstraintName("ID_CCOSTO_UNEGOCIO_FK");

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.Proyectos)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ID_CLIENTE_PROYECTO_FK");

                entity.HasOne(d => d.IdPresupuestoNavigation)
                    .WithMany(p => p.Proyectos)
                    .HasForeignKey(d => d.IdPresupuesto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ID_PRESUPUESTO_FK");

                entity.HasOne(d => d.IdTipologiaNavigation)
                    .WithMany(p => p.Proyectos)
                    .HasForeignKey(d => d.IdTipologia)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ID_TIPOLOGIA_FK");

                entity.HasOne(d => d.TipoEmpresaNavigation)
                    .WithMany(p => p.Proyectos)
                    .HasForeignKey(d => d.TipoEmpresa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TIPO_EMPRESA_FK");
            });

            modelBuilder.Entity<ProyectoGasto>(entity =>
            {
                entity.ToTable("PROYECTO_GASTOS");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.IdGastos).HasColumnName("ID_GASTOS");

                entity.Property(e => e.IdProyecto).HasColumnName("ID_PROYECTO");

                entity.HasOne(d => d.IdGastosNavigation)
                    .WithMany(p => p.ProyectoGastos)
                    .HasForeignKey(d => d.IdGastos)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ID_GASTOS_FK");

                entity.HasOne(d => d.IdProyectoNavigation)
                    .WithMany(p => p.ProyectoGastos)
                    .HasForeignKey(d => d.IdProyecto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ID_PRO_GASTOS_IDFK");
            });

            modelBuilder.Entity<Recurso>(entity =>
            {
                entity.ToTable("RECURSO");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CostoUnitario)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("COSTO_UNITARIO");

                entity.Property(e => e.IdSegmentocostos).HasColumnName("ID_SEGMENTOCOSTOS");

                entity.Property(e => e.NombreRecurso)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE_RECURSO");

                entity.Property(e => e.NumeroHoras).HasColumnName("NUMERO_HORAS");

                entity.HasOne(d => d.IdSegmentocostosNavigation)
                    .WithMany(p => p.Recursos)
                    .HasForeignKey(d => d.IdSegmentocostos)
                    .HasConstraintName("ID_SEGMENTO_COSTOS_FK");
            });

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("ROL");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE");
            });

            modelBuilder.Entity<SegmentoCosto>(entity =>
            {
                entity.ToTable("SEGMENTO_COSTOS");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Cuenta)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("CUENTA");

                entity.Property(e => e.IdCuenta).HasColumnName("ID_CUENTA");

                entity.Property(e => e.IdHonorariosExternos).HasColumnName("ID_HONORARIOS_EXTERNOS");
            });

            modelBuilder.Entity<Sucursal>(entity =>
            {
                entity.ToTable("SUCURSAL");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE");
            });

            modelBuilder.Entity<SucursalCliente>(entity =>
            {
                entity.ToTable("SUCURSAL_CLIENTE");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.IdCliente).HasColumnName("ID_CLIENTE");

                entity.Property(e => e.IdSucursal).HasColumnName("ID_SUCURSAL");

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.SucursalClientes)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ID_CLIENTE_SU_FK");

                entity.HasOne(d => d.IdSucursalNavigation)
                    .WithMany(p => p.SucursalClientes)
                    .HasForeignKey(d => d.IdSucursal)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ID_SUCURSAL_CLI_FK");
            });

            modelBuilder.Entity<Tipologium>(entity =>
            {
                entity.ToTable("TIPOLOGIA");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.TipoTipologia)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("TIPO_TIPOLOGIA");
            });

            modelBuilder.Entity<Unegocio>(entity =>
            {
                entity.ToTable("UNEGOCIO");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.TipoUnegocio)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("TIPO_UNEGOCIO");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("USUARIO");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Contrasena).HasColumnName("CONTRASENA");

                entity.Property(e => e.Email)
                    .HasMaxLength(90)
                    .IsUnicode(false)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.IdRecurso).HasColumnName("ID_RECURSO");

                entity.Property(e => e.IdRol).HasColumnName("ID_ROL");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE");

                entity.Property(e => e.NombreUsuario)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE_USUARIO");

                entity.Property(e => e.Telefono)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TELEFONO");

                entity.HasOne(d => d.IdRecursoNavigation)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.IdRecurso)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RECURSO_CK");

                entity.HasOne(d => d.IdRolNavigation)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.IdRol)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ID_ROL_FK");
            });

            modelBuilder.Entity<UsuarioProyecto>(entity =>
            {
                entity.ToTable("USUARIO_PROYECTO");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.HhSemanales).HasColumnName("HH_SEMANALES");

                entity.Property(e => e.IdProyecto).HasColumnName("ID_PROYECTO");

                entity.Property(e => e.IdUsuario).HasColumnName("ID_USUARIO");

                entity.HasOne(d => d.IdProyectoNavigation)
                    .WithMany(p => p.UsuarioProyectos)
                    .HasForeignKey(d => d.IdProyecto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("USUARIO_PROYECTO_IDPROYECTO_FK");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.UsuarioProyectos)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("USUARIO_PROYECTO_IDUSUARIO_FK");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
