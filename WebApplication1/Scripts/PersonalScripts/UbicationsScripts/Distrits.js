/*Ejecuta un post que devuelve una lista de distritos
 * y cargar un dropDown con la siguiente estructura:
 
   <select name="DistritId" id="DistritId" class="form-control" onchange="distritValidation()">
        <option value=0 disabled selected>Seleccione un distrito</option>
    </select>

    <span class="text-danger field-validation-error" id="DistritError"></span>

*/


function FillDistrits(CantonId) {
    $("#DistritId").html("");
    $.post("GetDistrits/Ubications", { CantonId: CantonId })
        .done(function (data) {
            $("#DistritId").html(data);
            cantonValidation();
        })
        .fail(function (data) {
            alert("no Sirve")
        });
}

/* Consulta si en la estructura anterior hay un valor seleccionado diferente de 0
 * que es el valor seleccionado por defecto, en caso dde no haber seleccionado una opcion valida
 * muestra un error en la etiqueta span que esta en la estructura anterior.
 
 */

function distritValidation() {
    var distrit = document.getElementById('DistritId');
    if (!distrit.selectedIndex > 0) {
        document.getElementById('DistritError').innerHTML =
            '<span>Debes agregar un distrito</span>';
        return false;
    } else {
        document.getElementById('DistritError').innerHTML = "";
        return true;
    }
}