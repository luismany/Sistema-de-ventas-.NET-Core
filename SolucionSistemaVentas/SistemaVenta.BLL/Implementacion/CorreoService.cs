using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
    

namespace SistemaVenta.BLL.Implementacion
{
    public class CorreoService : ICorreoService
    {
        private readonly IGenericRepository<Configuracion> _repositorio;
        public CorreoService(IGenericRepository<Configuracion> repositorio) 
        {
            _repositorio = repositorio;
        }
        public async Task<bool> EnviarCorreo(string destinatario, string asunto, string mensaje)
        {
#pragma warning disable CS8602 // Desreferencia de una referencia posiblemente NULL.
            IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Servicio_Correo"));
#pragma warning restore CS8602 // Desreferencia de una referencia posiblemente NULL.

#pragma warning disable CS8619 // La nulabilidad de los tipos de referencia del valor no coincide con el tipo de destino
#pragma warning disable CS8621 // La nulabilidad de los tipos de referencia del tipo de valor devuelto no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
#pragma warning disable CS8714 // El tipo no se puede usar como parámetro de tipo en el método o tipo genérico. La nulabilidad del argumento de tipo no coincide con la restricción "notnull"
            Dictionary<string, string> config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);
#pragma warning restore CS8714 // El tipo no se puede usar como parámetro de tipo en el método o tipo genérico. La nulabilidad del argumento de tipo no coincide con la restricción "notnull"
#pragma warning restore CS8621 // La nulabilidad de los tipos de referencia del tipo de valor devuelto no coincide con el delegado de destino (posiblemente debido a los atributos de nulabilidad).
#pragma warning restore CS8619 // La nulabilidad de los tipos de referencia del valor no coincide con el tipo de destino

            var credenciales = new NetworkCredential(config["correo"], config["clave"]);
            var correo = new MailMessage()
            {
                From = new MailAddress(config["correo"], config["alias"]),
                Subject = asunto,
                Body = mensaje,
                IsBodyHtml = true
            };

            correo.To.Add(new MailAddress(destinatario));

            var clienteServidor = new SmtpClient()
            {
                Host = config["host"],
                Port = int.Parse(config["puerto"]),
                Credentials= credenciales,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = true
            };

            clienteServidor.Send(correo);
            return true;   
        }
    }
}
