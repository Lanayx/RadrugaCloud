(function () {
    
    $("#divCalculationParameters").on("click", ".removeCalculationParameter", function ()
    {
        $(this).parent().remove();
        renameParameters();
    });
})();

function renameParameters() {
    var i = 1;
    var commonDiv = $("#divCalculationParameters");
    var divs = commonDiv.find("div");

    divs.each(function () {
        var div = $(this);
        var span = div.find("span.numerator");
        span.text("param" + i++);
    });
}

