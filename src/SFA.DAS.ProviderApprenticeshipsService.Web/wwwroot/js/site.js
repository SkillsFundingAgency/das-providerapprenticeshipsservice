const selectElements = $('.das-autocomplete')
selectElements.each(function () {
    const form = $(this).closest('form');
    accessibleAutocomplete.enhanceSelectElement({
        selectElement: this,
        minLength: 3,
        autoselect: true,
        defaultValue: '',
        displayMenu: 'overlay',
        placeholder: $(this).data('placeholder') || '',
        onConfirm: function (opt) {
            const txtInput = document.querySelector('#' + this.id);
            const searchString = opt || txtInput.value;
            const requestedOption = [].filter.call(this.selectElement.options,
                function (option) {
                    return (option.textContent || option.innerText) === searchString
                }
            )[0];
            if (requestedOption) {
                requestedOption.selected = true;
            } else {
                this.selectElement.selectedIndex = 0;
            }
        }
    });
    form.on('submit', function() {
        $('.autocomplete__input').each(function() {
            const that = $(this);
            if (that.val().length === 0) {
                const fieldId = that.attr('id'),
                selectField = $('#' + fieldId + '-select');
                selectField[0].selectedIndex = 0;
            }
        });
    });
})

// AUTOCOMPLETE

const $keywordsInput = $('#search-location');
const $submitOnConfirm = $('#search-location').data('submit-on-selection');
const $defaultValue = $('#search-location').data('default-value');
if ($keywordsInput.length > 0) {
    $keywordsInput.wrap('<div id="autocomplete-container" class="das-autocomplete-wrap"></div>');
    const container = document.querySelector('#autocomplete-container');
    const apiUrl = '/locations';
    $(container).empty();
    function getSuggestions(query, updateResults) {
        let results = [];
        $.ajax({
            url: apiUrl,
            type: "get",
            dataType: 'json',
            data: { searchTerm: query }
        }).done(function (data) {
            results = data.locations.map(function (r) {
                return r.name;
            });
            updateResults(results);
        });
    }
    function onConfirm() {
        const $form = $(this.element).closest('form');
        setTimeout(function(){
            if ($form && $submitOnConfirm) {
                $form.submit()
            }
        },200);
    }

    accessibleAutocomplete({
        element: container,
        id: 'search-location',
        name: 'location',
        displayMenu: 'overlay',
        showNoOptionsFound: false,
        minLength: 2,
        source: getSuggestions,
        placeholder: "",
        onConfirm: onConfirm,
        defaultValue: $defaultValue,
        confirmOnBlur: false,
        autoselect: true
    });
}

// BACK LINK
// If users history-1 does not come from this site, 
// then show a link to homepage

const $backLinkOrHome = $('.das-js-back-link-or-home');
const backLinkOrHome = function () {

    const referrer = document.referrer;

    const backLink = $('<a>')
        .attr({'href': '#', 'class': 'govuk-back-link'})
        .text('Back')
        .on('click', function (e) {
            window.history.back();
            e.preventDefault();
        });

    if (referrer && referrer !== document.location.href) {
        $backLinkOrHome.replaceWith(backLink);
    }
}

if ($backLinkOrHome) {
    backLinkOrHome();
}

// NUMBER OF APPRENTICES RADIO
const radioNoOfApp = $('#NumberOfApprenticesKnown-false');
if (radioNoOfApp) {
    radioNoOfApp.on('click', function () {
        $('#NumberOfApprentices').val('');
    })
}

