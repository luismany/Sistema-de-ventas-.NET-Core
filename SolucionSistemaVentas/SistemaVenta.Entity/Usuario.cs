﻿using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string? Nombre { get; set; }

    public string? Correo { get; set; }

    public string? Telefono { get; set; }

    public int? IdRol { get; set; }

    public int? NombreRol { get; set; }
    public string? UrlFoto { get; set; }

    public int EsActivo { get; set; }
}
