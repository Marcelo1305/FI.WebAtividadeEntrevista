using FI.AtividadeEntrevista.DAL;
using FI.AtividadeEntrevista.DML;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FI.AtividadeEntrevista.BLL
{
    public class BoBeneficiario
    {
        private readonly BLL.BoCPF _boCPF = new BoCPF();
        private readonly DAL.DaoBeneficiario _beneficiario = new DaoBeneficiario();

        /// <summary>
        /// Inclui um novo beneficiario
        /// </summary>
        /// <param name="cliente">Objeto de beneficiario</param>
        public long Incluir(DML.Beneficiario beneficiario)
        {
            return _beneficiario.Incluir(beneficiario);
        }

        /// <summary>
        /// Altera um beneficiario
        /// </summary>
        /// <param name="beneficiario">Objeto de beneficiario</param>
        public void Alterar(DML.Beneficiario beneficiario)
        {
            _beneficiario.Alterar(beneficiario);
        }

        /// <summary>
        /// Excluir o beneficiario
        /// </summary>
        /// <param name="id">id do beneficiario</param>
        /// <returns></returns>
        public void Excluir(long id)
        {
            _beneficiario.Excluir(id);
        }

        /// <summary>
        /// Lista os beneficiarios do cliente
        /// <param name="idCliente">Objeto de idCliente</param>
        /// </summary>
        public List<DML.Beneficiario> Listar(long idCliente)
        {
            return _beneficiario.Pesquisa(idCliente);
        }

        /// <summary>
        /// Verifica Existencia 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CPF"></param>
        /// <param name="IdCliente"></param>
        /// <returns></returns>
        public bool VerificarExistencia(long Id, string CPF, long IdCliente)
        {
            return _beneficiario.VerificarExistencia(Id, CPF, IdCliente); ;
        }

        /// <summary>
        /// Atualiza/Insere/Exclui beneficiarios
        /// </summary>
        /// <param name="beneficiariosList">Objeto lista beneficiarios</param>
        /// <param name="idCliente">Objeto de idCliente</param>
        public string UpdateBeneficiarios(List<Beneficiario> beneficiariosList, long idCliente)
        {
            try
            {
                beneficiariosList = beneficiariosList ?? new List<Beneficiario>();
                List<Beneficiario> beneficiariosExistentes = Listar(idCliente) ?? new List<Beneficiario>();

                //Updates
                var beneficiariosUpdate = beneficiariosList.Where(x => x.Id > 0).ToList();
                UpdateBeneficiarios(idCliente, beneficiariosUpdate);

                //Inserts
                var beneficiariosInsert = beneficiariosList.Where(x => x.Id == 0).ToList();
                InsertBeneficiarios(idCliente, beneficiariosInsert);

                //Deletes
                var beneficiariosDelete = beneficiariosExistentes.Where(x => !beneficiariosList.Select(y => y.Id).Contains(x.Id)).ToList();
                beneficiariosDelete?.ForEach(x => Excluir(x.Id));

                return "Cadastro efetuado com sucesso";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Insere beneficiarios
        /// </summary>
        /// <param name="idCliente">Objeto de idCliente</param>
        /// <param name="beneficiariosList">Objeto lista beneficiarios</param>

        private void InsertBeneficiarios(long idCliente, List<Beneficiario> beneficiariosList)
        {
            beneficiariosList?.ForEach(beneficiario =>
            {
                if (VerificarExistencia(beneficiario.Id, beneficiario.CPF, idCliente))
                {
                    throw new Exception($"CPF({beneficiario.CPF}) já cadastrado para o cliente.");
                }
                if (!_boCPF.Validar(beneficiario.CPF))
                {
                    throw new Exception($"CPF({beneficiario.CPF}) do beneficiário inválido.");
                }
            });
            beneficiariosList?.ForEach(x => Incluir(x));
        }

        /// <summary>
        /// Atualiza beneficiarios
        /// </summary>
        /// <param name="idCliente">Objeto de idCliente</param>
        /// <param name="beneficiariosList">Objeto lista beneficiarios</param>
        private void UpdateBeneficiarios(long idCliente, List<Beneficiario> beneficiariosList)
        {
            beneficiariosList?.ForEach(x =>
            {
                if (VerificarExistencia(x.Id, x.CPF, idCliente))
                {
                    throw new Exception($"CPF({x.CPF}) já cadastrado para o cliente.");
                }
                if (!_boCPF.Validar(x.CPF))
                {
                    throw new Exception($"CPF({x.CPF}) do beneficiário inválido.");
                }
            });
            beneficiariosList?.ForEach(x => Alterar(x));
        }
    }
}
