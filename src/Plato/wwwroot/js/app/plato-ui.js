// <reference path="~/js/vendors/jquery.js" />
// <reference path="~/js/vendors/bootstrap.js" />

if (typeof window.jQuery === "undefined") {
    throw new Error("jQuery 3.3.1 or above Required");
}

if (typeof window.$().modal === 'undefined') {
    throw new Error("BootStrap 4.1.1 or above Required");
}

if (typeof window.$.Plato === "undefined") {
    throw new Error("$.Plato Required");
}

// --------------
// Core
// --------------

$(function (win, doc, $) {

    'use strict';

    // --------

    var app = win.$.Plato;

    // --------

    /* dialog */
    var dialog = function () {

        var dataKey = "dialog",
            dataIdKey = dataKey + "Id";

        var defaults = {
            id: "dialog",
            title: "Dialog Title",
            body: {
                html: null,
                url: null
            },
            buttons: [
                {
                    text: "Close",
                    id: "dialog",
                    click: function($modal) {
                        $modal.dismiss();
                    }
                }
            ],
            css: {
                modal: "modal fade",
                dialog: "modal-dialog" // add modal-lg,  modal-sm for sizing
            },
            onLoad: function ($caller) {}, // triggers when body.url is loaded
            onShow: function ($caller) {}, // triggers when the dialog is shown
            onHide: function ($caller) {} // triggers when the dialog is hidden
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

                methods.bind($caller);

            },
            bind: function($caller) {

                var $dialog = methods.getOrCreate($caller);
                $dialog.modal();

            },
            show: function($caller) {

                var $dialog = methods.getOrCreate($caller);
                $dialog.modal("show");
                
                methods.load($caller);

                // onShow event
                if ($caller.data(dataKey).onShow) {
                    $caller.data(dataKey).onShow($caller);
                }

            },
            hide: function($caller) {

                var $dialog = methods.getOrCreate($caller);
                $dialog.modal("hide");
                
                // onHide event
                if ($caller.data(dataKey).onShow) {
                    $caller.data(dataKey).onShow($caller);
                }

            },
            load: function($caller) {

                var url = $caller.data(dataKey).body.url;
                if (url === null) {
                    return;
                }
                if (url === "") {
                    return;
                }

                app.http({
                    method: "GET",
                    url: url
                }).done(function(response) {
                    var $body = $caller.find(".modal-content");
                    if ($body.length > 0) {
                        $body.empty();
                        if (response !== "") {
                            $body.html(response);
                            // Enable tooltips within loaded content
                            app.ui.initToolTips($body);
                            // confirm
                            $body.find('[data-provide="confirm"]').confirm();
                            // markdown body
                            $body.find('[data-provide="markdownBody"]').markdownBody();
                            /* popper */
                            $body.find('[data-provide="popper"]').popper();
                        }
                    }
                    
                    // onLoad event
                    if ($caller.data(dataKey).onLoad) {
                        $caller.data(dataKey).onLoad($caller, response.result);
                    }
                });

            },
            getOrCreate: function($caller) {

                var id = $caller.data(dataKey).id,
                    $dialog = $("#" + id);

                if ($dialog.length === 0) {

                    $dialog = $("<div>",
                        {
                            "id": id,
                            "role": "dialog",
                            "class": $caller.data(dataKey).css.modal,
                            "tabIndex": "-1"
                        });

                    var $model = $("<div>",
                            {
                                "class": $caller.data(dataKey).css.dialog
                            }),
                        $content = $("<div>",
                            {
                                "class": "modal-content"
                            }).append($('<p class="my-4 text-center"><i class="fal my-4 fa-spinner fa-spin"></i></p>'));

                    $model.append($content);
                    $dialog.append($model);
                    $("body").append($dialog);

                    return $($dialog);
                }

                return $dialog;

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
                    // $(selector).dialog()
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
                    // $().dialog()
                    var $caller = $("body");
                    if (methodName) {
                        if (methods[methodName]) {
                            $caller.data(dataKey, $.extend({}, defaults, options));
                            methods[methodName].apply(this, [$caller]);
                        } else {
                            alert(methodName + " is not a valid method!");
                        }
                    }
                    methods.init($caller);
                }

            }

        };

    }();

    /* dialogSpy */
    var dialogSpy = function () {

        var dataKey = "dialogSpy",
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

                $caller.bind("click",
                    function(e) {

                        e.preventDefault();
                        e.stopPropagation();

                        $().dialog({
                                id: $(this).data("dialogId") || "dialogSpy",
                                body: {
                                    url: $(this).attr("href")
                                },
                                css: {
                                    modal: methods.getModalCss($(this)),
                                    dialog: methods.getDialogCss($(this))
                                }
                            },
                            "show");
                    });

            },
            getModalCss: function($caller) {
                var css = $caller.data("dialogModalCss");
                if (css) {
                    return css;
                }
                return "modal fade";
            },
            getDialogCss: function($caller) {
                var css = $caller.data("dialogCss");
                if (css) {
                    return css;
                }
                return "modal-dialog modal-lg";
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
                    // $(selector).dialogSpy()
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
                    // $().dialogSpy()
                    var $caller = $("body");
                    if (methodName) {
                        if (methods[methodName]) {
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

    /* scrollTo */
    var scrollTo = function () {

        var dataKey = "scrollTo",
            dataIdKey = dataKey + "Id";

        $.extend($.easing,
            {
                def: 'easeOutQuad',
                easeInOutExpo: function (x, t, b, c, d) {
                    if (t === 0) return b;
                    if (t === d) return b + c;
                    if ((t /= d / 2) < 1) return c / 2 * Math.pow(2, 10 * (t - 1)) + b;
                    return c / 2 * (-Math.pow(2, -10 * --t) + 2) + b;
                }
            });


        var defaults = {
            offset: 0,
            interval: 250,
            event: "click",
            position: "top",
            target: null,
            animate: true,
            onBeforeComplete: null,
            onComplete: null
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
                            methods.go($caller);
                        });
                }

            },
            go: function($caller) {
                
                var $body = $("body,html");
                var $target = null,
                    href = $caller.prop("tagName") === "A" && $caller.attr("href");
                if (href) {
                    $target = $(href);
                } else {
                    $target = $caller.data(dataKey).target;
                    $body = $caller;
                }

                var interval = $caller.data(dataKey).interval,
                    position = $caller.data(dataKey).position,
                    offset = $caller.data(dataKey).offset;

                var top = 0;
                if ($target) {
                    top = position === "top" ? $target.offset().top : $target.offset().bottom;
                }
                             
                // animate scroll
                if ($caller.data(dataKey).animate) {
                    $body.stop().animate({
                            scrollTop: top + offset
                        },
                        interval,
                        'easeInOutExpo',
                        function() {
                            if ($caller.data(dataKey).onComplete) {
                                $caller.data(dataKey).onComplete($caller, $target);
                            }
                        });
                    if ($caller.data(dataKey).onBeforeComplete) {
                        $caller.data(dataKey).onBeforeComplete($caller, $target);
                    }
                } else {
                       if ($caller.data(dataKey).onBeforeComplete) {
                        $caller.data(dataKey).onBeforeComplete($caller, $target);
                    }
                    $caller.scrollTop(top + offset);
                    if ($caller.data(dataKey).onComplete) {
                        $caller.data(dataKey).onComplete($caller, $target);
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
                            var $caller = $("html");
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
    
    /* sticky */
    var sticky = function () {

        var dataKey = "sticky",
            dataIdKey = dataKey + "Id";

        var defaults = {
            offset: 0, // optional offset from scrollTop to trigger sticky positioning
            onUpdate: function ($caller) { },   // raised when element is made sticky / not sticky
            onScroll: function($caller, e, $win) {}
        };

        var methods = {
            init: function ($caller) {

                // Initialize initial state
                methods._setOriginalTop($caller);
                methods.update($caller);

                // Invoke onScroll and onUpdate upon load to position correctly

                if ($caller.data(dataKey).onScroll) {
                    $caller.data(dataKey).onScroll($caller, $(win));
                }

                if ($caller.data(dataKey).onUpdate) {
                    $caller.data(dataKey).onUpdate($caller);
                }
                
                // Bind events
                this.bind($caller);
            },
            bind: function ($caller) {

                // Bind resize events
                $(win).resizeSpy({
                    onResize: function ($win, e) {
                        if ($caller.data(dataKey).onScroll) {
                            $caller.data(dataKey).onScroll($caller, win);
                        }
                        if ($caller.data(dataKey).onUpdate) {
                            $caller.data(dataKey).onUpdate($caller);
                        }
                    }
                });

                // Bind scroll events
                $(win).scrollSpy({
                    onScroll: function (spy, e, $win) {

                        var scrollTop = $win.scrollTop(),
                            top = methods._getOriginalTop($caller),
                            offset = methods._getOffset($caller);

                        // onUpdate event
                        if (scrollTop > top - offset) {
                            if (!$caller.hasClass("fixed")) {
                                $caller.addClass("fixed");
                                if ($caller.data(dataKey).onUpdate) {
                                    $caller.data(dataKey).onUpdate($caller);
                                }
                            }
                        } else {
                            if ($caller.hasClass("fixed")) {
                                $caller.removeClass("fixed");
                                if ($caller.data(dataKey).onUpdate) {
                                    $caller.data(dataKey).onUpdate($caller);
                                }
                            }
                        }

                        // onScroll event
                        if ($caller.data(dataKey).onScroll) {
                            $caller.data(dataKey).onScroll($caller, $win);
                        }

                    }
                });
                
            },
            unbind: function ($caller) {
                var event = $caller.data(dataKey).event;
                $caller.unbind(event);
            },
            update: function ($caller) {

                var scrollTop = $(win).scrollTop(),
                    top = methods._getOriginalTop($caller),
                    offset = methods._getOffset($caller);

                if (scrollTop > top - offset) {
                    if (!$caller.hasClass("fixed")) {
                        $caller.addClass("fixed");
                        if ($caller.data(dataKey).onUpdate) {
                            $caller.data(dataKey).onUpdate($caller);
                        }
                    }
                } else {
                    if ($caller.hasClass("fixed")) {
                        $caller.removeClass("fixed");
                        if ($caller.data(dataKey).onUpdate) {
                            $caller.data(dataKey).onUpdate($caller);
                        }
                    }
                }
            },
            _setOriginalTop: function ($caller, top) {
                var key = dataKey + "_top";
                if (!$caller.data(key)) {
                    $caller.data(key, $caller.offset().top);
                }
            },
            _getOriginalTop: function($caller) {
                return $caller.data(dataKey + "_top");
            },
            _getOffset: function ($caller) {
                return $caller.data("stickyOffset") || $caller.data(dataKey).offset;
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
                    // $(selector).sticky()
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
                    // $().sticky()
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
            onToggle: function ($tree, $toggle, e) {
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

                // Bind toggle events
                $caller.find(toggleSelector).off(event).on(event,
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
                $caller.find(linkSelector).off(event).on(event,
                    function (e) {
                        if ($caller.data(dataKey).onClick) {
                            $caller.data(dataKey).onClick($caller, $(this), e);
                        }
                    });

                // Bind active state on mouse events
                $caller.find(".list-group-item").off("mouseover").on("mouseover",
                    function (e) {
                        e.stopPropagation();
                        $(this).parents(".list-group-item").each(function () {
                            if (!$(this).hasClass("checked")) {
                                $(this).removeClass("active");
                            }
                        });
                        $(this).addClass("active");
                    });

                $caller.find(".list-group-item").off("mouseleave").on("mouseleave",
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
            scrollToSelected: function ($caller) {

                // Ensure selected are expanded
                methods.expandSelected($caller);

                // Scroll to active item
                var $active = $caller.find(".active");
                if ($active.length > 0) {
                    var offset = $active.offset(),
                        top = offset.top - $caller.offset().top;
                    $caller.scrollTo({
                            offset: top - 20,
                            interval: 500
                        },
                        "go");
                }

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

                noResultsText = app.T(noResultsText);
            
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
            loaderTemplate: '<p class="text-center"><i class="fal fa-spinner fa-spin"></i></p>', // a handlebars style template for auto complete list items
            noResultsText: "Sorry no results matched your search!", // the text to display when no results are available
            noResultsIcon: null, // optional icon to display above noResultsText
            itemSelection: {
                enable: true,
                index: -1,
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
                app.http(config).done(function(response) {
                    if (response.statusCode !== 200) {
                        return;
                    }
                    if ($caller.data(dataKey).onLoad) {
                        $caller.data(dataKey).onLoad($caller, response.result);
                    }
                });

            },
            getConfig: function($caller) {

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
            getItemIndex: function($caller) {
                var index = -1,
                    selector = null,
                    selection = $caller.data(dataKey).itemSelection,
                    itemCss = $caller.data(dataKey).itemCss;
                if (selection) {
                    selector = "a." + itemCss;
                    if (selection.enable === false) {
                        return index;
                    }
                    $caller.find(selector).each(function() {
                        if ($(this).hasClass(selection.css)) {
                            return index;
                        }
                        index++;
                    });
                }
                return index;
            },
            setItemIndex: function($caller) {
                var selection = $caller.data(dataKey).itemSelection,
                    itemCss = $caller.data(dataKey).itemCss;

                if (selection) {
                    if (selection.enable === false) {
                        return;
                    }
                    var index = selection.index;
                    var selector = "a." + itemCss,
                        tag = selection.tag,
                        css = selection.css,
                        $el = $caller.find(selector + ":eq(" + index + ")");

                    if (index < 0) {
                        $caller.find(selector).each(function() {
                            $(this).removeClass(css);
                        });
                    } else {
                        $caller.find(selector).each(function() {
                            $(this).removeClass(css);
                        });
                        if ($el.length > 0) {
                            $el.addClass(css);
                        }
                    }

                }
            },
            setPageIndex: function($caller, pageIndex) {
                $caller.data(dataKey).page = pageIndex;
            },
            getPageIndex: function($caller) {
                return $caller.data("pagedListPageIndex") || $caller.data(dataKey).page;
            },
            setPageSize: function($caller, pageSize) {
                $caller.data(dataKey).pageSize = pageSize;
            },
            getPageSize: function($caller) {
                return $caller.data("pagedListPageSize") || $caller.data(dataKey).pageSize;
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
            init: function($caller, methodName) {

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
            bind: function($caller) {

                $caller.bind("focus",
                    function() {
                        if ($(this).val().length === 0) {
                            methods.hide($(this));
                        } else {
                            // Show
                            methods.show($(this),
                                function() {
                                    // Update if not already visible
                                    methods.update($caller);
                                });

                        }
                    });

                $caller.bind("keydown.",
                    function(e) {
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
                                        $target.find("." + itemCss).each(function() {
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
                                        if (newIndex > pageSize - 1) {
                                            newIndex = pageSize - 1;
                                        }
                                        break;
                                    }

                                    $target.pagedList({
                                            itemSelection: $.extend(itemSelection,
                                                {
                                                    index: newIndex
                                                })
                                        },
                                        "setItemIndex");


                                }

                            }
                        }

                        if ($caller.data(dataKey).onKeyDown) {
                            $caller.data(dataKey).onKeyDown($caller, e);
                        }

                    });

                // spy on our input
                $caller.typeSpy({
                    onKeyUp: function($el, e) {
                        if (e.keyCode === 27) {
                            // escape
                            methods.hide($el);
                        }
                    },
                    onChange: function($el, e) {

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
                $(doc).click(function(e) {
                    var target = e.target;
                    if (target) {
                        if (target.tagName.toUpperCase() === "INPUT") {
                            return;
                        }
                        if (target.tagName.toUpperCase() === "A") {
                            return;
                        }
                        if (target.tagName.toUpperCase() === "UL") {
                            return;
                        }
                        if (target.tagName.toUpperCase() === "I") {
                            return;
                        }
                    }
                    methods.hide($caller);
                });

            },
            unbind: function($caller) {
                $caller
                    .unbind("focus")
                    .unbind("keydown");
                $caller.typeSpy("unbind");
            },
            show: function($caller, onShow) {
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
                        if ($caller.data(dataKey).onShow) {
                            $caller.data(dataKey).onShow($caller, $target);
                        }
                    }
                }
            },
            hide: function($caller, onHide) {
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
                        if ($caller.data(dataKey).onHide) {
                            $caller.data(dataKey).onHide($caller, $target);
                        }
                    }
                }
            },
            update: function($caller) {

                // no target found
                var $target = methods.getTarget($caller);
                if ($target.length === 0) {
                    return;
                }

                // Clone config & get keywords field name
                var config = $.extend({}, $caller.data(dataKey).config),
                    valueField = $caller.data(dataKey).valueField;

                // Any supplied data-autocomplete-url attribute  
                // should override any configured config.url property
                if ($caller.data("autocompleteUrl")) {
                    config.url = $caller.data("autocompleteUrl");
                }
                
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
                $target.pagedList($.extend({},
                    $caller.data(dataKey),
                    {
                        page: 1,
                        config: config
                    }));

            },
            getTarget: function($caller) {

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
            init: function($caller) {
                this.bind($caller);
            },
            bind: function($caller) {

                var event = $caller.data(dataKey).event;
                if (event === null) {
                    return;
                }

                $caller.bind(event,
                    function(e) {

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
            startTimer: function($caller, e) {
                this.stopTimer();
                this.timer = setTimeout(function() {
                        if ($caller.data(dataKey).onChange) {
                            $caller.data(dataKey).onChange($caller, e);
                        }
                    },
                    $caller.data(dataKey).interval);
            },
            stopTimer: function() {
                win.clearTimeout(this.timer);
                this.timer = null;
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
            id: "blurSpy", // unique namespace
            interval: 100, // interval in milliseconds to wait before firing onBlur event
            onBlur: null // triggers after interval if element does not receive focus again
        };

        var methods = {
            timer: null,
            init: function($caller) {
                this.bind($caller);
            },
            bind: function($caller) {

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
                    function(e) {
                        methods.startTimer($(this), e);
                    });

            },
            unbind: function($caller) {
                $caller.unbind('blur');
                $caller.unbind('focus');
            },
            startTimer: function($caller, e) {
                this.stopTimer();
                this.timer = setTimeout(function() {
                        if ($caller.data(dataKey).onBlur) {
                            $caller.data(dataKey).onBlur($caller, e);
                        }
                    },
                    $caller.data(dataKey).interval);
            },
            stopTimer: function() {
                win.clearTimeout(this.timer);
                this.timer = null;
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

    /* scrollSpy */
    var scrollSpy = function () {

        var dataKey = "scrollSpy",
            dataIdKey = dataKey + "Id",
            eventsKey = dataKey + "Events";

        var defaults = {
            interval: 350, // the duration to wait in milliseconds before invoking the onScrollEnd event
            onScrollStart: null,
            onScrollEnd: null,
            onScroll: null
        };

        var methods = {
            _timer: null,
            _scrolling: false,
            init: function($caller, methodName) {

                if (methodName) {
                    if (this[methodName] !== null && typeof this[methodName] !== "undefined") {
                        this[methodName].apply(this, [$caller]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return;
                }

                // Store scrollSpy events on the caller as multiple 
                // scrollSpy events could be tied to the same caller 
                // i.e.$(win).scrollSpy(opts)
                this._storeEvents($caller);

                // Bind events
                this.bind($caller);

            },
            bind: function($caller) {
                
                // Bind scroll
                $caller.on("scroll",
                    function(e) {
                        
                        // Start timer to detect end of scroll
                        methods.start($caller, e);

                        // Events to execute
                        var i = 0, events = $caller.data(eventsKey);
                        // Raise onScrollStart event
                        // _scrolling is set to false when the scroll ends
                        if (methods._scrolling === false) {
                            methods._scrolling = true;
                            if (events.onScrollStart.length > 0) {
                                for (i = 0; i < events.onScrollStart.length; i++) {
                                    events.onScrollStart[i](e);
                                }
                            }
                        }

                        // Raise onScroll passing in a normalized scroll threshold
                        if (events.onScroll.length > 0) {
                            var scrollTop = $caller.scrollTop(),
                                docHeight = $(doc).height(),
                                winHeight = $caller.height();
                            for (i = 0; i < events.onScroll.length; i++) {
                                events.onScroll[i]({
                                        scrollTop: Math.ceil(scrollTop),
                                        scrollBottom: Math.ceil(scrollTop + winHeight),
                                        documentHeight: docHeight,
                                        windowHeight: winHeight
                                    },
                                    e, $caller);
                            }
                        }

                    });
            },
            unbind: function($caller) {
                $caller.off("scroll");
            },
            start: function($caller, e) {
                methods.stop($caller);
                methods._timer = win.setTimeout(function() {
                        methods._scrolling = false;
                        var events = $caller.data(eventsKey);
                        if (events.onScrollEnd.length > 0) {
                            for (var i = 0; i < events.onScrollEnd.length; i++) {
                                events.onScrollEnd[i](e);
                            }
                        }
                    },
                    $caller.data(dataKey).interval);
            },
            stop: function($caller) {
                win.clearTimeout(methods._timer);
                methods._timer = null;
            },
            _storeEvents: function ($caller) {

                var events = {
                    onScrollStart: [],
                    onScrollEnd: [],
                    onScroll: []
                };

                if ($caller.data(eventsKey)) {
                    events = $caller.data(eventsKey);
                }

                var validEvent = function (arr, func) {
                    if (!func) { return false; }
                    return arr.indexOf(func) === -1;
                };
                
                if (validEvent(events.onScrollStart, $caller.data(dataKey).onScrollStart)) {
                    events.onScrollStart.push($caller.data(dataKey).onScrollStart);
                }

                if (validEvent(events.onScrollEnd, $caller.data(dataKey).onScrollEnd)) {
                    events.onScrollEnd.push($caller.data(dataKey).onScrollEnd);
                }

                if (validEvent(events.onScroll, $caller.data(dataKey).onScroll)) {
                    events.onScroll.push($caller.data(dataKey).onScroll);
                }
             
                // Store events on caller
                $caller.data(eventsKey, events);

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
                    // $(selector).scrollSpy()
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
                    // $().scrollSpy()
                    var $caller = $(win);
                    if ($caller.length > 0) {
                        if (!$caller.data(dataIdKey)) {
                            var id = dataKey + parseInt(Math.random() * 100) + new Date().getTime();
                            $caller.data(dataIdKey, id);
                            $caller.data(dataKey, $.extend({}, defaults, options));
                        } else {
                            $caller.data(dataKey, $.extend({}, $caller.data(dataKey), options));
                        }
                        methods.init($caller, methodName);
                    }
              
                }

            }

        };

    }();

    /* resizeSpy */
    var resizeSpy = function () {

        var dataKey = "resizeSpy",
            dataIdKey = dataKey + "Id",
            eventsKey = dataKey + "Events";

        var defaults = {
            interval: 350, // the duration to wait in milliseconds before invoking the onScrollEnd event
            onResizeStart: null,
            onResizeEnd: null,
            onResize: null
        };

        var methods = {
            _timer: null,
            _resizing: false,
            init: function ($caller, methodName) {

                if (methodName) {
                    if (this[methodName] !== null && typeof this[methodName] !== "undefined") {
                        this[methodName].apply(this, [$caller]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return;
                }

                // Store resizeSpy events on the caller as multiple 
                // resizeSpy events could be tied to the same caller 
                // i.e.$(win).resizeSpy(opts)
                this._storeEvents($caller);

                // Bind events
                this.bind($caller);

            },
            bind: function ($caller) {

                // Bind resize
                $caller.on("resize",
                    function (e) {

                        // Start timer to detect end of resize
                        methods.start($caller, e);

                        // Events to execute
                        var i = 0, events = $caller.data(eventsKey);

                        // Raise onResizeStart event
                        // _resizing is set to false when the resize ends
                        if (methods._resizing === false) {
                            methods._resizing = true;
                            if (events.onResizeStart.length > 0) {
                                for (i = 0; i < events.onResizeStart.length; i++) {
                                    events.onResizeStart[i](e);
                                }
                            }
                        }

                        // Raise onResize
                        if (events.onResize.length > 0) {
                            for (i = 0; i < events.onResize.length; i++) {
                                events.onResize[i]($caller, e);
                            }
                        }

                    });
            },
            unbind: function ($caller) {
                $caller.off("resize");
            },
            start: function ($caller, e) {
                methods.stop($caller);
                methods._timer = win.setTimeout(function () {
                    methods._resizing = false;
                    var events = $caller.data(eventsKey);
                    if (events.onResizeEnd.length > 0) {
                        for (var i = 0; i < events.onResizeEnd.length; i++) {
                            events.onResizeEnd[i](e);
                        }
                    }
                },
                    $caller.data(dataKey).interval);
            },
            stop: function ($caller) {
                win.clearTimeout(methods._timer);
                methods._timer = null;
            },
            _storeEvents: function ($caller) {

                var events = {
                    onResizeStart: [],
                    onResizeEnd: [],
                    onResize: []
                };

                if ($caller.data(eventsKey)) {
                    events = $caller.data(eventsKey);
                }

                if ($caller.data(dataKey).onResizeStart) {
                    events.onResizeStart.push($caller.data(dataKey).onResizeStart);
                }

                if ($caller.data(dataKey).onResizeEnd) {
                    events.onResizeEnd.push($caller.data(dataKey).onResizeEnd);
                }

                if ($caller.data(dataKey).onResize) {
                    events.onResize.push($caller.data(dataKey).onResize);
                }

                // Store events on caller
                $caller.data(eventsKey, events);

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
                    // $(selector).scrollSpy()
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
                    // $().scrollSpy()
                    var $caller = $(win);
                    if ($caller.length > 0) {
                        if (!$caller.data(dataIdKey)) {
                            var id = dataKey + parseInt(Math.random() * 100) + new Date().getTime();
                            $caller.data(dataIdKey, id);
                            $caller.data(dataKey, $.extend({}, defaults, options));
                        } else {
                            $caller.data(dataKey, $.extend({}, $caller.data(dataKey), options));
                        }
                        methods.init($caller, methodName);
                    }

                }

            }

        };

    }();

    /* leaveSpy */
    var leaveSpy = function () {

        var dataKey = "leaveSpy",
            dataIdKey = dataKey + "Id";

        var defaults = {
            selector: null, // the selector we must leave to trigger the onLeave event 
            interval: 500, // interval in milliseconds to wait before firing leave
            onLeave: null // triggers after interval when mouse leaves element
        };

        var methods = {
            timer: null,
            init: function ($caller) {
                this.bind($caller);
            },
            bind: function ($caller) {

                $caller.on("mouseleave", function (e) {

                    var target = e.target,
                        related = e.relatedTarget,
                        selector = $caller.data(dataKey).selector,
                        match;

                    if (selector) {

                        // search for a parent node matching our selector
                        while (target && target !== document && !(match = matches(target, selector))) {
                            target = target.parentNode;
                        }

                        // exit if no matching node has been found
                        if (!match) { return; }

                        // loop through the parent of the related target to make sure that it's not a child of the target
                        while (related && related !== target && related !== document) {
                            related = related.parentNode;
                        }

                        // exit if this is the case
                        if (related === target) { return; }

                    }
                 
                    // we are not on selector, start timer to trigger event
                    methods.startTimer($(this));

                    // We'll use querySelectorAll to find all element matching the selector,
                    // then check if the given element is included in that list.
                    // Executing the query on the parentNode reduces the resulting nodeList,
                    // document doesn't have a parentNode, though.
                    function matches(elem, selector) {
                        var nodeList = (elem.parentNode || document).querySelectorAll(selector) || [],
                            i = nodeList.length;
                        while (i--) {
                            if (nodeList[i] === elem) { return true; }
                        }
                        return false;
                    };

                });

                $caller.on("mouseenter", function (e) {
                    // we've moved back in, cancel timer
                    methods.stopTimer();
                });

            },
            unbind: function ($caller) {
                $caller.off('mouseenter');
                $caller.off('mouseenter');
            },
            startTimer: function ($caller) {
                this.stopTimer();
                this.timer = setTimeout(function () {
                    if ($caller.data(dataKey).onLeave) {
                        $caller.data(dataKey).onLeave($caller);
                    }
                }, $caller.data(dataKey).interval);
            },
            stopTimer: function () {
                win.clearTimeout(this.timer);
                this.timer = null;
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
                    // $(selector).leaveSpy()
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
                } 
            }

        };

    }();
    
    /* InfiniteScroll */
    var infiniteScroll = function() {

        var dataKey = "infiniteScroll",
            dataIdKey = dataKey + "Id",
            state = win.history.state || {};

        var defaults = {
            scrollSpacing: 0, // optional spacing to apply when scrolling to selected offset
            offsetSuffix: "/",
            pagerKey: "pager.page",
            loaderSelector: ".infinite-scroll-loader",
            loaderTemplate: '<p class="text-center"><i class="fal fa-2x fa-spinner fa-spin py-5"></i></p>',
            onPageLoaded: null,
            css: {
                item: "infinite-scroll-item",
                active: "infinite-scroll-item-active",
                inactive: "infinite-scroll-item-inactive"
            }
        };

        var methods = {
            _loading: false, // track loading state
            _page: 1, // current page
            _rowOffset: 0, // starting row offset
            _offset: 0, // optional selected offset
            _totalPages: 1, // total pages
            _loadedPages: [], // keep track of which pages have been loaded
            _readyList: [], // functions to execute when dom is updated
            ready: function($caller, fn) { // Accepts functions that will be executed upon each load
                methods._readyList.push(fn);
                return this;
            },
            init: function($caller, methodName, func) {

                if (methodName) {
                    if (this[methodName] !== null && typeof this[methodName] !== "undefined") {
                        this[methodName].apply(this, [$caller, func]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return;
                }

                if ($caller.data("infiniteScrollPage")) {
                    var page = parseInt($caller.data("infiniteScrollPage"));
                    if (!isNaN(page)) {
                        methods._page = page;
                    }
                }

                if (typeof $caller.data("infiniteScrollRowOffset") !== "undefined") {
                    var rowOffset = parseInt($caller.data("infiniteScrollRowOffset"));
                    if (!isNaN(rowOffset)) {
                        methods._rowOffset = rowOffset;
                    }
                }

                if (typeof $caller.data("infiniteScrollOffset") !== "undefined") {
                    var offset = parseInt($caller.data("infiniteScrollOffset"));
                    if (!isNaN(offset)) {
                        methods._offset = offset;
                    }
                }

                if ($caller.data("infiniteScrollTotalPages")) {
                    var totalPages = parseInt($caller.data("infiniteScrollTotalPages"));
                    if (!isNaN(totalPages)) {
                        methods._totalPages = totalPages;
                    }
                }

                // Default page already rendered server side
                methods._loadedPages.push({
                    page: methods._page,
                    offset: methods._rowOffset
                });

                // Bind events
                this.bind($caller);

            },
            bind: function($caller) {

                var bindScrollEvents = function () {

                    var scrollTop = 0;

                    // Bind scroll events
                    $(win).scrollSpy({
                        onScrollEnd: function() {
                            methods.updateState($caller);
                        },
                        onScroll: function(spy, e, $win) {
                            
                            // Ensure we are not already loading 
                            if (methods._loading) {
                                $win.scrollTop(scrollTop);
                                $win.scrollSpy("stop");
                                e.preventDefault();
                                e.stopPropagation();
                                return false;
                            }

                            scrollTop = spy.scrollTop;

                            // Get container bounds
                            var top = $caller.offset().top,
                                bottom = top + $caller.outerHeight();

                            // Optional scroll spacing
                            var scrollSpacing = 0;
                            if ($caller.data(dataKey).scrollSpacing) {
                                scrollSpacing = $caller.data(dataKey).scrollSpacing;
                            }

                            // At the very top of the window remove offset from url
                            if (spy.scrollTop === 0) {
                                methods.resetState($caller);
                            } else {
                                // When we reach the top of our container + any scroll spacing load previous page
                                if (spy.scrollTop < top - scrollSpacing) {
                                    methods.loadPrevious($caller, spy);
                                }
                            }

                            // At the very bottom of the page
                            if (spy.scrollBottom === spy.docHeight) {
                                methods.resetState($caller);
                            } else {
                                // When we reach the bottom of our container load next page
                                if (spy.scrollBottom > bottom) {
                                    methods.loadNext($caller, spy);
                                }
                            }
                        }
                    });

                };

                // Scroll to any selected offset, wait until we complete
                // scrolling before binding our scrollSpy events
                if (methods._offset > 0) {
                    var $marker = methods.getOffsetMarker($caller, methods._offset),
                        $highlight = methods.getHighlightMarker($caller, methods._offset);
                    if ($marker && $highlight) {
                        $().scrollTo({
                                offset: -$caller.data(dataKey).scrollSpacing,
                                target: $marker,
                                onComplete: function() {
                                    // Apply css to deactivate selected offset css (set server side)
                                    // Css can be applied directly to marker or a child of the marker
                                    if ($highlight.hasClass(defaults.css.active)) {
                                        $highlight
                                            .removeClass(defaults.css.active)
                                            .addClass(defaults.css.inactive);
                                    } else {
                                        $caller.find("." + defaults.css.active)
                                            .removeClass(defaults.css.active)
                                            .addClass(defaults.css.inactive);
                                    }
                                    bindScrollEvents();
                                }
                            },
                            "go"); // initialize scrollTo
                    } else {
                        // If we didn't find a marker ensure we still bind scrollSpy
                        bindScrollEvents();
                    }

                } else {
                    // Bind events right away
                    bindScrollEvents();
                }

            },
            unbind: function($caller) {
                $(win).scrollSpy("unbind");
                methods._readyList = [];
                methods._page = 1;
                methods._loading = false;
            },
            loadPrevious: function($caller, spy) {

                // Get page and check bounds
                var pageNumber = methods.getPreviousPageNumber($caller);
                if (pageNumber <= 0) {
                    return;
                }

                // When scrolling up or loading the previous page we disable overflow on the body
                // This ensures the scrollbar loses focus and the user is forced to focus the scrollbar again
                // This prevents the user being able to scroll above the page we are loading whilst the 
                // page is being loaded, once the page is loaded overflow is enabled again via app.http.onAlways
                $("body").css({ "overflow": "hidden" });

                // Show loader
                var $loader = methods.getLoader($caller, "previous");
                if ($loader) {
                    $loader.show();
                }

                // Load data
                methods.load($caller,
                    pageNumber,
                    spy,
                    function(data) {
                        if ($loader) {
                            $loader.hide();
                        }
                        if (data !== "") {

                            // Append response 
                            $loader.after(data);

                            // Get loaded page
                            var page = methods.getLoadedPage(methods._page);
                            if (page) {

                                // Scroll position before content was loaded
                                var previousPosition = page.spy.documentHeight - page.spy.scrollTop;

                                // Persist scroll position after content load
                                $(win).scrollSpy("unbind");
                                $().scrollTo({
                                        offset: $(doc).height() - previousPosition,
                                        interval: 100,
                                        onComplete: function() {
                                            $(win).scrollSpy("bind");
                                        }
                                    },
                                    "go");

                            }

                            // Highlight first marker in newly loaded page
                            methods.highlightFirstMarkerOnPage($caller, methods._page + 1);
                        }

                    });
            },
            loadNext: function($caller, spy) {

                // Get page and check bounds
                var pageNumber = methods.getNextPageNumber($caller);
                if (pageNumber > methods._totalPages) {
                    return;
                }

                // Show loader
                var $loader = methods.getLoader($caller, "next");
                if ($loader) {
                    $loader.show();
                }

                // Load data
                methods.load($caller,
                    pageNumber,
                    spy,
                    function(data) {
                        if ($loader) {
                            $loader.hide();
                        }
                        if (data !== "") {

                            // Append response
                            $loader.before(data);

                            // Highlight first marker in newly loaded page
                            methods.highlightFirstMarkerOnPage($caller, methods._page);
                        }

                    });

            },
            load: function($caller, page, spy, func) {

                // Ensure we have a callback url
                var url = methods.getUrl($caller),
                    pageLoaded = methods.isPageLoaded($caller, page);
                if (url === "" || pageLoaded || methods._loading === true) {
                    return;
                }
                
                // Indicate load
                methods._loading = true;

                // onLoad event
                if ($caller.data(dataKey).onLoad) {
                    $caller.data(dataKey).onLoad($caller);
                }

                // Append our page index to the callback url
                url += url.indexOf("?") >= 0 ? "&" : "?";
                url += defaults.pagerKey + "=" + page;

                // Request
                app.http({
                    url: url,
                    method: "GET",
                    onAlways: function (xhr, textStatus) {
                        $("body").css({ "overflow": "auto" });
                        // Update loading flag
                        methods._loading = false;
                    }
                }).done(function(data) {

               
                    // Mark done loading 
                    methods._loading = false;

                    // If a page was returned register page as loaded
                    if (data !== "") {

                        var offset = 0,
                            marker = null,
                            $markers = methods.getOffsetMarkers($(data));

                        // Get first offset marker within response
                        if ($markers) {
                            for (var x = 0; x < $markers.length - 1; x++) {
                                marker = $markers[x];
                                break;
                            }
                        }

                        // Get offset from first marker
                        if (marker) {
                            offset = parseInt(marker.getAttribute("data-infinite-scroll-offset"));
                        }

                        // Add loaded page with offset and scrollSpy position
                        methods._loadedPages.push({
                            spy: spy,
                            page: page,
                            offset: !isNaN(offset) ? offset : 0
                        });

                        // Update current page
                        methods._page = page;

                    }

                    // Callback
                    func(data);

                    // onLoaded event
                    if ($caller.data(dataKey).onLoaded) {
                        $caller.data(dataKey).onLoaded($caller);
                    }

                    // Execute any externally registered functions
                    for (var i = 0; i < methods._readyList.length; i++) {
                        if (typeof methods._readyList[i] === "function") {
                            methods._readyList[i]($caller);
                        }
                    }

                });

            },
            isElementInViewPort: function($caller, el) {
                var rect = el.getBoundingClientRect(),
                    scrollSpacing = $caller.data(dataKey).scrollSpacing || 0;
                return (
                    rect.top >= scrollSpacing &&
                        rect.left >= 0 &&
                        rect.bottom <= (window.innerHeight || $(window).height()) &&
                        rect.right <= (window.innerWidth || $(window).width())
                );

            },
            updateState: function($caller) {

                // Iterate each offset marker and detect the first
                // visible marker within the client viewport
                var $marker = null,
                    $markers = methods.getOffsetMarkers($caller);
                if ($markers) {
                    $markers.each(function() {
                        if (methods.isElementInViewPort($caller, this)) {
                            $marker = $(this);
                            return false;
                        }
                    });
                }

                // Ensure we found a marker
                if ($marker) {
                    // Update url with offset if valid
                    var offset = parseInt($marker.data("infiniteScrollOffset"));
                    if (!isNaN(offset)) {

                        // Use replaceState to ensure the address bar is updated
                        // but we don't actually add new history state
                        history.replaceState(state, doc.title, methods.getStateUrl($caller, offset));
                    }
                }

            },
            getStateUrl: function($caller, offset) {
                
                function getUrl(input) {
                    
                    // We always need a Url
                    if (!input) {
                        throw new Error("A Url is required.");
                    }

                    var qs = null,
                        parts = input.split("?"),
                        url = parts[0];
                    
                    if (parts.length > 1) {
                        qs = "?" + parts[parts.length - 1];
                    }
                    
                    var params = null;
                    if (qs) {
                        params = [];
                        var pairs = qs.split("&");
                        for (var i = 0; i < pairs.length; i++) {
                            var pair = pairs[i].split("=");
                            if (pair.length > 1) {
                                params.push({
                                    key: pair[0],
                                    value: pair[1]
                                });
                            }
                        }
                    }

                    return {
                        url: url,
                        qs: qs ? qs : "",
                        params: params
                    };

                  
                }

                var url = methods.getUrl($caller), parts = getUrl(url);
                
                // Append offset if supplied
                var offsetString = "";
                if (offset !== null && typeof offset !== "undefined") {
                    offsetString = "/" + offset.toString();
                }

                return parts.url + offsetString + parts.qs;
                

            },
            resetState: function($caller) {
                // Stop scrollspy to prevent the OnScrollEnd event from executing
                $(win).scrollSpy("stop");
                // Clear offset
                if (state) {
                    history.replaceState(state, doc.title, methods.getStateUrl($caller));
                }
            },
            scrollToPage: function($caller, pageNumber) {
                var page = methods.getLoadedPage(pageNumber);
                if (page) {
                    var $marker = methods.getOffsetMarker($caller, page.offset);
                    if ($marker) {
                        $(win).scrollSpy("unbind");
                        // Scroll to offset marker for page
                        $().scrollTo({
                                offset: -75,
                                interval: 0,
                                target: $marker,
                                onComplete: function() {
                                    $(win).scrollSpy("bind");
                                }
                            },
                            "go");
                    }
                }
            },
            highlightFirstMarkerOnPage: function($caller, pageNumber) {
                var page = methods.getLoadedPage(pageNumber);
                if (page) {
                    var $marker = methods.getHighlightMarker($caller, page.offset);
                    if ($marker) {
                        if ($marker.hasClass(defaults.css.inactive)) {
                            $marker.removeClass(defaults.css.inactive);
                        }
                        if (!$marker.hasClass(defaults.css.active)) {
                            $marker.addClass(defaults.css.active);
                        }
                        win.setTimeout(function() {
                                if ($marker.hasClass(defaults.css.active)) {
                                    $marker
                                        .removeClass(defaults.css.active)
                                        .addClass(defaults.css.inactive);
                                }
                            },
                            250);
                    }
                }
            },
            getSortedPageNumbers: function() {
                var pages = [];
                for (var i = 0; i < methods._loadedPages.length; i++) {
                    pages.push(methods._loadedPages[i].page);
                }
                return pages.sort(function(a, b) {
                    return a - b;
                });
            },
            getLoadedPage: function(page) {
                for (var i = 0; i < methods._loadedPages.length; i++) {
                    if (methods._loadedPages[i].page === page) {
                        return methods._loadedPages[i];
                    }
                }
                return null;
            },
            isPageLoaded: function($caller, pageNumber) {
                var page = methods.getLoadedPage(pageNumber);
                return page !== null ? true : false;
            },
            getOffsetMarkers: function($container) {
                var $markers = $container.find("[data-infinite-scroll-offset]");
                if ($markers.length > 0) {
                    return $markers;
                }
                return null;
            },
            getOffsetMarker: function($caller, offset) {
                var $marker = $caller.find('[data-infinite-scroll-offset="' + offset + '"]');
                if ($marker.length > 0) {
                    return $($marker[0]);
                }
                return null;
            },
            getHighlightMarker: function($caller, offset) {
                var $marker = $caller.find('[data-infinite-scroll-highlight="' + offset + '"]');
                if ($marker.length > 0) {
                    return $marker;
                }
                return null;
            },
            getFirstOffsetMarker: function($caller) {
                var $markers = methods.getOffsetMarkers($caller);
                if ($markers) {
                    return $($markers[0]);
                }
                return null;
            },
            getLastOffsetMarker: function($caller) {
                var $markers = methods.getOffsetMarkers($caller);
                if ($markers) {
                    return $($markers[$markers.length - 1]);
                }
                return null;
            },
            getPreviousPageNumber: function($caller) {
                // Get a sorted array of page numbers, get first element and decrement by 1
                var numbers = methods.getSortedPageNumbers($caller);
                if (numbers) {
                    return numbers[0] - 1;
                }
                return methods._page - 1;
            },
            getNextPageNumber: function($caller) {
                // Get a sorted array of page numbers, get last element and increment by 1
                var numbers = methods.getSortedPageNumbers($caller);
                if (numbers) {
                    return numbers[numbers.length - 1] + 1;
                }
                return methods._page + 1;
            },
            getLoader: function($caller, type) {
                var $loader = null,
                    $loaders = methods.getLoaders($caller);
                if ($loaders) {
                    $loaders.each(function() {
                        if ($(this).data("type") === type) {
                            $loader = $(this);
                            return false;
                        }
                    });
                    if ($loader) {
                        $loader
                            .empty()
                            .append($($caller.data(dataKey).loaderTemplate));
                    }
                }
                return $loader;
            },
            getLoaders: function($caller) {
                var selector = $caller.data("infiniteScrollLoadingSelector") ||
                    $caller.data(dataKey).loaderSelector;
                if (selector) {
                    var $loaders = $caller.find(selector);
                    if ($loaders.length > 0) {
                        return $loaders;
                    }
                }
                return null;
            },
            getUrl: function($caller) {
                if ($caller.data("infiniteScrollUrl")) {
                    return $caller.data("infiniteScrollUrl");
                }
                return "";
            },
            getUrlSuffix: function($caller) {
                if ($caller.data("infiniteScrollUrlSuffix")) {
                    return $caller.data("infiniteScrollUrlSuffix");
                }
                return "/";
            }
        };

        return {
            init: function() {

                var options = {};
                var methodName = null;
                var func = null;
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
                        func = a;
                        break;
                    }
                }

                if (this.length > 0) {
                    // $(selector).infiniteScroll()
                    return this.each(function() {
                        if (!$(this).data(dataIdKey)) {
                            var id = dataKey + parseInt(Math.random() * 100) + new Date().getTime();
                            $(this).data(dataIdKey, id);
                            $(this).data(dataKey, $.extend({}, defaults, options));
                        } else {
                            $(this).data(dataKey, $.extend({}, $(this).data(dataKey), options));
                        }
                        methods.init($(this), methodName, func);
                    });
                } else {
                    // $().infiniteScroll()
                    var $caller = $('[data-provide="infiniteScroll"]');
                    if ($caller.length > 0) {
                        if (!$caller.data(dataIdKey)) {
                            var id = dataKey + parseInt(Math.random() * 100) + new Date().getTime();
                            $caller.data(dataIdKey, id);
                            $caller.data(dataKey, $.extend({}, defaults, options));
                        } else {
                            $caller.data(dataKey, $.extend({}, $caller.data(dataKey), options));
                        }
                        methods.init($caller, methodName, func);
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
            init: function($caller) {
                this.bind($caller);
            },
            bind: function($caller) {
                $caller.bind('keydown',
                    function(e) {
                        if (e.keyCode && e.keyCode === 13) {
                            e.preventDefault();
                        }
                    });
                $caller.bind('keyup',
                    function(e) {
                        methods.filter($(this));
                    });
            },
            unbind: function($caller) {
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
            find: function($root, word) {

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
            getTarget: function($caller) {
                var target = $caller.data("filterListTarget") || $caller.data(dataKey).target;
                if (target === null) {
                    throw new Error('You must set a target for the filter list either via the data-filter-list-target data attribute on the calling element or via the target option property.');
                }
                if (typeof target === "string") {
                    var $target = $(target);
                    if ($target.length > 0) {
                        return $(target);
                    }
                    throw new Error('Could not locate a target for filter list with selector "' + target + '".');
                }
                return target;
            },
            getListItems: function($caller) {
                var $list = this.getTarget($caller);
                return $list.find(".list-group-item");
            },
            getEmpty: function($caller) {
                var target = $caller.data("filterListEmpty") || $caller.data(dataKey).empty;
                if (typeof target === "string") {
                    return $(target);
                }
                return target;
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
                        maxItems = this.getMaxItems($caller);
                    if (maxItems > 0 && items.length >= maxItems) {
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
                        $inputLi.css({
                            "min-width": "initial",
                            "width": "100%"
                        });
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
                    maxItems = this.getMaxItems($caller);
                if (maxItems > 0 && items.length < maxItems) {
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
            getMaxItems: function ($caller) {
                return $caller.data("maxItems")
                    ? parseInt($caller.data("maxItems"))
                    : $caller.data(dataKey).maxItems;
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
                return $caller.data(dataKey).items;
            },
            setItems: function ($caller, items) {
                $caller.data(dataKey).items = items;
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
                if ($store) {
                    $store.val(value);
                }
            },
            serialize: function($caller) {
                var $store = this.getStore($caller);
                if ($store) {
                    var items = this.getItems($caller);
                    if (items) {
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
                            return methods[methodName].apply(this, [$caller]);
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

        function getOffset($caller) {

            function getOffsetFromParent($el, $parent) {

                var x = 0,
                    y = 0,
                    el = $el[0],
                    parent = $parent[0];
                while (el && el !== parent && !isNaN(el.offsetLeft) && !isNaN(el.offsetTop)) {
                    x += el.offsetLeft - el.scrollLeft + el.clientLeft;
                    y += el.offsetTop - el.scrollTop + el.clientTop;
                    el = el.offsetParent;
                }
                return { top: y, left: x };
            }

            return getOffsetFromParent($caller.find(".active"), $caller);

        }
        
        var defaults = {
            event: "click",
            items: [], // our array of selected items
            highlightIndex: 0,
            ensureUnique: true, // boolean to restrict duplicate items
            store: null, // optional selector for dom element which will store the JSON representing selected items
            itemTemplate: '<li class="list-group-item select-dropdown-item">{text} <a href="#" class="tagit-list-item-delete"><i class="fal fa-times"></i></a></li>',
            itemTemplateEmpty: '<li class="list-group-item">No results</li>',
            parseItemTemplate: function (html, data) {
                if (data.text) {
                    html = html.replace(/\{text}/g, data.text);
                }
                if (data.value) {
                    html = html.replace(/\{value}/g, data.value);
                }
                return html;
            }, // provides a method to parse our itemTemplate with data returned from service url
            onShow: function($caller, $dropdown) {

                $caller.find('input[type="radio"], input[type="checkbox"]').each(function() {
                    if ($(this).is(":checked")) {
                        var $parent = $(this).parent(".dropdown-item");
                        if (!$parent.hasClass("active")) {
                            $parent.addClass("active");
                        }
                    }
                });
                
                // Add initial text
                var $a = $caller.find(".dropdown-toggle");
                if ($a.length > 0) {
                    $a.data("navTextOriginal", $a.text());
                }

                // Collapse
                $caller.find('[data-toggle="collapse"]').each(function () {
                    $(this).bind("click",
                        function (e) {
                            // If not disabled toggle collapse
                            if (!$(this).attr("disabled")) {
                                if ($(this).attr("data-target")) {
                                    var $target = $($(this).attr("data-target"));
                                    if ($target.length > 0) {
                                        $target.collapse("toggle");
                                    }
                                }
                            }
                        });
                });
                
                // Scroll to active item
                var $scrollable = $caller.find(".overflow-auto");
                if ($scrollable.length > 0) {
                    var offset = getOffset($caller),
                        top = offset.top - $caller.height();
                    $scrollable.scrollTo({
                            offset: top,
                            interval: 500
                        },
                        "go");
                }
            

            }, // triggers when the dropdown is shown
            onChange: function ($caller, $input, e) {

                e.preventDefault();
                e.stopPropagation();

                // Label
                var $label = $input.parent(".dropdown-item");

                // Accomodate for disabled elements
                if ($input.attr("disabled") || $label.attr("disabled")) {
                    return;
                }
           
                // Clear active
                $caller.find(".dropdown-item").removeClass("active");
                
                // Apply active
                var items = [];
                $caller.find('input[type="radio"], input[type="checkbox"]').each(function () {

                    // Label
                    var $parent = $(this).parent(".dropdown-item");

                    // If we have a collapse target ensure it's hidden
                    if ($parent.attr("data-target")) {
                        var $target = $($parent.attr("data-target"));
                        if ($target.length > 0) {
                            $target.collapse("hide");
                        }
                    }

                    // Toggle active class
                    if ($(this).is(":checked")) {
                        if (!$parent.hasClass("active")) {
                            $parent.addClass("active");
                        }
                        if ($(this).data("navText")) {
                            items.push($(this).data("navText"));
                        }
                    } 

                    // Scroll to first active item
                    var $scrollable = $caller.find(".overflow-auto");
                    if ($scrollable.length > 0) {
                        var offset = getOffset($caller),
                            top = offset.top - $caller.height();
                        $scrollable.scrollTo({
                                offset: top,
                                interval: 500
                            },
                            "go");
                    }

                });

                // Optionally update .dropdown-toggle text upon change
                var s = "",
                    text = "",
                    $a = $caller.find(".dropdown-toggle");
                
                if (items.length > 0) {
                    for (var i = 0; i < items.length; i++) {
                        s += items[i];
                        if (i < items.length - 1) {
                            s += ", ";
                        }
                    }
                    text = s;
                } else {
                    if ($a.data("navTextOriginal")) {
                        text = $a.data("navTextOriginal");
                    }
                }
                $a.text(text);

            }, // triggers when a checkbox or radio button is changed within the dropdown
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

                // Provide optional stopPropagation support for labels
                $dropdown.find("label").click(function (e) {
                    if ($(this).data("stopPropagation")) {
                        e.stopPropagation();
                    }
                    if ($(this).data("preventDefault")) {
                        e.preventDefault();
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
                    if ($preview.data("emptyPreviewText")) {
                        var $li = $("<li class=\"list-group-item\">")
                            .append($preview.data("emptyPreviewText"));
                        $preview.append($li);
                    } else {
                        $preview
                            .append($($caller.data(dataKey).itemTemplateEmpty));
                    }
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
                $item.data("selectDropdownItem", data);

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
                if ($store) {
                    $store.val(value);
                }
            },
            serialize: function ($caller) {
                var $store = this.getStore($caller);
                if ($store) {
                    var items = this.getItems($caller);
                    if (items) {
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
                        throw new Error("A preview area could not be found for the select dropdown.");
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
    
    /* confirm */
    var confirm = function () {

        var dataKey = "confirm",
            dataIdKey = dataKey + "Id";

        var defaults = {
            event: "click", // unique namespace
            message: "Are you sure you wish to permanently delete this item?\n\nThis operation cannot be undone.\n\nClick OK to confirm..."
        };

        var methods = {
            timer: null,
            init: function ($caller) {
                this.bind($caller);
            },
            bind: function ($caller) {
                var event = $caller.data(dataKey).event,
                    message = $caller.data("confirmMessage") || $caller.data(dataKey).message;
                $caller.on(event,
                    function (e) {
                        return win.confirm(message);
                    });
            },
            unbind: function ($caller) {
                var event = $caller.data(dataKey).event;
                $caller.unbind(event);
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
                    // $(selector).confirm()
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
                    // $().confirm()
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

    /* resizeable */
    var resizeable = function () {

        var dataKey = "resizeable",
            dataIdKey = dataKey + "Id";

        var defaults = {
            onShow: null,
            onHide: null
        };

        var methods = {
            init: function ($caller, methodName) {

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

                var $bar = $caller.find(".resizable-bar"),
                    $container = $caller.find(".resizable-container"),
                    resizing = false,
                    cursorPosition = { x: 0, y: 0 },
                    dimensions = { w: 0, h: 0 };

                // Bar events

                $bar.bind("mousedown",
                    function (e) {
                        resizing = true;
                        if (!$bar.hasClass("active")) {
                            $bar.addClass("active");
                        }
                        cursorPosition = { x: e.clientX, y: e.clientY };
                        dimensions = { w: $caller.width(), h: $caller.height() };
                        if (!methods._isExpanded($caller)) {
                            if (methods._isHorizontal($caller)) {
                                methods._setCollapseSize($caller, $caller.height());
                            } else {
                                methods._setCollapseSize($caller, $caller.width());
                            }

                        }
                    });

                $bar.bind("dblclick",
                    function (e) {
                        methods.toggle($caller);
                    });

                // Window events 

                $(win).bind("mouseup",
                    function (e) {
                        resizing = false;
                        if ($bar.hasClass("active")) {
                            $bar.removeClass("active");
                        }
                        cursorPosition = { x: 0, y: 0 };
                    });

                $(win).bind("mousemove",
                    function (e) {
                        if (resizing === false) {
                            return;
                        }

                        var newPosition = { x: e.clientX, y: e.clientY },
                            horizontal = methods._isHorizontal($caller),
                            delta = horizontal
                                ? parseInt(newPosition.y - cursorPosition.y)
                                : parseInt(newPosition.x - cursorPosition.x),
                            size = horizontal
                                ? parseInt(dimensions.h - Math.floor(delta))
                                : parseInt(dimensions.w - Math.floor(delta));

                        if (horizontal) {
                            $caller.css({ "height": size });
                            $container.css({ "height": size - $bar.height() });
                        } else {
                            $caller.css({ "width": size });
                            $container.css({ "width": size - $bar.width() });
                        }

                    });

                // Bind close buttons
                $caller.find(".resizable-close").bind("click",
                    function (e) {
                        e.preventDefault();
                        methods.hide($caller);
                    });

            },
            unbind: function ($caller) {

                var $bar = $caller.find(".resizable-bar");
                if ($bar.length > 0) {
                    $bar.unbind("mousedown");
                }
                $(win).unbind("mouseup");
                $(win).unbind("mousemove");

            },
            toggle: function ($caller) {
                if (methods._isExpanded($caller)) {
                    methods.collapse($caller);
                } else {
                    methods.expand($caller);
                }
            },
            expand: function ($caller) {
                var $bar = $caller.find(".resizable-bar"),
                    $container = $caller.find(".resizable-container");
                if (methods._isHorizontal($caller)) {
                    $caller.css({ "height": $(win).height() });
                    $container.css({ "height": $(win).height() - $bar.height() });
                } else {
                    $caller.css({ "width": $(win).width() });
                    $container.css({ "height": $(win).width() - $bar.width() });
                }
            },
            collapse: function ($caller) {
                var size = methods._getCollapseSize($caller),
                    $bar = $caller.find(".resizable-bar"),
                    $container = $caller.find(".resizable-container");
                if (methods._isHorizontal($caller)) {
                    $caller.css({ "height": size });
                    $container.css({ "height": size - $bar.height() });
                } else {
                    $caller.css({ "width": size });
                    $container.css({ "width": size - $bar.width() });
                }
            },
            show: function ($caller) {
                if ($caller.hasClass("resizable-hidden")) {

                    var $bar = $caller.find(".resizable-bar"),
                        $container = $caller.find(".resizable-container");

                    // Make visible
                    $caller.removeClass("resizable-hidden");

                    // Set container height to ensure correct overflow upon show
                    $container.css({ "height": $caller.height() - $bar.height() });

                    // Ensure we scroll the container to the top
                    $container.scrollTo({
                            offset: 0,
                            interval: 500
                        },
                        "go");

                    // onShow event
                    if ($caller.data(dataKey).onShow) {
                        $caller.data(dataKey).onShow($caller);
                    }
                }
            },
            hide: function ($caller) {
                if (!$caller.hasClass("resizable-hidden")) {
                    $caller.addClass("resizable-hidden");
                    // onHide event
                    if ($caller.data(dataKey).onHide) {
                        $caller.data(dataKey).onHide($caller);
                    }
                }
            },
            toggleVisibility: function ($caller) {
                if ($caller.hasClass("resizable-hidden")) {
                    methods.show($caller);
                } else {
                    methods.hide($caller);
                }
            },
            _isExpanded($caller) {
                if (methods._isHorizontal($caller)) {
                    return $caller.height() === $(win).height();
                }
                return $caller.width() === $(win).width();
            },
            _setCollapseSize: function ($caller, size) {
                $caller.attr("data-collapse-size", size);
            },
            _getCollapseSize: function ($caller) {
                var size = parseInt($caller.attr("data-collapse-size"));
                if (!win.isNaN(size)) {
                    return size;
                }
                return Math.floor($(win).height() / 2);
            },
            _isHorizontal: function ($caller) {
                return methods._isTop($caller) || methods._isBottom($caller);
            },
            _isVertical: function ($caller) {
                return methods._isLeft($caller) || methods._isRight($caller);
            },
            _isLeft: function ($caller) {
                return $caller.hasClass("resizable-left");
            },
            _isTop: function ($caller) {
                return $caller.hasClass("resizable-top");
            },
            _isRight: function ($caller) {
                return $caller.hasClass("resizable-right");
            },
            _isBottom: function ($caller) {
                return $caller.hasClass("resizable-bottom");
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
                    // $(selector).resizeable()
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
                    // $().resizeable()
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
    
    /* autoTargetBlank */
    var autoTargetBlank = function () {

        var dataKey = "autoTargetBlank",
            dataIdKey = dataKey + "Id";

        var defaults = {
            selector: "a"
        };

        var methods = {
            timer: null,
            init: function ($caller, methodName) {
                
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
                var selector = $caller.data(dataKey).selector;
                $caller.find(selector).each(function() {
                    if ($(this).attr("href")) {
                        $(this).attr("data-auto-target", "true");
                        $(this).attr("target", "_blank");
                    }
                });
            },
            unbind: function ($caller) {
                var selector = $caller.data(dataKey).selector;
                $caller.find(selector).each(function () {
                    if ($(this).attr("data-auto-target")) {
                        $(this).removeAttr("target");
                    }
                });
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
                    // $(selector).autoTargetBlank()
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
                    // $().autoTargetBlank()
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

    /* autoLinkImages */
    var autoLinkImages = function () {

        var dataKey = "autoLinkImages",
            dataIdKey = dataKey + "Id";

        var defaults = {
            selector: "img"
        };

        var methods = {
            timer: null,
            init: function ($caller, methodName) {
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
                // Iterate images to auto link
                var selector = $caller.data(dataKey).selector;
                $caller.find(selector).each(function(i) {
                    // Ensure we have a src attribute to link to
                    if ($(this).attr("src")) {

                        // Is the parent tag already a link?
                        var parentTag = $(this).parent().prop("tagName");
                        if (parentTag && parentTag !== "A") {
                            // Wrap link around image
                            $(this).wrap($("<a/>",
                                {
                                    "href": $(this).attr("src"),
                                    "title": $(this).attr("alt") || "",
                                    "target": "_blank"
                                }));
                        }

                    }
                });
            },
            unbind: function ($caller) {
                // Iterate images to auto link
                var selector = $caller.data(dataKey).selector;
                $caller.find(selector).each(function (i) {
                    // Ensure we have a src attribute to link to
                    var src = $(this).attr("src");
                    if (src) {
                        // Is the parent tag a link?
                        var $parent = $(this).parent();
                        var parentTag = $parent.prop("tagName");
                        var href = $parent.attr("href");
                        // Ensure a parent link to the same image
                        if (parentTag && parentTag === "A" && href === src) {
                            // Add the inner image after the link
                            $parent.after($(this));
                            // Remove the link
                            $parent.remove();
                        }
                    }
                });
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
                    // $(selector).autoLinkImages()
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
                    // $().autoLinkImages()
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
    
    /* markdownBody */
    var markdownBody = function () {

        var dataKey = "markdownBody",
            dataIdKey = dataKey + "Id";

        var defaults = {
            autoTargetBlank: true,
            autoLinkImages : true
        };

        var methods = {
            timer: null,
            init: function ($caller, methodName) {
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
                if ($caller.data(dataKey).autoTargetBlank) {
                    $caller.autoTargetBlank();
                }
                if ($caller.data(dataKey).autoLinkImages) {
                    $caller.autoLinkImages();
                }
            },
            unbind: function ($caller) {
                if ($caller.data(dataKey).autoTargetBlank) {
                    $caller.autoTargetBlank("unbind");
                }
                if ($caller.data(dataKey).autoLinkImages) {
                    $caller.autoLinkImages("unbind");
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
                    // $(selector).targetBlank()
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
                    // $().targetBlank()
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
                '<a class="{itemCss}" href="{url}"><span class="avatar avatar-sm mr-2"><span style="background-image: url({avatar.url});"></span></span>{displayName}<span class="float-right">@{userName}</span></a>',
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

                if (result.avatar.url) {
                    html = html.replace(/\{avatar.url}/g, result.avatar.url);
                } else {
                    html = html.replace(/\{avatar.url}/g, "");
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

    /* userTagIt */
    var userTagIt = function (options) {

        var dataKey = "userTagIt",
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

            },
            bind: function ($caller) {

                // Maximum number of allowed selections
                var maxItems = methods.getMaxItems($caller);

                // init tagIt
                $caller.tagIt($.extend({
                    maxItems: maxItems,
                    itemTemplate:
                        '<li class="tagit-list-item"><div class="btn-group"><div class="btn btn-sm label label-outline font-weight-bold"><span class="avatar avatar-xs mr-2"><span style="background-image: url({avatar.url});"></span></span>{displayName}</div> <a href="#" class="btn btn-sm label label-outline dropdown-toggle-split tagit-list-item-delete"><i class="fal fa-fw fa-times"></i></a></div></li>',
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

                        if (result.avatar.url) {
                            html = html.replace(/\{avatar.url}/g, result.avatar.url);
                        } else {
                            html = html.replace(/\{avatar.url}/g, "");
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
                methods.getInput($caller).userAutoComplete($.extend({
                    onItemClick: function ($input, result, e) {

                        e.preventDefault();
                        
                        // Get index if item already exists, else return -1
                        var index = methods.getIndex($caller, result);
                        if (index === -1) {

                            // Add new item if within allowed bounds
                            var tagIt = $caller.data("tagIt");
                            var isBelowMax = maxItems > 0 && $caller.data("tagIt").items.length < maxItems;
                            if (isBelowMax) {
                                tagIt.items.push(result);
                                $caller.tagIt("update");
                                $caller.tagIt("focus");
                                $caller.tagIt("select");
                                $caller.tagIt("show");
                            } else {
                                $caller.tagIt("hide");
                            }

                            // We've reached max allowed bounds hide autoComplete
                            if (tagIt.items.length === tagIt.maxItems) {
                                $caller.tagIt("hide");
                            }

                        } else {
                            // Highlight duplicates
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
                    items = $caller.data("tagIt").items,
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
            getMaxItems: function ($caller) {
                return $caller.data("maxItems")
                    ? parseInt($caller.data("maxItems"))
                    : $caller.data(dataKey).maxItems;
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
    
    /* keyBinder */
    var keyBinder = function () {

        var dataKey = "keyBinder",
            dataIdKey = dataKey + "Id";

        var defaults = {
            id: "",
            event: "keyup",
            keys: [
                {
                    match: /(^|\s|\()(@([a-z0-9\-_/]*))$/i,
                    search: function ($input) { },
                    bind: function ($input) { }
                },
                {
                    match: /(^|\s|\()(#([a-z0-9\-_/]*))$/i,
                    search: function ($input) { },
                    bind: function ($input) { }
                }
            ],
            internalKeys: []
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

                // Build an array of all keys
                var keys = $caller.data(dataKey).keys;
                for (var i = 0; i < keys.length; i++) {
                    $caller.data(dataKey).internalKeys.push(keys[i]);
                }
            },
            bind: function ($caller) {

                // Bind should be called after all internalKeys have been initialized
                var key = null,
                    id = $caller.data(dataKey).id,
                    keys = $caller.data(dataKey).internalKeys,
                    event = $caller.data(dataKey).event;

                if (event === null || typeof event === "undefined") {
                    return;
                }

                // namespace event
                if (event.indexOf(".") === -1) {
                    if (id) {
                        event = event + "." + id;
                    }
                }

                $caller.on(event,
                    function (e) {
                        var match = false;
                        for (var i = 0; i < keys.length; i++) {

                            key = keys[i];

                            var selection = methods.getSelection($(this)),
                                searchResult = key.search($(this), selection),
                                search = searchResult.value;

                            if (search && key.match) {
                                match = key.match.test(search);
                            }

                            if (match) {

                                if (key.bind) {

                                    switch (e.which) {
                                        case 13: // carriage return
                                            return;
                                        case 38: // up
                                            return;
                                        case 40: // down
                                            return;
                                        case 27: // escape
                                            return;
                                        default:
                                            key.bind($(this), searchResult, e);
                                            break;
                                    }
                                }
                            }
                        }
                        if (!match && key) {
                            if (key.unbind) {
                                key.unbind($(this), key, e);
                            }
                        }
                    });

            },
            unbind: function ($caller) {
                $caller.unbind($caller.data(dataKey).event);
            },
            getSelection: function ($caller) {

                var e = $caller[0];

                return (
                    'selectionStart' in e &&
                    function () {
                        var l = e.selectionEnd - e.selectionStart;
                        return {
                            start: e.selectionStart,
                            end: e.selectionEnd,
                            length: l,
                            text: e.value.substr(e.selectionStart, l)
                        };
                    } ||

                    /* browser not supported */
                    function () {
                        return null;
                    }
                )();

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
                    // $(selector).keyBinder
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
                    // $().keyBinder
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

    /* textFieldMirror */
    var textFieldMirror = function () {

        var dataKey = "textFieldMirror",
            dataIdKey = dataKey + "Id";

        var defaults = {
            type: "",
            start: 0,
            ready: function ($caller) {

            }
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

                this.bind($caller);
            },
            bind: function ($caller) {

                var id = $caller.data(dataKey).id,
                    start = $caller.data(dataKey).start,
                    prefix = $caller.val().substring(0, start),
                    suffix = $caller.val().substring(start, $caller.val().length - 1),
                    marker = '_@_',
                    html = prefix + marker + suffix;

                var $mirror = methods.getOrCreateMirror($caller);
                if ($mirror) {
                    
                    // Populate & show mirror, ensure the mirrored fields input
                    // is encoded for safe display
                    $mirror.html(html
                        .replace(/</gi, '&lt;')
                        .replace(/>/gi, '&gt;')
                        .replace(/_@_/gi, '<span class="text-field-mirror-marker position-relative">@</span>')
                        .replace(/\n/gi, '<br/>')).show();

                    // Ensure mirror is always same height as caller
                    $mirror.css({
                        "height": $caller.outerHeight()
                    });

                    // Ensure mirror is always scrolled to same position as caller
                    $mirror[0].scrollTop = $caller.scrollTop();

                    // Marker added raise ready event
                    if ($caller.data(dataKey).ready) {
                        $caller.data(dataKey).ready($mirror);
                    }

                }

            },
            hide: function ($caller) {
                var $mirror = this.getOrCreateMirror($caller);
                if ($mirror) {
                    $mirror.hide();
                }
            },
            getOrCreateMirror: function ($caller) {
                var elementId = $caller.attr("id") ? $caller.attr("id") : $caller.attr("name"),
                    id = elementId + "Mirror",
                    $mirror = $("#" + id);
                if ($mirror.length === 0) {
                    $mirror = $('<div>',
                        {
                            "id": id,
                            "class": "form-control text-field-mirror"
                        })
                        .css({
                            "height": $caller.outerHeight()
                        });
                    $caller.before($mirror);
                }
                return $mirror;
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
                    // $(selector).textFieldMirror
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
                    // $().textFieldMirror
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

    /* suggester */
    var suggester = function () {

        var dataKey = "suggester",
            dataIdKey = dataKey + "Id";

        var defaults = {
            insertData: null // object representing data to insert
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

                // Normalize line breaks within suggester element
                // Required for Edge to correctly determine selectionStart & selectionEnd indexes
                var normalizedValue = $caller.val().replace(/(?:\r\n|\r|\n)/g, "\r\n");
                $caller.val(normalizedValue);

                // Wrap a relative wrapper around the input to correctly
                // position the absolutely positioned suggester menu
                if (!$caller.parent().hasClass("position-relative")) {
                    $caller.wrap($('<div class="position-relative"></div>'));
                }

                // Track key patterns
                $caller.keyBinder($caller.data(dataKey));

            },
            unbind: function ($caller) {
                $caller.keyBinder("unbind");
            },
            show: function ($caller) {

                // Ensure our input has focus
                $caller.focus();

                // Get selection
                var cursor = this.getSelection($caller);

                // Invoke text field mirror to correctly position suggester menu
                $caller.textFieldMirror({
                    start: cursor.start,
                    ready: function ($mirror) {
                        
                        // Get position from mirrored marker
                        var $marker = $mirror.find(".text-field-mirror-marker");

                        // We always need a marker to position the suggester menu
                        if ($marker.length === 0) {
                            return false;
                        }

                        var position = $marker.position(),
                            left = Math.floor(position.left),
                            top = Math.floor(position.top + 26);

                        // Build & position menu
                        var $menu = methods.getOrCreateMenu($caller);
                        if ($menu) {
                            $menu.css({
                                "left": left + "px",
                                "top": top + "px"
                            }).show();

                            // Hide mirror after positioning menu
                            $caller.textFieldMirror("hide");

                            // Invoke paged list
                            $menu.pagedList($.extend($caller.data(dataKey),
                                {

                                }));
                        }


                    }
                });

            },
            hide: function ($caller) {
                var $menu = methods.getOrCreateMenu($caller);
                if ($menu) {
                    $menu.hide();
                }
            },
            insert: function ($caller) {

                $caller.focus();

                var data = $caller.data(dataKey).insertData;
                if (data) {

                    var sel = this.getSelection($caller),
                        index = data.index, // index from which everything will be replaced upto selection.start
                        value = data.value + " ", // add a space after the value we are inserting
                        cursor = index + value.length + 1; // position at end of inserted value

                    // Select everything from marker + 1 to cursor
                    if (index >= 0) {
                        methods.setSelection($caller, index + 1, sel.start);
                    }

                    // Replace selection with value
                    methods.replaceSelection($caller, value);

                    // Place cursor at end of inserted value
                    methods.setSelection($caller, cursor, cursor);

                }

            },
            getOrCreateMenu: function ($caller) {
                var elementId = $caller.attr("id") ? $caller.attr("id") : $caller.attr("name"),
                    id = elementId + "Suggester",
                    $menu = $("#" + id);

                // Create menu & bind events the first time the menu is created
                if ($menu.length === 0) {

                    // Create suggester menu
                    $menu = $("<div>",
                        {
                            "id": id,
                            "class": "dropdown-menu dropdown-menu-no-arrow suggester-menu col-8",
                            "role": "menu"
                        });
                    $caller.after($menu);

                    // Bind events to caller to hide suggester menu
                    $caller.bind("keydown.",
                        function (e) {
                            var $target = methods.getOrCreateMenu($(this));
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
                                                if (newIndex > pageSize - 1) {
                                                    newIndex = pageSize - 1;
                                                }
                                                break;
                                            case 27: // escape
                                                e.preventDefault();
                                                e.stopPropagation();
                                                $target.hide();
                                                break;
                                        }
                                        if (newIndex >= 0) {
                                            $target.pagedList({
                                                itemSelection: $.extend(itemSelection,
                                                    {
                                                        index: newIndex
                                                    })
                                            },
                                                "setItemIndex");
                                        }

                                    }


                                }
                            }
                        });

                    // Hide menu on click & scroll
                    $caller.bind("click scroll",
                        function () {
                            var $target = methods.getOrCreateMenu($(this));
                            if ($target) {
                                $target.hide();
                            }
                        });

                    // spy for blur (allows for a period of time before closing menu)
                    $caller.blurSpy({
                        onBlur: function ($el, e) {
                            var $target = methods.getOrCreateMenu($el);
                            if ($target) {
                                $target.hide();
                            }
                        }
                    });

                }
                return $menu;
            },
            getSelection: function ($caller) {

                var e = $caller[0];

                return (
                    'selectionStart' in e &&
                    function () {
                        var l = e.selectionEnd - e.selectionStart;
                        return {
                            start: e.selectionStart,
                            end: e.selectionEnd,
                            length: l,
                            text: e.value.substr(e.selectionStart, l)
                        };
                    } ||

                    /* browser not supported */
                    function () {
                        return null;
                    }
                )();

            },
            setSelection: function ($caller, start, end) {

                var e = $caller[0];

                return (
                    'selectionStart' in e &&
                    function () {
                        e.selectionStart = start;
                        e.selectionEnd = end;
                        return;
                    } ||

                    /* browser not supported */
                    function () {
                        return null;
                    }
                )();

            },
            replaceSelection: function ($caller, text) {

                var e = $caller[0];

                return (
                    'selectionStart' in e &&
                    function () {
                        e.value = e.value.substr(0, e.selectionStart) +
                            text +
                            e.value.substr(e.selectionEnd, e.value.length);
                        // Set cursor to the last replacement end
                        e.selectionStart = e.value.length;
                        return this;
                    } ||

                    /* browser not supported */
                    function () {
                        e.value += text;
                        return jQuery(e);
                    }
                )();
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
                    // $(selector).suggester
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
                    // $().suggester
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
    
    /* popper */
    var popper = function () {

        var dataKey = "popper",
            dataIdKey = dataKey + "Id";

        var defaults = {
            id: "popper",
            event: "mouseenter",
            position: "top", // popper position
            css: "w-500", // css class to apply to popper
            url: "" // URL to load content via XmlHttp
        };

        var methods = {
            timer: null,
            callers: [], // array of all active callers
            init: function ($caller, methodName) {
                
                if (methodName) {
                    if (this[methodName]) {
                        this[methodName].apply(this, [$caller]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return;
                }

                // Store the original position so this can be reset when the popper is hidden
                var position = methods._getPosition($caller);
                if (!$caller.data("popperOriginalPosition")) {
                    $caller.data("popperOriginalPosition", position);
                }
                
                this.bind($caller);

            },
            bind: function ($caller) {

                var event = $caller.data(dataKey).event;

                $caller.off(event).on(event,
                    function(e) {
                        if (event === "click") {
                            e.preventDefault(e);
                        }
                        methods.show($(this));
                    });
                
                if (event === "mouseenter" || event === "mouseover") {

                    $caller.leaveSpy({
                        interval: 150,
                        onLeave: function () {
                            methods.hideAll();
                        }
                    });

                    $caller.mouseleave(function () {
                        methods._clearTimer();
                    });
                }
                
            },
            show: function ($caller) {

                var delay = 250;
                
                if ($caller.data("popperDelay")) {
                    delay = $caller.data("popperDelay");
                }
                
                if (delay > 0) {
                    methods.timer = win.setTimeout(function () {
                        if (methods.timer) { showTip(); }
                    }, delay);
                } else {
                    showTip();
                }

                function showTip() {

                    var position = methods._getPosition($caller),
                        $popper = methods._getOrCreate($caller);
                 
                    if ($popper) {
                        
                        // hide all poppers
                        methods.hideAll();
                        
                        // add to stack
                        methods.callers.push($caller);

                        // Add optional Css
                        methods._addCss($caller);

                        // Load content
                        methods._load($caller, function($el) {
                        
                            // Position
                            methods._position($caller);
                            
                        });
                        
                        // onShow
                        if ($caller.data(dataKey) && $caller.data(dataKey).onShow) {
                            $caller.data(dataKey).onShow($caller, $popper);
                        }

                    }
                }

            },
            hide: function ($caller) {

                var $popper = methods._getOrCreate($caller);
                if ($popper) {
                    
                    // Remove Css
                    methods._removeCss($caller);

                    // Reset popper position
                    $caller.data("popperPosition", $caller.data("popperOriginalPosition") || "top");

                    // onHide event
                    if ($caller.data(dataKey) && $caller.data(dataKey).onHide) {
                        $caller.data(dataKey).onHide();
                    }
                    
                }

            },
            hideAll: function () {
                var len = methods.callers.length;
                if (len === 0) {
                    return;
                }
                for (var i = 0; i < len; i++) {
                    var $caller = methods.callers[i];
                    methods.hide($caller);
                    methods.callers.splice(i, 1);
                }
            },
            _load: function ($caller, func) {

                var url = $caller.data("popperUrl") || ($caller.data(dataKey) ? $caller.data(dataKey).url : null);
                if (!url) { return; }

                var $popper = methods._getOrCreate($caller);
                if ($popper) {
                    
                    // Update entity dropdown
                    app.http({
                        method: "GET",
                        url: url
                    }).done(function(response) {

                        // Populate content
                        if ($popper.length > 0) {
                            $popper.empty();
                            if (response !== "") {
                                $popper.html(response);
                            }
                        }

                        // Init tooltips within loaded content
                        app.ui.initToolTips($popper);

                        // Callback 
                        if (func) {
                            func($popper);
                        }

                        // onLoad event
                        if ($caller.data(dataKey) && $caller.data(dataKey).onLoad) {
                            $caller.data(dataKey).onLoad($caller, $popper);
                        }


                    });
                    
                }

            },
            _getOrCreate: function($caller) {

                var id = $caller.data(dataKey).id,
                    selector = "#" + id,
                    $popper = $("<div/>",
                        {
                            "id": id,
                            "class": "popper",
                            "style": "display: none;"
                        });

                // Build popper
                if ($(selector).length === 0) {
                    
                    // Add to dom
                    $("body").append($popper);
                    
                    // Bind leave events
                    $popper.leaveSpy({
                        selector: ".popper",
                        interval: 500,
                        onLeave: function () {
                            methods.hideAll();
                        }
                    });
                    
                    // Return new popper
                    return $popper;

                }

                // Return existing popper
                return $(selector);
          
            },
            _addCss: function($caller) {

                var $popper = methods._getOrCreate($caller),
                    css = $caller.data("popperCss") || $caller.data(dataKey).css,
                    position = methods._getPosition($caller);

                // Custom Css
                if (css) {
                    if (!$popper.hasClass(css)) {
                        $popper.addClass(css);
                    }
                }
                
                // Add show Css
                if (position === "top") {
                    $popper.addClass("popper-n");
                } else if (position === "bottom") {
                    $popper.addClass("popper-s");
                } else if (position === "left") {
                    $popper.addClass("popper-w");
                } else if (position === "right") {
                    $popper.addClass("popper-e");
                }
                
            },
            _removeCss: function ($caller) {

                var $popper = methods._getOrCreate($caller),
                    css = $caller.data("popperCss") || $caller.data(dataKey).css;

                $popper
                    .css({
                        "display": "none"
                    })
                    .removeClass("popper-right")
                    .removeClass("popper-n")
                    .removeClass("popper-e")
                    .removeClass("popper-s")
                    .removeClass("popper-w");
                
                // Custom Css
                if (css) {
                    if ($popper.hasClass(css)) {
                        $popper.removeClass(css);
                    }
                }
                
            },
            _position($caller) {

                var $popper = methods._getOrCreate($caller);
                if ($popper) {

                    $popper.css({
                        "display": "block",
                        "visibility": "hidden"
                    });
                    
                    // get caller coords
                    var $offset = $caller.offset(),
                        callerTop = Math.floor($offset.top),
                        callerLeft = Math.floor($offset.left),
                        callerWidth = Math.floor($caller.outerWidth()),
                        callerHeight = Math.floor($caller.outerHeight()),
                        callerRight = callerLeft + callerWidth,
                        width = $popper.outerWidth(),
                        height = $popper.outerHeight(),
                        centerX = callerLeft + Math.floor(callerWidth / 2),
                        centerY = callerTop + Math.floor(callerHeight / 2);

                    // Position popper
                    var position = methods._getPosition($caller);
                    if (position === "top") {
                        $popper.css({
                            "top": callerTop - height,
                            "left": callerLeft
                        });
                    } else if (position === "right") {
                        $popper.css({
                            "top": centerY - height / 2,
                            "left": callerLeft + callerWidth
                        });
                    } else if (position === "left") {
                        $popper.css({
                            "top": centerY - height / 2,
                            "left": callerLeft - width
                        });
                    } else if (position === "bottom") {
                        $popper.css({
                            "top": callerTop + callerHeight,
                            "left": callerLeft
                        });
                    }

                    // Checks bounds
                    var padding = 20,
                        left = parseInt($popper.css("left").split("p")[0]),
                        top = parseInt($popper.css("top").split("p")[0]),
                        right = left + width,
                        bottom = top + height,
                        winHeight = $(win).height(),
                        scrollTop = $(win).scrollTop(),
                        scrollBottom = scrollTop + winHeight,
                        aboveFold = top < scrollTop + padding,
                        belowFold = bottom > scrollBottom + padding;
                    
                    if (position === "top" || position === "bottom") {
                        
                        if (aboveFold && !belowFold) {

                            // Remove Css
                            methods._removeCss($caller);

                            // Update position
                            $caller.data("popperPosition", "bottom");

                            // Reset position
                            methods._addCss($caller);
                            methods._position($caller);

                        }

                        if (belowFold && !aboveFold) {

                            // Remove Css
                            methods._removeCss($caller);

                            // Update position
                            $caller.data("popperPosition", "top");

                            // Reset position
                            methods._addCss($caller);
                            methods._position($caller);

                        }
                            
                        if (left <= 0) {
                            $popper.css({ "left": padding });
                        } else {
                            if (right >= $(win).width()) {
                                $popper.css({ "left": callerRight - width });
                                $popper.addClass("popper-right");
                            }
                        }

                    }

                    if (position === "left" || position === "right") {
                        if (aboveFold && !belowFold) {
                            $popper.css({ "top": scrollTop + padding });
                        }
                        if (belowFold && !aboveFold) {
                            $popper.css({ "top": scrollBottom - (height + padding) });
                        }
                    }

                    $popper.css({
                        "display": "block",
                        "visibility": "visible"
                    });

                }

            },
            _getPosition: function ($caller) {
                if ($caller.data("popperPosition")) {
                    return $caller.data("popperPosition");
                }
                return $caller.data(dataKey) ? $caller.data(dataKey).position : "top";
            },
            _clearTimer: function () {
                win.clearTimeout(methods.timer);
                methods.timer = false;
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
                    // $(selector).popper()
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
                    // $().popper
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

    /* password */
    var password = function () {

        var dataKey = "password",
            dataIdKey = dataKey + "Id";

        var defaults = {
            event: "click"
        };

        var methods = {
            timer: null,
            init: function ($caller) {
                // Ensure input is set to password by default
                $caller.attr("type", "password");
                // Bind events
                this.bind($caller);
            },
            bind: function ($caller) {
                var $btn = methods._getButton($caller),
                    event = $caller.data(dataKey).event;
                if ($btn) {
                    $btn.off(event).on(event,
                        function (e) {
                            e.preventDefault();
                            methods.toggle($caller);
                        });
                }
            },
            toggle: function($caller) {
                if ($caller.attr("type") === "password") {
                    $caller.attr("type", "text");
                } else {
                    $caller.attr("type", "password");
                }
            },
            unbind: function ($caller) {
                var $btn = methods._getButton($caller),
                    event = $caller.data(dataKey).event;
                if ($btn) {
                    $btn.off(event);
                }
            },
            _getButton: function($caller) {
                var $btn = $caller.next();
                if ($btn[0].tagName === "BUTTON" || $btn[0].tagName === "A") {
                    return $btn;
                }
                return null;
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
                    // $(selector).password()
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
                    // $().password()
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

    /* loader */
    var loader = function () {

        var dataKey = "loader",
            dataIdKey = dataKey + "Id";

        var defaults = {};

        var methods = {
            timer: null,
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
            show: function ($caller) {
                if ($caller.hasClass("page-loader-hidden")) {
                    $caller.removeClass("page-loader-hidden");
                }
            },
            hide: function($caller) {
                if ($caller.hasClass("page-loader")) {
                    $caller.addClass("page-loader-hidden");
                }
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
                    // $(selector).password()
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
                    // $().password()
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

    /* loaderSpy */
    var loaderSpy = function () {

        var dataKey = "loaderSpy",
            dataIdKey = dataKey + "Id";

        var defaults = {
            event: "click"
        };

        var methods = {
            timer: null,
            init: function ($caller) {
                this.bind($caller);
            },
            bind: function ($caller) {
                var event = $caller.data(dataKey).event;
                if (event) {
                    $caller.on(event, function () {
                        // Simply return if we have a specific target
                        if ($(this).attr("target")) {
                            return;
                        }
                        // Show loader
                        $('[data-provide="loader"]').loader("show");
                    });
                }
            },
            unbind: function ($caller) {
                var event = $caller.data(dataKey).event;
                if (event) {
                    $caller.off(event);
                }
            }
        };

        return {
            init: function () {

                var options = {};

                if (this.length > 0) {
                    // $(selector).loaderSpy()
                    return this.each(function () {
                        if (!$(this).data(dataIdKey)) {
                            var id = dataKey + parseInt(Math.random() * 100) + new Date().getTime();
                            $(this).data(dataIdKey, id);
                            $(this).data(dataKey, $.extend({}, defaults, options));
                        } else {
                            $(this).data(dataKey, $.extend({}, $(this).data(dataKey), options));
                        }
                        methods.init($(this));
                    });
                } 
            }
        };

    }();
    
    /* Register Plugins */
    $.fn.extend({
        dialog: dialog.init,
        dialogSpy: dialogSpy.init,
        scrollTo: scrollTo.init,
        sticky: sticky.init,
        treeView: treeView.init,
        pagedList: pagedList.init,
        autoComplete: autoComplete.init,
        typeSpy: typeSpy.init,
        blurSpy: blurSpy.init,
        leaveSpy: leaveSpy.init,
        scrollSpy: scrollSpy.init,
        resizeSpy: resizeSpy.init,
        infiniteScroll: infiniteScroll.init,
        filterList: filterList.init,
        tagIt: tagIt.init,
        userAutoComplete: userAutoComplete.init,
        userTagIt: userTagIt.init,
        selectDropdown: selectDropdown.init,
        autoTargetBlank: autoTargetBlank.init,
        autoLinkImages: autoLinkImages.init,
        markdownBody: markdownBody.init,
        confirm: confirm.init,
        resizeable: resizeable.init,
        keyBinder: keyBinder.init,
        textFieldMirror: textFieldMirror.init,
        suggester: suggester.init,
        popper: popper.init,
        password: password.init,
        loader: loader.init,
        loaderSpy: loaderSpy.init
    });

    // ---------------------------
    // Initialize core plug-ins
    // ----------------------------
   
    $.fn.platoUI = function(opts) {
        
        /* dialogSpy */
        this.find('[data-provide="dialog"]').dialogSpy();

        /* scrollTo. */
        this.find('[data-provide="scroll"]').scrollTo();

        /* sticky */
        this.find('[data-provide="sticky"]').sticky();

        /* pagedList */
        this.find('[data-provide="paged-list"]').pagedList();

        /* select dropdown */
        this.find('[data-provide="select-dropdown"]').selectDropdown();
        
        /* treeView */
        this.find('[data-provide="tree"]').treeView();

        /* filterList */
        this.find('[data-provide="filter-list"]').filterList();

        /* autoComplete */
        this.find('[data-provide="autoComplete"]').autoComplete();

        /* userAutoComplete */
        this.find('[data-provide="userAutoComplete"]').userAutoComplete();
        
        /* tagIt */
        this.find('[data-provide="tagIt"]').tagIt();

        /* userTagIt */
        this.find('[data-provide="userTagIt"]').userTagIt();

        /* confirm */
        this.find('[data-provide="confirm"]').confirm();

        /* autoTargetBlank */
        this.find('[data-provide="autoTargetBlank"]').autoTargetBlank();

        /* autoLinkImages */
        this.find('[data-provide="autoLinkImages"]').autoLinkImages();

        /* markdownBody */
        this.find('[data-provide="markdownBody"]').markdownBody();

        /* infiniteScroll */
        this.find('[data-provide="infiniteScroll"]').infiniteScroll();

        /* resizeable */
        this.find('[data-provide="resizeable"]').resizeable();

        /* popper */
        this.find('[data-provide="popper"]').popper();

        /* password */
        this.find('[data-provide="password"]').password();

        /* loader */
        this.find('[data-provide="loader"]').loader("hide");

        /* loaderSpy */
        this.find('[data-provide="loader-spy"]').loaderSpy();

        // Bind scroll events
        $(win).scrollSpy({
            onScrollStart: function () {
                $().popper("hideAll");
            }
        });

    };

    // --------------
    // ready
    // --------------

    app.ready(function () {

        // Init plato UI
        $("body").platoUI();

        // Activate plug-ins used within infiniteScroll load
        $().infiniteScroll(function ($ele) {

            /* tooltips */
            app.ui.initToolTips($ele);
            
            /* markdownBody */
            $ele.find('[data-provide="markdownBody"]').markdownBody();

            /* dialogSpy */
            $ele.find('[data-provide="dialog"]').dialogSpy();

            /* replySpy */
            $ele.replySpy("bind");

            /* popper */
            $ele.find('[data-provide="popper"]').popper();

        }, "ready");
        
    });

}(window, document, jQuery));

// --------------
// App
// --------------

$(function (win, doc, $) {

    'use strict';

    // --------

    var app = win.$.Plato;

    // --------

    /* replySpy */
    var replySpy = function () {

        var dataKey = "replySpy",
            dataIdKey = dataKey + "Id";

        var defaults = {
            postQuoteSelector: '[data-provide="postQuote"]',
            postReplySelector: '[data-provide="postReply"]',
            onQuote: function ($caller) { },
            onReply: function ($caller) { }
        };

        var methods = {
            init: function ($caller) {
                this.bind($caller);
            },
            bind: function ($caller) {

                var postQuoteSelector = $caller.data(dataKey).postQuoteSelector,
                    postReplySelector = $caller.data(dataKey).postReplySelector;

                // Bind Quote
                if (postQuoteSelector) {
                    $caller.find(postQuoteSelector).unbind("click").bind("click",
                        function (e) {

                            e.preventDefault();

                            // Get element containing quote
                            var value = "",
                                selector = $(this).attr("data-quote-selector"),
                                $quote = $(selector);

                            // Apply locale
                            var text = app.T("In response to");

                            if ($quote.length > 0) {
                                var displayName = $quote.attr("data-display-name"),
                                    replyUrl = $quote.attr("data-reply-url");
                                value = "> " + $quote.html()
                                    .replace(/\n\r/g, "\n")
                                    .replace(/[\s]\n/g, "\n")
                                    .replace(/\n/g, "\n> ");
                                if (displayName && replyUrl) {
                                    value += "\n> ^^ " + text + " [" + displayName + "](" + replyUrl + ")";
                                }
                                value += "\n\n";
                            }

                            /* resizeable */
                            $('[data-provide="resizeable"]').resizeable("toggleVisibility", {
                                onShow: function ($resizeable) {
                                    var $textArea = $resizeable.find(".md-textarea");
                                    if ($textArea.length > 0) {
                                        $textArea.val(value);
                                        $textArea.focus();
                                    }
                                }
                            });

                            // onQuote event
                            if ($caller.data(dataKey).onQuote) {
                                $caller.data(dataKey).onQuote($caller);
                            }

                        });
                }

                // Bind Reply
                if (postReplySelector) {
                    $caller.find(postReplySelector).unbind("click").bind("click", function (e) {

                        e.preventDefault();

                        /* resizeable */
                        $('[data-provide="resizeable"]').resizeable("toggleVisibility", {
                            onShow: function ($caller) {
                                var $textArea = $caller.find(".md-textarea");
                                if ($textArea.length > 0) {
                                    $textArea.focus();
                                }
                            }
                        });

                        // onReply event
                        if ($caller.data(dataKey).onReply) {
                            $caller.data(dataKey).onReply($caller);
                        }

                    });
                }

            },
            unbind: function ($caller) {

                var postQuoteSelector = $caller.data(dataKey).postQuoteSelector,
                    postReplySelector = $caller.data(dataKey).postReplySelector;

                if (postQuoteSelector) {
                    $caller.find(postQuoteSelector).unbind("click");
                }
                if (postReplySelector) {
                    $caller.find(postReplySelector).unbind("click");
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
                    // $(selector).replySpy()
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
                    // $().replySpy()
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

    /* navSite */
    var navSite = function () {

        var dataKey = "navSite",
            dataIdKey = dataKey + "Id";

        var defaults = {
            collapsedNavSelector: "#navbar-collapse",
            toggleCollapsedNavSelector: '[data-target="#navbar-collapse"]'
        };

        var methods = {
            init: function ($caller) {
                this.bind($caller);
            },
            bind: function ($caller) {

                // Disable bootstrap tooltips within .nav-site dependent on screen size 

                var entered = false,
                    breakpoint = 992, // tooltips will be disabled at this width and below
                    $nav = $caller.find(defaults.collapsedNavSelector);

                if ($nav.length === 0) {
                    return;
                }

                $nav.bind("mouseleave",
                    function () {
                        entered = false;
                    });

                $nav.bind("mouseenter",
                    function () {
                        if (entered) {
                            return;
                        }
                        entered = true;
                        if ($(win).width() < breakpoint) {
                            // Dispose tooltips for mobile navigation
                            if ($nav.length > 0) {
                                app.ui.disposeToolTips($nav);
                            }
                        } else {
                            // Enable tooltips for desktop navigation
                            if ($nav.length > 0) {
                                app.ui.initToolTips($nav);
                            }
                        }
                    });

            },
            unbind: function ($caller) {
                var $nav = $caller.find(defaults.collapsedNavSelector);
                if ($nav.length > 0) {
                    $nav.unbind("mouseenter");
                    $nav.unbind("mouseleave");
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
                    // $(selector).navSite()
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
                    // $().navSite()
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
    
    /* layout */
    var layout = function () {

        var dataKey = "layout",
            dataIdKey = dataKey + "Id";

        var defaults = {
            stickyHeaders: true,
            stickySidebars: true
        };

        var methods = {
            init: function ($caller) {
                this.bind($caller);
            },
            bind: function ($caller) {

                // Layout elements
                var $stickyHeader = $caller.find(".layout-header-sticky"),
                    $stickySidebar = $caller.find(".layout-sidebar-sticky"),
                    $stickySidebarContent = $stickySidebar.find(".layout-sidebar-content"),
                    $body = $caller.find(".layout-body"),
                    $content = $caller.find(".layout-content"),
                    $footer = $caller.find(".layout-footer");

                // Layout options
                var sidebarOffsetTop = 0,
                    stickyHeaders = $caller.data(dataKey).stickyHeaders,
                    stickySidebars = $caller.data(dataKey).stickySidebars;

                // If we don't find our sticky elements disable flags
                if ($stickyHeader.length === 0) { stickyHeaders = false; }
                if ($stickySidebar.length === 0) { stickySidebars = false; }

                // Apply sticky headers?
                if (stickyHeaders) {

                    // Default offset for sticky sidebars
                    sidebarOffsetTop = $stickyHeader.outerHeight();

                    // Important: Set initial height of header
                    // This ensures other calculations are correct
                    $stickyHeader.css({
                        "height": sidebarOffsetTop
                    });

                    // Apply sticky headers
                    $stickyHeader.sticky({
                        onUpdate: function ($this) {
                            if ($this.hasClass("fixed")) {
                                // Ensure width matches container when element becomes fixed
                                $this.find(".layout-header-content").css({
                                    "width": $this.width()
                                });
                            } else {
                                // Reset width
                                $this.find(".layout-header-content").css({
                                    "width": "auto"
                                });
                            }
                        }
                    });

                    // Update infinite default scroll spacing 
                    // to accomodate for fixed headers
                    $().infiniteScroll({
                        scrollSpacing: sidebarOffsetTop
                    });

                }
                
                // Apply sticky sidebar?
                if (stickySidebars) {
                    
                    // Important: Accomodate for the static 
                    // content being smaller than the fixed content
                    if ($content.length > 0) {
                        // Ensure sidebar is greater than our content
                        if ($stickySidebar.height() >= $content.height()) {
                            $content.css({ "minHeight": $body.height() });
                        }
                    }
                    
                    // Apply sticky to sidebars
                    $stickySidebar.sticky({
                        offset: sidebarOffsetTop,
                        onScroll: function ($this) {
                            var top = Math.floor($footer.offset().top),
                                scrollTop = Math.floor($(win).scrollTop() + $(win).height());
                            if (scrollTop > top) {
                                $stickySidebarContent.css({
                                    "bottom": scrollTop - top
                                });
                            } else {
                                $stickySidebarContent.css({
                                    "bottom": 0
                                });
                            }
                        },
                        onUpdate: function ($this) {
                            if ($this.hasClass("fixed")) {
                                // Setup content when container becomes fixed
                                $stickySidebarContent.css({
                                    "overflowY": "auto",
                                    "top": sidebarOffsetTop,
                                    "width": $this.width()
                                });
                                // Apply overflow CSS
                                if (!$stickySidebarContent.hasClass("overflow-auto")) {
                                    $stickySidebarContent.addClass("overflow-auto");
                                }
                            } else {
                                // Reset
                                $stickySidebarContent.css({
                                    "overflowY": "visible",
                                    "top": "auto",
                                    "width": "auto"
                                });
                                // Remove overflow CSS
                                if ($stickySidebarContent.hasClass("overflow-auto")) {
                                    $stickySidebarContent.removeClass("overflow-auto");
                                }
                            }

                        }
                    });
                }

            },
            unbind: function ($caller) {
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
                    // $(selector).layout()
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
                    // $().layout()
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

    /* badgeList */
    var badgeList = function () {

        var dataKey = "badgeList",
            dataIdKey = dataKey + "Id";

        var defaults = {};

        var methods = {
            init: function ($caller) {
                this.bind($caller);
            },
            bind: function ($caller) {
                
                $caller.find(".list-group-item").on("mouseenter",
                    function() {
                        var $desc = $(this).find(".badge-description"),
                            $details = $(this).find(".badge-details");
                        $desc.hide();
                        $details.show();
                    });

                $caller.find(".list-group-item").on("mouseleave",
                    function () {
                        var $desc = $(this).find(".badge-description"),
                            $details = $(this).find(".badge-details");
                        $details.hide();
                        $desc.show();
                    });

            },
            unbind: function ($caller) {
                $caller.find(".list-group-item").off("mouseenter");
                $caller.find(".list-group-item").off("mouseleave");
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
                    // $(selector).badgeList()
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
                    // $().badgeList()
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
        replySpy: replySpy.init,
        navSite: navSite.init,
        layout: layout.init,
        badgeList: badgeList.init
    });

    // ---------------------------
    // Initialize app plug-ins
    // ----------------------------

    $.fn.appUI = function(opts) {

        /* navigation */
        this.find(".nav-site").navSite();

        /* layout */
        this.find(".layout").layout({
            stickyHeaders: opts.layout.stickyHeaders,
            stickySidebars: opts.layout.stickySidebars
        });

        /* replySpy */
        this.replySpy();
        
        /* badgeList */
        this.find('[data-provide="badge-list"]').badgeList();

        // Scroll to validation errors?
        if (opts.validation.scrollToErrors) {
            // Raised when the form is submitted but invalid
            this.find("form").bind("invalid-form.validate",
                function() {
                    // Scroll to errors if any
                    var $errors = $(this).find(".validation-summary-errors");
                    if ($errors.length > 0) {
                        $().scrollTo({
                                target: $errors,
                                offset: -20,
                                interval: 250
                            },
                            "go");
                    }
                });
        }

        // SEt-up header alerts
        var $alerts = this.find(".layout-header").find(".alert");
        if ($alerts.length > 0) {

            // Stack sticky alerts
            var initialOffset = 6,
                offset = initialOffset;
            $alerts.each(function () {
                $(this).css({ "top": offset });
                offset += $(this).outerHeight() + initialOffset;
                if (!$(this).hasClass("alert-visible")) {
                    $(this).addClass("alert-visible");
                }
            });

            // Auto close alerts?
            if (opts.alerts.autoClose) {
                win.setTimeout(function () {
                        $alerts.each(function () {
                            if (!$(this).hasClass("alert-hidden")) {
                                $(this).addClass("alert-hidden");
                            }
                        });
                        //$(".alert").alert('close');
                    },
                    opts.alerts.autoCloseDelay * 1000);
            }
        }
      
    };
    
    // --------------
    // ready
    // --------------

    app.ready(function () {

        // Init app UI
        $("body").appUI(win.$.Plato.defaults);

    });

}(window, document, jQuery));

// --------------
// Validation
// --------------

$(function (win, doc, $) {

    'use strict';

    // formAction
    // ------------------
    // Accomodate for custom "formaction" attributes added
    // when using multiple submit elements for example...
    // <button type="submit" asp-controller="Admin" asp-action="Delete" asp-route-id="@Model.Id.ToString()" data-provide="confirm" class="btn btn-danger btn-sm">
    //      <i class="fal fa-trash"></i>      
    // </button>
    // Produces the following HTML...
    // <button type="submit" formaction="/action">
    //      <i class="fal fa-trash"></i>
    //      Delete
    // </button>

    var app = win.$.Plato,
        formAction = null;

    app.ready(function () {
        // Populate formAction on any submit click
        $('*[type="submit"]').click(function () {
            var attr = $(this).attr("formaction");
            if (attr) {
                formAction = attr;
            }
        });
    });

    // Update jQuery validation defaults
    $.validator.setDefaults({
        focusInvalid: true,
        submitHandler: function(form) {

            var $form = $(form);
            if ($form.data("disableSubmit")) {
                $form.find('*[type="submit"]')
                    .addClass("disabled")
                    .attr("disabled", "disabled");
            }

            // Do we need to update the form action?
            if (formAction) {
                form.action = formAction;
                formAction = null;
            }

            // Note don't call $(form).submit() as this 
            // internally calls the validators submitHandler again
            form.submit();
            
        },
        invalidHandler: function(event, validator) {
            // Cannot be updated after MVC initialization
            // https://github.com/jquery-validation/jquery-validation/issues/765
        }
    });
    
}(window, document, jQuery));
