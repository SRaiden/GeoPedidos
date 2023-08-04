using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GeoPedidos.BLL.Interfaces;
using GeoPedidos.DAL.Interfaces;
using GeoPedidos.Entity;

namespace GeoPedidos.BLL.Implementacion
{
    public class EmpresasServices : IEmpresasServices
    {
        private readonly IGenericRepository<GeneralEmpresa> _repository;
        private readonly IGenericRepository<GeneralSucursales> _sucursalRepository;

        public EmpresasServices(IGenericRepository<GeneralEmpresa> repository, IGenericRepository<GeneralSucursales> sucursalRepository)
        {
            _repository = repository;
            _sucursalRepository = sucursalRepository;
        }

        public async Task<List<GeneralEmpresa>> Lista()
        {
            IQueryable<GeneralEmpresa> query = await _repository.Consultar();
            return query.ToList();
        }

        public async Task<int> UltimoId()
        {
            var query =  await _repository.Consultar();
            return Int32.Parse(query.OrderByDescending(d => d.Id).First().Id.ToString());
        }

        public async Task<GeneralEmpresa> Crear(GeneralEmpresa entidad)
        {
            try
            {
                GeneralEmpresa empresaCreada = await _repository.Crear(entidad);
                if (empresaCreada.Id == 0)
                    throw new TaskCanceledException("No se pudo crear la empresa");

                return empresaCreada;
            }
            catch
            {
                throw;
            }
        }

        public async Task<GeneralEmpresa> Editar(GeneralEmpresa entidad)
        {
            try
            {
                GeneralEmpresa empresaEncontrada = await _repository.Obtener(c => c.Id == entidad.Id);
                empresaEncontrada.NombreEmpresa = entidad.NombreEmpresa;
                empresaEncontrada.Confirmar = entidad.Confirmar;
                empresaEncontrada.MailAvisoPedido = entidad.MailAvisoPedido;
                bool respuesta = await _repository.Editar(empresaEncontrada);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo editar la empresa");

                return empresaEncontrada;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int idEmpresa)
        {
            try
            {
                GeneralEmpresa empresaEncontrada = await _repository.Obtener(c => c.Id == idEmpresa);
                if (empresaEncontrada == null)
                    throw new TaskCanceledException("La empresa no existe");

                bool respuesta = await _repository.Eliminar(empresaEncontrada);
                return respuesta;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> BajaLogica(int idEmpresa)
        {
            try
            {
                GeneralEmpresa empresaEncontrada = await _repository.Obtener(c => c.Id == idEmpresa);
                empresaEncontrada.Estado = "inactivo";
                bool respuesta = await _repository.Editar(empresaEncontrada);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo dar de baja la Empresa");

                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> ObtenerNombreEmpresa(int id)
        {
            IQueryable<GeneralEmpresa> query = await _repository.Consultar(); // obtengo todas las empresas
            return query.Where(i => i.Id == id).First().NombreEmpresa.ToString();
        }

        public async Task<GeneralSucursales> ObtenerDatoEmpresa(int idSucursal)
        {
            IQueryable<GeneralSucursales> query = await _sucursalRepository.Consultar();
            return query.Where(i => i.Id == idSucursal).First();
        }
    }
}
