using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable CS0105 // La directiva using apareció anteriormente en este espacio de nombre
using SistemaVenta.BLL.Interfaces;
#pragma warning restore CS0105 // La directiva using apareció anteriormente en este espacio de nombre
using SistemaVenta.DAL.Interfaces;
#pragma warning disable CS0105 // La directiva using apareció anteriormente en este espacio de nombre
using SistemaVenta.Entity;
#pragma warning restore CS0105 // La directiva using apareció anteriormente en este espacio de nombre


namespace SistemaVenta.BLL.Implementacion
{
    public class RolService : IRolService
    {
        private readonly IGenericRepository<Rol> _repositorio;

        public RolService(IGenericRepository<Rol> repositorio)
        {
            _repositorio = repositorio;
        }
        public async Task<List<Rol>> ListaRol()
        {
            IQueryable<Rol> query= await _repositorio.Consultar();
            return query.ToList();
        }
    }
}
