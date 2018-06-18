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
};

/* Simple logging */
$.Plato.Logger = {
    info: "Info",
    warning: "Warning",
    error: "Error",
    log: function(level, message) {
        if (!$.Plato.Options.debug) {
            return;
        }
        console.log(level + ": " + message);
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
    var o = $.Plato.Options,
        logger = $.Plato.Logger;
    
    logger.log(logger.info, "$.Plato.Options = " + JSON.stringify(o, null, "     "));
    
    $(doc).ready(function() {
        
        logger.log(logger.info, "$(doc).ready() event fited");
      
        // Enable bootstratp tooltips
        if (o.BSToolTipEnabled) {
            $(o.BSToolTipSelector).tooltip();
            logger.log(logger.info, "Bootstratp tooltips initialized.");
        }
        

        $("ul.dropdown-menu [data-toggle='dropdown']").on("click", function (event) {

          
            logger.log(logger.info, $(this).text() + " menu item clicked");
          
            // Avoid following the href location when clicking
            event.preventDefault();
            // Avoid having the menu to close when clicking
            event.stopPropagation();
            // If a menu is already open we close it
            $("ul.dropdown-menu [data-toggle='dropdown']").parent().removeClass('show');
            // opening the one you clicked on
            $(this).parent().addClass('show');
            
        });
        

    });


}(window, document, jQuery));
