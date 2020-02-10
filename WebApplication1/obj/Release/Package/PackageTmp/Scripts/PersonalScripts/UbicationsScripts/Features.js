/* Estructura utilizada para mostrar en la vista los elementos seleccionados. El boton de 'caracteristicas disponibles'
 * despliega un modal para que el usuario seleccione o elimine los features segun desee.
 * la estructura del modal se encuentra al final de este documento.
  
   <div class="col-md-12" id="selectedFeatures">
    <div class="form-group">
        <div class="col-md-12 mb-3">
            <button type="button" class="btn btn-outline-primary" data-toggle="modal" data-target="#UbicationFeaturesModal">
                Caracteristicas disponibles
            </button>
        </div>
        <div class="col-md-12">
            <span class="text-danger field-validation-error" id="FeatureError">
            </span>
        </div>

    </div>
    <div class="col-md-12 d-flex justify-content-center">
        <ul class="list-group " id="featuresContent" style="width:60%;">

=> EL siguiente codigo razor se utiliza en caso de que el controlador nos envie de vuelta a la vista o si estamos editando
el modelo. en el 'ViewBag.SelectedUbicationFeatures' se deberan incluir las caracteristicas preseleccionadas asi este codigo
podra mostrar en la vista estas caracteristicas. 

            @if (ViewBag.SelectedUbicationFeatures != null)
            {
                foreach (var feature in ViewBag.UbicationFeaturesId)
                {
                    foreach (var selectedFeature in ViewBag.SelectedUbicationFeatures)
                    {
                        if (Int32.Parse(feature.Value) == selectedFeature)
                        {
                            <li class="list-group-item-success list-group-item" id=@feature.Value>@feature.Text</li>
                        }
                    }

                }
            }
        </ul>
 */



//Array para almacenar los valores de las caracteristicas seleccionadas por el usuario.
var Features = new Array

/* 
 * Variable creada para recibir datos provenientes del controlador en caso de que este por un error 
 * nos devuelva a la vista o en caso de estar editando el modelo.
*/
var SelectedFeatures = document.getElementsByClassName('list-group-item-success');

/*
 * Si la variable anterior recibe algun contenido vamos a rellenar el array que almacena
 * los datos de Features seleccionados por el usuario anteriormente
 */
if (SelectedFeatures.length > 0) {
    for (var i = 0; i < SelectedFeatures.length; i++) {
        Features.push(SelectedFeatures[i].textContent);
    }
}


/*
 * Detecta cuando un usuario clickea una opcion de Features disponibles, y segun el estado de esta opcion
 * (Seleccionada previamente o no) procede a cambiar la clase que nos indica si la opcion esta seleccionada
 * o no mediante las clases establecidas para dicha funcion,
 * ademas llama a la funcion 'fillFeatures' y si el elemento no estaba seleccionado lo ingresa al array y
 * en caso contrario lo elimina.
 * Se antepone la palabra lbl y chk al id para diferenciar los elementos y poder accederlos.
 */
function clickedLabel(id, name) {

    var lblId = "lbl-" + id;
    var chkId = "chk-" + id;

    if (document.getElementById(chkId).checked) {

        document.getElementById(lblId).classList.add('ubication-feature-lbl');
        document.getElementById(lblId).classList.remove('ubication-feature-lbl-selected');
    } else {

        document.getElementById(lblId).classList.remove('ubication-feature-lbl');
        document.getElementById(lblId).classList.add('ubication-feature-lbl-selected');
    }

    fillFeatures(name)
}

/*
 * Recibe el nombre del feature seleccionado, lo agrega o lo elimina del array utilizado para dicha funcion y
 * llama a la funcion 'drawSelectedOptions'  para que esta agregue los elementos del array a la vista.
 */

function fillFeatures(name) {

    if (!Features.includes(name)) {
        Features.push(name);
    }
    else {

        var i = Features.indexOf(name);
        Features.splice(i, 1);
    }

    drawSelectedOptions();
}

/*
 * Se encarga de añadir a la vista los elementos existentes dentro del array
 * en caso de estar eliminando un elemento previamente este fue retirado del array
 * por lo cual la funcion añadira solo los que esten en el mismo excluyendo asi el elemento eliminado.
 * una vez finalizado el proceso ejecuta la validacion para que en caso de no haber elementos indicar
 * al usuario que debe seleccionar almenos 1 de estos elementos disponibles.
 */

function drawSelectedOptions() {

    var content = "";

    if (Features.length == 0) {
        content += " <li class=\"list-group-item list-group-item-danger\"> <label>No has seleccionado ninguna caracteristica</label></li>";
    }

    for (var i = 0; i < Features.length; i++) {
        content += "<li class=\"list-group-item-success list-group-item\" id= \"" + Features[i] + "\" >" + Features[i] + "</li>"
    }

    document.getElementById("featuresContent").innerHTML = content;
    FeatureValidation();
}

/*
 * Si no existen elementos seleccionados en el array, le indica al usuario mediante
 * una etiqueta span que  se encuentra dentro de la estructura, que debe seleccionar almenos
 * una de las opciones disponibles.
 */

function FeatureValidation() {
    if (Features.length > 0) {
        document.getElementById('FeatureError').innerHTML = "";
        return true;
    } else {
        document.getElementById('FeatureError').innerHTML =
            '<span>Debes agregar almenos una caracteristica</span>';
        return false;
    }
}

/*
     <!-- Modal -->
    <div class="modal fade" id="UbicationFeaturesModal" tabindex="-1" role="dialog" aria-labelledby="UbicationFeaturesLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="UbicationFeatureslLabel">Lista de caracteristicas</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <ul class="list-group list-group-flush">
                        @foreach (var feature in ViewBag.UbicationFeaturesId)
                        {
                            var lblId = feature.Value;
                            var name = Json.Encode(@feature.Text);

=> En caso de estar editando o si el controlador por algun error nos devuelve a la vista a causa de un error
en el 'ViewBag.SelectedUbicationFeatures' debemos enviar las caracteristicas seleccionadas previamente y el siguiente
codigo razor se encargara de mostrarlas. Ademas creara los inputs que se envian de vuelta al controlador.

                            if (ViewBag.SelectedUbicationFeatures != null)
                            {
                                string state = "";
                                string className = "ubication-feature-lbl";
                                foreach (var selectedFeature in ViewBag.SelectedUbicationFeatures)
                                {
                                    state = Int32.Parse(lblId) == selectedFeature ? "checked" : state;
                                    className = Int32.Parse(lblId) == selectedFeature ? "ubication-feature-lbl-selected" : className;
                                    if (state == "checked")
                                    {
                                        break;
                                    }
                                }
                                <li class="list-group-item @className" id="lbl-@lblId">
                                    <div class="custom-control">
                                        <label for="chk-@lblId" onclick="clickedLabel(@feature.Value, @name)">@feature.Text</label>
                                        <input @state name="ubicationFeatures[]" id="chk-@lblId" type="checkbox" style="display:none" value="@lblId" />
                                    </div>
                                </li>
                            }
                            else
                            {
                                <li class="list-group-item ubication-feature-lbl" id="lbl-@lblId">
                                    <div class="custom-control">
                                        <label for="chk-@lblId" onclick="clickedLabel(@feature.Value, @name)">@feature.Text</label>
                                        <input name="ubicationFeatures[]" id="chk-@lblId" type="checkbox" style="display:none" value="@lblId" />
                                    </div>
                                </li>
                            }


                        }
                    </ul>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-outline-secondary" data-dismiss="modal">Cerrar</button>
                </div>
            </div>
        </div>
    </div>
 */