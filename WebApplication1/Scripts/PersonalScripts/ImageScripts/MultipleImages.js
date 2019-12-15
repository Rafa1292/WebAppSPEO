/*Estructura utilizada para la carga de imagenes y la previsualizacion de las mismas con boton para eliminarlas.
 *
 *<div class="col-md-6 text-center p-0">
    <h4>Agregar fotos </h4>
    <hr />
    <div class="form-group">
        <div class="col-md-12 row m-0" id="imagesPreview">
            <div class="col-md-12 d-flex justify-content-center p-0">
                <label for="pickImage" class="image-label-style" style="width:300px !important;"><i class="fas fa-camera-retro fa-2x" style="color: gray;"></i></label>
                <input multiple id="pickImage" type="file" name="imageload" style="display: none" onchange="showPreview(event)" />
            </div>
            <div class="col-md-12">
            => Etiquetas para señalar errores en caso de ser necesario
                <span class="text-danger field-validation-error" id="ImageError">
                </span>
                <span class="text-danger field-validation-error" id="OutstandingImageError">
                </span>
            </div>
            => Este segmento de razor se utiliza en caso de que el controlador nos devuelva a la vista por un error o
            para editar el modelo. Se deben cargar las imagenes en formato de base64.
            La imagen deportada debe venir en el 'ViewBag.Selectedurl' mientras que el resto debe venir en el'ViewBag.urls'.
            (Arroba)if (ViewBag.urls != null)
            {
                <div class="col-md-2 my-2 d-flex" id="0">
                    <figure class="figure-style">
                        <img class="thumbnail 0 figure-selected-style" id="0" onclick="SelectOutstandingPicture(this.id)" src=(Arroba)ViewBag.Selectedurl>
                    </figure>
                    <i id="0" class="fas fa-times-circle close-button-style 0" aria-hidden="true"></i>
                    <input name="outstandingPicture" class="inputs" style="display: none;" id="0" type="text" value="(Arroba)ViewBag.Selectedurl">
                </div>

                var i = 1;
                foreach (var url in ViewBag.urls)
                {
                    <div class="col-md-2 my-2 d-flex" id="(Arroba)i">
                        <figure class="figure-style">
                            <img class="thumbnail (Arroba)i" onclick="SelectOutstandingPicture(this.id)" id="(Arroba)i" src=(Arroba)url>
                        </figure>
                        <i id="(Arroba)i" class="fas fa-times-circle close-button-style (Arroba)i" aria-hidden="true"></i>
                        <input name="urls[]" class="inputs" style="display: none;" id="(Arroba)i" type="text" value="(Arroba)url">
                    </div>
                    i++;
                }
            }

        </div>
    </div>
</div>
 */


/*
 * Se ejecuta cuando el usuario carga archivos a través del input file destinado para esto.
 * La funcion recibe el evento y por cada elemento seleccionado crea un thumbnail mediante
 * la funcion 'CreateThumbnail' enviando como parametros el archivo de imagenes como tal,
 * la iteracion en ejecucion y el id que lo toma del nombre que tenga la imagen.
 * Al finalizar el ciclo limpia el evento para evitar conflictos.
 */
function showPreview(event) {

    var input = event.target;

    for (var i = 0; i < input.files.length; i++) {
        var thumbnail_id = input.files[i].name;
        createThumbnail(input, i, thumbnail_id);
    }
    input.value = "";
}


/*
 * Esta funcion recibe el input con las imagenes cargadas, el iterator que es el momento del ciclo en
 * el que vamos o el indice del array que se esta evaluando y el thumbnail_id que se obtiene del nombre de la
 * imagen.
 * Cada thumbnail se crea con el atributo name y se le asigna de valor el nombre de la imagen por lo que
 * antes de crear un thumbnail se intenta obtener algun elemento con ese nombre de esta forma si existe alguno quiere
 * decir que la imagen ya fue seleccionada y se avisa al usuario dicha situacion y se cancela la carga de dicha imagen.
 *
 */
