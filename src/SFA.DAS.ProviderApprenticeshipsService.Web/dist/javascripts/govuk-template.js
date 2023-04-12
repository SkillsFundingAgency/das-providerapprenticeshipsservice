(function () {
  "use strict"
  var root = this;
  if(typeof root.GOVUK === 'undefined') { root.GOVUK = {}; }

  /*
    Cookie methods
    ==============

    Usage:

      Setting a cookie:
      GOVUK.cookie('hobnob', 'tasty', { days: 30 });

      Reading a cookie:
      GOVUK.cookie('hobnob');

      Deleting a cookie:
      GOVUK.cookie('hobnob', null);
  */
  GOVUK.cookie = function(name, value, options) {
      if (typeof value !== 'undefined') {
          if (value === false || value === null) {
              return GOVUK.setCookie(name, '', { days: -1 });
          } else {
              return GOVUK.setCookie(name, value, options);
          }
      } else {
          return GOVUK.getCookie(name);
      }
  };
  GOVUK.setCookie = function(name, value, options) {
      if (typeof options === 'undefined') {
          options = {};
      }
      var cookieString = name + "=" + value + "; path=/";
      if (options.days) {
          var date = new Date();
          date.setTime(date.getTime() + (options.days * 24 * 60 * 60 * 1000));
          cookieString = cookieString + "; expires=" + date.toGMTString();
      }
      if (document.location.protocol == 'https:') {
          cookieString = cookieString + "; Secure";
      }
      document.cookie = cookieString;
  };
  GOVUK.getCookie = function(name) {
      var nameEQ = name + "=";
      var cookies = document.cookie.split(';');
      for (var i = 0, len = cookies.length; i < len; i++) {
          var cookie = cookies[i];
          while (cookie.charAt(0) == ' ') {
              cookie = cookie.substring(1, cookie.length);
          }
          if (cookie.indexOf(nameEQ) === 0) {
              return decodeURIComponent(cookie.substring(nameEQ.length));
          }
      }
      return null;
  };
  GOVUK.getEnv = function() {
      var domain = window.location.hostname;
      if (domain.indexOf("at-") >= 0) {
          return "AT";
      }
      if (domain.indexOf("test-") >= 0) {
          return "TEST";
      }
      if (domain.indexOf("test2-") >= 0) {
          return "TEST2";
      }
      if (domain.indexOf("pp-") >= 0) {
          return "PP";
      }
      return "";
  };
}).call(this);

(function () {
  "use strict"
  var root = this;
  if(typeof root.GOVUK === 'undefined') { root.GOVUK = {}; }

  GOVUK.addCookieMessage = function () {
    var message = document.getElementById('global-cookie-message'),
        hasCookieMessage = (message && GOVUK.cookie('seen_cookie_message') === null);

    if (hasCookieMessage) {
      message.style.display = 'block';
      GOVUK.cookie('seen_cookie_message', 'yes', { days: 28 });
    }
  };
}).call(this);

(function() {
    "use strict"

    // add cookie message
    if (window.GOVUK && GOVUK.addCookieMessage) {
        GOVUK.addCookieMessage();
    }

    // header navigation toggle
    if (document.querySelectorAll && document.addEventListener) {
        var els = document.querySelectorAll('.js-header-toggle'),
            i,
            _i;
        for (i = 0, _i = els.length; i < _i; i++) {
            els[i].addEventListener('click',
                function(e) {
                    e.preventDefault();
                    var target = document.getElementById(this.getAttribute('href').substr(1)),
                        targetClass = target.getAttribute('class') || '',
                        sourceClass = this.getAttribute('class') || '';

                    if (targetClass.indexOf('js-visible') !== -1) {
                        target.setAttribute('class', targetClass.replace(/(^|\s)js-visible(\s|$)/, ''));
                    } else {
                        target.setAttribute('class', targetClass + " js-visible");
                    }
                    if (sourceClass.indexOf('js-hidden') !== -1) {
                        this.setAttribute('class', sourceClass.replace(/(^|\s)js-hidden(\s|$)/, ''));
                    } else {
                        this.setAttribute('class', sourceClass + " js-hidden");
                    }
                });
        }
    }
}).call(this);

