using FI.AtividadeEntrevista.BLL;
using FI.AtividadeEntrevista.DML;
using FI.WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace FI.WebAtividadeEntrevista.Controllers
{
    public class BeneficiarioController : Controller
    {
        [HttpGet]
        public JsonResult Get(long id)
        {
            try
            {

                List<Beneficiario> beneficiarios = new BoBeneficiario().Listar(id);
                List<BeneficiarioModel> model = new List<BeneficiarioModel>();
                beneficiarios.ForEach(beneficiario => model.Add(new BeneficiarioModel()
                {
                    Id = beneficiario.Id,
                    CPF = beneficiario.CPF,
                    Nome = beneficiario.Nome,
                    IdCliente = beneficiario.IdCliente
                }));

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}
