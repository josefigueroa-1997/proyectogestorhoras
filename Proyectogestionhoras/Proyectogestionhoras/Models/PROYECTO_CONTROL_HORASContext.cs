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
        public virtual DbSet<Cuentum> Cuenta { get; set; } = null!;
        public virtual DbSet<Empresa> Empresas { get; set; } = null!;
        public virtual DbSet<Factura> Facturas { get; set; } = null!;
        public virtual DbSet<Gasto> Gastos { get; set; } = null!;
        public virtual DbSet<HistorialCosto> HistorialCostos { get; set; } = null!;
        public virtual DbSet<Planilla> Planillas { get; set; } = null!;
        public virtual DbSet<PlanillaUsusarioProyecto> PlanillaUsusarioProyectos { get; set; } = null!;
        public virtual DbSet<Presupuesto> Presupuestos { get; set; } = null!;
        public virtual DbSet<Proyecto> Proyectos { get; set; } = null!;
        public virtual DbSet<ProyectoGasto> ProyectoGastos { get; set; } = null!;
        public virtual DbSet<ProyectoServicio> ProyectoServicios { get; set; } = null!;
        public virtual DbSet<Recurso> Recursos { get; set; } = null!;
        public virtual DbSet<Rol> Rols { get; set; } = null!;
        public virtual DbSet<Segmento> Segmentos { get; set; } = null!;
        public virtual DbSet<SegmentoCcosto> SegmentoCcostos { get; set; } = null!;
        public virtual DbSet<Servicio> Servicios { get; set; } = null!;
        public virtual DbSet<StatusProyecto> StatusProyectos { get; set; } = null!;
        public virtual DbSet<Sucursal> Sucursals { get; set; } = null!;
        public virtual DbSet<SucursalCliente> SucursalClientes { get; set; } = null!;
        public virtual DbSet<Tipologium> Tipologia { get; set; } = null!;
        public virtual DbSet<Unegocio> Unegocios { get; set; } = null!;
        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;
        public virtual DbSet<UsuarioProyecto> UsuarioProyectos { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=PROYECTO_CONTROL_HORAS.mssql.somee.com;Database=PROYECTO_CONTROL_HORAS;user=pepelechero_SQLLogin_1;pwd=87zhqvm9wv;");
            }
        }

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

                entity.HasIndex(e => e.IdCliente, "UQ__CLIENTE__23A341318A5D8AAA")
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
                    .HasColumnName("ID_CLIENTE");

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
                    .HasMaxLength(50)
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

            modelBuilder.Entity<Cuentum>(entity =>
            {
                entity.ToTable("CUENTA");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Cuenta)
                    .IsUnicode(false)
                    .HasColumnName("CUENTA");

                entity.Property(e => e.Idcuenta).HasColumnName("IDCUENTA");
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

                entity.Property(e => e.IdProyecto).HasColumnName("ID_PROYECTO");

                entity.Property(e => e.Idsegmento).HasColumnName("IDSEGMENTO");

                entity.HasOne(d => d.IdProyectoNavigation)
                    .WithMany(p => p.Facturas)
                    .HasForeignKey(d => d.IdProyecto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ID_PRO_FAC");

                entity.HasOne(d => d.IdsegmentoNavigation)
                    .WithMany(p => p.Facturas)
                    .HasForeignKey(d => d.Idsegmento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("IDSEGFAC");
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

                entity.Property(e => e.Nombre)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE");
            });

            modelBuilder.Entity<HistorialCosto>(entity =>
            {
                entity.ToTable("HISTORIAL_COSTOS");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CostoUnitario).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.FechaFin).HasColumnType("date");

                entity.Property(e => e.FechaInicio).HasColumnType("date");

                entity.Property(e => e.IdUsuario).HasColumnName("ID_USUARIO");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.HistorialCostos)
                    .HasForeignKey(d => d.IdUsuario)
                    .HasConstraintName("FK_HISTORIAL_COSTOS_USUARIO");
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

                entity.Property(e => e.Montoiva)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("MONTOIVA");
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

                entity.Property(e => e.FechaPlazoNeg)
                    .HasColumnType("date")
                    .HasColumnName("FECHA_PLAZO_NEG");

                entity.Property(e => e.FechaTermino)
                    .HasColumnType("date")
                    .HasColumnName("FECHA_TERMINO");

                entity.Property(e => e.IdCcostoUnegocio).HasColumnName("ID_CCOSTO_UNEGOCIO");

                entity.Property(e => e.IdClienteSucursal).HasColumnName("ID_CLIENTE_SUCURSAL");

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

                entity.Property(e => e.PorcentajeProbabilidad)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("PORCENTAJE_PROBABILIDAD");

                entity.Property(e => e.Probabilidad)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("PROBABILIDAD");

                entity.Property(e => e.StatusProyecto).HasColumnName("STATUS_PROYECTO");

                entity.Property(e => e.TipoEmpresa).HasColumnName("TIPO_EMPRESA");

                entity.HasOne(d => d.IdCcostoUnegocioNavigation)
                    .WithMany(p => p.Proyectos)
                    .HasForeignKey(d => d.IdCcostoUnegocio)
                    .HasConstraintName("ID_CCOSTO_UNEGOCIO_FK");

                entity.HasOne(d => d.IdClienteSucursalNavigation)
                    .WithMany(p => p.Proyectos)
                    .HasForeignKey(d => d.IdClienteSucursal)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ID_CLI_SUC_FK");

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

                entity.HasOne(d => d.StatusProyectoNavigation)
                    .WithMany(p => p.Proyectos)
                    .HasForeignKey(d => d.StatusProyecto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("STATUS_PRO_FK");

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

                entity.Property(e => e.Idsegmento).HasColumnName("IDSEGMENTO");

                entity.Property(e => e.Monto)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("MONTO");

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

                entity.HasOne(d => d.IdsegmentoNavigation)
                    .WithMany(p => p.ProyectoGastos)
                    .HasForeignKey(d => d.Idsegmento)
                    .HasConstraintName("IDGASEG");
            });

            modelBuilder.Entity<ProyectoServicio>(entity =>
            {
                entity.ToTable("PROYECTO_SERVICIO");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.IdProyecto).HasColumnName("ID_PROYECTO");

                entity.Property(e => e.IdServicio).HasColumnName("ID_SERVICIO");

                entity.Property(e => e.Idsegmento).HasColumnName("IDSEGMENTO");

                entity.Property(e => e.Monto)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("MONTO");

                entity.HasOne(d => d.IdProyectoNavigation)
                    .WithMany(p => p.ProyectoServicios)
                    .HasForeignKey(d => d.IdProyecto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ID_PROYECTO_S_FK");

                entity.HasOne(d => d.IdServicioNavigation)
                    .WithMany(p => p.ProyectoServicios)
                    .HasForeignKey(d => d.IdServicio)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ID_SERVICIO_FK");

                entity.HasOne(d => d.IdsegmentoNavigation)
                    .WithMany(p => p.ProyectoServicios)
                    .HasForeignKey(d => d.Idsegmento)
                    .HasConstraintName("IDSERSEG");
            });

            modelBuilder.Entity<Recurso>(entity =>
            {
                entity.ToTable("RECURSO");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CostoUnitario)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("COSTO_UNITARIO");

                entity.Property(e => e.Desde)
                    .HasColumnType("date")
                    .HasColumnName("DESDE");

                entity.Property(e => e.Hasta)
                    .HasColumnType("date")
                    .HasColumnName("HASTA");

                entity.Property(e => e.HhAnuales)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("HH_ANUALES");

                entity.Property(e => e.HhMensuales)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("HH_MENSUALES");

                entity.Property(e => e.NombreRecurso)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE_RECURSO");

                entity.Property(e => e.NumeroHoras).HasColumnName("NUMERO_HORAS");

                entity.Property(e => e.ProcentajeProyecto)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("PROCENTAJE_PROYECTO");
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

            modelBuilder.Entity<Segmento>(entity =>
            {
                entity.ToTable("SEGMENTO");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.IdCuenta).HasColumnName("ID_CUENTA");

                entity.Property(e => e.Nombre)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE");

                entity.Property(e => e.TipoSegmento)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("TIPO_SEGMENTO");

                entity.HasOne(d => d.IdCuentaNavigation)
                    .WithMany(p => p.Segmentos)
                    .HasForeignKey(d => d.IdCuenta)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("CUENTA_FK");
            });

            modelBuilder.Entity<SegmentoCcosto>(entity =>
            {
                entity.ToTable("SEGMENTO_CCOSTO");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.IdCcosto).HasColumnName("ID_CCOSTO");

                entity.Property(e => e.IdSegmento).HasColumnName("ID_SEGMENTO");

                entity.HasOne(d => d.IdCcostoNavigation)
                    .WithMany(p => p.SegmentoCcostos)
                    .HasForeignKey(d => d.IdCcosto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ID_CCOSTO_S_FK");

                entity.HasOne(d => d.IdSegmentoNavigation)
                    .WithMany(p => p.SegmentoCcostos)
                    .HasForeignKey(d => d.IdSegmento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ID_SEGMENTO_FK");
            });

            modelBuilder.Entity<Servicio>(entity =>
            {
                entity.ToTable("SERVICIOS");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE");
            });

            modelBuilder.Entity<StatusProyecto>(entity =>
            {
                entity.ToTable("STATUS_PROYECTO");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.TipoStatus)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("TIPO_STATUS");
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

                entity.Property(e => e.CostoUnitarioAsignado).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.HhConsultora).HasColumnName("HH_CONSULTORA");

                entity.Property(e => e.HhConsultorb).HasColumnName("HH_CONSULTORB");

                entity.Property(e => e.HhConsultorc).HasColumnName("HH_CONSULTORC");

                entity.Property(e => e.HhSocios).HasColumnName("HH_SOCIOS");

                entity.Property(e => e.HhStaff).HasColumnName("HH_STAFF");

                entity.Property(e => e.IdProyecto).HasColumnName("ID_PROYECTO");

                entity.Property(e => e.IdUsuario).HasColumnName("ID_USUARIO");

                entity.Property(e => e.Idsegmento).HasColumnName("IDSEGMENTO");

                entity.HasOne(d => d.IdProyectoNavigation)
                    .WithMany(p => p.UsuarioProyectos)
                    .HasForeignKey(d => d.IdProyecto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PRO_USU_FK");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.UsuarioProyectos)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("USUARIO_PRO_FK");

                entity.HasOne(d => d.IdsegmentoNavigation)
                    .WithMany(p => p.UsuarioProyectos)
                    .HasForeignKey(d => d.Idsegmento)
                    .HasConstraintName("IDUSUSEG");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