(function(global) {
    'use strict'

    var GOVUK = global.GOVUK || {}

    GOVUK.details = {
        NATIVE_DETAILS: typeof document.createElement('details').open === 'boolean',
        KEY_ENTER: 13,
        KEY_SPACE: 32,

        // Create a started flag so we can prevent the initialisation
        // function firing from both DOMContentLoaded and window.onload
        started: false,

        // Add event construct for modern browsers or IE
        // which fires the callback with a pre-converted target reference
        addEvent: function(node, type, callback) {
            if (node.addEventListener) {
                node.addEventListener(type,
                    function(e) {
                        callback(e, e.target)
                    },
                    false)
            } else if (node.attachEvent) {
                node.attachEvent('on' + type,
                    function(e) {
                        callback(e, e.srcElement)
                    })
            }
        },

        removeEvent: function(node, type) {
            if (node.removeEventListener) {
                node.removeEventListener(type,
                    function(e) {
                    },
                    false)
            } else if (node.detachEvent) {
                node.detachEvent('on' + type,
                    function(e) {
                    })
            }
        },

        // Cross-browser character code / key pressed
        charCode: function(e) {
            return (typeof e.which === 'number') ? e.which : e.keyCode
        },

        // Cross-browser preventing default action
        preventDefault: function(e) {
            if (e.preventDefault) {
                e.preventDefault()
            } else {
                e.returnValue = false
            }
        },

        // Handle cross-modal click events
        addClickEvent: function(node, callback) {
            GOVUK.details.addEvent(node,
                'keypress',
                function(e, target) {
                    // When the key gets pressed - check if it is enter or space
                    if (GOVUK.details.charCode(e) === GOVUK.details.KEY_ENTER ||
                        GOVUK.details.charCode(e) === GOVUK.details.KEY_SPACE) {
                        if (target.nodeName.toLowerCase() === 'summary') {
                            // Prevent space from scrolling the page
                            // and enter from submitting a form
                            GOVUK.details.preventDefault(e)
                            // Click to let the click event do all the necessary action
                            if (target.click) {
                                target.click()
                            } else {
                                // except Safari 5.1 and under don't support .click() here
                                callback(e, target)
                            }
                        }
                    }
                })

            // Prevent keyup to prevent clicking twice in Firefox when using space key
            GOVUK.details.addEvent(node,
                'keyup',
                function(e, target) {
                    if (GOVUK.details.charCode(e) === GOVUK.details.KEY_SPACE) {
                        if (target.nodeName === 'SUMMARY') {
                            GOVUK.details.preventDefault(e)
                        }
                    }
                })

            GOVUK.details.addEvent(node,
                'click',
                function(e, target) {
                    callback(e, target)
                })
        },

        // Get the nearest ancestor element of a node that matches a given tag name
        getAncestor: function(node, match) {
            do {
                if (!node || node.nodeName.toLowerCase() === match) {
                    break
                }
                node = node.parentNode
            } while (node)

            return node
        },

        // Initialisation function
        addDetailsPolyfill: function(list, container) {
            container = container || document.body
            // If this has already happened, just return
            // else set the flag so it doesn't happen again
            if (GOVUK.details.started) {
                return
            }
            GOVUK.details.started = true
            // Get the collection of details elements, but if that's empty
            // then we don't need to bother with the rest of the scripting
            if ((list = container.getElementsByTagName('details')).length === 0) {
                return
            }
            // else iterate through them to apply their initial state
            var n = list.length
            var i = 0
            for (i; i < n; i++) {
                var details = list[i]

                // Save shortcuts to the inner summary and content elements
                details.__summary = details.getElementsByTagName('summary').item(0)
                details.__content = details.getElementsByTagName('div').item(0)

                if (!details.__summary || !details.__content) {
                    return
                }
                // If the content doesn't have an ID, assign it one now
                // which we'll need for the summary's aria-controls assignment
                if (!details.__content.id) {
                    details.__content.id = 'details-content-' + i
                }

                // Add ARIA role="group" to details
                details.setAttribute('role', 'group')

                // Add role=button to summary
                details.__summary.setAttribute('role', 'button')

                // Add aria-controls
                details.__summary.setAttribute('aria-controls', details.__content.id)

                // Set tabIndex so the summary is keyboard accessible for non-native elements
                // http://www.saliences.com/browserBugs/tabIndex.html
                if (!GOVUK.details.NATIVE_DETAILS) {
                    details.__summary.tabIndex = 0
                }

                // Detect initial open state
                var openAttr = details.getAttribute('open') !== null
                if (openAttr === true) {
                    details.__summary.setAttribute('aria-expanded', 'true')
                    details.__content.setAttribute('aria-hidden', 'false')
                } else {
                    details.__summary.setAttribute('aria-expanded', 'false')
                    details.__content.setAttribute('aria-hidden', 'true')
                    if (!GOVUK.details.NATIVE_DETAILS) {
                        details.__content.style.display = 'none'
                    }
                }

                // Create a circular reference from the summary back to its
                // parent details element, for convenience in the click handler
                details.__summary.__details = details

                // If this is not a native implementation, create an arrow
                // inside the summary
                if (!GOVUK.details.NATIVE_DETAILS) {
                    var twisty = document.createElement('i')

                    if (openAttr === true) {
                        twisty.className = 'arrow arrow-open'
                        twisty.appendChild(document.createTextNode('\u25bc'))
                    } else {
                        twisty.className = 'arrow arrow-closed'
                        twisty.appendChild(document.createTextNode('\u25ba'))
                    }

                    details.__summary.__twisty = details.__summary.insertBefore(twisty, details.__summary.firstChild)
                    details.__summary.__twisty.setAttribute('aria-hidden', 'true')
                }
            }

            // Bind a click event to handle summary elements
            GOVUK.details.addClickEvent(container,
                function(e, summary) {
                    if (!(summary = GOVUK.details.getAncestor(summary, 'summary'))) {
                        return true
                    }
                    return GOVUK.details.statechange(summary)
                })
        },

        // Define a statechange function that updates aria-expanded and style.display
        // Also update the arrow position
        statechange: function(summary) {
            var expanded = summary.__details.__summary.getAttribute('aria-expanded') === 'true'
            var hidden = summary.__details.__content.getAttribute('aria-hidden') === 'true'

            summary.__details.__summary.setAttribute('aria-expanded', (expanded ? 'false' : 'true'))
            summary.__details.__content.setAttribute('aria-hidden', (hidden ? 'false' : 'true'))

            if (!GOVUK.details.NATIVE_DETAILS) {
                summary.__details.__content.style.display = (expanded ? 'none' : '')

                var hasOpenAttr = summary.__details.getAttribute('open') !== null
                if (!hasOpenAttr) {
                    summary.__details.setAttribute('open', 'open')
                } else {
                    summary.__details.removeAttribute('open')
                }
            }

            if (summary.__twisty) {
                summary.__twisty.firstChild.nodeValue = (expanded ? '\u25ba' : '\u25bc')
                summary.__twisty.setAttribute('class', (expanded ? 'arrow arrow-closed' : 'arrow arrow-open'))
            }

            return true
        },

        destroy: function(node) {
            GOVUK.details.removeEvent(node, 'click')
        },

        // Bind two load events for modern and older browsers
        // If the first one fires it will set a flag to block the second one
        // but if it's not supported then the second one will fire
        init: function($container) {
            GOVUK.details.addEvent(document, 'DOMContentLoaded', GOVUK.details.addDetailsPolyfill)
            GOVUK.details.addEvent(window, 'load', GOVUK.details.addDetailsPolyfill)
        }
    }
    global.GOVUK = GOVUK
})(window);
