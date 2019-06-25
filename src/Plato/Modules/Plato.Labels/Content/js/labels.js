// <reference path="/wwwroot/js/app.js" />

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

    "use strict";

    // --------

    var app = win.$.Plato;

    // --------

    /* labelDropdown */
    var labelDropdown = function (options) {

        var dataKey = "labelDropDown",
            dataIdKey = dataKey + "Id";

       var defaults = {
           maxItems: 10
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

                this.bind($caller);

                return null;

            },
            bind: function ($caller) {
                
                // Maximum number of allowed selections
                var maxItems = $caller.data("maxItems")
                    ? parseInt($caller.data("maxItems"))
                    : $caller.data(dataKey).maxItems;
               
                // init select dropdown
                $caller.selectDropdown($.extend({
                    itemTemplate:
                        '<li class="list-group-item select-dropdown-item"><span class="btn btn-sm label font-weight-bold" style="background-color: {backColor}; color: {foreColor};">{name}</span><a href="#" class="btn btn-secondary float-right select-dropdown-delete" data-toggle="tooltip" title="Delete"><i class="fal fa-times"></i></a></li>',
                    parseItemTemplate: function (html, result) {

                        if (result.id) {
                            html = html.replace(/\{id}/g, result.id);
                        } else {
                            html = html.replace(/\{id}/g, "0");
                        }

                        if (result.name) {
                            html = html.replace(/\{name}/g, result.name);
                        } else {
                            html = html.replace(/\{name}/g, "(no name)");
                        }

                        if (result.foreColor) {
                            html = html.replace(/\{foreColor}/g, result.foreColor);
                        } else {
                            html = html.replace(/\{foreColor}/g, "");
                        }

                        if (result.backColor) {
                            html = html.replace(/\{backColor}/g, result.backColor);
                        } else {
                            html = html.replace(/\{backColor}/g, "");
                        }

                        return html;

                    },
                    onShow: function ($sender, $dropdown) {
                        // Focus search & set-up autoComplete on dropdown show
                        var $input = $dropdown.find('[type="search"]');
                        if ($input.length > 0) {
                            $input.focus()
                                .labelAutoComplete("show")
                                .labelAutoComplete("update");
                        }
                    },
                    onUpdated: function ($sender) {

                        // Set active items within dropdown when preview is updated
                        var $dropdown = $sender.find(".dropdown-menu"),
                            items = $sender.data("selectDropdown").items;

                        // Clear all selections within dropdown
                        $dropdown.find("label").each(function () {
                            var $ckb = $("#" + $(this).attr("for"));
                            if ($ckb.length > 0) {
                                $ckb.prop("checked", false);
                                $(this).removeClass("active");
                            }
                        });

                        // Set all selections within dropdown based on items within our array
                        if (items && items.length > 0) {
                            for (var i = 0; i < items.length; i++) {
                                var checkId = "label-" + items[i].id,
                                    $ckb = $dropdown.find("#" + checkId),
                                    $lbl = $dropdown.find('[for="' + checkId + '"]');
                                if ($ckb.length > 0) {
                                    $ckb.prop("checked", true);
                                }
                                if ($lbl.length > 0) {
                                    $lbl.addClass("active");
                                }
                            }
                        }
                    }
                },
                    defaults,
                    options));

                // init auto complete
                methods.getInput($caller).labelAutoComplete($.extend({
                    itemTemplate:
                        '<input type="checkbox" value="{id}" id="label-{id}"/><label for="label-{id}" class="{itemCss}"><i class="fal mr-2 check-icon"></i><span class="btn btn-sm label font-weight-bold" style="background-color: {backColor}; color: {foreColor}">{name}</span><span title="Occurrences" data-toggle="tooltip" class="float-right btn btn-sm btn-secondary">{totalEntities.text}</span></label>',
                    onItemClick: function ($input, result, e) {

                        e.preventDefault();
                        e.stopPropagation();
                        
                        // ensure we only add unique entries
                        var index = methods.getIndex($caller, result);
                        if (index === -1) {
                            var isBelowMax = maxItems > 0 && $caller.data("selectDropdown").items.length < maxItems;
                            if (isBelowMax) {
                                $caller.data("selectDropdown").items.push(result);
                            }
                        } else {
                            $caller.data("selectDropdown").items.splice(index, 1);
                        }
                        
                        $caller.selectDropdown("update");
                    },
                    onLoaded: function ($input) {
                        $caller.selectDropdown("update");
                    },
                    onHide: function ($input) {
                        // autoComplete will hide it's target if no input is provided
                        // Override this behaviour to ensure the auto complete target
                        // is always visible even if the search input is empty
                        $input
                            .autoComplete("show")
                            .autoComplete("update");
                    }
                },
                    defaults,
                    options));

            },
            getInput: function ($caller) {
                return $caller.find('[type="search"]');
            },
            getIndex: function ($caller, item) {

                var ensureUnique = $caller.data("selectDropdown").ensureUnique,
                    selectDropdown = $caller.data("selectDropdown"),
                    items = selectDropdown.items,
                    index = -1;
                if (ensureUnique === false) {
                    return index;
                }
                for (var i = 0; i < items.length; i++) {
                    if (item.id === items[i].id) {
                        index = i;
                    }
                }
                return index;
            },
            getUrl: function($caller) {

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
                    // $(selector).labelSelectDropdown()
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
                    // $().labelSelectDropdown()
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

    /* labelAutoComplete */
    var labelAutoComplete = function () {

        var dataKey = "labelAutoComplete",
            dataIdKey = dataKey + "Id";

        var defaults = {
            valueField: "keywords",
            config: {
                method: "GET",
                url: 'api/labels/get?pager.page={page}&pager.size={pageSize}&opts.search={keywords}',
                data: {
                    sort: "TotalEntities",
                    order: "Desc"
                }
            },
            itemCss: "dropdown-item",
            itemTemplate:
                '<a class="{itemCss}" href="{url}"><span class="btn btn-sm label font-weight-bold" style="background-color: {backColor}; color: {foreColor}">{name}</span><span title="Occurrences" data-toggle="tooltip" class="float-right btn btn-sm btn-secondary">{totalEntities.text}</span></a>',
            parseItemTemplate: function (html, result) {

                if (result.id) {
                    html = html.replace(/\{id}/g, result.id);
                } else {
                    html = html.replace(/\{id}/g, "0");
                }

                if (result.name) {
                    html = html.replace(/\{name}/g, result.name);
                } else {
                    html = html.replace(/\{name}/g, "(no name)");
                }

                if (result.foreColor) {
                    html = html.replace(/\{foreColor}/g, result.foreColor);
                } else {
                    html = html.replace(/\{foreColor}/g, "");
                }

                if (result.backColor) {
                    html = html.replace(/\{backColor}/g, result.backColor);
                } else {
                    html = html.replace(/\{backColor}/g, "");
                }

                if (result.totalEntities.text) {
                    html = html.replace(/\{totalEntities.text}/g, result.totalEntities.text);
                } else {
                    html = html.replace(/\{totalEntities.text}/g, "");
                }

                if (result.alias) {
                    html = html.replace("{alias}", result.alias);
                } else {
                    html = html.replace("{alias}", "");
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
                // init autoComplete
                $caller.autoComplete($caller.data(dataKey), methodName);

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
                    // $(selector).labelAutoComplete()
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
                    // $().labelAutoComplete()
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

    /* labelTagIt */
    var labelTagIt = function (options) {

        var dataKey = "labelTagIt",
            dataIdKey = dataKey + "Id";

        var defaults = {};

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

                return null;

            },
            bind: function ($caller) {

                // init tagIt
                $caller.tagIt($.extend({
                    itemTemplate:
                        '<li class="tagit-list-item"><span class="btn btn-sm label font-weight-bold" style="background-color: {backColor}; color: {foreColor};">{name} <a href="#" class="tagit-list-item-delete" data-toggle="tooltip" title="Delete" style="color: {foreColor};"><i class="fal fa-times"></i></a></span></li>',
                    parseItemTemplate: function (html, result) {

                        if (result.id) {
                            html = html.replace(/\{id}/g, result.id);
                        } else {
                            html = html.replace(/\{id}/g, "0");
                        }

                        if (result.name) {
                            html = html.replace(/\{name}/g, result.name);
                        } else {
                            html = html.replace(/\{name}/g, "(no name)");
                        }

                        if (result.foreColor) {
                            html = html.replace(/\{foreColor}/g, result.foreColor);
                        } else {
                            html = html.replace(/\{foreColor}/g, "");
                        }

                        if (result.backColor) {
                            html = html.replace(/\{backColor}/g, result.backColor);
                        } else {
                            html = html.replace(/\{backColor}/g, "");
                        }

                        return html;

                    },
                    onAddItem: function ($input, result, e) {
                        $input.val("");
                    }
                },
                    defaults,
                    options));

                // init auto complete

                methods.getInput($caller).labelAutoComplete($.extend({
                    onItemClick: function ($input, result, e) {

                        e.preventDefault();

                        // ensure we only add uunque entries
                        var index = methods.getIndex($caller, result);

                        if (index === -1) {
                            var tagit = $caller.data("tagIt");
                            tagit.items.push(result);
                            $caller.tagIt("update");
                        } else {
                            $caller.tagIt({
                                highlightIndex: index
                            },
                                "highlight");
                        }

                        $caller.tagIt("focus");
                        $caller.tagIt("select");

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
                    if (item.id === items[i].id) {
                        index = i;
                    }
                }
                return index;
            }

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
                    // $(selector).labelTagIt()
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
                    // $().labelTagIt()
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
        labelDropdown: labelDropdown.init,
        labelAutoComplete: labelAutoComplete.init,
        labelTagIt: labelTagIt.init
    });

    app.ready(function () {

        $('[data-provide="label-dropdown"]')
            .labelDropdown();

        $('[data-provide="labelAutoComplete"]')
            .labelAutoComplete();

        $('[data-provide="labelTagIt"]')
            .labelTagIt();

    });

}(window, document, jQuery));