// <reference path="/wwwroot/js/app.js" />

if (typeof window.jQuery === "undefined") {
    throw new Error("Plato requires jQuery");
}

if (typeof window.$.Plato === "undefined") {
    throw new Error("$.Plato Required");
}

if (typeof window.$.fn.platoUI === "undefined") {
    throw new Error("$.Plato UI Required");
}

$(function (win, doc, $) {

    "use strict";
    
    // --------

    var app = win.$.Plato;

    // --------

    var questions = {
        init: function () {
            this.bind();
        },
        bind: function () {
            
            // -------------
            // Answer
            // -------------

            $('[data-provide="postAnswer"]').bind("click", function (e) {

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
        questions.init();
    });

}(window, document, jQuery));
