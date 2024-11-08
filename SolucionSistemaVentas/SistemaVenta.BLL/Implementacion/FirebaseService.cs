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
#pragma warning disable CS8602 // Desreferencia de una referencia posiblemente NULL.
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage"));
#pragma warning restore CS8602 // Desreferencia de una referencia posiblemente NULL.

#pragma warning disable CS8714 // El tipo no se puede usar como parámetro de tipo en el método o tipo genérico. La nulabilidad del argumento de tipo no coincide con la restricción "notnull"
#pragma warning disable CS8619 // La nulabilidad de los tipos de referencia del valor no coincide con el tipo de destino
#pragma warning disable CS8621 // La nulabilidad de los tipos de referencia del tipo de valor devuelto no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
                Dictionary<string, string> config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);
#pragma warning restore CS8621 // La nulabilidad de los tipos de referencia del tipo de valor devuelto no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
#pragma warning restore CS8619 // La nulabilidad de los tipos de referencia del valor no coincide con el tipo de destino
#pragma warning restore CS8714 // El tipo no se puede usar como parámetro de tipo en el método o tipo genérico. La nulabilidad del argumento de tipo no coincide con la restricción "notnull"

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
#pragma warning disable CS8602 // Desreferencia de una referencia posiblemente NULL.
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage"));
#pragma warning restore CS8602 // Desreferencia de una referencia posiblemente NULL.

#pragma warning disable CS8714 // El tipo no se puede usar como parámetro de tipo en el método o tipo genérico. La nulabilidad del argumento de tipo no coincide con la restricción "notnull"
#pragma warning disable CS8619 // La nulabilidad de los tipos de referencia del valor no coincide con el tipo de destino
#pragma warning disable CS8621 // La nulabilidad de los tipos de referencia del tipo de valor devuelto no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
                Dictionary<string, string> config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);
#pragma warning restore CS8621 // La nulabilidad de los tipos de referencia del tipo de valor devuelto no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
#pragma warning restore CS8619 // La nulabilidad de los tipos de referencia del valor no coincide con el tipo de destino
#pragma warning restore CS8714 // El tipo no se puede usar como parámetro de tipo en el método o tipo genérico. La nulabilidad del argumento de tipo no coincide con la restricción "notnull"

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
