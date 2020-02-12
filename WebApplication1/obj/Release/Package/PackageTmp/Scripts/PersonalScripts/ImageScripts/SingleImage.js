/*
 * Se ejecuta cuando el usuario carga archivos a través del input file destinado para esto.
 * La funcion recibe el evento y por cada elemento seleccionado crea un thumbnail mediante
 * la funcion 'CreateThumbnail' enviando como parametros el archivo de imagenes como tal,
 * la iteracion en ejecucion y el id que lo toma del nombre que tenga la imagen.
 * Al finalizar el ciclo limpia el evento para evitar conflictos.
 */
function showPreview(event) {

    var input = event.target.files[0];
    var thumbnail_id = input.name;
    createThumbnail(input, thumbnail_id);

    event.target.value = "";
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
function createThumbnail(input, thumbnail_id) {


    var thumbnail = document.createElement('img');

    thumbnail.classList.add('thumbnail', thumbnail_id);
    thumbnail.setAttribute('src', URL.createObjectURL(input))
    thumbnail.setAttribute('id', thumbnail_id);
    var closeButton = createCloseButton(thumbnail_id);


    document.getElementById('imagePreview').innerHTML = "";
    document.getElementById('imagePreview').append(thumbnail);
    document.getElementById('imagePreview').append(closeButton);
    getBase64Image(input, thumbnail_id);

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

        document.getElementById('imagePreview').innerHTML = "";
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
        base64String.setAttribute('name', 'url');
        base64String.setAttribute('style', 'display: none;');
        base64String.setAttribute('type', 'text');
        base64String.value = dataURL;
        document.getElementById(id).appendChild(base64String);
    };
    reader.readAsDataURL(input);

}

/*
 *Obtiene los elementos ya existentes y compara los nombres de la imagen, en caso de encontrar alguna coincidencia
 * cancela la carga inmediatamente.
 */

function ImageValidation() {
    var images = document.getElementsByName('url')
    if (!images.length > 0) {
        document.getElementById('ImageError').innerHTML =
            '<span>Debes agregar almenos una foto</span>';
        return false;
    } else {
        document.getElementById('ImageError').innerHTML = "";
        return true;
    }
}