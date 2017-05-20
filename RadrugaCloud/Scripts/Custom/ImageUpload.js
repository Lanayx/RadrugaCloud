var missionImageUpload = (function ()
{

    //init
    var $divImageManager = $("#divImageManager");

    $divImageManager.on("change", ".draftImageInput", function () {
        var input = ($(this))[0];
        missionImageUpload.uploadImage(input);
    });

    this.uploadImage = function (input) {
        if (!window.File || !window.FileReader || !window.FileList || !window.Blob) {
            /*alert('The File APIs are not fully supported in this browser.');*/
            alert('Текущий механизм загрузки не поддерживается вашим браузером. Для взаимодействия используйте, пожалуйста, современные браузеры.');
            return;
        }
        
        if (!input) {
            /*alert("Um, couldn't find the fileinput element.");*/
            alert("Кхм, не удалось найти элемент ввода. Попробуйте еще раз.");
            return;
        }
        if (!input.files) {
            /*alert("This browser doesn't seem to support the `files` property of file inputs.");*/
            alert("К сожаление, ваш браузер не поддеживает некоторые необходимые функции.  Для взаимодействия используйте, пожалуйста, современные браузеры.");
            return;
        }
        if (!input.files[0]) {
            /*alert("Please select a file before clicking 'Load'");*/
            alert("Выберите, пожалуйста, корректное изображение.");
            return;
        }

        var jqueryInput = $(input);
        var actionUrl = jqueryInput.data("actionurl");

        var data = new FormData();
        data.append('image', input.files[0]);

        $.ajax({
            url: actionUrl,
            type: "POST",
            data: data,
            /*dataType: 'json',*/
            processData: false, // Don't process the files
            contentType: false, // Set content type to false as jQuery will tell the server its a query string request
            success: function(url) {
                uploadSuccess(url, input);
            },
            error: uploadError
        });
    }

    var uploadSuccess = function (url, input) {
        /*var siblings = $(input).siblings();
        /*var urlHidden = siblings.find(".imageUrlHidden");#1#
        var urlHidden = siblings.get(0);
        $(urlHidden).val(url);
        /*var imageContent = siblings.find(".");#1#
        var imageContent = siblings.get(1);
        $(imageContent).attr('src', url);
        /*var validator = siblings.find(".imageUrlValidator");#1#
        var validator = siblings.get(2);
        $(validator).html("");*/
        $(input).siblings(".imageUrlHidden").val(url);
        $(input).siblings(".imageContent").attr('src', url);
        $(input).siblings(".imageUrlValidator").html("");

    }

    var uploadError = function (xhr) {
        var responseText = xhr.responseText;;
        var regex = /(?:<title>)(.+)(?:<\/title>)/i;
        var errorMessage = regex.exec(responseText);
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "Неизвестная ошибка при обработке запроса";
        } else {
            errorMessage = errorMessage[1];
        }

        alert("Упс, что-то пошло не так: " + errorMessage);
    }

    return this;
})();