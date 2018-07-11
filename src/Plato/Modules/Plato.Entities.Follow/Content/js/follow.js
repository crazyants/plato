
if (typeof jQuery === "undefined") {
    throw new Error("Plato requires jQuery");
}

if (typeof $.Plato.Context === "undefined") {
    throw new Error("$.Plato.Context Required");
}

/* follow button */
$(function (win, doc, $) {

    'use strict';
    
    var entityFollowButton = function () {

        var dataKey = "entityFollowButton",
            dataIdKey = dataKey + "Id";

        var defaults = {
            event: "click"
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
            getUniqueId: function ($caller) {

                return parseInt($caller.attr("data-markdown-id")) || 0;
            },
            bind: function ($caller) {
                
                var event = $caller.data(dataKey).event;
                if (event) {
                    $caller.on(event,
                        function (e) {
                            e.preventDefault();
                            methods.handleEvent($caller);
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
            subscribe: function($caller) {

                var params = {
                    Id: 0,
                    UserId: 0,
                    EntityId: this.getEntityId($caller)
                };

                win.$.Plato.Http({
                    url: "api/follows/entity/post",
                    method: "POST",
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(params)
                }).done(function(data) {
                    
                    if (data.statusCode === 200) {

                        $caller
                            .removeClass("btn-secondary")
                            .addClass("btn-danger")
                            .attr("data-action", "unsubscribe");

                        $caller.find("i")
                            .removeClass("fa-bell")
                            .addClass("fa-bell-slash");
                    }
                 
                });

            }, 
            unsubscribe: function($caller) {
                
                var params = {
                    EntityId: this.getEntityId($caller)
                };

                win.$.Plato.Http({
                    url: "api/follows/entity/delete",
                    method: "DELETE",
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(params)
                }).done(function (data) {

                    if (data.statusCode === 200) {
                        $caller
                            .addClass("btn-secondary")
                            .removeClass("btn-danger")
                            .attr("data-action", "subscribe");
                        
                        $caller.find("i")
                            .removeClass("fa-bell-slash")
                            .addClass("fa-bell");

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

        }

    }();

    $.fn.extend({
        entityFollowButton: entityFollowButton.init
    });
    
    $(doc).ready(function () {

        $('[data-provide="follow-button"]')
            .entityFollowButton();
     
    });

}(window, document, jQuery));
