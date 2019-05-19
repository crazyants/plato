
if (typeof jQuery === "undefined") {
    throw new Error("Plato requires jQuery");
}

if (typeof $.Plato === "undefined") {
    throw new Error("$.Plato Required");
}

/* follow button */
$(function (win, doc, $) {
    
    'use strict';

    // Plato Global Object
    var app = win.$.Plato;

    // Provides state changes functionality for the follow button
    var followToggle = function () {

        var dataKey = "followToggle",
            dataIdKey = dataKey + "Id";

        var defaults = {
            onCss: null,
            offCss: null,
            onIcon: "fa fa-bell",
            offIcon: "fal fa-bell"
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
                    offCss = $caller.data("offCss") || $caller.data(dataKey).offCss,
                    offIcon = $caller.data("offIcon") || $caller.data(dataKey).offIcon,
                    onIcon = $caller.data("onIcon") || $caller.data(dataKey).onIcon;

                // Update action
                $caller
                    .removeClass(offCss)
                    .addClass(onCss)
                    .attr("data-action", "unsubscribe");

                // Update icon
                $caller.find("i")
                    .removeClass(offIcon)
                    .addClass(onIcon);
                
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
                    offCss = $caller.data("offCss") || $caller.data(dataKey).offCss,
                    offIcon = $caller.data("offIcon") || $caller.data(dataKey).offIcon,
                    onIcon = $caller.data("onIcon") || $caller.data(dataKey).onIcon;

                if (onCss) {
                    $caller.removeClass(onCss);
                }

                if (offCss) {
                    $caller.addClass(offCss);
                }

                // Update action
                $caller.attr("data-action", "subscribe");

                // Update icon
                $caller.find("i")
                    .removeClass(onIcon)
                    .addClass(offIcon);
                
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
                    // $(selector).followToggle
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
                    // $().followToggle
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

    // Provides the ability to follow an entity
    var followButton = function () {

        var dataKey = "followButton",
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

                app.http({
                    url: "api/follows/follow/post",
                    method: "POST",
                    data: JSON.stringify(params)
                }).done(function(data) {
                    if (data.statusCode === 200) {

                        // Enable button
                        $caller.followToggle("enable");
                        
                    }

                });

            },
            unsubscribe: function($caller) {

                var params = {
                    Name: this.getFollowType($caller),
                    ThingId: this.getThingId($caller)
                };

                app.http({
                    url: "api/follows/follow/delete",
                    method: "DELETE",
                    data: JSON.stringify(params)
                }).done(function(data) {
                    if (data.statusCode === 200) {

                        // Disable button
                        $caller.followToggle("disable");
                        
                        //// Bootstrap notify
                        //win.$.Plato.UI.notify({
                        //        // options
                        //    message: win.$.Plato.Locale.get("Follow Deleted Successfully")
                        //    },
                        //    {
                        //        width: "auto",
                        //        type: 'success',
                        //        delay: 2000
                        //    });

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
                    // $(selector).markdownEditor
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
                    // $().markdownEditor 
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

    // Mimics the follow button but uses a hidden checkbox to persist state
    var followCheckbox = function () {

        var dataKey = "followCheckable",
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
                                $caller.parent().followToggle("enable");
                            } else {
                                $caller.parent().followToggle("disable");
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
                    // $(selector).markdownEditor
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
                    // $().markdownEditor 
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
        followButton: followButton.init,
        followToggle: followToggle.init,
        followCheckbox: followCheckbox.init
    });
    
    app.ready(function () {

        $('[data-provide="follow-button"]')
            .followButton();

        $('[data-provide="follow-checkbox"]')
            .followCheckbox();
     
    });

}(window, document, jQuery));
