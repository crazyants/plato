
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
                entityReplyId: 0,
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

                // Set post parameters
                $caller.data(dataKey).params.entityId = this.getEntityId($caller);
                $caller.data(dataKey).params.entityReplyId = this.getEntityReplyId($caller);

                // Bind events
                methods.bind($caller);

            },
            bind: function ($caller) {

                $caller.find("a").each(function () {

                    var event = "";

                    // Bind tooltip handler
                    if ($(this).attr('data-toggle')) {
                        // Bind bootstrap tooltips
                        $(this).tooltip({ trigger: "hover" });
                    } else {
                        // Bind custom handler
                        event = $caller.data(dataKey).tooltipEvent;
                        if (event) {
                            $(this).unbind(event).bind(event,
                                function(e) {
                                    var desc = $(this).attr("data-reaction-description");
                                    if (desc) {
                                        $caller.find(".text-muted").text(desc);
                                    }
                                });
                        }
                    }

                    // Bind click handler
                    event = $caller.data(dataKey).event;
                    if (event) {
                        $(this).unbind(event).bind(event,
                            function (e) {
                                e.preventDefault();
                                if ($(this).attr("data-toggle")) {
                                    $(this).tooltip("hide");
                                }
                                $caller.data(dataKey).params.reactionName = $(this).attr("data-reaction-name");
                                methods.update($caller);
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
            update: function ($caller) {

                var params = $caller.data(dataKey).params;
                params.entityId = this.getEntityId($caller);
                params.entityReplyId = this.getEntityReplyId($caller);

                win.$.Plato.Http({
                    url: "api/reactions/react/post",
                    method: "POST",
                    data: JSON.stringify(params)
                }).done(function (data) {
                    // Created or deleted
                    if (data.statusCode === 201 || data.statusCode === 202) {
                        if (data.result) {
                            methods.build($caller, data.result);
                        }
                    }
                });
            },
            build: function($caller, results) {

                var $target = this.getTarget($caller);
                if ($target) {
                    $target.empty();
                    for (var i = 0; i < results.length; i++) {

                        var result = results[i],
                            $li = $("<li>",
                                {
                                    "class": "bg-light border-right border-secondary"
                                }),
                            $a = $("<a>",
                                {
                                    "href": "#",
                                    "class": "list-group-item list-group-item-action py-2 px-3",
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

                    methods.bind($target);
                }

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
            },
            getEntityReplyId: function ($caller) {
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
