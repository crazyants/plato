
if (typeof jQuery === "undefined") {
    throw new Error("Plato requires jQuery");
}

if (typeof $.Plato.Context === "undefined") {
    throw new Error("$.Plato.Context Required");
}

/* markdown */
$(function (win, doc, $) {

    'use strict';
    
    var markdown = function () {

        var dataKey = "markdownEditor",
            dataIdKey = dataKey + "Id";

        var defaults = {
            event: "show.bs.tab"
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
            getUniqueId: function ($caller) {

                return parseInt($caller.attr("data-markdown-id")) || 0;
            },
            bind: function ($caller) {
                
                var event = $caller.data(dataKey).event;
                if (event) {
                    var $tabs = $caller.find('a[data-toggle="tab"]');
                    $tabs.on(event,
                        function (e) {
                            methods.showTab($caller, e);
                        });
                }

            },
            showTab: function($caller, e) {
                
                var id = this.getUniqueId($caller);
                var $editor = $caller.find("textarea");
                
                var tabId = e.target.href.split("#")[1];
                switch (tabId) {
                    case "write_" + id:
                        win.setTimeout(function() {
                                $editor.focus();
                            },
                            200);

                        break;

                    case "preview_" + id:

                        if ($editor.val().trim() === "") {
                            e.preventDefault();
                            $editor.focus();
                            return;
                        }

                        this.getHtml({
                                markdown: $editor.val()
                            },
                            function(data) {
                                if (data.statusCode === 200) {
                                    $caller
                                        .find("#preview_" + id)
                                        .empty()
                                        .html(data.html);
                                }
                            });

                        break;
                    }

            },
            getHtml: function (params, fn) {

                win.$.Plato.Http({
                    url: "api/markdown/parse/post",
                    method: "POST",
                    async: false,
                    data: JSON.stringify(params)
                }).done(function (data) {
                    fn(data);
                });

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

        }

    }();

    $.fn.extend({
        markdownEditor: markdown.init
    });
    
    $(doc).ready(function () {

        $('[data-provide="markdown-container"]')
            .markdownEditor();
     
    });

}(window, document, jQuery));
