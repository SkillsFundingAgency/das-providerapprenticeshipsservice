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


// Cookie Banner and Settings

function CookieBanner(module) {
    this.module = module;
    this.settings = {
        seenCookieName: 'DASSeenCookieMessage',
        env: window.GOVUK.getEnv(),
        cookiePolicy: {
            AnalyticsConsent: false
        }
    }
    if (!window.GOVUK.cookie(this.settings.seenCookieName + this.settings.env)) {
        this.start();
    }
}

CookieBanner.prototype.start = function () {
    this.module.cookieBanner = this.module.querySelector('.das-cookie-banner');
    this.module.cookieBannerInnerWrap = this.module.querySelector('.das-cookie-banner__wrapper');
    this.module.cookieBannerConfirmationMessage = this.module.querySelector('.das-cookie-banner__confirmation');
    this.setupCookieMessage();
}

CookieBanner.prototype.setupCookieMessage = function () {
    this.module.hideLink = this.module.querySelector('button[data-hide-cookie-banner]');
    this.module.acceptCookiesButton = this.module.querySelector('button[data-accept-cookies]');

    if (this.module.hideLink) {
        this.module.hideLink.addEventListener('click', this.hideCookieBanner.bind(this));
    }

    if (this.module.acceptCookiesButton) {
        this.module.acceptCookiesButton.addEventListener('click', this.acceptAllCookies.bind(this));
    }
    this.showCookieBanner();
}

CookieBanner.prototype.showCookieBanner = function () {
    var cookiePolicy = this.settings.cookiePolicy;
    this.module.cookieBanner.style.display = 'block';
    that = this;

    Object.keys(cookiePolicy).forEach(function (cookieName) {
        window.GOVUK.cookie(cookieName + that.settings.env, cookiePolicy[cookieName].toString(), { days: 365 });
    });
}

CookieBanner.prototype.hideCookieBanner = function () {
    this.module.cookieBanner.style.display = 'none';
    window.GOVUK.cookie(this.settings.seenCookieName + this.settings.env, true, { days: 365 });
}

CookieBanner.prototype.acceptAllCookies = function () {
    this.module.cookieBannerInnerWrap.style.display = 'none';
    this.module.cookieBannerConfirmationMessage.style.display = 'block';
    that = this;

    window.GOVUK.cookie(this.settings.seenCookieName + this.settings.env, true, { days: 365 });

    Object.keys(this.settings.cookiePolicy).forEach(function (cookieName) {
        window.GOVUK.cookie(cookieName + that.settings.env, true, { days: 365 });
    });
}

function CookieSettings(module, options) {
    this.module = module;
    this.settings = {
        seenCookieName: 'DASSeenCookieMessage',
        env: window.GOVUK.getEnv(),
        cookiePolicy: {
            AnalyticsConsent: false
        }
    }

    // Hide cookie banner on settings page
    var cookieBanner = document.querySelector('.das-cookie-banner');
    cookieBanner.style.display = 'none';

    this.start();
}

CookieSettings.prototype.start = function () {
    this.setRadioValues();
    this.module.addEventListener('submit', this.formSubmitted.bind(this));
}

CookieSettings.prototype.setRadioValues = function () {
    var cookiePolicy = this.settings.cookiePolicy;
    that = this;

    Object.keys(cookiePolicy).forEach(function (cookieName) {
        
        var existingCookie = window.GOVUK.cookie(cookieName + that.settings.env),
            radioButtonValue = existingCookie !== null ? existingCookie : cookiePolicy[cookieName],
            radioButton = document.querySelector('input[name=cookies-' + cookieName + '][value=' + (radioButtonValue === 'true' ? 'on' : 'off') + ']')


        radioButton.parentElement.classList.add("selected");

        radioButton.checked = true;
    });
}

