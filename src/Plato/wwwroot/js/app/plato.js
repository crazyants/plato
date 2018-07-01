// <reference path="~/js/vendors/jquery.js" />
// <reference path="~/js/vendors/bootstrap.js" />

if (typeof jQuery === "undefined") {
    throw new Error("Plato requires jQuery");
}

$(function (win, doc, $) {

    "use strict";
    
    win.$.Plato = {};

    /* Default options */
    win.$.Plato.Options = {
        debug: true,
        // bootstrap tooltips
        BSToolTipEnabled: true,
        BSToolTipSelector: "[data-toggle='tooltip']",
        MagnificSelector: "[data-toggle='dialog']",
        AvatarUploadSelector: "[data-upload='avatar']",
    };

    /* Simple logging */
    win.$.Plato.Logger = {
        info: "Info",
        warning: "Warning",
        error: "Error",
        logInfo: function (message) {
            this.log(this.info, message);
        },
        logWarning: function (message) {
            this.log(this.warning, message);
        },
        logError: function (message) {
            this.log(this.error, message);
        },
        log: function (level, message) {
            if (!$.Plato.Options.debug) {
                return;
            }
            var ticks = (new Date().getTime() * 10000);
            console.log(level + ": " + message + " - " + ticks);
        }
    }
    
    // access to options & core functionality
    win.$.Plato.Context = {
        options: function () {
            // Extend the options if external options exist
            if (typeof window.PlatoOptions !== "undefined") {
                $.extend(true, $.Plato.Options, window.PlatoOptions);
            }
            return $.Plato.Options;
        },
        logger: $.Plato.Logger
    }

    /* Plato UI */
    win.$.Plato.UI = {
        context: null,
        init: function (context) {
            this.context = context;

            // init
            this.initToolTips();
            this.initDropDowns();
            this.initAvatar();

        },
        logInfo: function (message) {
            this.context.logger.logInfo(message);
        },
        logError: function (message) {
            this.context.logger.logError(message);
        },
        initToolTips: function () {

            this.logInfo("initToolTips()");

            // Enable bootstratp tooltips
            if (this.context.options.BSToolTipEnabled) {
                $(this.context.options.BSToolTipSelector).tooltip();
                this.logInfo("Bootstratp tooltipss initialized.");
            }

        },
        initDropDowns: function () {

            this.logInfo("initDropDowns()");

            // Enable
            // Enable nested dropdown support
            $("ul.dropdown-menu [data-toggle='dropdown']").on("click",
                function (event) {

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
        initAvatar: function () {

            this.logInfo("initAvatar()");

            // Avatar upload selector with preview
            $(this.context.options.AvatarUploadSelector).change(function () {
                function readUrl(input) {
                    if (input.files && input.files[0]) {
                        var reader = new FileReader();
                        reader.onload = function (e) {
                            var previewSelector = $(input).attr("data-upload-preview-selector");
                            $(previewSelector).css('background-image', ' url(' + e.target.result + ')');
                            $(previewSelector).hide();
                            $(previewSelector).fadeIn(650);
                        }
                        reader.readAsDataURL(input.files[0]);
                    }
                }

                readUrl(this);
            });

        }
    }

    /* Plato Http */
    win.$.Plato.Http = function(config) {

        var context = win.$.Plato.Context;
        if (context) {
            context.logger.logInfo("$.Plato.Http - Starting Request: " + JSON.stringify(config, null, "     "));
        }
        // update URL to include absolute URL
        //config.url = _opts.url + config.url;

        // add basic authentication headers
        //if (_opts.apiKey) {
        //    config.beforeSend = function (xhr) {
        //        xhr.setRequestHeader("Authorization", "Basic " + _opts.apiKey);
        //    }
        //}

        // set content type & API version
        config.headers = {
            'Content-Type': 'application/json',
            'X-Api-Version': '1'
        };
        
        var http = (function() {

            var onError = function(config, xhr, ajaxOptions, thrownError) {
                    if (context) {
                        context.logger.logInfo("$.Plato.Http - Error: " +
                            JSON.stringify(xhr, null, "     "));
                    }
                },
                onAlways = function (xhr, textStatus) {
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
                        .always(function (xhr, textStatus) {
                            if (onAlways) {
                                onAlways(xhr, textStatus);
                            }
                        });
                }
            };
        }());

        return http.promise(config);

    }


    /* Initialize */
    /* ---------------------------------------------*/
    
    $(doc).ready(function () {
        
        var context = $.Plato.Context;
        context.logger.logInfo("$.Plato.Options = " + JSON.stringify(context.options(), null, "     "));
        $.Plato.UI.init(context);

    });

}(window, document, jQuery));
