using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.V2.Controllers
{
    // api/v2.0/palavras

    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    // [Route("api/[controller]")]
    [ApiVersion("2.0")]
    public class PalavrasController : ControllerBase
    {
        /// <summary>
        /// Operação que retorna do banco de dados todas as palabras existentes
        /// </summary>
        /// <param name="query">Filtros de pesquisa</param>
        /// <returns>Listagem de palavras</returns>
        [HttpGet]
        public string ObterTodas()
        {
            return "Versão 2.0";
        }
    }
}
