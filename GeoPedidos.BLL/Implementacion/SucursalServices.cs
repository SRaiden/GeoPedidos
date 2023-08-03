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
    public class SucursalServices : ISucursalServices
    {
        private readonly IGenericRepository<GeneralSucursales> _repository;

        public SucursalServices(IGenericRepository<GeneralSucursales> repository)
        {
            _repository = repository;
        }

        public async Task<List<GeneralSucursales>> Lista(int idEmpresa)
        {
            IQueryable<GeneralSucursales> query = await _repository.Consultar();
            return query.Where(i => i.EmpresaId == idEmpresa).ToList();
        }

        public async Task<GeneralSucursales> ObtenerDatosSucursal(int id)
        {
            IQueryable<GeneralSucursales> query = await _repository.Consultar(); // obtengo todas las sucursales
            return query.Where(i => i.Id == id).First();
        }

        public async Task<string> ObtenerNombreSucursal(int id)
        {
            IQueryable<GeneralSucursales> query = await _repository.Consultar(); // obtengo todas las sucursales
            return query.Where(i => i.Id == id).First().NombreSucursal.ToString();
        }
    }
}
