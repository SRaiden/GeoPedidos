using GeoPedidos.DAL.DBContext;
using GeoPedidos.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GeoPedidos.DAL.Implementacion
{
    public class PedidosRepository<TEntity> : IPedidoRepository<TEntity> where TEntity : class
    {
        private readonly GeoPedidosContext _dbcontext;

        public PedidosRepository(GeoPedidosContext dbcontext) // Constructor
        {
            _dbcontext = dbcontext;
        }

        public IQueryable<TEntity> Consultar(Expression<Func<TEntity, bool>> filtro = null)
        {
            IQueryable<TEntity> queryEntidad = filtro == null ? _dbcontext.Set<TEntity>() : _dbcontext.Set<TEntity>().Where(filtro);
            return queryEntidad;
        }
    }
}
