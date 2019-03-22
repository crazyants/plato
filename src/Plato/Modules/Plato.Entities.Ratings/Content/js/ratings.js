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

                console.log(JSON.stringify(params));

                app.http({
                    url: "api/ratings/rate/post",
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

                console.log(JSON.stringify(results));

                var results = $caller.data(dataKey).data;
                var $target = this.getTarget($caller);
                if ($target) {

                    // empty target
                    $target.empty();

                    if (results.length > 0) {

                        for (var i = 0; i < results.length; i++) {

                            var result = results[i],
                                $li = $("<li>",
                                    {
                                        "class": "bg-light border-right border-secondary"
                                    }),
                                $a = $("<a>",
                                    {
                                        "href": "#",
                                        "class": "list-group-item list-group-item-action py-2 px-3 border-0",
                                        "data-toggle": "tooltip",
                                        "title": result.toolTip,
                                        "data-reaction-name": result.name
                                    }),
                                $emoji = $("<span>",
                                    {
                                        "class": "d-inline-block mx-1"
                                    }).html(result.emoji),
                                $total = $("<span>",
                                    {
                                        "class": "d-inline-block mx-1"
                                    }).html(result.total);

                            $a.append($emoji).append($total);
                            $li.append($a);
                            $target.append($li);
                        }

                        // ensure target is visible
                        if ($target.hasClass("hidden")) {
                            $target.removeClass("hidden");
                        }

                    } else {

                        // ensure target is hidden
                        if (!$target.hasClass("hidden")) {
                            $target.addClass("hidden");
                        }

                    }

                    methods.bind($target);
                }

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
            getTarget: function($caller) {
                var target = $caller.attr("data-reactions-target");
                if (target) {
                    var $target = $(target);
                    if ($target.length > 0) {
                        return $target;
                    }
                }
                return null;
            }
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
