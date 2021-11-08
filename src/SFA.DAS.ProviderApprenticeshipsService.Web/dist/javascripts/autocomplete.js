var autocompleteInputs = document.querySelectorAll(".app-autocomplete");

if (autocompleteInputs.length > 0) {

    for (var i = 0; i < autocompleteInputs.length; i++) {

        var input = autocompleteInputs[i]
        var container = document.createElement('div');
        var apiUrl = input.dataset.autocompleteUrl;

        container.className = "das-autocomplete-wrap"
        input.parentNode.replaceChild(container, input);

        var getSuggestions = function (query, updateResults) {
            ;
        };

        accessibleAutocomplete({
            element: container,
            id: input.id,
            name: input.name,
            defaultValue: input.value,
            displayMenu: 'overlay',
            showNoOptionsFound: false,
            minLength: 3,
            source: autoCompleteSource,
            placeholder: "",
            confirmOnBlur: false,
            autoselect: true
        });
    }
}