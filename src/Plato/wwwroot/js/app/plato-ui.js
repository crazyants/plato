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
                    if (this[methodName] !== null && typeof this[methodName] !== "undefined") {
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
            onClick: null, // triggers when the linkSelector is clicked
            onToggle: function ($tree, $toggler, e) {
                // Prevent onClick raising when we toggle a node
                e.preventDefault();
                e.stopPropagation();
                return true;
            }, // triggers when the toggleSelector is clicked
            onExpand: null, // triggers when a node is expanded
            onCollapse: null // triggers when a node is collapsed
        };

        var methods = {
            init: function ($caller, methodName) {

                if (methodName) {
                    if (this[methodName] !== null && typeof this[methodName] !== "undefined") {
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
                        var toggle = true;
                        if ($caller.data(dataKey).onToggle) {
                            toggle = $caller.data(dataKey).onToggle($caller, $(this), e);
                        }
                        if (toggle) {
                            methods._toggleNode($caller, $(this).attr("data-node-id"), e);
                        }
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
            expandAll: function ($caller) {
                $caller.find(".list-group-item").each(function () {
                    methods._expand($caller, $(this).attr("id"), false, win.event);
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
            _toggleNode: function ($caller, nodeId, e) {
                var $item = methods.getNodeListItem($caller, nodeId);
                if ($item.hasClass("show")) {
                    methods._collapse($caller, nodeId, true, e);
                } else {
                    methods._expand($caller, nodeId, true, e);
                }
            },
            _expand: function ($caller, nodeId, slide, e) {
                var $li = methods.getNodeListItem($caller, nodeId),
                    $child = $li.find("ul").first();
                $li.addClass("show");
                if (slide) {
                    $child.slideDown("fast");
                } else {
                    $child.show();
                }
                if ($caller.data(dataKey).onExpand) {
                    $caller.data(dataKey).onExpand($caller, $child, e);
                }
            },
            _expandParents: function ($caller, nodeId) {
                var $li = methods.getNodeListItem($caller, nodeId);
                $li.parents(".list-group-item").each(function () {
                    methods._expand($caller, $(this).attr("id"), false);
                });
            },
            _collapse: function ($caller, nodeId, slide, e) {
                var $li = methods.getNodeListItem($caller, nodeId),
                    $child = $li.find("ul").first();
                $li.removeClass("show");
                if (slide) {
                    $child.slideUp("fast");
                } else {
                    $child.hide();
                }
                if ($caller.data(dataKey).onCollapse) {
                    $caller.data(dataKey).onCollapse($caller, $child, e);
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

    /* pagedList */
    var pagedList = function () {

        var dataKey = "pagedList",
            dataIdKey = dataKey + "Id";
        
        var defaults = {
            page: 1,
            pageSize: 10,
            config: {}, // optional configuration options for ajax request
            target: null, // optional target selector for auto complete results. if no target a dropdown-menu is used
            enablePaging: true, // indicates if paging should be enabled for results
            onLoad: function ($caller, results) {

                if (results) {

                    $caller.empty();

                    // build results
                    for (var i = 0; i < results.data.length; i++) {
                        $caller.append(this.buildItem($caller, results.data[i]));
                    }
                    // build paging
                    var enablePaging = $caller.data(dataKey).enablePaging;
                    if (enablePaging) {
                        if (results.total > results.size) {
                            $caller.append(this.buildPager($caller, results));
                        }
                    }
                } else {
                    // no data
                    $caller.empty().append(this.buildNoResults($caller));
                }

                $caller.pagedList("setItemIndex");

                if ($caller.data(dataKey).onLoaded) {
                    $caller.data(dataKey).onLoaded($caller, results);
                }
                
            }, // triggers after paged results have finished loading
            onLoaded: null, // triggers after paged results have been added to the dom
            onItemClick: null, // event raised when you click a paged result item
            onPagerClick: function ($caller, page, e) {
                e.preventDefault();
                e.stopPropagation();
                $caller.pagedList({
                    page: page
                });
            }, // event raised when you click next / previous
            buildItem: function ($caller, result) {

                // apply default css
                var itemTemplate = $caller.data(dataKey).itemTemplate,
                    itemCss = $caller.data("pagedListItemCss") || $caller.data(dataKey).itemCss;
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
            buildPager: function ($caller, results) {

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

                var icon = $("<i>").addClass("fa fa-chevron-left"),
                    $a = $("<a>")
                    .attr("href", "#")
                    .addClass("list-group-item list-group-item-action float-left prev-page col-3 text-center")
                    .append(icon);

                $a.click(function (e) {
                    if ($caller.data(dataKey).onPagerClick) {
                        $caller.data(dataKey).onPagerClick($caller, page - 1, e);
                    }
                });

                return $a;

            },
            buildNext: function ($caller, page) {

                var icon = $("<i>").addClass("fa fa-chevron-right"),
                    $a = $("<a>")
                    .attr("href", "#")
                    .addClass("list-group-item list-group-item-action float-left next-page col-3 text-center")
                    .append(icon);

                $a.click(function (e) {
                    if ($caller.data(dataKey).onPagerClick) {
                        $caller.data(dataKey).onPagerClick($caller, page + 1, e);
                    }
                });

                return $a;

            },
            buildNoResults: function ($caller) {

                var noResultsText = $caller.data(dataKey).noResultsText,
                    noResultsIcon = $caller.data(dataKey).noResultsIcon;

                // Apply localizaer
                if (context.localizer) {
                    noResultsText = context.localizer.get(noResultsText);
                }
                
                var $div = $("<div>")
                    .addClass("text-center p-4");

                if (noResultsIcon) {
                    var $icon = $("<i>")
                        .addClass(noResultsIcon);
                    $div.append($icon);
                }

                $div.append(noResultsText);

                var li = $('<li class="no-results">');
                li.append($div);

                return li;

            },
            buildInfo: function ($caller, results) {

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
            itemTemplate: '<a class="{itemCss}" href="{url}"><span style="white-space: nowrap;overflow: hidden;text-overflow: ellipsis;max-width: 85%;">{text}</span> <span style="opacity: .7;">@{value}</span><span class="float-right">{rank}</span></a>',
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
            loaderTemplate: '<p class="text-center"><i class="fal fa-spinner fa-spin" ></i></p >', // a handlebars style template for auto complete list items
            noResultsText: "Sorry no results matched your search!", // the text to display when no results are available
            noResultsIcon: null, // optional icon to display above noResultsText
            itemSelection: {
                enable: true,
                index: 0,
                css: "active"
            }
        };

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
                
                // bind events
                this.bind($caller);

            },
            bind: function($caller) {
                
                // Add loader
                var loaderTemplate = $caller.data(dataKey).loaderTemplate;
                if (loaderTemplate) {
                    $caller.empty().append(loaderTemplate);
                }
                
                // Begin populate
                var config = this.getConfig($caller);
                win.$.Plato.Http(config).done(function (response) {
                    if (response.statusCode !== 200) {
                        return;
                    }
                    if ($caller.data(dataKey).onLoad) {
                        $caller.data(dataKey).onLoad($caller, response.result);
                    }
                });

            },
            getConfig: function ($caller) {
                
                var config = $.extend({}, $caller.data(dataKey).config),
                    url = $caller.data("pagedListUrl") || config.url,
                    pageIndex = this.getPageIndex($caller) || 1,
                    pageSize = this.getPageSize($caller) || 10;
                
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
                
                if (url) {

                    if (url.indexOf("{page}") >= 0) {
                        url = url.replace(/\{page}/g, pageIndex);
                    }
                    if (url.indexOf("{pageSize}") >= 0) {
                        url = url.replace(/\{pageSize}/g, pageSize);
                    }
                    
                    config.url = url;
                }

                return config;

            },
            setItemIndex: function($caller) {
                var selection = $caller.data(dataKey).itemSelection,
                    itemCss = $caller.data(dataKey).itemCss;
            
                if (selection) {
                    if (selection.enable === false) {
                        return;
                    }
                    var index = selection.index,
                        selector = "a." + itemCss,
                        tag = selection.tag,
                        css = selection.css,
                        $el = $caller.find(selector + ":eq(" + index + ")");
                    $caller.find(selector).each(function () {
                        $(this).removeClass(css);
                    });
                    if ($el.length > 0) {
                        $el.addClass(css);
                    }
                }
            },
            setPageIndex: function ($caller, pageIndex) {
                $caller.data(dataKey).page = pageIndex;
            },
            getPageIndex: function ($caller) {
                return $caller.data("pagedListPageIndex") || $caller.data(dataKey).page;
            },
            setPageSize: function ($caller, pageSize) {
                $caller.data(dataKey).pageSize = pageSize;
            },
            getPageSize: function ($caller) {
                return $caller.data("pagedListPageSize") || $caller.data(dataKey).pageSize;
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
                    // $(selector).pagedList()
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
                    // $().pagedList()
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
            target: null, // optional target selector for auto complete results. if no target a dropdown-menu is used
            onShow: null, // triggers when the autocomplete target is displayed
            onHide: null, // triggers when the autocomplete target is hidden
            onKeyDown: null, // triggers for key down events within the autocomplete input element
            valueField: null // the name of the querystring or post parameter representing the keywords for the request
        };

        var methods = {
            init: function ($caller, methodName) {

                if (methodName) {
                    if (this[methodName] !== null && typeof this[methodName] !== "undefined") {
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
                
                $caller.bind("keydown.",
                    function (e) {
                        var $target = methods.getTarget($(this));
                        if ($target) {
                            if ($target.is(":visible")) {

                                var itemCss = $target.data("pagedList").itemCss,
                                    pageSize = $target.find("." + itemCss).length,
                                    itemSelection = $target.data("pagedList").itemSelection,
                                    newIndex = -1;

                                if (itemSelection.enable) {
                                    switch (e.which) {
                                        case 13: // carriage return
                                            e.preventDefault();
                                            e.stopPropagation();
                                            // find active and click
                                            $target.find("." + itemCss).each(function () {
                                                if ($(this).hasClass(itemSelection.css)) {
                                                    $(this).click();
                                                }
                                            });
                                            break;
                                        case 38: // up
                                            e.preventDefault();
                                            e.stopPropagation();
                                            newIndex = itemSelection.index - 1;
                                            if (newIndex < 0) {
                                                newIndex = 0;
                                            }
                                            break;
                                        case 40: // down
                                            e.preventDefault();
                                            e.stopPropagation();
                                            newIndex = itemSelection.index + 1;
                                            if (newIndex > (pageSize - 1)) {
                                                newIndex = (pageSize - 1);
                                            }
                                            break;
                                    }
                                    if (newIndex >= 0) {
                                        $target.pagedList({
                                            itemSelection: $.extend(itemSelection,
                                                {
                                                    index: newIndex
                                                })
                                        }, "setItemIndex");
                                    }

                                }


                            }
                        }
                      
                    });
                
                // spy on our input
                $caller.typeSpy({
                    onKeyUp: function ($el, e) {
                        if (e.keyCode === 27) {
                            // escape
                            methods.hide($el);
                        }
                    },
                    onChange: function ($el, e) {
                      
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
                
                // Clone config & get keywords field name
                var config = $.extend({}, $caller.data(dataKey).config),
                    valueField = $caller.data(dataKey).valueField;

                if (valueField) {

                    // For get requests replace keywords in URL with auto complete value
                    if (config.method.toUpperCase() === "GET") {
                        config.url = config.url.replace("{" + valueField + "}", encodeURIComponent($caller.val()));
                    }

                    // For post requests add keyword to data object literal
                    if (config.method.toUpperCase() === "POST") {
                        config.data[valueField] = $caller.val();
                    }

                }
             
                // Init pagedList
                $target.pagedList($.extend({}, $caller.data(dataKey),
                        {
                            page: 1,
                            config: config
                        }));
                
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
            itemTemplate: '<a class="{itemCss}" href="{url}"><span class="avatar avatar-sm mr-2"><span style="background-image: url(/users/photo/{id});"></span></span>{displayName}<span class="float-right">@{userName}</span></a>',
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
            itemTemplate: '<a class="{itemCss}" href="{url}"><span class="btn btn-sm label font-weight-bold" style="background-color: {backColor}; color: {foreColor}">{name}</span><span title="Occurrences" data-toggle="tooltip" class="float-right btn btn-sm btn-secondary">{totalEntities.text}</span></a>',
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
            event: "keyup", // default event to spy
            interval: 500, // interval in milliseconds to wait between typing before fireing onChange event
            onChange: null, // triggers after interval when no key up on caller
            onKeyUp: null // triggers on every key up event within caller
        };

        var methods = {
            timer: null,
            init: function ($caller) {
                this.bind($caller);
            },
            bind: function ($caller) {

                var event = $caller.data(dataKey).event;
                if (event === null) {
                    return;
                }

                $caller.bind(event, function (e) {

                    if (e.which) {

                        switch (e.which) {
                            case 13: // carriage return
                                e.preventDefault();
                                methods.stopTimer();
                                if ($caller.data(dataKey).onComplete) {
                                    $caller.data(dataKey).onComplete($(this), e);
                                }
                            return;
                        case 38: // up
                            return;
                        case 40: // down
                            return;
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

    /* blurSpy */
    var blurSpy = function () {

        var dataKey = "blurSpy",
            dataIdKey = dataKey + "Id";

        var defaults = {
            id: "blurSpy", // uniuqe namespace
            interval: 100, // interval in milliseconds to wait before fireing onBlur event
            onBlur: null // triggers after interval if element does not reeive focus again
        };

        var methods = {
            timer: null,
            init: function ($caller) {
                this.bind($caller);
            },
            bind: function ($caller) {

                var id = $caller.data(dataKey).id,
                    focusEvent = "focus",
                    blurEvent = "blur";

                if (id !== "") {
                    focusEvent = focusEvent + "." + id;
                    blurEvent = blurEvent + "." + id;
                }

                $caller.on(focusEvent,
                    function(e) {
                        methods.stopTimer();
                    });

                $caller.on(blurEvent,
                    function (e) {
                        methods.startTimer($(this), e);
                    });

            },
            unbind: function ($caller) {
                $caller.unbind('blur');
                $caller.unbind('focus');
            },
            startTimer: function ($caller, e) {
                this.stopTimer();
                this.timer = setTimeout(function () {
                    if ($caller.data(dataKey).onBlur) {
                        $caller.data(dataKey).onBlur($caller, e);
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
                    // $(selector).blurSpy()
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
                    // $().blurSpy()
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
    
    /* filterList */
    var filterList = function () {

        var dataKey = "filterList",
            dataIdKey = dataKey + "Id";

        var defaults = {
            target: null, // the list group to filter (string selector or object)
            empty: null // the no filter results element (string selector or object)
        };

        var methods = {
            timer: null,
            init: function ($caller) {
                this.bind($caller);
            },
            bind: function ($caller) {
                $caller.bind('keydown',
                    function (e) {
                        if ((e.keyCode && e.keyCode === 13)) {
                            e.preventDefault();
                        }
                    });
                $caller.bind('keyup', function (e) {
                    methods.filter($(this));
                });
            },
            unbind: function ($caller) {
                $caller.unbind('keydown');
                $caller.unbind('keyup');
            },
            filter: function($caller) {

                var $target = this.getTarget($caller),
                    $items = this.getListItems($caller),
                    $empty = this.getEmpty($caller),
                    word = $caller.val().trim().toLowerCase(),
                    length = $items.length,
                    hidden = 0;
                
                $target.treeView("expandAll");
                
                // First hide all items
                for (var i = 0; i < length; i++) {
                    var $label = $($items[i]);
                    if ($label.length > 0) {
                        $label.removeClass("hidden");
                        if (!this.find($label, word)) {
                            $label.addClass("hidden");
                            hidden++;
                        }
                    }
                }
                
                //If all items are hidden, show the empty element
                if (hidden === length) {
                    $empty.show();
                } else {
                    $empty.hide();
                }

            },
            find: function ($root, word) {

                // Search in supplied list item
                var value = $root.data("filterListValue");
                if (value) {
                    if (value.toLowerCase().indexOf(word) >= 0) {
                        return true;
                    }
                }

                // Search in child list items
                var $labels = $root.find(".list-group-item");
                for (var i = 0; i < $labels.length; i++) {
                    var $label = $($labels[i]);
                    if ($label.length > 0) {
                        value = $label.data("filterListValue");
                        if (value.toLowerCase().indexOf(word) >= 0) {
                            return true;
                        }
                        if ($label.find(".list-group-item").length > 0) {
                            this.find($label, word);
                        }
                    }
                 
                }
                return false;

            },
            getTarget: function ($caller) {
                var target = $caller.data("filterListTarget") || $caller.data(dataKey).target;
                if (typeof target == "string") {
                    return $(target);
                }
                return target;
            },
            getListItems: function($caller) {
                var $list = this.getTarget($caller);
                return $list.find(".list-group-item");
            },
            getEmpty: function ($caller) {
                var target = $caller.data("filterListEmpty") || $caller.data(dataKey).empty;
                if (typeof target == "string") {
                    return $(target);
                }
                return target;
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
                    // $(selector).filterList()
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
                    // $().filterList()
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
                    if (this[methodName] !== null && typeof this[methodName] !== "undefined") {
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

                    $input.bind("focus",
                        function() {
                            $caller.addClass("form-control-active");
                        });

                    $input.bind("blur",
                        function() {
                            $caller.removeClass("form-control-active");
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
            onShow: null, // triggers when the dropdown is shown
            onChange: null, // triggers when a checkbox or radio button is changed within the dropdown
            onUpdated: null // triggers whenever our items array is rendered
        };

        var methods = {
            init: function ($caller, methodName) {

                if (methodName) {
                    if (this[methodName] !== null && typeof this[methodName] !== "undefined") {
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

                var $dropdown = $caller.find(".dropdown-menu");

                // On dropdown shown 
                $caller.on('shown.bs.dropdown',
                    function () {
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
            unbind: function ($caller) {
                $caller.unbind("shown.bs.dropdown");
                $caller.find(".dropdown-menu").unbind("change");
            },
            update: function ($caller) {

                var $preview = this.getPreview($caller),
                    items = this.getItems($caller);

                // Clear preview
                $preview.empty();

                // Populate preview
                if (items && items.length > 0) {
                    for (var i = 0; i < items.length; i++) {
                        // Build preview item and append
                        $preview.append(this.buildItem($caller, items[i], i));
                    }
                } else {
                    $preview
                        .append($($caller.data(dataKey).itemTemplateEmpty));
                }

                // Serialize items to hidden field
                this.serialize($caller);

                // Raise onUpdated event
                if ($caller.data(dataKey).onUpdated) {
                    $caller.data(dataKey).onUpdated($caller);
                }

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
                var $preview = this.getPreview($caller),
                    index = $caller.data(dataKey).highlightIndex,
                    $li = $preview.find("li:eq(" + index + ")");
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
                return $caller.data(dataKey).items;
            },
            setItems: function ($caller, value) {
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
            getPreview: function ($caller) {

                var $preview = $caller.find(".select-dropdown-preview");
                if ($preview.length === 0) {
                    $preview = $caller.next();
                    if (!$preview.hasClass("select-dropdown-preview")) {
                        throw new Error("A preview area coulod not be found for the select dropdown.");
                    }
                }
                return $preview;
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

                // init selectDropdown
                $caller.selectDropdown($.extend({
                        itemTemplate: '<li class="list-group-item select-dropdown-item"><span class="btn btn-sm label font-weight-bold" style="background-color: {backColor}; color: {foreColor};">{name}</span><a href="#" class="btn btn-secondary float-right select-dropdown-delete" data-toggle="tooltip" title="Delete"><i class="fal fa-times"></i></a></li>',
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
                        onShow: function($sender, $dropdown) {
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
                            $dropdown.find("label").each(function() {
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
                        itemTemplate: '<input type="checkbox" value="{id}" id="label-{id}"/><label for="label-{id}" class="{itemCss}"><i class="fal mr-2 check-icon"></i><span class="btn btn-sm label font-weight-bold" style="background-color: {backColor}; color: {foreColor}">{name}</span><span title="Occurrences" data-toggle="tooltip" class="float-right btn btn-sm btn-secondary">{totalEntities.text}</span></label>',
                        onItemClick: function($input, result, e) {

                            e.preventDefault();
                            e.stopPropagation();

                            // ensure we only add uunque entries
                            var index = methods.getIndex($caller, result);
                            if (index === -1) {
                                $caller.data("selectDropdown").items.push(result);
                            } else {
                                $caller.data("selectDropdown").items.splice(index, 1);
                            }
                            $caller.selectDropdown("update");
                        },
                        onLoaded: function($input) {
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

                            // get tree
                            var $tree = $dropdown.find('[data-provide="tree"]');

                            // Focus & set-up search on dropdown shwn
                            var $input = $dropdown.find('[type="search"]');
                            if ($input.length > 0) {
                                $input.focus()
                                    .filterList({
                                        target: $tree
                                    });
                            }

                            // Expand tree view selection on dropdown shown

                            if ($tree.length > 0) {
                                $tree.treeView("expandSelected");
                            }

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
                                            "highlight");
                                    }

                                }
                            });


                        }
                    },
                    defaults,
                    options));

                // init tree
                $caller.find('[data-provide="tree"]').treeView($.extend({
                            onClick: function($tree, $link, e) {

                                e.preventDefault();
                                e.stopPropagation();

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
                            },
                            onToggle: function($tree, $toggler, e) {
                                // Prevent onClick raising when we toggle a node
                                e.preventDefault();
                                e.stopPropagation();
                                return true;
                            }
                        },
                        defaults,
                        options),
                    "expandSelected");


            },
            getIndex: function ($caller, $label) {

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

    /* confirmer */
    var confirmer = function () {

        var dataKey = "confirmer",
            dataIdKey = dataKey + "Id";

        var defaults = {
            event: "click", // uniuqe namespace
            message: "Are you sure you wish to delete this item?\n\nClick OK to confirm..."
        };

        var methods = {
            timer: null,
            init: function ($caller) {
                this.bind($caller);
            },
            bind: function ($caller) {

                var event = $caller.data(dataKey).event,
                    message = $caller.data("confirmerMessage") || $caller.data(dataKey).message;;
                $caller.on(event, function (e) {
                    return confirm(message);
                    });

            },
            unbind: function ($caller) {
                var event = $caller.data(dataKey).event;
                $caller.unbind(event);
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
                    // $(selector).confirmer()
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
                    // $().confirmer()
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
        pagedList: pagedList.init,
        autoComplete: autoComplete.init,
        typeSpy: typeSpy.init,
        blurSpy: blurSpy.init,
        filterList: filterList.init,
        tagIt: tagIt.init,
        userAutoComplete: userAutoComplete.init,
        labelAutoComplete: labelAutoComplete.init,
        userTagIt: userTagIt.init,
        labelTagIt: labelTagIt.init,
        selectDropdown: selectDropdown.init,
        labelSelectDropdown: labelSelectDropdown.init,
        categorySelectDropdown: categorySelectDropdown.init,
        confirmer: confirmer.init
    });

    // ---------------------------
    // Initialize plug-ins
    // ----------------------------

    $.fn.platoUI = function (opts) {

        /* Scolls to a specific element. Typical usage...
         * <a href="#somelement" data-provide="scroll"> */
        this.find('[data-provide="scroll"]').scrollTo();

        /* pagedList */
        this.find('[data-provide="paged-list"]').pagedList();
        
        /* select dropdown */
        this.find('[data-provide="select-dropdown"]').selectDropdown();

        /* label select dropdown */
        this.find('[data-provide="label-select-dropdown"]').labelSelectDropdown();

        /* category select dropdown */
        this.find('[data-provide="category-select-dropdown"]').categorySelectDropdown();

        /* treeView */
        this.find('[data-provide="tree"]').treeView();

        /* filterList */
        this.find('[data-provide="filter-list"]').filterList();
        
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

        /* confirmer */
        this.find('[data-provide="confirmer"]').confirmer();

    }

    $(doc).ready(function() {
        $("body").platoUI();
    });


}(window, document, jQuery));