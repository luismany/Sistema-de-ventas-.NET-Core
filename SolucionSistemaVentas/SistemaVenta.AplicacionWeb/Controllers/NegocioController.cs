using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class NegocioController : Controller
    {
        private readonly IMapper _mapper;
        private readonly INegocioService _negocioService;
        public NegocioController(IMapper mapper, INegocioService negocioService)
        {
                _mapper = mapper;
            _negocioService = negocioService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            GenericResponse<VMNegocio> gResponse = new GenericResponse<VMNegocio>();
            try
            {
                
                VMNegocio vMNegocio = _mapper.Map<VMNegocio>( await _negocioService.Obtener());
                gResponse.Estado= true;
                gResponse.Objeto= vMNegocio;

            }
            catch (Exception ex)
            {

                gResponse.Estado = false;
                gResponse.Mensaje= ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
        [HttpPost]
        public async Task<IActionResult> GuardarCambios([FromForm]IFormFile logo, [FromForm]string modelo)
        {

            GenericResponse<VMNegocio> gResponse = new GenericResponse<VMNegocio>();

            try
            {
                VMNegocio vMNegocio = JsonConvert.DeserializeObject<VMNegocio>(modelo);

                string nombreLogo = "";
                Stream logoStream= null;

                if(logo!=null) { 
                
                    string nombreEnCodigo=Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(logo.FileName);
                    nombreLogo = string.Concat(nombreEnCodigo,extension);
                    logoStream= logo.OpenReadStream();

                }

                Negocio NegocioEditado = await _negocioService.GuardarCambios( _mapper.Map<Negocio>(vMNegocio),logoStream,nombreLogo);

                vMNegocio =  _mapper.Map<VMNegocio>(NegocioEditado);
                gResponse.Estado=true;
                gResponse.Objeto = vMNegocio;
            }
            catch (Exception ex)
            {

                gResponse.Estado = false;
                gResponse.Mensaje= ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
