
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
                            methods.click($caller);
                        });
                }

            },
            click: function ($caller) {

                var action = "subscribe";
                if ($caller.attr("data-action")) {
                    action = $caller.attr("data-action");
                }
                var entityId = 0;
                if ($caller.attr("data-entity-id")) {
                    entityId = parseInt($caller.attr("data-entity-id"));
                }

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

                win.$.Plato.Http({
                    url: "/api/entities/follow/",
                    method: "POST",
                    async: false,
                    data: params
                }).done(function(data) {

                    if (data.id > 0) {
                        $caller
                            .removeClass("btn-secondary")
                            .addClass("btn-danger");
                    }
                });

            }, 
            unsubscribe: function($caller) {

                win.$.Plato.Http({
                    url: "/api/entities/follow",
                    method: "DELETE",
                    async: false,
                    data: params
                }).done(function(data) {
                    fn(data);
                });

            },
            getHtml: function (params, fn) {

                win.$.Plato.Http({
                    url: "/api/entities/follow/",
                    method: "GET",
                    async: false,
                    data: params
                }).done(function (data) {
                    fn(data);
                });

            }
            //show: function ($caller) {

            //    var delay = $caller.data("notify").delay || $caller.data("notifyDelay");

            //    var $target = notify.getElement($caller);
            //    $target.addClass("i-notify-visible");

            //    if (delay > 0) {
            //        win.setTimeout(function () {
            //            notify.hide($caller);
            //        },
            //            delay);
            //    }

            //},
            //hide: function ($caller) {
            //    $(".i-notify").removeClass("i-notify-visible");
            //},
            //getElement: function ($caller) {

            //    var text = $caller.data("notifyText") || $caller.data("notify").text,
            //        css = $caller.data("notifyCss") || $caller.data("notify").css,
            //        closeButton = $caller.data("notifyCloseButton") || $caller.data("notify").closeButton,
            //        iconCss = $caller.data("notifyIconCss") || $caller.data("notify").iconCss;

            //    // create alert html
            //    var s = "<div class=\"" + css + "\"><div>";
            //    s += (iconCss ? "<i class=\"" + iconCss + "\"></i>" : "");
            //    s += text;

            //    if (closeButton === true) {
            //        s += "<a class=\"i-notify-close\" href=\"#\"><i class=\"fa fa-times\"></i></a>";
            //    }
            //    s += "</div></div>";

            //    // create and add to dom
            //    var $alert = $(s);
            //    $("body").append($alert);

            //    $("body")
            //        .find(".i-notify-close")
            //        .unbind("click")
            //        .bind("click",
            //            function (e) {
            //                e.preventDefault();
            //                $($caller).inotify("hide");
            //            });

            //    return $alert;
            //}
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
