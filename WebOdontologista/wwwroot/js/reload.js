function reload() {
    var value = document.getElementById("show").value;
    var index = window.location.href.indexOf("?");
    var url = window.location.href.substring(0, index);
    window.location.href = url + "?show=" + value;
}