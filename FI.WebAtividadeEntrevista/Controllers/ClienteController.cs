using FI.AtividadeEntrevista.BLL;
using FI.AtividadeEntrevista.DML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebAtividadeEntrevista.Models;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        private readonly BoBeneficiario _boBeneficiario = new BoBeneficiario();
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
            BoCPF boCPF = new BoCPF();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                if (bo.VerificarExistencia(model.CPF, 0))
                {
                    Response.StatusCode = 400;
                    return Json($"CPF({model.CPF}) já cadastrado.");
                }
                if (!boCPF.Validar(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json($"CPF({model.CPF}) inválido.");
                }

                model.Id = bo.Incluir(new Cliente()
                {
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF
                });

                try
                {
                    List<Beneficiario> beneficiarios = new List<Beneficiario>();
                    model?.Beneficiarios?.ForEach(x => beneficiarios.Add(new Beneficiario()
                    {
                        Id = x.Id,
                        Nome = x.Nome,
                        CPF = x.CPF,
                        IdCliente = model.Id
                    }));
                    _boBeneficiario.UpdateBeneficiarios(beneficiarios, model.Id);
                }
                catch (Exception ex)
                {
                    Response.StatusCode = 400;
                    return Json(ex.Message);
                }

                return Json("Cadastro efetuado com sucesso");
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
            BoCPF boCPF = new BoCPF();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }

            if (!boCPF.Validar(model.CPF))
            {
                Response.StatusCode = 400;
                return Json("CPF inválido.");
            }
            if (bo.VerificarExistencia(model.CPF, model.Id))
            {
                Response.StatusCode = 400;
                return Json($"CPF ({model.CPF}) já existe.");
            }

            bo.Alterar(new Cliente()
            {
                Id = model.Id,
                CEP = model.CEP,
                Cidade = model.Cidade,
                Email = model.Email,
                Estado = model.Estado,
                Logradouro = model.Logradouro,
                Nacionalidade = model.Nacionalidade,
                Nome = model.Nome,
                Sobrenome = model.Sobrenome,
                Telefone = model.Telefone,
                CPF = model.CPF
            });

            try
            {
                List<Beneficiario> beneficiarios = new List<Beneficiario>();
                model?.Beneficiarios?.ForEach(x => beneficiarios.Add(new Beneficiario()
                {
                    Id = x.Id,
                    Nome = x.Nome,
                    CPF = x.CPF,
                    IdCliente = model.Id
                }));
                _boBeneficiario.UpdateBeneficiarios(beneficiarios, model.Id);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                return Json(ex.Message);
            }

            return Json("Cadastro alterado com sucesso");

        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    CPF = cliente.CPF
                };
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}