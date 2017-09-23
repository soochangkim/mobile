/**
 * Created by skim7663 on 4/17/2017.
 */

function getPhotoFromLibrary(){
    var options = {
        quality: 50,
        allowEdit: false,
        destinationType: Camera.DestinationType.FILE_URI,
        sourceType: Camera.PictureSourceType.PHOTOLIBRARY
    };

    function onSuccess(img){
        $("#imgPostImage").prop("src", img);
        $("#txtPostImage").val(img);
    }
    function onFail(msg){
        alert("Failed to load " + msg);
    }

    navigator.camera.getPicture(onSuccess, onFail, options);
}