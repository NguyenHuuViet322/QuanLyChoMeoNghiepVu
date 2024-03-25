/*!
 * AdminLTE v3.1.0-pre (https://adminlte.io)
 * Copyright 2014-2021 Colorlib <https://colorlib.com>
 * Licensed under MIT (https://github.com/ColorlibHQ/AdminLTE/blob/master/LICENSE)
 */
(function (global, factory) {
  typeof exports === 'object' && typeof module !== 'undefined' ? factory(exports, require('jquery')) :
  typeof define === 'function' && define.amd ? define(['exports', 'jquery'], factory) :
  (global = typeof globalThis !== 'undefined' ? globalThis : global || self, factory(global.adminlte = {}, global.jQuery));
}(this, (function (exports, $$1) { 'use strict';

  function _interopDefaultLegacy (e) { return e && typeof e === 'object' && 'default' in e ? e : { 'default': e }; }

  var $__default = /*#__PURE__*/_interopDefaultLegacy($$1);

  /**
   * --------------------------------------------
   * AdminLTE CardRefresh.js
   * License MIT
   * --------------------------------------------
   */
  /**
   * Constants
   * ====================================================
   */

  var NAME = 'CardRefresh';
  var DATA_KEY = 'lte.cardrefresh';
  var EVENT_KEY = "." + DATA_KEY;
  var JQUERY_NO_CONFLICT = $__default['default'].fn[NAME];
  var EVENT_LOADED = "loaded" + EVENT_KEY;
  var EVENT_OVERLAY_ADDED = "overlay.added" + EVENT_KEY;
  var EVENT_OVERLAY_REMOVED = "overlay.removed" + EVENT_KEY;
  var CLASS_NAME_CARD = 'card';
  var SELECTOR_CARD = "." + CLASS_NAME_CARD;
  var SELECTOR_DATA_REFRESH = '[data-card-widget="card-refresh"]';
  var Default = {
    source: '',
    sourceSelector: '',
    params: {},
    trigger: SELECTOR_DATA_REFRESH,
    content: '.card-body',
    loadInContent: true,
    loadOnInit: true,
    responseType: '',
    overlayTemplate: '<div class="overlay"><i class="fas fa-2x fa-sync-alt fa-spin"></i></div>',
    onLoadStart: function onLoadStart() {},
    onLoadDone: function onLoadDone(response) {
      return response;
    }
  };

  var CardRefresh = /*#__PURE__*/function () {
    function CardRefresh(element, settings) {
      this._element = element;
      this._parent = element.parents(SELECTOR_CARD).first();
      this._settings = $__default['default'].extend({}, Default, settings);
      this._overlay = $__default['default'](this._settings.overlayTemplate);

      if (element.hasClass(CLASS_NAME_CARD)) {
        this._parent = element;
      }

      if (this._settings.source === '') {
        throw new Error('Source url was not defined. Please specify a url in your CardRefresh source option.');
      }
    }

    var _proto = CardRefresh.prototype;

    _proto.load = function load() {
      var _this = this;

      this._addOverlay();

      this._settings.onLoadStart.call($__default['default'](this));

      $__default['default'].get(this._settings.source, this._settings.params, function (response) {
        if (_this._settings.loadInContent) {
          if (_this._settings.sourceSelector !== '') {
            response = $__default['default'](response).find(_this._settings.sourceSelector).html();
          }

          _this._parent.find(_this._settings.content).html(response);
        }

        _this._settings.onLoadDone.call($__default['default'](_this), response);

        _this._removeOverlay();
      }, this._settings.responseType !== '' && this._settings.responseType);
      $__default['default'](this._element).trigger($__default['default'].Event(EVENT_LOADED));
    };

    _proto._addOverlay = function _addOverlay() {
      this._parent.append(this._overlay);

      $__default['default'](this._element).trigger($__default['default'].Event(EVENT_OVERLAY_ADDED));
    };

    _proto._removeOverlay = function _removeOverlay() {
      this._parent.find(this._overlay).remove();

      $__default['default'](this._element).trigger($__default['default'].Event(EVENT_OVERLAY_REMOVED));
    } // Private
    ;

    _proto._init = function _init() {
      var _this2 = this;

      $__default['default'](this).find(this._settings.trigger).on('click', function () {
        _this2.load();
      });

      if (this._settings.loadOnInit) {
        this.load();
      }
    } // Static
    ;

    CardRefresh._jQueryInterface = function _jQueryInterface(config) {
      var data = $__default['default'](this).data(DATA_KEY);

      var _options = $__default['default'].extend({}, Default, $__default['default'](this).data());

      if (!data) {
        data = new CardRefresh($__default['default'](this), _options);
        $__default['default'](this).data(DATA_KEY, typeof config === 'string' ? data : config);
      }

      if (typeof config === 'string' && config.match(/load/)) {
        data[config]();
      } else {
        data._init($__default['default'](this));
      }
    };

    return CardRefresh;
  }();
  /**
   * Data API
   * ====================================================
   */


  $__default['default'](document).on('click', SELECTOR_DATA_REFRESH, function (event) {
    if (event) {
      event.preventDefault();
    }

    CardRefresh._jQueryInterface.call($__default['default'](this), 'load');
  });
  $__default['default'](function () {
    $__default['default'](SELECTOR_DATA_REFRESH).each(function () {
      CardRefresh._jQueryInterface.call($__default['default'](this));
    });
  });
  /**
   * jQuery API
   * ====================================================
   */

  $__default['default'].fn[NAME] = CardRefresh._jQueryInterface;
  $__default['default'].fn[NAME].Constructor = CardRefresh;

  $__default['default'].fn[NAME].noConflict = function () {
    $__default['default'].fn[NAME] = JQUERY_NO_CONFLICT;
    return CardRefresh._jQueryInterface;
  };

  /**
   * --------------------------------------------
   * AdminLTE CardWidget.js
   * License MIT
   * --------------------------------------------
   */
  /**
   * Constants
   * ====================================================
   */

  var NAME$1 = 'CardWidget';
  var DATA_KEY$1 = 'lte.cardwidget';
  var EVENT_KEY$1 = "." + DATA_KEY$1;
  var JQUERY_NO_CONFLICT$1 = $__default['default'].fn[NAME$1];
  var EVENT_EXPANDED = "expanded" + EVENT_KEY$1;
  var EVENT_COLLAPSED = "collapsed" + EVENT_KEY$1;
  var EVENT_MAXIMIZED = "maximized" + EVENT_KEY$1;
  var EVENT_MINIMIZED = "minimized" + EVENT_KEY$1;
  var EVENT_REMOVED = "removed" + EVENT_KEY$1;
  var CLASS_NAME_CARD$1 = 'card';
  var CLASS_NAME_COLLAPSED = 'collapsed-card';
  var CLASS_NAME_COLLAPSING = 'collapsing-card';
  var CLASS_NAME_EXPANDING = 'expanding-card';
  var CLASS_NAME_WAS_COLLAPSED = 'was-collapsed';
  var CLASS_NAME_MAXIMIZED = 'maximized-card';
  var SELECTOR_DATA_REMOVE = '[data-card-widget="remove"]';
  var SELECTOR_DATA_COLLAPSE = '[data-card-widget="collapse"]';
  var SELECTOR_DATA_MAXIMIZE = '[data-card-widget="maximize"]';
  var SELECTOR_CARD$1 = "." + CLASS_NAME_CARD$1;
  var SELECTOR_CARD_HEADER = '.card-header';
  var SELECTOR_CARD_BODY = '.card-body';
  var SELECTOR_CARD_FOOTER = '.card-footer';
  var Default$1 = {
    animationSpeed: 'normal',
    collapseTrigger: SELECTOR_DATA_COLLAPSE,
    removeTrigger: SELECTOR_DATA_REMOVE,
    maximizeTrigger: SELECTOR_DATA_MAXIMIZE,
    collapseIcon: 'fa-minus',
    expandIcon: 'fa-plus',
    maximizeIcon: 'fa-expand',
    minimizeIcon: 'fa-compress'
  };

  var CardWidget = /*#__PURE__*/function () {
    function CardWidget(element, settings) {
      this._element = element;
      this._parent = element.parents(SELECTOR_CARD$1).first();

      if (element.hasClass(CLASS_NAME_CARD$1)) {
        this._parent = element;
      }

      this._settings = $__default['default'].extend({}, Default$1, settings);
    }

    var _proto = CardWidget.prototype;

    _proto.collapse = function collapse() {
      var _this = this;

      this._parent.addClass(CLASS_NAME_COLLAPSING).children(SELECTOR_CARD_BODY + ", " + SELECTOR_CARD_FOOTER).slideUp(this._settings.animationSpeed, function () {
        _this._parent.addClass(CLASS_NAME_COLLAPSED).removeClass(CLASS_NAME_COLLAPSING);
      });

      this._parent.find("> " + SELECTOR_CARD_HEADER + " " + this._settings.collapseTrigger + " ." + this._settings.collapseIcon).addClass(this._settings.expandIcon).removeClass(this._settings.collapseIcon);

      this._element.trigger($__default['default'].Event(EVENT_COLLAPSED), this._parent);
    };

    _proto.expand = function expand() {
      var _this2 = this;

      this._parent.addClass(CLASS_NAME_EXPANDING).children(SELECTOR_CARD_BODY + ", " + SELECTOR_CARD_FOOTER).slideDown(this._settings.animationSpeed, function () {
        _this2._parent.removeClass(CLASS_NAME_COLLAPSED).removeClass(CLASS_NAME_EXPANDING);
      });

      this._parent.find("> " + SELECTOR_CARD_HEADER + " " + this._settings.collapseTrigger + " ." + this._settings.expandIcon).addClass(this._settings.collapseIcon).removeClass(this._settings.expandIcon);

      this._element.trigger($__default['default'].Event(EVENT_EXPANDED), this._parent);
    };

    _proto.remove = function remove() {
      this._parent.slideUp();

      this._element.trigger($__default['default'].Event(EVENT_REMOVED), this._parent);
    };

    _proto.toggle = function toggle() {
      if (this._parent.hasClass(CLASS_NAME_COLLAPSED)) {
        this.expand();
        return;
      }

      this.collapse();
    };

    _proto.maximize = function maximize() {
      this._parent.find(this._settings.maximizeTrigger + " ." + this._settings.maximizeIcon).addClass(this._settings.minimizeIcon).removeClass(this._settings.maximizeIcon);

      this._parent.css({
        height: this._parent.height(),
        width: this._parent.width(),
        transition: 'all .15s'
      }).delay(150).queue(function () {
        var $element = $__default['default'](this);
        $element.addClass(CLASS_NAME_MAXIMIZED);
        $__default['default']('html').addClass(CLASS_NAME_MAXIMIZED);

        if ($element.hasClass(CLASS_NAME_COLLAPSED)) {
          $element.addClass(CLASS_NAME_WAS_COLLAPSED);
        }

        $element.dequeue();
      });

      this._element.trigger($__default['default'].Event(EVENT_MAXIMIZED), this._parent);
    };

    _proto.minimize = function minimize() {
      this._parent.find(this._settings.maximizeTrigger + " ." + this._settings.minimizeIcon).addClass(this._settings.maximizeIcon).removeClass(this._settings.minimizeIcon);

      this._parent.css('cssText', "height: " + this._parent[0].style.height + " !important; width: " + this._parent[0].style.width + " !important; transition: all .15s;").delay(10).queue(function () {
        var $element = $__default['default'](this);
        $element.removeClass(CLASS_NAME_MAXIMIZED);
        $__default['default']('html').removeClass(CLASS_NAME_MAXIMIZED);
        $element.css({
          height: 'inherit',
          width: 'inherit'
        });

        if ($element.hasClass(CLASS_NAME_WAS_COLLAPSED)) {
          $element.removeClass(CLASS_NAME_WAS_COLLAPSED);
        }

        $element.dequeue();
      });

      this._element.trigger($__default['default'].Event(EVENT_MINIMIZED), this._parent);
    };

    _proto.toggleMaximize = function toggleMaximize() {
      if (this._parent.hasClass(CLASS_NAME_MAXIMIZED)) {
        this.minimize();
        return;
      }

      this.maximize();
    } // Private
    ;

    _proto._init = function _init(card) {
      var _this3 = this;

      this._parent = card;
      $__default['default'](this).find(this._settings.collapseTrigger).click(function () {
        _this3.toggle();
      });
      $__default['default'](this).find(this._settings.maximizeTrigger).click(function () {
        _this3.toggleMaximize();
      });
      $__default['default'](this).find(this._settings.removeTrigger).click(function () {
        _this3.remove();
      });
    } // Static
    ;

    CardWidget._jQueryInterface = function _jQueryInterface(config) {
      var data = $__default['default'](this).data(DATA_KEY$1);

      var _options = $__default['default'].extend({}, Default$1, $__default['default'](this).data());

      if (!data) {
        data = new CardWidget($__default['default'](this), _options);
        $__default['default'](this).data(DATA_KEY$1, typeof config === 'string' ? data : config);
      }

      if (typeof config === 'string' && config.match(/collapse|expand|remove|toggle|maximize|minimize|toggleMaximize/)) {
        data[config]();
      } else if (typeof config === 'object') {
        data._init($__default['default'](this));
      }
    };

    return CardWidget;
  }();
  /**
   * Data API
   * ====================================================
   */


  $__default['default'](document).on('click', SELECTOR_DATA_COLLAPSE, function (event) {
    if (event) {
      event.preventDefault();
    }

    CardWidget._jQueryInterface.call($__default['default'](this), 'toggle');
  });
  $__default['default'](document).on('click', SELECTOR_DATA_REMOVE, function (event) {
    if (event) {
      event.preventDefault();
    }

    CardWidget._jQueryInterface.call($__default['default'](this), 'remove');
  });
  $__default['default'](document).on('click', SELECTOR_DATA_MAXIMIZE, function (event) {
    if (event) {
      event.preventDefault();
    }

    CardWidget._jQueryInterface.call($__default['default'](this), 'toggleMaximize');
  });
  /**
   * jQuery API
   * ====================================================
   */

  $__default['default'].fn[NAME$1] = CardWidget._jQueryInterface;
  $__default['default'].fn[NAME$1].Constructor = CardWidget;

  $__default['default'].fn[NAME$1].noConflict = function () {
    $__default['default'].fn[NAME$1] = JQUERY_NO_CONFLICT$1;
    return CardWidget._jQueryInterface;
  };

  /**
   * --------------------------------------------
   * AdminLTE ControlSidebar.js
   * License MIT
   * --------------------------------------------
   */
  /**
   * Constants
   * ====================================================
   */

  var NAME$2 = 'ControlSidebar';
  var DATA_KEY$2 = 'lte.controlsidebar';
  var EVENT_KEY$2 = "." + DATA_KEY$2;
  var JQUERY_NO_CONFLICT$2 = $__default['default'].fn[NAME$2];
  var EVENT_COLLAPSED$1 = "collapsed" + EVENT_KEY$2;
  var EVENT_EXPANDED$1 = "expanded" + EVENT_KEY$2;
  var SELECTOR_CONTROL_SIDEBAR = '.control-sidebar';
  var SELECTOR_CONTROL_SIDEBAR_CONTENT = '.control-sidebar-content';
  var SELECTOR_DATA_TOGGLE = '[data-widget="control-sidebar"]';
  var SELECTOR_HEADER = '.main-header';
  var SELECTOR_FOOTER = '.main-footer';
  var CLASS_NAME_CONTROL_SIDEBAR_ANIMATE = 'control-sidebar-animate';
  var CLASS_NAME_CONTROL_SIDEBAR_OPEN = 'control-sidebar-open';
  var CLASS_NAME_CONTROL_SIDEBAR_SLIDE = 'control-sidebar-slide-open';
  var CLASS_NAME_LAYOUT_FIXED = 'layout-fixed';
  var CLASS_NAME_NAVBAR_FIXED = 'layout-navbar-fixed';
  var CLASS_NAME_NAVBAR_SM_FIXED = 'layout-sm-navbar-fixed';
  var CLASS_NAME_NAVBAR_MD_FIXED = 'layout-md-navbar-fixed';
  var CLASS_NAME_NAVBAR_LG_FIXED = 'layout-lg-navbar-fixed';
  var CLASS_NAME_NAVBAR_XL_FIXED = 'layout-xl-navbar-fixed';
  var CLASS_NAME_FOOTER_FIXED = 'layout-footer-fixed';
  var CLASS_NAME_FOOTER_SM_FIXED = 'layout-sm-footer-fixed';
  var CLASS_NAME_FOOTER_MD_FIXED = 'layout-md-footer-fixed';
  var CLASS_NAME_FOOTER_LG_FIXED = 'layout-lg-footer-fixed';
  var CLASS_NAME_FOOTER_XL_FIXED = 'layout-xl-footer-fixed';
  var Default$2 = {
    controlsidebarSlide: true,
    scrollbarTheme: $__default['default'](SELECTOR_CONTROL_SIDEBAR).hasClass('control-sidebar-light') ? 'os-theme-dark' : 'os-theme-light',
    scrollbarAutoHide: 'l'
  };
  /**
   * Class Definition
   * ====================================================
   */

  var ControlSidebar = /*#__PURE__*/function () {
    function ControlSidebar(element, config) {
      this._element = element;
      this._config = config;

      this._init();
    } // Public


    var _proto = ControlSidebar.prototype;

    _proto.collapse = function collapse() {
      var $body = $__default['default']('body');
      var $html = $__default['default']('html'); // Show the control sidebar

      if (this._config.controlsidebarSlide) {
        $html.addClass(CLASS_NAME_CONTROL_SIDEBAR_ANIMATE);
        $body.removeClass(CLASS_NAME_CONTROL_SIDEBAR_SLIDE).delay(300).queue(function () {
          $__default['default'](SELECTOR_CONTROL_SIDEBAR).hide();
          $html.removeClass(CLASS_NAME_CONTROL_SIDEBAR_ANIMATE);
          $__default['default'](this).dequeue();
        });
      } else {
        $body.removeClass(CLASS_NAME_CONTROL_SIDEBAR_OPEN);
      }

      $__default['default'](this._element).trigger($__default['default'].Event(EVENT_COLLAPSED$1));
    };

    _proto.show = function show() {
      var $body = $__default['default']('body');
      var $html = $__default['default']('html'); // Collapse the control sidebar

      if (this._config.controlsidebarSlide) {
        $html.addClass(CLASS_NAME_CONTROL_SIDEBAR_ANIMATE);
        $__default['default'](SELECTOR_CONTROL_SIDEBAR).show().delay(10).queue(function () {
          $body.addClass(CLASS_NAME_CONTROL_SIDEBAR_SLIDE).delay(300).queue(function () {
            $html.removeClass(CLASS_NAME_CONTROL_SIDEBAR_ANIMATE);
            $__default['default'](this).dequeue();
          });
          $__default['default'](this).dequeue();
        });
      } else {
        $body.addClass(CLASS_NAME_CONTROL_SIDEBAR_OPEN);
      }

      this._fixHeight();

      this._fixScrollHeight();

      $__default['default'](this._element).trigger($__default['default'].Event(EVENT_EXPANDED$1));
    };

    _proto.toggle = function toggle() {
      var $body = $__default['default']('body');
      var shouldClose = $body.hasClass(CLASS_NAME_CONTROL_SIDEBAR_OPEN) || $body.hasClass(CLASS_NAME_CONTROL_SIDEBAR_SLIDE);

      if (shouldClose) {
        // Close the control sidebar
        this.collapse();
      } else {
        // Open the control sidebar
        this.show();
      }
    } // Private
    ;

    _proto._init = function _init() {
      var _this = this;

      this._fixHeight();

      this._fixScrollHeight();

      $__default['default'](window).on('resize', function () {
        _this._fixHeight();

        _this._fixScrollHeight();
      });
      $__default['default'](window).on('scroll', function () {
        var $body = $__default['default']('body');
        var shouldFixHeight = $body.hasClass(CLASS_NAME_CONTROL_SIDEBAR_OPEN) || $body.hasClass(CLASS_NAME_CONTROL_SIDEBAR_SLIDE);

        if (shouldFixHeight) {
          _this._fixScrollHeight();
        }
      });
    };

    _proto._fixScrollHeight = function _fixScrollHeight() {
      var $body = $__default['default']('body');

      if (!$body.hasClass(CLASS_NAME_LAYOUT_FIXED)) {
        return;
      }

      var heights = {
        scroll: $__default['default'](document).height(),
        window: $__default['default'](window).height(),
        header: $__default['default'](SELECTOR_HEADER).outerHeight(),
        footer: $__default['default'](SELECTOR_FOOTER).outerHeight()
      };
      var positions = {
        bottom: Math.abs(heights.window + $__default['default'](window).scrollTop() - heights.scroll),
        top: $__default['default'](window).scrollTop()
      };
      var navbarFixed = ($body.hasClass(CLASS_NAME_NAVBAR_FIXED) || $body.hasClass(CLASS_NAME_NAVBAR_SM_FIXED) || $body.hasClass(CLASS_NAME_NAVBAR_MD_FIXED) || $body.hasClass(CLASS_NAME_NAVBAR_LG_FIXED) || $body.hasClass(CLASS_NAME_NAVBAR_XL_FIXED)) && $__default['default'](SELECTOR_HEADER).css('position') === 'fixed';
      var footerFixed = ($body.hasClass(CLASS_NAME_FOOTER_FIXED) || $body.hasClass(CLASS_NAME_FOOTER_SM_FIXED) || $body.hasClass(CLASS_NAME_FOOTER_MD_FIXED) || $body.hasClass(CLASS_NAME_FOOTER_LG_FIXED) || $body.hasClass(CLASS_NAME_FOOTER_XL_FIXED)) && $__default['default'](SELECTOR_FOOTER).css('position') === 'fixed';
      var $controlSidebar = $__default['default'](SELECTOR_CONTROL_SIDEBAR);
      var $controlsidebarContent = $__default['default'](SELECTOR_CONTROL_SIDEBAR + " " + SELECTOR_CONTROL_SIDEBAR_CONTENT);
      var offsetBody = $__default['default']('.content').position().top;
      var offsetContent = $controlsidebarContent.position().top + offsetBody;
      var top = heights.header - positions.top;

      if (positions.top === 0 && positions.bottom === 0) {
        $controlSidebar.css({
          bottom: heights.footer,
          top: heights.header + offsetBody,
          height: heights.window - (heights.header + heights.footer) - offsetBody
        });
        $controlsidebarContent.css({
          height: heights.window - (heights.header + heights.footer) - offsetContent
        });
      } else if (positions.bottom <= heights.footer) {
        if (footerFixed === false) {
          $controlSidebar.css({
            bottom: heights.footer - positions.bottom,
            top: (top >= 0 ? top : 0) + offsetBody,
            height: heights.window - (heights.footer - positions.bottom) - offsetBody
          });
          $controlsidebarContent.css({
            height: heights.window - (heights.footer - positions.bottom) - offsetContent
          });
        } else {
          $controlSidebar.css({
            bottom: heights.footer
          });
        }
      } else if (positions.top <= heights.header) {
        if (navbarFixed === false) {
          $controlSidebar.css({
            top: heights.header - positions.top + offsetBody,
            height: heights.window - (heights.header - positions.top) - offsetBody
          });
          $controlsidebarContent.css({
            height: heights.window - (heights.header - positions.top) - offsetContent
          });
        } else {
          $controlSidebar.css({
            top: heights.header + offsetBody
          });
        }
      } else if (navbarFixed === false) {
        $controlSidebar.css({
          top: offsetBody,
          height: heights.window - offsetBody
        });
        $controlsidebarContent.css({
          height: heights.window - offsetContent - offsetBody
        });
      } else {
        $controlSidebar.css({
          top: heights.header + offsetBody
        });
      }
    };

    _proto._fixHeight = function _fixHeight() {
      var $body = $__default['default']('body');

      if (!$body.hasClass(CLASS_NAME_LAYOUT_FIXED)) {
        return;
      }

      var heights = {
        window: $__default['default'](window).height(),
        header: $__default['default'](SELECTOR_HEADER).outerHeight(),
        footer: $__default['default'](SELECTOR_FOOTER).outerHeight()
      };
      var sidebarHeight = heights.window - heights.header;
      var footerFixed = ($body.hasClass(CLASS_NAME_FOOTER_FIXED) || $body.hasClass(CLASS_NAME_FOOTER_SM_FIXED) || $body.hasClass(CLASS_NAME_FOOTER_MD_FIXED) || $body.hasClass(CLASS_NAME_FOOTER_LG_FIXED) || $body.hasClass(CLASS_NAME_FOOTER_XL_FIXED)) && $__default['default'](SELECTOR_FOOTER).css('position') === 'fixed';

      if (footerFixed) {
        sidebarHeight = heights.window - heights.header - heights.footer;
      }

      var $controlSidebar = $__default['default'](SELECTOR_CONTROL_SIDEBAR + " " + SELECTOR_CONTROL_SIDEBAR_CONTENT);
      $controlSidebar.css('height', sidebarHeight);

      if (typeof $__default['default'].fn.overlayScrollbars !== 'undefined') {
        $controlSidebar.overlayScrollbars({
          className: this._config.scrollbarTheme,
          sizeAutoCapable: true,
          scrollbars: {
            autoHide: this._config.scrollbarAutoHide,
            clickScrolling: true
          }
        });
      }
    } // Static
    ;

    ControlSidebar._jQueryInterface = function _jQueryInterface(operation) {
      return this.each(function () {
        var data = $__default['default'](this).data(DATA_KEY$2);

        var _options = $__default['default'].extend({}, Default$2, $__default['default'](this).data());

        if (!data) {
          data = new ControlSidebar(this, _options);
          $__default['default'](this).data(DATA_KEY$2, data);
        }

        if (data[operation] === 'undefined') {
          throw new Error(operation + " is not a function");
        }

        data[operation]();
      });
    };

    return ControlSidebar;
  }();
  /**
   *
   * Data Api implementation
   * ====================================================
   */


  $__default['default'](document).on('click', SELECTOR_DATA_TOGGLE, function (event) {
    event.preventDefault();

    ControlSidebar._jQueryInterface.call($__default['default'](this), 'toggle');
  });
  jQuery(function () {
    var $body = $__default['default']('body');
    var hasSidebar = $body.hasClass(CLASS_NAME_CONTROL_SIDEBAR_OPEN);
    var opened = $body.hasClass(CLASS_NAME_CONTROL_SIDEBAR_SLIDE);
    hasSidebar && ControlSidebar._jQueryInterface.call($__default['default'](this), opened ? 'show' : 'collapse');
  });
  /**
   * jQuery API
   * ====================================================
   */

  $__default['default'].fn[NAME$2] = ControlSidebar._jQueryInterface;
  $__default['default'].fn[NAME$2].Constructor = ControlSidebar;

  $__default['default'].fn[NAME$2].noConflict = function () {
    $__default['default'].fn[NAME$2] = JQUERY_NO_CONFLICT$2;
    return ControlSidebar._jQueryInterface;
  };

  /**
   * --------------------------------------------
   * AdminLTE DirectChat.js
   * License MIT
   * --------------------------------------------
   */
  /**
   * Constants
   * ====================================================
   */

  var NAME$3 = 'DirectChat';
  var DATA_KEY$3 = 'lte.directchat';
  var EVENT_KEY$3 = "." + DATA_KEY$3;
  var JQUERY_NO_CONFLICT$3 = $__default['default'].fn[NAME$3];
  var EVENT_TOGGLED = "toggled" + EVENT_KEY$3;
  var SELECTOR_DATA_TOGGLE$1 = '[data-widget="chat-pane-toggle"]';
  var SELECTOR_DIRECT_CHAT = '.direct-chat';
  var CLASS_NAME_DIRECT_CHAT_OPEN = 'direct-chat-contacts-open';
  /**
   * Class Definition
   * ====================================================
   */

  var DirectChat = /*#__PURE__*/function () {
    function DirectChat(element) {
      this._element = element;
    }

    var _proto = DirectChat.prototype;

    _proto.toggle = function toggle() {
      $__default['default'](this._element).parents(SELECTOR_DIRECT_CHAT).first().toggleClass(CLASS_NAME_DIRECT_CHAT_OPEN);
      $__default['default'](this._element).trigger($__default['default'].Event(EVENT_TOGGLED));
    } // Static
    ;

    DirectChat._jQueryInterface = function _jQueryInterface(config) {
      return this.each(function () {
        var data = $__default['default'](this).data(DATA_KEY$3);

        if (!data) {
          data = new DirectChat($__default['default'](this));
          $__default['default'](this).data(DATA_KEY$3, data);
        }

        data[config]();
      });
    };

    return DirectChat;
  }();
  /**
   *
   * Data Api implementation
   * ====================================================
   */


  $__default['default'](document).on('click', SELECTOR_DATA_TOGGLE$1, function (event) {
    if (event) {
      event.preventDefault();
    }

    DirectChat._jQueryInterface.call($__default['default'](this), 'toggle');
  });
  /**
   * jQuery API
   * ====================================================
   */

  $__default['default'].fn[NAME$3] = DirectChat._jQueryInterface;
  $__default['default'].fn[NAME$3].Constructor = DirectChat;

  $__default['default'].fn[NAME$3].noConflict = function () {
    $__default['default'].fn[NAME$3] = JQUERY_NO_CONFLICT$3;
    return DirectChat._jQueryInterface;
  };

  /**
   * --------------------------------------------
   * AdminLTE Dropdown.js
   * License MIT
   * --------------------------------------------
   */
  /**
   * Constants
   * ====================================================
   */

  var NAME$4 = 'Dropdown';
  var DATA_KEY$4 = 'lte.dropdown';
  var JQUERY_NO_CONFLICT$4 = $__default['default'].fn[NAME$4];
  var SELECTOR_NAVBAR = '.navbar';
  var SELECTOR_DROPDOWN_MENU = '.dropdown-menu';
  var SELECTOR_DROPDOWN_MENU_ACTIVE = '.dropdown-menu.show';
  var SELECTOR_DROPDOWN_TOGGLE = '[data-toggle="dropdown"]';
  var CLASS_NAME_DROPDOWN_RIGHT = 'dropdown-menu-right';
  var CLASS_NAME_DROPDOWN_SUBMENU = 'dropdown-submenu'; // TODO: this is unused; should be removed along with the extend?

  var Default$3 = {};
  /**
   * Class Definition
   * ====================================================
   */

  var Dropdown = /*#__PURE__*/function () {
    function Dropdown(element, config) {
      this._config = config;
      this._element = element;
    } // Public


    var _proto = Dropdown.prototype;

    _proto.toggleSubmenu = function toggleSubmenu() {
      this._element.siblings().show().toggleClass('show');

      if (!this._element.next().hasClass('show')) {
        this._element.parents(SELECTOR_DROPDOWN_MENU).first().find('.show').removeClass('show').hide();
      }

      this._element.parents('li.nav-item.dropdown.show').on('hidden.bs.dropdown', function () {
        $__default['default']('.dropdown-submenu .show').removeClass('show').hide();
      });
    };

    _proto.fixPosition = function fixPosition() {
      var $element = $__default['default'](SELECTOR_DROPDOWN_MENU_ACTIVE);

      if ($element.length === 0) {
        return;
      }

      if ($element.hasClass(CLASS_NAME_DROPDOWN_RIGHT)) {
        $element.css({
          left: 'inherit',
          right: 0
        });
      } else {
        $element.css({
          left: 0,
          right: 'inherit'
        });
      }

      var offset = $element.offset();
      var width = $element.width();
      var visiblePart = $__default['default'](window).width() - offset.left;

      if (offset.left < 0) {
        $element.css({
          left: 'inherit',
          right: offset.left - 5
        });
      } else if (visiblePart < width) {
        $element.css({
          left: 'inherit',
          right: 0
        });
      }
    } // Static
    ;

    Dropdown._jQueryInterface = function _jQueryInterface(config) {
      return this.each(function () {
        var data = $__default['default'](this).data(DATA_KEY$4);

        var _config = $__default['default'].extend({}, Default$3, $__default['default'](this).data());

        if (!data) {
          data = new Dropdown($__default['default'](this), _config);
          $__default['default'](this).data(DATA_KEY$4, data);
        }

        if (config === 'toggleSubmenu' || config === 'fixPosition') {
          data[config]();
        }
      });
    };

    return Dropdown;
  }();
  /**
   * Data API
   * ====================================================
   */


  $__default['default'](SELECTOR_DROPDOWN_MENU + " " + SELECTOR_DROPDOWN_TOGGLE).on('click', function (event) {
    event.preventDefault();
    event.stopPropagation();

    Dropdown._jQueryInterface.call($__default['default'](this), 'toggleSubmenu');
  });
  $__default['default'](SELECTOR_NAVBAR + " " + SELECTOR_DROPDOWN_TOGGLE).on('click', function (event) {
    event.preventDefault();

    if ($__default['default'](event.target).parent().hasClass(CLASS_NAME_DROPDOWN_SUBMENU)) {
      return;
    }

    setTimeout(function () {
      Dropdown._jQueryInterface.call($__default['default'](this), 'fixPosition');
    }, 1);
  });
  /**
   * jQuery API
   * ====================================================
   */

  $__default['default'].fn[NAME$4] = Dropdown._jQueryInterface;
  $__default['default'].fn[NAME$4].Constructor = Dropdown;

  $__default['default'].fn[NAME$4].noConflict = function () {
    $__default['default'].fn[NAME$4] = JQUERY_NO_CONFLICT$4;
    return Dropdown._jQueryInterface;
  };

  /**
   * --------------------------------------------
   * AdminLTE ExpandableTable.js
   * License MIT
   * --------------------------------------------
   */
  /**
    * Constants
    * ====================================================
    */

  var NAME$5 = 'ExpandableTable';
  var DATA_KEY$5 = 'lte.expandableTable';
  var EVENT_KEY$4 = "." + DATA_KEY$5;
  var JQUERY_NO_CONFLICT$5 = $__default['default'].fn[NAME$5];
  var EVENT_EXPANDED$2 = "expanded" + EVENT_KEY$4;
  var EVENT_COLLAPSED$2 = "collapsed" + EVENT_KEY$4;
  var SELECTOR_DATA_TOGGLE$2 = '[data-widget="expandable-table"]';
  var SELECTOR_ARIA_ATTR = 'aria-expanded';
  /**
    * Class Definition
    * ====================================================
    */

  var ExpandableTable = /*#__PURE__*/function () {
    function ExpandableTable(element, options) {
      this._options = options;
      this._element = element;
    } // Public


    var _proto = ExpandableTable.prototype;

    _proto.init = function init() {
      $__default['default'](SELECTOR_DATA_TOGGLE$2).each(function (_, $header) {
        var $type = $__default['default']($header).attr(SELECTOR_ARIA_ATTR);
        var $body = $__default['default']($header).next().children().first().children();

        if ($type === 'true') {
          $body.show();
        } else if ($type === 'false') {
          $body.hide();
          $body.parent().parent().addClass('d-none');
        }
      });
    };

    _proto.toggleRow = function toggleRow() {
      var $element = this._element;
      var time = 300;
      var $type = $element.attr(SELECTOR_ARIA_ATTR);
      var $body = $element.next().children().first().children();
      $body.stop();

      if ($type === 'true') {
        $body.slideUp(time, function () {
          $element.next().addClass('d-none');
        });
        $element.attr(SELECTOR_ARIA_ATTR, 'false');
        $element.trigger($__default['default'].Event(EVENT_COLLAPSED$2));
      } else if ($type === 'false') {
        $element.next().removeClass('d-none');
        $body.slideDown(time);
        $element.attr(SELECTOR_ARIA_ATTR, 'true');
        $element.trigger($__default['default'].Event(EVENT_EXPANDED$2));
      }
    } // Static
    ;

    ExpandableTable._jQueryInterface = function _jQueryInterface(operation) {
      return this.each(function () {
        var data = $__default['default'](this).data(DATA_KEY$5);

        if (!data) {
          data = new ExpandableTable($__default['default'](this));
          $__default['default'](this).data(DATA_KEY$5, data);
        }

        if (typeof operation === 'string' && operation.match(/init|toggleRow/)) {
          data[operation]();
        }
      });
    };

    return ExpandableTable;
  }();
  /**
    * Data API
    * ====================================================
    */


  jQuery(function () {
    if ($__default['default']('.quickShowDetail').length > 0) {
      $__default['default'](document).on("click", SELECTOR_DATA_TOGGLE$2 + ' > td > .right', function () {
        var toggler = $__default['default'](this);
        var $row = $__default['default'](this).parent().parent();
        var $template = $__default['default']('<tr />').addClass('expandable-body');
        var hasExpandableBody = $row.next().hasClass('expandable-body');
        var $expandableBody = hasExpandableBody ? $row.next() : $template.clone();

        if (!hasExpandableBody) {
          $expandableBody.insertAfter($row);
          hasExpandableBody = true;
        }

        if ($__default['default'].trim($expandableBody.html()) == '') {
          $__default['default'].ajax({
            type: "POST",
            async: true,
            url: $row.data("url"),
            data: {
              IDProfile: $row.data("id")
            },
            beforeSend: function beforeSend() {
              toggler.addClass('fa-spin fa-spinner');
            },
            success: function success(data) {
              $expandableBody.html(data);
              $expandableBody.find("td:first").attr("colspan", $row.data("colspan"));

              ExpandableTable._jQueryInterface.call($__default['default'](this), 'init');
            },
            complete: function complete() {
              toggler.removeClass('fa-spin fa-spinner');

              ExpandableTable._jQueryInterface.call($row, 'toggleRow');
            }
          });
        } else {
          ExpandableTable._jQueryInterface.call($row, 'toggleRow');
        }
      });
    } else {
      ExpandableTable._jQueryInterface.call($__default['default'](this), 'init');

      $__default['default'](document).on('click', SELECTOR_DATA_TOGGLE$2, function () {
        ExpandableTable._jQueryInterface.call($__default['default'](this), 'toggleRow');
      });
    }
  });
  /**
    * jQuery API
    * ====================================================
    */

  $__default['default'].fn[NAME$5] = ExpandableTable._jQueryInterface;
  $__default['default'].fn[NAME$5].Constructor = ExpandableTable;

  $__default['default'].fn[NAME$5].noConflict = function () {
    $__default['default'].fn[NAME$5] = JQUERY_NO_CONFLICT$5;
    return ExpandableTable._jQueryInterface;
  };

  /**
   * --------------------------------------------
   * AdminLTE Fullscreen.js
   * License MIT
   * --------------------------------------------
   */
  /**
   * Constants
   * ====================================================
   */

  var NAME$6 = 'Fullscreen';
  var DATA_KEY$6 = 'lte.fullscreen';
  var JQUERY_NO_CONFLICT$6 = $__default['default'].fn[NAME$6];
  var SELECTOR_DATA_WIDGET = '[data-widget="fullscreen"]';
  var SELECTOR_ICON = SELECTOR_DATA_WIDGET + " i";
  var Default$4 = {
    minimizeIcon: 'fa-compress-arrows-alt',
    maximizeIcon: 'fa-expand-arrows-alt'
  };
  /**
   * Class Definition
   * ====================================================
   */

  var Fullscreen = /*#__PURE__*/function () {
    function Fullscreen(_element, _options) {
      this.element = _element;
      this.options = $__default['default'].extend({}, Default$4, _options);
    } // Public


    var _proto = Fullscreen.prototype;

    _proto.toggle = function toggle() {
      if (document.fullscreenElement || document.mozFullScreenElement || document.webkitFullscreenElement || document.msFullscreenElement) {
        this.windowed();
      } else {
        this.fullscreen();
      }
    };

    _proto.fullscreen = function fullscreen() {
      if (document.documentElement.requestFullscreen) {
        document.documentElement.requestFullscreen();
      } else if (document.documentElement.webkitRequestFullscreen) {
        document.documentElement.webkitRequestFullscreen();
      } else if (document.documentElement.msRequestFullscreen) {
        document.documentElement.msRequestFullscreen();
      }

      $__default['default'](SELECTOR_ICON).removeClass(this.options.maximizeIcon).addClass(this.options.minimizeIcon);
    };

    _proto.windowed = function windowed() {
      if (document.exitFullscreen) {
        document.exitFullscreen();
      } else if (document.webkitExitFullscreen) {
        document.webkitExitFullscreen();
      } else if (document.msExitFullscreen) {
        document.msExitFullscreen();
      }

      $__default['default'](SELECTOR_ICON).removeClass(this.options.minimizeIcon).addClass(this.options.maximizeIcon);
    } // Static
    ;

    Fullscreen._jQueryInterface = function _jQueryInterface(config) {
      var data = $__default['default'](this).data(DATA_KEY$6);

      if (!data) {
        data = $__default['default'](this).data();
      }

      var _options = $__default['default'].extend({}, Default$4, typeof config === 'object' ? config : data);

      var plugin = new Fullscreen($__default['default'](this), _options);
      $__default['default'](this).data(DATA_KEY$6, typeof config === 'object' ? config : data);

      if (typeof config === 'string' && config.match(/toggle|fullscreen|windowed/)) {
        plugin[config]();
      } else {
        plugin.init();
      }
    };

    return Fullscreen;
  }();
  /**
    * Data API
    * ====================================================
    */


  $__default['default'](document).on('click', SELECTOR_DATA_WIDGET, function () {
    Fullscreen._jQueryInterface.call($__default['default'](this), 'toggle');
  });
  /**
   * jQuery API
   * ====================================================
   */

  $__default['default'].fn[NAME$6] = Fullscreen._jQueryInterface;
  $__default['default'].fn[NAME$6].Constructor = Fullscreen;

  $__default['default'].fn[NAME$6].noConflict = function () {
    $__default['default'].fn[NAME$6] = JQUERY_NO_CONFLICT$6;
    return Fullscreen._jQueryInterface;
  };

  /**
   * --------------------------------------------
   * AdminLTE Layout.js
   * License MIT
   * --------------------------------------------
   */
  /**
   * Constants
   * ====================================================
   */

  var NAME$7 = 'Layout';
  var DATA_KEY$7 = 'lte.layout';
  var JQUERY_NO_CONFLICT$7 = $__default['default'].fn[NAME$7];
  var SELECTOR_HEADER$1 = '.main-header';
  var SELECTOR_MAIN_SIDEBAR = '.main-sidebar';
  var SELECTOR_SIDEBAR = '.main-sidebar .sidebar';
  var SELECTOR_CONTENT = '.content-wrapper';
  var SELECTOR_CONTROL_SIDEBAR_CONTENT$1 = '.control-sidebar-content';
  var SELECTOR_CONTROL_SIDEBAR_BTN = '[data-widget="control-sidebar"]';
  var SELECTOR_FOOTER$1 = '.main-footer';
  var SELECTOR_PUSHMENU_BTN = '[data-widget="pushmenu"]';
  var SELECTOR_LOGIN_BOX = '.login-box';
  var SELECTOR_REGISTER_BOX = '.register-box';
  var CLASS_NAME_SIDEBAR_FOCUSED = 'sidebar-focused';
  var CLASS_NAME_LAYOUT_FIXED$1 = 'layout-fixed';
  var CLASS_NAME_CONTROL_SIDEBAR_SLIDE_OPEN = 'control-sidebar-slide-open';
  var CLASS_NAME_CONTROL_SIDEBAR_OPEN$1 = 'control-sidebar-open';
  var Default$5 = {
    scrollbarTheme: $__default['default'](SELECTOR_SIDEBAR).has('[class^="sidebar-light"]') ? 'os-theme-dark' : 'os-theme-light',
    scrollbarAutoHide: 'l',
    panelAutoHeight: true,
    loginRegisterAutoHeight: true
  };
  /**
   * Class Definition
   * ====================================================
   */

  var Layout = /*#__PURE__*/function () {
    function Layout(element, config) {
      this._config = config;
      this._element = element;

      this._init();
    } // Public


    var _proto = Layout.prototype;

    _proto.fixLayoutHeight = function fixLayoutHeight(extra) {
      if (extra === void 0) {
        extra = null;
      }

      var $body = $__default['default']('body');
      var controlSidebar = 0;

      if ($body.hasClass(CLASS_NAME_CONTROL_SIDEBAR_SLIDE_OPEN) || $body.hasClass(CLASS_NAME_CONTROL_SIDEBAR_OPEN$1) || extra === 'control_sidebar') {
        controlSidebar = $__default['default'](SELECTOR_CONTROL_SIDEBAR_CONTENT$1).height();
      }

      var heights = {
        window: $__default['default'](window).height(),
        header: $__default['default'](SELECTOR_HEADER$1).length !== 0 ? $__default['default'](SELECTOR_HEADER$1).outerHeight() : 0,
        footer: $__default['default'](SELECTOR_FOOTER$1).length !== 0 ? $__default['default'](SELECTOR_FOOTER$1).outerHeight() : 0,
        sidebar: $__default['default'](SELECTOR_SIDEBAR).length !== 0 ? $__default['default'](SELECTOR_SIDEBAR).height() : 0,
        controlSidebar: controlSidebar
      };

      var max = this._max(heights);

      var offset = this._config.panelAutoHeight;

      if (offset === true) {
        offset = 0;
      }

      var $contentSelector = $__default['default'](SELECTOR_CONTENT);

      if (offset !== false) {
        if (max === heights.controlSidebar) {
          $contentSelector.css('min-height', max + offset);
        } else if (max === heights.window) {
          $contentSelector.css('min-height', max + offset - heights.header - heights.footer);
        } else {
          $contentSelector.css('min-height', max + offset - heights.header);
        }

        if (this._isFooterFixed()) {
          $contentSelector.css('min-height', parseFloat($contentSelector.css('min-height')) + heights.footer);
        }
      }

      if (!$body.hasClass(CLASS_NAME_LAYOUT_FIXED$1)) {
        return;
      }

      if (offset !== false) {
        $contentSelector.css('min-height', max + offset - heights.header - heights.footer);
      }

      if (typeof $__default['default'].fn.overlayScrollbars !== 'undefined') {
        $__default['default'](SELECTOR_SIDEBAR).overlayScrollbars({
          className: this._config.scrollbarTheme,
          sizeAutoCapable: true,
          scrollbars: {
            autoHide: this._config.scrollbarAutoHide,
            clickScrolling: true
          }
        });
      }
    };

    _proto.fixLoginRegisterHeight = function fixLoginRegisterHeight() {
      var $body = $__default['default']('body');
      var $selector = $__default['default'](SELECTOR_LOGIN_BOX + ", " + SELECTOR_REGISTER_BOX);

      if ($selector.length === 0) {
        $body.css('height', 'auto');
        $__default['default']('html').css('height', 'auto');
      } else {
        var boxHeight = $selector.height();

        if ($body.css('min-height') !== boxHeight) {
          $body.css('min-height', boxHeight);
        }
      }
    } // Private
    ;

    _proto._init = function _init() {
      var _this = this;

      // Activate layout height watcher
      this.fixLayoutHeight();

      if (this._config.loginRegisterAutoHeight === true) {
        this.fixLoginRegisterHeight();
      } else if (this._config.loginRegisterAutoHeight === parseInt(this._config.loginRegisterAutoHeight, 10)) {
        setInterval(this.fixLoginRegisterHeight, this._config.loginRegisterAutoHeight);
      }

      $__default['default'](SELECTOR_SIDEBAR).on('collapsed.lte.treeview expanded.lte.treeview', function () {
        _this.fixLayoutHeight();
      });
      $__default['default'](SELECTOR_PUSHMENU_BTN).on('collapsed.lte.pushmenu shown.lte.pushmenu', function () {
        _this.fixLayoutHeight();
      });
      $__default['default'](SELECTOR_CONTROL_SIDEBAR_BTN).on('collapsed.lte.controlsidebar', function () {
        _this.fixLayoutHeight();
      }).on('expanded.lte.controlsidebar', function () {
        _this.fixLayoutHeight('control_sidebar');
      });
      $__default['default'](window).on('resize', function () {
        _this.fixLayoutHeight();
      });
      setTimeout(function () {
        $__default['default']('body.hold-transition').removeClass('hold-transition');
      }, 50);
    };

    _proto._max = function _max(numbers) {
      // Calculate the maximum number in a list
      var max = 0;
      Object.keys(numbers).forEach(function (key) {
        if (numbers[key] > max) {
          max = numbers[key];
        }
      });
      return max;
    };

    _proto._isFooterFixed = function _isFooterFixed() {
      return $__default['default'](SELECTOR_FOOTER$1).css('position') === 'fixed';
    } // Static
    ;

    Layout._jQueryInterface = function _jQueryInterface(config) {
      if (config === void 0) {
        config = '';
      }

      return this.each(function () {
        var data = $__default['default'](this).data(DATA_KEY$7);

        var _options = $__default['default'].extend({}, Default$5, $__default['default'](this).data());

        if (!data) {
          data = new Layout($__default['default'](this), _options);
          $__default['default'](this).data(DATA_KEY$7, data);
        }

        if (config === 'init' || config === '') {
          data._init();
        } else if (config === 'fixLayoutHeight' || config === 'fixLoginRegisterHeight') {
          data[config]();
        }
      });
    };

    return Layout;
  }();
  /**
   * Data API
   * ====================================================
   */


  $__default['default'](window).on('load', function () {
    Layout._jQueryInterface.call($__default['default']('body'));
  });
  $__default['default'](SELECTOR_SIDEBAR + " a").on('focusin', function () {
    $__default['default'](SELECTOR_MAIN_SIDEBAR).addClass(CLASS_NAME_SIDEBAR_FOCUSED);
  });
  $__default['default'](SELECTOR_SIDEBAR + " a").on('focusout', function () {
    $__default['default'](SELECTOR_MAIN_SIDEBAR).removeClass(CLASS_NAME_SIDEBAR_FOCUSED);
  });
  /**
   * jQuery API
   * ====================================================
   */

  $__default['default'].fn[NAME$7] = Layout._jQueryInterface;
  $__default['default'].fn[NAME$7].Constructor = Layout;

  $__default['default'].fn[NAME$7].noConflict = function () {
    $__default['default'].fn[NAME$7] = JQUERY_NO_CONFLICT$7;
    return Layout._jQueryInterface;
  };

  /**
   * --------------------------------------------
   * AdminLTE PushMenu.js
   * License MIT
   * --------------------------------------------
   */
  /**
   * Constants
   * ====================================================
   */

  var NAME$8 = 'PushMenu';
  var DATA_KEY$8 = 'lte.pushmenu';
  var EVENT_KEY$5 = "." + DATA_KEY$8;
  var JQUERY_NO_CONFLICT$8 = $__default['default'].fn[NAME$8];
  var EVENT_COLLAPSED$3 = "collapsed" + EVENT_KEY$5;
  var EVENT_SHOWN = "shown" + EVENT_KEY$5;
  var SELECTOR_TOGGLE_BUTTON = '[data-widget="pushmenu"]';
  var SELECTOR_BODY = 'body';
  var SELECTOR_OVERLAY = '#sidebar-overlay';
  var SELECTOR_WRAPPER = '.wrapper';
  var CLASS_NAME_COLLAPSED$1 = 'sidebar-collapse';
  var CLASS_NAME_OPEN = 'sidebar-open';
  var CLASS_NAME_IS_OPENING = 'sidebar-is-opening';
  var CLASS_NAME_CLOSED = 'sidebar-closed';
  var Default$6 = {
    autoCollapseSize: 1280,
    enableRemember: false,
    noTransitionAfterReload: true
  };
  /**
   * Class Definition
   * ====================================================
   */

  var PushMenu = /*#__PURE__*/function () {
    function PushMenu(element, options) {
      this._element = element;
      this._options = $__default['default'].extend({}, Default$6, options);

      if ($__default['default'](SELECTOR_OVERLAY).length === 0) {
        this._addOverlay();
      }

      this._init();
    } // Public


    var _proto = PushMenu.prototype;

    _proto.expand = function expand() {
      var $bodySelector = $__default['default'](SELECTOR_BODY);

      if (this._options.autoCollapseSize) {
        if ($__default['default'](window).width() <= this._options.autoCollapseSize) {
          $bodySelector.addClass(CLASS_NAME_OPEN);
        }
      }

      $bodySelector.addClass(CLASS_NAME_IS_OPENING).removeClass(CLASS_NAME_COLLAPSED$1 + " " + CLASS_NAME_CLOSED).delay(50).queue(function () {
        $bodySelector.removeClass(CLASS_NAME_IS_OPENING);
        $__default['default'](this).dequeue();
      });

      if (this._options.enableRemember) {
        localStorage.setItem("remember" + EVENT_KEY$5, CLASS_NAME_OPEN);
      }

      $__default['default'](this._element).trigger($__default['default'].Event(EVENT_SHOWN));
    };

    _proto.collapse = function collapse() {
      var $bodySelector = $__default['default'](SELECTOR_BODY);

      if (this._options.autoCollapseSize) {
        if ($__default['default'](window).width() <= this._options.autoCollapseSize) {
          $bodySelector.removeClass(CLASS_NAME_OPEN).addClass(CLASS_NAME_CLOSED);
        }
      }

      $bodySelector.addClass(CLASS_NAME_COLLAPSED$1);

      if (this._options.enableRemember) {
        localStorage.setItem("remember" + EVENT_KEY$5, CLASS_NAME_COLLAPSED$1);
      }

      $__default['default'](this._element).trigger($__default['default'].Event(EVENT_COLLAPSED$3));
    };

    _proto.toggle = function toggle() {
      if ($__default['default'](SELECTOR_BODY).hasClass(CLASS_NAME_COLLAPSED$1)) {
        this.expand();
      } else {
        this.collapse();
      }
    };

    _proto.autoCollapse = function autoCollapse(resize) {
      if (resize === void 0) {
        resize = false;
      }

      if (!this._options.autoCollapseSize) {
        return;
      }

      var $bodySelector = $__default['default'](SELECTOR_BODY);

      if ($__default['default'](window).width() <= this._options.autoCollapseSize) {
        if (!$bodySelector.hasClass(CLASS_NAME_OPEN)) {
          this.collapse();
        }
      } else if (resize === true) {
        if ($bodySelector.hasClass(CLASS_NAME_OPEN)) {
          $bodySelector.removeClass(CLASS_NAME_OPEN);
        } else if ($bodySelector.hasClass(CLASS_NAME_CLOSED)) {
          this.expand();
        }
      }
    };

    _proto.remember = function remember() {
      if (!this._options.enableRemember) {
        return;
      }

      var $body = $__default['default']('body');
      var toggleState = localStorage.getItem("remember" + EVENT_KEY$5);

      if (toggleState === CLASS_NAME_COLLAPSED$1) {
        if (this._options.noTransitionAfterReload) {
          $body.addClass('hold-transition').addClass(CLASS_NAME_COLLAPSED$1).delay(50).queue(function () {
            $__default['default'](this).removeClass('hold-transition');
            $__default['default'](this).dequeue();
          });
        } else {
          $body.addClass(CLASS_NAME_COLLAPSED$1);
        }
      } else if (this._options.noTransitionAfterReload) {
        $body.addClass('hold-transition').removeClass(CLASS_NAME_COLLAPSED$1).delay(50).queue(function () {
          $__default['default'](this).removeClass('hold-transition');
          $__default['default'](this).dequeue();
        });
      } else {
        $body.removeClass(CLASS_NAME_COLLAPSED$1);
      }
    } // Private
    ;

    _proto._init = function _init() {
      var _this = this;

      this.remember();
      this.autoCollapse();
      $__default['default'](window).on('resize', function () {
        _this.autoCollapse(true);
      });
    };

    _proto._addOverlay = function _addOverlay() {
      var _this2 = this;

      var overlay = $__default['default']('<div />', {
        id: 'sidebar-overlay'
      });
      overlay.on('click', function () {
        _this2.collapse();
      });
      $__default['default'](SELECTOR_WRAPPER).append(overlay);
    } // Static
    ;

    PushMenu._jQueryInterface = function _jQueryInterface(operation) {
      return this.each(function () {
        var data = $__default['default'](this).data(DATA_KEY$8);

        var _options = $__default['default'].extend({}, Default$6, $__default['default'](this).data());

        if (!data) {
          data = new PushMenu(this, _options);
          $__default['default'](this).data(DATA_KEY$8, data);
        }

        if (typeof operation === 'string' && operation.match(/collapse|expand|toggle/)) {
          data[operation]();
        }
      });
    };

    return PushMenu;
  }();
  /**
   * Data API
   * ====================================================
   */


  $__default['default'](document).on('click', SELECTOR_TOGGLE_BUTTON, function (event) {
    event.preventDefault();
    var button = event.currentTarget;

    if ($__default['default'](button).data('widget') !== 'pushmenu') {
      button = $__default['default'](button).closest(SELECTOR_TOGGLE_BUTTON);
    }

    PushMenu._jQueryInterface.call($__default['default'](button), 'toggle');
  });
  $__default['default'](window).on('load', function () {
    PushMenu._jQueryInterface.call($__default['default'](SELECTOR_TOGGLE_BUTTON));
  });
  /**
   * jQuery API
   * ====================================================
   */

  $__default['default'].fn[NAME$8] = PushMenu._jQueryInterface;
  $__default['default'].fn[NAME$8].Constructor = PushMenu;

  $__default['default'].fn[NAME$8].noConflict = function () {
    $__default['default'].fn[NAME$8] = JQUERY_NO_CONFLICT$8;
    return PushMenu._jQueryInterface;
  };

  /**
   * --------------------------------------------
   * AdminLTE SidebarSearch.js
   * License MIT
   * --------------------------------------------
   */
  /**
   * Constants
   * ====================================================
   */

  var NAME$9 = 'SidebarSearch';
  var DATA_KEY$9 = 'lte.sidebar-search';
  var JQUERY_NO_CONFLICT$9 = $__default['default'].fn[NAME$9];
  var CLASS_NAME_OPEN$1 = 'sidebar-search-open';
  var CLASS_NAME_ICON_SEARCH = 'fa-search';
  var CLASS_NAME_ICON_CLOSE = 'fa-times';
  var CLASS_NAME_HEADER = 'nav-header';
  var CLASS_NAME_SEARCH_RESULTS = 'sidebar-search-results';
  var CLASS_NAME_LIST_GROUP = 'list-group';
  var SELECTOR_DATA_WIDGET$1 = '[data-widget="sidebar-search"]';
  var SELECTOR_SIDEBAR$1 = '.main-sidebar .nav-sidebar';
  var SELECTOR_NAV_LINK = '.nav-link';
  var SELECTOR_NAV_TREEVIEW = '.nav-treeview';
  var SELECTOR_SEARCH_INPUT = SELECTOR_DATA_WIDGET$1 + " .form-control";
  var SELECTOR_SEARCH_BUTTON = SELECTOR_DATA_WIDGET$1 + " .btn";
  var SELECTOR_SEARCH_ICON = SELECTOR_SEARCH_BUTTON + " i";
  var SELECTOR_SEARCH_LIST_GROUP = "." + CLASS_NAME_LIST_GROUP;
  var SELECTOR_SEARCH_RESULTS = "." + CLASS_NAME_SEARCH_RESULTS;
  var SELECTOR_SEARCH_RESULTS_GROUP = SELECTOR_SEARCH_RESULTS + " ." + CLASS_NAME_LIST_GROUP;
  var Default$7 = {
    arrowSign: '->',
    minLength: 3,
    maxResults: 7,
    highlightName: true,
    highlightPath: false,
    highlightClass: 'text-light',
    notFoundText: 'No element found!'
  };
  var SearchItems = [];
  /**
   * Class Definition
   * ====================================================
   */

  var SidebarSearch = /*#__PURE__*/function () {
    function SidebarSearch(_element, _options) {
      this.element = _element;
      this.options = $__default['default'].extend({}, Default$7, _options);
      this.items = [];
    } // Public


    var _proto = SidebarSearch.prototype;

    _proto.init = function init() {
      var _this = this;

      if ($__default['default'](SELECTOR_DATA_WIDGET$1).length == 0) {
        return;
      }

      if ($__default['default'](SELECTOR_DATA_WIDGET$1).next(SELECTOR_SEARCH_RESULTS).length == 0) {
        $__default['default'](SELECTOR_DATA_WIDGET$1).after($__default['default']('<div />', {
          class: CLASS_NAME_SEARCH_RESULTS
        }));
      }

      if ($__default['default'](SELECTOR_SEARCH_RESULTS).children(SELECTOR_SEARCH_LIST_GROUP).length == 0) {
        $__default['default'](SELECTOR_SEARCH_RESULTS).append($__default['default']('<div />', {
          class: CLASS_NAME_LIST_GROUP
        }));
      }

      this._addNotFound();

      $__default['default'](SELECTOR_SIDEBAR$1).children().each(function (i, child) {
        _this._parseItem(child);
      });
    };

    _proto.search = function search() {
      var _this2 = this;

      var searchValue = $__default['default'](SELECTOR_SEARCH_INPUT).val().toLowerCase();

      if (searchValue.length < this.options.minLength) {
        $__default['default'](SELECTOR_SEARCH_RESULTS_GROUP).empty();

        this._addNotFound();

        this.close();
        return;
      }

      var searchResults = SearchItems.filter(function (item) {
        return item.name.toLowerCase().includes(searchValue);
      });
      var endResults = $__default['default'](searchResults.slice(0, this.options.maxResults));
      $__default['default'](SELECTOR_SEARCH_RESULTS_GROUP).empty();

      if (endResults.length === 0) {
        this._addNotFound();
      } else {
        endResults.each(function (i, result) {
          $__default['default'](SELECTOR_SEARCH_RESULTS_GROUP).append(_this2._renderItem(result.name, result.link, result.path));
        });
      }

      this.open();
    };

    _proto.open = function open() {
      $__default['default'](SELECTOR_DATA_WIDGET$1).parent().addClass(CLASS_NAME_OPEN$1);
      $__default['default'](SELECTOR_SEARCH_ICON).removeClass(CLASS_NAME_ICON_SEARCH).addClass(CLASS_NAME_ICON_CLOSE);
    };

    _proto.close = function close() {
      $__default['default'](SELECTOR_DATA_WIDGET$1).parent().removeClass(CLASS_NAME_OPEN$1);
      $__default['default'](SELECTOR_SEARCH_ICON).removeClass(CLASS_NAME_ICON_CLOSE).addClass(CLASS_NAME_ICON_SEARCH);
    };

    _proto.toggle = function toggle() {
      if ($__default['default'](SELECTOR_DATA_WIDGET$1).parent().hasClass(CLASS_NAME_OPEN$1)) {
        this.close();
      } else {
        this.open();
      }
    } // Private
    ;

    _proto._parseItem = function _parseItem(item, path) {
      var _this3 = this;

      if (path === void 0) {
        path = [];
      }

      if ($__default['default'](item).hasClass(CLASS_NAME_HEADER)) {
        return;
      }

      var itemObject = {};
      var navLink = $__default['default'](item).clone().find("> " + SELECTOR_NAV_LINK);
      var navTreeview = $__default['default'](item).clone().find("> " + SELECTOR_NAV_TREEVIEW);
      var link = navLink.attr('href');
      var name = navLink.find('p').children().remove().end().text();
      itemObject.name = this._trimText(name);
      itemObject.link = link;
      itemObject.path = path;

      if (navTreeview.length === 0) {
        SearchItems.push(itemObject);
      } else {
        var newPath = itemObject.path.concat([itemObject.name]);
        navTreeview.children().each(function (i, child) {
          _this3._parseItem(child, newPath);
        });
      }
    };

    _proto._trimText = function _trimText(text) {
      return $$1.trim(text.replace(/(\r\n|\n|\r)/gm, ' '));
    };

    _proto._renderItem = function _renderItem(name, link, path) {
      var _this4 = this;

      path = path.join(" " + this.options.arrowSign + " ");

      if (this.options.highlightName || this.options.highlightPath) {
        var searchValue = $__default['default'](SELECTOR_SEARCH_INPUT).val().toLowerCase();
        var regExp = new RegExp(searchValue, 'gi');

        if (this.options.highlightName) {
          name = name.replace(regExp, function (str) {
            return "<b class=\"" + _this4.options.highlightClass + "\">" + str + "</b>";
          });
        }

        if (this.options.highlightPath) {
          path = path.replace(regExp, function (str) {
            return "<b class=\"" + _this4.options.highlightClass + "\">" + str + "</b>";
          });
        }
      }

      return "<a href=\"" + link + "\" class=\"list-group-item\">\n        <div class=\"search-title\">\n          " + name + "\n        </div>\n        <div class=\"search-path\">\n          " + path + "\n        </div>\n      </a>";
    };

    _proto._addNotFound = function _addNotFound() {
      $__default['default'](SELECTOR_SEARCH_RESULTS_GROUP).append(this._renderItem(this.options.notFoundText, '#', []));
    } // Static
    ;

    SidebarSearch._jQueryInterface = function _jQueryInterface(config) {
      var data = $__default['default'](this).data(DATA_KEY$9);

      if (!data) {
        data = $__default['default'](this).data();
      }

      var _options = $__default['default'].extend({}, Default$7, typeof config === 'object' ? config : data);

      var plugin = new SidebarSearch($__default['default'](this), _options);
      $__default['default'](this).data(DATA_KEY$9, typeof config === 'object' ? config : data);

      if (typeof config === 'string' && config.match(/init|toggle|close|open|search/)) {
        plugin[config]();
      } else {
        plugin.init();
      }
    };

    return SidebarSearch;
  }();
  /**
   * Data API
   * ====================================================
   */


  $__default['default'](document).on('click', SELECTOR_SEARCH_BUTTON, function (event) {
    event.preventDefault();

    SidebarSearch._jQueryInterface.call($__default['default'](SELECTOR_DATA_WIDGET$1), 'toggle');
  });
  $__default['default'](document).on('keyup', SELECTOR_SEARCH_INPUT, function (event) {
    if (event.keyCode == 38) {
      event.preventDefault();
      $__default['default'](SELECTOR_SEARCH_RESULTS_GROUP).children().last().focus();
      return;
    }

    if (event.keyCode == 40) {
      event.preventDefault();
      $__default['default'](SELECTOR_SEARCH_RESULTS_GROUP).children().first().focus();
      return;
    }

    var timer = 0;
    clearTimeout(timer);
    timer = setTimeout(function () {
      SidebarSearch._jQueryInterface.call($__default['default'](SELECTOR_DATA_WIDGET$1), 'search');
    }, 100);
  });
  $__default['default'](document).on('keydown', SELECTOR_SEARCH_RESULTS_GROUP, function (event) {
    var $focused = $__default['default'](':focus');

    if (event.keyCode == 38) {
      event.preventDefault();

      if ($focused.is(':first-child')) {
        $focused.siblings().last().focus();
      } else {
        $focused.prev().focus();
      }
    }

    if (event.keyCode == 40) {
      event.preventDefault();

      if ($focused.is(':last-child')) {
        $focused.siblings().first().focus();
      } else {
        $focused.next().focus();
      }
    }
  });
  $__default['default'](window).on('load', function () {
    SidebarSearch._jQueryInterface.call($__default['default'](SELECTOR_DATA_WIDGET$1), 'init');
  });
  /**
   * jQuery API
   * ====================================================
   */

  $__default['default'].fn[NAME$9] = SidebarSearch._jQueryInterface;
  $__default['default'].fn[NAME$9].Constructor = SidebarSearch;

  $__default['default'].fn[NAME$9].noConflict = function () {
    $__default['default'].fn[NAME$9] = JQUERY_NO_CONFLICT$9;
    return SidebarSearch._jQueryInterface;
  };

  /**
   * --------------------------------------------
   * AdminLTE Toasts.js
   * License MIT
   * --------------------------------------------
   */
  /**
   * Constants
   * ====================================================
   */

  var NAME$a = 'Toasts';
  var DATA_KEY$a = 'lte.toasts';
  var EVENT_KEY$6 = "." + DATA_KEY$a;
  var JQUERY_NO_CONFLICT$a = $__default['default'].fn[NAME$a];
  var EVENT_INIT = "init" + EVENT_KEY$6;
  var EVENT_CREATED = "created" + EVENT_KEY$6;
  var EVENT_REMOVED$1 = "removed" + EVENT_KEY$6;
  var SELECTOR_CONTAINER_TOP_RIGHT = '#toastsContainerTopRight';
  var SELECTOR_CONTAINER_TOP_LEFT = '#toastsContainerTopLeft';
  var SELECTOR_CONTAINER_BOTTOM_RIGHT = '#toastsContainerBottomRight';
  var SELECTOR_CONTAINER_BOTTOM_LEFT = '#toastsContainerBottomLeft';
  var CLASS_NAME_TOP_RIGHT = 'toasts-top-right';
  var CLASS_NAME_TOP_LEFT = 'toasts-top-left';
  var CLASS_NAME_BOTTOM_RIGHT = 'toasts-bottom-right';
  var CLASS_NAME_BOTTOM_LEFT = 'toasts-bottom-left';
  var POSITION_TOP_RIGHT = 'topRight';
  var POSITION_TOP_LEFT = 'topLeft';
  var POSITION_BOTTOM_RIGHT = 'bottomRight';
  var POSITION_BOTTOM_LEFT = 'bottomLeft';
  var Default$8 = {
    position: POSITION_TOP_RIGHT,
    fixed: true,
    autohide: false,
    autoremove: true,
    delay: 1000,
    fade: true,
    icon: null,
    image: null,
    imageAlt: null,
    imageHeight: '25px',
    title: null,
    subtitle: null,
    close: true,
    body: null,
    class: null
  };
  /**
   * Class Definition
   * ====================================================
   */

  var Toasts = /*#__PURE__*/function () {
    function Toasts(element, config) {
      this._config = config;

      this._prepareContainer();

      $__default['default']('body').trigger($__default['default'].Event(EVENT_INIT));
    } // Public


    var _proto = Toasts.prototype;

    _proto.create = function create() {
      var toast = $__default['default']('<div class="toast" role="alert" aria-live="assertive" aria-atomic="true"/>');
      toast.data('autohide', this._config.autohide);
      toast.data('animation', this._config.fade);

      if (this._config.class) {
        toast.addClass(this._config.class);
      }

      if (this._config.delay && this._config.delay != 500) {
        toast.data('delay', this._config.delay);
      }

      var toastHeader = $__default['default']('<div class="toast-header">');

      if (this._config.image != null) {
        var toastImage = $__default['default']('<img />').addClass('rounded mr-2').attr('src', this._config.image).attr('alt', this._config.imageAlt);

        if (this._config.imageHeight != null) {
          toastImage.height(this._config.imageHeight).width('auto');
        }

        toastHeader.append(toastImage);
      }

      if (this._config.icon != null) {
        toastHeader.append($__default['default']('<i />').addClass('mr-2').addClass(this._config.icon));
      }

      if (this._config.title != null) {
        toastHeader.append($__default['default']('<strong />').addClass('mr-auto').html(this._config.title));
      }

      if (this._config.subtitle != null) {
        toastHeader.append($__default['default']('<small />').html(this._config.subtitle));
      }

      if (this._config.close == true) {
        var toastClose = $__default['default']('<button data-dismiss="toast" />').attr('type', 'button').addClass('ml-2 mb-1 close').attr('aria-label', 'Close').append('<span aria-hidden="true">&times;</span>');

        if (this._config.title == null) {
          toastClose.toggleClass('ml-2 ml-auto');
        }

        toastHeader.append(toastClose);
      }

      toast.append(toastHeader);

      if (this._config.body != null) {
        toast.append($__default['default']('<div class="toast-body" />').html(this._config.body));
      }

      $__default['default'](this._getContainerId()).prepend(toast);
      var $body = $__default['default']('body');
      $body.trigger($__default['default'].Event(EVENT_CREATED));
      toast.toast('show');

      if (this._config.autoremove) {
        toast.on('hidden.bs.toast', function () {
          $__default['default'](this).delay(200).remove();
          $body.trigger($__default['default'].Event(EVENT_REMOVED$1));
        });
      }
    } // Static
    ;

    _proto._getContainerId = function _getContainerId() {
      if (this._config.position == POSITION_TOP_RIGHT) {
        return SELECTOR_CONTAINER_TOP_RIGHT;
      }

      if (this._config.position == POSITION_TOP_LEFT) {
        return SELECTOR_CONTAINER_TOP_LEFT;
      }

      if (this._config.position == POSITION_BOTTOM_RIGHT) {
        return SELECTOR_CONTAINER_BOTTOM_RIGHT;
      }

      if (this._config.position == POSITION_BOTTOM_LEFT) {
        return SELECTOR_CONTAINER_BOTTOM_LEFT;
      }
    };

    _proto._prepareContainer = function _prepareContainer() {
      if ($__default['default'](this._getContainerId()).length === 0) {
        var container = $__default['default']('<div />').attr('id', this._getContainerId().replace('#', ''));

        if (this._config.position == POSITION_TOP_RIGHT) {
          container.addClass(CLASS_NAME_TOP_RIGHT);
        } else if (this._config.position == POSITION_TOP_LEFT) {
          container.addClass(CLASS_NAME_TOP_LEFT);
        } else if (this._config.position == POSITION_BOTTOM_RIGHT) {
          container.addClass(CLASS_NAME_BOTTOM_RIGHT);
        } else if (this._config.position == POSITION_BOTTOM_LEFT) {
          container.addClass(CLASS_NAME_BOTTOM_LEFT);
        }

        $__default['default']('body').append(container);
      }

      if (this._config.fixed) {
        $__default['default'](this._getContainerId()).addClass('fixed');
      } else {
        $__default['default'](this._getContainerId()).removeClass('fixed');
      }
    } // Static
    ;

    Toasts._jQueryInterface = function _jQueryInterface(option, config) {
      return this.each(function () {
        var _options = $__default['default'].extend({}, Default$8, config);

        var toast = new Toasts($__default['default'](this), _options);

        if (option === 'create') {
          toast[option]();
        }
      });
    };

    return Toasts;
  }();
  /**
   * jQuery API
   * ====================================================
   */


  $__default['default'].fn[NAME$a] = Toasts._jQueryInterface;
  $__default['default'].fn[NAME$a].Constructor = Toasts;

  $__default['default'].fn[NAME$a].noConflict = function () {
    $__default['default'].fn[NAME$a] = JQUERY_NO_CONFLICT$a;
    return Toasts._jQueryInterface;
  };

  /**
   * --------------------------------------------
   * AdminLTE TodoList.js
   * License MIT
   * --------------------------------------------
   */
  /**
   * Constants
   * ====================================================
   */

  var NAME$b = 'TodoList';
  var DATA_KEY$b = 'lte.todolist';
  var JQUERY_NO_CONFLICT$b = $__default['default'].fn[NAME$b];
  var SELECTOR_DATA_TOGGLE$3 = '[data-widget="todo-list"]';
  var CLASS_NAME_TODO_LIST_DONE = 'done';
  var Default$9 = {
    onCheck: function onCheck(item) {
      return item;
    },
    onUnCheck: function onUnCheck(item) {
      return item;
    }
  };
  /**
   * Class Definition
   * ====================================================
   */

  var TodoList = /*#__PURE__*/function () {
    function TodoList(element, config) {
      this._config = config;
      this._element = element;

      this._init();
    } // Public


    var _proto = TodoList.prototype;

    _proto.toggle = function toggle(item) {
      item.parents('li').toggleClass(CLASS_NAME_TODO_LIST_DONE);

      if (!$__default['default'](item).prop('checked')) {
        this.unCheck($__default['default'](item));
        return;
      }

      this.check(item);
    };

    _proto.check = function check(item) {
      this._config.onCheck.call(item);
    };

    _proto.unCheck = function unCheck(item) {
      this._config.onUnCheck.call(item);
    } // Private
    ;

    _proto._init = function _init() {
      var _this = this;

      var $toggleSelector = this._element;
      $toggleSelector.find('input:checkbox:checked').parents('li').toggleClass(CLASS_NAME_TODO_LIST_DONE);
      $toggleSelector.on('change', 'input:checkbox', function (event) {
        _this.toggle($__default['default'](event.target));
      });
    } // Static
    ;

    TodoList._jQueryInterface = function _jQueryInterface(config) {
      return this.each(function () {
        var data = $__default['default'](this).data(DATA_KEY$b);

        if (!data) {
          data = $__default['default'](this).data();
        }

        var _options = $__default['default'].extend({}, Default$9, typeof config === 'object' ? config : data);

        var plugin = new TodoList($__default['default'](this), _options);
        $__default['default'](this).data(DATA_KEY$b, typeof config === 'object' ? config : data);

        if (config === 'init') {
          plugin[config]();
        }
      });
    };

    return TodoList;
  }();
  /**
   * Data API
   * ====================================================
   */


  $__default['default'](window).on('load', function () {
    TodoList._jQueryInterface.call($__default['default'](SELECTOR_DATA_TOGGLE$3));
  });
  /**
   * jQuery API
   * ====================================================
   */

  $__default['default'].fn[NAME$b] = TodoList._jQueryInterface;
  $__default['default'].fn[NAME$b].Constructor = TodoList;

  $__default['default'].fn[NAME$b].noConflict = function () {
    $__default['default'].fn[NAME$b] = JQUERY_NO_CONFLICT$b;
    return TodoList._jQueryInterface;
  };

  /**
   * --------------------------------------------
   * AdminLTE Treeview.js
   * License MIT
   * --------------------------------------------
   */
  /**
   * Constants
   * ====================================================
   */

  var NAME$c = 'Treeview';
  var DATA_KEY$c = 'lte.treeview';
  var EVENT_KEY$7 = "." + DATA_KEY$c;
  var JQUERY_NO_CONFLICT$c = $__default['default'].fn[NAME$c];
  var EVENT_EXPANDED$3 = "expanded" + EVENT_KEY$7;
  var EVENT_COLLAPSED$4 = "collapsed" + EVENT_KEY$7;
  var EVENT_LOAD_DATA_API = "load" + EVENT_KEY$7;
  var SELECTOR_LI = '.nav-item';
  var SELECTOR_LINK = '.nav-link';
  var SELECTOR_TREEVIEW_MENU = '.nav-treeview';
  var SELECTOR_OPEN = '.menu-open';
  var SELECTOR_DATA_WIDGET$2 = '[data-widget="treeview"]';
  var CLASS_NAME_OPEN$2 = 'menu-open';
  var CLASS_NAME_IS_OPENING$1 = 'menu-is-opening';
  var CLASS_NAME_SIDEBAR_COLLAPSED = 'sidebar-collapse';
  var Default$a = {
    trigger: SELECTOR_DATA_WIDGET$2 + " " + SELECTOR_LINK,
    animationSpeed: 300,
    accordion: true,
    expandSidebar: false,
    sidebarButtonSelector: '[data-widget="pushmenu"]'
  };
  /**
   * Class Definition
   * ====================================================
   */

  var Treeview = /*#__PURE__*/function () {
    function Treeview(element, config) {
      this._config = config;
      this._element = element;
    } // Public


    var _proto = Treeview.prototype;

    _proto.init = function init() {
      $__default['default']("" + SELECTOR_LI + SELECTOR_OPEN + " " + SELECTOR_TREEVIEW_MENU).css('display', 'block');

      this._setupListeners();
    };

    _proto.expand = function expand(treeviewMenu, parentLi) {
      var _this = this;

      var expandedEvent = $__default['default'].Event(EVENT_EXPANDED$3);

      if (this._config.accordion) {
        var openMenuLi = parentLi.siblings(SELECTOR_OPEN).first();
        var openTreeview = openMenuLi.find(SELECTOR_TREEVIEW_MENU).first();
        this.collapse(openTreeview, openMenuLi);
      }

      parentLi.addClass(CLASS_NAME_IS_OPENING$1);
      treeviewMenu.stop().slideDown(this._config.animationSpeed, function () {
        parentLi.addClass(CLASS_NAME_OPEN$2);
        $__default['default'](_this._element).trigger(expandedEvent);
      });

      if (this._config.expandSidebar) {
        this._expandSidebar();
      }
    };

    _proto.collapse = function collapse(treeviewMenu, parentLi) {
      var _this2 = this;

      var collapsedEvent = $__default['default'].Event(EVENT_COLLAPSED$4);
      parentLi.removeClass(CLASS_NAME_IS_OPENING$1 + " " + CLASS_NAME_OPEN$2);
      treeviewMenu.stop().slideUp(this._config.animationSpeed, function () {
        $__default['default'](_this2._element).trigger(collapsedEvent);
        treeviewMenu.find(SELECTOR_OPEN + " > " + SELECTOR_TREEVIEW_MENU).slideUp();
        treeviewMenu.find(SELECTOR_OPEN).removeClass(CLASS_NAME_OPEN$2);
      });
    };

    _proto.toggle = function toggle(event) {
      var $relativeTarget = $__default['default'](event.currentTarget);
      var $parent = $relativeTarget.parent();
      var treeviewMenu = $parent.find("> " + SELECTOR_TREEVIEW_MENU);

      if (!treeviewMenu.is(SELECTOR_TREEVIEW_MENU)) {
        if (!$parent.is(SELECTOR_LI)) {
          treeviewMenu = $parent.parent().find("> " + SELECTOR_TREEVIEW_MENU);
        }

        if (!treeviewMenu.is(SELECTOR_TREEVIEW_MENU)) {
          return;
        }
      }

      event.preventDefault();
      var parentLi = $relativeTarget.parents(SELECTOR_LI).first();
      var isOpen = parentLi.hasClass(CLASS_NAME_OPEN$2);

      if (isOpen) {
        this.collapse($__default['default'](treeviewMenu), parentLi);
      } else {
        this.expand($__default['default'](treeviewMenu), parentLi);
      }
    } // Private
    ;

    _proto._setupListeners = function _setupListeners() {
      var _this3 = this;

      var elementId = this._element.attr('id') !== undefined ? "#" + this._element.attr('id') : '';
      $__default['default'](document).on('click', "" + elementId + this._config.trigger, function (event) {
        _this3.toggle(event);
      });
    };

    _proto._expandSidebar = function _expandSidebar() {
      if ($__default['default']('body').hasClass(CLASS_NAME_SIDEBAR_COLLAPSED)) {
        $__default['default'](this._config.sidebarButtonSelector).PushMenu('expand');
      }
    } // Static
    ;

    Treeview._jQueryInterface = function _jQueryInterface(config) {
      return this.each(function () {
        var data = $__default['default'](this).data(DATA_KEY$c);

        var _options = $__default['default'].extend({}, Default$a, $__default['default'](this).data());

        if (!data) {
          data = new Treeview($__default['default'](this), _options);
          $__default['default'](this).data(DATA_KEY$c, data);
        }

        if (config === 'init') {
          data[config]();
        }
      });
    };

    return Treeview;
  }();
  /**
   * Data API
   * ====================================================
   */


  $__default['default'](window).on(EVENT_LOAD_DATA_API, function () {
    $__default['default'](SELECTOR_DATA_WIDGET$2).each(function () {
      Treeview._jQueryInterface.call($__default['default'](this), 'init');
    });
  });
  /**
   * jQuery API
   * ====================================================
   */

  $__default['default'].fn[NAME$c] = Treeview._jQueryInterface;
  $__default['default'].fn[NAME$c].Constructor = Treeview;

  $__default['default'].fn[NAME$c].noConflict = function () {
    $__default['default'].fn[NAME$c] = JQUERY_NO_CONFLICT$c;
    return Treeview._jQueryInterface;
  };

  /**
   * jquery-bootstrap-scrolling-tabs
   * @version v2.6.1
   * @link https://github.com/mikejacobson/jquery-bootstrap-scrolling-tabs
   * @author Mike Jacobson <michaeljjacobson1@gmail.com>
   * @license MIT License, http://www.opensource.org/licenses/MIT
   */

  (function ($, window) {
    /* jshint unused:false */

    /* exported CONSTANTS */

    var CONSTANTS = {
      CONTINUOUS_SCROLLING_TIMEOUT_INTERVAL: 50,
      // timeout interval for repeatedly moving the tabs container
      // by one increment while the mouse is held down--decrease to
      // make mousedown continous scrolling faster
      SCROLL_OFFSET_FRACTION: 6,
      // each click moves the container this fraction of the fixed container--decrease
      // to make the tabs scroll farther per click
      DATA_KEY_DDMENU_MODIFIED: 'scrtabsddmenumodified',
      DATA_KEY_IS_MOUSEDOWN: 'scrtabsismousedown',
      DATA_KEY_BOOTSTRAP_TAB: 'bs.tab',
      CSS_CLASSES: {
        BOOTSTRAP4: 'scrtabs-bootstrap4',
        RTL: 'scrtabs-rtl',
        SCROLL_ARROW_CLICK_TARGET: 'scrtabs-click-target',
        SCROLL_ARROW_DISABLE: 'scrtabs-disable',
        SCROLL_ARROW_WITH_CLICK_TARGET: 'scrtabs-with-click-target'
      },
      SLIDE_DIRECTION: {
        LEFT: 1,
        RIGHT: 2
      },
      EVENTS: {
        CLICK: 'click.scrtabs',
        DROPDOWN_MENU_HIDE: 'hide.bs.dropdown.scrtabs',
        DROPDOWN_MENU_SHOW: 'show.bs.dropdown.scrtabs',
        FORCE_REFRESH: 'forcerefresh.scrtabs',
        MOUSEDOWN: 'mousedown.scrtabs',
        MOUSEUP: 'mouseup.scrtabs',
        TABS_READY: 'ready.scrtabs',
        TOUCH_END: 'touchend.scrtabs',
        TOUCH_MOVE: 'touchmove.scrtabs',
        TOUCH_START: 'touchstart.scrtabs',
        WINDOW_RESIZE: 'resize.scrtabs'
      }
    }; // smartresize from Paul Irish (debounced window resize)

    (function (sr) {
      var debounce = function debounce(func, threshold, execAsap) {
        var timeout;
        return function debounced() {
          var obj = this,
              args = arguments;

          function delayed() {
            if (!execAsap) {
              func.apply(obj, args);
            }

            timeout = null;
          }

          if (timeout) {
            clearTimeout(timeout);
          } else if (execAsap) {
            func.apply(obj, args);
          }

          timeout = setTimeout(delayed, threshold || 100);
        };
      };

      $.fn[sr] = function (fn, customEventName) {
        var eventName = customEventName || CONSTANTS.EVENTS.WINDOW_RESIZE;
        return fn ? this.bind(eventName, debounce(fn)) : this.trigger(sr);
      };
    })('smartresizeScrtabs');
    /* ***********************************************************************************
     * ElementsHandler - Class that each instance of ScrollingTabsControl will instantiate
     * **********************************************************************************/


    function ElementsHandler(scrollingTabsControl) {
      var ehd = this;
      ehd.stc = scrollingTabsControl;
    } // ElementsHandler prototype methods


    (function (p) {
      p.initElements = function (options) {
        var ehd = this;
        ehd.setElementReferences(options);
        ehd.setEventListeners(options);
      };

      p.listenForTouchEvents = function () {
        var ehd = this,
            stc = ehd.stc,
            smv = stc.scrollMovement,
            ev = CONSTANTS.EVENTS;
        var touching = false;
        var touchStartX;
        var startingContainerLeftPos;
        var newLeftPos;
        stc.$movableContainer.on(ev.TOUCH_START, function (e) {
          touching = true;
          startingContainerLeftPos = stc.movableContainerLeftPos;
          touchStartX = e.originalEvent.changedTouches[0].pageX;
        }).on(ev.TOUCH_END, function () {
          touching = false;
        }).on(ev.TOUCH_MOVE, function (e) {
          if (!touching) {
            return;
          }

          var touchPageX = e.originalEvent.changedTouches[0].pageX;
          var diff = touchPageX - touchStartX;

          if (stc.rtl) {
            diff = -diff;
          }

          var minPos;
          newLeftPos = startingContainerLeftPos + diff;

          if (newLeftPos > 0) {
            newLeftPos = 0;
          } else {
            minPos = smv.getMinPos();

            if (newLeftPos < minPos) {
              newLeftPos = minPos;
            }
          }

          stc.movableContainerLeftPos = newLeftPos;
          var leftOrRight = stc.rtl ? 'right' : 'left';
          stc.$movableContainer.css(leftOrRight, smv.getMovableContainerCssLeftVal());
          smv.refreshScrollArrowsDisabledState();
        });
      };

      p.refreshAllElementSizes = function () {
        var ehd = this,
            stc = ehd.stc,
            smv = stc.scrollMovement,
            scrollArrowsWereVisible = stc.scrollArrowsVisible,
            actionsTaken = {
          didScrollToActiveTab: false
        },
            isPerformingSlideAnim = false,
            minPos;
        ehd.setElementWidths();
        ehd.setScrollArrowVisibility(); // this could have been a window resize or the removal of a
        // dynamic tab, so make sure the movable container is positioned
        // correctly because, if it is far to the left and we increased the
        // window width, it's possible that the tabs will be too far left,
        // beyond the min pos.

        if (stc.scrollArrowsVisible) {
          // make sure container not too far left
          minPos = smv.getMinPos();
          isPerformingSlideAnim = smv.scrollToActiveTab({
            isOnWindowResize: true
          });

          if (!isPerformingSlideAnim) {
            smv.refreshScrollArrowsDisabledState();

            if (stc.rtl) {
              if (stc.movableContainerRightPos < minPos) {
                smv.incrementMovableContainerLeft(minPos);
              }
            } else {
              if (stc.movableContainerLeftPos < minPos) {
                smv.incrementMovableContainerRight(minPos);
              }
            }
          }

          actionsTaken.didScrollToActiveTab = true;
        } else if (scrollArrowsWereVisible) {
          // scroll arrows went away after resize, so position movable container at 0
          stc.movableContainerLeftPos = 0;
          smv.slideMovableContainerToLeftPos();
        }

        return actionsTaken;
      };

      p.setElementReferences = function (settings) {
        var ehd = this,
            stc = ehd.stc,
            $tabsContainer = stc.$tabsContainer,
            $leftArrow,
            $rightArrow,
            $leftArrowClickTarget,
            $rightArrowClickTarget;
        stc.isNavPills = false;

        if (stc.rtl) {
          $tabsContainer.addClass(CONSTANTS.CSS_CLASSES.RTL);
        }

        if (stc.usingBootstrap4) {
          $tabsContainer.addClass(CONSTANTS.CSS_CLASSES.BOOTSTRAP4);
        }

        stc.$fixedContainer = $tabsContainer.find('.scrtabs-tabs-fixed-container');
        $leftArrow = stc.$fixedContainer.prev();
        $rightArrow = stc.$fixedContainer.next(); // if we have custom arrow content, we might have a click target defined

        if (settings.leftArrowContent) {
          $leftArrowClickTarget = $leftArrow.find('.' + CONSTANTS.CSS_CLASSES.SCROLL_ARROW_CLICK_TARGET);
        }

        if (settings.rightArrowContent) {
          $rightArrowClickTarget = $rightArrow.find('.' + CONSTANTS.CSS_CLASSES.SCROLL_ARROW_CLICK_TARGET);
        }

        if ($leftArrowClickTarget && $leftArrowClickTarget.length) {
          $leftArrow.addClass(CONSTANTS.CSS_CLASSES.SCROLL_ARROW_WITH_CLICK_TARGET);
        } else {
          $leftArrowClickTarget = $leftArrow;
        }

        if ($rightArrowClickTarget && $rightArrowClickTarget.length) {
          $rightArrow.addClass(CONSTANTS.CSS_CLASSES.SCROLL_ARROW_WITH_CLICK_TARGET);
        } else {
          $rightArrowClickTarget = $rightArrow;
        }

        stc.$movableContainer = $tabsContainer.find('.scrtabs-tabs-movable-container');
        stc.$tabsUl = $tabsContainer.find('.nav-tabs'); // check for pills

        if (!stc.$tabsUl.length) {
          stc.$tabsUl = $tabsContainer.find('.nav-pills');

          if (stc.$tabsUl.length) {
            stc.isNavPills = true;
          }
        }

        stc.$tabsLiCollection = stc.$tabsUl.find('> li');
        stc.$slideLeftArrow = stc.reverseScroll ? $leftArrow : $rightArrow;
        stc.$slideLeftArrowClickTarget = stc.reverseScroll ? $leftArrowClickTarget : $rightArrowClickTarget;
        stc.$slideRightArrow = stc.reverseScroll ? $rightArrow : $leftArrow;
        stc.$slideRightArrowClickTarget = stc.reverseScroll ? $rightArrowClickTarget : $leftArrowClickTarget;
        stc.$scrollArrows = stc.$slideLeftArrow.add(stc.$slideRightArrow);
        stc.$win = $(window);
      };

      p.setElementWidths = function () {
        var ehd = this,
            stc = ehd.stc;
        stc.winWidth = stc.$win.width();
        stc.scrollArrowsCombinedWidth = stc.$slideLeftArrow.outerWidth() + stc.$slideRightArrow.outerWidth();
        ehd.setFixedContainerWidth();
        ehd.setMovableContainerWidth();
      };

      p.setEventListeners = function (settings) {
        var ehd = this,
            stc = ehd.stc,
            evh = stc.eventHandlers,
            ev = CONSTANTS.EVENTS,
            resizeEventName = ev.WINDOW_RESIZE + stc.instanceId;

        if (settings.enableSwiping) {
          ehd.listenForTouchEvents();
        }

        stc.$slideLeftArrowClickTarget.off('.scrtabs').on(ev.MOUSEDOWN, function (e) {
          evh.handleMousedownOnSlideMovContainerLeftArrow.call(evh, e);
        }).on(ev.MOUSEUP, function (e) {
          evh.handleMouseupOnSlideMovContainerLeftArrow.call(evh, e);
        }).on(ev.CLICK, function (e) {
          evh.handleClickOnSlideMovContainerLeftArrow.call(evh, e);
        });
        stc.$slideRightArrowClickTarget.off('.scrtabs').on(ev.MOUSEDOWN, function (e) {
          evh.handleMousedownOnSlideMovContainerRightArrow.call(evh, e);
        }).on(ev.MOUSEUP, function (e) {
          evh.handleMouseupOnSlideMovContainerRightArrow.call(evh, e);
        }).on(ev.CLICK, function (e) {
          evh.handleClickOnSlideMovContainerRightArrow.call(evh, e);
        });

        if (stc.tabClickHandler) {
          stc.$tabsLiCollection.find('a[data-toggle="tab"]').off(ev.CLICK).on(ev.CLICK, stc.tabClickHandler);
        }

        if (settings.handleDelayedScrollbar) {
          ehd.listenForDelayedScrollbar();
        }

        stc.$win.off(resizeEventName).smartresizeScrtabs(function (e) {
          evh.handleWindowResize.call(evh, e);
        }, resizeEventName);
        $('body').on(CONSTANTS.EVENTS.FORCE_REFRESH, stc.elementsHandler.refreshAllElementSizes.bind(stc.elementsHandler));
      };

      p.listenForDelayedScrollbar = function () {
        var iframe = document.createElement('iframe');
        iframe.id = "scrtabs-scrollbar-resize-listener";
        iframe.style.cssText = 'height: 0; background-color: transparent; margin: 0; padding: 0; overflow: hidden; border-width: 0; position: absolute; width: 100%;';

        iframe.onload = function () {
          var timeout;

          function handleResize() {
            try {
              $(window).trigger('resize');
              timeout = null;
            } catch (e) {}
          }

          iframe.contentWindow.addEventListener('resize', function () {
            if (timeout) {
              clearTimeout(timeout);
            }

            timeout = setTimeout(handleResize, 100);
          });
        };

        document.body.appendChild(iframe);
      };

      p.setFixedContainerWidth = function () {
        var ehd = this,
            stc = ehd.stc,
            tabsContainerRect = stc.$tabsContainer.get(0).getBoundingClientRect();
        /**
         * @author poletaew
         * It solves problem with rounding by jQuery.outerWidth
         * If we have real width 100.5 px, jQuery.outerWidth returns us 101 px and we get layout's fail
         */

        stc.fixedContainerWidth = tabsContainerRect.width || tabsContainerRect.right - tabsContainerRect.left;
        stc.fixedContainerWidth = stc.fixedContainerWidth * stc.widthMultiplier;
        stc.$fixedContainer.width(stc.fixedContainerWidth);
      };

      p.setFixedContainerWidthForHiddenScrollArrows = function () {
        var ehd = this,
            stc = ehd.stc;
        stc.$fixedContainer.width(stc.fixedContainerWidth);
      };

      p.setFixedContainerWidthForVisibleScrollArrows = function () {
        var ehd = this,
            stc = ehd.stc;
        stc.$fixedContainer.width(stc.fixedContainerWidth - stc.scrollArrowsCombinedWidth);
      };

      p.setMovableContainerWidth = function () {
        var ehd = this,
            stc = ehd.stc,
            $tabLi = stc.$tabsUl.find('> li');
        stc.movableContainerWidth = 0;

        if ($tabLi.length) {
          $tabLi.each(function () {
            var $li = $(this),
                totalMargin = 0;

            if (stc.isNavPills) {
              // pills have a margin-left, tabs have no margin
              totalMargin = parseInt($li.css('margin-left'), 10) + parseInt($li.css('margin-right'), 10);
            }

            stc.movableContainerWidth += $li.outerWidth() + totalMargin;
          });
          stc.movableContainerWidth += 1; // if the tabs don't span the width of the page, force the
          // movable container width to full page width so the bottom
          // border spans the page width instead of just spanning the
          // width of the tabs

          if (stc.movableContainerWidth < stc.fixedContainerWidth) {
            stc.movableContainerWidth = stc.fixedContainerWidth;
          }
        }

        stc.$movableContainer.width(stc.movableContainerWidth);
      };

      p.setScrollArrowVisibility = function () {
        var ehd = this,
            stc = ehd.stc,
            shouldBeVisible = stc.movableContainerWidth > stc.fixedContainerWidth;

        if (shouldBeVisible && !stc.scrollArrowsVisible) {
          stc.$scrollArrows.show();
          stc.scrollArrowsVisible = true;
        } else if (!shouldBeVisible && stc.scrollArrowsVisible) {
          stc.$scrollArrows.hide();
          stc.scrollArrowsVisible = false;
        }

        if (stc.scrollArrowsVisible) {
          ehd.setFixedContainerWidthForVisibleScrollArrows();
        } else {
          ehd.setFixedContainerWidthForHiddenScrollArrows();
        }
      };
    })(ElementsHandler.prototype);
    /* ***********************************************************************************
     * EventHandlers - Class that each instance of ScrollingTabsControl will instantiate
     * **********************************************************************************/


    function EventHandlers(scrollingTabsControl) {
      var evh = this;
      evh.stc = scrollingTabsControl;
    } // prototype methods


    (function (p) {
      p.handleClickOnSlideMovContainerLeftArrow = function () {
        var evh = this,
            stc = evh.stc;
        stc.scrollMovement.incrementMovableContainerLeft();
      };

      p.handleClickOnSlideMovContainerRightArrow = function () {
        var evh = this,
            stc = evh.stc;
        stc.scrollMovement.incrementMovableContainerRight();
      };

      p.handleMousedownOnSlideMovContainerLeftArrow = function () {
        var evh = this,
            stc = evh.stc;
        stc.$slideLeftArrowClickTarget.data(CONSTANTS.DATA_KEY_IS_MOUSEDOWN, true);
        stc.scrollMovement.continueSlideMovableContainerLeft();
      };

      p.handleMousedownOnSlideMovContainerRightArrow = function () {
        var evh = this,
            stc = evh.stc;
        stc.$slideRightArrowClickTarget.data(CONSTANTS.DATA_KEY_IS_MOUSEDOWN, true);
        stc.scrollMovement.continueSlideMovableContainerRight();
      };

      p.handleMouseupOnSlideMovContainerLeftArrow = function () {
        var evh = this,
            stc = evh.stc;
        stc.$slideLeftArrowClickTarget.data(CONSTANTS.DATA_KEY_IS_MOUSEDOWN, false);
      };

      p.handleMouseupOnSlideMovContainerRightArrow = function () {
        var evh = this,
            stc = evh.stc;
        stc.$slideRightArrowClickTarget.data(CONSTANTS.DATA_KEY_IS_MOUSEDOWN, false);
      };

      p.handleWindowResize = function () {
        var evh = this,
            stc = evh.stc,
            newWinWidth = stc.$win.width();

        if (newWinWidth === stc.winWidth) {
          return false;
        }

        stc.winWidth = newWinWidth;
        stc.elementsHandler.refreshAllElementSizes();
      };
    })(EventHandlers.prototype);
    /* ***********************************************************************************
     * ScrollMovement - Class that each instance of ScrollingTabsControl will instantiate
     * **********************************************************************************/


    function ScrollMovement(scrollingTabsControl) {
      var smv = this;
      smv.stc = scrollingTabsControl;
    } // prototype methods


    (function (p) {
      p.continueSlideMovableContainerLeft = function () {
        var smv = this,
            stc = smv.stc;
        setTimeout(function () {
          if (stc.movableContainerLeftPos <= smv.getMinPos() || !stc.$slideLeftArrowClickTarget.data(CONSTANTS.DATA_KEY_IS_MOUSEDOWN)) {
            return;
          }

          if (!smv.incrementMovableContainerLeft()) {
            // haven't reached max left
            smv.continueSlideMovableContainerLeft();
          }
        }, CONSTANTS.CONTINUOUS_SCROLLING_TIMEOUT_INTERVAL);
      };

      p.continueSlideMovableContainerRight = function () {
        var smv = this,
            stc = smv.stc;
        setTimeout(function () {
          if (stc.movableContainerLeftPos >= 0 || !stc.$slideRightArrowClickTarget.data(CONSTANTS.DATA_KEY_IS_MOUSEDOWN)) {
            return;
          }

          if (!smv.incrementMovableContainerRight()) {
            // haven't reached max right
            smv.continueSlideMovableContainerRight();
          }
        }, CONSTANTS.CONTINUOUS_SCROLLING_TIMEOUT_INTERVAL);
      };

      p.decrementMovableContainerLeftPos = function (minPos) {
        var smv = this,
            stc = smv.stc;
        stc.movableContainerLeftPos -= stc.fixedContainerWidth / CONSTANTS.SCROLL_OFFSET_FRACTION;

        if (stc.movableContainerLeftPos < minPos) {
          stc.movableContainerLeftPos = minPos;
        } else if (stc.scrollToTabEdge) {
          smv.setMovableContainerLeftPosToTabEdge(CONSTANTS.SLIDE_DIRECTION.LEFT);

          if (stc.movableContainerLeftPos < minPos) {
            stc.movableContainerLeftPos = minPos;
          }
        }
      };

      p.disableSlideLeftArrow = function () {
        var smv = this,
            stc = smv.stc;

        if (!stc.disableScrollArrowsOnFullyScrolled || !stc.scrollArrowsVisible) {
          return;
        }

        stc.$slideLeftArrow.addClass(CONSTANTS.CSS_CLASSES.SCROLL_ARROW_DISABLE);
      };

      p.disableSlideRightArrow = function () {
        var smv = this,
            stc = smv.stc;

        if (!stc.disableScrollArrowsOnFullyScrolled || !stc.scrollArrowsVisible) {
          return;
        }

        stc.$slideRightArrow.addClass(CONSTANTS.CSS_CLASSES.SCROLL_ARROW_DISABLE);
      };

      p.enableSlideLeftArrow = function () {
        var smv = this,
            stc = smv.stc;

        if (!stc.disableScrollArrowsOnFullyScrolled || !stc.scrollArrowsVisible) {
          return;
        }

        stc.$slideLeftArrow.removeClass(CONSTANTS.CSS_CLASSES.SCROLL_ARROW_DISABLE);
      };

      p.enableSlideRightArrow = function () {
        var smv = this,
            stc = smv.stc;

        if (!stc.disableScrollArrowsOnFullyScrolled || !stc.scrollArrowsVisible) {
          return;
        }

        stc.$slideRightArrow.removeClass(CONSTANTS.CSS_CLASSES.SCROLL_ARROW_DISABLE);
      };

      p.getMinPos = function () {
        var smv = this,
            stc = smv.stc;
        return stc.scrollArrowsVisible ? stc.fixedContainerWidth - stc.movableContainerWidth - stc.scrollArrowsCombinedWidth : 0;
      };

      p.getMovableContainerCssLeftVal = function () {
        var smv = this,
            stc = smv.stc;
        return stc.movableContainerLeftPos === 0 ? '0' : stc.movableContainerLeftPos + 'px';
      };

      p.incrementMovableContainerLeft = function () {
        var smv = this,
            stc = smv.stc,
            minPos = smv.getMinPos();
        smv.decrementMovableContainerLeftPos(minPos);
        smv.slideMovableContainerToLeftPos();
        smv.enableSlideRightArrow(); // return true if we're fully left, false otherwise

        return stc.movableContainerLeftPos === minPos;
      };

      p.incrementMovableContainerRight = function (minPos) {
        var smv = this,
            stc = smv.stc; // if minPos passed in, the movable container was beyond the minPos

        if (minPos) {
          stc.movableContainerLeftPos = minPos;
        } else {
          stc.movableContainerLeftPos += stc.fixedContainerWidth / CONSTANTS.SCROLL_OFFSET_FRACTION;

          if (stc.movableContainerLeftPos > 0) {
            stc.movableContainerLeftPos = 0;
          } else if (stc.scrollToTabEdge) {
            smv.setMovableContainerLeftPosToTabEdge(CONSTANTS.SLIDE_DIRECTION.RIGHT);
          }
        }

        smv.slideMovableContainerToLeftPos();
        smv.enableSlideLeftArrow(); // return true if we're fully right, false otherwise
        // left pos of 0 is the movable container's max position (farthest right)

        return stc.movableContainerLeftPos === 0;
      };

      p.refreshScrollArrowsDisabledState = function () {
        var smv = this,
            stc = smv.stc;

        if (!stc.disableScrollArrowsOnFullyScrolled || !stc.scrollArrowsVisible) {
          return;
        }

        if (stc.movableContainerLeftPos >= 0) {
          // movable container fully right
          smv.disableSlideRightArrow();
          smv.enableSlideLeftArrow();
          return;
        }

        if (stc.movableContainerLeftPos <= smv.getMinPos()) {
          // fully left
          smv.disableSlideLeftArrow();
          smv.enableSlideRightArrow();
          return;
        }

        smv.enableSlideLeftArrow();
        smv.enableSlideRightArrow();
      };

      p.scrollToActiveTab = function () {
        var smv = this,
            stc = smv.stc,
            $activeTab,
            $activeTabAnchor,
            activeTabLeftPos,
            activeTabRightPos,
            rightArrowLeftPos,
            activeTabWidth,
            leftPosOffset,
            offsetToMiddle,
            leftScrollArrowWidth,
            rightScrollArrowWidth;

        if (!stc.scrollArrowsVisible) {
          return;
        }

        if (stc.usingBootstrap4) {
          $activeTabAnchor = stc.$tabsUl.find('li > .nav-link.active');

          if ($activeTabAnchor.length) {
            $activeTab = $activeTabAnchor.parent();
          }
        } else {
          $activeTab = stc.$tabsUl.find('li.active');
        }

        if (!$activeTab || !$activeTab.length) {
          return;
        }

        rightScrollArrowWidth = stc.$slideRightArrow.outerWidth();
        activeTabWidth = $activeTab.outerWidth();
        /**
         * @author poletaew
         * We need relative offset (depends on $fixedContainer), don't absolute
         */

        activeTabLeftPos = $activeTab.offset().left - stc.$fixedContainer.offset().left;
        activeTabRightPos = activeTabLeftPos + activeTabWidth;
        rightArrowLeftPos = stc.fixedContainerWidth - rightScrollArrowWidth;

        if (stc.rtl) {
          leftScrollArrowWidth = stc.$slideLeftArrow.outerWidth();

          if (activeTabLeftPos < 0) {
            // active tab off left side
            stc.movableContainerLeftPos += activeTabLeftPos;
            smv.slideMovableContainerToLeftPos();
            return true;
          } else {
            // active tab off right side
            if (activeTabRightPos > rightArrowLeftPos) {
              stc.movableContainerLeftPos += activeTabRightPos - rightArrowLeftPos + 2 * rightScrollArrowWidth;
              smv.slideMovableContainerToLeftPos();
              return true;
            }
          }
        } else {
          if (activeTabRightPos > rightArrowLeftPos) {
            // active tab off right side
            leftPosOffset = activeTabRightPos - rightArrowLeftPos + rightScrollArrowWidth;
            offsetToMiddle = stc.fixedContainerWidth / 2;
            leftPosOffset += offsetToMiddle - activeTabWidth / 2;
            stc.movableContainerLeftPos -= leftPosOffset;
            smv.slideMovableContainerToLeftPos();
            return true;
          } else {
            leftScrollArrowWidth = stc.$slideLeftArrow.outerWidth();

            if (activeTabLeftPos < 0) {
              // active tab off left side
              offsetToMiddle = stc.fixedContainerWidth / 2;
              stc.movableContainerLeftPos += -activeTabLeftPos + offsetToMiddle - activeTabWidth / 2;
              smv.slideMovableContainerToLeftPos();
              return true;
            }
          }
        }

        return false;
      };

      p.setMovableContainerLeftPosToTabEdge = function (slideDirection) {
        var smv = this,
            stc = smv.stc,
            offscreenWidth = -stc.movableContainerLeftPos,
            totalTabWidth = 0; // make sure LeftPos is set so that a tab edge will be against the
        // left scroll arrow so we won't have a partial, cut-off tab

        stc.$tabsLiCollection.each(function () {
          var tabWidth = $(this).width();
          totalTabWidth += tabWidth;

          if (totalTabWidth > offscreenWidth) {
            stc.movableContainerLeftPos = slideDirection === CONSTANTS.SLIDE_DIRECTION.RIGHT ? -(totalTabWidth - tabWidth) : -totalTabWidth;
            return false; // exit .each() loop
          }
        });
      };

      p.slideMovableContainerToLeftPos = function () {
        var smv = this,
            stc = smv.stc,
            minPos = smv.getMinPos(),
            leftOrRightVal;

        if (stc.movableContainerLeftPos > 0) {
          stc.movableContainerLeftPos = 0;
        } else if (stc.movableContainerLeftPos < minPos) {
          stc.movableContainerLeftPos = minPos;
        }

        stc.movableContainerLeftPos = stc.movableContainerLeftPos / 1;
        leftOrRightVal = smv.getMovableContainerCssLeftVal();
        smv.performingSlideAnim = true;
        var targetPos = stc.rtl ? {
          right: leftOrRightVal
        } : {
          left: leftOrRightVal
        };
        stc.$movableContainer.stop().animate(targetPos, 'slow', function __slideAnimComplete() {
          var newMinPos = smv.getMinPos();
          smv.performingSlideAnim = false; // if we slid past the min pos--which can happen if you resize the window
          // quickly--move back into position

          if (stc.movableContainerLeftPos < newMinPos) {
            smv.decrementMovableContainerLeftPos(newMinPos);
            targetPos = stc.rtl ? {
              right: smv.getMovableContainerCssLeftVal()
            } : {
              left: smv.getMovableContainerCssLeftVal()
            };
            stc.$movableContainer.stop().animate(targetPos, 'fast', function () {
              smv.refreshScrollArrowsDisabledState();
            });
          } else {
            smv.refreshScrollArrowsDisabledState();
          }
        });
      };
    })(ScrollMovement.prototype);
    /* **********************************************************************
     * ScrollingTabsControl - Class that each directive will instantiate
     * **********************************************************************/


    function ScrollingTabsControl($tabsContainer) {
      var stc = this;
      stc.$tabsContainer = $tabsContainer;
      stc.instanceId = $.fn.scrollingTabs.nextInstanceId++;
      stc.movableContainerLeftPos = 0;
      stc.scrollArrowsVisible = false;
      stc.scrollToTabEdge = false;
      stc.disableScrollArrowsOnFullyScrolled = false;
      stc.reverseScroll = false;
      stc.widthMultiplier = 1;
      stc.scrollMovement = new ScrollMovement(stc);
      stc.eventHandlers = new EventHandlers(stc);
      stc.elementsHandler = new ElementsHandler(stc);
    } // prototype methods


    (function (p) {
      p.initTabs = function (options, $scroller, readyCallback, attachTabContentToDomCallback) {
        var stc = this,
            elementsHandler = stc.elementsHandler,
            num;

        if (options.enableRtlSupport && $('html').attr('dir') === 'rtl') {
          stc.rtl = true;
        }

        if (options.scrollToTabEdge) {
          stc.scrollToTabEdge = true;
        }

        if (options.disableScrollArrowsOnFullyScrolled) {
          stc.disableScrollArrowsOnFullyScrolled = true;
        }

        if (options.reverseScroll) {
          stc.reverseScroll = true;
        }

        if (options.widthMultiplier !== 1) {
          num = Number(options.widthMultiplier); // handle string value

          if (!isNaN(num)) {
            stc.widthMultiplier = num;
          }
        }

        if (options.bootstrapVersion.toString().charAt(0) === '4') {
          stc.usingBootstrap4 = true;
        }

        setTimeout(initTabsAfterTimeout, 100);

        function initTabsAfterTimeout() {
          var actionsTaken; // if we're just wrapping non-data-driven tabs, the user might
          // have the .nav-tabs hidden to prevent the clunky flash of
          // multi-line tabs on page refresh, so we need to make sure
          // they're visible before trying to wrap them

          $scroller.find('.nav-tabs').show();
          elementsHandler.initElements(options);
          actionsTaken = elementsHandler.refreshAllElementSizes();
          $scroller.css('visibility', 'visible');

          if (attachTabContentToDomCallback) {
            attachTabContentToDomCallback();
          }

          if (readyCallback) {
            readyCallback();
          }
        }
      };

      p.scrollToActiveTab = function (options) {
        var stc = this,
            smv = stc.scrollMovement;
        smv.scrollToActiveTab(options);
      };
    })(ScrollingTabsControl.prototype);
    /* exported buildNavTabsAndTabContentForTargetElementInstance */


    var tabElements = function () {
      return {
        getElTabPaneForLi: getElTabPaneForLi,
        getNewElNavTabs: getNewElNavTabs,
        getNewElScrollerElementWrappingNavTabsInstance: getNewElScrollerElementWrappingNavTabsInstance,
        getNewElTabAnchor: getNewElTabAnchor,
        getNewElTabContent: getNewElTabContent,
        getNewElTabLi: getNewElTabLi,
        getNewElTabPane: getNewElTabPane
      }; ///////////////////
      // ---- retrieve existing elements from the DOM ----------

      function getElTabPaneForLi($li) {
        return $($li.find('a').attr('href'));
      } // ---- create new elements ----------


      function getNewElNavTabs() {
        return $('<ul class="nav nav-tabs" role="tablist"></ul>');
      }

      function getNewElScrollerElementWrappingNavTabsInstance($navTabsInstance, settings) {
        var $tabsContainer = $('<div class="scrtabs-tab-container"></div>'),
            leftArrowContent = settings.leftArrowContent || '<div class="scrtabs-tab-scroll-arrow scrtabs-tab-scroll-arrow-left"><span class="' + settings.cssClassLeftArrow + '"></span></div>',
            $leftArrow = $(leftArrowContent),
            rightArrowContent = settings.rightArrowContent || '<div class="scrtabs-tab-scroll-arrow scrtabs-tab-scroll-arrow-right"><span class="' + settings.cssClassRightArrow + '"></span></div>',
            $rightArrow = $(rightArrowContent),
            $fixedContainer = $('<div class="scrtabs-tabs-fixed-container"></div>'),
            $movableContainer = $('<div class="scrtabs-tabs-movable-container"></div>');

        if (settings.disableScrollArrowsOnFullyScrolled) {
          $leftArrow.add($rightArrow).addClass(CONSTANTS.CSS_CLASSES.SCROLL_ARROW_DISABLE);
        }

        return $tabsContainer.append($leftArrow, $fixedContainer.append($movableContainer.append($navTabsInstance)), $rightArrow);
      }

      function getNewElTabAnchor(tab, propNames) {
        return $('<a role="tab" data-toggle="tab"></a>').attr('href', '#' + tab[propNames.paneId]).html(tab[propNames.title]);
      }

      function getNewElTabContent() {
        return $('<div class="tab-content"></div>');
      }

      function getNewElTabLi(tab, propNames, options) {
        var liContent = options.tabLiContent || '<li role="presentation" class=""></li>',
            $li = $(liContent),
            $a = getNewElTabAnchor(tab, propNames).appendTo($li);

        if (tab[propNames.disabled]) {
          $li.addClass('disabled');
          $a.attr('data-toggle', '');
        } else if (options.forceActiveTab && tab[propNames.active]) {
          $li.addClass('active');
        }

        if (options.tabPostProcessor) {
          options.tabPostProcessor($li, $a);
        }

        return $li;
      }

      function getNewElTabPane(tab, propNames, options) {
        var $pane = $('<div role="tabpanel" class="tab-pane"></div>').attr('id', tab[propNames.paneId]).html(tab[propNames.content]);

        if (options.forceActiveTab && tab[propNames.active]) {
          $pane.addClass('active');
        }

        return $pane;
      }
    }(); // tabElements


    var tabUtils = function () {
      return {
        didTabOrderChange: didTabOrderChange,
        getIndexOfClosestEnabledTab: getIndexOfClosestEnabledTab,
        getTabIndexByPaneId: getTabIndexByPaneId,
        storeDataOnLiEl: storeDataOnLiEl
      }; ///////////////////

      function didTabOrderChange($currTabLis, updatedTabs, propNames) {
        var isTabOrderChanged = false;
        $currTabLis.each(function (currDomIdx) {
          var newIdx = getTabIndexByPaneId(updatedTabs, propNames.paneId, $(this).data('tab')[propNames.paneId]);

          if (newIdx > -1 && newIdx !== currDomIdx) {
            // tab moved
            isTabOrderChanged = true;
            return false; // exit .each() loop
          }
        });
        return isTabOrderChanged;
      }

      function getIndexOfClosestEnabledTab($currTabLis, startIndex) {
        var lastIndex = $currTabLis.length - 1,
            closestIdx = -1,
            incrementFromStartIndex = 0,
            testIdx = 0; // expand out from the current tab looking for an enabled tab;
        // we prefer the tab after us over the tab before

        while (closestIdx === -1 && testIdx >= 0) {
          if ((testIdx = startIndex + ++incrementFromStartIndex) <= lastIndex && !$currTabLis.eq(testIdx).hasClass('disabled') || (testIdx = startIndex - incrementFromStartIndex) >= 0 && !$currTabLis.eq(testIdx).hasClass('disabled')) {
            closestIdx = testIdx;
          }
        }

        return closestIdx;
      }

      function getTabIndexByPaneId(tabs, paneIdPropName, paneId) {
        var idx = -1;
        tabs.some(function (tab, i) {
          if (tab[paneIdPropName] === paneId) {
            idx = i;
            return true; // exit loop
          }
        });
        return idx;
      }

      function storeDataOnLiEl($li, tabs, index) {
        $li.data({
          tab: $.extend({}, tabs[index]),
          // store a clone so we can check for changes
          index: index
        });
      }
    }(); // tabUtils


    function buildNavTabsAndTabContentForTargetElementInstance($targetElInstance, settings, readyCallback) {
      var tabs = settings.tabs,
          propNames = {
        paneId: settings.propPaneId,
        title: settings.propTitle,
        active: settings.propActive,
        disabled: settings.propDisabled,
        content: settings.propContent
      },
          ignoreTabPanes = settings.ignoreTabPanes,
          hasTabContent = tabs.length && tabs[0][propNames.content] !== undefined,
          $navTabs = tabElements.getNewElNavTabs(),
          $tabContent = tabElements.getNewElTabContent(),
          $scroller,
          attachTabContentToDomCallback = ignoreTabPanes ? null : function () {
        $scroller.after($tabContent);
      };

      if (!tabs.length) {
        return;
      }

      tabs.forEach(function (tab, index) {
        var options = {
          forceActiveTab: true,
          tabLiContent: settings.tabsLiContent && settings.tabsLiContent[index],
          tabPostProcessor: settings.tabsPostProcessors && settings.tabsPostProcessors[index]
        };
        tabElements.getNewElTabLi(tab, propNames, options).appendTo($navTabs); // build the tab panes if we weren't told to ignore them and there's
        // tab content data available

        if (!ignoreTabPanes && hasTabContent) {
          tabElements.getNewElTabPane(tab, propNames, options).appendTo($tabContent);
        }
      });
      $scroller = wrapNavTabsInstanceInScroller($navTabs, settings, readyCallback, attachTabContentToDomCallback);
      $scroller.appendTo($targetElInstance);
      $targetElInstance.data({
        scrtabs: {
          tabs: tabs,
          propNames: propNames,
          ignoreTabPanes: ignoreTabPanes,
          hasTabContent: hasTabContent,
          tabsLiContent: settings.tabsLiContent,
          tabsPostProcessors: settings.tabsPostProcessors,
          scroller: $scroller
        }
      }); // once the nav-tabs are wrapped in the scroller, attach each tab's
      // data to it for reference later; we need to wait till they're
      // wrapped in the scroller because we wrap a *clone* of the nav-tabs
      // we built above, not the original nav-tabs

      $scroller.find('.nav-tabs > li').each(function (index) {
        tabUtils.storeDataOnLiEl($(this), tabs, index);
      });
      return $targetElInstance;
    }

    function wrapNavTabsInstanceInScroller($navTabsInstance, settings, readyCallback, attachTabContentToDomCallback) {
      // Remove tab data stored by Bootstrap in order to fix tabs that were already visited
      $navTabsInstance.find('a[data-toggle="tab"]').removeData(CONSTANTS.DATA_KEY_BOOTSTRAP_TAB);
      var $scroller = tabElements.getNewElScrollerElementWrappingNavTabsInstance($navTabsInstance.clone(true), settings),
          // use clone because we replaceWith later
      scrollingTabsControl = new ScrollingTabsControl($scroller),
          navTabsInstanceData = $navTabsInstance.data('scrtabs');

      if (!navTabsInstanceData) {
        $navTabsInstance.data('scrtabs', {
          scroller: $scroller
        });
      } else {
        navTabsInstanceData.scroller = $scroller;
      }

      $navTabsInstance.replaceWith($scroller.css('visibility', 'hidden'));

      if (settings.tabClickHandler && typeof settings.tabClickHandler === 'function') {
        $scroller.hasTabClickHandler = true;
        scrollingTabsControl.tabClickHandler = settings.tabClickHandler;
      }

      $scroller.initTabs = function () {
        scrollingTabsControl.initTabs(settings, $scroller, readyCallback, attachTabContentToDomCallback);
      };

      $scroller.scrollToActiveTab = function () {
        scrollingTabsControl.scrollToActiveTab(settings);
      };

      $scroller.initTabs();
      listenForDropdownMenuTabs($scroller, scrollingTabsControl);
      return $scroller;
    }
    /* exported listenForDropdownMenuTabs,
                refreshTargetElementInstance,
                scrollToActiveTab */


    function checkForTabAdded(refreshData) {
      var updatedTabsArray = refreshData.updatedTabsArray,
          updatedTabsLiContent = refreshData.updatedTabsLiContent || [],
          updatedTabsPostProcessors = refreshData.updatedTabsPostProcessors || [],
          propNames = refreshData.propNames,
          ignoreTabPanes = refreshData.ignoreTabPanes,
          options = refreshData.options,
          $currTabLis = refreshData.$currTabLis,
          $navTabs = refreshData.$navTabs,
          $currTabContentPanesContainer = ignoreTabPanes ? null : refreshData.$currTabContentPanesContainer,
          $currTabContentPanes = ignoreTabPanes ? null : refreshData.$currTabContentPanes,
          isInitTabsRequired = false; // make sure each tab in the updated tabs array has a corresponding DOM element

      updatedTabsArray.forEach(function (tab, idx) {
        var $li = $currTabLis.find('a[href="#' + tab[propNames.paneId] + '"]'),
            isTabIdxPastCurrTabs = idx >= $currTabLis.length,
            $pane;

        if (!$li.length) {
          // new tab
          isInitTabsRequired = true; // add the tab, add its pane (if necessary), and refresh the scroller

          options.tabLiContent = updatedTabsLiContent[idx];
          options.tabPostProcessor = updatedTabsPostProcessors[idx];
          $li = tabElements.getNewElTabLi(tab, propNames, options);
          tabUtils.storeDataOnLiEl($li, updatedTabsArray, idx);

          if (isTabIdxPastCurrTabs) {
            // append to end of current tabs
            $li.appendTo($navTabs);
          } else {
            // insert in middle of current tabs
            $li.insertBefore($currTabLis.eq(idx));
          }

          if (!ignoreTabPanes && tab[propNames.content] !== undefined) {
            $pane = tabElements.getNewElTabPane(tab, propNames, options);

            if (isTabIdxPastCurrTabs) {
              // append to end of current tabs
              $pane.appendTo($currTabContentPanesContainer);
            } else {
              // insert in middle of current tabs
              $pane.insertBefore($currTabContentPanes.eq(idx));
            }
          }
        }
      });
      return isInitTabsRequired;
    }

    function checkForTabPropertiesUpdated(refreshData) {
      var tabLiData = refreshData.tabLi,
          ignoreTabPanes = refreshData.ignoreTabPanes,
          $li = tabLiData.$li,
          $contentPane = tabLiData.$contentPane,
          origTabData = tabLiData.origTabData,
          newTabData = tabLiData.newTabData,
          propNames = refreshData.propNames,
          isInitTabsRequired = false; // update tab title if necessary

      if (origTabData[propNames.title] !== newTabData[propNames.title]) {
        $li.find('a[role="tab"]').html(origTabData[propNames.title] = newTabData[propNames.title]);
        isInitTabsRequired = true;
      } // update tab disabled state if necessary


      if (origTabData[propNames.disabled] !== newTabData[propNames.disabled]) {
        if (newTabData[propNames.disabled]) {
          // enabled -> disabled
          $li.addClass('disabled');
          $li.find('a[role="tab"]').attr('data-toggle', '');
        } else {
          // disabled -> enabled
          $li.removeClass('disabled');
          $li.find('a[role="tab"]').attr('data-toggle', 'tab');
        }

        origTabData[propNames.disabled] = newTabData[propNames.disabled];
        isInitTabsRequired = true;
      } // update tab active state if necessary


      if (refreshData.options.forceActiveTab) {
        // set the active tab based on the tabs array regardless of the current
        // DOM state, which could have been changed by the user clicking a tab
        // without those changes being reflected back to the tab data
        $li[newTabData[propNames.active] ? 'addClass' : 'removeClass']('active');
        $contentPane[newTabData[propNames.active] ? 'addClass' : 'removeClass']('active');
        origTabData[propNames.active] = newTabData[propNames.active];
        isInitTabsRequired = true;
      } // update tab content pane if necessary


      if (!ignoreTabPanes && origTabData[propNames.content] !== newTabData[propNames.content]) {
        $contentPane.html(origTabData[propNames.content] = newTabData[propNames.content]);
        isInitTabsRequired = true;
      }

      return isInitTabsRequired;
    }

    function checkForTabRemoved(refreshData) {
      var tabLiData = refreshData.tabLi,
          ignoreTabPanes = refreshData.ignoreTabPanes,
          $li = tabLiData.$li,
          idxToMakeActive;

      if (tabLiData.newIdx !== -1) {
        // tab was not removed--it has a valid index
        return false;
      } // if this was the active tab, make the closest enabled tab active


      if ($li.hasClass('active')) {
        idxToMakeActive = tabUtils.getIndexOfClosestEnabledTab(refreshData.$currTabLis, tabLiData.currDomIdx);

        if (idxToMakeActive > -1) {
          refreshData.$currTabLis.eq(idxToMakeActive).addClass('active');

          if (!ignoreTabPanes) {
            refreshData.$currTabContentPanes.eq(idxToMakeActive).addClass('active');
          }
        }
      }

      $li.remove();

      if (!ignoreTabPanes) {
        tabLiData.$contentPane.remove();
      }

      return true;
    }

    function checkForTabsOrderChanged(refreshData) {
      var $currTabLis = refreshData.$currTabLis,
          updatedTabsArray = refreshData.updatedTabsArray,
          propNames = refreshData.propNames,
          ignoreTabPanes = refreshData.ignoreTabPanes,
          newTabsCollection = [],
          newTabPanesCollection = ignoreTabPanes ? null : [];

      if (!tabUtils.didTabOrderChange($currTabLis, updatedTabsArray, propNames)) {
        return false;
      } // the tab order changed...


      updatedTabsArray.forEach(function (t) {
        var paneId = t[propNames.paneId];
        newTabsCollection.push($currTabLis.find('a[role="tab"][href="#' + paneId + '"]').parent('li'));

        if (!ignoreTabPanes) {
          newTabPanesCollection.push($('#' + paneId));
        }
      });
      refreshData.$navTabs.append(newTabsCollection);

      if (!ignoreTabPanes) {
        refreshData.$currTabContentPanesContainer.append(newTabPanesCollection);
      }

      return true;
    }

    function checkForTabsRemovedOrUpdated(refreshData) {
      var $currTabLis = refreshData.$currTabLis,
          updatedTabsArray = refreshData.updatedTabsArray,
          propNames = refreshData.propNames,
          isInitTabsRequired = false;
      $currTabLis.each(function (currDomIdx) {
        var $li = $(this),
            origTabData = $li.data('tab'),
            newIdx = tabUtils.getTabIndexByPaneId(updatedTabsArray, propNames.paneId, origTabData[propNames.paneId]),
            newTabData = newIdx > -1 ? updatedTabsArray[newIdx] : null;
        refreshData.tabLi = {
          $li: $li,
          currDomIdx: currDomIdx,
          newIdx: newIdx,
          $contentPane: tabElements.getElTabPaneForLi($li),
          origTabData: origTabData,
          newTabData: newTabData
        };

        if (checkForTabRemoved(refreshData)) {
          isInitTabsRequired = true;
          return; // continue to next $li in .each() since we removed this tab
        }

        if (checkForTabPropertiesUpdated(refreshData)) {
          isInitTabsRequired = true;
        }
      });
      return isInitTabsRequired;
    }

    function listenForDropdownMenuTabs($scroller, stc) {
      var $ddMenu; // for dropdown menus to show, we need to move them out of the
      // scroller and append them to the body

      $scroller.on(CONSTANTS.EVENTS.DROPDOWN_MENU_SHOW, handleDropdownShow).on(CONSTANTS.EVENTS.DROPDOWN_MENU_HIDE, handleDropdownHide);

      function handleDropdownHide(e) {
        // move the dropdown menu back into its tab
        $(e.target).append($ddMenu.off(CONSTANTS.EVENTS.CLICK));
      }

      function handleDropdownShow(e) {
        var $ddParentTabLi = $(e.target),
            ddLiOffset = $ddParentTabLi.offset(),
            $currActiveTab = $scroller.find('li[role="presentation"].active'),
            ddMenuRightX,
            tabsContainerMaxX,
            ddMenuTargetLeft;
        $ddMenu = $ddParentTabLi.find('.dropdown-menu').attr('data-' + CONSTANTS.DATA_KEY_DDMENU_MODIFIED, true); // if the dropdown's parent tab li isn't already active,
        // we need to deactivate any active menu item in the dropdown

        if ($currActiveTab[0] !== $ddParentTabLi[0]) {
          $ddMenu.find('li.active').removeClass('active');
        } // we need to do our own click handling because the built-in
        // bootstrap handlers won't work since we moved the dropdown
        // menu outside the tabs container


        $ddMenu.on(CONSTANTS.EVENTS.CLICK, 'a[role="tab"]', handleClickOnDropdownMenuItem);
        $('body').append($ddMenu); // make sure the menu doesn't go off the right side of the page

        ddMenuRightX = $ddMenu.width() + ddLiOffset.left;
        tabsContainerMaxX = $scroller.width() - (stc.$slideRightArrow.outerWidth() + 1);
        ddMenuTargetLeft = ddLiOffset.left;

        if (ddMenuRightX > tabsContainerMaxX) {
          ddMenuTargetLeft -= ddMenuRightX - tabsContainerMaxX;
        }

        $ddMenu.css({
          'display': 'block',
          'top': ddLiOffset.top + $ddParentTabLi.outerHeight() - 2,
          'left': ddMenuTargetLeft
        });

        function handleClickOnDropdownMenuItem() {
          /* jshint validthis: true */
          var $selectedMenuItemAnc = $(this),
              $selectedMenuItemLi = $selectedMenuItemAnc.parent('li'),
              $selectedMenuItemDropdownMenu = $selectedMenuItemLi.parent('.dropdown-menu'),
              targetPaneId = $selectedMenuItemAnc.attr('href');

          if ($selectedMenuItemLi.hasClass('active')) {
            return;
          } // once we select a menu item from the dropdown, deactivate
          // the current tab (unless it's our parent tab), deactivate
          // any active dropdown menu item, make our parent tab active
          // (if it's not already), and activate the selected menu item


          $scroller.find('li.active').not($ddParentTabLi).add($selectedMenuItemDropdownMenu.find('li.active')).removeClass('active');
          $ddParentTabLi.add($selectedMenuItemLi).addClass('active'); // manually deactivate current active pane and activate our pane

          $('.tab-content .tab-pane.active').removeClass('active');
          $(targetPaneId).addClass('active');
        }
      }
    }

    function refreshDataDrivenTabs($container, options) {
      var instanceData = $container.data().scrtabs,
          scroller = instanceData.scroller,
          $navTabs = $container.find('.scrtabs-tab-container .nav-tabs'),
          $currTabContentPanesContainer = $container.find('.tab-content'),
          isInitTabsRequired = false,
          refreshData = {
        options: options,
        updatedTabsArray: instanceData.tabs,
        updatedTabsLiContent: instanceData.tabsLiContent,
        updatedTabsPostProcessors: instanceData.tabsPostProcessors,
        propNames: instanceData.propNames,
        ignoreTabPanes: instanceData.ignoreTabPanes,
        $navTabs: $navTabs,
        $currTabLis: $navTabs.find('> li'),
        $currTabContentPanesContainer: $currTabContentPanesContainer,
        $currTabContentPanes: $currTabContentPanesContainer.find('.tab-pane')
      }; // to preserve the tab positions if we're just adding or removing
      // a tab, don't completely rebuild the tab structure, but check
      // for differences between the new tabs array and the old

      if (checkForTabAdded(refreshData)) {
        isInitTabsRequired = true;
      }

      if (checkForTabsOrderChanged(refreshData)) {
        isInitTabsRequired = true;
      }

      if (checkForTabsRemovedOrUpdated(refreshData)) {
        isInitTabsRequired = true;
      }

      if (isInitTabsRequired) {
        scroller.initTabs();
      }

      return isInitTabsRequired;
    }

    function refreshTargetElementInstance($container, options) {
      if (!$container.data('scrtabs')) {
        // target element doesn't have plugin on it
        return;
      } // force a refresh if the tabs are static html or they're data-driven
      // but the data didn't change so we didn't call initTabs()


      if ($container.data('scrtabs').isWrapperOnly || !refreshDataDrivenTabs($container, options)) {
        $('body').trigger(CONSTANTS.EVENTS.FORCE_REFRESH);
      }
    }

    function _scrollToActiveTab() {
      /* jshint validthis: true */
      var $targetElInstance = $(this),
          scrtabsData = $targetElInstance.data('scrtabs');

      if (!scrtabsData) {
        return;
      }

      scrtabsData.scroller.scrollToActiveTab();
    }

    var methods = {
      destroy: function destroy() {
        var $targetEls = this;
        return $targetEls.each(destroyPlugin);
      },
      init: function init(options) {
        var $targetEls = this,
            targetElsLastIndex = $targetEls.length - 1,
            settings = $.extend({}, $.fn.scrollingTabs.defaults, options || {}); // ---- tabs NOT data-driven -------------------------

        if (!settings.tabs) {
          // just wrap the selected .nav-tabs element(s) in the scroller
          return $targetEls.each(function (index) {
            var dataObj = {
              isWrapperOnly: true
            },
                $targetEl = $(this).data({
              scrtabs: dataObj
            }),
                readyCallback = index < targetElsLastIndex ? null : function () {
              $targetEls.trigger(CONSTANTS.EVENTS.TABS_READY);
            };
            wrapNavTabsInstanceInScroller($targetEl, settings, readyCallback);
          });
        } // ---- tabs data-driven -------------------------


        return $targetEls.each(function (index) {
          var $targetEl = $(this),
              readyCallback = index < targetElsLastIndex ? null : function () {
            $targetEls.trigger(CONSTANTS.EVENTS.TABS_READY);
          };
          buildNavTabsAndTabContentForTargetElementInstance($targetEl, settings, readyCallback);
        });
      },
      refresh: function refresh(options) {
        var $targetEls = this,
            settings = $.extend({}, $.fn.scrollingTabs.defaults, options || {});
        return $targetEls.each(function () {
          refreshTargetElementInstance($(this), settings);
        });
      },
      scrollToActiveTab: function scrollToActiveTab() {
        return this.each(_scrollToActiveTab);
      }
    };

    function destroyPlugin() {
      /* jshint validthis: true */
      var $targetElInstance = $(this),
          scrtabsData = $targetElInstance.data('scrtabs'),
          $tabsContainer;

      if (!scrtabsData) {
        return;
      }

      if (scrtabsData.enableSwipingElement === 'self') {
        $targetElInstance.removeClass(CONSTANTS.CSS_CLASSES.ALLOW_SCROLLBAR);
      } else if (scrtabsData.enableSwipingElement === 'parent') {
        $targetElInstance.closest('.scrtabs-tab-container').parent().removeClass(CONSTANTS.CSS_CLASSES.ALLOW_SCROLLBAR);
      }

      scrtabsData.scroller.off(CONSTANTS.EVENTS.DROPDOWN_MENU_SHOW).off(CONSTANTS.EVENTS.DROPDOWN_MENU_HIDE); // if there were any dropdown menus opened, remove the css we added to
      // them so they would display correctly

      scrtabsData.scroller.find('[data-' + CONSTANTS.DATA_KEY_DDMENU_MODIFIED + ']').css({
        display: '',
        left: '',
        top: ''
      }).off(CONSTANTS.EVENTS.CLICK).removeAttr('data-' + CONSTANTS.DATA_KEY_DDMENU_MODIFIED);

      if (scrtabsData.scroller.hasTabClickHandler) {
        $targetElInstance.find('a[data-toggle="tab"]').off('.scrtabs');
      }

      if (scrtabsData.isWrapperOnly) {
        // we just wrapped nav-tabs markup, so restore it
        // $targetElInstance is the ul.nav-tabs
        $tabsContainer = $targetElInstance.parents('.scrtabs-tab-container');

        if ($tabsContainer.length) {
          $tabsContainer.replaceWith($targetElInstance);
        }
      } else {
        // we generated the tabs from data so destroy everything we created
        if (scrtabsData.scroller && scrtabsData.scroller.initTabs) {
          scrtabsData.scroller.initTabs = null;
        } // $targetElInstance is the container for the ul.nav-tabs we generated


        $targetElInstance.find('.scrtabs-tab-container').add('.tab-content').remove();
      }

      $targetElInstance.removeData('scrtabs');

      while (--$.fn.scrollingTabs.nextInstanceId >= 0) {
        $(window).off(CONSTANTS.EVENTS.WINDOW_RESIZE + $.fn.scrollingTabs.nextInstanceId);
      }

      $('body').off(CONSTANTS.EVENTS.FORCE_REFRESH);
    }

    $.fn.scrollingTabs = function (methodOrOptions) {
      if (methods[methodOrOptions]) {
        return methods[methodOrOptions].apply(this, Array.prototype.slice.call(arguments, 1));
      } else if (!methodOrOptions || typeof methodOrOptions === 'object') {
        return methods.init.apply(this, arguments);
      } else {
        $.error('Method ' + methodOrOptions + ' does not exist on $.scrollingTabs.');
      }
    };

    $.fn.scrollingTabs.nextInstanceId = 0;
    $.fn.scrollingTabs.defaults = {
      tabs: null,
      propPaneId: 'paneId',
      propTitle: 'title',
      propActive: 'active',
      propDisabled: 'disabled',
      propContent: 'content',
      ignoreTabPanes: false,
      scrollToTabEdge: false,
      disableScrollArrowsOnFullyScrolled: false,
      forceActiveTab: false,
      reverseScroll: false,
      widthMultiplier: 1,
      tabClickHandler: null,
      cssClassLeftArrow: 'glyphicon glyphicon-chevron-left',
      cssClassRightArrow: 'glyphicon glyphicon-chevron-right',
      leftArrowContent: '',
      rightArrowContent: '',
      tabsLiContent: null,
      tabsPostProcessors: null,
      enableSwiping: false,
      enableRtlSupport: false,
      handleDelayedScrollbar: false,
      bootstrapVersion: 3
    };
  })(jQuery, window);

  $.ui.plugin.add('resizable', 'alsoResizeReverse', {
    start: function start() {
      var that = $(this).resizable('instance'),
          o = that.options;
      $(o.alsoResizeReverse).each(function () {
        var el = $(this);
        el.data('ui-resizable-alsoresizeReverse', {
          width: parseInt(el.width(), 10),
          height: parseInt(el.height(), 10),
          left: parseInt(el.css('left'), 10),
          top: parseInt(el.css('top'), 10)
        });
      });
    },
    resize: function resize(event, ui) {
      var that = $(this).resizable('instance'),
          o = that.options,
          os = that.originalSize,
          op = that.originalPosition,
          delta = {
        height: that.size.height - os.height || 0,
        width: that.size.width - os.width || 0,
        top: that.position.top - op.top || 0,
        left: that.position.left - op.left || 0
      };
      $(o.alsoResizeReverse).each(function () {
        var el = $(this),
            start = $(this).data('ui-resizable-alsoresize-reverse'),
            style = {},
            css = el.parents(ui.originalElement[0]).length ? ['width', 'height'] : ['width', 'height', 'top', 'left'];
        $.each(css, function (i, prop) {
          var sum = (start[prop] || 0) - (delta[prop] || 0);

          if (sum && sum >= 0) {
            style[prop] = sum || null;
          }
        });
        el.css(style);
      });
    },
    stop: function stop() {
      $(this).removeData('resizable-alsoresize-reverse');
    }
  });

  exports.CardRefresh = CardRefresh;
  exports.CardWidget = CardWidget;
  exports.ControlSidebar = ControlSidebar;
  exports.DirectChat = DirectChat;
  exports.Dropdown = Dropdown;
  exports.ExpandableTable = ExpandableTable;
  exports.Fullscreen = Fullscreen;
  exports.Layout = Layout;
  exports.PushMenu = PushMenu;
  exports.SidebarSearch = SidebarSearch;
  exports.Toasts = Toasts;
  exports.TodoList = TodoList;
  exports.Treeview = Treeview;

  Object.defineProperty(exports, '__esModule', { value: true });

})));
//# sourceMappingURL=adminlte.js.map
