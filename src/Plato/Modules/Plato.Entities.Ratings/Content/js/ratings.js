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

    /* base rating */
    var rating = function () {

        var dataKey = "rating",
            dataIdKey = dataKey + "Id";

        var defaults = {
            event: "click",
            params: {
                rating: 0,
                entityId: 0,
                entityReplyId: 0
            },
            onUpdated: function ($el, results) {}
        };

        var methods = {
            _loading: false,
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

                // Ensure we are not already posting
                if (methods._loading) {
                    return;
                }

                // Indicate post
                methods._loading = true;

                // Build post params
                var params = $caller.data(dataKey).params;
                params.entityId = this.getEntityId($caller);
                params.entityReplyId = this.getEntityReplyId($caller);
                
                // Post
                app.http({
                    url: methods.getUrl($caller),
                    method: "POST",
                    data: JSON.stringify(params)
                }).done(function (data) {
                    // Created or deleted response
                    if (data.statusCode === 201 || data.statusCode === 202) {
                        // No longer loading
                        methods._loading = false;
                        // Call onUpdated delegate
                        if ($caller.data(dataKey).onUpdated) {
                            $caller.data(dataKey).onUpdated($caller, data.result);
                        }
                    }
                });
            },
            getUrl: function ($caller) {
                // Allow API end point to be customized
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
                    throw new Error("A entity id is required in order to rate an entity");
                }
                return entityId;
            },
            getEntityReplyId: function($caller) {
                var replyId = 0;
                if ($caller.attr("data-entity-reply-id")) {
                    replyId = parseInt($caller.attr("data-entity-reply-id"));
                }
                return replyId;
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

    /* vote toggle */
    var voteToggle = function (options) {

        var dataKey = "voteToggle",
            dataIdKey = dataKey + "Id";

        var defaults = {};

        var methods = {
            init: function ($caller, methodName) {

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
            bind: function ($caller) {

                // Initialize rating plug-in overriding onUpdated event
                $caller.rating($.extend({
                        onUpdated: function($el, result) {
                            var $header = $el.find("h6");
                            if ($header.length > 0) {

                                // Update total votes tooltip
                                var total = result.totalRatings;
                                if (total !== null && typeof total !== "undefined") {

                                    // Strings
                                    var title = app.T("Based on {0}"),
                                        vote = app.T("vote"),
                                        votes = app.T("votes"),
                                        text = total === 1
                                            ? total + " " + vote
                                            : total + " " + votes;

                                    title = title.replace("{0}", text);

                                    // update title
                                    if ($header.attr("title")) {
                                        $header.attr("title", title);
                                    }

                                    // accomodate for twitter bootstrap
                                    if ($header.attr("data-original-title")) {
                                        $header.attr("data-original-title", title);
                                    }

                                }

                                // Update summed rating text
                                $header.empty().text(result.summedRating);
                            }
                        }
                    },
                    defaults,
                    options));

            },
            unbind: function ($caller) {
                $caller.rating("unbind");
            }
        };

        return {
            init: function () {

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
                    // $(selector).voteToggle
                    return this.each(function () {
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
                    // $().voteToggle
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
        rating: rating.init,
        voteToggle: voteToggle.init
    });

    // Initial load
    app.ready(function () {

        $('[data-provide="rating"]')
            .rating();

        $('[data-provide="vote-toggle"]')
            .voteToggle();

        

    });

    // infinite scroll load
    $().infiniteScroll(function ($ele) {

        $ele.find('[data-provide="rating"]')
            .rating();
        
        $ele.find('[data-provide="vote-toggle"]')
            .voteToggle();

    }, "ready");

}(window, document, jQuery));
