// <reference path="~/js/vendors/jquery.js" />
// <reference path="~/js/vendors/bootstrap.js" />

/*======================================================================*/
// Plato UI
// © InstantASP Ltd.
/*======================================================================*/

if (typeof jQuery === "undefined") {
    throw new Error("Plato UI requires jQuery 3.3.1");
}

if (typeof $().modal === 'undefined') {
    throw new Error("Plato UI requires BootStrap 4.1.1");
}

if (typeof $.Plato.Context === "undefined") {
    throw new Error("Plato UI requires $.Plato.Context");
}

if (typeof $.Plato.Http === "undefined") {
    throw new Error("Plato UI requires $.Plato.Http");
}

$(function (win, doc, $) {

    var context = $.Plato.Context;

    /* scrollTo */
    var scrollTo = function () {

        var dataKey = "scrollTo",
            dataIdKey = dataKey + "Id";

        var defaults = {
            event: "click",
            onBeforeComplete: function () { },
            onComplete: function () { }
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

                var event = $caller.data(dataKey).event;
                if (event) {
                    $caller.on(event,
                        function (e) {
                            e.preventDefault();
                            methods.scrollTo($caller);


                        });
                }

            },
            scrollTo: function ($caller) {

                jQuery.extend(jQuery.easing,
                    {
                        def: 'easeOutQuad',
                        easeInOutExpo: function (x, t, b, c, d) {
                            if (t === 0) return b;
                            if (t === d) return b + c;
                            if ((t /= d / 2) < 1) return c / 2 * Math.pow(2, 10 * (t - 1)) + b;
                            return c / 2 * (-Math.pow(2, -10 * --t) + 2) + b;
                        }
                    });

                var href = $caller.attr("href");
                if (href) {
                    var $target = $(href);
                    if ($target.length > 0) {
                        $('html, body').stop().animate({
                            scrollTop: $target.offset().top - 10
                        },
                            250,
                            'easeInOutExpo',
                            function () {
                                $caller.data(dataKey).onComplete($caller, $target);
                            });
                        $caller.data(dataKey).onBeforeComplete($caller, $target);
                    }
                }

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
                    // $(selector).markdownEditor
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
                    // $().markdownEditor 
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

    /* treeView */
    var treeView = function () {

        var dataKey = "treeView",
            dataIdKey = dataKey + "Id";

        var defaults = {
            selectedNodeId: null,
            event: "click",
            toggleSelector: '[data-toggle="tree"]',
            linkSelector: ".list-group-item",
            enableCheckBoxes: true,
            onBeforeComplete: function () { },
            onClick: function ($caller, $link, e) { }
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

                // Bind events
                methods.bind($caller);

            },
            bind: function ($caller) {

                var linkSelector = $caller.data(dataKey).linkSelector,
                    toggleSelector = $caller.data(dataKey).toggleSelector,
                    event = $caller.data(dataKey).event;

                // Bind toggler events
                $(toggleSelector).unbind(event).bind(event,
                    function (e) {
                        e.preventDefault();
                        methods._toggleNode($caller, $(this).attr("data-node-id"));
                    });

                $caller.on('change',
                    'input[type="checkbox"], input[type="radio"]',
                    function (e) {
                        $caller.find(".list-group-item").each(function () {
                            $(this).removeClass("active").removeClass("checked");
                        });
                        $caller.find('input:checked').each(function () {
                            var checkId = $(this).attr("id");
                            var $lbl = $('[for="' + checkId + '"]');
                            if ($lbl.length > 0) {
                                var $item = $lbl.closest(".list-group-item");
                                $item.addClass("active").addClass("checked");
                            }
                        });
                    });

                // Check / Uncheck child inputs
                $caller.on('change',
                    'input[type="checkbox"]',
                    function (e) {
                        var nodeId = $(this).attr("data-node-id"),
                            $li = methods.getNodeListItem($caller, nodeId),
                            $firstChild = $li.find("ul"),
                            $inputs = $firstChild.find('[type="checkbox"]');
                        if ($inputs.length > 0) {
                            if ($(this).is(":checked")) {
                                $inputs.prop("checked", true);
                            } else {
                                $inputs.prop("checked", false);
                            }
                        }
                    });

                // Bind link click events
                $caller.find(linkSelector).unbind(event).bind(event,
                    function (e) {
                        e.preventDefault();
                        e.stopPropagation();
                        if ($caller.data(dataKey).onClick) {
                            $caller.data(dataKey).onClick($caller, $(this), e);
                        }
                    });

                // Bind active state on mouse events
                $caller.find(".list-group-item").unbind("mouseover").bind("mouseover",
                    function (e) {
                        e.stopPropagation();
                        $(this).parents(".list-group-item").each(function () {
                            if (!$(this).hasClass("checked")) {
                                $(this).removeClass("active");
                            }
                        });
                        $(this).addClass("active");
                    });

                $caller.find(".list-group-item").unbind("mouseleave").bind("mouseleave",
                    function () {
                        if (!$(this).hasClass("checked")) {
                            $(this).removeClass("active");
                        }
                    });

            },
            expand: function ($caller) {
                var nodeId = $caller.data(dataKey).selectedNodeId;
                if (!nodeId) {
                    return;
                }
                if (!$caller.hasClass("show")) {
                    methods._expand($caller, nodeId);
                }
            },
            collapse: function ($caller) {
                var nodeId = $caller.data(dataKey).selectedNodeId;
                if (!nodeId) {
                    return;
                }
                if ($caller.hasClass("show")) {
                    methods._collapse($caller, nodeId);
                }
            },
            toggle: function ($caller) {
                if ($caller.hasClass("show")) {
                    methods.collapse($caller);
                } else {
                    methods.expand($caller);
                }
            },
            expandAll: function ($caller) {
                $caller.find(".list-group-item").each(function () {
                    methods._expand($caller, $(this).attr("id"));
                });
            },
            expandSelected: function ($caller) {
                $caller.find(".active").each(function () {
                    methods._expandParents($caller, $(this).attr("id"));
                });
            },
            collapseAll: function ($caller) {
                $caller.find(".list-group-item").each(function () {
                    methods._collapse($caller, $(this).attr("id"));
                });
            },
            _toggleNode: function ($caller, nodeId) {
                var $item = methods.getNodeListItem($caller, nodeId);
                if ($item.hasClass("show")) {
                    methods._collapse($caller, nodeId, true);
                } else {
                    methods._expand($caller, nodeId, true);
                }
            },
            _expand: function ($caller, nodeId, slide) {
                var $li = methods.getNodeListItem($caller, nodeId),
                    $child = $li.find("ul").first();
                $li.addClass("show");
                if (slide) {
                    $child.slideDown("fast");
                } else {
                    $child.show();
                }
            },
            _expandParents: function ($caller, nodeId) {
                var $li = methods.getNodeListItem($caller, nodeId);
                $li.parents(".list-group-item").each(function () {
                    methods._expand($caller, $(this).attr("id"), false);
                });
            },
            _collapse: function ($caller, nodeId, slide) {
                var $li = methods.getNodeListItem($caller, nodeId),
                    $child = $li.find("ul").first();
                $li.removeClass("show");
                if (slide) {
                    $child.slideUp("fast");
                } else {
                    $child.hide();
                }
            },
            getNodeListItem: function ($caller, nodeId) {
                return $caller.find("#" + nodeId);
            },
            getNodeToggler: function ($caller, nodeId) {
                var $toggler = $caller.find('[data-node-id="' + nodeId + '"]');
                if ($toggler.length > 0) {
                    if ($toggler[0].tagName === "I") {
                        return $toggler;
                    }
                    return $toggler.find("i");
                }
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
                    // $(selector).markdownEditor
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
                    // $().markdownEditor 
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

    /* autoComplete */
    var autoComplete = function () {

        var dataKey = "autoComplete",
            dataIdKey = dataKey + "Id";

        var defaults = {
            page: 1,
            pageSize: 10,
            config: { }, // optional configuration options for ajax request
            target: null, // optional target selector for auto complete results. if no target a dropdown-menu is used
            enablePaging: true, // indicates if paging should be enabled for results
            alwaysShow: true, // if true the autocomplete results will be displayed even if no results are returned from the server, if false the dropdown is only shown if results are returned
            onShow: null, // triggers when the autocomplete target is displayed
            onHide: null, // triggers when the autocomplete target is hidden
            onLoad: function ($caller, $target, results) {
                
                if (results) {

                    $target.empty();
                    
                    // build results
                    for (var i = 0; i < results.data.length; i++) {
                        $target.append(this.buildItem($caller, results.data[i]));
                    }
                    // build paging
                    var enablePaging = $caller.data(dataKey).enablePaging;
                    if (enablePaging) {
                        if (results.total > results.size) {
                            $target.append(this.buildPager($caller, results));
                        }
                    }
                } else {
                    // no data
                    $target.empty().append(this.buildNoResults($caller));
                }
                
                if ($caller.data(dataKey).onLoaded) {
                    $caller.data(dataKey).onLoaded($caller, $target);
                }

                $target.platoUI();

            }, // triggers after autocomplete results have finished loading
            onLoaded: null, // triggers after autocomplete results have been added to the dom
            onKeyDown: null, // triggers for key down events within the autocomplete input element
            onItemClick: null, // event raised when you click an autocomplete result item
            buildItem: function($caller, result) {

                // apply default css
                var itemTemplate = $caller.data(dataKey).itemTemplate,
                    itemCss = $caller.data("autocompleteItemCss") || $caller.data(dataKey).itemCss;
                itemTemplate = itemTemplate.replace(/\{itemCss}/g, itemCss);

                // parse template
                if ($caller.data(dataKey) && $caller.data(dataKey).parseItemTemplate) {
                    itemTemplate = $caller.data(dataKey).parseItemTemplate(itemTemplate, result);
                }

                // bind onItemClick

                var $item = $(itemTemplate);
                $item.click(function (e) {
                    if ($caller.data(dataKey).onItemClick) {
                        $caller.data(dataKey).onItemClick($caller, result, e, $item);
                    }
                });
                return $item;
            },
            buildPager: function($caller, results) {

                var $div = $('<div class="d-block">');
                if (results.page > 1) {
                    $div.append(this.buildPrev($caller, results.page));
                } else {
                    $div.append($('<div class="float-left col-3">'));
                }

                $div.append(this.buildInfo($caller, results));

                if (results.page < results.totalPages) {
                    $div.append(this.buildNext($caller, results.page));
                } else {
                    $div.append($('<div class="float-left col-3">'));
                }


                return $div;

            },
            buildPrev: function ($caller, page) {

                var icon = $("<i>").addClass("fa fa-chevron-left");
                var a = $("<a>")
                    .attr("href", "#")
                    .addClass("list-group-item list-group-item-action float-left col-3 text-center")
                    .append(icon);

                a.click(function(e) {
                    e.preventDefault();
                    e.stopPropagation();
                    if (page > 1) {
                        $caller.autoComplete({
                                page: page -= 1
                            },
                            "update");
                    }
                });

                return a;

            },
            buildNext: function ($caller, page) {

                var icon = $("<i>").addClass("fa fa-chevron-right");
             
                var a = $("<a>")
                    .attr("href", "#")
                    .addClass("list-group-item list-group-item-action float-left col-3 text-center")
                    .append(icon);

                a.click(function(e) {
                    e.preventDefault();
                    e.stopPropagation();
                    $caller.autoComplete({
                            page: page += 1
                        },
                        "update");
                });

                return a;

            },
            buildNoResults: function($caller) {

                var noResultsText = $caller.data(dataKey).noResultsText;

                var a = $("<div>")
                    .addClass("text-center p-4")
                    .append(noResultsText);

                var li = $('<li class="no-results">');
                li.append(a);

                return li;

            },
            buildInfo: function($caller, results) {

                var pages = "{0} of {1}";
                pages = pages.replace("{0}", results.page);
                pages = pages.replace("{1}", results.totalPages);

                var total = "{0} results";
                if (results.total === 1) {
                    total = "{0} result";
                }
                total = total.replace("{0}", results.total);

                
                var $div = $('<div>').addClass("list-group-item float-left col-6 text-center");
                $div.text(pages + ", " + total);
                return $div;

            },
            itemCss: "dropdown-item", // the CSS to apply to links within the itemTemplate
            itemTemplate:
                '<a class="{itemCss}" href="{url}"><span style="white-space: nowrap;overflow: hidden;text-overflow: ellipsis;max-width: 85%;">{text}</span> <span style="opacity: .7;">@{value}</span><span class="float-right">{rank}</span></a>',
            parseRequestValue:
                null, // optional method which can be used to parse the textbox value before passing to our API endpoint
            parseItemTemplate: function (html, result) {
                
                if (result.text) {
                    html = html.replace(/\{text}/g, result.text);
                }
                if (result.value) {
                    html = html.replace(/\{value}/g, result.value);
                }
                if (result.url) {
                    html = html.replace(/\{url}/g, result.url);
                }
                if (result.rank) {
                    if (result.rank > 0) {
                        html = html.replace(/\{rank}/g,
                            '<span class="label label-right label-primary">' + result.rank + '%</span>');
                    } else {
                        html = html.replace(/\{rank}/g, "");
                    }
                } else {
                    html = html.replace(/\{rank}/g, "");
                }

                return html;

            }, // provides a method to parse our itemTemplate with data returned from service url
            loaderTemplate:
                '<p class="text-center"><i class="fal fa-spinner fa-spin" ></i></p >', // a handlebars style template for auto complete list items
            noResultsText: "Sorry no results matched your search!", // the text to display when no results are available
            valueField: "Keywords" // the name of the parameter passed to the server with the input value
        };

        var methods = {
            init: function ($caller, methodName) {

                if (methodName) {
                    if (this[methodName] !== null) {
                        this[methodName].apply(this, [$caller]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return;
                }
                
                // disable browser autocomplete if not already disabled
                var attr = $caller.attr("autocomplete");
                if (typeof attr === typeof undefined || attr === false) {
                    $caller.attr("autocomplete", "off");
                }
                
                // bind events
                methods.bind($caller);

            },
            bind: function ($caller) {

                $caller.bind("focus", function () {
                    methods.resetUI($(this));
                    if ($(this).val().length === 0) {
                        methods.hide($(this));
                    } else {
                        // Show
                        methods.show($(this), function() {
                            // Update if not already visible
                            methods.update($caller);
                        });
                  
                    }
                });

                $caller.bind("keydown", function (e) {
                    if ($caller.data(dataKey).onKeyDown) {
                        $caller.data(dataKey).onKeyDown($(this), e);
                    }
                });

                // spy on our input
                $caller.typeSpy({
                    onKeyUp: function ($el, e) {
                        methods.resetUI($el);
                        if (e.keyCode === 27) {
                            // escape
                            methods.hide($el);
                        }
                    },
                    onChange: function ($el, e) {
                        methods.setPageIndex($caller, 1);
                        // !escape && !tab
                        if (e.keyCode !== 27 && e.keyCode !== 9) {
                            if ($el.val().length === 0) {
                                methods.hide($el);
                            } else {
                                // Show
                                methods.show($el);
                                // Update
                                methods.update($el);
                            }
                        }
                    }
                });

                // hide menu
                $(doc).click(function (e) {
                    var target = e.target;
                    if (target) {
                        if (target.tagName.toUpperCase() === "INPUT") { return; }
                        if (target.tagName.toUpperCase() === "A") { return; }
                        if (target.tagName.toUpperCase() === "UL") { return; }
                        if (target.tagName.toUpperCase() === "I") { return; }
                    }
                    methods.hide($caller);
                });

            },
            unbind: function ($caller) {
                $caller
                    .unbind("focus")
                    .unbind("keydown");
                $caller.typeSpy("unbind");
            },
            show: function ($caller, onShow) {
                var $target = this.getTarget($caller);
                if ($target.length > 0) {

                    // Allows for initially hidden state
                    if ($target.hasClass("hidden")) {
                        $target.removeClass("hidden");
                    }

                    // Show our target
                    if (!$target.hasClass("show")) {
                        $target.addClass("show");
                        if (onShow) {
                            onShow();
                        }
                    }
                }
            },
            hide: function ($caller, onHide) {
                var $target = this.getTarget($caller);
                if ($target.length > 0) {

                    // Allows for initially hidden state
                    if (!$target.hasClass("hidden")) {
                        $target.addClass("hidden");
                    }

                    // Hide our target
                    if ($target.hasClass("show")) {
                        $target.removeClass("show");
                        if (onHide) {
                            onHide();
                        }
                    }
                }
            },
            update: function ($caller) {

                // no dropdown target found
                var $target = methods.getTarget($caller);
                if ($target.length === 0) {
                    return;
                }

                // add loader for updates
                var loaderTemplate = $caller.data(dataKey).loaderTemplate;
                $target.empty().append(loaderTemplate);
                
                var page = $caller.data(dataKey).page;
                if (page) {
                    this.setPageIndex($caller, page);
                }
                
                // ---- configure

                var config = this.getConfig($caller),
                    val = this.getParsedValue($caller);
                
                // ensure we have a parsed search value
                //if (val === "") { return; }

                // ------ request
                
                // set content type for post data
                if (config.method) {
                    if (config.method.toUpperCase() === "POST") {
                        config.headers = {
                            'Content-Type': 'application/json; charset=utf-8'
                        };
                    }
                }

                // serialize post data 
                if (typeof config.data !== "string") {
                    config.data = JSON.stringify(config.data);
                }

                win.$.Plato.Http(config).done(function(response) {
                    if (response.statusCode !== 200) {
                        return;
                    }
                    if ($caller.data(dataKey).onLoad) {
                        $caller.data(dataKey).onLoad($caller, $target, response.result);
                    }
                });
                
            },
            getConfig: function ($caller) {

                // clone config - we reuse this config upon each request 
                // so don't want to make changes to the original config
                var config = $.extend({}, $caller.data(dataKey).config),
                    url = $caller.data("autocompleteUrl") || config.url,
                    pageIndex = this.getPageIndex($caller) || 1,
                    pageSize = this.getPageSize($caller) || 10,
                    keywords = this.getParsedValue($caller) || "";
                
                if (url) {
                    if (url.indexOf("{page}") >= 0) {
                        url = url.replace(/\{page}/g, pageIndex);
                    }
                    if (url.indexOf("{pageSize}") >= 0) {
                        url = url.replace(/\{pageSize}/g, pageSize);
                    }

                    var valueField = this.getValueField($caller);
                    if (keywords) {
                        if (valueField) {
                            if (url.indexOf("{" + valueField + "}") >= 0) {
                                url = url.replace("{" + valueField + "}", keywords);
                            }
                        }
                        if (typeof config.data === "object" && config.method.toUpperCase() === "POST") {
                            if (valueField) {
                                config.data[valueField] = keywords;
                            }
                        }
                    } else {
                        url = url.replace("{" + valueField + "}", "");
                    }
                    config.url = url;
                }
                
                return config;

            },
            getValueField: function ($caller) {
                return $caller.data("autocompleteValueField") || $caller.data(dataKey).valueField;
            },
            getParsedValue: function ($caller) {
                // prepare request value
                var val = $caller.val();
                if ($caller.data(dataKey) && $caller.data(dataKey).parseRequestValue) {
                    val = $caller.data(dataKey).parseRequestValue(val);
                }
                return val;
            },
            setPageIndex: function ($caller, pageIndex) {
                $caller.data(dataKey).page = pageIndex;
            },
            getPageIndex: function ($caller) {
                return $caller.data("autocompletePageIndex") || $caller.data(dataKey).page;
            },
            setPageSize: function ($caller, pageSize) {
                $caller.data(dataKey).pageSize = pageSize;
            },
            getPageSize: function ($caller) {
                return $caller.data("autocompletePageSize") || $caller.data(dataKey).pageSize;
            },
            getTarget: function ($caller) {

                // do we have an explicit target
                var target = $caller.data("autocompleteTarget") ||
                    $caller.data(dataKey).target,
                    dynamicId = $caller.data(dataIdKey) + "_target";

                if (target) {
                    var $target = $(target);
                    if ($target.length > 0) {
                        return $target;
                    }
                } else {

                    // is our next element the dropdown?
                    var $next = $caller.next();
                    if ($next.hasClass("dropdown-menu")) {
                        if (!$next.attr("id")) {
                            $next.attr("id", dynamicId);
                        }
                        return $next;
                    }

                    // else create a dropdown
                    var loaderTemplate = $caller.data(dataKey).loaderTemplate;

                    var $ul = $("#" + dynamicId);
                    if ($ul.length === 0) {

                        // build ul
                        $ul = $("<ul>")
                            .attr("id", dynamicId)
                            .css({ "width": "100%" })
                            .addClass("i-dropdown-menu i-dropdown-menu-nowrap")
                            .html(loaderTemplate);

                        // append
                        $caller.after($ul);

                    }

                    return $ul;

                }

                return null;

            },
            alwaysShow: function ($caller) {
                return $caller.data("autocompleteAlwaysShow")
                    ? $caller.data("autocompleteAlwaysShow")
                    : ($caller.data(dataKey) ? $caller.data(dataKey).alwaysShow : true);
            },
            resetUI: function ($caller) {
                var $target = methods.getTarget($caller);
                if ($target) {
                    //$target.find(".tooltip").itooltip("hideAll");
                }
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
                    // $(selector).autoComplete()
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
                    // $().autoComplete()
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

    /* userAutoComplete */
    var userAutoComplete = function () {

        var dataKey = "userAutoComplete",
            dataIdKey = dataKey + "Id";

        var defaults = {
            valueField: "keywords",
            config: {
                method: "GET",
                url: 'api/users/get?page={page}&size={pageSize}&keywords={keywords}',
                data: {
                    sort: "LastLoginDate",
                    order: "Desc"
                }
            },

            itemTemplate:
                '<a class="{itemCss}" href="{url}"><span class="avatar avatar-sm mr-2"><span style="background-image: url(/users/photo/{id});"></span></span>{displayName}<span class="float-right">@{userName}</span></a>',
            parseItemTemplate: function (html, result) {

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
                    if (this[methodName] != null) {
                        this[methodName].apply(this, [$caller]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return null;
                }

                // init autoComplete
                $caller.autoComplete($caller.data(dataKey), methodName);

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

    /* labelAutoComplete */
    var labelAutoComplete = function () {

        var dataKey = "labelutoComplete",
            dataIdKey = dataKey + "Id";

        var defaults = {
            valueField: "keywords",
            config: {
                method: "GET",
                url: 'api/labels/get?page={page}&size={pageSize}&keywords={keywords}',
                data: {
                    sort: "TotalEntities",
                    order: "Desc"
                }
            },
            itemCss: "dropdown-item",
            //buildPager: function ($caller, results) {
                
            //    // Not last page
            //    if (results.page !== results.totalPages) {
                    
            //        var icon = $("<i>").addClass("fa fa-chevron-down"),
            //            itemCss = $caller.data("autocompleteItemCss") || $caller.data(dataKey).itemCss;

            //        var $a = $("<a>")
            //            .attr("href", "#")
            //            .addClass(itemCss)
            //            .addClass("text-center")
            //            .append(icon);

            //        $a.click(function (e) {
            //            e.preventDefault();
            //            $caller.autoComplete({
            //                    page: results.page += 1
            //                },
            //                "update");
            //        });

            //        return $a;

            //    }

            //    return null;

            //},
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
        }

        var methods = {
            init: function ($caller, methodName, func) {

                if (func) {
                    return func(this);
                }               
                // init autoComplete
                $caller.autoComplete($caller.data(dataKey), methodName);

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
    
    /* typeSpy */
    var typeSpy = function () {

        var dataKey = "typeSpy",
            dataIdKey = dataKey + "Id";

        var defaults = {
            interval: 1000, // interval in milliseconds to wait between typing before fireing onChange event
            onChange: null, // triggers after interval when no key up on caller
            onKeyUp: null // triggers on every key up event within caller
        };

        var methods = {
            timer: null,
            init: function ($caller) {
                this.bind($caller);
            },
            bind: function ($caller) {

                $caller.bind('keyup', function (e) {

                    if ((e.keyCode && e.keyCode === 13)) {
                        e.preventDefault();
                        methods.stopTimer();
                        if ($caller.data(dataKey).onComplete) {
                            $caller.data(dataKey).onComplete($(this), e);
                        }
                    }

                    if ($(this).val() !== "") {
                        methods.startTimer($(this), e);
                    } else {
                        if ($caller.data(dataKey).onChange) {
                            $caller.data(dataKey).onChange($(this), e);
                        }
                        methods.stopTimer();
                    }

                    if ($caller.data(dataKey).onKeyUp) {
                        $caller.data(dataKey).onKeyUp($(this), e);
                    }

                });

            },
            unbind: function($caller) {
                $caller.unbind('keyup');
            },
            startTimer: function ($caller, e) {
                this.stopTimer();
                this.timer = setTimeout(function () {
                    if ($caller.data(dataKey).onChange) {
                        $caller.data(dataKey).onChange($caller, e);
                    }
                }, $caller.data(dataKey).interval);
            },
            stopTimer: function () {
                win.clearTimeout(this.timer);
                this.timer = null;
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
                    // $(selector).typeSpy()
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
                    // $().typeSpy()
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
    
    /* tagIt */
    var tagIt = function () {

        var dataKey = "tagIt",
            dataIdKey = dataKey + "Id";

        var defaults = {
            items: [], // our array of items
            ensureUnique: true, // boolean to restrict duplicate items
            maxItems: 0, // the maximum number of allowed selected items - 0 = no limit
            store: null, // optional selector for dom element which will store the JSON representing selected items
            itemTemplate:
                '<li class="tagit-list-item">{text} <a href="#" class="tagit-list-item-delete"><i class="fal fa-times"></i></a></li>',
            parseItemTemplate: function(html, data) {
                if (data.text) {
                    html = html.replace(/\{text}/g, data.text);
                }
                if (data.value) {
                    html = html.replace(/\{value}/g, data.value);
                }
                return html;
            }, // provides a method to parse our itemTemplate with data returned from service url
            onAddItem: function ($caller, result, e) {
                var items = $caller.data(dataKey).items;
                items.push({
                    text: result.text,
                    value: result.value
                });
            }
        };

        var methods = {
            init: function($caller, methodName, func) {

                if (func) {
                    return func(this);
                }
                if (methodName) {
                    if (this[methodName] != null) {
                        this[methodName].apply(this, [$caller]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return null;
                }

                // Bind events
                this.bind($caller);
                
                // Initialize state from store JSON
                var $store = this.getStore($caller);
                if ($store) {
                    if ($store.val() !== "") {
                        this.setItems($caller, $.parseJSON($store.val()));
                        this.update($caller);
                    }
                }

                return null;


            },
            bind: function($caller) {

                var $input = this.getInput($caller);
                if ($input) {
                    
                    $caller.bind("click",
                        function (e) {
                            $input.focus();
                        });
                    
                    var onFocus = function() {
                        $caller.addClass("form-control-active");
                        if ($(this).val() !== "") {
                            $(this).autoComplete("show");
                        }
                    };

                    var onBlur = function() {
                        $caller.removeClass("form-control-active");
                    };

                    $input.bind("focus", onFocus);
                    $input.bind("blur", onBlur);
                    
                    // set-up auto complete on tagIt input
                    $input.autoComplete({
                        onItemClick: function ($caller, result, e) {

                            // Prevent defaults
                            e.preventDefault();

                            // Check uniqueness
                            if (methods.isUnique($caller, result)) {

                                // Raise overrideable event
                                if ($caller.data(dataKey).onAddItem) {
                                    $caller.data(dataKey).onAddItem($caller, result, e);
                                }

                                // Update event may change state
                                methods.update($caller);

                            }

                            // Focus input after selection
                            methods.focus($caller);
                            methods.show($caller);

                        }
                    });

                }
                
            },
            unbind: function($caller) {
                var $input = this.getInput($caller);
                if ($input) {
                    $input.unbind("keyup");
                    $input.unbind("focus");
                    $input.unbind("blur");
                    $input.autoComplete('unbind');
                }
            },
            show: function($caller) {
                var $input = this.getInput($caller);
                if ($input) {
                    $input.autoComplete("show");
                }

            },
            hide: function ($caller) {
                var $input = this.getInput($caller);
                if ($input) {
                    $input.autoComplete("hide");
                }

            },
            update: function ($caller) {

                if ($caller.data(dataKey).onBeforeUpdate) {
                    $caller.data(dataKey).onBeforeUpdate();
                }

                this.removeItems($caller);

                var items = this.getItems($caller);
                if (items) {
                    for (var i = items.length - 1; i >= 0; i--) {
                        var $li = this.buildItem($caller, items[i], i);
                        $caller.find("li:eq(0)").before($li);
                    }
                    var $input = this.getInput($caller),
                        maxAllowed = this.getMaxItems($caller);
                    if ((maxAllowed > 0) && (items.length >= maxAllowed)) {
                        if ($input) {
                            $input.hide();
                        }

                    } else {
                        if ($input) {
                            $input.show();
                        }
                    }
                }
                this.fixUI($caller);
                this.serialize($caller);
            },
            fixUI: function ($caller) {
                var $lis = $caller.find(".tagit-list-item"),
                    $input = this.getInput($caller),
                    $inputLi = this.getInputLi($caller);
                if ($lis.length > 0) {
                    if ($input) {
                        var placeHolder = $input.attr("placeholder");
                        $input.data("tagitPlaceHolder", placeHolder);
                        $input.attr("placeholder", "");
                    }
                    if ($inputLi) {
                        $inputLi.css({ "min-width": "30%", "width": "auto" });
                    }
                } else {
                    if ($input) {
                        if ($input.data("tagitPlaceHolder")) {
                            $input.attr("placeholder", $input.data("tagitPlaceHolder"));
                        }
                    }
                    if ($inputLi) {
                        $inputLi.css({ "min-width": "initial", "width": "100%" });
                    }
                }
            },
            reset: function($caller) {
                this.setInput($caller, "");
            },
            clear: function($caller) {
                this.setStore($caller, "");
                this.setItems($caller, []);
                this.update($caller);
            },
            focus: function($caller) {
                var $input = this.getInput($caller),
                    items = this.getItems($caller),
                    maxAllowed = this.getMaxItems($caller);
                if ((maxAllowed > 0) && (items.length < maxAllowed)) {
                    if ($input && $input.is(":visible")) {
                        $input.focus();
                    }
                } else {
                    if ($input && $input.is(":visible")) {
                        $input.focus();
                    }
                }
            },
            select: function($caller) {
                var $input = this.getInput($caller);
                if ($input.length > 0) {
                    $input.select();
                }
            },
            buildItem: function($caller, data, index) {
                
                var itemTemplate = this.getItemTemplate($caller);
                if ($caller.data(dataKey) && $caller.data(dataKey).parseItemTemplate) {
                    itemTemplate = $caller.data(dataKey).parseItemTemplate(itemTemplate, data);
                }

                var $item = $(itemTemplate);
                $item.data("tagitItem", data);

                var $del = $item.find(".tagit-list-item-delete");
                if ($del.length > 0) {
                    $del.data("tagitItemIndex", index);
                    $del.click(function(e) {
                        e.preventDefault();
                        var items = methods.getItems($caller);
                        items.splice($(this).data("tagitItemIndex"), 1);
                        methods.update($caller);
                        methods.focus($caller);
                    });
                }

                return $item;

            },
            removeItems: function($caller) {
                $caller.find(".tagit-list-item").remove();
            },
            itemExists: function($caller, item) {
                var items = this.getItems($caller);
                if (items) {
                    for (var i = 0; i < items.length; i++) {
                        if (items[i] === item) {
                            return true;
                        }
                    }
                }
                return false;
            },
            isUnique: function($caller, result) {

                var ensureUnique = $caller.data(dataKey).ensureUnique;
                if (ensureUnique === false) {
                    return true;
                }

                var $input = this.getInput($caller),
                    item = {
                        value: result.value
                    },
                    items = this.getItems($caller),
                    existsAt = -1;
                for (var i = 0; i < items.length; i++) {
                    if (item.value === items[i].value) {
                        existsAt = i;
                    }
                }

                if (existsAt === -1) {
                    return true;
                } else {
                    $caller.data(dataKey).highlightIndex = existsAt;
                    this.highlight($caller);
                }
                return false;

            },
            highlight: function($caller) {
                var index = $caller.data(dataKey).highlightIndex,
                    $li = $caller.find("li:eq(" + index + ")");
                if ($li.length > 0) {
                    $li.addClass("bg-warning");
                    window.setTimeout(function() {
                            $li.removeClass("bg-warning");
                        },
                        1000);
                }
            },
            getInputLi: function($caller) {
                var $li = $caller.find(".tagit-list-item-input");
                if ($li.length > 0) {
                    return $li;
                }
                return null;
            },
            getMaxItems: function($caller) {
                return $caller.data("tagitMaxItems") || $caller.data(dataKey).maxItems;
            },
            getInput: function($caller) {
                var $li = this.getInputLi($caller);
                if ($li) {
                    var $input = $li.find("input");
                    if ($input.length > 0) {
                        return $input;
                    }
                }
                return null;
            },
            getItemTemplate: function($caller) {
                return $caller.data("tagitItemTemplate") || $caller.data(dataKey).itemTemplate;
            },
            setInput: function($caller, value) {
                var $input = $caller.find("input");
                if ($input) {
                    $input.val(value);
                }
            },
            getItems: function($caller) {
                return $caller.data("tagitItems") || $caller.data(dataKey).items;
            },
            setItems: function($caller, value) {
                $caller.data("tagitItems", value)
                $caller.data(dataKey).items = value;
            },
            getStore: function($caller) {
                var selector = $caller.data("tagitStore") ||
                    $caller.data(dataKey).store;
                if (selector) {
                    var $input = $(selector);
                    if ($input.length > 0) {
                        return $input;
                    }
                }
                return null;
            },
            setStore: function($caller, value) {
                var $store = this.getStore($caller);
                if ($store != null) {
                    $store.val(value);
                }
            },
            serialize: function($caller) {
                var $store = this.getStore($caller);
                if ($store) {
                    var items = this.getItems($caller);
                    if (items != null) {
                        if (items.length > 0) {
                            $store.val(JSON.stringify(items));
                        } else {
                            $store.val("");
                        }
                        $store.trigger("change");
                    }
                }
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
                    // $(selector).tagIt()
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
                    // $().tagIt()
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
    
    /* userTagIt */
    var userTagIt = function(options) {

        var dataKey = "userTagIt",
            dataIdKey = dataKey + "Id";

        var defaults = {}

        var methods = {
            init: function($caller, methodName, func) {

                if (func) {
                    return func(this);
                }
                if (methodName) {
                    if (this[methodName] != null) {
                        this[methodName].apply(this, [$caller]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return null;
                }

                this.bind($caller);

                return null;
                
            },
            bind: function($caller) {
                
                // init tagIt
                $caller.tagIt($.extend({
                        itemTemplate:
                            '<li class="tagit-list-item"><span class="avatar avatar-sm mr-2"><span style="background-image: url(/users/photo/{id});"></span></span>{displayName} <a href="#" class="tagit-list-item-delete"><i class="fal fa-times"></i></a></li>',
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
                            return html;

                            return html;
                        },
                        onAddItem: function($input, result, e) {
                            $input.val("");
                        }
                    },
                    defaults,
                    options));
                
                // user auto complete
                methods.getInput($caller).userAutoComplete($.extend({
                        onItemClick: function($input, result, e) {

                            e.preventDefault();

                            // ensure we only add uunque entries
                            var index = methods.getIndex($caller, result);

                            if (index === -1) {
                                var tagit = $caller.data("tagIt");
                                tagit.items.push(result);
                                $caller.tagIt("update");
                                $caller.tagIt("focus");
                                $caller.tagIt("select");
                                $caller.tagIt("show");
                            } else {
                                $caller.tagIt({
                                        highlightIndex: index
                                    },
                                    "highlight");
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
                    if (item.id === items[i].id) {
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
                    // $(selector).userTagIt()
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
                    // $().userTagIt()
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

        var defaults = {}

        var methods = {
            init: function ($caller, methodName, func) {

                if (func) {
                    return func(this);
                }
                if (methodName) {
                    if (this[methodName] != null) {
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
                        parseItemTemplate: function(html, result) {

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

                            return html;
                        },
                        onAddItem: function($input, result, e) {
                            $input.val("");
                        }
                    },
                    defaults,
                    options));

                // init auto complete

                methods.getInput($caller).labelAutoComplete($.extend({
                        onItemClick: function($input, result, e) {

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
    
    /* selectDropdown */
    var selectDropdown = function () {

        var dataKey = "selectDropdown",
            dataIdKey = dataKey + "Id";

        var defaults = {
            event: "click",
            items: [], // our array of selected items
            highlightIndex: 0,
            ensureUnique: true, // boolean to restrict duplicate items
            maxItems: 0, // the maximum number of allowed selected items - 0 = no limit
            store: null, // optional selector for dom element which will store the JSON representing selected items
            itemTemplate: '<li class="list-group-item select-dropdown-item">{text} <a href="#" class="tagit-list-item-delete"><i class="fal fa-times"></i></a></li>',
            itemTemplateEmpty: '<li class="list-group-item">No Results</li>',
            parseItemTemplate: function (html, data) {
                if (data.text) {
                    html = html.replace(/\{text}/g, data.text);
                }
                if (data.value) {
                    html = html.replace(/\{value}/g, data.value);
                }
                return html;
            }, // provides a method to parse our itemTemplate with data returned from service url
            onAddItem: function ($caller, result, e) {
                var items = $caller.data(dataKey).items;
                items.push({
                    text: result.text,
                    value: result.value
                });
            },
            onShow: function ($caller, $dropdown) { }, // triggers when the dropdown is shown
            onChange: function($caller, $input, e) { } // triggers when a checkbox or radio button is changed within the dropdown
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

                // Bind events
                methods.bind($caller);

                // Initialize state from store JSON
                var $store = this.getStore($caller);
                if ($store) {
                    if ($store.val() !== "") {
                        this.setItems($caller, $.parseJSON($store.val()));
                        this.update($caller);
                    }
                }

            },
            bind: function ($caller) {

                var $dropdown = this.getDropdownMenu($caller),
                    $input = this.getDropdownSearchInput($caller);

                // On dropdown shown focus search
                $caller.on('shown.bs.dropdown',
                    function () {
                        $input.focus();
                        if ($caller.data(dataKey).onShow) {
                            $caller.data(dataKey).onShow($caller, $dropdown);
                        }
                        
                    });

                // On checkbox or radiobutton change within dropdown 
                $dropdown.on('change',
                    'input[type="radio"], input[type="checkbox"]',
                    function (e) {
                        if ($caller.data(dataKey).onChange) {
                            $caller.data(dataKey).onChange($caller, $(this), e);
                        }
                    });


            },
            update: function ($caller) {

                var $preview = this.getDropdownPreview($caller),
                    $items = this.getDropdownListGroupItems($caller),
                    $dropdown = this.getDropdownMenu($caller);

               

                var items = this.getItems($caller);
            
                if (items && items.length > 0) {
                  
                    $preview.empty();
                    for (var i = 0; i < items.length; i++) {

                        // // Set active items within dropdown
                        //var checkId = items[i].id || items[i].attr("for"),
                        //    $ckb = $dropdown.find("#" + checkId),
                        //    $lbl = $dropdown.find('[for="' + checkId + '"]');
                      
                        //if ($ckb.length > 0) {
                        //    $ckb.prop("checked", "true");
                        //}
                        //if ($lbl.length > 0) {
                        //    $lbl.addClass("active");
                        //}
                        
                        // Build preview item and append
                        $preview.append(this.buildItem($caller, items[i], i));

                    }
                } else {
                    $preview
                        .empty()
                        .append($($caller.data(dataKey).itemTemplateEmpty));
                }

                this.serialize($caller);

            },
            buildItem: function ($caller, data, index) {

                var itemTemplate = this.getItemTemplate($caller);
                if ($caller.data(dataKey) && $caller.data(dataKey).parseItemTemplate) {
                    itemTemplate = $caller.data(dataKey).parseItemTemplate(itemTemplate, data);
                }

                var $item = $(itemTemplate);
                $item.data("tagitItem", data);

                var $del = $item.find(".select-dropdown-delete");
                if ($del.length > 0) {
                    $del.data("selectDropdownItemIndex", index);
                    $del.click(function (e) {
                        e.preventDefault();
                        var items = methods.getItems($caller);
                        items.splice($(this).data("selectDropdownItemIndex"), 1);
                        methods.update($caller);
                    });
                }

                return $item;

            },
            getItemTemplate: function ($caller) {
                return $caller.data("selectDropdownItemTemplate") || $caller.data(dataKey).itemTemplate;
            },
            highlight: function ($caller) {
                var index = $caller.data(dataKey).highlightIndex,
                    $li = $caller.find("li:eq(" + index + ")");
                if ($li.length > 0) {
                    $li.addClass("bg-warning");
                    window.setTimeout(function () {
                        $li.removeClass("bg-warning");
                    },
                        1000);
                }
            },
            clear: function ($caller) {
                this.setStore($caller, "");
                this.setItems($caller, []);
                this.update($caller);
            },
            getItems: function ($caller) {
                return $caller.data("selectDropDownItems") || $caller.data(dataKey).items;
            },
            setItems: function ($caller, value) {
                $caller.data("tagitItems", value)
                $caller.data(dataKey).items = value;
            },
            getStore: function ($caller) {
                var selector = $caller.data("selectDropdownStore") ||
                    $caller.data(dataKey).store;
                if (selector) {
                    var $input = $(selector);
                    if ($input.length > 0) {
                        return $input;
                    }
                }
                return null;
            },
            setStore: function ($caller, value) {
                var $store = this.getStore($caller);
                if ($store != null) {
                    $store.val(value);
                }
            },
            serialize: function ($caller) {
                var $store = this.getStore($caller);
                if ($store) {
                    var items = this.getItems($caller);
                    if (items != null) {
                        if (items.length > 0) {
                            $store.val(JSON.stringify(items));
                        } else {
                            $store.val("");
                        }
                        $store.trigger("change");
                    }
                }
            },
            filterItems: function ($caller) {

                var $items = this.getDropdownListGroupItems($caller),
                    $input = this.getDropdownSearchInput($caller),
                    $noResults = this.getDropdownSearchNoResults($caller),
                    word = $input.val().trim(),
                    length = $items.length,
                    hidden = 0;

                if (word.length === 0) {
                    $items.show();
                    $caller.treeView("collapseAll");
                }

                var i = 0, $label = null;

                // first ensure all matches are visible
                for (i = 0; i < length; i++) {
                    $label = $($items[i]);
                    if ($label.length > 0 && $label.data("filterValue")) {
                        if ($label.data("filterValue").toLowerCase().startsWith(word)) {
                            $label.parent(".list-group").show();
                            $label.show();
                        }
                    }
                }

                // Next hide all others if children are not visible
                for (i = 0; i < length; i++) {
                    $label = $($items[i]);
                    if ($label.length > 0 && $label.data("filterValue")) {
                        if (!$label.data("filterValue").toLowerCase().startsWith(word)) {
                            if (!$label.find(".list-group").is(":visible")) {
                                $label.hide();
                                hidden++;
                            }
                        }
                    }
                }

                //If all items are hidden, show the empty view
                if (hidden === length) {
                    $noResults.show();
                } else {
                    $noResults.hide();
                }

            },
            getDropdownButton: function ($caller) {
                return $caller.find('[data-toggle="dropdown"]');
            },
            getDropdownMenu: function ($caller) {
                return $caller.find(".dropdown-menu");
            },
            getDropdownListGroupItems: function ($caller) {
                var $menu = this.getDropdownMenu($caller);
                return $menu.find(".list-group-item");
            },
            getDropdownSearchInput: function ($caller) {
                var $menu = this.getDropdownMenu($caller);
                return $menu.find('[type="search"]');
            },
            getDropdownSearchNoResults: function ($caller) {
                return $caller.find(".empty");
            },
            getDropdownPreview: function ($caller) {

                var $preview = $caller.find(".select-dropdown-preview");
                if ($preview.length === 0) {
                    $preview = $caller.next();
                    if (!$preview.hasClass("select-dropdown-preview")) {
                        throw new Error("A preview area coulod not be found for the select dropdown.");
                    }
                }
                return $preview;
            },
            getDropdownPreviewText: function ($caller) {
                var $preview = this.getDropdownPreview($caller);
                if ($preview.length > 0) {
                    return $preview.attr("data-empty-preview-text");
                }
                return "No selection";
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
                    // $(selector).markdownEditor
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
                    // $().markdownEditor 
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

    /* labelSelectDropdown */
    var labelSelectDropdown = function (options) {

        var dataKey = "labelSelectDropdown",
            dataIdKey = dataKey + "Id";

        var defaults = {}

        var methods = {
            init: function ($caller, methodName, func) {

                if (func) {
                    return func(this);
                }
                if (methodName) {
                    if (this[methodName] != null) {
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

                // init selectDropdown
                $caller.selectDropdown($.extend({
                        itemTemplate:
                            '<li class="list-group-item select-dropdown-item"><span class="btn btn-sm label font-weight-bold" style="background-color: {backColor}; color: {foreColor};">{name}</span><a href="#" class="btn btn-secondary float-right select-dropdown-delete" data-toggle="tooltip" title="Delete"><i class="fal fa-times"></i></a></li>',
                        parseItemTemplate: function(html, result) {

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

                            return html;
                        },
                        onAddItem: function($input, result, e) {
                            $input.val("");
                        },
                        onShow: function($sender, $dropdown) {

                            var $preview = $sender.find(".select-dropdown-preview"),
                                $input = $dropdown.find('[type="search"]');
                            //if ($input.length === 0) { return; }

                            // Show & update auto complete when dropdown is shown
                            $input.labelAutoComplete("show")
                                .labelAutoComplete("update");

                        }
                    },
                    defaults,
                    options));

                // init auto complete
                methods.getInput($caller).labelAutoComplete($.extend({
                        itemTemplate:
                            '<input type="checkbox" value="{id}" id="label-{id}"/><label for="label-{id}" class="{itemCss}"><i class="fal mr-2 check-icon"></i><span class="btn btn-sm label font-weight-bold" style="background-color: {backColor}; color: {foreColor}">{name}</span><span title="Occurrences" data-toggle="tooltip" class="float-right btn btn-sm btn-secondary">{totalEntities.text}</span></label>',
                        onItemClick: function($input, result, e) {

                            e.preventDefault();
                            e.stopPropagation();

                            // ensure we only add uunque entries
                            var index = methods.getIndex($caller, result);
                            if (index === -1) {
                                var selectDropdown = $caller.data("selectDropdown");
                                selectDropdown.items.push(result);
                                $caller.selectDropdown("update");
                            } else {
                                $caller.selectDropdown({
                                        highlightIndex: index
                                    },
                                    "highlight");
                            }
                        },
                        onLoaded: function($input, $dropdown) {
                            $caller.selectDropdown("update");
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

    /* categorySelectDropdown */
    var categorySelectDropdown = function (options) {

        var dataKey = "categorySelectDropdown",
            dataIdKey = dataKey + "Id";

        var defaults = {}

        var methods = {
            init: function ($caller, methodName, func) {

                if (func) {
                    return func(this);
                }
                if (methodName) {
                    if (this[methodName] != null) {
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

                //var $dropdown = $caller.find(".dropdown-menu");
               
                // init selectDropdown
                $caller.selectDropdown($.extend({
                        itemTemplate:
                            '<li class="list-group-item"><span class="btn btn-sm label font-weight-bold" style="background-color: {backColor}; color: {foreColor};">{name}</span><a href="#" class="btn btn-secondary float-right select-dropdown-delete" data-toggle="tooltip" title="Delete"><i class="fal fa-times"></i></a></li>',
                        parseItemTemplate: function(html, $label) {
                            var $div = $('<div class="list-group-item">');
                            $div.append($label.clone());
                            return $div;
                        },
                        onAddItem: function($input, result, e) {
                            $input.val("");
                        },
                        onShow: function($sender, $dropdown) {

                            $dropdown.find('[data-provide="tree"]').treeView("expandSelected");

                        },
                        onChange: function($dropdown, $input, e) {
                            
                            e.preventDefault();
                            e.stopPropagation();

                            // Get all checked labels within the dropdown

                       
                            $dropdown.find('input:checked').each(function() {

                                var checkId = $(this).attr("id");
                                var $label = $dropdown.find('[for="' + checkId + '"]');
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
                                            "highlight");}

                                }
                            });

                         


                        }
                    },
                    defaults,
                    options));

                // init tree
                $caller.find('[data-provide="tree"]').treeView($.extend({
                    onClick: function ($tree, $link, e) {

                  
                                 // Toggle checkbox when we click a tree node item
                                var $inputs = $link.find("input").first();
                                $inputs.each(function(i) {
                                    if ($(this).is(":checked")) {
                                        $(this).prop("checked", false);
                                    } else {
                                        $(this).prop("checked", true);
                                    }
                                    $(this).trigger("change");
                                });
                            }
                        },
                        defaults,
                        options),
                    "expandSelected");


            },
            getIndex: function ($caller, $label) {

                // Has the label been added to our items array
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
                    // $(selector).categorySelectDropdown()
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
                    // $().categorySelectDropdown()
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


    /* Register Plugins */
    $.fn.extend({
        scrollTo: scrollTo.init,
        treeView: treeView.init,
        autoComplete: autoComplete.init,
        typeSpy: typeSpy.init,
        tagIt: tagIt.init,
        userAutoComplete: userAutoComplete.init,
        labelAutoComplete: labelAutoComplete.init,
        userTagIt: userTagIt.init,
        labelTagIt: labelTagIt.init,
        selectDropdown: selectDropdown.init,
        labelSelectDropdown: labelSelectDropdown.init,
        categorySelectDropdown: categorySelectDropdown.init
    });

    // ---------------------------
    // Initialize plug-ins
    // ----------------------------

    $.fn.platoUI = function (opts) {

        /* Scolls to a specific element. Typical usage...
         * <a href="#somelement" data-provide="scroll"> */
        this.find('[data-provide="scroll"]').scrollTo();

        /* select dropdown */
        this.find('[data-provide="select-dropdown"]').selectDropdown();

        /* label select dropdown */
        this.find('[data-provide="label-select-dropdown"]').labelSelectDropdown();

        /* category select dropdown */
        this.find('[data-provide="category-select-dropdown"]').categorySelectDropdown();

        /* treeView */
        this.find('[data-provide="tree"]').treeView();

        /* autoComplete */
        this.find('[data-provide="autoComplete"]').autoComplete();

        /* userAutoComplete */
        this.find('[data-provide="userAutoComplete"]').userAutoComplete();

        /* labelAutoComplete */
        this.find('[data-provide="labelAutoComplete"]').labelAutoComplete();
        
        /* tagIt */
        this.find('[data-provide="tagIt"]').tagIt();
        
        /* userTagIt */
        this.find('[data-provide="userTagIt"]').userTagIt();

        /* labelTagIt */
        this.find('[data-provide="labelTagIt"]').labelTagIt();

    }

    $(doc).ready(function() {
        $("body").platoUI();
    });


}(window, document, jQuery));