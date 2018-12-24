
if (typeof jQuery === "undefined") {
    throw new Error("Notifications requires jQuery");
}

if (typeof $.Plato.Context === "undefined") {
    throw new Error("Notifications require $.Plato.Context");
}

if (typeof $.Plato.Http === "undefined") {
    throw new Error("Notifications require $.Plato.Http");
}

$(function (win, doc, $) {

    'use strict';
    
    // notifications
    var notifications = function () {
        
        var dataKey = "notifications",
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

                this.bind($caller);

            },
            bind: function ($caller) {
                
                // Invoke suggester
                $caller.pagedList({
                    page: 1,
                    pageSize: 5,
                    enablePaging: false,
                    loaderTemplate: null,
                    itemSelection: {
                        enable: false,
                        index: 0,
                        css: "active"
                    },
                    noResultsIcon: null,
                    noResultsText: "No notifications at this time",
                    config: {
                        method: "GET",
                        url: 'api/notifications/user/get?page={page}&size={pageSize}',
                        data: {
                            sort: "CreatedDate",
                            order: "Desc"
                        }
                    },
                    itemCss: "dropdown-item p-2",
                    itemTemplate: '<a id="notification{id}" class="{itemCss}" href="{url}"><span class="list-left"><span class="avatar avatar-sm mr-2" data-toggle="tooltip" title="{from.displayName}"><span style="background-image: url({from.avatar.url});"></span></span></span><span class="list-body"><span class="float-right text-muted notification-date">{date.text}</span><span class="float-right notification-dismiss" data-notification-id="{id}"><i class="fal fa-times"></i></span><h6>{title}</h6>{message}</span></a>',
                    parseItemTemplate: function (html, result) {

                        if (result.id) {
                            html = html.replace(/\{id}/g, result.id);
                        } else {
                            html = html.replace(/\{id}/g, "0");
                        }

                        // to

                        if (result.to.id) {
                            html = html.replace(/\{to.Id}/g, result.to.id);
                        } else {
                            html = html.replace(/\{to.Id}/g, "");
                        }

                        if (result.to.displayName) {
                            html = html.replace(/\{to.displayName}/g, result.to.displayName);
                        } else {
                            html = html.replace(/\{to.displayName}/g, "");
                        }

                        if (result.to.userName) {
                            html = html.replace(/\{to.userName}/g, result.to.userName);
                        } else {
                            html = html.replace(/\{to.userName}/g, "");
                        }

                        if (result.to.url) {
                            html = html.replace(/\{to.url}/g, result.to.url);
                        } else {
                            html = html.replace(/\{to.url}/g, "");
                        }

                        if (result.to.avatar.url) {
                            html = html.replace(/\{to.avatar.url}/g, result.to.avatar.url);
                        } else {
                            html = html.replace(/\{to.avatar.url}/g, "");
                        }
                        
                        // from

                        if (result.from.id) {
                            html = html.replace(/\{from.id}/g, result.from.id);
                        } else {
                            html = html.replace(/\{from.id}/g, "");
                        }

                        if (result.from.displayName) {
                            html = html.replace(/\{from.displayName}/g, result.from.displayName);
                        } else {
                            html = html.replace(/\{from.displayName}/g, "");
                        }

                        if (result.from.userName) {
                            html = html.replace(/\{from.userName}/g, result.from.userName);
                        } else {
                            html = html.replace(/\{from.userName}/g, "");
                        }

                        if (result.from.url) {
                            html = html.replace(/\{from.url}/g, result.from.url);
                        } else {
                            html = html.replace(/\{from.url}/g, "");
                        }

                        if (result.from.avatar.url) {
                            html = html.replace(/\{from.avatar.url}/g, result.from.avatar.url);
                        } else {
                            html = html.replace(/\{from.avatar.url}/g, "");
                        }

                        // notification

                        if (result.title) {
                            html = html.replace(/\{title}/g, result.title);
                        } else {
                            html = html.replace(/\{title}/g, "");
                        }

                        if (result.message) {
                            html = html.replace(/\{message}/g, result.message);
                        } else {
                            html = html.replace(/\{message}/g, "");
                        }

                        if (result.date.text) {
                            html = html.replace(/\{date.text}/g, result.date.text);
                        } else {
                            html = html.replace(/\{date.text}/g, "");
                        }

                        if (result.date.value) {
                            html = html.replace(/\{date.value}/g, result.date.value);
                        } else {
                            html = html.replace(/\{date.value}/g, "");
                        }


                        if (result.url) {
                            html = html.replace(/\{url}/g, result.url);
                        } else {
                            html = html.replace(/\{url}/g, "#");
                        }
                        return html;

                    },
                    onLoaded: function ($caller, results) {

                        var $badge = $('[data-provide="notifications-badge"]'),
                            $dismiss = $(".notification-dismiss");

                        // Update notification badge
                        $badge.notificationsBadge({
                            count: results ? results.total : 0
                        });

                        // Activate tooltips
                        $caller.find('[data-toggle="tooltip"]')
                            .tooltip({ trigger: "hover" });

                        // Bind dismiss click event
                        $dismiss.each(function () {
                            $(this).click(function (e) {

                                e.preventDefault();
                                e.stopPropagation();

                                var id = $(this).attr("data-notification-id"),
                                    $target = $caller.find("#notification" + id);
                                
                                $badge.notificationsBadge("pulseIn");
                                $target.slideUp("fast", function () {

                                    if (id === null) { return; }
                                    if (id <= 0) { return; }

                                    win.$.Plato.Http({
                                        method: "DELETE",
                                        url: 'api/notifications/user/delete?id=' + id
                                    }).done(function(response) {
                                        if (response.statusCode === 200) {
                                            methods.update($caller);
                                        }
                                    });

                                });

                            });
                        });
                        
                    }
                });

            },
            update: function($caller) {
                $caller.pagedList("bind");
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
                    // $(selector).notifications
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
                    // $().notifications
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

    // notificationsBadge
    var notificationsBadge = function () {

        var dataKey = "notificationsBadge",
            dataIdKey = dataKey + "Id";

        var defaults = {
            count: 0
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

                this.bind($caller);

            },
            bind: function ($caller) {

                // Set count
                var count = parseInt($caller.data(dataKey).count);
                $caller.text(count);

                // Show & pulse
                if (count > 0) {
                    this.show($caller);
                    this.pulseOut($caller);
                } else {
                    this.hide($caller);
                }
        

            },
            show: function ($caller) {

                if ($caller.hasClass("hidden")) {
                    $caller.removeClass("hidden");
                }

                if (!$caller.hasClass("show")) {
                    $caller.addClass("show");;
                }

            },
            hide: function ($caller) {
                
                if ($caller.hasClass("show")) {
                    $caller.removeClass("show");;
                }

                if (!$caller.hasClass("hidden")) {
                    $caller.addClass("hidden");
                }

            },
            pulseIn: function ($caller) {
                
                if ($caller.hasClass("anim-2x")) {
                    $caller.removeClass("anim-2x");
                }

                if ($caller.hasClass("anim-pulse-in")) {
                    $caller.removeClass("anim-pulse-in");
                }

                if ($caller.hasClass("anim-pulse-out")) {
                    $caller.removeClass("anim-pulse-out");
                }

                $caller
                    .addClass("anim-2x")
                    .addClass("anim-pulse-in");
                    

            },
            pulseOut: function ($caller) {

                if ($caller.hasClass("anim-2x")) {
                    $caller.removeClass("anim-2x");
                }

                if ($caller.hasClass("anim-pulse-out")) {
                    $caller.removeClass("anim-pulse-out");
                }

                if ($caller.hasClass("anim-pulse-in")) {
                    $caller.removeClass("anim-pulse-in");
                }

                $caller
                    .addClass("anim-2x")
                    .addClass("anim-pulse-out");
       

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
                    // $(selector).notificationsBadge
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
                    // $().notificationsBadge
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
        notifications: notifications.init,
        notificationsBadge: notificationsBadge.init
    });

    $(doc).ready(function () {

        $('[data-provide="notifications"]').notifications();
        
    });

}(window, document, jQuery));
