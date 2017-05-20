var questionEditor = (function ()
{
    //init
    var $divOptions = $("#divOptions");

    $divOptions.on("click", ".removeOption", function ()
    {
        $(this).parent().remove();
        questionEditor.updateOption();
    });

    $divOptions.on("click", ".removePersonQuality", function ()
    {
        var currentPersonQualitiesDiv = $(this).parent().parent();
        $(this).parent().remove();
        questionEditor.updatePersonQuality(currentPersonQualitiesDiv);
    });

    $divOptions.on("change","input[type=range]", function ()
    {
        $(this).next().text(this.value);
    }).next().val(this.value);

    //init end


    var updatePersonQualityInteral = function(currentPersonQualitiesDiv, optionIndex) {
        var personQualities = currentPersonQualitiesDiv.children("div");
        personQualities.each(function(persontTypeIndex, personQualityDiv) {
            var $perstonTypeDiv = $(personQualityDiv);
            $perstonTypeDiv.find("select").attr("name", "QuestionOptions[" + optionIndex + "].PersonQualitiesWithScores[" + persontTypeIndex + "].PersonQualityId");
            $perstonTypeDiv.find("input[type=range]").attr("name", "QuestionOptions[" + optionIndex + "].PersonQualitiesWithScores[" + persontTypeIndex + "].Score").change();
        });
    };

    this.updateOption = function() {
        var allOptions = $("#divOptions").children("div");
        allOptions.each(function(optionIndex, questionOptionDiv) {
            var $questionOptionDiv = $(questionOptionDiv);

            var currentPersonQualitiesDivId = "divPersonQualitiesWithScores" + optionIndex;
            var currentPersonQualitiesDiv = $questionOptionDiv.find("div[id^='divPersonQualitiesWithScores']");

            currentPersonQualitiesDiv.attr("id", currentPersonQualitiesDivId).attr("data-option-index", optionIndex);
            $questionOptionDiv.find(".btn").attr("data-ajax-update", "#" + currentPersonQualitiesDivId);
            $questionOptionDiv.find(".js-optionText").attr("name", "QuestionOptions[" + optionIndex + "].Text");
            $questionOptionDiv.find(".js-optionNextQuestionId").attr("name", "QuestionOptions[" + optionIndex + "].NextQuestionId");
            $questionOptionDiv.find("input[type=hidden]").attr("name", "QuestionOptions[" + optionIndex + "].Number").val(optionIndex);

            updatePersonQualityInteral(currentPersonQualitiesDiv, optionIndex);

        });
    };

    this.updatePersonQuality = function(currentPersonQualitiesDiv) {
        var optionIndex = currentPersonQualitiesDiv.attr("data-option-index");

        updatePersonQualityInteral(currentPersonQualitiesDiv, optionIndex);
    };

    return this;
})();

$(function() {
    questionEditor.updateOption();
});

