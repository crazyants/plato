
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
    
    /* searchAutoComplete */
    var searchAutoComplete = function () {

        var dataKey = "searchAutoComplete",
            dataIdKey = dataKey + "Id";

        var defaults = {
            valueField: "keywords",
            config: {
                method: "GET",
                url: 'api/search/get?page={page}&size={pageSize}&keywords={keywords}',
                data: {
                    sort: "LastReplyDate",
                    order: "Desc"
                }
            },
            itemTemplate: '<a class="{itemCss}" href="{url}"><span class="avatar avatar-sm mr-2"><span style="background-image: url(/users/photo/{id});"></span></span>{title}<span class="float-right badge badge-primary">{rank}</span></a>',
            parseItemTemplate: function (html, result) {

                if (result.id) {
                    html = html.replace(/\{id}/g, result.id);
                } else {
                    html = html.replace(/\{id}/g, "0");
                }

                if (result.title) {
                    html = html.replace(/\{title}/g, result.title);
                } else {
                    html = html.replace(/\{title}/g, "(no title)");
                }
                if (result.userName) {
                    html = html.replace(/\{userName}/g, result.userName);
                } else {
                    html = html.replace(/\{userName}/g, "(no username)");
                }

                if (result.email) {
                    html = html.replace(/\{email}/g, result.email);
                } else {
                    html = html.replace(/\{email}/g, "");
                }
                if (result.agent_url) {
                    html = html.replace(/\{url}/g, result.url);
                } else {
                    html = html.replace(/\{url}/g, "#");
                }
                return html;

            },
            onKeyDown: function ($caller, e) {
                if (e.keyCode === 13) {
                    e.preventDefault();
                }
            },
            onItemClick: function ($caller, result, e) {
                e.preventDefault();
            }
        }

        var methods = {
            init: function ($caller, methodName, func) {

                if (func) {
                    return func(this);
                }
                if (methodName) {
                    if (this[methodName] !== null && typeof this[methodName] !== "undefined") {
                        this[methodName].apply(this, [$caller]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return null;
                }

                // init autoComplete
                $caller.autoComplete($caller.data(dataKey), methodName);

            },
            show: function ($caller) {
                $caller.autoComplete("show");
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
                    // $(selector).userAutoComplete()
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
                    // $().userAutoComplete()
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
        searchAutoComplete: searchAutoComplete.init
    });

    $(doc).ready(function () {

        $('[data-provide="searchAutoComplete"]').searchAutoComplete();
        
    });

}(window, document, jQuery));
