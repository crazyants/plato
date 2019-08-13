// <reference path="/wwwroot/js/app.js" />

if (typeof $().modal === "undefined") {
    throw new Error("BootStrap 4.1.1 or above Required");
}

if (typeof window.$.Plato === "undefined") {
    throw new Error("$.Plato Required");
}


/* lity */
(function (window, factory) {
    if (typeof define === "function" && define.amd) {
        define(["jquery"], function ($) {
            return factory(window, $);
        });
    } else if (typeof module === "object" && typeof module.exports === "object") {
        module.exports = factory(window, require("jquery"));
    } else {
        window.lity = factory(window, window.jQuery || window.Zepto);
    }
}(window, function (window, $) {
    "use strict";

    var document = window.document;

    var _win = $(window);
    var _html = $("html");
    var _instanceCount = 0;

    var _imageRegexp = /\.(php|png|jpe?g|gif|svg|webp|bmp|ico|tiff?)(\?\S*)?$/i;
    var _youtubeRegex = /(youtube(-nocookie)?\.com|youtu\.be)\/(watch\?v=|v\/|u\/|embed\/?)?([\w-]{11})(.*)?/i;
    var _vimeoRegex = /(vimeo(pro)?.com)\/(?:[^\d]+)?(\d+)\??(.*)?$/;
    var _googlemapsRegex = /((maps|www)\.)?google\.([^\/\?]+)\/?((maps\/?)?\?)(.*)/i;

    var _defaultHandlers = {
        image: imageHandler,
        inline: inlineHandler,
        iframe: iframeHandler
    };

    var _defaultOptions = {
        esc: true,
        handler: null,
        template: '<div class="lity" tabindex="-1"><div class="lity-wrap" data-lity-close><div class="lity-loader">Loading...</div><div class="lity-container"><div class="lity-content"></div><button class="lity-close" type="button" title="Close (Esc)" data-lity-close>×</button></div></div></div>'
    };

    function globalToggle() {
        _html[_instanceCount > 0 ? "addClass" : "removeClass"]("lity-active");
    }

    var transitionEndEvent = (function () {
        var el = document.createElement("div");

        var transEndEventNames = {
            WebkitTransition: "webkitTransitionEnd",
            MozTransition: "transitionend",
            OTransition: "oTransitionEnd otransitionend",
            transition: "transitionend"
        };

        for (var name in transEndEventNames) {
            if (el.style[name] !== undefined) {
                return transEndEventNames[name];
            }
        }

        return false;
    })();

    function transitionEnd(element) {
        var deferred = $.Deferred();

        if (!transitionEndEvent) {
            deferred.resolve();
        } else {
            element.one(transitionEndEvent, deferred.resolve);
            setTimeout(deferred.resolve, 500);
        }

        return deferred.promise();
    }

    function settings(currSettings, key, value) {
        if (arguments.length === 1) {
            return $.extend({}, currSettings);
        }

        if (typeof key === "string") {
            if (typeof value === "undefined") {
                return typeof currSettings[key] === "undefined" ?
                    null :
                    currSettings[key];
            }
            currSettings[key] = value;
        } else {
            $.extend(currSettings, key);
        }

        return this;
    }

    function protocol() {
        return "file:" === window.location.protocol ? "http:" : "";
    }

    function parseQueryParams(params) {
        var pairs = decodeURI(params).split("&");
        var obj = {}, p;

        for (var i = 0, n = pairs.length; i < n; i++) {
            if (!pairs[i]) {
                continue;
            }

            p = pairs[i].split("=");
            obj[p[0]] = p[1];
        }

        return obj;
    }

    function appendQueryParams(url, params) {
        return url + (url.indexOf("?") > -1 ? "&" : "?") + $.param(params);
    }

    function error(msg) {
        return $('<span class="lity-error"/>').append(msg);
    }

    function imageHandler(el) {

        var target = el.data("lity-target") || el.attr("href") || el.attr("src");
        var className = el.find("img").attr("class");
        if (!_imageRegexp.test(target)) {
            return false;
        }
        
        var css = className !== "" ? ' class="' + className + '"' : "";
        var img = $("<img" + css + ' src="' + target + '">');
        var deferred = $.Deferred();
        var failed = function () {
            deferred.reject(error("Failed loading image"));
        };

        img
            .on("load", function () {
                if (this.naturalWidth === 0) {
                    return failed();
                }

                deferred.resolve(img);
            })
            .on("error", failed);

        return deferred.promise();
    }

    function inlineHandler(target) {
        var el;

        try {
            el = $(target);
        } catch (e) {
            return false;
        }

        if (!el.length) {
            return false;
        }

        var placeholder = $('<span class="lity-inline-placeholder"/>');

        return el
            .after(placeholder)
            .on("lity:ready", function (e, instance) {
                instance.one("lity:remove", function () {
                    placeholder
                        .before(el)
                        .remove();
                });
            })
            ;
    }

    function iframeHandler(target) {
        var matches, url = target;

        matches = _youtubeRegex.exec(target);

        if (matches) {
            url = appendQueryParams(
                protocol() + "//www.youtube" + (matches[2] || "") + ".com/embed/" + matches[4],
                $.extend(
                    {
                        autoplay: 1
                    },
                    parseQueryParams(matches[5] || "")
                )
            );
        }

        matches = _vimeoRegex.exec(target);

        if (matches) {
            url = appendQueryParams(
                protocol() + "//player.vimeo.com/video/" + matches[3],
                $.extend(
                    {
                        autoplay: 1
                    },
                    parseQueryParams(matches[4] || "")
                )
            );
        }

        matches = _googlemapsRegex.exec(target);

        if (matches) {
            url = appendQueryParams(
                protocol() + "//www.google." + matches[3] + "/maps?" + matches[6],
                {
                    output: matches[6].indexOf("layer=c") > 0 ? "svembed" : "embed"
                }
            );
        }

        return '<div class="lity-iframe-container"><iframe frameborder="0" allowfullscreen src="' + url + '"></iframe></div>';
    }

    function lity(options) {
        var _options = {},
            _handlers = {},
            _instance,
            _content,
            _ready = $.Deferred().resolve();

        function keyup(e) {
            if (e.keyCode === 27) {
                close();
            }
        }

        function resize() {
            var height = document.documentElement.clientHeight ? document.documentElement.clientHeight : Math.round(_win.height()),
                width = document.documentElement.clientWidth ? document.documentElement.clientWidth : Math.round(_win.width());
            $(".lity-content")
                .css({ 'max-height': Math.floor(height - 75) + "px", 'max-width': Math.floor(width - 75) + "px" })
                .trigger("lity:resize", [_instance, popup]);

            //$(".crop-frame")
            //    .css({ 'max-height': Math.floor(height - 75) + "px", 'max-width': Math.floor(width - 75) + "px" })
                ;
        }

        function ready(content) {
            if (!_instance) {
                return;
            }

            _content = $(content);

            _win.on("resize", function () {
                resize();
            });
            resize();

            _instance
                .find(".lity-loader")
                .each(function () {
                    var el = $(this);
                    transitionEnd(el).always(function () {
                        el.remove();
                    });
                })
                ;

            _instance
                .removeClass("lity-loading")
                .find(".lity-content")
                .empty()
                .append(_content);

            _content
                .removeClass("lity-hide")
                .trigger("lity:ready", [_instance, popup]);

            _ready.resolve();

        }

        function init(handler, content, options) {
            _instanceCount++;
            globalToggle();

            _instance = $(options.template)
                .addClass("lity-loading")
                .appendTo("body");

            if (options.esc) {
                _win.one("keyup", keyup);
            }

            setTimeout(function () {
                _instance
                    .addClass("lity-opened lity-" + handler)
                    .on("click", "[data-lity-close]", function (e) {
                        if ($(e.target).is("[data-lity-close]")) {
                            close();
                        }
                    })
                    .trigger("lity:open", [_instance, popup])
                    ;

                $.when(content).always(ready);
            }, 0);
        }

        function open(el, options) {
            var handler, content, handlers = $.extend({}, _defaultHandlers, _handlers);

            if (options.handler && handlers[options.handler]) {
                content = handlers[options.handler](el, instance, popup);
                handler = options.handler;
            } else {
                var lateHandlers = {};

                // Run inline and iframe handlers after all other handlers
                $.each(["inline", "iframe"], function (i, name) {
                    if (handlers[name]) {
                        lateHandlers[name] = handlers[name];
                    }

                    delete handlers[name];
                });

                var call = function (name, callback) {
                    // Handler might be "removed" by setting callback to null
                    if (!callback) {
                        return true;
                    }

                    content = callback(el, popup);

                    if (content) {
                        handler = name;
                        return false;
                    }
                };

                $.each(handlers, call);

                if (!handler) {
                    $.each(lateHandlers, call);
                }
            }

            if (content) {
                _ready = $.Deferred();
                $.when(close()).done($.proxy(init, null, handler, content, options));
            }

            return !!content;
        }

        function close() {
            if (!_instance) {
                return false;
            }

            var deferred = $.Deferred();

            _ready.done(function () {
                _instanceCount--;
                globalToggle();

                _win
                    .off("resize", resize)
                    .off("keyup", keyup);

                _content.trigger("lity:close", [_instance, popup]);

                _instance
                    .removeClass("lity-opened")
                    .addClass("lity-closed")
                    ;

                var instance = _instance, content = _content;
                _instance = null;
                _content = null;

                transitionEnd(content.add(instance)).always(function () {
                    content.trigger("lity:remove", [instance, popup]);
                    instance.remove();
                    deferred.resolve();
                });
            });

            return deferred.promise();
        }

        function popup(event) {
            // If not an event, act as alias of popup.open
            if (!event.preventDefault) {
                return popup.open(event);
            }

            var el = $(this);
            var target = el.data("lity-target") || el.attr("href") || el.attr("src");

            if (!target) {
                return;
            }

            var options = $.extend(
                {},
                _defaultOptions,
                _options,
                el.data("lity-options") || el.data("lity")
            );

            if (open(el, options)) {
                event.preventDefault();
            }
        }

        popup.handlers = $.proxy(settings, popup, _handlers);
        popup.options = $.proxy(settings, popup, _options);

        popup.open = function (el) {
            open(el, $.extend({}, _defaultOptions, _options));
            return popup;
        };

        popup.close = function () {
            close();
            return popup;
        };

        return popup.options(options);
    }

    lity.version = "@VERSION";
    lity.handlers = $.proxy(settings, lity, _defaultHandlers);
    lity.options = $.proxy(settings, lity, _defaultOptions);

    $(document).on("click", "[data-lity]", lity());

    return lity;
}));


