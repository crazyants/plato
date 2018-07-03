// <reference path="/wwwroot/js/app.js" />

// doc ready

$(function (win, doc, $) {

    "use strict";

    if (typeof $.Plato.Options === "undefined") {
        throw new Error("$.Plato.Discuss requires $.Plato.Options");
    }

    if (typeof $.Plato.Logger === "undefined") {
        throw new Error("$.Plato.Discuss requires $.Plato.Logger");
    }
    
    var context = {
        options: $.Plato.Options,
        logger: $.Plato.Logger
    }
    
    $(doc).ready(function () {
        //$.Plato.Discuss.init(context);
    });
    

}(window, document, jQuery));

/* --------------------*/
/* Plato Discuss */
/* --------------------*/

$.Plato.Discuss = {
    context: null,
    init: function (context) {

        this.context = context;

        context.logger.logInfo("$.Plato.Discuss initializing");

        if (!window.Vue) {
            context.logger.logError("Vue.js is required for Plato.Discuss");
        }

        var app = new Vue({
            el: '#app',
            data: {
                update: false,
                fieldTemplate: {},
                details: [],
                fields: [],
                custom: []
            },
            created: function() {
                this.refresh();
            },
            methods: {
                refresh: function(update) {
                   
                },
                // -------------------
                // events
                // -------------------
                editDefaultValueClick: function(detail, e) {

             
                },
                editFieldClick: function(fieldId, e) {

                
                },
                editLogicGroupClick: function(fieldId, e) {

                    if (e) {
                        e.preventDefault();
                    }

            

                },
                deleteFieldClick: function(fieldId, fieldTemplateDetailId, e) {

              
                },
                requiredToggle: function(id, required) {
                  
                },
                visibleToggle: function(fieldId, visible) {
                   

                }
            },
            mounted: function() { // triggers when vue istance is mounted

                var fieldTemplateList = document.getElementById("fieldTemplateList"),
                    customFieldList = document.getElementById("customFieldList"),
                    defaultFieldList = document.getElementById("defaultFieldList");


            },
            updated: function() { // triggers whenever vue data changes

                // remove dropped lis 
                var fieldTemplateList = document.getElementById("fieldTemplateList");
                var lis = fieldTemplateList.getElementsByTagName("li");
                for (var i = 0; i < lis.length; i++) {
                    var li = lis[i];
                    if (li.className) {
                        if (li.className === "ikb-admin-add-field-list-item") {
                            if (li.parent) {
                                li.parent.removeChild(li);
                            }
                        }
                    }
                }

                // ignition UI
                $("#app").tidyUI();
            }
        });

    }
}

// vue

Vue.component('field-icon', {
    template: '<img class="list-item-icon i-tooltip" v-bind:title="tooltip" v-bind:src="getImageUrl()" />',
    props: ['type', 'tooltip'],
    methods: {
        getImageUrl: function () {
            return "../skins/classic/images/admin/fieldtypes/" + this.type + ".png";
        }
    }
});