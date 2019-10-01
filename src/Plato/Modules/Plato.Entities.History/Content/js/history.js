// <reference path="/wwwroot/js/app.js" />

if (typeof $().modal === 'undefined') {
    throw new Error("BootStrap 4.1.1 or above Required");
}

if (typeof window.$.Plato === "undefined") {
    throw new Error("$.Plato Required");
}

$(function (win, doc, $) {

    'use strict';

    // --------

    var app = win.$.Plato;

    // --------

    // history list
    var history = function () {
        
        var dataKey = "history",
            dataIdKey = dataKey + "Id";

        var defaults = {
            entityId: 0,
            entityReplyId: 0,
            dialogUrl: null
        };

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

                var entityId = this.getEntityId($caller),
                    entityReplyId = this.getEntityReplyId($caller),
                    dialogUrl = this.getDialogUrl($caller),
                    params = '?page={page}&size={pageSize}&entityId={entityId}&entityReplyId={entityReplyId}';

                // We need a dialog url
                if (dialogUrl === null) {
                    throw new Error("The history menu requires a data-dialog-url attribute");
                }

                params = params.replace(/\{entityId}/g, entityId);
                params = params.replace(/\{entityReplyId}/g, entityReplyId);

                // No results text
                var noResultsText = app.T("No history available");

                // Invoke suggester
                $caller.pagedList({
                    page: 1,
                    pageSize: 50,
                    enablePaging: true,
                    loaderTemplate: null,
                    itemSelection: {
                        enable: false,
                        index: 0,
                        css: "active"
                    },
                    noResultsIcon: null,
                    noResultsText: noResultsText,
                    config: {
                        method: "GET",
                        url: 'api/history/entity/get' + params,
                        data: {
                            sort: "CreatedDate",
                            order: 1
                        }
                    },
                    itemCss: "dropdown-item p-2",
                    itemTemplate:
                        '<a data-history-id="{id}" class="{itemCss}" href="{url}"><span class="list-left list-left-sm"><span class="avatar avatar-sm" data-toggle="tooltip" title="{createdBy.displayName}"><span style="background-image: url({createdBy.avatar.url}"></span></span></span><span class="list-body"><span class="float-right">{version}</span>{text}</span></a>',
                    parseItemTemplate: function(html, result) {

                        if (result.id) {
                            html = html.replace(/\{id}/g, result.id);
                        } else {
                            html = html.replace(/\{id}/g, "0");
                        }

                        if (result.text) {
                            html = html.replace(/\{text}/g, result.text);
                        } else {
                            html = html.replace(/\{text}/g, "");
                        }

                        if (result.version) {
                            html = html.replace(/\{version}/g, result.version);
                        } else {
                            html = html.replace(/\{version}/g, "");
                        }
                      
                        if (result.createdBy.id) {
                            html = html.replace(/\{createdBy.id}/g, result.createdBy.id);
                        } else {
                            html = html.replace(/\{createdBy.id}/g, "");
                        }

                        if (result.createdBy.displayName) {
                            html = html.replace(/\{createdBy.displayName}/g, result.createdBy.displayName);
                        } else {
                            html = html.replace(/\{createdBy.displayName}/g, "");
                        }

                        if (result.createdBy.url) {
                            html = html.replace(/\{createdBy.url}/g, result.createdBy.url);
                        } else {
                            html = html.replace(/\{createdBy.url}/g, "");
                        }

                        if (result.createdBy.avatar.url) {
                            html = html.replace(/\{createdBy.avatar.url}/g, result.createdBy.avatar.url);
                        } else {
                            html = html.replace(/\{createdBy.avatar.url}/g, "");
                        }

                        if (result.date.value) {
                            html = html.replace(/\{date.value}/g, result.date.value);
                        } else {
                            html = html.replace(/\{date.value}/g, "");
                        }

                        // Build url to version dialog
                        html = html.replace(/\{url}/g, dialogUrl.replace("0", result.id));
                      
                        return html;

                    },
                    onItemClick: function ($caller, result, e, $item) {
                        
                        e.preventDefault();
                        e.stopPropagation();
                                               
                        // Load the diff
                        $().dialog({
                                id: "historyDialog",
                                body: {
                                    url: $item.attr("href")
                                },
                                css: {
                                    modal: "modal fade",
                                    dialog: "modal-dialog modal-lg"
                                }
                            },
                            "show");

                    },
                    onLoaded: function ($pagedList, results) {
                        // Enable tooltips
                        app.ui.initToolTips($caller);
                    }
                });

            },
            update: function($caller) {
                $caller.pagedList("unbind");
            },
            getEntityId: function($caller) {
                var output = typeof $caller.data("entityId") !== "undefined" && $caller.data("entityId") !== null
                    ? $caller.data("entityId")
                    : $caller.data(dataKey).entityId;
                if (output !== null) {
                    var id = parseInt(output);
                    if (!isNaN(id)) {
                        return id;
                    }

                }
                return 0;
            },
            getEntityReplyId: function($caller) {
                var output = typeof $caller.data("entityReplyId") !== "undefined" &&
                    $caller.data("entityReplyId") !== null
                    ? $caller.data("entityReplyId")
                    : $caller.data(dataKey).entityReplyId;
                if (output !== null) {
                    var id = parseInt(output);
                    if (!isNaN(id)) {
                        return id;
                    }

                }
                return 0;
            },
            getDialogUrl: function ($caller) {
                return typeof $caller.data("dialogUrl") !== "undefined" && $caller.data("dialogUrl") !== null
                    ? $caller.data("dialogUrl")
                    : $caller.data(dataKey).dialogUrl;                
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
                    // $(selector).notifications
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
                    // $().notifications
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

    // history dropdown - simply populates history on dropdown click
    var historyDropdown = function () {

        var dataKey = "historyDropdown",
            dataIdKey = dataKey + "Id";

        var defaults = {};

        var methods = {
            init: function($caller) {
                this.bind($caller);
            },
            bind: function($caller) {
                $caller.find(".dropdown-toggle").click(function(e) {
                    $caller.find('[data-provide="history"]').history();
                });
            }
        };

        return {
            init: function() {
                var options = {};
                // $(selector).historyDropdown
                return this.each(function() {
                    if (!$(this).data(dataIdKey)) {
                        var id = dataKey + parseInt(Math.random() * 1000) + new Date().getTime();
                        $(this).data(dataIdKey, id);
                        $(this).data(dataKey, $.extend({}, defaults, options));
                    } else {
                        $(this).data(dataKey, $.extend({}, $(this).data(dataKey), options));
                    }
                    methods.init($(this));
                });
            }

        };

    }();
    
    $.fn.extend({
        history: history.init,
        historyDropdown: historyDropdown.init
    });

    // Initial load
    app.ready(function () {

        $('[data-provide="history-dropdown"]')
            .historyDropdown();

    });

    // infiniteScroll load
    $().infiniteScroll(function ($ele) {
        $ele.find('[data-provide="history-dropdown"]').historyDropdown();
    }, "ready");

}(window, document, jQuery));
