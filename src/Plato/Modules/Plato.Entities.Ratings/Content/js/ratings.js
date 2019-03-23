// <reference path="/wwwroot/js/app.js" />

if (typeof window.jQuery === "undefined") {
    throw new Error("jQuery 3.3.1 or above Required");
}

if (typeof window.$.Plato === "undefined") {
    throw new Error("$.Plato Required");
}

if (typeof window.$.fn.platoUI === "undefined") {
    throw new Error("$.Plato UI Required");
}

$(function (win, doc, $) {

    'use strict';

    // --------

    var app = win.$.Plato;

    // --------

    var rating = function () {

        var dataKey = "rating",
            dataIdKey = dataKey + "Id";

        var defaults = {
            tooltipEvent: "mouseenter",
            event: "click",
            params: {
                rating: 0,
                entityId: 0,
                entityReplyId: 0
            },
            data: {}
        };

        var methods = {
            init: function($caller, methodName) {

                if (methodName) {
                    if (this[methodName]) {
                        this[methodName].apply(this, [$caller]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return;
                }

                // Bind events
                methods.bind($caller);

            },
            bind: function($caller) {

                $caller.find("a").each(function() {

                    // Bind click handler
                    var event = $caller.data(dataKey).event;
                    if (event) {
                        $(this).unbind(event).bind(event,
                            function(e) {
                                var rating = parseInt($(this).attr("data-rating"));
                                if (!isNaN(rating)) {
                                    e.preventDefault();
                                    $caller.data(dataKey).params.rating = rating;
                                    methods.post($caller);
                                }
                            });
                    }
                });

            },
            unbind: function($caller) {

                // Unbind events
                $caller.find("a").each(function() {
                    var event = $caller.data(dataKey).tooltipEvent;
                    if (event) {
                        $(this).unbind(event);
                    }
                    event = $caller.data(dataKey).event;
                    if (event) {
                        $(this).unbind(event);
                    }
                });

            },
            post: function($caller) {

                var params = $caller.data(dataKey).params;
                params.entityId = this.getEntityId($caller);
                params.entityReplyId = this.getEntityReplyId($caller);
                
                app.http({
                    url: methods.getUrl($caller),
                    method: "POST",
                    data: JSON.stringify(params)
                }).done(function(data) {
                    // Created or deleted response
                    if (data.statusCode === 201 || data.statusCode === 202) {
                        if (data.result) {
                            $caller.data(dataKey).data = data.result;
                            methods.refresh($caller);
                        }
                    }
                });
            },
            refresh: function($caller) {

            
                var result = $caller.data(dataKey).data;
                var $header = $caller.find("h6");

                console.log(JSON.stringify(result));
                
                if ($header.length > 0) {
                    $header
                        .empty()
                        .text(result.meanRating);
                }
            },
            getUrl: function($caller) {
                if ($caller.attr("data-rating-url") && $caller.attr("data-rating-url") !== "") {
                    return $caller.attr("data-rating-url");
                }
                return "api/ratings/rate/post";
            },
            getEntityId: function($caller) {
                var entityId = 0;
                if ($caller.attr("data-entity-id")) {
                    entityId = parseInt($caller.attr("data-entity-id"));
                }
                if (entityId === 0) {
                    throw new Error("A entity id is required in order to react to an entity");
                }
                return entityId;
            },
            getEntityReplyId: function($caller) {
                var replyId = 0;
                if ($caller.attr("data-entity-reply-id")) {
                    replyId = parseInt($caller.attr("data-entity-reply-id"));
                }
                return replyId;
            },
         
        };

        return {
            init: function() {

                var options = {};
                var methodName = null;
                for (var i = 0; i < arguments.length; ++i) {
                    var a = arguments[i];
                    if (a) {
                        switch (a.constructor) {
                        case Object:
                            $.extend(options, a);
                            break;
                        case String:
                            methodName = a;
                            break;
                        case Boolean:
                            break;
                        case Number:
                            break;
                        case Function:
                            break;
                        }
                    }
                }

                if (this.length > 0) {
                    // $(selector).rating
                    return this.each(function() {
                        if (!$(this).data(dataIdKey)) {
                            var id = dataKey + parseInt(Math.random() * 100) + new Date().getTime();
                            $(this).data(dataIdKey, id);
                            $(this).data(dataKey, $.extend({}, defaults, options));
                        } else {
                            $(this).data(dataKey, $.extend({}, $(this).data(dataKey), options));
                        }
                        methods.init($(this), methodName);
                    });
                } else {
                    // $().rating
                    if (methodName) {
                        if (methods[methodName]) {
                            var $caller = $("body");
                            $caller.data(dataKey, $.extend({}, defaults, options));
                            methods[methodName].apply(this, [$caller]);
                        } else {
                            alert(methodName + " is not a valid method!");
                        }
                    }
                }

            }

        };

    }();
    
    $.fn.extend({
        rating: rating.init
    });

    // Initial load
    app.ready(function () {

        $('[data-provide="rating"]')
            .rating();

    });

    // infinite scroll load
    $().infiniteScroll(function ($ele) {
        $ele.find('[data-provide="rating"]').rating();
    }, "ready");

}(window, document, jQuery));
