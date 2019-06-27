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

sfa.navigation = {
    elems: {
        userNav: $('nav#user-nav > ul'),
        levyNav: $('ul#global-nav-links')
    },
    init: function () {
        this.setupMenus(this.elems.userNav);
        this.setupEvents(this.elems.userNav);
    },
    setupMenus: function (menu) {
        menu.find('ul').addClass("js-hidden").attr("aria-expanded", "false");
    },
    setupEvents: function (menu) {
        var that = this;
        menu.find('li.has-sub-menu > a').on('click', function (e) {
            var $that = $(this);
            that.toggleMenu($that, $that.next('ul'));
            e.stopPropagation();
            e.preventDefault();
        });
        // Focusout event on the links in the dropdown menu
        menu.find('li.has-sub-menu > ul > li > a').on('focusout', function (e) {
            // If its the last link in the drop down menu, then close
            var $that = $(this);
            if ($(this).parent().is(':last-child')) {
                that.toggleMenu($that, $that.next('ul'));
            }
        });

    },
    toggleMenu: function (link, subMenu) {
        var $li = link.parent();
        if ($li.hasClass("open")) {
            // Close menu
            $li.removeClass("open");
            subMenu.addClass("js-hidden").attr("aria-expanded", "false");
        } else {
            // Open menu
            this.closeAllOpenMenus();
            $li.addClass("open");
            subMenu.removeClass("js-hidden").attr("aria-expanded", "true");
        }
    },
    closeAllOpenMenus: function () {
        this.elems.userNav.find('li.has-sub-menu.open').removeClass('open').find('ul').addClass("js-hidden").attr("aria-expanded", "false");
        this.elems.levyNav.find('li.open').removeClass('open').addClass("js-hidden").attr("aria-expanded", "false");
    },
    linkSettings: function () {
        var $settingsLink = $('a#link-settings'),
            that = this;
        this.toggleUserMenu();
        $settingsLink.attr("aria-expanded", "false");
        $settingsLink.on('click touchstart', function (e) {
            var target = $(this).attr('href');
            $(this).toggleClass('open');
            that.toggleUserMenu();
            e.preventDefault();
        });
    },
    toggleUserMenu: function () {
        var $userNavParent = this.elems.userNav.parent();
        if ($userNavParent.hasClass("close")) {
            //open it
            $userNavParent.removeClass("close").attr("aria-expanded", "true");
        } else {
            // close it 
            $userNavParent.addClass("close").attr("aria-expanded", "false");
        }
    }
}

sfa.navigation.init();
$('ul#global-nav-links').collapsableNav();

var selectionButtons = new GOVUK.SelectionButtons("label input[type='radio'], label input[type='checkbox']");

// cohorts bingo balls - clickable block
$(".clickable").on('click touchstart', (function () {
    window.location = $(this).find("a").attr("href");
    return false;
}));

// apprentice filter page :: expand/collapse functionality
$('.container-head').on('click touchstart',(function () {
    $(this).toggleClass('showHide');
    $(this).next().toggleClass("hideOptions");

}));

//floating menu
$(window).scroll(function () {
    if ($(window).scrollTop() >= 140) {
        $('#floating-menu').addClass('fixed-header');
    }
    else {
        $('#floating-menu').removeClass('fixed-header');
    }
});


//clear search box text

var placeholderText = $('.js-enabled #search-input').data('default-value');

window.onload = function () {

    if ($('.js-enabled #search-input').val() === "") {
        $('.js-enabled #search-input').addClass('placeholder-text');
        $('.js-enabled #search-input').val(placeholderText);
    }
};

$("#search-input").on("focus click touchstart", (function () {
    $('.js-enabled #search-input').removeClass('placeholder-text');
    if ($(this).val() === placeholderText)
        $(this).val("");
}));

$("#search-input").on("blur", (function () {
    if ($(this).val() === "") {
        $('.js-enabled #search-input').addClass('placeholder-text');
        $(this).val(placeholderText);
    }
}));

