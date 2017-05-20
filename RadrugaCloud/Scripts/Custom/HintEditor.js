var hintEditor = (function () {
    //init
    var $hintsDiv = $("#divHints");

    $hintsDiv.on("click", ".removeHint", function () {
        var removedHintDiv = $(this).parent().parent();
        $(this).parent().remove();
        hintEditor.updateHints(removedHintDiv);
    });

    $hintsDiv.on("change", ".js-HintScore", function () {
        $(this).next().text(this.value);
    }).next().val(this.value);

    $hintsDiv.on("change", ".js-HintType", function () {
        var selectedType = $(this).find(":selected").val();        
        if (selectedType == 0) {
            $(this).siblings(".js-HintText").show();
        }
        else {
            $(this).siblings(".js-HintText").hide();
        }


    });
    //init end

    this.updateHints = function () {
        var hints = $hintsDiv.children("div");
        hints.each(function (hintIndex, hintDiv) {
            var $hintDiv = $(hintDiv);
            $hintDiv.find(".js-HintId").attr("name", "Hints[" + hintIndex + "].Id");
            $hintDiv.find(".js-HintType").attr("name", "Hints[" + hintIndex + "].Type").change();
            $hintDiv.find(".js-HintText").attr("name", "Hints[" + hintIndex + "].Text");
            $hintDiv.find(".js-HintScore").attr("name", "Hints[" + hintIndex + "].Score").change();
        });
    };

    return this;
})();

$(function () {
    hintEditor.updateHints();
});

