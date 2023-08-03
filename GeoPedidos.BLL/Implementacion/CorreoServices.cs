using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Mail;
using GeoPedidos.BLL.Interfaces;
using GeoPedidos.DAL.Interfaces;
using GeoPedidos.Entity;
using Microsoft.Extensions.Configuration;

namespace GeoPedidos.BLL.Implementacion
{
    public class CorreoServices : ICorreoServices
    {
        private readonly IGenericRepository<Configuracion> _repository;

        public CorreoServices(IGenericRepository<Configuracion> repository)
        {
            _repository = repository;
        }

        public async Task<bool> EnviarCorreo(string CorreoDestino, string Asunto, string Mensaje)
        {
            try
            {
                // Llamamos a la tabla Consultar y traemos todos los registros de la columna Recurso que se llamen Servicio_Correo
                IQueryable<Configuracion> query = await _repository.Consultar(c => c.Recurso.Equals("Servicio_Correo"));
                // Creamos un diccionario que guardara 2 elementos, la columna Propiedad y Valor
                Dictionary<string, string> config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                // Creamos el cuerpo del mensaje
                var credenciales = new NetworkCredential(config["correo"], config["clave"]);
                var correo = new MailMessage()
                {
                    From = new MailAddress(config["correo"], config["alias"]),
                    Subject = Asunto,
                    Body = Mensaje,
                    IsBodyHtml = true
                };

                // Creamos el tipo de envio
                correo.To.Add(new MailAddress(CorreoDestino));
                var clienteServidor = new SmtpClient()
                {
                    Host = config["host"],
                    Port = Int32.Parse(config["puerto"]),
                    Credentials = credenciales,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = true
                };

                // Enviamos mensaje
                clienteServidor.Send(correo);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
