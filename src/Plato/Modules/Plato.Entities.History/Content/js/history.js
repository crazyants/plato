
if (typeof jQuery === "undefined") {
    throw new Error("History requires jQuery");
}

if (typeof $.Plato.Context === "undefined") {
    throw new Error("History require $.Plato.Context");
}

if (typeof $.Plato.Http === "undefined") {
    throw new Error("History require $.Plato.Http");
}

if (typeof $.Plato.Http === "undefined") {
    throw new Error("History require $.Plato.Ui");
}

$(function (win, doc, $) {

    'use strict';
    
    // notifications
    var history = function () {
        
        var dataKey = "history",
            dataIdKey = dataKey + "Id";

        var defaults = {
            entityId: 0,
            entityReplyId: 0
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

                var entityId = this.getEntityId($caller),
                    entityReplyId = this.getEntityReplyId($caller),
                    params = '?page={page}&size={pageSize}&entityId={entityId}&entityReplyId={entityReplyId}';

                params = params.replace(/\{entityId}/g, entityId);
                params = params.replace(/\{entityReplyId}/g, entityReplyId);
                
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
                    noResultsText: "No history at this time",
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
                        '<a data-history-id="{id}" class="{itemCss}" href="{url}"><span class="list-left"><span class="avatar avatar-sm" data-toggle="tooltip" title="{createdBy.displayName}"><span style="background-image: url({createdBy.avatar.url}"></span></span></span><span class="list-body"><span class="float-right badge badge-secondary">{version}</span>{text} - {id}</span></a>',
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

                        // createdBy

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
                        
                        // date

                        if (result.date.value) {
                            html = html.replace(/\{date.value}/g, result.date.value);
                        } else {
                            html = html.replace(/\{date.value}/g, "");
                        }

                        if (result.url) {
                            html = html.replace(/\{url}/g, result.url);
                        } else {
                            html = html.replace(/\{url}/g, "#");
                        }
                        return html;

                    },
                    onItemClick: function ($caller, result, e) {
                        e.preventDefault();
                        e.stopPropagation();
                    },
                    onLoaded: function($pagedList, results) {
                        
                        $pagedList.find(".dropdown-item").click(function (e) {

                            e.preventDefault();
                            e.stopPropagation();

                            // Ensure we have a historyId
                            var historyId = parseInt($(this).data("historyId"));
                            if (historyId === 0 || isNaN(historyId)) {
                                throw new Error("A history id is required!");
                            }

                            // Load the diff
                            $().dialog({
                                id: "historyDialog",
                                body: {
                                    url: "/discuss/history/home/" + historyId
                                },
                                css: {
                                    modal: "modal fade",
                                    dialog: "modal-dialog modal-lg"
                                }
                            }, "show");
                            
                        });

                        // Activate tooltips
                        $caller.find('[data-toggle="tooltip"]')
                            .tooltip({ trigger: "hover" });
                        
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
            getEntityReplyId: function ($caller) {
                var output = typeof $caller.data("entityReplyId") !== "undefined" && $caller.data("entityReplyId") !== null
                    ? $caller.data("entityReplyId")
                    : $caller.data(dataKey).entityReplyId;
                if (output !== null) {
                    var id = parseInt(output);
                    if (!isNaN(id)) {
                        return id;
                    }

                }
                return 0;
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
                    // $(selector).notifications
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

        }

    }();

    
    $.fn.extend({
        history: history.init
    });

    $(doc).ready(function () {

        $('[data-provide="history"]').history();
        
    });

}(window, document, jQuery));
