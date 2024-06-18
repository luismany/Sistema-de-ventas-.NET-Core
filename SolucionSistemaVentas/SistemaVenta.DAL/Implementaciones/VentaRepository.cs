using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.Entity;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Interfaces;

namespace SistemaVenta.DAL.Implementaciones
{
    
    public class VentaRepository : GenericRepository<Venta>, IVentaReposotory
    {
        private readonly DbventaContext _dbContext;
        public VentaRepository(DbventaContext dbContext): base(dbContext)
        {

            _dbContext = dbContext;

        }
        public async Task<Venta> Registrar(Venta entidad)
        {
            Venta ventaGenerada= new Venta();


            using( var Transaction= _dbContext.Database.BeginTransaction() )
            {
                try
                {
                    foreach (DetalleVenta dv in entidad.DetalleVenta)
                    {
                        Producto productoEncontrado = _dbContext.Productos.Where(p => p.IdProducto == dv.IdProducto).First();

                        productoEncontrado.Stock=productoEncontrado.Stock - dv.Cantidad;
                        _dbContext.Productos.Update(productoEncontrado);

                    }
                    await _dbContext.SaveChangesAsync();

                    NumeroCorrelativo correlativo = _dbContext.NumeroCorrelativos.Where(n => n.Gestion == "venta").First();

                    correlativo.UltimoNumero=correlativo.UltimoNumero + 1;
                    correlativo.FechaActualizacion=DateTime.Now;

                    _dbContext.NumeroCorrelativos.Update(correlativo);
                    await _dbContext.SaveChangesAsync();

                    string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value));
                    string numeroVenta= ceros + correlativo.UltimoNumero.ToString();
                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - correlativo.CantidadDigitos.Value, correlativo.CantidadDigitos.Value);
                    entidad.NumeroVenta=numeroVenta;

                    await _dbContext.Venta.AddAsync(entidad);
                    await _dbContext.SaveChangesAsync();

                    ventaGenerada = entidad;
                    Transaction.Commit();

                }catch (Exception ex)
                {
                    Transaction.Rollback(); 
                    throw ex;
                }
            }

            return ventaGenerada;
        }

        public async Task<List<DetalleVenta>> Reporte(DateTime fechaInicio, DateTime fechaFin)
        {
            List<DetalleVenta> listaResumen = await _dbContext.DetalleVenta
                .Include(v=> v.IdVentaNavigation)
                .ThenInclude(u=> u.IdUsuarioNavigation)
                .Include(v=> v.IdVentaNavigation)
                .ThenInclude(tdv=> tdv.IdTipoDocumentoVentaNavigation)
                .Where(dv=> dv.IdVentaNavigation.FechaRegistro.Value.Date >= fechaInicio.Date &&
                dv.IdVentaNavigation.FechaRegistro.Value.Date <= fechaFin.Date).ToListAsync();

            return listaResumen;
        }
    }
}
