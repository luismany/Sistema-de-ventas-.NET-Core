﻿using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Implementacion
{
    public class NegocioService : INegocioService
    {
        private readonly IGenericRepository<Negocio> _repository;
        private readonly IFirebaseService _firebaseService;

        public NegocioService(IGenericRepository<Negocio> repository, IFirebaseService firebaseService)
        {
                _repository = repository;
            _firebaseService = firebaseService;
        }

        public async Task<Negocio> Obtener()
        {
            try
            {
                Negocio negocioEncontrado = await _repository.Obtener(n => n.IdNegocio == 1);
                return negocioEncontrado;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<Negocio> GuardarCambios(Negocio entidad, Stream logo = null, string NombreLogo = "")
        {
            try
            {
                Negocio negocioEncontrado = await _repository.Obtener(n => n.IdNegocio == 1);

                negocioEncontrado.NumeroDocumento = entidad.NumeroDocumento;
                negocioEncontrado.Nombre= entidad.Nombre;
                negocioEncontrado.Correo = entidad.Correo;
                negocioEncontrado.Direccion = entidad.Direccion;
                negocioEncontrado.Telefono = entidad.Telefono;
                negocioEncontrado.PorcentajeImpuesto=entidad.PorcentajeImpuesto;
                negocioEncontrado.SimboloMoneda=entidad.SimboloMoneda;

                negocioEncontrado.NombreLogo= negocioEncontrado.NombreLogo == ""? NombreLogo : negocioEncontrado.NombreLogo;

                if(logo != null)
                {
                    string urlLogo = await _firebaseService.SubirStorage(logo,"carpeta_logo", negocioEncontrado.NombreLogo);
                    negocioEncontrado.UrlLogo = urlLogo;
                }

                await _repository.Editar(negocioEncontrado);
                return negocioEncontrado;
                
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        
    }
}
