var CreateHouseForm = verifyFormExist("houseForm");
var CreateHouseFormAux = verifyFormExist("houseFormAux");

function verifyFormExist(id) {
    var element = document.getElementById(id);
    var elementStyle = window.getComputedStyle(element);
    var display = elementStyle.getPropertyValue('display');

    validate = display == "flex" ? true : false;

    return validate;
}

function showHouseForm() {

    if (!CreateHouseForm) {
        var form = document.getElementById("houseForm");
        form.style.display = "flex";
        var btnInfo = document.getElementById("articleValidateButtonHouse")
        btnInfo.style.display = "flex";
        CreateHouseForm = true;
    }
    else {
        var form = document.getElementById("houseFormAux");
        form.style.display = "flex ";
        var btnInfo = document.getElementById("articleValidateButtonHouseAux")
        btnInfo.style.display = "flex";
        CreateHouseFormAux = true;
    }
    toggleHouseFormBtn();
}

function deleteHouseForm(formDeleteBtn) {

    if (formDeleteBtn) {
        var form = document.getElementById("houseForm");
        form.style.display = "none";
        var btnInfo = document.getElementById("articleValidateButtonHouse")
        btnInfo.style.display = "none";
        document.getElementById("Article_HouseFeaturesList").innerHTML = '<span id="errorArticle_House" class="text-danger col-md-12"></span>';
        houseFeatures = new Array();
        CreateHouseForm = false;
        cleanForm("House");
    } else {
        var form = document.getElementById("houseFormAux");
        form.style.display = "none";
        var btnInfo = document.getElementById("articleValidateButtonHouseAux")
        btnInfo.style.display = "none";
        document.getElementById("Article_HouseAuxFeaturesList").innerHTML = '<span id="errorArticle_HouseAux" class="text-danger col-md-12"></span>';
        houseAuxFeatures = new Array();
        CreateHouseFormAux = false;
        cleanForm("HouseAux");
    }
    toggleHouseFormBtn();
}

function cleanForm(id) {
    var elements = document.getElementsByClassName(`${id}Element`);
    for (var i = 0; i < elements.length; i++) {
        elements[i].value = "";
    }
}

function toggleHouseFormBtn() {

    var containerButton = document.getElementById("containerButton")

    if (CreateHouseForm && CreateHouseFormAux) {
        containerButton.style.display = "none";
        containerButton.classList.remove("d-block");
    }
    else {
        containerButton.classList.add("d-block");
    }
}

function hideHouseBodyForm(id) {
    var form = document.getElementsByName(`${id}FormBody`);
    for (var i = 0; i < form.length; i++) {
        form[i].style.display = "none";
    }
    var minusButton = document.getElementById(`${id}MinusButton`);
    minusButton.style.display = "none";
    var plusButton = document.getElementById(`${id}PlusButton`);
    plusButton.style.display = "inline-block";

}

function showHouseBodyForm(id) {
    var form = document.getElementsByName(`${id}FormBody`);
    for (var i = 0; i < form.length; i++) {
        form[i].style.display = "flex";
    }
    var plusButton = document.getElementById(`${id}PlusButton`);
    plusButton.style.display = "none";
    var minusButton = document.getElementById(`${id}MinusButton`);
    minusButton.style.display = "inline-block";
}
