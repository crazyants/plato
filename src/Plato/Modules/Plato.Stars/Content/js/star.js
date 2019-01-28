
if (typeof jQuery === "undefined") {
    throw new Error("Plato requires jQuery");
}

if (typeof $.Plato.Context === "undefined") {
    throw new Error("$.Plato.Context Required");
}

/* follow buttons */
$(function (win, doc, $) {

    'use strict';

    // Provides state changes functionality for the star button
    var starToggler = function () {

        var dataKey = "starToggler",
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

                $caller
                    .removeClass(offCss)
                    .addClass(onCss)
                    .attr("data-action", "unsubscribe");

                $caller.find("i")
                    .removeClass("fa-bell")
                    .addClass("fa-bell-slash");

                $caller.find("span").text($caller.attr("data-unsubscribe-text"));

            },
            disable: function($caller) {

                var onCss = $caller.data("onCss") || $caller.data(dataKey).onCss,
                    offCss = $caller.data("offCss") || $caller.data(dataKey).offCss;

                $caller
                    .removeClass(onCss)
                    .addClass(offCss)
                    .attr("data-action", "subscribe");

                $caller.find("i")
                    .removeClass("fa-bell-slash")
                    .addClass("fa-bell");

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
                    // $(selector).starToggler
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
                    // $().starToggler
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
                    Name: this.getFollowType($caller),
                    CreatedUserId: 0,
                    ThingId: this.getThingId($caller)
                };

                win.$.Plato.Http({
                    url: "api/stars/star/post",
                    method: "POST",
                    data: JSON.stringify(params)
                }).done(function(data) {

                    if (data.statusCode === 200) {
                        $caller.followToggler("enable");
                    }

                });

            },
            unsubscribe: function($caller) {

                var params = {
                    Name: this.getFollowType($caller),
                    ThingId: this.getThingId($caller)
                };

                win.$.Plato.Http({
                    url: "api/stars/star/delete",
                    method: "DELETE",
                    data: JSON.stringify(params)
                }).done(function(data) {
                    if (data.statusCode === 200) {
                        $caller.followToggler("disable");
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
            getFollowType: function($caller) {
                var followType = "";
                if ($caller.attr("data-follow-type")) {
                    followType = $caller.attr("data-follow-type");
                }
                if (followType === "") {
                    throw new Error("A follow type  is required in order to follow an item.");
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

    // Mimics the star button but uses a hidden checkbox to persist state
    var starCheckbox = function () {

        var dataKey = "starCheckbox",
            dataIdKey = dataKey + "Id";

        var defaults = {
            event: "change"
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
                            if ($(this).is(":checked")) {
                                $caller.parent().followToggler("enable");
                            } else {
                                $caller.parent().followToggler("disable");
                            }

                        });
                }

            },
            unbind: function($caller) {
                var event = $caller.data(dataKey).event;
                if (event) {
                    $caller.unbind(event);
                }
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
                    // $(selector).starCheckbox
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
                    // $().starCheckbox
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
        starButton: starButton.init,
        starToggler: starToggler.init,
        starheckbox: starCheckbox.init
    });
    
    $(doc).ready(function () {

        $('[data-provide="star-button"]')
            .starButton();

        $('[data-provide="star-checkbox"]')
            .starCheckbox();
     
    });

}(window, document, jQuery));
