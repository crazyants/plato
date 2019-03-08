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

/* star button */
$(function (win, doc, $) {

    'use strict';

    // Plato Global Object
    var app = win.$.Plato;
    
    // Provides state changes functionality for the star button
    var starToggle = function () {

        var dataKey = "starToggle",
            dataIdKey = dataKey + "Id";

        var defaults = {
            onCss: null,
            offCss: null
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

            },
            enable: function($caller) {

                var onCss = $caller.data("onCss") || $caller.data(dataKey).onCss,
                    offCss = $caller.data("offCss") || $caller.data(dataKey).offCss;

                if (offCss) {
                    $caller.removeClass(offCss);
                }

                if (onCss) {
                    $caller.addClass(onCss);
                }

                // Update action
                $caller.attr("data-action", "unsubscribe");

                // Update icon
                $caller.find("i").removeClass("fal").addClass("fa");

                // Hide tooltip
                if ($caller.tooltip) {
                    $caller.tooltip("hide");
                }

                // Update tooltip
                if ($caller.attr("data-unsubscribe-tooltip")) {
                    if ($caller.tooltip) {
                        $caller.attr("data-original-title", $caller.attr("data-unsubscribe-tooltip"));
                    } else {
                        $caller.attr("title", $caller.attr("data-unsubscribe-tooltip"));
                    }
                }

                // Update text
                $caller.find("span").text($caller.attr("data-unsubscribe-text"));
              
            },
            disable: function($caller) {

                var onCss = $caller.data("onCss") || $caller.data(dataKey).onCss,
                    offCss = $caller.data("offCss") || $caller.data(dataKey).offCss;

                if (onCss) {
                    $caller.removeClass(onCss);
                }

                if (offCss) {
                    $caller.addClass(offCss);
                }

                // Update action
                $caller.attr("data-action", "subscribe");

                // Update icon
                $caller.find("i").removeClass("fa").addClass("fal");

                // Hide tooltip
                if ($caller.tooltip) {
                    $caller.tooltip("hide");
                }

                // Update tooltip
                if ($caller.attr("data-subscribe-tooltip")) {
                    if ($caller.tooltip) {
                        $caller.attr("data-original-title", $caller.attr("data-subscribe-tooltip"));
                    } else {
                        $caller.attr("title", $caller.attr("data-subscribe-tooltip"));
                    }
                }

                // Update text
                $caller.find("span").text($caller.attr("data-subscribe-text"));

            }
        };

        return {
            init: function() {

                var options = {};
                var methodName = null;
                for (var i = 0; i < arguments.length; ++i) {
                    var a = arguments[i];
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

                if (this.length > 0) {
                    // $(selector).starToggle
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
                    // $().starToggle
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

    // Provides the ability to star an entity
    var starButton = function () {

        var dataKey = "starButton",
            dataIdKey = dataKey + "Id";

        var defaults = {
            event: "click"
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

                methods.bind($caller);

            },
            bind: function($caller) {

                var event = $caller.data(dataKey).event;
                if (event) {
                    $caller.on(event,
                        function(e) {
                            e.preventDefault();
                            methods.handleEvent($caller);
                        });
                }

            },
            unbind: function($caller) {
                var event = $caller.data(dataKey).event;
                if (event) {
                    $caller.unbind(event);
                }
            },
            handleEvent: function($caller) {

                var action = this.getAction($caller);
                switch (action) {
                case "subscribe":
                    this.subscribe($caller);
                    break;

                case "unsubscribe":
                    this.unsubscribe($caller);
                    break;
                }

            },
            subscribe: function($caller) {

                var params = {
                    Id: 0,
                    Name: this.getStarType($caller),
                    CreatedUserId: 0,
                    ThingId: this.getThingId($caller)
                };

                app.http({
                    url: "api/stars/star/post",
                    method: "POST",
                    data: JSON.stringify(params)
                }).done(function(data) {

                    if (data.statusCode === 200) {

                        // Enable button
                        $caller.starToggle("enable");

                        //// Bootstrap notify
                        //app.ui.notify({
                        //        // options
                        //    message: win.$.Plato.Locale.get("Star Added Successfully")
                        //    },
                        //    {
                        //        width: "auto",
                        //        type: 'success',
                        //        delay: 2000
                        //    });

                    }

                });

            },
            unsubscribe: function($caller) {

                var params = {
                    Name: this.getStarType($caller),
                    ThingId: this.getThingId($caller)
                };

                app.http({
                    url: "api/stars/star/delete",
                    method: "DELETE",
                    data: JSON.stringify(params)
                }).done(function(data) {
                    if (data.statusCode === 200) {
                        // Disable button
                        $caller.starToggle("disable");
                    }
                });

            },
            getAction: function($caller) {
                var action = "subscribe";
                if ($caller.attr("data-action")) {
                    action = $caller.attr("data-action");
                }
                return action;
            },
            getStarType: function($caller) {
                var followType = "";
                if ($caller.attr("data-star-type")) {
                    followType = $caller.attr("data-star-type");
                }
                if (followType === "") {
                    throw new Error("A star type  is required in order to star an item.");
                }
                return followType;
            },
            getThingId: function($caller) {
                var thingId = 0;
                if ($caller.attr("data-thing-id")) {
                    thingId = parseInt($caller.attr("data-thing-id"));
                }
                if (thingId < 0) {
                    throw new Error("A thing id is required in order to follow an item.");
                }
                return thingId;
            }

        };

        return {
            init: function() {

                var options = {};
                var methodName = null;
                for (var i = 0; i < arguments.length; ++i) {
                    var a = arguments[i];
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

                if (this.length > 0) {
                    // $(selector).starButton
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
                    // $().starButton
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

    // Init plugins
    $.fn.extend({
        starButton: starButton.init,
        starToggle: starToggle.init,
    });
    
    app.ready(function () {

        $('[data-provide="star-button"]')
            .starButton();

    });

}(window, document, jQuery));
