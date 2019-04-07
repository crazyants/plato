// <reference path="/wwwroot/js/app.js" />

if (typeof $().modal === 'undefined') {
    throw new Error("BootStrap 4.1.1 or above Required");
}

if (typeof window.$.Plato === "undefined") {
    throw new Error("$.Plato Required");
}

$(function (win, doc, $) {

    "use strict";

    // --------

    var app = win.$.Plato,
        featureId = "Plato.Docs";


    // --------

    var docs = {
        init: function () {
            app.logger.logInfo(featureId + " initializing");
            this.bind();
            app.logger.logInfo(featureId + " initialized");
        },
        bind: function () {
            
            // -------------
            // Quote
            // -------------

            $('[data-provide="postDocQuote"]').bind("click",
                function (e) {

                    e.preventDefault();

                    // Get element containing quote
                    var value = "",
                        selector = $(this).attr("data-quote-selector"),
                        $quote = $(selector);

                    // Apply locale
                    var text = app.T("In response to");

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
            
            // -------------
            // Reply
            // -------------

            $('[data-provide="postDocComment"]').bind("click", function (e) {

                e.preventDefault();

                /* resizeable */
                $('[data-provide="resizeable"]').resizeable("toggleVisibility", {
                    onShow: function ($caller) {
                        var $textArea = $caller.find(".md-textarea");
                        if ($textArea.length > 0) {
                            $textArea.focus();
                        }
                    }
                });

            });
            
        }
    };

    // --------

    app.ready(function () {
        docs.init();
    });
    
}(window, document, jQuery));
