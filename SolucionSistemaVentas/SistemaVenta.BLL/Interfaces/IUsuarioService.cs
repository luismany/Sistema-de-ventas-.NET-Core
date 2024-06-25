﻿using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IUsuarioService
    {
        Task<List<Usuario>> Listar();
        Task<Usuario> Crear(Usuario entidad,Stream foto= null, string nombreFoto="",string UrlPlantillaCorreo="");
        Task<Usuario> Editar(Usuario entidad, Stream foto = null, string nombreFoto = "");
        Task<bool> Eliminar(int idUsuario);
        Task<Usuario> ObtenerPorCredenciales(string correo, string clave);
        Task<Usuario> ObtenentPorId(int idUsuario);
        Task<bool> GuardarPerfil(Usuario entidad);
        Task<bool> CambiarClave(int idUsuario, string claveActual, string nuevaClave);
        Task<bool> RestablecerClave(string correo, string UrlPlantillaCorreo);




    }
}
