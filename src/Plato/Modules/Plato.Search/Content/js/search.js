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

    /* searchDropDown */
    var searchDropDown = function () {
        
        var dataKey = "searchDropDown",
            dataIdKey = dataKey + "Id";

        var defaults = {};

        var methods = {
            init: function($caller, methodName) {
                
                if (methodName) {
                    if (this[methodName] !== null && typeof this[methodName] !== "undefined") {
                        this[methodName].apply(this, [$caller]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return;
                }

                methods.bind($caller);

            },
            bind: function($caller) {

                // On dropdown shown 
                $caller.on('shown.bs.dropdown',
                    function() {
                        $(this).find(".form-control").focus();
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
                    // $(selector).searchDropDown()
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
                    // $().searchDropDown()
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
            itemTemplate:
                '<a class="{itemCss}" href="{url}"><div style=\"display:inline-block; width: 85%; overflow:hidden; text-overflow: ellipsis;\"><span class="avatar avatar-sm mr-2"><span style="background-image: url({createdBy.avatar.url});"></span></span>{title}</div>{relevance}</a>',
            parseItemTemplate: function(html, result) {

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
                if (result.excerpt) {
                    html = html.replace(/\{excerpt}/g, result.userName);
                } else {
                    html = html.replace(/\{excerpt}/g, "(no excerpt)");
                }

                if (result.url) {
                    html = html.replace(/\{url}/g, result.url);
                } else {
                    html = html.replace(/\{url}/g, "#");
                }

                if (result.relevance) {
                    if (result.relevance > 0) {
                        html = html.replace(/\{relevance}/g,
                            '<span class="float-right badge badge-primary" data-provide="tooltip" title="Relevancy">' + result.relevance + '%</span>');
                    } else {
                        html = html.replace(/\{relevance}/g, "");
                    }

                } else {
                    html = html.replace(/\{relevance}/g, "");
                }
                
                if (result.createdBy) {

                    if (result.createdBy.avatar) {
                        if (result.createdBy.avatar.url) {
                            html = html.replace(/\{createdBy.avatar.url}/g, result.createdBy.avatar.url);
                        } else {
                            html = html.replace(/\{createdBy.avatar.url}/g, "#");
                        }
                    }
                }
                
                if (result.modifiedBy) {

                    if (result.modifiedBy.avatar) {
                        if (result.modifiedBy.avatar.url) {
                            html = html.replace(/\{modifiedBy.avatar.url}/g, result.modifiedBy.avatar.url);
                        } else {
                            html = html.replace(/\{modifiedBy.avatar.url}/g, "#");
                        }
                    }
                }
                
                if (result.lastReplyBy) {

                    if (result.lastReplyBy.avatar) {
                        if (result.lastReplyBy.avatar.url) {
                            html = html.replace(/\{lastReplyBy.avatar.url}/g, result.lastReplyBy.avatar.url);
                        } else {
                            html = html.replace(/\{lastReplyBy.avatar.url}/g, "#");
                        }
                    }
                }


                return html;

            },
            onKeyDown: function($caller, e) {
                if (e.keyCode === 13) {
                    e.preventDefault();
                    var $btn = $caller.parent().find(".btn");
                    if ($btn.length > 0) {
                        $btn[0].click();
                    }
                }
            },
            onItemClick: function($caller, result, e) {
                return;
            }
        };

        var methods = {
            init: function($caller, methodName, func) {

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
            show: function($caller) {
                $caller.autoComplete("show");
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
        searchAutoComplete: searchAutoComplete.init,
        searchDropDown: searchDropDown.init
    });

    app.ready(function () {

        // Search auto complete
        $('[data-provide="searchAutoComplete"]')
            .searchAutoComplete();

        // Search drop down menu
        $('[data-provide="searchDropDown"]')
            .searchDropDown();
        
    });

}(window, document, jQuery));
