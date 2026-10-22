// Exposure's web half. Runs in Root.html, the document every game page sits in as an iframe. The plugin sends, through
// window.__slexposure.set(), the bar's numbers, the time-to-full line and each furniture button's exposure cost by its
// function id. The expedition meter (CoreUI1) gets the numbers in place of its percent and the line under the bar;
// the furniture menu (SelectMenu) gets "+N" on each button that adds exposure. A part sent as null is left exactly as
// the game draws it.
(function () {
  if (window.__slexposure) return;

  var POLL_MS = 500;
  var MARK = '__slexposurePatched';
  var state = { numbers: null, full: null, costs: null };
  var lastJson = '';
  var menus = [];

  var METER_CSS =
    'html.slexp-numbers .exp-info-overlay .exp-value:not(.slexp-value){display:none}' +
    '.slexp-full{position:absolute;top:100%;left:0;width:260px;margin-top:3px;text-align:center;font-size:12px;' +
    'font-weight:bold;color:#fff;text-shadow:1px 1px 2px black;white-space:nowrap;pointer-events:none}';

  var COST_CSS =
    '.slexp-cost{margin-left:6px;flex-shrink:0;font-family:Consolas,monospace;font-size:12px;font-weight:bold;' +
    'color:#FFC107;white-space:nowrap}';

  function addStyle(doc, id, css) {
    if (doc.getElementById(id) || !doc.head) return;

    var style = doc.createElement('style');

    style.id = id;
    style.textContent = css;
    doc.head.appendChild(style);
  }

  // A child this script owns: made when there is text for it, kept up to date, and removed when there is none.
  function ownChild(doc, parent, className, tag, text) {
    var node = parent.querySelector('.' + className.split(' ').pop());

    if (text === null || text === undefined) {
      if (node) node.parentNode.removeChild(node);

      return;
    }

    if (!node) {
      node = doc.createElement(tag);
      node.className = className;
      parent.appendChild(node);
    }

    if (node.textContent !== text) node.textContent = text;
  }

  // ===== Expedition meter =====

  // Vue draws the meter and drops the whole panel when the expedition ends, taking these with it, so they are put
  // back on every poll. The game's own percent is only hidden by a class, never changed.
  function fixMeter(w) {
    var doc = w.document;
    var panel = doc.querySelector('.exploration-mode-panel');

    addStyle(doc, 'slexp-meter-style', METER_CSS);
    doc.documentElement.classList.toggle('slexp-numbers', !!state.numbers);

    if (!panel) return;

    var overlay = panel.querySelector('.exp-info-overlay');

    if (overlay) ownChild(doc, overlay, 'exp-value slexp-value', 'span', state.numbers);

    ownChild(doc, panel, 'slexp-full', 'div', state.full);
  }

  // ===== Furniture menu =====

  // The menu keeps its buttons in a closure, so the list is read from the same message the page gets. The page's own
  // listener was added first and has built the buttons by the time the position message reaches this one.
  function watchMenu(w) {
    var entry = { win: w, list: [] };

    w[MARK] = true;
    addStyle(w.document, 'slexp-cost-style', COST_CSS);
    menus.push(entry);

    w.addEventListener('message', function (event) {
      var message = event.data;

      if (!message) return;

      try {
        if (message.type === 'WebUI_SelectMenu_MenuMsg') {
          var data = typeof message.data === 'string' ? JSON.parse(message.data) : message.data;

          if (data && Array.isArray(data.btnList)) entry.list = data.btnList;
        } else if (message.type === 'WebUI_SelectMenu_PositionMsg') {
          markCosts(entry);
        }
      } catch (e) {}
    });
  }

  function markCosts(entry) {
    var doc = entry.win.document;
    var buttons = doc.querySelectorAll('#menu-container .menu-btn[data-index]');

    for (var i = 0; i < buttons.length; i++) {
      var button = buttons[i];
      var item = entry.list[parseInt(button.getAttribute('data-index'), 10)];
      var cost = state.costs && item ? state.costs[item.funcId] : undefined;
      var text = typeof cost === 'number' ? '+' + cost : null;
      var label = button.querySelector('.menu-btn-text');

      if (label) ownCost(doc, button, label, text);
    }
  }

  // Right after the button's own text, so it sits beside the label and clear of the hotkey and the tooltip.
  function ownCost(doc, button, label, text) {
    var node = button.querySelector('.slexp-cost');

    if (text === null) {
      if (node) node.parentNode.removeChild(node);

      return;
    }

    if (!node) {
      node = doc.createElement('span');
      node.className = 'slexp-cost';
      label.parentNode.insertBefore(node, label.nextSibling);
    }

    if (node.textContent !== text) node.textContent = text;
  }

  // ===== Pages =====

  function pageWindows() {
    var frames = document.getElementsByTagName('iframe');
    var found = [];

    for (var i = 0; i < frames.length; i++) {
      try {
        var w = frames[i].contentWindow;

        if (w && w.document) found.push(w);
      } catch (e) {}
    }

    return found;
  }

  function isPage(w, path) {
    try {
      return String(w.location.pathname).indexOf(path) >= 0;
    } catch (e) {
      return false;
    }
  }

  function poll() {
    var windows = pageWindows();

    for (var i = 0; i < windows.length; i++) {
      var w = windows[i];

      try {
        if (isPage(w, '/CoreUI1/')) fixMeter(w);
        else if (isPage(w, '/SelectMenu/') && !w[MARK]) watchMenu(w);
      } catch (e) {}
    }

    menus = menus.filter(function (entry) {
      try {
        return !entry.win.closed && !!entry.win.document;
      } catch (e) {
        return false;
      }
    });
  }

  function set(next) {
    var json = JSON.stringify(next);

    if (json === lastJson) return;

    lastJson = json;
    state = next || { numbers: null, full: null, costs: null };
    poll();

    for (var i = 0; i < menus.length; i++) {
      try {
        markCosts(menus[i]);
      } catch (e) {}
    }
  }

  setInterval(function () {
    try {
      poll();
    } catch (e) {}
  }, POLL_MS);

  window.__slexposure = { set: set };
})();
