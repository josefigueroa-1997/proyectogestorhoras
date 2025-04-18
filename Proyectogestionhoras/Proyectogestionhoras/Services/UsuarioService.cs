﻿using Proyectogestionhoras.Models;
using BCrypt;
using Proyectogestionhoras.Services.Interface;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Data.Common;
using System.Data;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Proyectogestionhoras.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using iText.Commons.Actions.Contexts;

namespace Proyectogestionhoras.Services
{
    public class UsuarioService : IUsuario
    {
        private readonly Conexion conexion;
        private readonly PROYECTO_CONTROL_HORASContext context;
        public UsuarioService(Conexion conexion, PROYECTO_CONTROL_HORASContext context)
        {

            this.conexion = conexion;
            this.context = context;
        }


        public async Task<bool> RegistrarUsuario(string nombre, string rut, string telefono, string email, int idrol, string nombrerecurso, int? numhoras, decimal costounitario, float? porcentajehoras, DateTime? fechainicio, DateTime? fechafin)
        {
            try
            {
                string primerosCuatroDigitosRut = rut.Length >= 4 ? rut.Substring(0, 4) : rut;
#pragma warning disable CS8600
                object numhorasparameter = (object)numhoras ?? 0;
                object porcentajeparameter = (object)porcentajehoras ?? DBNull.Value;
                object fechainicioparamater = (object)fechainicio ?? DBNull.Value;
                object fechafinparamater = (object)fechafin ?? DBNull.Value;
#pragma warning restore CS8600
                string passencrypted = EncriptarContrasena(primerosCuatroDigitosRut);
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "INGRESAR_USUARIO";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@NOMBRE", nombre));
                    command.Parameters.Add(new SqlParameter("@NOMBRE_USUARIO", rut));
                    command.Parameters.Add(new SqlParameter("@CONTRASENA", passencrypted));
                    command.Parameters.Add(new SqlParameter("@TELEFONO", telefono));
                    command.Parameters.Add(new SqlParameter("@EMAIL", email));
                    command.Parameters.Add(new SqlParameter("@ID_ROL", idrol));
                    command.Parameters.Add(new SqlParameter("@NOMBRE_RECURSO", nombrerecurso));
                    command.Parameters.Add(new SqlParameter("@NUMERO_HORAS_SEMANALES", numhorasparameter));
                    command.Parameters.Add(new SqlParameter("@COSTO_UNITARIO", costounitario));
                    command.Parameters.Add(new SqlParameter("@PORCENTAJEHORAS", porcentajeparameter));
                    command.Parameters.Add(new SqlParameter("@FECHAINICIO", fechainicioparamater));
                    command.Parameters.Add(new SqlParameter("@FECHAFIN", fechafinparamater));
                    await command.ExecuteNonQueryAsync();
                    await conexion.CloseDatabaseConnectionAsync();
                }
                int idusuario = ultimoidusuario();
                await AgregarProyectosUsuario(idusuario);
                //EnviarCorreo(email);
                return true;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al registrar el usuario" + ex.Message);
                return false;

            }
        }

        public int ultimoidusuario()
        {
            var ultimousuario = context.Usuarios
                                         .OrderByDescending(p => p.Id)
                                         .FirstOrDefault();


            if (ultimousuario != null)
            {
                return ultimousuario.Id;
            }


            return 0;
        }

