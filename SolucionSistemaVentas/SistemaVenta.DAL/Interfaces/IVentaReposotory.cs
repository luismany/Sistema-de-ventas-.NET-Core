using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.DAL.Interfaces
{
    public interface IVentaReposotory : IGenericRepository<Venta>
    {
        Task<Venta> Registrar(Venta entidad);
        Task<List<Venta>> Reporte(DateTime fechaInicio, DateTime fechaFin);
    }
}
