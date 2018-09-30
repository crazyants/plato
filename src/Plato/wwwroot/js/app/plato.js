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
    
    // access to options & core functionality
    win.$.Plato.Context = {
        options: function() {
            // Extend the options if external options exist
            if (typeof window.PlatoOptions !== "undefined") {
                $.extend(true, $.Plato.Options, window.PlatoOptions);
            }
            return $.Plato.Options;
        },
        logger: $.Plato.Logger
    };

    /* Client side localization */
    win.$.Plato.Locale = {
        lang: "en-US"
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

        }
    };

    /* Plato Http */
    win.$.Plato.Http = function(config) {

        var context = win.$.Plato.Context;
        if (!context) {
            throw new Error("Plato.Http requires a valid Plato.Context object");
        }

        var opts = context.options();
        if (!opts) {
            throw new Error("Plato.Http requires a valid Plato.Options object");
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

            var onError = function(config, xhr, ajaxOptions, thrownError) {
                    if (context) {
                        context.logger.logInfo("$.Plato.Http - Error: " +
                            JSON.stringify(xhr, null, "     ") +
                            thrownError);
                    }
                },
                onAlways = function(xhr, textStatus) {
                    if (context) {
                        context.logger.logInfo("$.Plato.Http - Completed: " +
                            JSON.stringify(xhr, null, "     "));
                    }
                };

            return {
                onError: function(onError) {
                    onError = onError;
                },
                onAlways: function(onAlways) {
                    onAlways = onAlways;
                },
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
        
    });

}(window, document, jQuery));
