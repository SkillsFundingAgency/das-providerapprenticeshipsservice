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

// variables
var totalChars = 20, countTextBox = $('#ProviderRef'), charsCountEl = $('#countchars');
charsCountEl.text(totalChars);
var thisChars = countTextBox.val().replace(/{.*}/g, '').length;

// function to trigger on page load
charLeft(countTextBox, thisChars, totalChars);

countTextBox.keyup(function (e) {
    var $this = $(this);
    var thisChars = $this.val().replace(/{.*}/g, '').length;

    if (thisChars > totalChars) {
        $this.val($this.val().slice(0, totalChars));
        $("#charCount").addClass('limit-reached');
    } else {
        charLeft($this, thisChars, totalChars);
    }
});

function charLeft($this, thisChars, totalChars) {
    charsCountEl.text(totalChars - thisChars);
    $("#charCount").removeClass('limit-reached');
}