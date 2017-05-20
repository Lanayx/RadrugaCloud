(function() {
    var hdnInput = $("#hdnMissionType");
    $("input[type=checkbox]").change(function() {
        if ($(this).is(':checked')) {
            hdnInput.val(parseInt(hdnInput.val()) + parseInt($(this).attr("data-val")));
        } else {
            hdnInput.val(hdnInput.val() - $(this).attr("data-val"));
        }
    });
})();