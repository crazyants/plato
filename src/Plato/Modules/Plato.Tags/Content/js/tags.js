
if (typeof jQuery === "undefined") {
    throw new Error("Plato requires jQuery");
}

if (typeof $.Plato === "undefined") {
    throw new Error("$.Plato Required");
}

$(function (win, doc, $) {

    'use strict';

    var app = win.$.Plato;

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
            parseItemTemplate: function(html, result) {

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
            onKeyDown: function($caller, e) {
                if (e.keyCode === 13) {
                    e.preventDefault();
                }
            },
            onItemClick: function($caller, result, e) {
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
    
    /* tagTagIt */
    var tagTagIt = function (options) {

        var dataKey = "tagTagIt",
            dataIdKey = dataKey + "Id";

        var defaults = {}

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

                this.bind($caller);

            },
            bind: function ($caller) {

                // init tagIt
                $caller.tagIt($.extend({
                    itemTemplate:
                        '<li class="tagit-list-item"><div class="btn-group"><div class="btn btn-sm label label-outline font-weight-bold">{name}</div><div class="btn btn-sm label label-outline dropdown-toggle-split tagit-list-item-delete"><i class="fal fa-times"></i></div></div></li>',
                    parseItemTemplate: function (html, result) {

                        if (result.id) {
                            html = html.replace(/\{id}/g, result.id);
                        } else {
                            html = html.replace(/\{id}/g, "0");
                        }
                        if (result.name) {
                            html = html.replace(/\{name}/g, win.$.Plato.Text.htmlEncode(result.name));
                        } else {
                            html = html.replace(/\{name}/g, "");
                        }
                        return html;
                    },
                    onAddItem: function ($input, result, e) {
                        $input.val("");
                    }
                },
                    defaults,
                    options));

                // user auto complete
                methods.getInput($caller).tagAutoComplete($.extend({
                        onItemClick: function($input, result, e) {

                            e.preventDefault();
                            
                            // ensure we only add uunque entries
                            var index = methods.getIndex($caller, result);

                            if (index === -1) {
                                var tagit = $caller.data("tagIt");
                                tagit.items.push(result);
                                $caller.tagIt("update")
                                    .tagIt("focus")
                                    .tagIt("reset")
                                    .tagIt("show");
                            } else {
                                $caller.tagIt({
                                        highlightIndex: index
                                    },
                                    "highlight");
                            }

                        },
                        onKeyDown: function($input, e) {

                            // handle carriage returns & comma (without modifier)
                            var noMod = (!e.shiftKey && !e.ctrlKey),
                                isComma = (noMod && e.keyCode === 188),
                                isCarriageReturn = (noMod && e.keyCode === 13);
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
                                                $target.find("a." + itemCss).each(function () {
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
                                }

                                // ensure we only add uunque entries
                                var index = methods.getIndex($caller, result);
                                if (index === -1) {
                                    $caller.data("tagIt").items.push(result);
                                    $caller
                                        .tagIt("update")
                                        .tagIt("reset")
                                        .tagIt("focus");
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
            getInput: function ($caller) {
                return $caller.find(".tagit-list-item-input").find("input");
            },
            getIndex: function ($caller, item) {
                var ensureUnique = $caller.data("tagIt").ensureUnique,
                    tagit = $caller.data("tagIt"),
                    items = tagit.items,
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


    $.fn.extend({
        tagTagIt: tagTagIt.init,
        tagAutoComplete: tagAutoComplete.init
    });
    
    $(doc).ready(function () {

        $('[data-provide="tagTagIt"]')
            .tagTagIt();

        $('[data-provide="tagsAutoComplete"]')
            .tagAutoComplete();

    });

}(window, document, jQuery));
