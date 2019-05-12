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

            var className = "docs-table-contents",
                $toc = $("." + className);

            if ($toc.length > 0) {

                var top = $toc.offset().top,
                    padding = 16;

                $(win).scrollSpy({
                    onScrollEnd: function () {

                    },
                    onScroll: function (spy, e) {
                        console.log(spy.scrollTop);
                        console.log(top);
                        if (spy.scrollTop > top - padding) {
                            if (!$toc.hasClass(className + "-sticky")) {
                                $toc.addClass(className + "-sticky");
                            }
                        } else {
                            if ($toc.hasClass(className + "-sticky")) {
                                $toc.removeClass(className + "-sticky");
                            }
                        }
                    }
                });
            }

        }
    };

    // --------

    // app ready
    app.ready(function () {
        docs.init();
    });
    
    // infinite scroll load
    $().infiniteScroll(function ($ele) { }, "ready");
    
}(window, document, jQuery));