$(function (win, doc, $) {

    "use strict";

    // --------

    var app = win.$.Plato;

    // --------

    /* lightBox */
    var lightBox = function(options) {

        var dataKey = "lightBox",
            dataIdKey = dataKey + "Id";

        var defaults = {
            esc: true,
            selector: "img",
            template:
                '<div class="lity" tabindex="-1"><div class="lity-wrap" data-lity-close><div class="lity-loader">Loading...</div><div class="lity-container"><div class="lity-content"></div><button class="lity-close" type="button" title="Close (Esc)" data-lity-close>×</button></div></div></div>'
        };

        var $template = null;

        var methods = {
            init: function($caller, methodName) {


                // Hydrate $template
                this._build($caller);

                // Method calls
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

            },
            bind: function($caller) {

                // Iterate images to auto link
                var selector = $caller.data(dataKey).selector;
                $caller.find(selector).each(function(i) {
                    // Ensure we have a src attribute to link to
                    var src = $(this).attr("src");
                    if (src) {
                        var $parent = $(this).parent();
                        var parentTag = $parent.prop("tagName");
                        if (parentTag && parentTag !== "A") {
                            var $a = $("<a/>",
                                {
                                    "href": $(this).attr("src"),
                                    "title": $(this).attr("alt") || "",
                                    "target": "_blank"
                                });
                            $a.on("click",
                                function(e) {
                                    e.preventDefault();
                                    e.stopPropagation();
                                    methods.open($(this).attr("href"));
                                });
                            $(this).wrap($a);
                        }
                    }
                });

            },
            open: function(src) {

                if (!$template) {
                    return false;
                }

                if ($template.hasClass("lity-closed")) {
                    $template.removeClass("lity-closed");
                }

                if (!$template.hasClass("lity-loading")) {
                    $template.addClass("lity-loading");
                }

                if (!$template.hasClass("lity-opened")) {
                    $template.addClass("lity-opened");
                }

                var $content = $template.find(".lity-content");
                if ($content.length > 0) {
                    var $img = $("<img/>",
                        {
                            "src": src
                        });

                    $img.load = function() {
                        console.log("image loaded");
                    };

                    $content.empty().append($img);
                }

            },
            close: function($caller) {

                if (!$template) {
                    return false;
                }

                if ($template.hasClass("lity-loading")) {
                    $template.removeClass("lity-loading");
                }

                if ($template.hasClass("lity-opened")) {
                    $template.removeClass("lity-opened");
                }

                if (!$template.hasClass("lity-closed")) {
                    $template.addClass("lity-closed");
                }

            },
            _build: function($caller) {
                var id = $caller.data(dataKey).id,
                    template = $caller.data(dataKey).template,
                    selector = "#" + id,
                    $el = $(template,
                        {
                            "id": id
                        });
                if ($(selector).length === 0) {

                    // Bind close click
                    $el.on("click",
                        function(e) {
                            if ($(e.target).is("[data-lity-close]") || $(e.target).is(".lity-container")) {
                                methods.close();
                            }
                        });

                    // Bind escape key
                    if ($caller.data(dataKey).esc) {
                        $(win).on("keyup", function (e) {
                            if (e.keyCode === 27) {
                                methods.close();
                            }
                        });
                    }
                
                    // Add to dom
                    $("body").append($el);
                    // Populate local variable
                    $template = $el;
                }
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
                    // $(selector).lightBox()
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
                    // $().lightBox()
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
        lightBox: lightBox.init
    });

    app.ready(function () {

        // Unbind default auto linking of images
        $('[data-provide="markdownBody"]')
            .autoLinkImages("unbind");

        // Add lightbox
        $('[data-provide="markdownBody"]')
            .lightBox();

    });

}(window, document, jQuery));