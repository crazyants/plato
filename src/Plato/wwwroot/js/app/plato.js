// <reference path="~/js/vendors/jquery.js" />
// <reference path="~/js/vendors/bootstrap.js" />

if (typeof jQuery === "undefined") {
    throw new Error("Plato requires jQuery");
}

/* Plato
 *
 * @type Object
 * @description $.Plato is the main object for the app.
 *              It's used for implementing functions and options related
 *              to the app. Keeping everything wrapped in an object
 *              prevents conflict with other plugins and is a better
 *              way to organize our code.
 */
$.Plato = {};

/* Default options */
$.Plato.Options = {
    debug: true,
    // bootstrap tooltips
    BSToolTipEnabled: true,
    BSToolTipSelector: "[data-toggle='tooltip']",
    MagnificSelector: "[data-toggle='dialog']",
    AvatarUploadSelector: "[data-upload='avatar']",
};

/* Simple logging */
$.Plato.Logger = {
    info: "Info",
    warning: "Warning",
    error: "Error",
    logInfo: function(message) {
        this.log(this.info, message);
    },
    logWarning: function (message) {
        this.log(this.warning, message);
    },
    logError: function (message) {
        this.log(this.error, message);
    },
    log: function(level, message) {
        if (!$.Plato.Options.debug) {
            return;
        }
        var ticks = (new Date().getTime() * 10000);
        console.log(level + ": " + message + " - " + ticks);
    }
}

/* --------------------*/

$(function(win, doc, $) {

    "use strict";

    // Extend the options if external options exist
    if (typeof window.PlatoOptions !== "undefined") {
        $.extend(true, $.Plato.Options, window.PlatoOptions);
    }

    // access to options
    var context = {
        options: $.Plato.Options,
        logger: $.Plato.Logger
    }

    context.logger.logInfo("$.Plato.Options = " + JSON.stringify(context.options, null, "     "));
    
    $(doc).ready(function () {

        context.logger.logInfo("$(doc).ready()");
        $.Plato.UI.init(context);

    });

}(window, document, jQuery));

/* --------------------*/
/* Plato UI */
/* --------------------*/

$.Plato.UI = {
    context: null,
    init: function (context) {
        this.context = context;

        // init
        this.initToolTips();
        this.initDropDowns();
        this.initAvatar();
        this.initMarkDown();

    },
    logInfo: function(message) {
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

    },
    initMarkDown: function () {

        this.logInfo("initMarkDown()");

        $(".md-textarea").markdown({
            autofocus: false,
            savable: false,
            resize: true,
            allowedUploadExtensions: [".gif", ".jpg", ".jpeg", ".png"],
            baseUrl: 'http://localhost:50439/',
            hiddenButtons: null,

            onPreview: function (e) {
                var output = '',
                    input = e.getContent();
                if (input) {
                    _kb.markdown.post({
                        input: input
                    },
                        function (data) {
                            output = data;
                        });
                }
                return output;
            },
            footer: function (e) {
                return '';
            },
            fullscreen: {
                enable: true
            },
            minRows: 4,
            maxRows: 15
        });

    }
}
