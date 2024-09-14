using Proyectogestionhoras.Models;
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


        public async Task<bool> RegistrarUsuario(string nombre, string rut, string telefono, string email, int idrol, string nombrerecurso, int numhoras, decimal costounitario, int idsegmento)
        {
            try
            {
                string primerosCuatroDigitosRut = rut.Length >= 4 ? rut.Substring(0, 4) : rut;

                string passencrypted = EncriptarContrasena(primerosCuatroDigitosRut);
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using(DbCommand command = connection.CreateCommand())
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
                    command.Parameters.Add(new SqlParameter("@NUMERO_HORAS", numhoras));
                    command.Parameters.Add(new SqlParameter("@COSTO_UNITARIO", costounitario));
                    command.Parameters.Add(new SqlParameter("@IDSEGMENTOCOSTO", idsegmento));
                    await command.ExecuteNonQueryAsync();
                    await conexion.CloseDatabaseConnectionAsync();
                }
                return true;
            }
            catch (Exception ex) {

                Debug.WriteLine($"Hubo un error al registrar el usuario"+ex.Message);
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
                                Nombre_Recurso = reader.GetString(reader.GetOrdinal("NOMBRE_RECURSO")),
                                Numero_Horas = reader.GetInt32(reader.GetOrdinal("NUMERO_HORAS")),
                                Costo_Unitario = reader.GetDecimal(reader.GetOrdinal("COSTO_UNITARIO")) ,
                                Rol = reader.GetString(reader.GetOrdinal("ROL")) 
                                

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
                Debug.WriteLine($"Hubo un error al obtener los roles:"+ex.Message);
                return new List<Rol>();
            }
        }
        public async Task<List<SegmentoCosto>> ObtenerSegmentoCosto()
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
    }
}
