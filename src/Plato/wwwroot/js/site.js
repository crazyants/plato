
if (typeof jQuery === "undefined") {
    throw new Error("Plato requires jQuery");
}

/* Plato
 *
 * @type Object
 * @description $.AdminLTE is the main object for the template's app.
 *              It's used for implementing functions and options related
 *              to the template. Keeping everything wrapped in an object
 *              prevents conflict with other plugins and is a better
 *              way to organize our code.
 */
$.Plato = {};

/* ------------------
 * - Options -
 * ------------------
 * The next block of code defines various global
 * application settings. This can be extended via the 
 * PlatoOptions object on the window object
 */
$.Plato.options = {
    debug: true,
    // bootstrap tooltips
    BSToolTipEnabled: true,
    BSToolTipSelector: "[data-toggle='tooltip']",
    
    MagnificSelector: "[data-toggle='dialog']",

};

/* ------------------
 * - Implementation -
 * ------------------
 * The next block of code implements AdminLTE's
 * functions and plugins as specified by the
 * options above.
 */
$(function(win, doc, $) {

    "use strict";

    //Extend options if external options exist
    if (typeof window.PlatoOptions !== "undefined") {
        $.extend(true, $.Plato.options, window.PlatoOptions);
    }

    // access to options
    var o = $.Plato.options;


    $(doc).ready(function() {

        if (o.debug) {
            console.log("jQuery $(doc).ready() event fited");
        }
        
        //$("ul.dropdown-menu [data-toggle='dropdown']").on("click", function (event) {

        //    if (o.debug) {
        //        console.log(($(this).text()) + " menu item clicked");
        //    }
           
        //    // Avoid following the href location when clicking
        //    event.preventDefault();
        //    // Avoid having the menu to close when clicking
        //    event.stopPropagation();
        //    // If a menu is already open we close it
        //    $("ul.dropdown-menu [data-toggle='dropdown']").parent().removeClass('show');
        //    // opening the one you clicked on
        //    $(this).parent().addClass('show');
            
        //});



    });


}(window, document, jQuery));
