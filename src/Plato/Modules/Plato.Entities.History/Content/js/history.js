
if (typeof jQuery === "undefined") {
    throw new Error("History requires jQuery");
}

if (typeof $.Plato.Context === "undefined") {
    throw new Error("History require $.Plato.Context");
}

if (typeof $.Plato.Http === "undefined") {
    throw new Error("History require $.Plato.Http");
}

$(function (win, doc, $) {

    'use strict';
    
    // notifications
    var history = function () {
        
        var dataKey = "history",
            dataIdKey = dataKey + "Id";

        var defaults = {
            entityId: 0,
            entityReplyId: 0
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
                    noResultsText: "No history at this time",
                    config: {
                        method: "GET",
                        url: 'api/history/entity/get?page={page}&size={pageSize}',
                        data: {
                            sort: "CreatedDate",
                            order: "Desc"
                        }
                    },
                    itemTemplate: '<a id="historyid}" class="{itemCss}" href="{url}"><span class="list-left"><span class="avatar avatar-sm mr-2" data-toggle="tooltip" title="{from.displayName}"><span style="background-image: url(/users/photo/{from.id});"></span></span></span><span class="list-body">{text}</span></a>',
                    parseItemTemplate: function (html, result) {

                        if (result.id) {
                            html = html.replace(/\{id}/g, result.id);
                        } else {
                            html = html.replace(/\{id}/g, "0");
                        }

                        if (result.text) {
                            html = html.replace(/\{text}/g, result.text);
                        } else {
                            html = html.replace(/\{text}/g, "");
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
                        
                        // Activate tooltips
                        $caller.find('[data-toggle="tooltip"]')
                            .tooltip({ trigger: "hover" });
                        
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

    $.fn.extend({
        history: history.init
    });

    $(doc).ready(function () {

        $('[data-provide="history"]').history();
        
    });

}(window, document, jQuery));
