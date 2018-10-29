// <reference path="~/js/vendors/jquery.js" />
// <reference path="~/js/vendors/bootstrap.js" />

/*======================================================================*/
// Plato
// © InstantASP Ltd.
/*======================================================================*/

if (typeof jQuery === "undefined") {
    throw new Error("Plato requires jQuery");
}

$(function (win, doc, $) {

    "use strict";

    /* $.Plato */
    /* ---------------------------------------------*/

    win.$.Plato = {};

    /* Default options */
    win.$.Plato.Options = {
        debug: true,
        url: "",
        locale: "en-US",
        apiKey: "",
        csrfHeaderName: "X-Csrf-Token",
        csrfCookieName: "",
        // UI tooltips
        BSToolTipEnabled: true,
        BSToolTipAlternativeSelector: "[data-provide='tooltip']",
        BSToolTipSelector: "[data-toggle='tooltip']",
        MagnificSelector: "[data-toggle='dialog']",
        AvatarUploadSelector: "[data-upload='avatar']",
        getCsrfCookieToken: function () {
            if (this.csrfCookieName !== "") {
                var storage = win.$.Plato.Storage;
                var cookie = storage.getCookie(this.csrfCookieName);
                if (cookie) {
                    return cookie;
                }
            }
            return "";
        }
    };

    /* Simple logging */
    win.$.Plato.Logger = {
        info: "Info",
        warning: "Warning",
        error: "Error",
        prevLogDate: null,
        logInfo: function(message) {
            this.log(this.info, message);
        },
        logWarning: function(message) {
            this.log(this.warning, message);
        },
        logError: function(message) {
            this.log(this.error, message);
        },
        log: function(level, message) {
            if (!$.Plato.Options.debug) {
                return;
            }
            var difference = this.getDifferenceInMilliseconds();
            this.prevLogDate = new Date();
            console.log(level + ": " + message + " - " + difference + "ms elapsed since previous log entry.");
        },
        getDifferenceInMilliseconds: function() {
            if (this.prevLogDate === null) {
                this.prevLogDate = new Date();
            } else {
                return (this.prevLogDate - new Date()) / 10000;
            }
            return 0;
        }
    };

    /* Client side localization */
    win.$.Plato.Locale = {
        init: function () {

            var context = win.$.Plato.Context;
            if (!context) {
                throw new Error("Plato.Locale requires a valid Plato.Context object");
            }

            var opts = context.options();
            if (!opts) {
                throw new Error("$.Plato.Locale requires a valid $.Plato.Options object");
            }

            context.logger.logInfo("Initializing $.Plato.Locale");

            // We need a locale
            if (opts.locale === "") {
                context.logger.logError("$.Plato.Locale could not be initialized as the $.Plato.Options.locale property is empty!");
                return;
            }

            // We need a url
            if (opts.url === "") {
                context.logger.logError("$.Plato.Locale could not be initialized as the $.Plato.Options.url property is empty!");
                return;
            }

            // append a forward slash if needed
            var baseUrl = opts.url;
            if (baseUrl.substring(baseUrl.length - 1, baseUrl.length) !== "/") {
                baseUrl += "/";
            }

            var url = baseUrl + "js/app/locale/app." + opts.locale + ".js";
            context.logger.logInfo("Loading locale: " + url);
            this._load(url);

        },
        get: function (key) {
            var strings = win.$.Plato.Strings;
            if (typeof strings !== 'undefined' &&
                typeof strings[key] !== 'undefined') {
                return strings[key];
            }
            return key;
        },
        _load: function (url) {

            var context = win.$.Plato.Context,
                head = document.getElementsByTagName('head'),
                buster = parseInt(Math.random() * 1000) + new Date().getTime();

            var script = document.createElement('script');
            script.setAttribute('src', url + '?v=' + buster);

            if (head) {
                head[0].appendChild(script);
                context.logger.logInfo("Adding locale (" + url + ") to the head element.");
            }

            script.onLoad = function () {
                context.logger.logInfo("Added locale (" + url + ") to the head element.");
            }
        }
    };

    // access to options & core functionality
    win.$.Plato.Context = {
        options: function() {
            // Extend the options if external options exist
            if (typeof window.PlatoOptions !== "undefined") {
                $.extend(true, $.Plato.Options, window.PlatoOptions);
            }
            return $.Plato.Options;
        },
        logger: $.Plato.Logger,
        localizer: $.Plato.Locale
    };
    
    /* Plato UI */
    win.$.Plato.UI = {
        context: win.$.Plato.Context,
        init: function() {

            // init
            this.initToolTips();
            this.initDropDowns();
            this.initAvatar();

        },
        logInfo: function(message) {
            this.context.logger.logInfo(message);
        },
        logError: function(message) {
            this.context.logger.logError(message);
        },
        initToolTips: function() {

            this.logInfo("initToolTips()");

            // Enable bootstratp tooltips
            if (this.context.options().BSToolTipEnabled) {
                $(this.context.options().BSToolTipSelector).tooltip({ trigger: "hover" });
                $(this.context.options().BSToolTipAlternativeSelector).tooltip({ trigger: "hover" });
                this.logInfo("Bootstratp tooltipss initialized.");
            }
        },
        initDropDowns: function() {

            this.logInfo("initDropDowns()");

            // Enable
            // Enable nested dropdown support
            $("ul.dropdown-menu [data-toggle='dropdown']").on("click",
                function(event) {

                    // Avoid following the href location when clicking
                    event.preventDefault();
                    // Avoid having the menu to close when clicking
                    event.stopPropagation();
                    // If a menu is already open we close it
                    $("ul.dropdown-menu [data-toggle='dropdown']").parent().removeClass('show');
                    // opening the one you clicked on
                    $(this).parent().addClass('show');

                });


        },
        initAvatar: function() {

            this.logInfo("initAvatar()");

            // Avatar upload selector with preview
            $(this.context.options().AvatarUploadSelector).change(function() {
                function readUrl(input) {
                    if (input.files && input.files[0]) {
                        var reader = new FileReader();
                        reader.onload = function(e) {
                            var previewSelector = $(input).attr("data-upload-preview-selector");
                            var $preview = $(previewSelector).find("span");
                            $preview.css('background-image', ' url(' + e.target.result + ')');
                            $preview.hide()
                                .fadeIn(650);
                        };
                        reader.readAsDataURL(input.files[0]);
                    }
                }

                readUrl(this);
            });

        },
        notify: function(options, settings) {
            // Bootstrap notify
            $.notify(options, settings);
        }
    };

    /* Plato Http */
    win.$.Plato.Http = function(config) {

        var context = win.$.Plato.Context;
        if (!context) {
            throw new Error("$.Plato.Http requires a valid Plato.Context object");
        }

        var ui = win.$.Plato.UI;
        if (!ui) {
            throw new Error("$.Plato.Http requires a valid $.Plato.UI object");
        }

        var opts = context.options();
        if (!opts) {
            throw new Error("$.Plato.Http requires a valid $.Plato.Options object");
        }
        
        var baseUrl = opts.url,
            virtualUrl = config.url;
   
        // Remove forward slash suffix from base url
        if (baseUrl.substring(baseUrl.length - 1, baseUrl.length) === "/") {
            baseUrl = baseUrl.substring(opts.url.length - 1);
        }

        // prefix a forward slash if non is provided for our end point
        if (virtualUrl.substring(0, 1) !== "/") {
            virtualUrl = "/" + virtualUrl;
        }

        // update to absolute URL
        config.url = baseUrl + virtualUrl;

        // add basic authentication headers
        var apiKey = opts.apiKey;
        if (apiKey) {
            context.logger.logInfo("ApiKey: " + apiKey);
            config.beforeSend = function(xhr) {
                xhr.setRequestHeader("Authorization", "Basic " + apiKey);
            };
        } else {
            context.logger.logInfo("No api key was supplied");
        }

        // set content type & API version
        config.headers = {
            'Content-Type': 'application/json',
            'X-Api-Version': '1',
            "X-Csrf-Token": opts.getCsrfCookieToken()
        };

        var http = (function() {

            var getHtmlErrorMessage = function(err) {
                    var s = '<h6>' + context.localizer.get("An error occurred!") + '</h6>';
                    s += context.localizer.get("Information is provided below...") + "<br/><br/>";
                    s += '<textarea style="min-height: 130px;" class="form-control">' + err + '</textarea>';
                    return s;
                },
                notify = function(message) {

                    // Bootstrap notify
                    ui.notify({
                            // options
                            message: message
                        },
                        {
                            // settings
                            mouse_over: "pause",
                            type: 'danger',
                            allow_dismiss: true
                        });

                },
                onError = function(config, xhr, ajaxOptions, thrownError) {

                    // Error details
                    var err = "$.Plato.Http - " +
                        thrownError +
                        "\n" +
                        JSON.stringify(xhr, null, "     ");

                    // Log
                    context.logger.logError(err);

                    // Notify
                    notify(getHtmlErrorMessage(err));

                },
                onAlways = function(xhr, textStatus) {

                    // Display a visual indicator if the request fails due to authentication
                    if (xhr.statusCode === 401) {
                        notify("<h6>" + context.localizer.get("Could not authenticate your request!") + "</h6>");
                    }

                    // Log
                    context.logger.logInfo("$.Plato.Http - Completed: " +
                        JSON.stringify(xhr, null, "     "));

                };

            return {
                promise: function(config) {
                    return $.ajax(config)
                        .fail(function(xhr, ajaxOptions, thrownError) {
                            if (onError) {
                                onError(config, xhr, ajaxOptions, thrownError);
                            }
                        })
                        .always(function(xhr, textStatus) {
                            if (onAlways) {
                                onAlways(xhr, textStatus);
                            }
                        });
                }
            };
        }());

        context.logger.logInfo("$.Plato.Http - Starting Request: " + JSON.stringify(config, null, "     "));

        return http.promise(config);

    };

    /* Plato Storage */
    win.$.Plato.Storage = {
        setCookie: function (key, value, expireDays, toJson, path) {

            toJson = toJson || false;
            if (toJson) { value = encodeURIComponent(JSON.stringify(value)); }
            if (!path) { path = "/"; }

            var data = "";
            if (expireDays) {
                var dateExpires = new Date();
                dateExpires.setTime(dateExpires.getTime() + 1000 * 60 * 60 * 24 * expireDays);
                data = '; expires=' + dateExpires.toGMTString();
            }
            document.cookie = key + "=" + value + data + ";path=" + path;
            return this;
        },
        getCookie: function (key) {
            var ckName = key + "=";
            var ckPos = document.cookie.indexOf(ckName);
            if (ckPos !== -1) {
                var ckStart = ckPos + ckName.length;
                var ckEnd = document.cookie.indexOf(";", ckStart);
                if (ckEnd === -1) { ckEnd = document.cookie.length; }
                return unescape(document.cookie.substring(ckStart, ckEnd));
            }
            return null;
        },
        updateCookie: function (key, value, update, delimiter) {
            // if update is false the value will be removed from the cookie
            // if update is true the value will be appended to the cookie
            var cookie = this.getCookie(key),
                temp = new Array();
            delimiter = delimiter || ",";
            if (cookie) {
                // read existing excluding value into temp array
                var values = cookie.split(delimiter);
                for (var i in values) {
                    if (values.hasOwnProperty(i)) {
                        if (values[i] !== value && values[i] !== "") {
                            temp[temp.length] = values[i];
                        }
                    }
                }
            }
            // should we append the current value?
            if (update) { temp[temp.length] = value; }
            // update the cookie
            this.setCookie(key, temp.join(delimiter));
            return this;
        },
        clearCookie: function (key) {
            this.setCookie(key, null, -1);
            return this;
        }
    };
    
    /* Initialize */
    /* ---------------------------------------------*/
    
    $(doc).ready(function () {

        var context = win.$.Plato.Context;
        context.logger.logInfo("$.Plato.Options = " + JSON.stringify(context.options(), null, "     "));
        $.Plato.UI.init();
        $.Plato.Locale.init();

    });

}(window, document, jQuery));
