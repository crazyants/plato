// <reference path="/wwwroot/js/app.js" />

// doc ready

$(function (win, doc, $) {

    "use strict";

    if (typeof $.Plato.Context === "undefined") {
        throw new Error("$.Plato.Discuss requires $.Plato.Context");
    }

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
                    $quote = $(selector),
                    context = $.Plato.Context;

                // Apply Localizer
                var text = "In response to";
                if (context.localizer) {
                    text = context.localizer.get(text);
                }
                
                if ($quote.length > 0) {
                    var displayName = $quote.attr("data-display-name"),
                        replyUrl = $quote.attr("data-reply-url");
                    value = "> " + $quote.html()
                        .replace(/\n\r/g, "\n")
                        .replace(/[\s]\n/g, "\n")
                        .replace(/\n/g, "\n> ");
                    if (displayName && replyUrl) {
                        value += "\n> ^^ " + text + " [" + displayName + "](" + replyUrl + ")";
                    }
                    value += "\n\n";
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
