using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GeoPedidos.DAL.Interfaces
{
    public interface IPedidoRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Consultar(Expression<Func<TEntity, bool>> filtro = null);
    }
}
