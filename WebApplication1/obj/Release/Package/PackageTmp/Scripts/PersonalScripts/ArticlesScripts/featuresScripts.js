var houseFeatures = new Array();
var houseAuxFeatures = new Array();
var terrainFeatures = new Array();

var selectedTerrainFeatures = document.getElementsByName("TerrainFeatures[]");
loadFeaturesViewBag(selectedTerrainFeatures, "Terrain");

var selectedHouseFeatures = document.getElementsByName("HouseFeatures[]");
loadFeaturesViewBag(selectedHouseFeatures, "House");

var selectedHouseAuxFeatures = document.getElementsByName("HouseAuxFeatures[]");
loadFeaturesViewBag(selectedHouseAuxFeatures, "HouseAux");

function showFeatures(featureJsonArray) {

    var array = selectArray(featureJsonArray[0].Model);

    var content = "";
    for (var i = 0; i < featureJsonArray.length; i++) {
        var itemClass = array.includes(featureJsonArray[i].FeatureId) ? "ubication-feature-lbl-selected" : "ubication-feature-lbl";
        content +=
            `<li class='list-group-item ${itemClass}' id='lbl-${featureJsonArray[i].FeatureId}'>
                        <div class='custom-control'>
                            <label class='features-label' for='chk-${featureJsonArray[i].FeatureId}'
                            onclick="clickedLabel(${featureJsonArray[i].FeatureId}, '${featureJsonArray[i].Description}','${featureJsonArray[i].Model}')">${featureJsonArray[i].Description}</label>
                        </div>
                    </li>`
    }
    document.getElementById("featuresList").innerHTML = content;
}

function clickedLabel(id, name, model) {
    toggleLabel(id, model, name);
    validateFeatures(model);
}

function toggleLabel(id, model, name) {
    var array = selectArray(model);
    var exist = editArray(id, array);
    var element = `lbl-${id}`;
    if (exist) {
        document.getElementById(element).classList.add('ubication-feature-lbl');
        document.getElementById(element).classList.remove('ubication-feature-lbl-selected');
        excludeFeatures(id, model);

    }
    else {
        document.getElementById(element).classList.remove('ubication-feature-lbl');
        document.getElementById(element).classList.add('ubication-feature-lbl-selected');
        includeFeatures(id, model, name);
    }

}

function selectArray(model) {
    switch (model) {
        case "Terrain":
            return terrainFeatures;
            break;
        case "House":
            return houseFeatures;
            break;
        case "HouseAux":
            return houseAuxFeatures;
            break;
    }
}

function editArray(id, arrayModel) {
    if (arrayModel.includes(id)) {
        arrayModel.splice(arrayModel.indexOf(id), 1);
        return true;
    }
    else {
        arrayModel.push(id);
        return false;
    }
}

function includeFeatures(id, model, name) {
    var content = "";
    elementId = `${model}FeaturesList`
    var element = document.getElementById(elementId);
    var container = document.createElement('div');
    container.setAttribute("class", `list-group-item col-md-2 m-3 ${model} list-group-item-success`);
    container.setAttribute("id", `${model}${id}`);
    container.innerText = name;
    var input = document.createElement('input');
    input.setAttribute("name", `${model}Features[]`);
    input.setAttribute("style", "display: none");
    input.setAttribute("value", id);
    container.append(input);
    element.append(container);

}

function excludeFeatures(id, model) {
    var elements = document.getElementsByClassName(model)
    for (var i = 0; i < elements.length; i++) {
        if (elements[i].id == `${model}${id}`) {
            var deleteId = elements[i].id;
            document.getElementById(deleteId).remove();
        }
    }
}

function loadFeaturesViewBag(SelectedFeatures, model) {
    if (SelectedFeatures != null && SelectedFeatures.length > 0) {
        var array = selectArray(model);
        for (var i = 0; i < SelectedFeatures.length; i++) {
            editArray(parseInt(SelectedFeatures[i].value), array);
        }
    }
}