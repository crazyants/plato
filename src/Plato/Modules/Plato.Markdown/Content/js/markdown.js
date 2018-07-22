/* ===================================================
 * Modified Fork by InstantASP of bootstrap-markdown.js v2.10.0
 * http://github.com/toopay/bootstrap-markdown
 * =================================================== */

(function(factory) {
  if (typeof define === "function" && define.amd) {
    // RequireJS
    define(["jquery"], factory);
  } else if (typeof exports === 'object') {
    // Backbone.js
      factory(require('jquery'), window);
  } else {
    // jQuery plugin
    factory(jQuery, window);
  }
}(function($, win) {
  "use strict";

  /* MARKDOWN CLASS DEFINITION
   * ========================== */

  var Markdown = function(element, options) {
    // @TODO : remove this BC on next major release
    // @see : https://github.com/toopay/bootstrap-markdown/issues/109
    var opts = ['autofocus', 'savable', 'hideable', 'width',
      'height', 'resize', 'iconlibrary', 'language',
      'footer', 'fullscreen', 'hiddenButtons', 'disabledButtons'
    ];
    $.each(opts, function(_, opt) {
      if (typeof $(element).data(opt) !== 'undefined') {
        options = typeof options == 'object' ? options : {};
        options[opt] = $(element).data(opt);
      }
    });
    // End BC

    // Class Properties
    this.$ns = 'i-markdown';
    this.$element = $(element);
    this.$editable = {
      el: null,
      type: null,
      attrKeys: [],
      attrValues: [],
      content: null
    };
    this.$options = $.extend(true, {}, $.fn.markdown.defaults, options, this.$element.data('options'));
    this.$oldContent = null;
    this.$isPreview = false;
    this.$isFullscreen = false;
    this.$editor = null;
    this.$textarea = null;
    this.$handler = [];
    this.$callback = [];
    this.$nextTab = [];

    this.showEditor();
  };

  Markdown.prototype = {

    constructor: Markdown,
    __alterButtons: function(name, alter) {
      var handler = this.$handler,
        isAll = (name === 'all'),
        that = this;

      $.each(handler, function(k, v) {
        var halt = true;
        if (isAll) {
          halt = false;
        } else {
          halt = v.indexOf(name) < 0;
        }

        if (halt === false) {
          alter(that.$editor.find('button[data-handler="' + v + '"]'));
        }
      });
    },
    __buildButtons: function(buttonsArray, container) {

        var i,
            ns = this.$ns,
            handler = this.$handler,
            callback = this.$callback;

        for (i = 0; i < buttonsArray.length; i++) {

            // Build each group container
            var y, btnGroups = buttonsArray[i];
            for (y = 0; y < btnGroups.length; y++) {

                // Build each button group
                var z,
                    buttons = btnGroups[y].data,
                    $group = $('<div />',
                        {
                            'class': btnGroups[y].css,
                            'role': 'group'
                        });

                for (z = 0; z < buttons.length; z++) {

                    var button = buttons[z],
                        buttonHandler = ns + '-' + button.name,
                        buttonIcon = this.__getIcon(button),
                        btnText = button.btnText ? button.btnText : '',
                        btnClass = button.btnClass ? button.btnClass : 'btn',
                        tabIndex = button.tabIndex ? button.tabIndex : '-1',
                        hotkey = typeof button.hotkey !== 'undefined' ? button.hotkey : '',
                        hotkeyCaption = typeof jQuery.hotkeys !== 'undefined' && hotkey !== ''
                            ? ' (' + hotkey + ')'
                            : '',
                        dropdown = typeof button.dropdown !== 'undefined' ? button.dropdown : null;

                    var $button = $('<button data-toggle="tooltip">');
                    $button.html(this.__localize(btnText))
                        .addClass('btn')
                        .addClass(btnClass);
                    if (btnClass.match(/btn\-(primary|success|info|warning|danger|link)/)) {
                        $button.removeClass('btn-secondary');
                    }

                    $button.attr({
                        'title': this.__localize(button.title) + hotkeyCaption,
                        'tabindex': tabIndex,
                        'data-hotkey': hotkey,
                        'data-provider': ns,
                        'data-handler': buttonHandler
                    });


                    if (dropdown) {
                        $button.addClass("dropdown-toggle");
                        $button.attr('data-toggle', 'dropdown');
                    }

                    if (button.toggle === true) {
                        $button.attr('data-toggle', 'button');
                    }

                    if (buttonIcon) {

                        var buttonIconContainer = $('<i>');
                        buttonIconContainer.addClass(buttonIcon);
                        buttonIconContainer.prependTo($button);
                    }

                    // dropdown
                    if (dropdown) {

                        var $dropdown = $('<div class="btn-group" role="group">');

                        var $ul = $('<ul class="dropdown-menu">');
                        if (dropdown.css) {
                            $ul.addClass(dropdown.css);
                        }

                        $ul.css({
                            "width": dropdown.width || "250ox"
                        });

                        if (dropdown.height) {
                            $ul.css({
                                "max-height": dropdown.height,
                                "overflow-y": "auto"
                            });
                        }

                        var $li = null;

                        if (dropdown.items != null) {

                            for (var x = 0; x < dropdown.items.length; x++) {

                                var item = dropdown.items[x],
                                    itemHandler = ns + '-' + item.name,
                                    itemHotkey = item.hotkey || "",
                                    itemValue = item.value || "";

                                var $a = $('<a href="#" class="dropdown-item">');
                                $a.html(item.text)
                                    .attr({
                                        'title': item.tooltip ? this.__localize(item.tooltip) + itemHotkey : "",
                                        'data-provider': ns,
                                        'data-handler': itemHandler,
                                        'data-hotkey': hotkey,
                                        'data-value': itemValue
                                    });

                                $li = $('<li>');
                                $li.append($a);
                                $ul.append($li);

                                // register item handler and callback
                                handler.push(itemHandler);
                                callback.push(item.callback);

                            }

                        } else {

                            $li = $("<li>");
                            $li.append(dropdown.html.replace("{baseUrl}", this.$options.baseUrl));
                            $ul.append($li);

                        }


                        $dropdown.append($button);
                        $dropdown.append('<div class="dropdown-arrow"></div>');
                        $dropdown.append($ul);

                        $group.append($dropdown);

                    } else {

                        $group.append($button);

                    }

                    // register button handler and callback
                    handler.push(buttonHandler);
                    callback.push(button.callback);

                }

                // add the button group into container DOM
                container.append($group);
            }
        }


        container.find("button").tooltip();

        return container;

    },
    __setListener: function() {
      // Set size and resizable Properties
    
      if (this.$options.resize) {
        this.$textarea.css('resize', this.$options.resize);
      }

      // Re-attach markdown data
      this.$textarea.data('markdown', this);
    },
    __setEventListeners: function() {
      this.$textarea.on({
        'focus': $.proxy(this.focus, this),
        'keyup': $.proxy(this.keyup, this),
        'change': $.proxy(this.change, this),
        'select': $.proxy(this.select, this)
      });

      if (this.eventSupported('keydown')) {
        this.$textarea.on('keydown', $.proxy(this.keydown, this));
      }

      if (this.eventSupported('keypress')) {
        this.$textarea.on('keypress', $.proxy(this.keypress, this));
      }
    },
    __handle: function(e) {
      var target = $(e.currentTarget),
        handler = this.$handler,
        callback = this.$callback,
        handlerName = target.attr('data-handler'),
        callbackIndex = handler.indexOf(handlerName),
        callbackHandler = callback[callbackIndex];

      // Trigger the focusin
      $(e.currentTarget).focus();

      callbackHandler(this, target);

      // Trigger onChange for each button handle
      this.change(this);

      // Unless it was the save handler,
      // focusin the textarea
      if (handlerName.indexOf('cmdSave') < 0) {
        this.$textarea.focus();
      }

      e.preventDefault();
    },
    __localize: function(string) {
      var messages = $.fn.markdown.messages,
        language = this.$options.language;
      if (
        typeof messages !== 'undefined' &&
        typeof messages[language] !== 'undefined' &&
        typeof messages[language][string] !== 'undefined'
      ) {
        return messages[language][string];
      }
      return string;
    },
    __getIcon: function(src) {
      if(typeof src == 'object'){
        var customIcon = this.$options.customIcons[src.name];
        return typeof customIcon == 'undefined' ? (src.icon ? src.icon[this.$options.iconlibrary] : null) : customIcon;
      } else {
        return src;
      }
    },
    setFullscreen: function (mode) {

          var $editor = this.$editor,
              $textarea = this.$textarea,
              height = $textarea.height();

        if (mode === true) {

            $editor.addClass('md-fullscreen-mode');
            $('body').addClass('md-nooverflow');
            this.$options.onFullscreen(this);

            // Ensure textarea takes up 100%
            $textarea.css({
                "height": '100%'
            });

            // Store original height
            $textarea.data("height", height);

            $textarea.focus();

            if (this.$isPreview === true) {
                var $preview = $editor.find('.md-preview');
                $preview.find("pre").css({ "max-width": $editor.width() });
                $preview.find("table").css({ "max-width": $editor.width() });
            }

        } else {

            $editor.removeClass('md-fullscreen-mode');
            $('body').removeClass('md-nooverflow');
            this.$options.onFullscreenExit(this);

            $textarea.focus();

            // Apply original height
            if ($textarea.data("height")) {
                $textarea.css({ "height": $textarea.data("height") + "px" });
            }

            if (this.$isPreview === true) {
                this.hidePreview()
                    .showPreview();
            }

        }

        this.$isFullscreen = mode;
  

    },
    showEditor: function() {

        var instance = this,
            textarea,
            ns = this.$ns,
            container = this.$element,
            originalHeigth = container.css('height'),
            originalWidth = container.css('width'),
            editable = this.$editable,
            handler = this.$handler,
            callback = this.$callback,
            options = this.$options,
            editor = $('<div/>',
                {
                    'class': 'md-editor',
                    click: function() {
                        instance.focus();
                    }
                });

        // Prepare the editor
        if (this.$editor === null) {


            var editorId = (new Date()).getTime();

            // Create the panel
            var editorHeader = $('<div/>',
                {
                    'class': 'md-header'
                });

            var editorHeaderRow = $('<div/>',
                {
                    'class': 'md-header-row'
                });
            
            var editorHeaderLeft = $('<div/>',
                {
                    'class': 'float-left'
                });

            var editorHeaderRight = $('<div/>',
                {
                    'class': 'float-right'
                });

            var editorTabs = $("<ul/>",
                {
                    'class': 'nav nav-tabs',
                    'roles': 'tablist'

                });
            
            var editorButtons = $('<div/>',
                {
                    'class': 'btn-toolbar text-right'
                });
            
            var writeLink = $("<a/>",
                {
                    'class': 'nav-link active',
                    'href': '#writeTab' + editorId,
                    'text': 'Write',
                    'role': 'tab',
                    'id': 'writeLink' + editorId
                });

            writeLink.on("click", function(e) {
                e.preventDefault();
                instance.hidePreview();
            });

            var writeTab = $("<li/>",
                {
                    'class': 'nav-item'

                });

            writeTab.append(writeLink);

            var previewLink = $("<a/>",
                {
                    'class': 'nav-link',
                    'href': '#previewTab' + editorId,
                    'text': 'Preview',
                    'role': 'tab',
                    'id': 'previewLink' + editorId
                });

            previewLink.on("click", function (e) {
                e.preventDefault();
                instance.showPreview();
            });

            var previewTab = $("<li/>",
                {
                    'class': 'nav-item'

                });
            previewTab.append(previewLink);

            editorTabs.append(writeTab);
            editorTabs.append(previewTab);
            
            var tabBody = $("<div/>",
                {
                    'class': 'tab-content d-block'
                });
            
            var writeTabBody = $("<div/>",
                {
                    'class': 'tab-pane show active md-write',
                    'role': 'tabpanel',
                    'id': 'writeTab' + editorId
                });

            var previewTabBody = $("<div/>",
                {
                    'class': 'tab-pane md-preview',
                    'role': 'tabpanel',
                    'id': 'previewTab' + editorId
                });
            
            // Merge the main & additional button groups together
            var allBtnGroups = [];
            if (options.buttons.length > 0) allBtnGroups = allBtnGroups.concat(options.buttons[0]);
            if (options.additionalButtons.length > 0) {
                // iterate the additional button groups
                $.each(options.additionalButtons[0],
                    function(idx, buttonGroup) {

                        // see if the group name of the additional group matches an existing group
                        var matchingGroups = $.grep(allBtnGroups,
                            function(allButtonGroup, allIdx) {
                                return allButtonGroup.name === buttonGroup.name;
                            });

                        // if it matches add the additional buttons to that group, if not just add it to the all buttons group
                        if (matchingGroups.length > 0) {
                            matchingGroups[0].data = matchingGroups[0].data.concat(buttonGroup.data);
                        } else {
                            allBtnGroups.push(options.additionalButtons[0][idx]);
                        }

                    });
            }

            // Reduce and/or reorder the button groups
            if (options.reorderButtonGroups.length > 0) {
                allBtnGroups = allBtnGroups
                    .filter(function(btnGroup) {
                        return options.reorderButtonGroups.indexOf(btnGroup.name) > -1;
                    })
                    .sort(function(a, b) {
                        if (options.reorderButtonGroups.indexOf(a.name) < options.reorderButtonGroups.indexOf(b.name))
                            return -1;
                        if (options.reorderButtonGroups.indexOf(a.name) > options.reorderButtonGroups.indexOf(b.name))
                            return 1;
                        return 0;
                    });
            }
            
            // Build the buttons
            if (allBtnGroups.length > 0) {
                editorButtons = this.__buildButtons([allBtnGroups], editorButtons);
            }

            if (options.fullscreen.enable) {
                var fullScreenToolTip = this.__localize('Full Screen');
                editorButtons
                    .append(
                        '<div class="btn-group"><button class="btn md-control-fullscreen" data-tooltip-position="bottom" title="' +
                        fullScreenToolTip +
                        '"><i class="' +
                        this.__getIcon(options.fullscreen.icons.fullscreenOn) +
                        '"></i></button></div>')
                    .on('click',
                        '.md-control-fullscreen',
                        function(e) {
                            e.preventDefault();
                            instance.setFullscreen(true);
                        });
            }
            if (options.fullscreen.enable && options.fullscreen !== false) {
                editorButtons.append('<div class="btn-group md-fullscreen-controls">' +
                        '<div class="btn-group"><button class="btn md-exit-fullscreen" title="Exit fullscreen"><i class="' +
                        this.__getIcon(options.fullscreen.icons.fullscreenOff) +
                        '"></i></button></div>' +
                        '</div>')
                    .on('click',
                        '.md-exit-fullscreen',
                        function(e) {
                            e.preventDefault();
                            instance.setFullscreen(false);
                        });
            }

            // Build header
            editorHeaderLeft.append(editorTabs);
            editorHeaderRight.append(editorButtons);
            editorHeaderRow.append(editorHeaderLeft);
            editorHeaderRow.append(editorHeaderRight);
            editor.append(editorHeaderRow);
            
            // Wrap the textarea
            if (container.is('textarea')) {

                container.before(editor);
                textarea = container;

                writeTabBody.append(textarea);
                previewTabBody.append($("<div/>"));

                tabBody.append(writeTabBody);
                tabBody.append(previewTabBody);

                editor.append(tabBody);

            } else {
                var rawContent = (typeof toMarkdown == 'function') ? toMarkdown(container.html()) : container.html(),
                    currentContent = $.trim(rawContent);

                // This is some arbitrary content that could be edited
                textarea = $('<textarea/>',
                    {
                        'class': 'i-input md-textarea',
                        'val': currentContent
                    });

                editor.append(textarea);

                // Save the editable
                editable.el = container;
                editable.type = container.prop('tagName').toLowerCase();
                editable.content = container.html();

                $(container[0].attributes).each(function() {
                    editable.attrKeys.push(this.nodeName);
                    editable.attrValues.push(this.nodeValue);
                });

                // Set editor to block the original container
                container.replaceWith(editor);
            }

            var editorFooter = $('<div/>',
                    {
                        'class': 'md-footer'
                    }),
                createFooter = false,
                footer = '';
            // Create the footer if savable
            if (options.savable) {
                createFooter = true;
                var saveHandler = 'cmdSave';

                // Register handler and callback
                handler.push(saveHandler);
                callback.push(options.onSave);

                editorFooter.append('<a class="btn btn-success" data-provider="' +
                    ns +
                    '" data-handler="' +
                    saveHandler +
                    '"><i class="fal fa-plus"></i> ' +
                    this.__localize('Save') +
                    '</button>');

            }

            footer = typeof options.footer === 'function' ? options.footer(this) : options.footer;

            if ($.trim(footer) !== '') {
                createFooter = true;
                editorFooter.append(footer);
            }

            if (createFooter) editor.append(editorFooter);

            // Set width
            if (options.width && options.width !== 'inherit') {
                if (jQuery.isNumeric(options.width)) {
                    editor.css('display', 'table');
                    textarea.css('width', options.width + 'px');
                } else {
                    editor.addClass(options.width);
                }
            }

            // Set height
            if (options.height && options.height !== 'inherit') {
                if ($.isNumeric(options.height)) {
                    var height = options.height;
                    if (editorHeader) height = Math.max(0, height - editorHeader.outerHeight());
                    if (editorFooter) height = Math.max(0, height - editorFooter.outerHeight());
                    textarea.css('height', height + 'px');
                } else {
                    editor.addClass(options.height);
                }
            }

            // Reference
            this.$editor = editor;
            this.$textarea = textarea;
            this.$editable = editable;
            this.$oldContent = this.getContent();

            this.__setListener();
            this.__setEventListeners();

            // Set editor attributes, data short-hand API and listener
            this.$editor.attr('id', editorId);
            this.$editor.on('click', '[data-provider="i-markdown"]', $.proxy(this.__handle, this));
            
            if (this.$element.is(':disabled') || this.$element.is('[readonly]')) {
                this.$editor.addClass('md-editor-disabled');
                this.disableButtons('all');
            }

            if (this.eventSupported('keydown') && typeof jQuery.hotkeys === 'object') {
                editorHeader.find('[data-provider="markdown"]').each(function() {
                    var $button = $(this),
                        hotkey = $button.attr('data-hotkey');
                    if (hotkey.toLowerCase() !== '') {
                        textarea.bind('keydown',
                            hotkey,
                            function() {
                                $button.trigger('click');
                                return false;
                            });
                    }
                });
            }

            if (options.initialstate === 'preview') {
                this.showPreview();
            } else if (options.initialstate === 'fullscreen' && options.fullscreen.enable) {
                this.setFullscreen(true);
            }

            // grow and shrink accordingly
            var maxRows = options.maxRows,
                minRows = options.minRows;

            var hasRows = typeof this.$textarea.attr('rows') !== 'undefined',
                startRows = hasRows ? this.$textarea.attr('rows') : maxRows;
            
            this.$textarea.on('keyup keypress paste',
                function() {
                    if (!$("body").first().attr("data-page-is-dirty")) {
                        $("body").first().attr("data-page-is-dirty", true);
                    }
                    //initRows($(this));
                });

            this.$textarea.on('focus',
                function() {
                    //initRows($(this));
                });

            function initRows($textArea) {

                // No need to adjust rows in full screen as the editor
                // takes up 100% of the screen space
                if (instance.$isFullscreen) {
                    return;
                }

                var lineHeight = 26,
                    rows = startRows || minRows;

                if ($textArea.val() !== "") {
                    rows = $textArea.val().split("\n").length;
                }
                rows = Math.max(rows || minRows);
                if (rows > maxRows) {
                    rows = maxRows;
                }
                if (rows < minRows) {
                    rows = minRows;
                }
                if (rows < startRows) {
                    rows = startRows;
                }

                $textArea.attr("rows", null);

                if (rows >= maxRows) {
                    $textArea.css({ "height": lineHeight * rows + "px" });
                } else {
                    $textArea.css({ "height": lineHeight * rows + "px" });
                }

            }

            //initRows(this.$textarea);

        } else {

            this.$editor.show();

        }

        if (options.autofocus) {
            this.$textarea.focus();
            this.$editor.addClass('active');
        }

        // hide hidden buttons from options
        this.hideButtons(options.hiddenButtons);

        // disable disabled buttons from options
        this.disableButtons(options.disabledButtons);

        // enable dropZone if available and configured
        if (options.dropZoneOptions) {
            if (this.$editor.dropzone) {
                if (!options.dropZoneOptions.init) {

                    options.dropZoneOptions.init = function() {

                        var caretPos = 0, allowedUploadExtensions = options.allowedUploadExtensions;
                        this.on('addedfile',
                            function(file) {

                                caretPos = textarea.prop('selectionStart');

                                var fileName = file.upload.filename;
                                var allowed = false;
                                if (fileName && allowedUploadExtensions) {
                                    var bits = fileName.split(".");
                                    var fileExtension = bits[bits.length - 1];
                                    for (var i = 0; i < allowedUploadExtensions.length; i++) {
                                        var allowedExtension = allowedUploadExtensions[i].ext;
                                        if (fileExtension === allowedExtension) {
                                            allowed = true;
                                        }
                                    }
                                }

                                if (allowed === false) {
                                    alert("File type is not allowed");
                                    this.removeFile(file);
                                    return false;
                                }

                                return true;

                            });

                        this.on('drop',
                            function(e) {
                                caretPos = textarea.prop('selectionStart');
                            });
                        this.on('success',
                            function(file, results) {

                                if (results) {
                                    for (var i = 0; i < results.length; i++) {
                                        var result = results[i];
                                        var text = textarea.val();
                                        textarea.val(text.substring(0, caretPos) +
                                            '\n![' +
                                            result.name +
                                            '](' +
                                            result.url +
                                            ')\n' +
                                            text.substring(caretPos));

                                    }
                                }


                            });
                        this.on('error',
                            function(file, error, xhr) {
                                console.log('Error:', error);
                            });
                    };
                }
                this.$editor.addClass('dropzone');
                this.$editor.dropzone(options.dropZoneOptions);
            } else {
                console.log('dropZoneOptions was configured, but DropZone was not detected.');
            }
        }

        // paste handler
        this.$editor.on('paste',
            function(event) {
                var items = (event.clipboardData || event.originalEvent.clipboardData).items;
                for (var index in items) {
                    var item = items[index];
                    if (item.kind === 'file') {
                        // add file to dropzone instance
                        this.dropzone.addFile(item.getAsFile());
                    }
                }
            });

        // enable data-uris via drag and drop
        if (options.enableDropDataUri === true) {
            this.$editor.on('drop',
                function(e) {
                    var caretPos = textarea.prop('selectionStart');
                    e.stopPropagation();
                    e.preventDefault();
                    $.each(e.originalEvent.dataTransfer.files,
                        function(index, file) {
                            var fileReader = new FileReader();
                            fileReader.onload = (function(file) {
                                var type = file.type.split('/')[0];
                                return function(e) {
                                    var text = textarea.val();
                                    if (type === 'image')
                                        textarea.val(text.substring(0, caretPos) +
                                            '\n<img src="' +
                                            e.target.result +
                                            '" />\n' +
                                            text.substring(caretPos));
                                    else
                                        textarea.val(text.substring(0, caretPos) +
                                            '\n<a href="' +
                                            e.target.result +
                                            '">Download ' +
                                            file.name +
                                            '</a>\n' +
                                            text.substring(caretPos));
                                };
                            })(file);
                            fileReader.readAsDataURL(file);
                        });
                });
        }

        // Trigger the onShow hook
        options.onShow(this);

        return this;

    },
    parseContent: function(val) {
        var content;

        // parse with supported markdown parser
        val = val || this.$textarea.val();

        if (this.$options.parser) {
            content = this.$options.parser(val);
        } else if (typeof markdown == 'object') {
            content = markdown.toHTML(val);
        } else if (typeof marked == 'function') {
            content = marked(val);
        } else {
            content = val;
        }

        return content;
    },
    showPreview: function() {

        if (this.$isPreview === true) {
            // Avoid sequenced element creation on misused scenario
            // @see https://github.com/toopay/bootstrap-markdown/issues/170
            return this;
        }
        
        // Give flag that tells the editor to enter preview mode
        this.$isPreview = true;

        var $editor = this.$editor,
            $textarea = this.$textarea,
            $writeTab = $("#writeTab" + $editor.attr("id")),
            $writeLink = $("#writeLink" + $editor.attr("id")),
            $previewTab = $("#previewTab" + $editor.attr("id")),
            $previewLink = $("#previewLink" + $editor.attr("id"));
        
        if ($textarea.val().trim() === "") {
            $textarea.focus();
            //return false;
        }

        win.$.Plato.Http({
            url: "api/markdown/parse/post",
            method: "POST",
            async: false,
            data: JSON.stringify({
                markdown: $textarea.val()
            })
        }).done(function (data) {

            // Update tabs
            $writeLink.removeClass("active");
            $writeTab.hide();

            $previewLink.addClass("active");
            $previewTab.show();


            if (data.statusCode === 200) {
                $previewTab
                    .empty()
                    .html(data.html);
            }


        });



  
        
        //var options = this.$options,
        //  container = this.$textarea,
        //  width = container.width(),
        //  afterContainer = container.next(),
        //  replacementContainer = $('<div/>', {
        //    'class': 'md-preview',
        //    'data-provider': 'markdown-preview'
        //  }),
        //  content,
        //  callbackContent;
        
        // Disable all buttons
        this.disableButtons('all');

        // Hide dropzone message
        this.$editor.find(".dz-message").hide();


        //// Try to get the content from callback
        //callbackContent = options.onPreview(this, replacementContainer);

        //// Set the content based on the callback content if string, otherwise parse value from textarea
        //content = typeof callbackContent == 'string' ? callbackContent : this.parseContent();

        //// Build preview element
        //replacementContainer.html(content);

        //// constrain pre and tables to preview width
        //replacementContainer.find("pre").css({ "max-width": width });
        //replacementContainer.find("table").css({ "max-width": width });

        //if (afterContainer && afterContainer.attr('class') == 'md-footer') {
        //  // If there is footer element, insert the preview container before it
        //  replacementContainer.insertBefore(afterContainer);
        //} else {
        //  // Otherwise, just append it after textarea
        //  container.parent().append(replacementContainer);
        //}

        //// Set the preview element dimensions
        //replacementContainer.css({
        //  "min-height": container.outerHeight() + 'px',
        //  "height": "auto"
        //});

        //if (this.$options.resize) {
        //  replacementContainer.css('resize', this.$options.resize);
        //}

        //// Hide the last-active textarea
        //container.hide();

        //// Attach the editor instances
        //replacementContainer.data('markdown', this);

        if (this.$element.is(':disabled') || this.$element.is('[readonly]')) {
          this.$editor.addClass('md-editor-disabled');
          this.disableButtons('all');
        }

        return this;
    },
    hidePreview: function () {

        // Give flag that tells the editor to quit preview mode
        this.$isPreview = false;

        var $editor = this.$editor,
            $writeTab = $("#writeTab" + $editor.attr("id")),
            $writeLink = $("#writeLink" + $editor.attr("id")),
            $previewTab = $("#previewTab" + $editor.attr("id")),
            $previewLink = $("#previewLink" + $editor.attr("id"));

        $writeLink.addClass("active");
        $writeTab.show();

        $previewLink.removeClass("active");
        $previewTab.hide();
        
        // Enable all buttons
        this.enableButtons('all');

        // Disable configured disabled buttons
        this.disableButtons(this.$options.disabledButtons);

        // Show dropzone message
        this.$editor.find(".dz-message").show();

        // Perform any callbacks
        this.$options.onPreviewEnd(this);

        //// Back to the editor
        //this.$textarea.show();
        //this.__setListener();

        return this;

    },
    isDirty: function() {
      return this.$oldContent != this.getContent();
    },
    getContent: function() {
      return this.$textarea.val();
    },
    setContent: function(content) {
      this.$textarea.val(content);

      return this;
    },
    findSelection: function(chunk) {
      var content = this.getContent(),
        startChunkPosition;

      if (startChunkPosition = content.indexOf(chunk), startChunkPosition >= 0 && chunk.length > 0) {
        var oldSelection = this.getSelection(),
          selection;

        this.setSelection(startChunkPosition, startChunkPosition + chunk.length);
        selection = this.getSelection();

        this.setSelection(oldSelection.start, oldSelection.end);

        return selection;
      } else {
        return null;
      }
    },
    getSelection: function() {

      var e = this.$textarea[0];

      return (

        ('selectionStart' in e && function() {
          var l = e.selectionEnd - e.selectionStart;
          return {
            start: e.selectionStart,
            end: e.selectionEnd,
            length: l,
            text: e.value.substr(e.selectionStart, l)
          };
        }) ||

        /* browser not supported */
        function() {
          return null;
        }

      )();

    },
    setSelection: function(start, end) {

      var e = this.$textarea[0];

      return (

        ('selectionStart' in e && function() {
          e.selectionStart = start;
          e.selectionEnd = end;
          return;
        }) ||

        /* browser not supported */
        function() {
          return null;
        }

      )();

    },
    replaceSelection: function(text) {

      var e = this.$textarea[0];

      return (

        ('selectionStart' in e && function() {
          e.value = e.value.substr(0, e.selectionStart) + text + e.value.substr(e.selectionEnd, e.value.length);
          // Set cursor to the last replacement end
          e.selectionStart = e.value.length;
          return this;
        }) ||

        /* browser not supported */
        function() {
          e.value += text;
          return jQuery(e);
        }

      )();
    },
    getNextTab: function() {
      // Shift the nextTab
      if (this.$nextTab.length === 0) {
        return null;
      } else {
        var nextTab, tab = this.$nextTab.shift();

        if (typeof tab == 'function') {
          nextTab = tab();
        } else if (typeof tab == 'object' && tab.length > 0) {
          nextTab = tab;
        }

        return nextTab;
      }
    },
    setNextTab: function(start, end) {
      // Push new selection into nextTab collections
      if (typeof start == 'string') {
        var that = this;
        this.$nextTab.push(function() {
          return that.findSelection(start);
        });
      } else if (typeof start == 'number' && typeof end == 'number') {
        var oldSelection = this.getSelection();

        this.setSelection(start, end);
        this.$nextTab.push(this.getSelection());

        this.setSelection(oldSelection.start, oldSelection.end);
      }

      return;
    },
    __parseButtonNameParam: function(names) {
      return typeof names == 'string' ?
        names.split(' ') :
        names;

    },
    enableButtons: function(name) {
      var buttons = this.__parseButtonNameParam(name),
        that = this;

      $.each(buttons, function(i, v) {
        that.__alterButtons(buttons[i], function(el) {
          el.removeClass('disabled');
        });
      });

      return this;
    },
    disableButtons: function(name) {
      var buttons = this.__parseButtonNameParam(name),
        that = this;

      $.each(buttons, function(i, v) {
        that.__alterButtons(buttons[i], function(el) {
          el.addClass("disabled");
        });
      });

      return this;
    },
    hideButtons: function(name) {
      var buttons = this.__parseButtonNameParam(name),
        that = this;
      $.each(buttons, function(i, v) {
          that.__alterButtons(buttons[i], function (el) {
          el.addClass('hidden');
        });
      });
      return this;
    },
    showButtons: function(name) {
      var buttons = this.__parseButtonNameParam(name),
        that = this;
      $.each(buttons, function(i, v) {
        that.__alterButtons(buttons[i], function(el) {
          el.removeClass('hidden');
        });
      });
      return this;
    },
    eventSupported: function(eventName) {
      var isSupported = eventName in this.$element;
      if (!isSupported) {
        this.$element.setAttribute(eventName, 'return;');
        isSupported = typeof this.$element[eventName] === 'function';
      }
      return isSupported;
    },
    keyup: function(e) {
      var blocked = false;
      switch (e.keyCode) {
        case 40: // down arrow
        case 38: // up arrow
        case 16: // shift
        case 17: // ctrl
        case 18: // alt
          break;

        case 9: // tab
          var nextTab;
          if (nextTab = this.getNextTab(), nextTab !== null) {
            // Get the nextTab if exists
            var that = this;
            setTimeout(function() {
              that.setSelection(nextTab.start, nextTab.end);
            }, 500);

            blocked = true;
          } else {
            // The next tab's memory contains nothing...
            // check the cursor position to determine tab action
            var cursor = this.getSelection();

            if (cursor.start == cursor.end && cursor.end == this.getContent().length) {
              // The cursor has reached the end of the content
              blocked = false;
            } else {
              // Put the cursor to the end
              this.setSelection(this.getContent().length, this.getContent().length);

              blocked = true;
            }
          }

          break;

        case 13: // enter
          blocked = false;
          var chars = this.getContent().split('');
          var enterIndex = this.getSelection().start;
          var priorNewlineIndex = -1; // initial line break at before index 0

          // traverse backwards through chars to check if last line break was num/bullet item
          for (var i = enterIndex - 2; i >= 0; i--) {
            if (chars[i] === '\n') {
              priorNewlineIndex = i;
              break;
            }
          }

          var charFollowingLastLineBreak = chars[priorNewlineIndex + 1];
          if (charFollowingLastLineBreak === '-') {
            this.addBullet(enterIndex);
          } else if ($.isNumeric(charFollowingLastLineBreak)) {
              var numBullet = this.getBulletNumber(priorNewlineIndex + 1);
              if (numBullet) {
                this.addNumberedBullet(enterIndex, numBullet);
              }
          }
          break;

        case 27: // escape
          if (this.$isFullscreen) this.setFullscreen(false);
          blocked = false;
          break;

        default:
          blocked = false;
      }

      if (blocked) {
        e.stopPropagation();
        e.preventDefault();
      }

      this.$options.onChange(this);
    },
    insertContent: function(index, content) {
      var firstHalf = this.getContent().slice(0, index);
      var secondHalf = this.getContent().slice(index + 1);
      this.setContent(firstHalf.concat(content).concat(secondHalf));
    },
    addBullet: function(index) {
      this.insertContent(index, '- \n');
      this.setSelection(index + 2, index + 2); // Put the cursor after the bullet
    },
    addNumberedBullet: function(index, num) {
      var numBullet = (num + 1) + '. \n';
      this.insertContent(index, numBullet);

      var prefixLength = num.toString().length + 2;
      this.setSelection(index + prefixLength, index + prefixLength); // Put the cursor after the number
    },
    getBulletNumber: function(startIndex) {
      var bulletNum = this.getContent().slice(startIndex).split('.')[0];
      return $.isNumeric(bulletNum) ? parseInt(bulletNum) : null;
    },
    change: function(e) {
      this.$options.onChange(this);
      return this;
    },
    select: function(e) {
      this.$options.onSelect(this);
      return this;
    },
    focus: function(e) {
      var options = this.$options,
        isHideable = options.hideable,
        editor = this.$editor;

      editor.addClass('active');

      // Blur other markdown(s)
      $(document).find('.md-editor').each(function() {
        if ($(this).attr('id') !== editor.attr('id')) {
          var attachedMarkdown;

          if (attachedMarkdown = $(this).find('textarea').data('markdown'),
            attachedMarkdown === null) {
            attachedMarkdown = $(this).find('div[data-provider="markdown-preview"]').data('markdown');
          }

          if (attachedMarkdown) {
            attachedMarkdown.blur();
          }
        }
      });

      // Trigger the onFocus hook
      options.onFocus(this);

      return this;
    },
    blur: function(e) {
      var options = this.$options,
        isHideable = options.hideable,
        editor = this.$editor,
        editable = this.$editable;

      if (editor.hasClass('active') || this.$element.parent().length === 0) {
        editor.removeClass('active');

        if (isHideable) {
          // Check for editable elements
          if (editable.el !== null) {
            // Build the original element
            var oldElement = $('<' + editable.type + '/>'),
              content = this.getContent(),
              currentContent = this.parseContent(content);

            $(editable.attrKeys).each(function(k, v) {
              oldElement.attr(editable.attrKeys[k], editable.attrValues[k]);
            });

            // Get the editor content
            oldElement.html(currentContent);

            editor.replaceWith(oldElement);
          } else {
            editor.hide();
          }
        }

        // Trigger the onBlur hook
        options.onBlur(this);
      }

      return this;
    }

  };

  /* MARKDOWN PLUGIN DEFINITION
   * ========================== */

  var old = $.fn.markdown;

  $.fn.markdown = function(option) {
    return this.each(function() {
      var $this = $(this),
        data = $this.data('markdown'),
        options = typeof option == 'object' && option;
      if (!data)
        $this.data('markdown', (data = new Markdown(this, options)));
    });
  };

  $.fn.markdown.messages = {};
    
    /* Custom Handlers (Added by InstantASP)
 * ==================== */
    
    $.fn.markdown.handlers = {

        insertContent: function(e, value) {

            // ensure we have focus
            e.$textarea.focus();

            var chunk = "",
                selected = e.getSelection(),
                cursor = selected.start + value.length;

            e.replaceSelection(value + chunk);
            e.setSelection(cursor, cursor + chunk.length);


        },
        insertContentLink: function(e, $target) {

            // ensure we have focus
            e.$textarea.focus();

            var chunk = "",
                selected = e.getSelection(),
                content = e.getContent(),
                value = $target.attr("data-value") || "#",
                cursor = selected.start + value.length;

            e.replaceSelection(value + chunk);
            e.setSelection(cursor, cursor + chunk.length);
            

        },
        insertQuote: function(e, $target) {

            var chunk, cursor, 
                selected = e.getSelection(),
                content = e.getContent(),
                value = $target.attr("data-value") || "> ";
                
            if (selected.length === 0) {

                chunk = e.__localize('quote here');
                e.replaceSelection(value + chunk);
                cursor = selected.start + value.length;

            } else {

                if (selected.text.indexOf('\n') < 0) {
                    chunk = selected.text;
                    e.replaceSelection(value + chunk);
                    cursor = selected.start + value.length;
                } else {
                    var list = [];
                    list = selected.text.split('\n');
                    chunk = list[0];
                    $.each(list, function (k, v) {
                        list[k] = value + v;
                    });
                    e.replaceSelection(list.join('\n'));
                    cursor = selected.start + value.length;
                }
            }

            e.setSelection(cursor, cursor + chunk.length);
            
            //// hide dropdown
            //if ($target.data("dropdownCaller")) {
            //    $target.data("dropdownCaller").idropdown("hide");
            //}

        },

        // inserts markdown header syntax (H1 = #, H2 = ##, H3 = ### etc)
        insertHeader: function(e, $target) {

            // Append/remove ### surround the selection
            var chunk,
                cursor,
                selected = e.getSelection(),
                content = e.getContent(),
                pointer,
                prevChar,
                value = $target.attr("data-value") || "#";
                        
            if (selected.length === 0) {
                // Give extra word
                chunk = e.__localize('heading text');
            } else {
                chunk = selected.text;
            }

            // transform selection and set the cursor into chunked text
            if ((pointer = value.length + 1,
                    content.substr(selected.start - pointer, pointer) === value + ' ') ||
                (pointer = value.length, content.substr(selected.start - pointer, pointer) === value)) {
          
                e.setSelection(selected.start - pointer, selected.end);
                e.replaceSelection(chunk);
                cursor = selected.start - pointer;
            } else if (selected.start > 0 &&
                (prevChar = content.substr(selected.start - 1, 1), !!prevChar && prevChar !== '\n')) {
                
                e.replaceSelection(value + ' ' + chunk);
                cursor = selected.start + value.length + 1;
            } else {
                // Empty string before element
                e.replaceSelection(value + ' ' + chunk);
                cursor = selected.start + value.length + 1;
            }

            // Set the cursor
            e.setSelection(cursor, cursor + chunk.length);

            // hide dropdown
            //if ($target.data("dropdownCaller")) {
            //    $target.data("dropdownCaller").idropdown("hide");
            //}

        }

    }

    $.fn.markdown.defaults = {
    /* Editor Properties */
    autofocus: false,
    hideable: false,
    savable: false,
    width: 'inherit',
    height: 'inherit',
    resize: null,
    iconlibrary: 'fa',
    language: 'en',
    initialstate: 'editor',
    parser: null,
    dropZoneOptions: {
        url: 'api/upload',
        fallbackClick: false,
        autoProcessQueue: true,
        disablePreview: true,
        dictDefaultMessage: 'Drop images here or click to select'
    },
    enableDropDataUri: false,
  
    /* Buttons Properties */
    buttons: [
      [
          //{
          //    name: 'groupUtil',
          //    css: 'btn-group mr-1 md-tabs',
          //    data: [
          //        {
          //            name: 'cmdWrite',
          //            hotkey: 'Ctrl+P',
          //            title: 'Write',
          //            btnText: 'Write',
          //            btnClass: 'btn md-btn-write selected',
          //            callback: function(e) {
          //                // Check the preview mode and toggle based on this flag
          //                var isPreview = e.$isPreview,
          //                    content;

          //                if (isPreview === true) {
          //                    e.hidePreview();
          //                }
          //            }
          //        }, {
          //            name: 'cmdPreview',
          //            hotkey: 'Ctrl+P',
          //            title: 'Preview',
          //            btnText: 'Preview',
          //            btnClass: 'btn md-btn-preview',
          //            callback: function(e) {
          //                // Check the preview mode and toggle based on this flag
          //                var isPreview = e.$isPreview,
          //                    content;

          //                if (isPreview === false) {
          //                    e.showPreview();
          //                }
          //            }
          //        }
          //    ]
          //  },
          {
              name: 'groupFont',
              css: 'btn-group mr-1',
              data: [
                  {
                      name: 'cmdBold',
                      hotkey: 'Ctrl+B',
                      title: 'Bold',
                      icon: {
                          glyph: 'glyphicon glyphicon-bold',
                          fa: 'fal fa-bold',
                          'fa-3': 'icon-bold',
                          octicons: 'octicon octicon-bold'
                      },
                      callback: function(e) {
                          // Give/remove ** surround the selection
                          var chunk,
                              cursor,
                              selected = e.getSelection(),
                              content = e.getContent();

                          if (selected.length === 0) {
                              // Give extra word
                              chunk = e.__localize('strong text');
                          } else {
                              chunk = selected.text;
                          }

                          // transform selection and set the cursor into chunked text
                          if (content.substr(selected.start - 2, 2) === '**' &&
                              content.substr(selected.end, 2) === '**') {
                              e.setSelection(selected.start - 2, selected.end + 2);
                              e.replaceSelection(chunk);
                              cursor = selected.start - 2;
                          } else {
                              e.replaceSelection('**' + chunk + '**');
                              cursor = selected.start + 2;
                          }

                          // Set the cursor
                          e.setSelection(cursor, cursor + chunk.length);
                      }
                  }, {
                      name: 'cmdItalic',
                      title: 'Italic',
                      hotkey: 'Ctrl+I',
                      icon: {
                          glyph: 'glyphicon glyphicon-italic',
                          fa: 'fal fa-italic',
                          'fa-3': 'icon-italic',
                          octicons: 'octicon octicon-italic'
                      },
                      callback: function(e) {
                          // Give/remove * surround the selection
                          var chunk,
                              cursor,
                              selected = e.getSelection(),
                              content = e.getContent();

                          if (selected.length === 0) {
                              // Give extra word
                              chunk = e.__localize('emphasized text');
                          } else {
                              chunk = selected.text;
                          }

                          // transform selection and set the cursor into chunked text
                          if (content.substr(selected.start - 1, 1) === '_' &&
                              content.substr(selected.end, 1) === '_') {
                              e.setSelection(selected.start - 1, selected.end + 1);
                              e.replaceSelection(chunk);
                              cursor = selected.start - 1;
                          } else {
                              e.replaceSelection('_' + chunk + '_');
                              cursor = selected.start + 1;
                          }

                          // Set the cursor
                          e.setSelection(cursor, cursor + chunk.length);
                      }
                  }, {
                      name: 'cmdHeading',
                      title: 'Heading',
                      hotkey: 'Ctrl+H',
                      dropdown: {
                          title: "Heading",
                          width: "200px",
                          css: "md-header-dropdown",
                          items: [
                              {
                                  name: 'cmdHeading1',
                                  hotkey: "",
                                  text: "<h1>Header 1</h1>",
                                  value: "#",
                                  callback: function(e, $target) {
                                      $.fn.markdown.handlers.insertHeader(e, $target);
                                  }
                              },
                              {
                                  name: 'cmdHeading2',
                                  text: "<h2>Header 2</h2>",
                                  value: "##",
                                  callback: function(e, $target) {
                                      $.fn.markdown.handlers.insertHeader(e, $target);
                                  }
                              },
                              {
                                  name: 'cmdHeading3',
                                  text: "<h3>Header 3</h3>",
                                  value: "###",
                                  callback: function(e, $target) {
                                      $.fn.markdown.handlers.insertHeader(e, $target);
                                  }
                              },
                              {
                                  name: 'cmdHeading4',
                                  text: "<h4>Header 4</h4>",
                                  value: "####",
                                  callback: function(e, $target) {
                                      $.fn.markdown.handlers.insertHeader(e, $target);
                                  }
                              },
                              {
                                  name: 'cmdHeading5',
                                  text: "<h5>Header 5</h5>",
                                  value: "#####",
                                  callback: function(e, $target) {
                                      $.fn.markdown.handlers.insertHeader(e, $target);
                                  }
                              },
                              {
                                  name: 'cmdHeading6',
                                  text: "<h6>Header 6</h6>",
                                  value: "######",
                                  callback: function(e, $target) {
                                      $.fn.markdown.handlers.insertHeader(e, $target);
                                  }
                              }
                          ]
                      },
                      icon: {
                          glyph: 'glyphicon glyphicon-header',
                          fa: 'fal fa-heading',
                          'fa-3': 'icon-font',
                          octicons: 'octicon octicon-text-size'
                      },
                      callback: function(e, $target) {


                      }
                  }
              ]
            }, {
                name: 'groupInsert',
                css: 'btn-group mr-1',
                data: [
                    {
                        name: 'cmdLink',
                        title: 'Link To URL',
                        hotkey: 'Ctrl+L',
                        icon: {
                            glyph: 'glyphicon glyphicon-link',
                            fa: 'fal fa-link',
                            'fa-3': 'icon-link',
                            octicons: 'octicon octicon-link'
                        },
                        callback: function (e) {

                            // Give [] surround the selection and prepend the link
                            var chunk,
                                cursor,
                                selected = e.getSelection(),
                                content = e.getContent(),
                                link = null;

                            if (selected.length === 0) {
                                // Give extra word
                                chunk = e.__localize('enter link description here');
                            } else {
                                chunk = selected.text;
                            }

                            link = prompt(e.__localize('Insert Hyperlink'), 'http://');

                            //var urlRegex = new RegExp('^((http|https)://|(mailto:)|(//))[a-z0-9]', 'i');
                            if (link !== null && link !== '' && link !== 'http://' && link !== 'https://') {
                                var sanitizedLink = $('<div>' + link + '</div>').text();

                                // transform selection and set the cursor into chunked text
                                e.replaceSelection('[' + chunk + '](' + sanitizedLink + ')');
                                cursor = selected.start + 1;

                                // Set the cursor
                                e.setSelection(cursor, cursor + chunk.length);
                            }
                        }
                    }, {
                        name: 'cmdImage',
                        title: 'Link To Image',
                        hotkey: 'Ctrl+G',
                        icon: {
                            glyph: 'glyphicon glyphicon-picture',
                            fa: 'fal fa-image',
                            'fa-3': 'icon-picture',
                            octicons: 'octicon octicon-file-media'
                        },
                        callback: function (e) {

                            // Give ![] surround the selection and prepend the image link
                            var chunk,
                                cursor,
                                selected = e.getSelection(),
                                content = e.getContent();

                            if (selected.length === 0) {
                                // Give extra word
                                chunk = e.__localize('enter image description here');
                            } else {
                                chunk = selected.text;
                            }

                            var link = prompt(e.__localize('Insert Image Hyperlink'), 'http://');

                            var urlRegex = new RegExp('^((http|https)://|(//))[a-z0-9]', 'i');
                            if (link !== null && link !== '' && link !== 'http://' && urlRegex.test(link)) {
                                var sanitizedLink = $('<div>' + link + '</div>').text();

                                // transform selection and set the cursor into chunked text
                                e.replaceSelection('![' + chunk + '](' + sanitizedLink + ' "' + e.__localize('enter image title here') + '")');
                                cursor = selected.start + 2;

                                // Set the next tab
                                e.setNextTab(e.__localize('enter image title here'));

                                // Set the cursor
                                e.setSelection(cursor, cursor + chunk.length);

                            }
                        }
                    },
                    {
                        name: 'cmdVideo',
                        title: 'Embed Video',
                        hotkey: 'Ctrl+Y',
                        dropdown: {
                            title: "Embed Video",
                            width: "500px",
                            css: "dropdown",
                            items: null,
                            html:
                                '<table class="i-table"><tr><td><div class="i-row"><input type="text" style="width: 100%;" class="i-input i-text-box"/></div><div class="i-row i-margin-top"><a href="#" class="btn btn-full btn-2x btn-primary"><span></span></a></div></td></tr></table>'
                        },
                        icon: {
                            glyph: 'glyphicon glyphicon-search',
                            fa: 'fal fa-play',
                            'fa-3': 'icon-search',
                            octicons: 'octicon octicon-search'
                        },
                        callback: function (editor, $target) {

                            var placeholderText = editor.__localize("Enter a YouTube or Vimeo URL..."),
                                buttonText = editor.__localize("Add Video");


                        }
                    }
                ]
            }, {
              name: 'groupLists',
              css: 'btn-group mr-1',
              data: [
                  {
                      name: 'cmdList',
                      hotkey: 'Ctrl+U',
                      title: 'Unordered List',
                      icon: {
                          glyph: 'glyphicon glyphicon-list',
                          fa: 'fal fa-list',
                          'fa-3': 'icon-list-ul',
                          octicons: 'octicon octicon-list-unordered'
                      },
                      callback: function(e) {
                          // Prepend/Give - surround the selection
                          var chunk,
                              cursor,
                              selected = e.getSelection(),
                              content = e.getContent();

                          // transform selection and set the cursor into chunked text
                          if (selected.length === 0) {
                              // Give extra word
                              chunk = e.__localize('list text here');

                              e.replaceSelection('- ' + chunk);
                              // Set the cursor
                              cursor = selected.start + 2;
                          } else {
                              if (selected.text.indexOf('\n') < 0) {
                                  chunk = selected.text;

                                  e.replaceSelection('- ' + chunk);

                                  // Set the cursor
                                  cursor = selected.start + 2;
                              } else {
                                  var list = [];

                                  list = selected.text.split('\n');
                                  chunk = list[0];

                                  $.each(list,
                                      function(k, v) {
                                          list[k] = '- ' + v;
                                      });

                                  e.replaceSelection('\n\n' + list.join('\n'));

                                  // Set the cursor
                                  cursor = selected.start + 4;
                              }
                          }

                          // Set the cursor
                          e.setSelection(cursor, cursor + chunk.length);
                      }
                  }, {
                      name: 'cmdListO',
                      hotkey: 'Ctrl+O',
                      title: 'Ordered List',
                      icon: {
                          glyph: 'glyphicon glyphicon-th-list',
                          fa: 'fal fa-list-ol',
                          'fa-3': 'icon-list-ol',
                          octicons: 'octicon octicon-list-ordered'
                      },
                      callback: function(e) {

                          // Prepend/Give - surround the selection
                          var chunk,
                              cursor,
                              selected = e.getSelection(),
                              content = e.getContent();

                          // transform selection and set the cursor into chunked text
                          if (selected.length === 0) {
                              // Give extra word
                              chunk = e.__localize('list text here');
                              e.replaceSelection('1. ' + chunk);
                              // Set the cursor
                              cursor = selected.start + 3;
                          } else {
                              if (selected.text.indexOf('\n') < 0) {
                                  chunk = selected.text;

                                  e.replaceSelection('1. ' + chunk);

                                  // Set the cursor
                                  cursor = selected.start + 3;
                              } else {
                                  var i = 1;
                                  var list = [];

                                  list = selected.text.split('\n');
                                  chunk = list[0];

                                  $.each(list,
                                      function(k, v) {
                                          list[k] = i + '. ' + v;
                                          i++;
                                      });

                                  e.replaceSelection('\n\n' + list.join('\n'));

                                  // Set the cursor
                                  cursor = selected.start + 5;
                              }
                          }

                          // Set the cursor
                          e.setSelection(cursor, cursor + chunk.length);
                      }
                  }
              ]
          }, {
              name: 'groupBlocks',
              css: 'btn-group mr-1',
              data: [
                  {
                      name: 'cmdBlockquote',
                      title: 'Blockquote',
                      hotkey: 'Ctrl+Q',
                      dropdown: {
                          title: "Blockquote",
                          width: "200px",
                          css: "dropdown md-blockquote-dropdown",
                          items: [
                              {
                                  name: 'cmdBlockquotePrimary',
                                  hotkey: "",
                                  text:
                                      '<blockquote class="i-blockquote i-blockquote-primary"><p>Primary</p></blockquote>',
                                  value: "> {primary} ",
                                  callback: function(e, $target) {
                                      $.fn.markdown.handlers.insertQuote(e, $target);
                                  }
                              },
                              {
                                  name: 'cmdBlockquoteDefault',
                                  text:
                                      '<blockquote class="i-blockquote i-blockquote-default"><p>Default</p></blockquote>',
                                  value: "> {default} ",
                                  callback: function(e, $target) {
                                      $.fn.markdown.handlers.insertQuote(e, $target);
                                  }
                              },
                              {
                                  name: 'cmdBlockquoteSuccess',
                                  text:
                                      '<blockquote class="i-blockquote i-blockquote-success"><p>Success</p></blockquote>',
                                  value: "> {success} ",
                                  callback: function(e, $target) {
                                      $.fn.markdown.handlers.insertQuote(e, $target);
                                  }
                              },
                              {
                                  name: 'cmdBlockquoteInfo',
                                  text: '<blockquote class="i-blockquote i-blockquote-info"><p>Info</p></blockquote>',
                                  value: "> {info} ",
                                  callback: function(e, $target) {
                                      $.fn.markdown.handlers.insertQuote(e, $target);
                                  }
                              },
                              {
                                  name: 'cmdBlockquoteWarning',
                                  text:
                                      '<blockquote class="i-blockquote i-blockquote-warning"><p>Warning</p></blockquote>',
                                  value: "> {warning} ",
                                  callback: function(e, $target) {
                                      $.fn.markdown.handlers.insertQuote(e, $target);
                                  }
                              },
                              {
                                  name: 'cmdBlockquoteDanger',
                                  text:
                                      '<blockquote class="i-blockquote i-blockquote-danger"><p>Danger</p></blockquote>',
                                  value: "> {danger} ",
                                  callback: function(e, $target) {
                                      $.fn.markdown.handlers.insertQuote(e, $target);
                                  }
                              }
                          ]
                      },
                      icon: {
                          glyph: 'glyphicon glyphicon-comment',
                          fa: 'fal fa-quote-left',
                          'fa-3': 'icon-quote-left',
                          octicons: 'octicon octicon-quote'
                      },
                      callback: function(e, $target) {
                       
                      }
                  }, {
                      name: 'cmdCode',
                      hotkey: 'Ctrl+K',
                      title: 'Code',
                      icon: {
                          glyph: 'glyphicon glyphicon-console',
                          fa: 'fal fa-code',
                          'fa-3': 'icon-code',
                          octicons: 'octicon octicon-code'
                      },
                      callback: function(e) {
                          // Give/remove ** surround the selection
                          var chunk,
                              cursor,
                              selected = e.getSelection(),
                              content = e.getContent();

                          if (selected.length === 0) {
                              // Give extra word
                              chunk = e.__localize('code text here');
                          } else {
                              chunk = selected.text;
                          }

                          // transform selection and set the cursor into chunked text
                          if (content.substr(selected.start - 4, 4) === '```\n' &&
                              content.substr(selected.end, 4) === '\n```') {
                              e.setSelection(selected.start - 4, selected.end + 4);
                              e.replaceSelection(chunk);
                              cursor = selected.start - 4;
                          } else if (content.substr(selected.start - 1, 1) === '`' &&
                              content.substr(selected.end, 1) === '`') {
                              e.setSelection(selected.start - 1, selected.end + 1);
                              e.replaceSelection(chunk);
                              cursor = selected.start - 1;
                          } else if (content.indexOf('\n') > -1) {
                              e.replaceSelection('```\n' + chunk + '\n```');
                              cursor = selected.start + 4;
                          } else {
                              e.replaceSelection('`' + chunk + '`');
                              cursor = selected.start + 1;
                          }

                          // Set the cursor
                          e.setSelection(cursor, cursor + chunk.length);
                      }
                  }
              ]
          }, {
              name: 'groupEmbed',
              css: 'btn-group mr-1',
              data: [
                  {
                      name: 'cmdLinkContent',
                      title: 'Link To Article',
                      hotkey: 'Ctrl+L',
                      dropdown: {
                          title: "Link To Article",
                          width: "600px",
                          css: "dropdown",
                          items: null,
                          html:
                              '<table class="i-table"><tr><td><div class="i-input-group i-dropdown"><input type="text" style="width: 100%;" class="i-input i-text-box" data-autocomplete-url="{baseUrl}api/search?page_index={pageIndex}&page_size={pageSize}" data-autocomplete-page-size="10" data-dropdown-arrow="false"/><ul class="i-dropdown-menu"><li style="height: 100%;"><div class="i-loader-jumbo"><div class="i-loader i-loader-inverted i-loader-2x"></div></div></li></ul></div></td></tr></table>'
                      },
                      icon: {
                          glyph: 'glyphicon glyphicon-search',
                          fa: 'fal fa-hashtag',
                          'fa-3': 'icon-search',
                          octicons: 'octicon octicon-search'
                      },
                      callback: function(editor, $target) {

                          var placeholderText = editor.__localize("Search for articles to link to...");


                      }
                  },
                  {
                      name: 'cmdCannedReply',
                      title: 'Canned Replies',
                      hotkey: 'Ctrl+L',
                      dropdown: {
                          title: "Canned Replies",
                          width: "250px",
                          height: "350px",
                          css: "dropdown-right",
                          items: null,
                          html:
                              '<div class="i-loader-jumbo"><div class="i-loader i-loader-2x i-loader-inverted"></div></div>',
                      },
                      icon: {
                          glyph: 'glyphicon glyphicon-search',
                          fa: 'fal fa-edit',
                          'fa-3': 'icon-search',
                          octicons: 'octicon octicon-search'
                      },
                      callback: function(editor, $target) {

                          var ajaxUrl = editor.$textarea.data("standardReplyUrl");
                          if (ajaxUrl) {
                              

                          } else {
                              alert("No data-standard-reply-url has been defined on the editor.$textarea");
                          }


                      }
                  }
              ]
          }
      ]
    ],
    customIcons: {},
    additionalButtons: [], // Place to hook more buttons by code
    reorderButtonGroups: [],
    hiddenButtons: [], // Default hidden buttons
    disabledButtons: [], // Default disabled buttons
    footer: '',
    minRows: 4,
    maxRows: 15,
    fullscreen: {
      enable: true,
      icons: {
        fullscreenOn: {
          name: "fullscreenOn",
          icon: {
            fa: 'fal fa-expand',
            glyph: 'glyphicon glyphicon-fullscreen',
            'fa-3': 'icon-resize-full',
            octicons: 'octicon octicon-link-external'
          }
        },
        fullscreenOff: {
          name: "fullscreenOff",
          icon: {
              fa: 'fal fa-compress',
            glyph: 'glyphicon glyphicon-fullscreen',
            'fa-3': 'icon-resize-small',
            octicons: 'octicon octicon-browser'
          }
        }
      }
    },
    buttonContainer: "#markdownButtons",

    /* Events hook */
    onShow: function(e) {},
    onPreview: function(e) {},
    onPreviewEnd: function(e) {},
    onSave: function(e) {},
    onBlur: function(e) {},
    onFocus: function(e) {},
    onChange: function(e) {},
    onFullscreen: function(e) {},
    onFullscreenExit: function(e) {},
    onSelect: function(e) {}
  };

  $.fn.markdown.Constructor = Markdown;
    
  /* MARKDOWN NO CONFLICT
   * ==================== */
    
  $.fn.markdown.noConflict = function() {
    $.fn.markdown = old;
    return this;
  };

  /* MARKDOWN GLOBAL FUNCTION & DATA-API
   * ==================================== */

  var initMarkdown = function(el) {
    var $this = el;

    if ($this.data('markdown')) {
      $this.data('markdown').showEditor();
      return;
    }

    $this.markdown();
  };

  var blurNonFocused = function(e) {
    var $activeElement = $(document.activeElement);

    // Blur event
    $(document).find('.md-editor').each(function() {
      var $this = $(this),
        focused = $activeElement.closest('.md-editor')[0] === this,
        attachedMarkdown = $this.find('textarea').data('markdown') ||
        $this.find('div[data-provider="markdown-preview"]').data('markdown');

      if (attachedMarkdown && !focused) {
        attachedMarkdown.blur();
      }
    });
  };
    
  $(document)
    .on('click.markdown.data-api', '[data-provide="markdown-editable"]', function(e) {
      initMarkdown($(this));
      e.preventDefault();
    })
    .on('click focusin', function(e) {
      blurNonFocused(e);
    })
    .ready(function() {
      $('textarea[data-provide="markdown"]').each(function() {
        initMarkdown($(this));
      });
    });

}));
