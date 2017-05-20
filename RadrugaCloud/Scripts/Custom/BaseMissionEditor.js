(function () {
    function processSelect(selectElem) {
        var select = $(selectElem);
        var linkButton = $(select.parent().find("a"));
        var value = select.val();
        if (value === "" || value === undefined) {
            linkButton.css("visibility", "hidden");
        } else {
            linkButton.css("visibility", "visible");
            var editUrl = $("#divBaseMissions").data("edit-url");
            linkButton.attr("href", editUrl + "/" + value);
        }
    }

    $("#divBaseMissions").on("click", ".removeBaseMission", function() {
        $(this).parent().remove();
    });
    $("#divBaseMissions").on("change", "select", function () {
        processSelect($(this));
    });
    $(function() {
        $("#divBaseMissions select").each(function (index, element) {
            processSelect(element);
        });
    });
    
})();