function createThumbnail(input, iterator, thumbnail_id) {

    //si el usuario carga la misma imagen durante la misma sesion este bloque impide la carga de imagenes con mismo nombre.
    var existing = document.getElementById(thumbnail_id)
    if (existing != null) {
        alert(`La foto: ${thumbnail_id} ya esta incluida`);
        return;
    }

    var imageContainer = document.createElement('div');
    var thumbnail = document.createElement('img');
    var figure = document.createElement('figure');
    /*
     * se agrega la clase figure para que se comporte como tal,
     * y figure style para darle los atributos creados para interactuar
     * con el usuario
     */

    figure.classList.add("figure", "figure-style");


    thumbnail.classList.add('thumbnail');// clase agregada para dar estilos
    thumbnail.setAttribute('src', URL.createObjectURL(input.files[iterator]))//blob de la imagen
    thumbnail.setAttribute('id', input.files[iterator].name);//Define el id con el nombre de la imagen, utilizado para evitar duplicados de imagen.
    thumbnail.setAttribute('onclick', "SelectOutstandingPicture(this.id)");//onclick, para definir imagen de portada

    var closeButton = createCloseButton(thumbnail_id);

    imageContainer.classList.add('col-md-2', 'my-2', 'd-flex');// definen estilos de contenedor
    imageContainer.setAttribute('name', input.files[iterator].name);// nombre de imagen utilizado para obtener todas las imagenes a la hora de validar.
    imageContainer.setAttribute('id', thumbnail_id);//id utilizado para identificar el contenedor en caso de que se elimine.
    figure.appendChild(thumbnail);
    imageContainer.appendChild(figure);
    imageContainer.appendChild(closeButton);
    document.getElementById('imagesPreview').appendChild(imageContainer);
    getBase64Image(input.files[iterator], thumbnail_id);

}

/*
 * Aca se crea el boton que al clickear elimina la imagen, este se representa mediante
 * un icono de 'x' obtenido de fontawesome y se le asigna el mismo id que al div que
 * almacena el contenido del img, de esta forma se ubica el elemento y se elimina
 */

function createCloseButton(thumbnail_id) {
    var closeButton = document.createElement('i');
    closeButton.setAttribute('id', thumbnail_id);
    closeButton.classList.add('fas', 'fa-times-circle', 'close-button-style', thumbnail_id);
    return closeButton;
}


/*
 * Listener agregado al body que detecta cada click y si este es al boton creado para eliminar la foto
 * toma el id e identifica el elemento que se desea eliminar y lo remueve de la vista.
 */
document.body.addEventListener('click', function (e) {
    if (e.target.classList.contains('close-button-style')) {
        var id = e.target.id;
        e.target.remove();
        document.getElementById(id).remove();
    }
});

/**
 * Recibe como parametro el archivo tipo file que contiene la imagen y el id del contenedor,
 * mediante el fileReader obtiene la url en base64, crea un elemento tipo input con el atributo name y se le
 * asigna el valor'urls[]' por medio de este input el controlador recibe la informacion.
 * Este input se agrega al contenedor que tiene el thumbnail asi al eliminar la vista previa
 * nos aseguramos que no llegue al controlador.
 * Por ultimo verifica mediante la cadena de base 64 que no exista ya entre las imagenes seleccionadas.
 */
