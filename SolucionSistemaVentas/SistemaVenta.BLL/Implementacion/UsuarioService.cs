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
                usuarioEditar.EsActivo=entidad.EsActivo;

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

        public async Task<bool> Eliminar(int idUsuario)
        {
            try
            {
                Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.IdUsuario == idUsuario);

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("usuario no existe");


                string nombreFoto = usuarioEncontrado.NombreFoto;

                bool respuesta = await _repositorio.Eliminar(usuarioEncontrado);

                if (respuesta)

                    await _firebaseService.EliminarStorage("carpeta_usuario", nombreFoto);


                return true;

            }
            catch (Exception)
            {

                throw;
            }
           

        }
        public async Task<Usuario> ObtenerPorCredenciales(string correo, string clave)
        {

            string claveEncriptada =  _utilidadesService.ConvertirSha256(clave);

            Usuario usuarioEncontrado = await _repositorio.Obtener(u=> u.Correo == correo && u.Clave == claveEncriptada);

             return usuarioEncontrado;

        }
        public async Task<Usuario> ObtenentPorId(int idUsuario)
        {

            IQueryable<Usuario> query = await _repositorio.Consultar(u=> u.IdUsuario == idUsuario);


            Usuario resultado = query.Include(rol => rol.IdRolNavigation).FirstOrDefault();



            return resultado;

        }
        public async Task<bool> GuardarPerfil(Usuario entidad)
        {
            try
            {
                Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.IdUsuario == entidad.IdUsuario);

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("usuario no existe");

                bool respuesta = await _repositorio.Editar(usuarioEncontrado);

                return respuesta;
            }
            catch (Exception)
            {

                throw;
            }
            

        }
        public async Task<bool> CambiarClave(int idUsuario, string claveActual, string nuevaClave)
        {
            try
            {
                Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.IdUsuario == idUsuario);

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("Usuario no existe");

                if (usuarioEncontrado.Clave != _utilidadesService.ConvertirSha256(claveActual))
                    throw new TaskCanceledException("la clave actual no coincide");

                usuarioEncontrado.Clave = _utilidadesService.ConvertirSha256(nuevaClave);

                bool resultado = await _repositorio.Editar(usuarioEncontrado);
                return resultado;
            }
            catch (Exception)
            {

                throw;
            }
           
        }
        public async Task<bool> RestablecerClave(string correo, string UrlPlantillaCorreo)
        {
            try
            {
                Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.Correo == correo);

                if (usuarioEncontrado== null)
                    throw new TaskCanceledException("No se encontro ningun usuario asociado a este correo");

                string claveGenerada = _utilidadesService.GenerarClave();
                usuarioEncontrado.Clave = _utilidadesService.ConvertirSha256(claveGenerada);

                UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[clave]", claveGenerada);

                string htmlCorreo = "";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {

                        StreamReader readerStream = null;


                        if (response.CharacterSet == null)
                            readerStream = new StreamReader(dataStream);
                        else
                            readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));

                        htmlCorreo = readerStream.ReadToEnd();
                        response.Close();
                        readerStream.Close();
                    }
                }

                bool correoEnviado = false;

                if (htmlCorreo != "")
                  correoEnviado = await _correoService.EnviarCorreo(correo, "Contraseña Reestablecida", htmlCorreo);

                if (!correoEnviado)
                    throw new TaskCanceledException("ha ocurrido un error intenta de nuevo mas tarde");

                bool respuesta = await _repositorio.Editar(usuarioEncontrado);
                return respuesta;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
