﻿// <reference path="/wwwroot/js/app.js" />

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

    var app = win.$.Plato,
        featureId = "Plato.Site";

    // --------

    var site = {
        init: function () {
            app.logger.logInfo(featureId + " initializing");
            this.bind();
            app.logger.logInfo(featureId + " initialized");
        },
        bind: function () {
            // Placeholder
        }
    };
    
    // --------

    // app ready
    app.ready(function () {
        site.init();
    });

    // infinite scroll load
    $().infiniteScroll(function ($ele) { }, "ready");

}(window, document, jQuery));