function getBase64Image(input, id) {

    var replaceString = "";
    switch (input.type) {

        case "image/jpeg":
            replaceString = "data:image/jpeg;base64,";
            break;
        case "image/jpg":
            replaceString = "data:image/jpg;base64,";
            break;
        case "image/png":
            replaceString = "data:image/png;base64,";
            break;

    }


    var reader = new FileReader();
    reader.onload = function () {
        var dataURL = reader.result;
        dataURL = dataURL.replace(replaceString, "");
        var base64String = document.createElement('input');
        base64String.setAttribute('id', input.name);
        base64String.setAttribute('class', "inputs");
        base64String.setAttribute('name', 'urls[]');//se usa para enviar info al controlador
        base64String.setAttribute('style', 'display: none;');
        base64String.setAttribute('type', 'text');
        base64String.value = dataURL;
        document.getElementById(id).appendChild(base64String);
        verifyExistingFile(dataURL, id, input.name);
    };
    reader.readAsDataURL(input);

}

/*
 * Recibe la cadena de base 64 del input que se esta creando, obtiene todas las imagenes ya cargadas y
 * compara el contenido para verificar que no exista, en caso de existir notifica al usuario y cancela la
 * carga.
 * Estamos usando doble verificacion: primero antes de crear la imagen tratamos de encontrar si existe
 * un archivo ya seleccionado con el mismo nombre, pero en caso de ir al controlador y este nos devuelva a
 * la vista o en caso de estar editando el modelo, el controlador nos devuelve las imagenes pero no el nombre
 * a estas se les identifica mediante un consecutivo por lo que es posible que la misma imagen tenga nombres diferentes
 * y se genere un duplicado de una imagen, para evitar esto hacemos este 2do paso donde comparamos contenido.
 */
function verifyExistingFile(dataURL, id,currentImageId) {
    var elements = document.getElementsByClassName('inputs');
    for (var i = 0; i < elements.length; i++) {
        if (elements[i].value == dataURL && elements[i].id != id) {
            if (elements[i].name == 'outstandingPicture') {
                SelectOutstandingPicture(currentImageId);
            }
            document.getElementById(elements[i].id).remove();
        }
    }
    ImageValidation();
}






/*
 *Obtiene los elementos ya existentes y compara los nombres de la imagen, en caso de encontrar alguna coincidencia
 * cancela la carga inmediatamente.
 */

function ImageValidation() {
    var images = document.getElementsByName('urls[]')
    if (!images.length > 0) {
        document.getElementById('ImageError').innerHTML =
            '<span>Debes agregar almenos dos foto</span>';
        return false;
    } else {
        document.getElementById('ImageError').innerHTML = "";
        return true;
    }
}

/*
 * Valida que el usuario haya seleccionado una imagen de portada.
 * Cuando un usuario selecciona una imagen de portada se crea un input con el nombre outstandingPicture,
 * esta funcion trata de obtener un elemento con dicho nombre y en caso de no hacerlo le indica al usuario
 * que debe seleccionar una foto de portada.
 */

function outstandingPictureValidation() {
    var images = document.getElementsByName('outstandingPicture')
    if (!images.length > 0) {
        document.getElementById('OutstandingImageError').innerHTML =
            '<span>Debes seleccionar una foto de portada. <br /> Para ello presiona sobre la imagen que quieres que aparezca como portada.</span>';
        return false;
    } else {
        document.getElementById('OutstandingImageError').innerHTML = "";
        return true;
    }


}

/*
 * Esta funcion recibe el id de una imagen, y obtiene todos los elementos img con sus respectivos inputs
 * a la imagen que coincide con el id recibido le agrega la clase 'figure-selected-style' y le elimina la misma clase
 * a todas las demas, de igual forma con los inputs a todos les agrega el nombre urls[] y al que coincida con el
 * id se le asigna el nombre outstandingPicture.
 */

function SelectOutstandingPicture(id) {
    var inputs = document.getElementsByClassName('inputs');
    var images = document.getElementsByClassName('thumbnail');

    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].id == id) {
            inputs[i].name = "outstandingPicture";
        }
        else {
            inputs[i].name = "urls[]";
        }
    }

    for (var i = 0; i < images.length; i++) {
        if (images[i].id == id) {
            images[i].classList.add("figure-selected-style");
        }
        else {
            images[i].classList.remove("figure-selected-style");
        }
    }

}