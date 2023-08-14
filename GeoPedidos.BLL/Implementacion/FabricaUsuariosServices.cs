using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GeoPedidos.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;
using GeoPedidos.DAL.Interfaces;
using GeoPedidos.Entity;
using System.Security.AccessControl;

namespace GeoPedidos.BLL.Implementacion
{
    public class FabricaUsuariosServices : IFabricaUsuariosServices
    {
        private readonly IGenericRepository<FabricaUsuario> _repository;
        private readonly IUtilidadesServices _utilidadesServices;
        private readonly ICorreoServices _correoServices;

        public FabricaUsuariosServices(IGenericRepository<FabricaUsuario> repository,
                        IUtilidadesServices utilidadesServices, ICorreoServices correoServices)
        {
            _repository = repository;
            _utilidadesServices = utilidadesServices;
            _correoServices = correoServices;
        }

        public async Task<List<FabricaUsuario>> Lista(int idEmpresa = 0)
        {
            IQueryable<FabricaUsuario> query = await _repository.Consultar(); // Obtengo todos los usuarios
            if (idEmpresa == 0) // Soy SUPERADMIN
            {
                return query.ToList(); // retorno la tabla Rol y su tabla de detalleRol por medio del FK (include)
            }
            else { // SOY ADMIN DE SUCURSAL
                return query.Where(d => d.IdEmpresa == idEmpresa).ToList(); // retorno la tabla Rol y su tabla de detalleRol por medio del FK (include)
            }
        }

        public async Task<FabricaUsuario> ObtenerCredenciales(string correo, string clave)
        {
            FabricaUsuario usuarioEncontrado = await _repository.Obtener(u => u.Email.Equals(correo) && u.Contraseña.Equals(clave));

            return usuarioEncontrado;
        }

        public async Task<FabricaUsuario> ObtenerId(int IdUsuario)
        {
            IQueryable<FabricaUsuario> query = await _repository.Consultar(u => u.Id == IdUsuario);

            FabricaUsuario resultado = query.FirstOrDefault();

            return resultado;
        }

        public async Task<bool> GuardarPerfil(FabricaUsuario entidad)
        {
            try
            {
                FabricaUsuario usuarioEncontrado = await _repository.Obtener(u => u.Id == entidad.Id);

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El usuario no existe");

                usuarioEncontrado.Nombre = entidad.Nombre;
                usuarioEncontrado.Apellido = entidad.Apellido;

                bool respuesta = await _repository.Editar(usuarioEncontrado);

                return respuesta;
            }
            catch
            {
                throw;
            }
        } // IMPLEMENTAR

        public async Task<bool> CambiarClave(int IdUsuario, string claveActual, string claveNueva)
        {
            try
            {
                FabricaUsuario usuarioEncontrado = await _repository.Obtener(u => u.Id == IdUsuario);

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El usuario no existe");

                // validamos si ingreso correctamente la clave actual
                if (usuarioEncontrado.Contraseña != claveActual)
                    throw new TaskCanceledException("La clave actual es incorrecta");

                usuarioEncontrado.Contraseña = claveNueva;

                bool respuesta = await _repository.Editar(usuarioEncontrado);

                return respuesta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> RestablecerClave(string CorreoDestino, string URLPlantillaCorreo)
        {
            try
            {
                FabricaUsuario usuarioEncontrado = await _repository.Obtener(u => u.Email == CorreoDestino);

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("No existe un usuario con este correo");

                string claveGenerada = _utilidadesServices.GenerarClave();
                usuarioEncontrado.Contraseña = claveGenerada;

                //---//
                URLPlantillaCorreo = URLPlantillaCorreo.Replace("[clave]", claveGenerada);
                string htmlCorreo = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URLPlantillaCorreo);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // Se pudo conectar?
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader readStream = null;

                        // Usa caracteres especiales??
                        if (response.CharacterSet == null)
                            readStream = new StreamReader(dataStream);
                        else
                            readStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));

                        // Limpiar memoria
                        htmlCorreo = readStream.ReadToEnd();
                        response.Close();
                        readStream.Close();
                    }
                }

                bool correoEnviado = false;

                if (htmlCorreo != "")
                    correoEnviado = await _correoServices.EnviarCorreo(CorreoDestino, "Contraseña Restablecida", htmlCorreo);

                if (!correoEnviado)
                    throw new TaskCanceledException("No se pudo enviar el correo. Intente mas tarde");

                bool respuesta = await _repository.Editar(usuarioEncontrado);
                return respuesta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> UltimoId()
        {
            var query = await _repository.Consultar();
            return Int32.Parse(query.OrderByDescending(d => d.Id).First().Id.ToString());
        }

        //------------------------------------------------------------------------------------------//


        public async Task<FabricaUsuario> Crear(FabricaUsuario entidad)
        {
            try
            {
                FabricaUsuario usuarioCreado = await _repository.Crear(entidad);
                if (usuarioCreado.Id == 0)
                    throw new TaskCanceledException("No se pudo crear el usuario");

                return usuarioCreado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<FabricaUsuario> Editar(FabricaUsuario entidad)
        {
            try
            {
                FabricaUsuario usuarioEncontrado = await _repository.Obtener(c => c.Id == entidad.Id);
                usuarioEncontrado.Nombre = entidad.Nombre;
                usuarioEncontrado.Apellido = entidad.Apellido;
                usuarioEncontrado.Email = entidad.Email;

                usuarioEncontrado.IdEmpresa = entidad.IdEmpresa;
                usuarioEncontrado.IdSucursal = entidad.IdSucursal;

                usuarioEncontrado.Active = entidad.Active;
                usuarioEncontrado.Rol = entidad.Rol;
                usuarioEncontrado.Contraseña = entidad.Contraseña;

                bool respuesta = await _repository.Editar(usuarioEncontrado);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo editar el usuario");

                return usuarioEncontrado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int idUsuario)
        {
            try
            {
                FabricaUsuario usuarioEncontrado = await _repository.Obtener(c => c.Id == idUsuario);
                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("E usuario no existe");

                bool respuesta = await _repository.Eliminar(usuarioEncontrado);
                return respuesta;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> BajaLogica(int idUsuario)
        {
            try
            {
                FabricaUsuario usuarioEncontrado = await _repository.Obtener(c => c.Id == idUsuario);
                usuarioEncontrado.Active = false;
                bool respuesta = await _repository.Editar(usuarioEncontrado);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo dar de baja al Usuario");

                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<FabricaUsuario> DatoUsuario(int idUsuario)
        {
            IQueryable<FabricaUsuario> query = await _repository.Consultar(); 
            return query.Where(u => u.Id == idUsuario).First(); 
        }

    }
}
