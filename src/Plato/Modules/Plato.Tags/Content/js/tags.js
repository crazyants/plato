// <reference path="/wwwroot/js/app.js" />

if (typeof window.jQuery === "undefined") {
    throw new Error("jQuery 3.3.1 or above Required");
}

if (typeof $().modal === 'undefined') {
    throw new Error("BootStrap 4.1.1 or above Required");
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
    
    /* tagTagIt */
    var tagTagIt = function (options) {

        var dataKey = "tagTagIt",
            dataIdKey = dataKey + "Id";

        var defaults = {
            maxItems: 5
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

                this.bind($caller);

            },
            bind: function($caller) {
                
                // Maximum number of allowed selections
                var maxItems = methods.getMaxItems($caller);

                // init tagIt
                $caller.tagIt($.extend({
                        maxItems: maxItems,
                        itemTemplate:
                            '<li class="tagit-list-item"><div class="btn-group"><div class="btn btn-sm label label-outline font-weight-bold">{name}</div><a href="#" class="btn btn-sm label label-outline dropdown-toggle-split tagit-list-item-delete"><i class="fal fa-times"></i></a></div></li>',
                        parseItemTemplate: function(html, result) {

                            if (result.id) {
                                html = html.replace(/\{id}/g, result.id);
                            } else {
                                html = html.replace(/\{id}/g, "0");
                            }
                            if (result.name) {
                                html = html.replace(/\{name}/g, app.text.htmlEncode(result.name));
                            } else {
                                html = html.replace(/\{name}/g, "");
                            }
                            return html;
                        },
                        onAddItem: function($input, result, e) {
                            $input.val("");
                        }
                    },
                    defaults,
                    options));

                // user auto complete
                methods.getInput($caller).tagAutoComplete($.extend({
                        onItemClick: function($input, result, e) {

                            e.preventDefault();

                            // ensure we only add unique entries
                            var index = methods.getIndex($caller, result);
                            if (index === -1) {
                                var isBelowMax = maxItems > 0 && $caller.data("tagIt").items.length < maxItems;
                                if (isBelowMax) {
                                    $caller.data("tagIt").items.push(result);
                                    $caller.tagIt("update")
                                        .tagIt("focus")
                                        .tagIt("reset")
                                        .tagIt("show");
                                    if ($caller.data("tagIt").items.length >= maxItems) {
                                        $caller.tagIt("hide");
                                    }
                                } else {
                                    $caller.tagIt("hide");
                                }
                            } else {
                                $caller.tagIt({
                                        highlightIndex: index
                                    },
                                    "highlight");
                            }

                        },
                        onKeyDown: function($input, e) {

                            // handle carriage returns & comma (without modifier)
                            var noMod = !e.shiftKey && !e.ctrlKey,
                                isComma = noMod && e.keyCode === 188,
                                isCarriageReturn = noMod && e.keyCode === 13;
                            if (isCarriageReturn | isComma) {

                                e.preventDefault();

                                // Add dropdown selection if available
                                var target = $input.data("autocompleteTarget") ||
                                    $caller.data("autoComplete").target;
                                if (target !== null) {
                                    var $target = $(target);
                                    if ($target) {
                                        if ($target.data("pagedList")) {
                                            var itemCss = $target.data("pagedList").itemCss;
                                            if (itemCss) {
                                                // Do we have a selection within our dropdown
                                                $target.find("a." + itemCss).each(function() {
                                                    if ($(this).hasClass("active")) {
                                                        return;
                                                    }
                                                });
                                            }
                                        }
                                    }
                                }

                                // Ensure we have a value to add
                                var value = $input.val();
                                if (value === "") {
                                    return;
                                }

                                // Json to represent value
                                var result = {
                                    id: 0,
                                    name: value
                                };

                                // ensure we only add unique entries
                                var index = methods.getIndex($caller, result);
                                if (index === -1) {
                                    var isBelowMax = maxItems > 0 && $caller.data("tagIt").items.length < maxItems;
                                    if (isBelowMax) {
                                        $caller.data("tagIt").items.push(result);
                                        $caller
                                            .tagIt("update")
                                            .tagIt("reset")
                                            .tagIt("focus");
                                        if ($caller.data("tagIt").items.length >= maxItems) {
                                            $caller.tagIt("hide");
                                        }
                                    } else {
                                        $caller.tagIt("hide");
                                    }
                                } else {
                                    $caller.tagIt({
                                            highlightIndex: index
                                        },
                                        "highlight");
                                }

                            }

                        }
                    },
                    defaults,
                    options));


            },
            getInput: function($caller) {
                return $caller.find(".tagit-list-item-input").find("input");
            },
            getIndex: function($caller, item) {
                var ensureUnique = $caller.data("tagIt").ensureUnique,
                    items = $caller.data("tagIt").items,
                    index = -1;
                if (ensureUnique === false) {
                    return index;
                }
                for (var i = 0; i < items.length; i++) {
                    if (item.name === items[i].name) {
                        index = i;
                    }
                }
                return index;
            },
            getMaxItems: function ($caller) {
                return $caller.data("maxItems")
                    ? parseInt($caller.data("maxItems"))
                    : $caller.data(dataKey).maxItems;
            },

        };

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
                    // $(selector).tagTagIt()
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
                    // $().tagTagIt()
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

    /* tagAutoComplete */
    var tagAutoComplete = function () {

        var dataKey = "tagAutoComplete",
            dataIdKey = dataKey + "Id";

        var defaults = {
            valueField: "keywords",
            config: {
                method: "GET",
                url: 'api/tags/tag/get?page={page}&size={pageSize}&keywords={keywords}',
                data: {
                    sort: "TotalEntities",
                    order: "Desc"
                }
            },
            itemTemplate: '<a class="{itemCss}" href="{url}">{name}</a>',
            parseItemTemplate: function (html, result) {

                if (result.id) {
                    html = html.replace(/\{id}/g, result.id);
                } else {
                    html = html.replace(/\{id}/g, "0");
                }

                if (result.name) {
                    html = html.replace(/\{name}/g, app.text.htmlEncode(result.name));
                } else {
                    html = html.replace(/\{name}/g, "(no name)");
                }

                if (result.url) {
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
        };

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
                    // $(selector).tagAutoComplete()
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
                    // $().tagAutoComplete()
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
        tagTagIt: tagTagIt.init,
        tagAutoComplete: tagAutoComplete.init
    });
    
    app.ready(function () {

        $('[data-provide="tagTagIt"]')
            .tagTagIt();

        $('[data-provide="tagsAutoComplete"]')
            .tagAutoComplete();

    });

}(window, document, jQuery));
