
if (typeof jQuery === "undefined") {
    throw new Error("Plato requires jQuery");
}

if (typeof $.Plato.Context === "undefined") {
    throw new Error("$.Plato.Context Required");
}

/* follow buttons */
$(function (win, doc, $) {

    'use strict';

    // keyBinder
    var keyBinder = function () {

        var dataKey = "keyBinder",
            dataIdKey = dataKey + "Id";

        var defaults = {
            event: "keypress",
            keys: [
                {
                    which: 35, // #
                    callback: function($input) {
                        console.log("# was pressed");
                    }
                },
                {
                    which: 64, // @
                    callback: function($input) {
                        console.log("@ was pressed");
                    }
                }
            ]
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
            bind: function($caller) {

                var key = null,
                    keys = $caller.data(dataKey).keys,
                    event = $caller.data(dataKey).event;

                $caller.bind(event, function(e) {
                    for (var i = 0; i < keys.length; i++) {
                        key = keys[i];
                        if (e.which === key.which) {
                            key.callback($(this));
                        }
                    }
                });

            },
            unbind: function ($caller) {
                $caller.unbind($caller.data(dataKey).event);
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

        }

    }();

    // textFieldMirror
    var textFieldMirror = function () {

        var dataKey = "textFieldMirror",
            dataIdKey = dataKey + "Id";

        var defaults = {
            start: 0,
            ready: function($caller) {

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

                var start = $caller.data(dataKey).start,
                    firstHalf = $caller.val().substring(0, start),
                    lastHalf = $caller.val().substring(start, $caller.val().length - 1);
                
                var marker = '<span class="text-field-mirror-marker position-relative">@</span>',
                    markerHtml = firstHalf + marker + lastHalf,
                    html = markerHtml.replace(/\n/gi, '<br>');

                var $mirror = $('<div class="form-control text-field-mirror">');
                $mirror.css({
                    "position": "absolute",
                    "backgroundColor": "red",
                    "height": $caller.height(),
                    "overflow": "auto"
                });
                $mirror.html(html);
                $caller.before($mirror);
               
                if ($caller.data(dataKey).ready) {
                    $caller.data(dataKey).ready($mirror);
                }
                
            },
            hide: function ($caller) {
               
            },
            addOrGetMirror: function($caller) {
                
            

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

        }

    }();


    // mentions
    var mentions = function () {

        var dataKey = "mentions",
            dataIdKey = dataKey + "Id";

        var defaults = {};

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
                
                // Track @ character
                $caller.keyBinder({
                    keys: [
                        {
                            which: 64, // @
                            match: /(^|\s|\()(@([a-z0-9\-_/]*))$/i,

                            callback: function ($input) {
                                methods.show($input);
                            }
                        }
                    ]
                });

                // Wrap a relative wrapper around the input to
                // correctly position the absolutely positioned mention menu
                $caller.wrap($('<div class="position-relative"></div>'));

            },
            unbind: function($caller) {
                $caller.keyBinder("unbind");
            },
            show: function($caller) {

                // ensure we have focus
                $caller.focus();

                var cursor = this.getSelection($caller);

                $caller.textFieldMirror({
                    start: cursor.start,
                    ready: function ($mirror) {

                        var $marker = $mirror.find(".text-field-mirror-marker"),
                            offset = $marker.position(),
                            menuLeft = Math.floor(offset.left),
                            menuTop = Math.floor(offset.top + 26),
                            scrollLeft = $mirror.scrollLeft(),
                            scrollTop = $mirror.scrollTop();

                        var $menu = methods.addOrGetMenu($caller);
                        $menu.css({
                            "left": menuLeft + scrollLeft + "px",
                            "top": menuTop + scrollTop + "px"
                        });

                        $caller.textFieldMirror("hide");

                        $caller.userAutoComplete({
                            target: "#" + $menu.attr("id")
                        });


                    }
                });
                
            },
            addOrGetMenu: function ($caller) {

                // Get or create menu
                var $menu = $caller.next(),
                    menuId = $caller.attr("id") + "MentionsDropDown";
                if (!$menu.hasClass("dropdown-menu")) {
                    $menu = $("<div>",
                        {
                            "id": menuId,
                            "class": "dropdown-menu col-5",
                            "role": "menu"
                        });
                    $caller.after($menu);
                }

                return $menu;


            },
            getSelection: function ($caller) {

                var e = $caller[0];

                return (

                    ('selectionStart' in e && function () {
                            var l = e.selectionEnd - e.selectionStart;
                            return {
                                start: e.selectionStart,
                                end: e.selectionEnd,
                                length: l,
                                text: e.value.substr(e.selectionStart, l)
                            };
                        }) ||

                        /* browser not supported */
                        function () {
                            return null;
                        }

                )();

            },
            setSelection: function ($caller, start, end) {

                var e = $caller[0];

                return (

                    ('selectionStart' in e && function () {
                            e.selectionStart = start;
                            e.selectionEnd = end;
                            return;
                        }) ||

                        /* browser not supported */
                        function () {
                            return null;
                        }

                )();

            },
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
                    // $(selector).mentions
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
                    // $().mentions
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
        keyBinder: keyBinder.init,
        textFieldMirror: textFieldMirror.init,
        mentions: mentions.init
    });

    $(doc).ready(function () {
        
        $('[data-provide="mentions"]').mentions();

        $('.md-textarea').mentions();

    });

}(window, document, jQuery));
