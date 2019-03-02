// <reference path="/wwwroot/js/app.js" />

$(function (win, doc, $) {

    "use strict";

    if (typeof $.Plato.Context === "undefined") {
        throw new Error("$.Plato.Articles requires $.Plato.Context");
    }

    if (typeof $.Plato.Options === "undefined") {
        throw new Error("$.Plato.Articles requires $.Plato.Options");
    }

    if (typeof $.Plato.Logger === "undefined") {
        throw new Error("$.Plato.Articles requires $.Plato.Logger");
    }

    var context = {
        options: $.Plato.Options,
        logger: $.Plato.Logger
    };

    $(doc).ready(function () {
        
        // Comment

        $('[data-provide="postComment"]').bind("click", function(e) {

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
/* Plato Articles */
/* --------------------*/

$.Plato.Articles = {
    context: null,
    init: function(context) {

        this.context = context;
        context.logger.logInfo("$.Plato.Articles initializing");

    }
};
