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

        // Quote

        $('[data-provide="postQuote"]').bind("click",
            function(e) {

                e.preventDefault();

                // Get element containing quote
                var value = "",
                    selector = $(this).attr("data-quote-selector"),
                    $quote = $(selector);
                if ($quote.length > 0) {
                    value = ">" + $quote.html()
                        .replace(/\n\r/g, "\n")
                        .replace(/\n/g, "\n > ");

                }

                /* resizeable */
                $('[data-provide="resizeable"]').resizeable("toggleVisibility", {
                    onShow: function ($caller) {
                        var $textArea = $caller.find(".md-textarea");
                        if ($textArea.length > 0) {
                            $textArea.val(value);
                            $textArea.focus();
                        }
                    }
                });


            });

        // Reply

        $('[data-provide="postReply"]').bind("click", function(e) {

            e.preventDefault();

            /* resizeable */
            $('[data-provide="resizeable"]').resizeable("toggleVisibility", {
                    onShow: function($caller) {
                        var $textArea = $caller.find(".md-textarea");
                        if ($textArea.length > 0) {
                            $textArea.focus();
                        }
                    }
                });

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
