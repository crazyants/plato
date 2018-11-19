
if (typeof jQuery === "undefined") {
    throw new Error("Plato requires jQuery");
}

if (typeof $.Plato.Context === "undefined") {
    throw new Error("$.Plato.Context Required");
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
                    itemSelection: {
                        enable: false,
                        index: 0,
                        css: "active"
                    },
                    valueField: null,
                    config: {
                        method: "GET",
                        url: 'api/notifications/users/get?page={page}&size={pageSize}',
                        data: {
                            sort: "CreatedDate",
                            order: "Desc"
                        }
                    },
                    itemCss: "dropdown-item",
                    itemTemplate: '<a class="{itemCss}" href="{url}"><span class="list-left"><span class="avatar avatar-sm mt-2 mr-2"><span style="background-image: url(/users/photo/{from.id});"></span></span></span><span class="list-body"><span class="float-right"><i class="fa fa-times dismiss"></i></span><h6>{title}</h6>{message}</span></a>',
                    parseItemTemplate: function (html, result) {

                        if (result.id) {
                            html = html.replace(/\{id}/g, result.id);
                        } else {
                            html = html.replace(/\{id}/g, "0");
                        }

                        // user

                        if (result.user.id) {
                            html = html.replace(/\{user.Id}/g, result.user.id);
                        } else {
                            html = html.replace(/\{user.Id}/g, "");
                        }

                        if (result.user.displayName) {
                            html = html.replace(/\{user.displayName}/g, result.user.displayName);
                        } else {
                            html = html.replace(/\{user.displayName}/g, "");
                        }

                        if (result.user.userName) {
                            html = html.replace(/\{user.userName}/g, result.user.userName);
                        } else {
                            html = html.replace(/\{user.userName}/g, "");
                        }

                        if (result.user.url) {
                            html = html.replace(/\{user.url}/g, result.user.url);
                        } else {
                            html = html.replace(/\{user.url}/g, "");
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

                        if (result.url) {
                            html = html.replace(/\{url}/g, result.url);
                        } else {
                            html = html.replace(/\{url}/g, "#");
                        }
                        return html;

                    },
                    onPagerClick: function ($self, page, e) {
                        e.preventDefault();
                        e.stopPropagation();
                        $caller.suggester({
                            page: page
                        },
                            "show");
                    },
                    onItemClick: function ($self, result, e) {

                        // Prevent default event
                        e.preventDefault();

                        // Focus input, hide suggest menu & insert result
                        $caller
                            .focus()
                            .suggester("hide")
                            .suggester({
                                insertData: {
                                    index: searchResult.startIndex,
                                    value: result.userName
                                }
                            },
                                "insert");

                    }
                });

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
    

    $.fn.extend({
        notifications: notifications.init
    });

    $(doc).ready(function () {

        $('[data-provide="notifications"]').notifications();
        
    });

}(window, document, jQuery));
