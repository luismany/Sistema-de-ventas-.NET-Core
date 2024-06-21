using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Firebase.Auth;
using Firebase.Storage;
using Microsoft.Win32.SafeHandles;





namespace SistemaVenta.BLL.Implementacion
{
    public class FirebaseService : IFirebaseService
    {
        private readonly IGenericRepository<Configuracion> _repositorio;
        public FirebaseService(IGenericRepository<Configuracion> repositorio) 
        {
            _repositorio = repositorio;
        }
        public async Task<string> SubirStorage(Stream streamArchivo, string carpetaDestino, string nombreArchivo)
        {
            string UrlImagen = "";

            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage"));

                Dictionary<string, string> config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var auth = new FirebaseAuthProvider(new FirebaseConfig(config["api_key"]));
                var a = await auth.CreateUserWithEmailAndPasswordAsync(config["email"], config["clave"]);

                var cancelation = new CancellationTokenSource();

                var task = new FirebaseStorage(

                    config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(config[carpetaDestino])
                    .Child(config[nombreArchivo])
                    .PutAsync(streamArchivo, cancelation.Token);

                UrlImagen = await task;

            }
            catch (Exception)
            {

                UrlImagen = "";
            }

            return UrlImagen;
           
        }
        public async Task<bool> EliminarStorage(string carpetaDestino, string nombreArchivo)
        {
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage"));

                Dictionary<string, string> config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var auth = new FirebaseAuthProvider(new FirebaseConfig(config["api_key"]));
                var a = await auth.CreateUserWithEmailAndPasswordAsync(config["email"], config["clave"]);

                var cancelation = new CancellationTokenSource();

                var task = new FirebaseStorage(

                    config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(config[carpetaDestino])
                    .Child(config[nombreArchivo])
                    .DeleteAsync();

                 await task;

                return true;

            }
            catch (Exception)
            {

                return  false;
            }

            
        }

        
    }
}
