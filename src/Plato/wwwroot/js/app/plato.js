// <reference path="~/js/vendors/jquery.js" />
// <reference path="~/js/vendors/bootstrap.js" />

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
        // UI tooltips
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
        prevLogDate: null,
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

    /* Client side localization */
    win.$.Plato.Locale = {
        lang: "en-US"
    }

    /* Plato UI */
    win.$.Plato.UI = {
        context: win.$.Plato.Context,
        init: function () {
            
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
            if (this.context.options().BSToolTipEnabled) {
                $(this.context.options().BSToolTipSelector).tooltip();
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
            $(this.context.options().AvatarUploadSelector).change(function () {
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
        if (!context) {
            throw new Error("Plato.Http requires a valid Plato.Context object");
        }

        var opts = context.options();
        if (!opts) {
            throw new Error("Plato.Http requires a valid Plato.Options object");
        }

        // update URL to include absolute URL
        config.url = opts.url + config.url;

        // add basic authentication headers
        var apiKey = opts.apiKey;
        if (apiKey) {
            context.logger.logInfo("ApiKey: " + apiKey);
            config.beforeSend = function(xhr) {
                xhr.setRequestHeader("Authorization", "Basic " + apiKey);
            }
        } else {
            context.logger.logInfo("No api key was supplied");
        }

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
        
        context.logger.logInfo("$.Plato.Http - Starting Request: " + JSON.stringify(config, null, "     "));

        return http.promise(config);

    }

    /* jQuery Plugins */
    /* ---------------------------------------------*/

    /* scrollTo */
    var scrollTo = function () {

        var dataKey = "scrollTo",
            dataIdKey = dataKey + "Id";

        var defaults = {
            event: "click",
            onBeforeComplete: function() {},
            onComplete: function() {}
        };

        var methods = {
            init: function ($caller, methodName) {
                if (methodName) {
                    if (this[methodName]) {
                        this[methodName].apply(this, [$caller]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return;
                }

                methods.bind($caller);

            },
            bind: function ($caller) {

                var event = $caller.data(dataKey).event;
                if (event) {
                    $caller.on(event,
                        function (e) {
                            e.preventDefault();
                            methods.scrollTo($caller);


                        });
                }

            },
            scrollTo: function ($caller) {

                jQuery.extend(jQuery.easing,
                    {
                        def: 'easeOutQuad',
                        easeInOutExpo: function (x, t, b, c, d) {
                            if (t === 0) return b;
                            if (t === d) return b + c;
                            if ((t /= d / 2) < 1) return c / 2 * Math.pow(2, 10 * (t - 1)) + b;
                            return c / 2 * (-Math.pow(2, -10 * --t) + 2) + b;
                        }
                    });

                var href = $caller.attr("href");
                if (href) {
                    var $target = $(href);
                    if ($target.length > 0) {
                        $('html, body').stop().animate({
                            scrollTop: ($target.offset().top - 10)
                        },
                            250,
                            'easeInOutExpo', function() {
                                $caller.data(dataKey).onComplete($caller, $target);
                            });
                        $caller.data(dataKey).onBeforeComplete($caller, $target);
                    }
                }

            }
        }

        return {
            init: function () {

                var options = {};
                var methodName = null;
                for (var i = 0; i < arguments.length; ++i) {
                    var a = arguments[i];
                    switch (a.constructor) {
                        case Object:
                            $.extend(options, a);
                            break;
                        case String:
                            methodName = a;
                            break;
                        case Boolean:
                            break;
                        case Number:
                            break;
                        case Function:
                            break;
                    }
                }

                if (this.length > 0) {
                    // $(selector).markdownEditor
                    return this.each(function () {
                        if (!$(this).data(dataIdKey)) {
                            var id = dataKey + parseInt(Math.random() * 100) + new Date().getTime();
                            $(this).data(dataIdKey, id);
                            $(this).data(dataKey, $.extend({}, defaults, options));
                        } else {
                            $(this).data(dataKey, $.extend({}, $(this).data(dataKey), options));
                        }
                        methods.init($(this), methodName);
                    });
                } else {
                    // $().markdownEditor 
                    if (methodName) {
                        if (methods[methodName]) {
                            var $caller = $("body");
                            $caller.data(dataKey, $.extend({}, defaults, options));
                            methods[methodName].apply(this, [$caller]);
                        } else {
                            alert(methodName + " is not a valid method!");
                        }
                    }
                }

            }

        }

    }();

    /* Register jQuery Plugins */
    $.fn.extend({
        scrollTo: scrollTo.init
    });
    
    /* Initialize */
    /* ---------------------------------------------*/
    
    $(doc).ready(function () {

        var context = win.$.Plato.Context;
        context.logger.logInfo("$.Plato.Options = " + JSON.stringify(context.options(), null, "     "));
        $.Plato.UI.init();

        /* plug-ins */
        $('[data-provide="scroll"]').scrollTo();

    });

}(window, document, jQuery));
