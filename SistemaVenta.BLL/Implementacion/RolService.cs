using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Referencias
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class RolService : IRolService
    {
        private readonly IGenericRepository<Rol> _repositorio; //Hago un repositorio para la clase de Rol

        //Constructor
        public RolService(IGenericRepository<Rol> repositorio)
        {
            _repositorio = repositorio;
        }

        //Devuelve la lista de errores
        public async Task<List<Rol>> Lista()
        {
            IQueryable<Rol> query = await _repositorio.Consultar();

            return query.ToList();
        }
    }
}
