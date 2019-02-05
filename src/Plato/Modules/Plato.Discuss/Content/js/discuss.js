// <reference path="/wwwroot/js/app.js" />

// doc ready

$(function (win, doc, $) {

    "use strict";

    if (typeof $.Plato.Options === "undefined") {
        throw new Error("$.Plato.Discuss requires $.Plato.Options");
    }

    if (typeof $.Plato.Logger === "undefined") {
        throw new Error("$.Plato.Discuss requires $.Plato.Logger");
    }
    
    var context = {
        options: $.Plato.Options,
        logger: $.Plato.Logger
    }
    
    $.fn.extend({
        scrollTo: scrollTo.init
    });

    $(doc).ready(function () {
        
        $('[data-provide="postQuote"]').scrollTo({
            onComplete: function ($caller, $target) {
                var $textarea = $target.find("textarea");

                $textarea.focus();
            }
        });

        //$('[data-provide="postReply"]').scrollTo({
        //    onComplete: function($caller, $target) {
        //        $target.find("textarea").focus();
        //    }
        //});
        
        $('[data-provide="postReply"]').bind("click", function(e) {

            e.preventDefault();

            /* resizeable */
            $('[data-provide="resizeable"]').resizeable("show");

        });

    });
    

}(window, document, jQuery));

/* --------------------*/
/* Plato Discuss */
/* --------------------*/

$.Plato.Discuss = {
    context: null,
    init: function(context) {

        this.context = context;

        context.logger.logInfo("$.Plato.Discuss initializing");

    }
};
