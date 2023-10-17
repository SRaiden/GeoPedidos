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
                // Llamamos a la tabla Configuracion y traemos todos los registros de la columna Recurso que se llamen Servicio_Correo
                //Queryable<Configuracion> query = await _repository.Consultar();
                // Creamos un diccionario que guardara 2 elementos, la columna Propiedad y Valor
                //Dictionary<string, string> config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                ////Creamos el cuerpo del mensaje
                var credenciales = new NetworkCredential("noresponder@geosoft-web.com.ar", "geoweb_nores*");
                //var correo = new MailMessage()
                //{
                //    From = new MailAddress("noresponder@geosoft-web.com.ar"),
                //    Subject = Asunto,
                //    Body = Mensaje,
                //    IsBodyHtml = true
                //};

                //// Creamos el tipo de envio
                //correo.To.Add(new MailAddress(CorreoDestino));
                //var clienteServidor = new SmtpClient()
                //{
                //    Host = "geosoft-web.com.ar",
                //    Port = Int32.Parse("465"),
                //    Credentials = credenciales,
                //    DeliveryMethod = SmtpDeliveryMethod.Network,
                //    UseDefaultCredentials = false,
                //    EnableSsl = true
                //};

                //--------------------------------------------------------------------------//

                SmtpClient smtp2 = new SmtpClient("geosoft-web.com.ar", 465)
                {
                    EnableSsl = true,
                    Credentials = credenciales,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                };

                MailMessage email = new MailMessage();
                email.From = new MailAddress("noresponder@geosoft-web.com.ar");
                email.To.Add(new MailAddress(CorreoDestino));
                email.Subject = Asunto;
                email.Body = Mensaje;
                email.IsBodyHtml = true;

                smtp2.Send(email);
                smtp2.Dispose();

                // Enviamos mensaje
                //clienteServidor.Send(correo);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
