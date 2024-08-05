var random
$(document).ready(function () {
    createModalBeneficiarios();
});
$(document).on('submit', '#form-beneficiario', function (event) {
    event.preventDefault();
    var formData = new FormData(this);

    var index = formData.get('id-beneficiario');
    var nomeBeneficiario = formData.get('nome-beneficiario');
    var cpfBeneficiario = formData.get('cpf-beneficiario');

    if (!checkCPF(cpfBeneficiario)) {
        alert('CPF invalido');
    }
    else {
        if (index >= 0) {
            if (beneficiarios.findIndex(x => x.CPF == cpfBeneficiario && x.Id !== beneficiarios[index].Id) >= 0) {
                alert('CPF duplicado.');
            }
            else {
                beneficiarios[index].Nome = nomeBeneficiario;
                beneficiarios[index].CPF = cpfBeneficiario;
            }
        }
        else if (beneficiarios.findIndex(x => x.CPF == cpfBeneficiario) >= 0) {
            alert('CPF duplicado.');
        }
        else {
            beneficiarios.push({ Id: 0, Nome: nomeBeneficiario, CPF: cpfBeneficiario });
        }
        beneficiarios.sort((a, b) => a.Nome.localeCompare(b.Nome));
        $('#id-beneficiario').val(-1);
        $('#form-beneficiario')[0].reset();
        getBeneficiarios();
        $('#incluir-atualizar').text('Incluir');
    }
});
function createModalBeneficiarios() {
    random = Math.random().toString().replace('.', '');
    var texto = `
    <meta charset="utf-8" />
    <div id="${random}" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="modal-title">${titulo}</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <form id="form-beneficiario" >
                        <input type="hidden" class="form-control" id="id-beneficiario" name="id-beneficiario" value="-1">
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label for="CPF">CPF:</label>
                                    <input required="required" type="text" class="form-control" id="cpf-beneficiario" name="cpf-beneficiario" placeholder="Ex.: 010.011.111-00" maxlength="14">
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label for="Nome">Nome:</label>
                                    <input required="required" type="text" class="form-control" id="nome-beneficiario" name="nome-beneficiario" placeholder="Ex.: Maria" maxlength="50">
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div style="padding-top: 26px;" class="form-group">
                                    <button type="submit" id="incluir-atualizar" class="btn btn-success">Incluir</button>
                                </div>
                            </div>
                        </div>
                    </form>
                    <table id="table-beneficiarios" class="table">
                        <thead>
                            <tr>
                                <th scope="col">CPF</th>
                                <th scope="col"style="width: 225px;">Nome</th>
                                <th scope="col">${actions}</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                    </div>
            </div><!-- /.modal-content -->
        </div><!-- /.modal-dialog -->
    </div><!-- /.modal -->
    `;

    $('body').append(texto);

    $('#' + random).on('hidden.bs.modal', function () {
        $('#form-beneficiario')[0].reset();
    });

    $('#cpf-beneficiario').on('input', function () {
        var value = $(this).val();
        value = value.replace(/\D/g, '');

        if (value.length > 9) {
            value = value.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
        } else if (value.length > 6) {
            value = value.replace(/(\d{3})(\d{3})(\d{3})/, '$1.$2.$3');
        } else if (value.length > 3) {
            value = value.replace(/(\d{3})(\d{3})/, '$1.$2');
        }

        $(this).val(value);
    });
}
function ModalBeneficiarios() {
    $(`#${random}`).modal('show');
    getBeneficiarios();
}
function getBeneficiarios() {
    if (!beneficiarios) {
        $.ajax({
            url: urlBeneficiarios,
            type: 'get',
            async: false,
            success: data => {
                beneficiarios = data;
            },
            error: error => { console.error(data); }
        });
    }
    var table = $('#table-beneficiarios tbody');
    table.empty();
    beneficiarios.sort((a, b) => a.Nome.localeCompare(b.Nome));
    beneficiarios.forEach(addBeneficiario)
}
function addBeneficiario(beneficiario) {
    var table = $('#table-beneficiarios tbody');
    table.append(`
    <tr>
        <td>${beneficiario.CPF}</td>
        <td>${beneficiario.Nome}</td>
        <td>
            <button type="button" class="btn btn-primary btn-sm" onclick="updateBeneficiario(${beneficiario.Id})">Alterar</button>
            <button type="button" class="btn btn-primary btn-sm" onclick="deleteBeneficiario(${beneficiario.Id})">Excluir</button>
        </td>
    </tr>
    `);
}
function updateBeneficiario(id) {
    index = beneficiarios.findIndex((b) => b.Id == id);
    $('#id-beneficiario').val(index);
    $('#nome-beneficiario').val(beneficiarios[index].Nome);
    $('#cpf-beneficiario').val(beneficiarios[index].CPF);
    $('#incluir-atualizar').text('Alterar');
}
function deleteBeneficiario(id) {
    var index = beneficiarios.findIndex(x => x.Id == id);
    beneficiarios.splice(index, 1);
    beneficiarios.sort((a, b) => a.Nome.localeCompare(b.Nome));
    getBeneficiarios();
}
function checkCPF(strCPF) {
    var Soma;
    var Resto;
    Soma = 0;
    strCPF = strCPF.replace(/[^0-9]/g, '');
    if (strCPF == "00000000000") return false;

    for (i = 1; i <= 9; i++) Soma = Soma + parseInt(strCPF.substring(i - 1, i)) * (11 - i);
    Resto = (Soma * 10) % 11;

    if ((Resto == 10) || (Resto == 11)) Resto = 0;
    if (Resto != parseInt(strCPF.substring(9, 10))) return false;

    Soma = 0;
    for (i = 1; i <= 10; i++) Soma = Soma + parseInt(strCPF.substring(i - 1, i)) * (12 - i);
    Resto = (Soma * 10) % 11;

    if ((Resto == 10) || (Resto == 11)) Resto = 0;
    if (Resto != parseInt(strCPF.substring(10, 11))) return false;
    return true;
}