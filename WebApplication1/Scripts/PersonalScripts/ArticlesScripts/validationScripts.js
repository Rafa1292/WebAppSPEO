function ValidateForm() {
    var articleForm = validateArticleForm()
    var terrainForm = validateTerrainForm()
    var houseForm = CreateHouseForm ? validateHouseForm("") : true;
    var houseAuxForm = CreateHouseFormAux ? validateHouseForm("Aux") : true;
    var pictureForm = ImageValidation()
    if (pictureForm) {
        var outstandingPictureForm = outstandingPictureValidation()

    }

    if (terrainForm && articleForm && houseForm && houseAuxForm && outstandingPictureForm) {
        document.getElementById("Form").submit();
    }
    else {
        alert("Revisa los campos del formulario!!!");
    }
}



function validateArticleForm() {
    var price = validateItem("Article_Price", 0, "");
    var ubication = validateItem("UbicationId", 0, "");
    //var enlisted = validateItem("IndividualContributorId", 0, "");
    var ownerName = validateItem("Article_OwnerName", null, "");
    var ownerPhone = validateItem("Article_OwnerPhone", 0, "");

    var validate = price && ubication && ownerName && ownerPhone? true : false;

    return validate;
}

function validateTerrainForm() {
    var foreheadMeasure = validateItem("Terrain_ForeheadMeasure", 0, "");
    var backgroundMeasure = validateItem("Terrain_BackgroundMeasure", 0, "");
    var topography = validateItem("Terrain_Topography", null, "");
    var model = validateFeatures("Terrain");
    var validate = foreheadMeasure && backgroundMeasure && topography && model ? true : false;

    return validate;
}

function validateHouseForm(id) {
    var foreheadMeasure = validateItem(`House${id}_HouseForeheadMeasure${id}`, 0, "");
    var backgroundMeasure = validateItem(`House${id}_HouseBackgroundMeasure${id}`, 0, "");
    var bedrooms = validateItem(`House${id}_Bedrooms${id}`, 0, "");
    var bathrooms = validateItem(`House${id}_Bathrooms${id}`, 0, "");
    var levels = validateItem(`House${id}_Levels${id}`, 0, "");
    var model = validateFeatures(`House${id}`);
    var validate = foreheadMeasure && backgroundMeasure && bedrooms && bathrooms && levels && model ? true : false;

    return validate;
}

function validateFeatures(model) {

    var message = "Debes seleccionar almenos 1 caracteristica";
    var spanId = `error${model}`;
    var array = selectArray(model);
    var comparer = array.length > 0 ? true : false;
    if (comparer) {
        cleanErrorSpan(spanId);
        return true;
    }
    else {
        if (message == "") {
            return;
        }
        errorMessageValidation(spanId, message)
        return false;
    }
}

function validateItem(id, dataType, message) {
    message = message == "" ? "Campo requerido" : message;
    var element = document.getElementById(id);
    var spanId = `error${id}`;

    var comparer = dataType == 0 ? (element.value > 0) : (element.value != "" && isNaN(element.value));
    if (comparer) {
        cleanErrorSpan(spanId);
        return true;
    }
    else {
        errorMessageValidation(spanId, message)
        return false;
    }
}

function errorMessageValidation(spanId, message) {
    var element = document.getElementById(spanId)
    element.innerHTML = "";
    element.innerHTML = message;
}

function cleanErrorSpan(spanId) {
    var element = document.getElementById(spanId)
    element.innerHTML = "";
}