CookieSettings.prototype.formSubmitted = function (event) {

    event.preventDefault();

    var formInputs = event.target.getElementsByTagName("input"),
        button = event.target.getElementsByTagName("button");

    that = this;

    for (var i = 0; i < formInputs.length; i++) {
        var input = formInputs[i];
        if (input.checked) {
            var name = input.name.replace('cookies-', '')
            var value = input.value === "on"
            window.GOVUK.setCookie(name + that.settings.env, value, { days: 365 });
        }
    }

    window.GOVUK.setCookie(this.settings.seenCookieName + that.settings.env, true, { days: 365 });

    if (button.length > 0) {
        button[0].removeAttribute('disabled');
    }

    if (this.settings.isModal) {
        document.location.href = document.location.pathname;
    }

    if (!this.settings.isModal) {
        this.showConfirmationMessage();
    }
}

CookieSettings.prototype.showConfirmationMessage = function () {
    var confirmationMessage = document.querySelector('div[data-cookie-confirmation]');
    var previousPageLink = document.querySelector('.cookie-settings__prev-page');
    var referrer = CookieSettings.prototype.getReferrerLink();

    document.body.scrollTop = document.documentElement.scrollTop = 0;

    if (referrer && referrer !== document.location.pathname) {
        previousPageLink.href = referrer;
        previousPageLink.style.display = "inline-block";
    } else {
        previousPageLink.style.display = "none";
    }

    confirmationMessage.style.display = "block";
}

CookieSettings.prototype.getReferrerLink = function () {
    return document.referrer ? new URL(document.referrer).pathname : false;
}

CookieSettings.prototype.hideCookieSettings = function () {
    document.getElementById('cookie-settings').style.display = 'none';
}

CookieSettings.prototype.modalControls = function () {
    var closeLink = document.createElement('a');
    var closeLinkText = document.createTextNode("Close cookie preferences");
    closeLink.appendChild(closeLinkText);
    closeLink.href = document.location.pathname;
    closeLink.classList.add('das-cookie-settings__close-modal');
    this.module.appendChild(closeLink);
}




window.GOVUK = window.GOVUK || {}

window.GOVUK.cookie = function (name, value, options) {
    if (typeof value !== 'undefined') {
        if (value === false || value === null) {
            return window.GOVUK.setCookie(name, '', { days: -1 })
        } else {
            // Default expiry date of 30 days
            if (typeof options === 'undefined') {
                options = { days: 30 }
            }
            return window.GOVUK.setCookie(name, value, options)
        }
    } else {
        return window.GOVUK.getCookie(name)
    }
}

window.GOVUK.setCookie = function (name, value, options) {
    if (typeof options === 'undefined') {
        options = {}
    }
    var cookieString = name + '=' + value + '; path=/'

    if (options.days) {
        var date = new Date()
        date.setTime(date.getTime() + (options.days * 24 * 60 * 60 * 1000))
        cookieString = cookieString + '; expires=' + date.toGMTString()
    }
    if (!options.domain) {
        options.domain = window.GOVUK.getDomain()
    }

    if (document.location.protocol === 'https:') {
        cookieString = cookieString + '; Secure'
    }
    document.cookie = cookieString + ';domain=' + options.domain
}

window.GOVUK.getCookie = function (name) {
    var nameEQ = name + '='
    var cookies = document.cookie.split(';')
    for (var i = 0, len = cookies.length; i < len; i++) {
        var cookie = cookies[i]
        while (cookie.charAt(0) === ' ') {
            cookie = cookie.substring(1, cookie.length)
        }
        if (cookie.indexOf(nameEQ) === 0) {
            return decodeURIComponent(cookie.substring(nameEQ.length))
        }
    }
    return null
}

window.GOVUK.getDomain = function () {
    return window.location.hostname !== 'localhost'
        ? '.' + window.location.hostname.slice(window.location.hostname.indexOf('.') + 1)
        : window.location.hostname;
}





// Cookie Banner GDS style
var $cookieBanner = document.querySelector('[data-module="cookie-banner"]');
if ($cookieBanner != null) {
    new CookieBanner($cookieBanner);
}

// Cookie Settings Page
var $cookieSettings = document.querySelector('[data-module="cookie-settings"]');
if ($cookieSettings != null) {
    var $cookieSettingsOptions = $cookieSettings.dataset.options;
    new CookieSettings($cookieSettings, $cookieSettingsOptions);
}