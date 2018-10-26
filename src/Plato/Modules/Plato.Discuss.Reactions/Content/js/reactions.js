
if (typeof jQuery === "undefined") {
    throw new Error("Plato requires jQuery");
}

if (typeof $.Plato.Context === "undefined") {
    throw new Error("$.Plato.Context Required");
}

/* reactions */
$(function (win, doc, $) {

    'use strict';
    
    var reactions = function () {

        var dataKey = "reactions",
            dataIdKey = dataKey + "Id";

        var defaults = {
            tooltipEvent: "mouseenter",
            event: "click",
            params: {
                id: 0,
                userId: 0,
                entityId: 0,
                reactionName: ""
            }
        };

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

                methods.bind($caller);

            },
            bind: function ($caller) {

                $caller.find("a").each(function() {
                    // Bind tooltip handler
                    var event = $caller.data(dataKey).tooltipEvent;
                    if (event) {
                        $(this).unbind(event).bind(event,
                            function(e) {
                                var desc = $(this).attr("data-reaction-description");
                                $caller.find(".text-muted").text(desc);
                            });
                    }
                    // Bind click handler
                    event = $caller.data(dataKey).event;
                    if (event) {
                        $(this).unbind(event).bind(event,
                            function (e) {
                                e.preventDefault();

                                $caller.data(dataKey).params.reactionName = $(this).attr("data-reaction-name");
                                methods.addOrRemove($caller);
                            });
                    }
                });

            },
            unbind: function ($caller) {

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
            addOrRemove: function ($caller) {

                var params = $caller.data(dataKey).params;
                params.entityId = this.getEntityId($caller);

                win.$.Plato.Http({
                    url: "api/reactions/entity/post",
                    method: "POST",
                    data: JSON.stringify(params)
                }).done(function (data) {
                    if (data.statusCode === 200) {
                        $caller.entityFollowToggler("enable");
                    }
                });

            },
            unsubscribe: function ($caller) {

                var params = {
                    EntityId: this.getEntityId($caller)
                };

                win.$.Plato.Http({
                    url: "api/follows/entity/delete",
                    method: "DELETE",
                    data: JSON.stringify(params)
                }).done(function (data) {
                    if (data.statusCode === 200) {
                        $caller.entityFollowToggler("disable");
                    }
                });

            },
            getAction: function ($caller) {
                var action = "subscribe";
                if ($caller.attr("data-action")) {
                    action = $caller.attr("data-action");
                }
                return action;
            },
            getEntityId: function ($caller) {
                var entityId = 0;
                if ($caller.attr("data-entity-id")) {
                    entityId = parseInt($caller.attr("data-entity-id"));
                }
                if (entityId === 0) {
                    throw new Error("An entity id is required in order to react to an entity");
                }
                return entityId;
            }

        }

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
                    // $(selector).reactionMenu
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
                    // $().reactionMenu
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

        }

    }();

 
    $.fn.extend({
        reactions: reactions.init
    });

    $(doc).ready(function () {

        $('[data-provide="reactions"]')
            .reactions();

    });

}(window, document, jQuery));
