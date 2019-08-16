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

    "use strict";

    // --------

    var app = win.$.Plato;

    // --------

    /* categorySelectDropdown */
    var categoryDropdown = function () {

        var dataKey = "categoryDropdown",
            dataIdKey = dataKey + "Id";

        var defaults = {
            onCategoryUpdated: function($caller) {}
        };

        var methods = {
            init: function($caller, methodName) {
                
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
                
                methods.bindSelect($caller);
                methods.bindTree($caller);

            },
            bindSelect: function ($caller) {

                // init selectDropdown
                $caller.selectDropdown($.extend({
                    itemTemplate:
                        '<li class="list-group-item"><span class="btn btn-sm label font-weight-bold" style="background-color: {backColor}; color: {foreColor};">{name}</span><a href="#" class="btn btn-secondary float-right select-dropdown-delete" data-toggle="tooltip" title="Delete"><i class="fal fa-times"></i></a></li>',
                    parseItemTemplate: function (html, $label) {
                        var $div = $('<div class="list-group-item">');
                        $div.append($label.clone().removeAttr("for"));
                        return $div;
                    },
                    onAddItem: function ($input, result, e) {
                        $input.val("");
                    },
                    onShow: function ($sender, $dropdown) {

                        // get tree
                        var $tree = $dropdown.find('[data-provide="tree"]');

                        // Focus & set-up search on dropdown show
                        var $input = $dropdown.find('[type="search"]');
                        if ($input.length > 0) {
                            $input.focus()
                                .filterList({
                                    target: $tree
                                });
                        }

                        // Expand tree view selection on dropdown shown
                        if ($tree.length > 0) {
                            $tree.treeView("scrollToSelected");
                        }

                    },
                    onChange: function ($dropdown, $input, e) {

                        e.preventDefault();
                        e.stopPropagation();

                        // Get all checked inputs within the dropdown
                        var $checked = $dropdown.find('input:checked');
                        if ($checked.length > 0) {
                            $checked.each(function () {

                                var checkId = $(this).attr("id"),
                                    $label = $dropdown.find('[for="' + checkId + '"]');
                                if ($label.length > 0) {
                                    var index = methods.getIndex($caller, $label);

                                    if (index === -1) {
                                        $dropdown.data("selectDropdown").items = [];
                                        $dropdown.data("selectDropdown").items.push($label);
                                        $dropdown.selectDropdown("update");
                                    } else {
                                        $dropdown.selectDropdown({
                                            highlightIndex: index
                                        },
                                            "highlight");
                                    }

                                }
                            });
                        } else {
                            $dropdown.data("selectDropdown").items = [];
                            $dropdown.selectDropdown("update");
                        }

                    },
                    onUpdated: function ($dropdown) {

                        // Raise an event so we know when the category has updated
                        var categoryId = 0,
                            items = $dropdown.data("selectDropdown").items;
                        if (items.length > 0) {
                            for (var i = 0; i < items.length; i++) {
                                if (items[i].data("categoryId")) {
                                    categoryId = parseInt(items[i].data("categoryId"));
                                    if (isNaN(categoryId)) {
                                        categoryId = 0;
                                    }
                                }
                            }
                        }

                        if ($caller.data(dataKey).onCategoryUpdated) {
                            $caller.data(dataKey).onCategoryUpdated($caller, categoryId);
                        }

                    }
                },
                    defaults,
                    $caller.data(dataKey)));

            },
            bindTree: function($caller) {

                // init treeView
                $caller.find('[data-provide="tree"]').treeView($.extend({
                            onClick: function ($tree, $link, e) {

                                e.preventDefault();
                                e.stopPropagation();

                                // Toggle checkbox when we click a tree node item
                                var $inputs = $link.find("input").first();
                                $inputs.each(function (i) {
                                    if ($(this).is(":checked")) {
                                        $(this).prop("checked", false);
                                    } else {
                                        $(this).prop("checked", true);
                                    }
                                    $(this).trigger("change");
                                });
                            },
                            onToggle: function ($tree, $toggler, e) {
                                // Prevent onClick raising when we toggle a node
                                e.preventDefault();
                                e.stopPropagation();
                                return true;
                            }
                        },
                        defaults,
                        $caller.data(dataKey)),
                    "expandAll");
            },
            getIndex: function($caller, $label) {

                // Has the item been added to our items array
                var ensureUnique = $caller.data("selectDropdown").ensureUnique,
                    selectDropdown = $caller.data("selectDropdown"),
                    items = selectDropdown.items,
                    index = -1;
                if (ensureUnique === false) {
                    return index;
                }
                for (var i = 0; i < items.length; i++) {
                    if ($label.attr("for") === items[i].attr("for")) {
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
                    // $(selector).categoryDropdown()
                    return this.each(function () {
                        if (!$(this).data(dataIdKey)) {
                            var id = dataKey + parseInt(Math.random() * 1000) + new Date().getTime();
                            $(this).data(dataIdKey, id);
                            $(this).data(dataKey, $.extend({}, defaults, options));
                        } else {
                            $(this).data(dataKey, $.extend({}, $(this).data(dataKey), options));
                        }
                        methods.init($(this), methodName);
                    });
                } else {
                    // $().categoryDropdown()
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

    // categoryIconModal
    var categoryIconModal = function () {

        var dataKey = "categoryIconModal",
            dataIdKey = dataKey + "Id";

        var defaults = {
                event: "click"
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

                methods.bind($caller);

            },
            bind: function ($caller) {

                var selectedIcon = null,
                    iconPrefix = $caller.attr("data-icon-prefix"),
                    $modal = $($caller.attr("href")),
                    $inputs = $modal.find("input[name=iconCss]"),
                    $checked = $modal.find("input[name=iconCss]:checked");
                   
                if ($checked.length > 0) {
                    selectedIcon = $checked.val();
                }

                $inputs.change(function () {
                    selectedIcon = $(this).val();
                });

                $modal.on('shown.bs.modal',
                    function (e) {
                        var $input = $modal.find('[type="search"]');
                        if ($input.length > 0) {
                            $input.focus();
                        }
                    });

                $modal.on('hidden.bs.modal',
                    function (e) {
                        if (selectedIcon === null) {
                            return;
                        }
                        if (selectedIcon === "") {
                            return;
                        }
                        var $i = $caller.find("i");
                        if ($i.length > 0) {
                            $i.removeClass()
                                .addClass(iconPrefix + selectedIcon);
                        }
                    });

            },
            unbind: function ($caller) {
                $caller.unbind('hidden.bs.modal');
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
                    // $(selector).selectIconModal
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
                    // $().selectIconModal
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
        categoryIconModal: categoryIconModal.init,
        categoryDropdown: categoryDropdown.init
    });

    app.ready(function () {

        $('[data-provide="icon-wrapper-modal"]')
            .categoryIconModal();

        /* category dropdown */
        $('[data-provide="category-dropdown"]')
            .categoryDropdown();
        
    });

}(window, document, jQuery));