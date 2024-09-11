using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PROYECTOGESTORPROYECTOS.Models;

public partial class ProyectoControlHorasContext : DbContext
{
    public ProyectoControlHorasContext()
    {
    }

    public ProyectoControlHorasContext(DbContextOptions<ProyectoControlHorasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ccosto> Ccostos { get; set; }

    public virtual DbSet<CcostoUnegocio> CcostoUnegocios { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Contacto> Contactos { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<FacturaProyecto> FacturaProyectos { get; set; }

    public virtual DbSet<Gasto> Gastos { get; set; }

    public virtual DbSet<Planilla> Planillas { get; set; }

    public virtual DbSet<PlanillaUsusarioProyecto> PlanillaUsusarioProyectos { get; set; }

    public virtual DbSet<Presupuesto> Presupuestos { get; set; }

    public virtual DbSet<Proyecto> Proyectos { get; set; }

    public virtual DbSet<ProyectoGasto> ProyectoGastos { get; set; }

    public virtual DbSet<Recurso> Recursos { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<SegmentoCosto> SegmentoCostos { get; set; }

    public virtual DbSet<Tipologium> Tipologia { get; set; }

    public virtual DbSet<Unegocio> Unegocios { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<UsuarioProyecto> UsuarioProyectos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-DR8BPEV\\SQLEXPRESS;DataBase=PROYECTO_CONTROL_HORAS;Integrated Security=true;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ccosto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CCOSTO__3214EC27F673DC77");

            entity.ToTable("CCOSTO");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.TipoCcosto)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("TIPO_CCOSTO");
        });

        modelBuilder.Entity<CcostoUnegocio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CCOSTO_U__3214EC27208D9176");

            entity.ToTable("CCOSTO_UNEGOCIO");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CODIGO");
            entity.Property(e => e.IdCcosto).HasColumnName("ID_CCOSTO");
            entity.Property(e => e.IdUnegocio).HasColumnName("ID_UNEGOCIO");

            entity.HasOne(d => d.IdCcostoNavigation).WithMany(p => p.CcostoUnegocios)
                .HasForeignKey(d => d.IdCcosto)
                .HasConstraintName("ID_CCOSTO_FK");

            entity.HasOne(d => d.IdUnegocioNavigation).WithMany(p => p.CcostoUnegocios)
                .HasForeignKey(d => d.IdUnegocio)
                .HasConstraintName("ID_UNEGOCIO_FK");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CLIENTE__3214EC27BBED25C2");

            entity.ToTable("CLIENTE");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Ciudad)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("CIUDAD");
            entity.Property(e => e.Direccion)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("DIRECCION");
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
            entity.HasKey(e => e.Id).HasName("PK__CONTACTO__3214EC2771F8DA19");

            entity.ToTable("CONTACTO");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Cargo)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("CARGO");
            entity.Property(e => e.Desde).HasColumnName("DESDE");
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Hasta).HasColumnName("HASTA");
            entity.Property(e => e.IdCliente).HasColumnName("ID_CLIENTE");
            entity.Property(e => e.Nombre)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("TELEFONO");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Contactos)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ID_CLIENTE_FK");
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EMPRESA__3214EC2767FCB857");

            entity.ToTable("EMPRESA");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.TipoEmpresa)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("TIPO_EMPRESA");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FACTURA__3214EC274D938A89");

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
            entity.HasKey(e => e.Id).HasName("PK__FACTURA___3214EC275F6BC516");

            entity.ToTable("FACTURA_PROYECTO");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IdFactura).HasColumnName("ID_FACTURA");
            entity.Property(e => e.IdProyecto).HasColumnName("ID_PROYECTO");

            entity.HasOne(d => d.IdFacturaNavigation).WithMany(p => p.FacturaProyectos)
                .HasForeignKey(d => d.IdFactura)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ID_FACTURA_FK");

            entity.HasOne(d => d.IdProyectoNavigation).WithMany(p => p.FacturaProyectos)
                .HasForeignKey(d => d.IdProyecto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ID_PROYECTOFACTURA_FK");
        });

        modelBuilder.Entity<Gasto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GASTOS__3214EC27C4D2CE8D");

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
            entity.HasKey(e => e.Id).HasName("PK__PLANILLA__3214EC27F912022D");

            entity.ToTable("PLANILLA");

            entity.Property(e => e.Id).HasColumnName("ID");
        });

        modelBuilder.Entity<PlanillaUsusarioProyecto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PLANILLA__3214EC272952E1EC");

            entity.ToTable("PLANILLA_USUSARIO_PROYECTO");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IdPlanilla).HasColumnName("ID_PLANILLA");
            entity.Property(e => e.IdUsuProy).HasColumnName("ID_USU_PROY");

            entity.HasOne(d => d.IdPlanillaNavigation).WithMany(p => p.PlanillaUsusarioProyectos)
                .HasForeignKey(d => d.IdPlanilla)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ID_PLANILLA_FK");

            entity.HasOne(d => d.IdUsuProyNavigation).WithMany(p => p.PlanillaUsusarioProyectos)
                .HasForeignKey(d => d.IdUsuProy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ID_USU_PRO_FK");
        });

        modelBuilder.Entity<Presupuesto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PRESUPUE__3214EC277FB1DA53");

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
            entity.HasKey(e => e.Id).HasName("PK__PROYECTO__3214EC2792AFEFFD");

            entity.ToTable("PROYECTO");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Fecha).HasColumnName("FECHA");
            entity.Property(e => e.FechaInicio).HasColumnName("FECHA_INICIO");
            entity.Property(e => e.FechaTermino).HasColumnName("FECHA_TERMINO");
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

            entity.HasOne(d => d.IdCcostoUnegocioNavigation).WithMany(p => p.Proyectos)
                .HasForeignKey(d => d.IdCcostoUnegocio)
                .HasConstraintName("ID_CCOSTO_UNEGOCIO_FK");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Proyectos)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ID_CLIENTE_PROYECTO_FK");

            entity.HasOne(d => d.IdPresupuestoNavigation).WithMany(p => p.Proyectos)
                .HasForeignKey(d => d.IdPresupuesto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ID_PRESUPUESTO_FK");

            entity.HasOne(d => d.IdTipologiaNavigation).WithMany(p => p.Proyectos)
                .HasForeignKey(d => d.IdTipologia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ID_TIPOLOGIA_FK");

            entity.HasOne(d => d.TipoEmpresaNavigation).WithMany(p => p.Proyectos)
                .HasForeignKey(d => d.TipoEmpresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TIPO_EMPRESA_FK");
        });

        modelBuilder.Entity<ProyectoGasto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PROYECTO__3214EC27222A2196");

            entity.ToTable("PROYECTO_GASTOS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IdGastos).HasColumnName("ID_GASTOS");
            entity.Property(e => e.IdProyecto).HasColumnName("ID_PROYECTO");

            entity.HasOne(d => d.IdGastosNavigation).WithMany(p => p.ProyectoGastos)
                .HasForeignKey(d => d.IdGastos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ID_GASTOS_FK");

            entity.HasOne(d => d.IdProyectoNavigation).WithMany(p => p.ProyectoGastos)
                .HasForeignKey(d => d.IdProyecto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ID_PRO_GASTOS_IDFK");
        });

        modelBuilder.Entity<Recurso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RECURSO__3214EC278A11C13B");

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

            entity.HasOne(d => d.IdSegmentocostosNavigation).WithMany(p => p.Recursos)
                .HasForeignKey(d => d.IdSegmentocostos)
                .HasConstraintName("ID_SEGMENTO_COSTOS_FK");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ROL__3214EC27146523C2");

            entity.ToTable("ROL");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
        });

        modelBuilder.Entity<SegmentoCosto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SEGMENTO__3214EC270D3BC9B0");

            entity.ToTable("SEGMENTO_COSTOS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Cuenta)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("CUENTA");
            entity.Property(e => e.IdCuenta).HasColumnName("ID_CUENTA");
            entity.Property(e => e.IdHonorariosExternos).HasColumnName("ID_HONORARIOS_EXTERNOS");
        });

        modelBuilder.Entity<Tipologium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TIPOLOGI__3214EC2714965733");

            entity.ToTable("TIPOLOGIA");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.TipoTipologia)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("TIPO_TIPOLOGIA");
        });

        modelBuilder.Entity<Unegocio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UNEGOCIO__3214EC27F04B30F5");

            entity.ToTable("UNEGOCIO");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.TipoUnegocio)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("TIPO_UNEGOCIO");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__USUARIO__3214EC272D9C947F");

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

            entity.HasOne(d => d.IdRecursoNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRecurso)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RECURSO_CK");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ID_ROL_FK");
        });

        modelBuilder.Entity<UsuarioProyecto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__USUARIO___3214EC2732AF7905");

            entity.ToTable("USUARIO_PROYECTO");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.HhSemanales).HasColumnName("HH_SEMANALES");
            entity.Property(e => e.IdProyecto).HasColumnName("ID_PROYECTO");
            entity.Property(e => e.IdUsuario).HasColumnName("ID_USUARIO");

            entity.HasOne(d => d.IdProyectoNavigation).WithMany(p => p.UsuarioProyectos)
                .HasForeignKey(d => d.IdProyecto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("USUARIO_PROYECTO_IDPROYECTO_FK");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UsuarioProyectos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("USUARIO_PROYECTO_IDUSUARIO_FK");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
