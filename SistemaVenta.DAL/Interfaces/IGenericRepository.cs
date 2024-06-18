using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;

namespace SistemaVenta.DAL.Interfaces
{
    //Este TEntity va a ser una clase
    public interface IGenericRepository<TEntity> where TEntity : class //Este TEntity va a ser una clase
    {
        //Creamos un método asíncrono. Ponemos una expresión que va a ser una función donde mandamos la entidad.
        //Este necesita una respuesta de tipo booleano, y toda la expresión de la línea se va a llamar filtro
        Task<TEntity> Obtener(Expression<Func<TEntity, bool>> filtro);
        Task<TEntity> Crear(TEntity entidad);
        Task<bool> Editar(TEntity entidad);
        Task<bool> Eliminar(TEntity entidad);
        Task<IQueryable<TEntity>> Consultar(Expression<Func<TEntity, bool>> filtro = null);
    }
}
