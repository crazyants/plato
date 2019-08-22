// <reference path="/wwwroot/js/app.js" />

if (typeof $().modal === "undefined") {
    throw new Error("BootStrap 4.1.1 or above Required");
}

if (typeof window.$.Plato === "undefined") {
    throw new Error("$.Plato Required");
}

$(function (win, doc, $) {

    "use strict";

    // --------

    /* lightBox */
    var lightBox = function (options) {

        var dataKey = "lightBox",
            dataIdKey = dataKey + "Id";

        var defaults = {
            esc: true,
            selector: "img",
            template:
                '<div class="lightbox" tabindex="-1"><div class="lightbox-wrap" data-lightbox-close="true"><div class="lightbox-loader">Loading...</div><div class="lightbox-container"><div class="lightbox-content"></div><button class="lightbox-close" type="button" title="Close (Esc)" data-lightbox-close="true">×</button></div></div></div>'
        };

        var $template = null;

        var methods = {
            init: function ($caller, methodName) {

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
            bind: function ($caller) {

                // Unbind default auto linking of images
                $caller.autoLinkImages("unbind");

                var selector = $caller.data(dataKey).selector;
                $caller.find(selector).each(function (i) {
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
                                function (e) {
                                    e.preventDefault();
                                    e.stopPropagation();
                                    methods.open($caller, $(this));
                                });
                            $(this).wrap($a);
                        }
                    }
                });

            },
            open: function ($caller, $link) {

                if (!$template) {
                    return false;
                }

                if ($template.hasClass("lightbox-closed")) {
                    $template.removeClass("lightbox-closed");
                }

                if (!$template.hasClass("lightbox-loading")) {
                    $template.addClass("lightbox-loading");
                }

                var $content = $template.find(".lightbox-content");
                if ($content.length > 0) {
                    $content.empty().append($("<img/>",
                        {
                            "src": $link.attr("href"),
                            "title": $link.attr("title") || ""
                        }));
                }

                if (!$template.hasClass("lightbox-opened")) {
                    $template.addClass("lightbox-opened");
                }


            },
            close: function ($caller) {

                if (!$template) {
                    return false;
                }

                if ($template.hasClass("lightbox-loading")) {
                    $template.removeClass("lightbox-loading");
                }

                if ($template.hasClass("lightbox-opened")) {
                    $template.removeClass("lightbox-opened");
                }

                if (!$template.hasClass("lightbox-closed")) {
                    $template.addClass("lightbox-closed");
                }

            },
            _build: function ($caller) {
                var id = $caller.data(dataKey).id,
                    template = $caller.data(dataKey).template,
                    selector = "#" + id,
                    $el = $(template,
                        {
                            "id": id
                        });
                if ($(selector).length === 0) {
                    // Bind close click
                    var closeClick = function (e) {
                        if ($(e.target).is("[data-lightbox-close]") || $(e.target).is(".lightbox-content")) {
                            methods.close();
                        }
                    };
                    $el.on("click", closeClick);
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
                    // $(selector).lightBox()
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

    // --------

    var app = win.$.Plato;
    
    app.ready(function () {
        
        // lightBox
        $('[data-provide="markdownBody"]')
            .lightBox();

        // Activate lightBox when loaded via infiniteScroll load
        $().infiniteScroll(function($ele) {
                $ele.find('[data-provide="markdownBody"]')
                    .lightBox();
            },
            "ready");

    });

}(window, document, jQuery));