using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using System.Net;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _repositorio;
        private readonly IFirebaseService _firebaseService;
        private readonly ICorreoService _correoService;
        private readonly IUtilidadesService _utilidadesService;
        public UsuarioService(IGenericRepository<Usuario> repositorio,
            IFirebaseService firebaseService,
            ICorreoService correoService,
            IUtilidadesService utilidadesService
            )
        {
            _repositorio = repositorio;
            _firebaseService = firebaseService;
            _correoService = correoService; 
            _utilidadesService = utilidadesService;

        }


        public async Task<List<Usuario>> Listar()
        {
            IQueryable<Usuario> query = await _repositorio.Consultar();
            return query.Include(rol=> rol.IdRolNavigation).ToList();
        }


        public async Task<Usuario> Crear(Usuario entidad, Stream foto = null, string nombreFoto = "", string UrlPlantillaCorreo = "")
        {
            //validamos si el usuario existe
            Usuario usuarioExiste= await _repositorio.Obtener(u=> u.Correo == entidad.Correo);
            if(usuarioExiste!=null) 
            throw new TaskCanceledException("El correo ya existe");
            

            try
            {
                string claveGenerada = _utilidadesService.GenerarClave();
                entidad.Clave = _utilidadesService.ConvertirSha256(claveGenerada);
                entidad.NombreFoto= nombreFoto;

                if(foto!=null)
                {
                    string urlFoto = await _firebaseService.SubirStorage(foto,"cartepa_usuario", nombreFoto);
                    entidad.UrlFoto= urlFoto;

                }
                Usuario usuarioCreado = await _repositorio.Crear(entidad);
                if (usuarioCreado.IdUsuario == 0)
                    throw new TaskCanceledException("No se pudo crear el usuario");

                if (UrlPlantillaCorreo != "")
                {
                    UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[correo]", usuarioCreado.Correo).Replace("[clave]",claveGenerada);

                    string htmlCorreo = "";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream dataStream= response.GetResponseStream())
                        {
                            StreamReader readerStream= null;

                            if (response.CharacterSet== null)
                             readerStream= new StreamReader(dataStream);
                            else
                                readerStream= new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));

                            htmlCorreo = readerStream.ReadToEnd();
                            response.Close();
                            readerStream.Close();
                        }
                    }

                    if (htmlCorreo != "")
                        await _correoService.EnviarCorreo(usuarioCreado.Correo,"Cuenta Creada",htmlCorreo);

                }

                IQueryable<Usuario> query = await _repositorio.Consultar(u => u.IdUsuario == usuarioCreado.IdUsuario);
                usuarioCreado = query.Include(rol=> rol.IdRolNavigation).First();

                return usuarioCreado;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<Usuario> Editar(Usuario entidad, Stream foto = null, string nombreFoto = "")
        {
            Usuario usuarioExiste = await _repositorio.Obtener(u => u.Correo == entidad.Correo && u.IdUsuario != entidad.IdUsuario);
            if (usuarioExiste != null)
                throw new TaskCanceledException("El correo ya existe");

            try
            {
                IQueryable<Usuario> queryUsuario = await _repositorio.Consultar(u=> u.IdUsuario == entidad.IdUsuario);

                Usuario usuarioEditar= queryUsuario.First();

                usuarioEditar.Nombre=entidad.Nombre;
                usuarioEditar.Correo=entidad.Correo;
                usuarioEditar.Telefono=entidad.Telefono;
                usuarioEditar.IdRol=entidad.IdRol;

                if (usuarioEditar.NombreFoto == "")
                    usuarioEditar.NombreFoto = nombreFoto;
                if (foto != null)
                {
                    string urlFoto = await _firebaseService.SubirStorage(foto,"carpeta_usuario",usuarioEditar.NombreFoto);
                    usuarioEditar.UrlFoto = urlFoto;
                }

                bool respuesta = await _repositorio.Editar(usuarioEditar);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo modificar el Usuario");

                Usuario usuarioEditado = queryUsuario.Include(rol=> rol.IdRolNavigation).First();
                return usuarioEditado;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<bool> Eliminar(int idUsuario)
        {
            throw new NotImplementedException();
        }
        public Task<Usuario> ObtenerPorCredenciales(string correo, string clave)
        {
            throw new NotImplementedException();
        }
        public Task<Usuario> ObtenentPorId(int idUsuario)
        {
            throw new NotImplementedException();
        }
        public Task<bool> GuardarPerfil(Usuario entidad)
        {
            throw new NotImplementedException();
        }
        public Task<bool> CambiarClave(int idUsuario, string claveActual, string nuevaClave)
        {
            throw new NotImplementedException();
        }
        public Task<bool> RestablecerClave(string correo, string UrlPlantillaCorreo)
        {
            throw new NotImplementedException();
        }
    }
}
