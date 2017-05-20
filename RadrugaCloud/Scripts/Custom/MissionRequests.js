$(function () {


    // Let's check if the browser supports notifications
    if (!("Notification" in window)) {
        if (!sessionStorage.notificationNotSupported) {
            alert("Браузер не поддерживает нотификации");
            sessionStorage.notificationNotSupported = true;
        }
        return;
    }


    if (Notification.permission !== 'granted') {
        Notification.requestPermission(function (permission) {
            // If the user accepts, let's create a notification
            alert("Notifications were set up!");
        });
    }



    setInterval(function () {
        var currentCount = $(".js-missionRequestItem").length;
        if (currentCount === 0) {
            $.ajax().then(function (result) {
                var calc = $(result).find(".js-missionRequestItem").length;
                if (calc > 0) {
                    var notification = new Notification("Новый запрос на прохождение!");
                    window.location.reload();
                }
            });
        }
    }, 100000);
})