var personQualityWithScoreEditor = (function ()
{
    //init
    var $qualitiesDiv = $("#divPersonQualitiesWithScores");

    $qualitiesDiv.on("click", ".removePersonQuality", function ()
    {
        var currentPersonQualitiesDiv = $(this).parent().parent();
        $(this).parent().remove();
        personQualityWithScoreEditor.updatePersonQuality(currentPersonQualitiesDiv);
    });

    $qualitiesDiv.on("change","input[type=range]", function ()
    {
        $(this).next().text(this.value);
    }).next().val(this.value);

    //init end

    this.updatePersonQuality = function() {
        var personQualities = $qualitiesDiv.children("div");
        personQualities.each(function (persontTypeIndex, personQualityDiv) {
            var $perstonTypeDiv = $(personQualityDiv);
            $perstonTypeDiv.find("select").attr("name", "PersonQualitiesWithScores[" + persontTypeIndex + "].PersonQualityId");
            $perstonTypeDiv.find("input[type=range]").attr("name", "PersonQualitiesWithScores[" + persontTypeIndex + "].Score").change();
        });
    };

    return this;
})();

$(function() {
    personQualityWithScoreEditor.updatePersonQuality();
});

