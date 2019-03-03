// <reference path="/wwwroot/js/app.js" />

$(function (win, doc, $) {

    "use strict";

    if (typeof $.Plato === "undefined") {
        throw new Error("$.Plato.Articles requires $.Plato");
    }

    // --------

    var app = $.Plato;

    // --------

    $(doc).ready(function () {
        $.Plato.Articles.init();
    });

    // --------

    $.Plato.Articles = {
        init: function () {
            app.logger.logInfo("$.Plato.Articles initializing");
            this.bind();
            app.logger.logInfo("$.Plato.Articles initialized");
        },
        bind: function () {
            
            // -------------
            // Comment
            // -------------

            $('[data-provide="postComment"]').bind("click", function (e) {

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

}(window, document, jQuery));