        public async Task<bool> EditarUsuario(int idusuario, string nombre, string nombreusuario, string telefono, string email, int? hhsemanales, decimal costo, float? porcentaje, DateTime? fechainicio, DateTime? fechatermino, string nombrerecurso, string estado)
        {
            try
            {
#pragma warning disable CS8600
                object numhorasparameter = (object)hhsemanales ?? 0;
                object porcentajeparameter = (object)porcentaje ?? DBNull.Value;
                object fechainicioparamater = (object)fechainicio ?? DBNull.Value;
                object fechafinparamater = (object)fechatermino ?? DBNull.Value;
#pragma warning restore CS8600
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "EDITARUSUARIO";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDUSUARIO", idusuario));
                    command.Parameters.Add(new SqlParameter("@NOMBRE", nombre));
                    command.Parameters.Add(new SqlParameter("@NOMBRE_USUARIO", nombreusuario));
                    command.Parameters.Add(new SqlParameter("@TELEFONO", telefono));
                    command.Parameters.Add(new SqlParameter("@EMAIL", email));
                    command.Parameters.Add(new SqlParameter("@NUMERO_HORAS_SEMANALES", numhorasparameter));
                    command.Parameters.Add(new SqlParameter("@COSTO_UNITARIO", costo));
                    command.Parameters.Add(new SqlParameter("@PORCENTAJEHORAS", porcentajeparameter));
                    command.Parameters.Add(new SqlParameter("@FECHAINICIO", fechainicioparamater));
                    command.Parameters.Add(new SqlParameter("@FECHAFIN", fechafinparamater));
                    command.Parameters.Add(new SqlParameter("@NOMBRE_RECURSO", nombrerecurso));
                    command.Parameters.Add(new SqlParameter("@ESTADO", estado));
                    await command.ExecuteNonQueryAsync();
                    await conexion.CloseDatabaseConnectionAsync();
                }
                await AgregarProyectosUsuario(idusuario);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Hubo un error al editar el usuario:{e.Message}");
                return false;
            }

        }


        public async Task<bool> CambiarContrasena(Usuario usuario)
        {
            try
            {
                if (usuario == null)
                {
                    return false;
                }

                var user = await context.Usuarios.FindAsync(usuario.Id);
                if (user == null)
                {
                    return false;
                }
                string contraencriptada = EncriptarContrasena(usuario.Contrasena);
                user.Contrasena = contraencriptada;
                context.Usuarios.Update(user);
                await context.SaveChangesAsync();
                return true;

            }
            catch (Exception e)
            {
                Debug.WriteLine($"Hubo un error al cambiar la contraseña:{e.Message}");
                return false;
            }
        }


        public async Task<List<UsuarioDTO>> ObtenerUusario(int? id, string? nombre, int? id_recurso)
        {
            try
            {
#pragma warning disable CS8600
                object idparameter = (object)id ?? DBNull.Value;
                object nombreparameter = (object)nombre ?? DBNull.Value;
                object idrecursoparameter = (object)id_recurso ?? DBNull.Value;
#pragma warning restore CS8600
                var usuarios = new List<UsuarioDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "BUSCAR_USUARIO";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ID", idparameter));
                    command.Parameters.Add(new SqlParameter("@NOMBRE", nombreparameter));
                    command.Parameters.Add(new SqlParameter("@ID_RECURSO", idrecursoparameter));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            UsuarioDTO usuario = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                Nombre = reader.GetString(reader.GetOrdinal("NOMBRE")),
                                Nombre_Usuario = reader.GetString(reader.GetOrdinal("NOMBRE_USUARIO")),
                                Telefono = reader.GetString(reader.GetOrdinal("TELEFONO")),
                                Email = reader.GetString(reader.GetOrdinal("EMAIL")),
                                Estado = reader.GetString(reader.GetOrdinal("ESTADO")),
                                NOMBRE_RECURSO_CORRELATIVO = reader.GetString(reader.GetOrdinal("NOMBRE_RECURSO_CORRELATIVO")),
                                RolRecurso = reader.GetString(reader.GetOrdinal("RolRecurso")),
                                Numero_Horas = reader.GetInt32(reader.GetOrdinal("NUMERO_HORAS")),
                                Costo_Unitario = reader.GetDecimal(reader.GetOrdinal("COSTO_UNITARIO")),
                                Rol = reader.GetString(reader.GetOrdinal("ROL")),
                                HH_MENSUALES = reader.IsDBNull(reader.GetOrdinal("HH_MENSUALES")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HH_MENSUALES")),
                                ID_RECURSO = reader.GetInt32(reader.GetOrdinal("ID_RECURSO")),
                                HH_ANUALES = reader.IsDBNull(reader.GetOrdinal("HH_ANUALES")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HH_ANUALES")),
                                PROCENTAJE_PROYECTO = reader.IsDBNull(reader.GetOrdinal("PROCENTAJE_PROYECTO")) ? 0 : reader.GetDecimal(reader.GetOrdinal("PROCENTAJE_PROYECTO")),
                                Desde = reader.IsDBNull(reader.GetOrdinal("DESDE"))
                                ? (DateTime?)null
    : reader.GetDateTime(reader.GetOrdinal("DESDE")),
                                Hasta = reader.IsDBNull(reader.GetOrdinal("HASTA"))
                                ? (DateTime?)null
    : reader.GetDateTime(reader.GetOrdinal("HASTA")),
                                Tipo_Consultor = reader.IsDBNull(reader.GetOrdinal("TIPO_CONSULTOR")) ? null : reader.GetString(reader.GetOrdinal("TIPO_CONSULTOR")),
                            };
                            usuarios.Add(usuario);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return usuarios;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener los clientes:" + ex.Message);
                return new List<UsuarioDTO>();

            }
        }



        public async Task<bool> Login(string rut, string contrasena)
        {
            try
            {
                if (!string.IsNullOrEmpty(rut) && !string.IsNullOrEmpty(contrasena))
                {

                    Login credenciales = new Login();
                    credenciales = await ObtenerCredenciales(rut);
                    if (credenciales != null)
                    {
                        if (credenciales.Contraseña != null)
                        {
                            bool validar = VerificarContrasena(contrasena, credenciales.Contraseña);
                            if (validar)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
                return false;

            }
            catch (Exception e)
            {
                Debug.WriteLine($"hubo un error al iniciar sesión" + e.Message);
                return false;
            }
        }

        public async Task<Login> ObtenerCredenciales(string rut)
        {
            try
            {
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "INICIOSESION";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@CORREO", rut));
                    Login usuario = new Login();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            usuario = new Login()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                Nombre = reader.GetString(reader.GetOrdinal("NOMBRE")),
                                Rol = reader.GetInt32(reader.GetOrdinal("ROL")),
                                Contraseña = reader.GetString(reader.GetOrdinal("CONTRASEÑA")),
                                Recurso = reader.GetString(reader.GetOrdinal("RECURSO")),
                            };
                        }
                    }
                    await conexion.CloseDatabaseConnectionAsync();
                    return usuario;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new Login();
            }
        }
        private bool VerificarContrasena(string passuser, string passbd)
        {
            try
            {
                bool verificar = BCrypt.Net.BCrypt.Verify(passuser, passbd);
                return verificar;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error al verificar las contraseñas:{e.Message}");
                return false;
            }
        }

        public async Task<List<Rol>> ObtenerRoles()
        {
            try
            {
                var roles = new List<Rol>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERROLES";
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Rol rol = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                Nombre = reader.GetString(reader.GetOrdinal("NOMBRE")),

                            };
                            roles.Add(rol);
                            Debug.WriteLine(rol.Nombre);
                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return roles;

            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine($"Error de operación inválida: {ex.Message}");
                return new List<Rol>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener los roles:" + ex.Message);
                return new List<Rol>();
            }
        }
        /*public async Task<List<SegmentoCosto>> ObtenerSegmentoCosto()
        {
            try
            {
                var segmentos = new List<SegmentoCosto>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERSEGEMENTOCOSTO";
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            SegmentoCosto segmento = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                Cuenta = reader.GetString(reader.GetOrdinal("CUENTA")),

                            };
                            segmentos.Add(segmento);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return segmentos;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener los roles:" + ex.Message);
                return new List<SegmentoCosto>();
            }
        }
       */
        private string EncriptarContrasena(string contrasena)
        {
            try
            {
                string salt = BCrypt.Net.BCrypt.GenerateSalt();
                string constrasenaencriptada = BCrypt.Net.BCrypt.HashPassword(contrasena, salt);
                return constrasenaencriptada;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error al encriptar la contraseña:{e.Message}");
                return "";
            }
        }

        public async Task<int> VerificarCorreo(string email)
        {
            int resultado = 2;
            DbConnection connection = await conexion.OpenDatabaseConnectionAsync();

            if (connection == null)
            {
                throw new InvalidOperationException("No se pudo abrir la conexión a la base de datos.");
            }

            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM USUARIO WHERE EMAIL = @CORREO";
                DbParameter correoParam = command.CreateParameter();
                correoParam.ParameterName = "@CORREO";
                correoParam.Value = email ?? string.Empty;
                command.Parameters.Add(correoParam);

                try
                {
                    resultado = (int)await command.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al ejecutar el comando.", ex);
                }
            }

            return resultado > 0 ? 1 : 2;
        }

        public async Task<int> Verificarrut(string rut)
        {
            try
            {
                int resultado = 2;
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "VERIFICARRUTUSUARIO";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@RUTCLIENTE", rut));
                    resultado = (int)await command.ExecuteScalarAsync();
                }

                return resultado > 0 ? 1 : 2;

            }
            catch (Exception e)
            {
                Debug.WriteLine($"Hubo un error al validar la existencia del idcliente:" + e.Message);
                return 0;
            }
        }

        public async Task<List<UsuarioProyectoDTO>> ObtenerUsuariosProyecto(int idusuario)
        {
            try
            {

                var usuarios = new List<UsuarioProyectoDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "ObtenerProyectosPorUsuario  ";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDUSUARIO", idusuario));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            UsuarioProyectoDTO usuario = new()
                            {

                                ID = reader.IsDBNull(reader.GetOrdinal("ID_PROYECTO")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID_PROYECTO")),
                                NUMPROYECTO = reader.IsDBNull(reader.GetOrdinal("NUM_PROYECTO")) ? null : reader.GetString(reader.GetOrdinal("NUM_PROYECTO")),
                                NOMBREPROYECTO = reader.IsDBNull(reader.GetOrdinal("Nombre_Proyecto")) ? null : reader.GetString(reader.GetOrdinal("Nombre_Proyecto")),
                                NOMBRECLIENTE = reader.IsDBNull(reader.GetOrdinal("NOMBRECLIENTE")) ? null : reader.GetString(reader.GetOrdinal("NOMBRECLIENTE"))



                            };
                            usuarios.Add(usuario);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return usuarios;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener los clientes:" + ex.Message);
                return new List<UsuarioProyectoDTO>();

            }
        }



        public async Task<List<HH_AsignaciónDTO>> RecuperarHHUsuarios(int idusuario, int idproyecto)
        {
            try
            {

                var hhusuario = new List<HH_AsignaciónDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "HH_USUARIO_PROYECTO";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDUSUARIO", idusuario));
                    command.Parameters.Add(new SqlParameter("@IDPROYECTO", idproyecto));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            HH_AsignaciónDTO hhusuarios = new()
                            {
                                IDUSUARIOPROYECTO = reader.IsDBNull(reader.GetOrdinal("IDUSUARIOPROYCTO")) ? 0 : reader.GetInt32(reader.GetOrdinal("IDUSUARIOPROYCTO")),
                                IDUSUARIO = reader.IsDBNull(reader.GetOrdinal("ID_USUARIO")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID_USUARIO")),
                                RECURSO = reader.IsDBNull(reader.GetOrdinal("RECURSO")) ? null : reader.GetString(reader.GetOrdinal("RECURSO")),
                                NOMBREUSUARIO = reader.IsDBNull(reader.GetOrdinal("NOMBRE_USUARIO")) ? null : reader.GetString(reader.GetOrdinal("NOMBRE_USUARIO")),
                                IDPROYECTO = reader.IsDBNull(reader.GetOrdinal("ID_PROYECTO")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID_PROYECTO")),
                                HHASIGNADAS = reader.IsDBNull(reader.GetOrdinal("HHASIGNADAS")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HHASIGNADAS")),
                                TIPOCONSULTOR = reader.IsDBNull(reader.GetOrdinal("TIPO_CONSULTOR")) ? null : reader.GetString(reader.GetOrdinal("TIPO_CONSULTOR")),

                            };
                            hhusuario.Add(hhusuarios);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return hhusuario;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener los hh usuarios:" + ex.Message);
                return new List<HH_AsignaciónDTO>();

            }
        }


        public async Task<List<StockHorasDTO>> ConsultaStockHoras(int idusuario)
        {
            try
            {

                var stock = new List<StockHorasDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "CONSULTA_HH_USUARIOS";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDUSUARIO", idusuario));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            StockHorasDTO hh = new()
                            {
                                hhasignadas = reader.IsDBNull(reader.GetOrdinal("HHASIGNADAS")) ? 0 : reader.GetInt32(reader.GetOrdinal("HHASIGNADAS")),
                                hhanuales = reader.IsDBNull(reader.GetOrdinal("HHANUALES")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HHANUALES")),


                            };
                            stock.Add(hh);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return stock;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener los stock horas:" + ex.Message);
                return new List<StockHorasDTO>();

            }
        }

        public async Task GestorHHSocios()
        {
            var hhsocios = await context.Recursos.Where(r => r.Estado == "Activo" && r.NombreRecurso == "Socio" && r.Desde <= DateTime.Now && r.Hasta >= DateTime.Now).SumAsync(r => r.HhAnuales.GetValueOrDefault());
           
            var socios = await context.TotalRecursos.Where(s => s.Anio == DateTime.Now.Year && s.TipoRecurso == "Socio").FirstOrDefaultAsync();
           
            if (socios == null)
            {
                socios = new TotalRecurso
                {
                    Anio = DateTime.Now.Year,
                    TipoRecurso = "Socio",
                    Totalinmodificable = context.TotalRecursos
                        .Where(t => t.Anio == DateTime.Now.Year - 1 && t.TipoRecurso == "Socio")
                        .Select(t => t.Totalinmodificable)
                        .FirstOrDefault() ?? 3000,

                    TotalHhAnuales = context.TotalRecursos
                        .Where(t => t.Anio == DateTime.Now.Year - 1 && t.TipoRecurso == "Socio")
                        .Select(t => (decimal)t.Totalinmodificable)
                        .FirstOrDefault()
                };


                context.TotalRecursos.Add(socios);
                await context.SaveChangesAsync();
            }


            var hhsociosocupadas = await (
                from p in context.Proyectos
                join h in context.HhUsuarioHistorials on p.Id equals h.IdProyecto
                where p.Fechaejecucion.HasValue && p.Fechaejecucion.Value.Year == DateTime.Now.Year
                select h.HhSocios
                ).SumAsync();


            socios.Totalinmodificable = hhsocios;

            socios.TotalHhAnuales = ((decimal)hhsocios - (decimal)hhsociosocupadas);
            if (socios.TotalHhAnuales < 0)
            {
                socios.TotalHhAnuales = 0;
            }
            await context.SaveChangesAsync();

        }

        public async Task GestorHHStaff()
        {
            var hhstaff = await context.Recursos.Where(r => r.Estado == "Activo" && r.NombreRecurso == "Staff" && r.Desde <= DateTime.Now && r.Hasta >= DateTime.Now).SumAsync(r => r.HhAnuales.GetValueOrDefault());

            var staff = await context.TotalRecursos.Where(s => s.Anio == DateTime.Now.Year && s.TipoRecurso == "Staff").FirstOrDefaultAsync();
            if (staff == null)
            {
                staff = new TotalRecurso
                {
                    Anio = DateTime.Now.Year,
                    TipoRecurso = "Staff",
                    Totalinmodificable = context.TotalRecursos
                        .Where(t => t.Anio == DateTime.Now.Year - 1 && t.TipoRecurso == "Staff")
                        .Select(t => t.Totalinmodificable)
                        .FirstOrDefault() ?? 3000,

                    TotalHhAnuales = context.TotalRecursos
                        .Where(t => t.Anio == DateTime.Now.Year - 1 && t.TipoRecurso == "Staff")
                        .Select(t => (decimal)t.Totalinmodificable)
                        .FirstOrDefault()
                };


                context.TotalRecursos.Add(staff);
                await context.SaveChangesAsync();
            }


            var hhstaffocupadas = await (
                from p in context.Proyectos
                join h in context.HhUsuarioHistorials on p.Id equals h.IdProyecto
                where p.Fechaejecucion.HasValue && p.Fechaejecucion.Value.Year == DateTime.Now.Year
                select h.HhStaff
                ).SumAsync();


            staff.Totalinmodificable = hhstaff;

            staff.TotalHhAnuales = ((decimal)hhstaff - (decimal)hhstaffocupadas);
            if (staff.TotalHhAnuales < 0)
            {
                staff.TotalHhAnuales = 0;
            }
            await context.SaveChangesAsync();

        }

        public async Task AgregarProyectosUsuario(int idusuario)
        {
            try
            {
                int hhsocios = 0;
                int hhstaff = 0;

                var nombreRecurso = await context.Usuarios
                    .Where(u => u.Id == idusuario)
                    .Select(u => u.IdRecursoNavigation.NombreRecurso)
                    .FirstOrDefaultAsync();

                if (nombreRecurso == "Socio")
                {
                    hhsocios = 5;
                }
                else if (nombreRecurso == "Staff")
                {
                    hhstaff = 5;
                }

                var proyectos = await context.Proyectos
                    .Where(p => p.StatusProyecto == 1 || p.StatusProyecto == 2)
                    .ToListAsync();

                foreach (var proyecto in proyectos)
                {
                    var relacionExistente = await context.UsuarioProyectos
                        .FirstOrDefaultAsync(up => up.IdProyecto == proyecto.Id && up.IdUsuario == idusuario);

                    if (relacionExistente != null)
                    {
                        
                        relacionExistente.HhSocios = hhsocios;
                        relacionExistente.HhStaff = hhstaff;
                        context.UsuarioProyectos.Update(relacionExistente);
                    }
                    else
                    {
                      
                        var nuevaRelacion = new UsuarioProyecto
                        {
                            IdProyecto = proyecto.Id,
                            IdUsuario = idusuario,
                            HhSocios = hhsocios,
                            HhStaff = hhstaff
                        };

                        await context.UsuarioProyectos.AddAsync(nuevaRelacion);
                    }
                }

                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Hubo un error al agregar o editar proyectos para el usuario: {e.Message}");
            }
        }


        private void EnviarCorreo(string email)
        {
            using (MailMessage mensaje = new MailMessage())
            {
                mensaje.From = new MailAddress("plataformaproyectosunit@gmail.com");
                mensaje.To.Add(email);
                mensaje.Subject = "Credenciales de Ingreso a la Plataforma de UNIT Proyectos";
                mensaje.IsBodyHtml = true;

                // Cuerpo del correo con la imagen
                string imagenUrl = "wwwroot/images/logo.PNG"; // Cambia por la URL correcta de la imagen
                string body = $@"
            <p>Estimado usuario,</p>
            <p>Las credenciales para ingresar a la plataforma de Unit Proyectos son las siguientes:</p>
            <p><strong>Nombre de usuario:</strong> su RUT</p>
            <p><strong>Contraseña:</strong> Los 4 primeros dígitos de su RUT</p>
            <br />
            <p>Se despide atentamente,</p>
            <p><strong>Soporte Plataforma Unit Proyectos</strong></p>
            <br />
            <p><small>Por favor, no responder a este correo.</small></p>
            <p><img src='{imagenUrl}' alt='Imagen de ejemplo'></p>
        ";

                mensaje.Body = body;

                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential("plataformaproyectosunit@gmail.com", "Unit2024.");
                    smtpClient.EnableSsl = true;

                    try
                    {
                        smtpClient.Send(mensaje);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }
            }
        }


    }
}
