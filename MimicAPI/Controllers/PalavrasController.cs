using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Repositories.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")]
    public class PalavrasController : ControllerBase
    {
        private readonly IPalavraRepository _repository;

        public PalavrasController(IPalavraRepository repositorio)
        {
            _repository = repositorio;
        }

        // -- /api/palavras?data=2019-05-01
        [Route("")]
        [HttpGet]
        public ActionResult ObterTodas([FromQuery] PalavraUrlQuery query)
        {
            var item = _repository.ObterPalavras(query);

            if (query.PagNumero > item.Paginacao.TotalPaginas)
            {
                return NotFound();
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(item.Paginacao));

            return Ok(item.ToList()); // Método do ControllerBase que converte para o modelo mais popular. No caso, JsonResult
        }

        // -- /api/palavras/1
        [Route("{id}")]
        [HttpGet]
        public ActionResult Obter(int id)
        {
            var obj = _repository.Obter(id);

            if (obj == null)
            {
                // return StatusCode(404);
                return NotFound();
            }
            return Ok(obj);
        }

        // -- /api/palavras (POST: id, nome, ...)
        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar([FromBody] Palavra palavra)
        {
            _repository.Cadastrar(palavra);

            return Created($"/api/palavras/{palavra.Id}", palavra);
        }

        // -- /api/palavras/1 (PUT: id, nome, ...)
        [Route("{id}")]
        [HttpPut]
        public ActionResult Atualizar(int id, [FromBody] Palavra palavra)
        {
            var obj = _repository.Obter(id);
            
            if (obj == null)
            {
                // return StatusCode(404);
                return NotFound();
            }

            palavra.Id = id;
            _repository.Atualizar(palavra);            

            return Ok();
        }

        // -- /api/palavras/1 (DELETE)
        [Route("{id}")]
        [HttpDelete]
        public ActionResult Deletar(int id)
        {
            var palavra = _repository.Obter(id);

            if (palavra == null)
            {
                // return StatusCode(404);
                return NotFound();
            }

            _repository.Deletar(id);

            return NoContent();
        }
    }
}
