var sfa = sfa || {};
    
sfa.homePage = {
    init: function () {
        this.startButton();
        this.toggleRadios();
    },
    startButton: function () {
        var that = this;
        $('#create_account').on('click touchstart', function (e) {
            var isYesClicked = $('#everything-yes').prop('checked'),
                errorShown = $('body').data('shownError') || false;
            if (!isYesClicked && !errorShown) {
                e.preventDefault();
                that.showError();
            }
        });
    }, 
    showError: function() {
        $('#have-not-got-everything').removeClass("js-hidden").attr("aria-hidden");
        $('body').data('shownError', true);
    },
    toggleRadios: function () {
        var radios = $('input[type=radio][name=everything-you-need]');
        radios.on('change', function () {

            radios.each(function () {
                if ($(this).prop('checked')) {
                    var target = $(this).parent().data("target");
                    $("#" + target).removeClass("js-hidden").attr("aria-hidden");
                } else {
                    var target = $(this).parent().data("target");
                    $("#" + target).addClass("js-hidden").attr("aria-hidden", "true");
                }
            });

        });
    }
}

var selectionButtons = new GOVUK.SelectionButtons("label input[type='radio'], label input[type='checkbox']");

// cohorts bingo balls - clickable block
$(".clickable").on('click touchstart', (function () {
    window.location = $(this).find("a").attr("href");
    return false;
}));


// character limitation script
$('#charCount').show(); // javascript enabled version only
$('#charCount-noJS').hide(); // javascript disabled version only

var totalChars = 20; //Total characters allowed in textarea
var countTextBox = $('#ProviderRef'); // Textarea input box
var charsCountEl = $('#countchars'); // Remaining characters
charsCountEl.text(totalChars); //initial value of countchars element

countTextBox.keyup(function () { //user releases a key on the keyboard
    var thisChars = this.value.replace(/{.*}/g, '').length; //get chars count in textarea
    if (thisChars > totalChars) //if we have more chars than it should be
        {
            var CharsToDel = (thisChars - totalChars); // total extra chars to delete
            this.value = this.value.substring(0, this.value.length - CharsToDel); //remove excess chars from textarea
            $("#charCount").addClass('limit-reached');       
    }
    else {
            charsCountEl.text(totalChars - thisChars); //count remaining chars
            $("#charCount").removeClass('limit-reached');
        }
});
