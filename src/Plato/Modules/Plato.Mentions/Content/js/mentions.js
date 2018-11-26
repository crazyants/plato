
if (typeof jQuery === "undefined") {
    throw new Error("Plato requires jQuery");
}

if (typeof $.Plato.Context === "undefined") {
    throw new Error("$.Plato.Context Required");
}

$(function (win, doc, $) {

    'use strict';

    // keyBinder
    var keyBinder = function () {

        var dataKey = "keyBinder",
            dataIdKey = dataKey + "Id";

        var defaults = {
            id: "",
            event: "keyup", 
            keys: [
                {
                    match: /(^|\s|\()(@([a-z0-9\-_/]*))$/i,
                    search: function ($input) {},
                    bind: function ($input) {}
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
            bind: function($caller) {

                // Bind shuuld be called after all internalKeys have been initialized
                var key = null,
                    id = $caller.data(dataKey).id,
                    keys = $caller.data(dataKey).internalKeys,
                    event = $caller.data(dataKey).event;

                if (event == null) {
                    return;
                }

                // namespace event
                if (event.indexOf(".") === -1) {
                    if (id) {
                        event = event + "." + id;
                    }
                }

                $caller.on(event,
                    function(e) {
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
            type: "",
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
                
                var id = $caller.data(dataKey).id,
                    start = $caller.data(dataKey).start,
                    prefix = $caller.val().substring(0, start),
                    suffix = $caller.val().substring(start, $caller.val().length - 1),
                    marker = '<span class="text-field-mirror-marker position-relative">@</span>',
                    markerHtml = prefix + marker + suffix,
                    html = markerHtml.replace(/\n/gi, '<br/>');

                var $mirror = methods.getOrCreateMirror($caller);
                if ($mirror) {

                    // Populate & show mirror
                    $mirror.html(html).show();

                    // Ensure mirror is always same height as called
                    $mirror.css({
                        "height": $caller.outerHeight()
                    });

                    // Ensure mirror is always scrolled to same position as calller
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
            getOrCreateMirror: function($caller) {
                var elementid = $caller.attr("id") ? $caller.attr("id") : $caller.attr("name"),
                    id = elementid + "Mirror",
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
    
    // suggester
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
            unbind: function($caller) {
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
                        var $marker = $mirror.find(".text-field-mirror-marker"),
                            position = $marker.position(),
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
                            $menu.pagedList($caller.data(dataKey));
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
            insert: function($caller) {

                $caller.focus();

                var data = $caller.data(dataKey).insertData;
                if (data) {

                    var sel = this.getSelection($caller),
                        index = data.index, // index from which everything will be replaced upto selection.start
                        value = data.value + " ", // add a space after the value we are inserting
                        cursor = (index + 1) + value.length; // position at end of inserted value

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
                var elementid = $caller.attr("id") ? $caller.attr("id") : $caller.attr("name"),
                    id = elementid + "Suggester",
                    $menu = $("#" + id);

                // Create menu & bind events the first time the menu is created
                if ($menu.length === 0) {
                    
                    // Create suggester menu
                    $menu = $("<div>",
                        {
                            "id": id,
                            "class": "dropdown-menu suggester-menu col-6",
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
            replaceSelection: function ($caller, text) {

                var e = $caller[0];

                return (

                    ('selectionStart' in e && function () {
                            e.value = e.value.substr(0, e.selectionStart) + text + e.value.substr(e.selectionEnd, e.value.length);
                            // Set cursor to the last replacement end
                            e.selectionStart = e.value.length;
                            return this;
                        }) ||

                        /* browser not supported */
                        function () {
                            e.value += text;
                            return jQuery(e);
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

        }

    }();

    // ---------------------

    // mentions
    var mentions = function () {

        var dataKey = "mentions",
            dataIdKey = dataKey + "Id";

        var defaults = {};

        var methods = {
            init: function($caller, methodName) {

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
            bind: function($caller) {
                $caller.suggester($.extend($caller.data(dataKey)),
                    {
                        // keyBinder options
                        keys: [
                            {
                                match: /(^|\s|\()(@([a-z0-9\-_/]*))$/i,
                                search: function($input, selection) {

                                    // The result of the search method is tested
                                    // against the match regular expiression within keyBinder
                                    // If a match is found the bind method is called 
                                    // otherwise the unbind method is called
                                    // This code executes on every key press so should be optimized
                                    
                                    var chars = $input.val().split(""),
                                        value = null,
                                        marker = "@",
                                        startIndex = -1,
                                        start = selection.start - 1,
                                        i;
                                    
                                    // Search backwards from caret for marker, until 
                                    // terminators & attempt to get marker position
                                    for (i = start; i >= 0; i--) {
                                        if (chars[i] === marker) {
                                            startIndex = i;
                                            break;
                                        } else {
                                            if (chars[i] === "\n" || chars[i] === " ") {
                                                break;
                                            }
                                        }
                                    }

                                    
                                    // If we have a marker search forward from
                                    // marker until terminator to get value
                                    if (startIndex >= 0) {
                                        value = "";
                                        for (i = startIndex; i <= chars.length - 1; i++) {
                                            if (chars[i] === "\n" || chars[i] === " ") {
                                                break;
                                            }
                                            value += chars[i];
                                        }
                                    }

                                    console.log("men startIndex: " + startIndex)
                                    console.log("men value: " + value)

                                    return {
                                        startIndex: startIndex,
                                        value: value
                                    };

                                },
                                bind: function ($input, searchResult, e) {

                                    var keywords = searchResult.value;
                                    if (keywords == null) {
                                        return;
                                    }

                                    console.log("bind mentions");

                                    // Remove any marker from search keywords
                                    if (keywords.substring(0, 1) === "@") {
                                        keywords = keywords.substring(1, keywords.length);
                                    }
                                   
                                    // Invoke suggester
                                    $caller.suggester({
                                            // pagedList options
                                            page: 1,
                                            pageSize: 5,
                                            itemSelection: {
                                                enable: true,
                                                index: 0,
                                                css: "active"
                                            },
                                            valueField: "keywords",
                                            config: {
                                                method: "GET",
                                                url:
                                                    'api/users/get?page={page}&size={pageSize}&keywords=' +
                                                        encodeURIComponent(keywords),
                                                data: {
                                                    sort: "LastLoginDate",
                                                    order: "Desc"
                                                }
                                            },
                                            itemCss: "dropdown-item",
                                            itemTemplate: '<a class="{itemCss}" href="{url}"><span class="avatar avatar-sm mr-2"><span style="background-image: url(/users/photo/{id});"></span></span>{displayName}<span class="float-right">@{userName}</span></a>',
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
                                            onPagerClick: function ($self, page, e)
                                            {
                                                e.preventDefault();
                                                e.stopPropagation();
                                                $caller.suggester({
                                                        page: page
                                                    },
                                                    "show");
                                            },
                                            onItemClick: function($self, result, e) {
                                              
                                                // Prevent default event
                                                e.preventDefault();
                                              
                                                // Focus input, hide suggest menu & insert result
                                                $caller
                                                    .focus()
                                                    .suggester("hide")
                                                    .suggester({
                                                            insertData: {
                                                                index: searchResult.startIndex,
                                                                value: result.userName
                                                            }
                                                        },
                                                        "insert");

                                            }
                                        },
                                        "show");

                                },
                                unbind: function ($input, key, e) {
                                    $caller.suggester("hide");
                                }
                            }
                        ]
                    }, defaults);
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

    // references
    var references = function () {

        var dataKey = "references",
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
            
                $caller.suggester($.extend($caller.data(dataKey)),
                    {
                        // keyBinder options
                        keys: [
                            {
                                match: /(^|\s|\()(#([a-z0-9\-_/]*))$/i,
                                search: function ($input, selection) {

                                    // The result of the search method is tested
                                    // against the match regular expiression within keyBinder
                                    // If a match is found the bind method is called 
                                    // otherwise unbind method is called
                                    // This code executes on everykey press so should be optimized

                                    var chars = $input.val().split(""),
                                        value = null,
                                        marker = "#",
                                        startIndex = -1,
                                        start = selection.start - 1,
                                        i;

                                    // Search backwards from caret for marker, until 
                                    // terminators & attempt to get marker position
                                    for (i = start; i >= 0; i--) {
                                        if (chars[i] === marker) {
                                            startIndex = i;
                                            break;
                                        } else {
                                            if (chars[i] === "\n" || chars[i] === " ") {
                                                break;
                                            }
                                        }
                                    }
                                    
                                    // If we have a marker search forward from
                                    // marker until terminator to get value
                                    if (startIndex >= 0) {
                                        value = "";
                                        for (i = startIndex; i <= chars.length - 1; i++) {
                                            if (chars[i] === "\n" || chars[i] === " ") {
                                                break;
                                            }
                                            value += chars[i];
                                        }
                                    }

                                    console.log("ref value: " + value);
                                    console.log("ref startIndex: " + startIndex);

                                    return {
                                        startIndex: startIndex,
                                        value: value
                                    };

                                },
                                bind: function ($input, searchResult, e) {

                                    var keywords = searchResult.value;
                                    if (keywords == null) {
                                        return;
                                    }
                             
                                    // Remove any marker prefix from search keywords
                                    if (keywords.substring(0, 1) === "#") {
                                        keywords = keywords.substring(1, keywords.length);
                                    }

                                    // Invoke suggester
                                    $caller.suggester({
                                        // pagedList options
                                        page: 1,
                                        pageSize: 5,
                                        itemSelection: {
                                            enable: true,
                                            index: 0,
                                            css: "active"
                                        },
                                        valueField: "keywords",
                                        config: {
                                            method: "GET",
                                            url:
                                                'api/users/get?page={page}&size={pageSize}&keywords=' +
                                                encodeURIComponent(keywords),
                                            data: {
                                                sort: "LastLoginDate",
                                                order: "Desc"
                                            }
                                        },
                                        itemCss: "dropdown-item",
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
                                        onPagerClick: function ($self, page, e) {
                                            e.preventDefault();
                                            e.stopPropagation();
                                            $caller.suggester({
                                                    page: page
                                                },
                                                "show");
                                        },
                                        onItemClick: function ($self, result, e) {

                                            // Prevent default event
                                            e.preventDefault();

                                            // Focus input, hide suggest menu & insert result
                                            $caller
                                                .focus()
                                                .suggester("hide")
                                                .suggester({
                                                    insertData: {
                                                        index: searchResult.startIndex,
                                                        value: result.userName
                                                    }
                                                },
                                                    "insert");

                                        }
                                    },
                                        "show");

                                },
                                unbind: function ($input, key, e) {
                                    $caller.suggester("hide");
                                }
                            }
                        ]
                    }, defaults);
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
                    // $(selector).references
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
                    // $().references
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
        suggester: suggester.init,
        mentions: mentions.init,
        references: references.init
    });

    $(doc).ready(function () {
        
        //$('[data-provide="mentions"]').mentions();

        // Build suggesters
        $('.md-textarea').mentions();
        $('.md-textarea').references();

        // bind suggesters
        $('.md-textarea').keyBinder("bind");

    });

}(window, document, jQuery));
