using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces; 
using SistemaVenta.Entity;
using Newtonsoft.Json; 

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioService _usuarioService;
        private readonly IRolService _rolService;
        public UsuarioController(IMapper mapper, IUsuarioService usuarioService, IRolService rolService)
        {
            _mapper = mapper;
            _usuarioService = usuarioService;
            _rolService = rolService;

        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaRol()
        {
            // var lista= await _rolService.ListaRol();
            // convertimos la lista en una vmlistarol
            // List<VMRol> vmListaRoles = _mapper.Map<List<VMRol>>(lista);

            List<VMRol> vmListaRoles = _mapper.Map<List<VMRol>>(await _rolService.ListaRol());
            return StatusCode(StatusCodes.Status200OK, vmListaRoles);
        }
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMUsuario> vmListaUsuario = _mapper.Map<List<VMUsuario>>(await _usuarioService.Listar());
            return StatusCode(StatusCodes.Status200OK, new { data = vmListaUsuario } );
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMUsuario> genericResponse= new GenericResponse<VMUsuario>();

            try
            {
                VMUsuario vmUsuario= JsonConvert.DeserializeObject<VMUsuario>(modelo);

                string nombreFoto = "";
                Stream fotoStream= null;

                if (foto != null)
                {
                    string nombreEnCodigo= Guid.NewGuid().ToString("N");
                    string extencion = Path.GetExtension(foto.FileName);
                    nombreFoto= string.Concat(nombreEnCodigo, extencion);
                    fotoStream= foto.OpenReadStream();
                }

                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";

                Usuario usuarioCreado = await _usuarioService.Crear(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto, urlPlantillaCorreo);

                vmUsuario= _mapper.Map<VMUsuario>(usuarioCreado);

                genericResponse.Estado = true; 
                genericResponse.Objeto= vmUsuario;

                
            }
            catch (Exception ex)
            {

                genericResponse.Estado = false;
                genericResponse.Mensaje= ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);

        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMUsuario> genericResponse = new GenericResponse<VMUsuario>();

            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null)
                {
                    string nombreEnCodigo = Guid.NewGuid().ToString("N");
                    string extencion = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombreEnCodigo, extencion);
                    fotoStream = foto.OpenReadStream();
                }

                Usuario usuarioEditado = await _usuarioService.Editar(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto);

                vmUsuario = _mapper.Map<VMUsuario>(usuarioEditado);

                genericResponse.Estado = true;
                genericResponse.Objeto = vmUsuario;


            }
            catch (Exception ex)
            {

                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);

        }
        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idUsuario)
        {
            GenericResponse<string> genericResponse = new GenericResponse<string>();

            try
            {
                genericResponse.Estado = await _usuarioService.Eliminar(idUsuario);
            }
            catch (Exception ex)
            {

                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message; 
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }
    }
}
