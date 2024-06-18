using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

//Referencias
using SistemaVenta.Entity;

namespace SistemaVenta.DAL.Interfaces
{
    public interface IVentaRepository : IGenericRepository<Venta> //La entidad estricta que utilizo es Venta
    {
        Task<Venta> Registrar(Venta entidad);
        Task<List<DetalleVenta>> ReporteVenta(DateTime FechaInicio, DateTime FechaFin);
        //Task<bool> Eliminar(Venta entidad);
        //Task<IQueryable<Venta>> Consultar(Expression<Func<Venta, bool>> filtro = null);
    }
}
