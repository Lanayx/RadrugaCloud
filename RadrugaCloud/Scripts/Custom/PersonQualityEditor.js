(function() {
    $("#divPersonQualities").on("click", ".removePersonQuality", function ()
    {
        $(this).parent().remove();
    });
})();