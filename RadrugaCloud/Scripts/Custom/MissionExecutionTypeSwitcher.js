$(function() {
    switchMissionType();

    $('#ExecutionTypeSelect').on("change", switchMissionType);

    function switchMissionType() {
        var select = $('#ExecutionTypeSelect');
        var val = select.val();
        var actionUrl = select.data("actionurl");
        $.ajax(
        {
            url: actionUrl,
            type: 'GET',
            data: { executionType: val },
            contentType: "application/json",
            success: changeView
        });
    }

    function changeView(partialView) {
        var viewDiv = $('#missionTypeDetails');
        viewDiv.html(partialView);
    }
});