function showImage(src, target) {
    var fr = new FileReader();
    fr.onload = function () {
        target.src = fr.result;
    }
    fr.readAsDataURL(src.files[0]);
}

function putImage() {
    var src = document.getElementById("imgBox");
    var target = document.getElementById("foodImage");
    var label = document.getElementById("imgLabel");
    imgLabel.innerHTML = "Change Image";
    showImage(src, target);
}
