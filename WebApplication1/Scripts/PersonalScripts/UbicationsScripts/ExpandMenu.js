function ShowMenu(id) {
    var element = document.getElementById(id);
    element.style.height = "500px";
    element.style.overflow = "scroll";

    var btnPlus = document.getElementById(`${id}-plus`);
    var btnMinus = document.getElementById(`${id}-minus`);
    btnPlus.style.display = "none";
    btnMinus.style.display = "contents";

}

function HideMenu(id) {
    var element = document.getElementById(id);
    element.style.height = "0px";
    element.style.overflow = "hidden";
    var btnPlus = document.getElementById(`${id}-plus`);
    var btnMinus = document.getElementById(`${id}-minus`);
    btnPlus.style.display = "contents";
    btnMinus.style.display = "none";
}