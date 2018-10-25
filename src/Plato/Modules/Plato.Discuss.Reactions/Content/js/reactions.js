
if (typeof jQuery === "undefined") {
    throw new Error("Plato requires jQuery");
}

if (typeof $.Plato.Context === "undefined") {
    throw new Error("$.Plato.Context Required");
}

/* follow buttons */
$(function (win, doc, $) {

    'use strict';
    
    var reactionMenu = function () {

        var dataKey = "reactionMenu",
            dataIdKey = dataKey + "Id";

        var defaults = {
            toggleToolTipEvent: "mouseenter"
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

                // Toggle tooltip on hover state
                var event = $caller.data(dataKey).toggleToolTipEvent;
                if (event) {
                    $caller.find("a").each(function() {
                        $(this).unbind(event).bind(event,
                            function () {
                                var label = $(this).attr("data-reaction-label");
                                $caller.find(".text-muted").text(label);
                            });
                    });
                }

            },
            unbind: function ($caller) {

                // Unbind tooltip on hover state
                var event = $caller.data(dataKey).toggleToolTipEvent;
                if (event) {
                    $caller.find("a").each(function () {
                        $(this).unbind(event);
                    });
                }

            },
            handleEvent: function ($caller) {

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
            subscribe: function ($caller) {

                var params = {
                    Id: 0,
                    UserId: 0,
                    EntityId: this.getEntityId($caller)
                };

                win.$.Plato.Http({
                    url: "api/follows/entity/post",
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
                    throw new Error("An entity id is required in order to follow an entity");
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
        reactionMenu: reactionMenu.init
    });

    $(doc).ready(function () {

        $('[data-provide="react-menu"]')
            .reactionMenu();

    });

}(window, document, jQuery));
