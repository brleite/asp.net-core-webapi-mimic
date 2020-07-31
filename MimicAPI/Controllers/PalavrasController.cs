using Microsoft.AspNetCore.Mvc;
using MimicAPI.Database;
using MimicAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Controllers
{
    public class PalavrasController : ControllerBase
    {
        private readonly MimicContext _banco;

        public PalavrasController(MimicContext banco)
        {
            _banco = banco;
        }

        public ActionResult ObterTodas()
        {
            return Ok(_banco.Palavras); // Método do ControllerBase que converte para o modelo mais popular. No caso, JsonResult
        }

        public ActionResult Obter(int id)
        {
            return Ok(_banco.Palavras.Find(id));
        }

        public ActionResult Cadastrar(Palavra palavra)
        {
            _banco.Palavras.Add(palavra);

            return Ok();
        }

        public ActionResult Atualizar(Palavra palavra)
        {
            _banco.Palavras.Update(palavra);

            return Ok();
        }

        public ActionResult Deletar(int id)
        {
            _banco.Palavras.Remove(_banco.Palavras.Find(id));

            return Ok();
        }
    }
}
