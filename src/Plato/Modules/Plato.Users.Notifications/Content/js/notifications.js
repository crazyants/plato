// <reference path="/wwwroot/js/app.js" />

if (typeof window.jQuery === "undefined") {
    throw new Error("Plato requires jQuery");
}

if (typeof window.$.Plato === "undefined") {
    throw new Error("$.Plato Required");
}

if (typeof window.$.fn.platoUI === "undefined") {
    throw new Error("$.Plato UI Required");
}

/* notifications */
$(function (win, doc, $) {

    'use strict';

    // --------

    var app = win.$.Plato;

    // --------

    // notifications
    var notifications = function () {
        
        var dataKey = "notifications",
            dataIdKey = dataKey + "Id";

        var defaults = {};

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

                this.bind($caller);

            },
            bind: function($caller) {

                var deleteText = app.T("Delete Notification");

                // Invoke paged list
                $caller.pagedList({
                    page: 1,
                    pageSize: 5,
                    enablePaging: false,
                    itemSelection: {
                        enable: false,
                        index: 0,
                        css: "active"
                    },
                    noResultsIcon: null,
                    noResultsText: app.T("No notifications at this time"),
                    config: {
                        method: "GET",
                        url: 'api/notifications/user/get?page={page}&size={pageSize}',
                        data: {
                            sort: "CreatedDate",
                            order: "Desc"
                        }
                    },
                    itemCss: "dropdown-item p-2",
                    itemTemplate:
                        '<a id="notification{id}" class="{itemCss}" href="{url}"><span class="list-left"><span class="avatar mt-1 mr-2" data-toggle="tooltip" title="{from.displayName}"><span style="background-image: url({from.avatar.url});"></span></span></span><span class="list-body"><span class="float-right text-muted notification-date">{date.text}</span><span class="float-right p-2 notification-dismiss" title="{Delete}" data-notification-id="{id}"><i class="fal fa-times"></i></span><h6 style="max-width: 300px; white-space:nowrap; overflow:hidden; text-overflow: ellipsis;">{title}</h6>{message}</span></a>',
                    parseItemTemplate: function(html, result) {

                        html = html.replace(/\{Delete}/g, deleteText);
                       
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
                    onLoaded: function($caller, results) {

                        var $dismiss = $(".notification-dismiss");

                        // Activate tooltips
                        $caller.find('[data-toggle="tooltip"]')
                            .tooltip({ trigger: "hover" });

                        // Bind dismiss / delete click event
                        $dismiss.each(function() {
                            $(this).click(function(e) {

                                e.preventDefault();
                                e.stopPropagation();

                                $(this).tooltip("hide");

                                var id = $(this).attr("data-notification-id"),
                                    $target = $caller.find("#notification" + id);

                                $target.slideUp("fast",
                                    function() {

                                        if (id === null) {
                                            return;
                                        }
                                        if (id <= 0) {
                                            return;
                                        }

                                        app.http({
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
            update: function ($caller) {
                $caller.pagedList("bind");
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
                    // $(selector).notifications
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
        };

    }();

    // notificationsBadge
    var notificationsBadge = function () {

        var dataKey = "notificationsBadge",
            dataIdKey = dataKey + "Id";

        var defaults = {
            onShow: null,
            onHide: null,
            count: 0,
            prevCount: -1,
            onPollComplete: null,
            pollInterval: 120 // the seconds to wait between polls for new notifications. Set to 0 to disable polling.
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

                this.bind($caller);

            },
            bind: function($caller) {
                
                // Poll for unread count immediately
                methods.poll($caller);

                // Periodically check for new notifications
                var interval = $caller.data(dataKey).pollInterval;
                if (interval > 0) {
                    win.setInterval(function () {
                            methods.poll($caller);
                        },
                        interval * 1000);
                }     

            },
            poll: function($caller) {
              
                if (app === null) {
                    throw new Error("$.Plato Required");
                }

                if (app.logger) {
                    app.logger.logInfo("Polling user notifications");
                }
                
                // Get the number of unread notifications and display badge if needed
                app.http({
                    url: "api/notifications/user/unread",
                    method: "GET"
                }).done(function (data) {
                    if (data.statusCode === 200) {
                        if (app.logger) {
                            app.logger.logInfo("Successfully polled user notifications");
                        }
                        // Raise poll complete event
                        if ($caller.data(dataKey).onPollComplete) {
                            $caller.data(dataKey).onPollComplete($caller, data.result);
                        }
                    }
                });

            },
            show: function($caller) {

                $caller.text($caller.data(dataKey).count.toString());
                
                if ($caller.hasClass("hidden")) {
                    $caller.removeClass("hidden");
                }

                if (!$caller.hasClass("show")) {
                    $caller.addClass("show");
                }
                
                if ($caller.data(dataKey).onShow) {
                    $caller.data(dataKey).onShow($caller);
                }

            },
            hide: function($caller) {

                if ($caller.hasClass("show")) {
                    $caller.removeClass("show");
                }

                if (!$caller.hasClass("hidden")) {
                    $caller.addClass("hidden");
                }

                if ($caller.data(dataKey).onHide) {
                    $caller.data(dataKey).onHide($caller);
                }

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
                    // $(selector).notificationsBadge
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

        };

    }();

    // notificationsDropdown
    var notificationsDropdown = function () {

        var dataKey = "notificationsDropdown",
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

                // Initialize list of notifications
                $caller.find('[data-provide="notifications"]').notifications();

                $caller.find(".dropdown-toggle").click(function (e) {
                    e.preventDefault();
                    // mark all notifications as read
                    methods.markRead($caller);
                });

            },
            update: function ($caller) {
                // Update notification list, for example if returned unread count changes
                $caller.find('[data-provide="notifications"]').notifications("update");
            },
            markRead: function($caller) {

                // If the badge is already hidden no need to mark notifications as read
                var $badge = $caller.find('[data-provide="notificationsBadge"]');
                if ($badge.hasClass("hidden")) {
                    return;
                }

                app.http({
                    url: "api/notifications/user/markread",
                    method: "POSt"
                }).done(function(data) {
                    if (data.statusCode === 200) {
                        if (data.result) {
                            // All notifications successfully marked read
                            // Hide our notification badge
                            $badge.notificationsBadge("hide");
                        }
                    }
                });

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
                    // $(selector).notificationsDropdown
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
                    // $().notificationsDropdown
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
        notifications: notifications.init,
        notificationsBadge: notificationsBadge.init,
        notificationsDropdown: notificationsDropdown.init
    });

    app.ready(function () {

        var $dropdown = $('[data-provide="notificationsDropdown"]'),
            $badge = $('[data-provide="notificationsBadge"]');

        // Init dropdown
        $dropdown.notificationsDropdown();

        // Init unread count badge
        $badge.notificationsBadge({
            onShow: function () { },
            onHide: function ($caller) {
                // If the badge is hidden the user may have expanded the dropdown which marks
                // all notifications as read and hides the badge. In this case ensure we reset
                // the previousCount back to 0 so if new message arrive the polling updates the list
                $caller.data("notificationsBadge").prevCount = 0;
            },
            onPollComplete: function($caller, count) {

                // Ensure we only update if we have unread notifications 
                // Ensure the unread count has changed since the previous poll
                if ($caller.data("notificationsBadge").prevCount !== count && count > 0) {
                    
                    // Update count & ensure badge is visible
                    $caller.notificationsBadge({ count: count }, "show");
                    
                    // Update notifications list within dropdown
                    $dropdown.notificationsDropdown("update");
                    
                }

                // Track changes between polls
                $caller.data("notificationsBadge").prevCount = count;

            }
        });

    });

}(window, document, jQuery));
