using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Referencias
using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity; //Para traer la clase Venta de la DB, así con todos

namespace SistemaVenta.DAL.Implementacion
{
    //Hereda también de IVentaRepository para usar los métodos plantilla
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {
        private readonly DBContext.DbventaContext _dbContext;

        //Este contexto lo envío al GenericRepository porque como obtenemos de una "base" de otra clase, entonces le mando ese contexto
        public VentaRepository(DBContext.DbventaContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Venta> Registrar(Venta entidad)
        {
            Venta ventaGenerada = new Venta();

            using(var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (DetalleVenta dv in entidad.DetalleVenta)
                    {
                        Producto productoEncontrado = _dbContext.Productos.Where(p => p.IdProducto == dv.IdProducto).FirstOrDefault();

                        productoEncontrado.Stock = productoEncontrado.Stock - dv.Cantidad; //Con esto disminuyo reduzco el stock de un producto por una venta

                        _dbContext.Productos.Update(productoEncontrado); //Actualizo la cantidad de productos
                    }
                    await _dbContext.SaveChangesAsync(); //Guardamos todos los cambios realizados

                    NumeroCorrelativo correlativo = _dbContext.NumeroCorrelativos.Where(n => n.Gestion == "venta").First();

                    //Genero un número del primero valor que hice desde SQL. Estos valores son de la tabla "NumeroCorrelativo"
                    correlativo.UltimoNumero = correlativo.UltimoNumero + 1; //Aumento 1 valor a la columna "último número" en la BD
                    correlativo.FechaActualizacion = DateTime.Now; //Actualiza la fecha del campo fechaActualización
                    //Esto con las clases generadas en SistemaVenta.Entity, almacenan los datos y los llevan a los valores de la BD

                    _dbContext.NumeroCorrelativos.Update(correlativo);
                    await _dbContext.SaveChangesAsync();

                    //Obtenemos el primer valor "000001" de la BD
                    string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value)); //Acá digo que: En ceros va a contener 6 veces 0, porque acá específicamos cuanto va a ser la cantidad
                    string numeroVenta = ceros + correlativo.UltimoNumero.ToString(); //Esto daría una suma de 7 digitos, ya que son seis 0 mas el último dígito
                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - correlativo.CantidadDigitos.Value, correlativo.CantidadDigitos.Value);

                    entidad.NumeroVenta = numeroVenta; //Va a ser igual al número de venta que generamos en las 3 líneas de arriba

                    await _dbContext.Venta.AddAsync(entidad); //entidad es todo el objeto de Venta
                    await _dbContext.SaveChangesAsync();

                    ventaGenerada = entidad;

                    transaction.Commit(); //Este commit, cuando pasen todas las operaciónes de arriba y no exista ningún error, llegará acá. Y las temporales pasaran a ser registros netos en las tablas
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return ventaGenerada;
        }

        //Listado del Detalle de una venta
        public async Task<List<DetalleVenta>> ReporteVenta(DateTime FechaInicio, DateTime FechaFin)
        {
            //El "Include()" sirve como un "JOIN" de SQL, entre las tablas
            List<DetalleVenta>listaResumen = await _dbContext.DetalleVenta
                .Include(v => v.IdVentaNavigation) //Este funciona para el _dbContext.DetalleVenta de arriba
                .ThenInclude(u => u.IdUsuarioNavigation) //Este  funciona para el Include de arriba
                .Include(v => v.IdVentaNavigation)
                //tdv: Tipo Detalle Venta
                .ThenInclude(tdv => tdv.IdTipoDocumentoVentaNavigation)
                .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= FechaInicio.Date &&
                    dv.IdVentaNavigation.FechaRegistro.Value.Date <= FechaFin.Date).ToListAsync();

            return listaResumen;
        }
    }
}
