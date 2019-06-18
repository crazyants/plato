// <reference path="/wwwroot/js/app.js" />

if (typeof window.jQuery === "undefined") {
    throw new Error("jQuery 3.3.1 or above Required");
}

if (typeof window.$.Plato === "undefined") {
    throw new Error("$.Plato Required");
}

if (typeof window.$.fn.platoUI === "undefined") {
    throw new Error("$.Plato UI Required");
}

$(function (win, doc, $) {

    'use strict';

    // --------

    var app = win.$.Plato;

    // --------

    // mentions
    var mentions = function () {

        var dataKey = "mentions",
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
                $caller.suggester($.extend($caller.data(dataKey)),
                    {
                        // keyBinder options
                        keys: [
                            {
                                match: /(^|\s|\()(@([a-z0-9\-_/]*))$/i,
                                search: function($input, selection) {

                                    // The result of the search method is tested
                                    // against the match regular expiression within keyBinder
                                    // If a match is found the bind method is called 
                                    // otherwise the unbind method is called
                                    // This code executes on every key press so should be optimized

                                    var chars = $input.val().split(""),
                                        value = null,
                                        marker = "@",
                                        startIndex = -1,
                                        start = selection.start - 1,
                                        i;

                                    // Search backwards from caret for marker, until 
                                    // terminators & attempt to get marker position
                                    for (i = start; i >= 0; i--) {
                                        if (chars[i] === marker) {
                                            startIndex = i;
                                            break;
                                        } else {
                                            if (chars[i] === "\n" || chars[i] === " ") {
                                                break;
                                            }
                                        }
                                    }


                                    // If we have a marker search forward from
                                    // marker until terminator to get value
                                    if (startIndex >= 0) {
                                        value = "";
                                        for (i = startIndex; i <= chars.length - 1; i++) {
                                            if (chars[i] === "\n" || chars[i] === " ") {
                                                break;
                                            }
                                            value += chars[i];
                                        }
                                    }

                                    return {
                                        startIndex: startIndex,
                                        value: value
                                    };

                                },
                                bind: function($input, searchResult, e) {

                                    var keywords = searchResult.value;
                                    if (!keywords) {
                                        return;
                                    }

                                    // Remove any marker from search keywords
                                    if (keywords.substring(0, 1) === "@") {
                                        keywords = keywords.substring(1, keywords.length);
                                    }

                                    // Invoke suggester
                                    $caller.suggester({
                                            // pagedList options
                                            page: 1,
                                            pageSize: 5,
                                            itemSelection: {
                                                enable: true,
                                                index: 0,
                                                css: "active"
                                            },
                                            valueField: "keywords",
                                            config: {
                                                method: "GET",
                                                url:
                                                    'api/users/get?page={page}&size={pageSize}&keywords=' +
                                                        encodeURIComponent(keywords),
                                                data: {
                                                    sort: "LastLoginDate",
                                                    order: "Desc"
                                                }
                                            },
                                            itemCss: "dropdown-item",
                                            itemTemplate:
                                                '<a class="{itemCss}" href="{url}"><span class="avatar avatar-sm mr-2"><span style="background-image: url({avatar.url});"></span></span><span class="d-inline-block text-truncate w-50" title="{displayName}">{displayName}</span><span class="float-right d-inline-block text-truncate max-w-100 text-right" title="@{userName}">@{userName}</span></a>',
                                            parseItemTemplate: function(html, result) {

                                                if (result.id) {
                                                    html = html.replace(/\{id}/g, result.id);
                                                } else {
                                                    html = html.replace(/\{id}/g, "0");
                                                }

                                                if (result.displayName) {
                                                    html = html.replace(/\{displayName}/g, result.displayName);
                                                } else {
                                                    html = html.replace(/\{displayName}/g, "(no username)");
                                                }

                                                if (result.userName) {
                                                    html = html.replace(/\{userName}/g, result.userName);
                                                } else {
                                                    html = html.replace(/\{userName}/g, "(no username)");
                                                }

                                                if (result.avatar) {
                                                    if (result.avatar.url) {
                                                        html = html.replace(/\{avatar.url}/g, result.avatar.url);
                                                    } else {
                                                        html = html.replace(/\{avatar.url}/g, "#");
                                                    }
                                                }
                                               
                                                return html;

                                            },
                                            onPagerClick: function($self, page, e) {
                                                e.preventDefault();
                                                e.stopPropagation();
                                                $caller.suggester({
                                                        page: page
                                                    },
                                                    "show");
                                            },
                                            onItemClick: function($self, result, e) {

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
                                        },
                                        "show");

                                },
                                unbind: function($input, key, e) {
                                    $caller.suggester("hide");
                                }
                            }
                        ]
                    },
                    defaults);
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
                    // $(selector).mentions
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
                    // $().mentions
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
        mentions: mentions.init
    });

    app.ready(function () {
        
        // @mentions
        $('[data-provide="mentions"]').mentions();

        // @mentions
        $('.md-textarea').mentions();

        // bind suggesters
        $('.md-textarea').keyBinder("bind");

    });

}(window, document, jQuery));
