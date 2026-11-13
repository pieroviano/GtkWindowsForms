// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Gtk.Windows.Forms.WebBrowsing;

/// <summary>
/// The JavaScript that stands in for the in-process DOM mono's
/// <c>
/// Mono.WebBrowser
/// </c> backend had.
/// </summary>
/// <remarks>
/// Injected into every document the engine loads. It maintains a registry mapping integer handles to
/// live nodes - so that a mutation posted seconds after a snapshot still addresses the same node rather
/// than re-querying by a selector that may since have matched something else - walks the document into
/// the JSON
/// <see cref="Gtk.Windows.Forms.WebBrowsing.DomSnapshot" /> reads, applies mutations, and forwards
/// delegated events back to the host.
///
/// The script is deliberately one string rather than a resource file: it has to survive the prepare
/// pipeline's file reorganisation as ordinary source, and inlining it keeps the injected code and the
/// C# that parses its output in one place, where a change to one is visible against the other.
/// </remarks>
internal static class DomBridgeScript
{
    /// <summary>
    /// The name the page uses to post DOM events back to the host, and the name the host registers as a
    /// script message handler.
    /// </summary>
    internal const string EventChannel = "gtkwfDomEvent";

    /// <summary>
    /// The global object the injected script installs, through which every other call is made.
    /// </summary>
    internal const string Global = "__gtkwfDom";

    /// <summary>
    /// The DOM event types the bridge listens for, at the document, in the capture phase, so that a
    /// handler attached to any element sees them without the page having to co-operate.
    /// </summary>
    internal static readonly string[] DelegatedEvents =
    {
        "click", "dblclick", "mousedown", "mouseup", "mousemove", "mouseover", "mouseout",
        "keydown", "keypress", "keyup", "focus", "blur", "scroll", "contextmenu",
        "drag", "dragend", "dragleave", "dragover", "load", "unload", "resize"
    };

    /// <summary>
    /// The script source, injected at document start into every frame the engine loads.
    /// </summary>
    internal static string Source =>
        @"(function () {
  if (window." + Global + @") { return; }

  var nodes = new Map();
  var handles = new WeakMap();
  var next = 1;

  function handleOf(node) {
    if (!node || node.nodeType !== 1) { return -1; }
    var existing = handles.get(node);
    if (existing !== undefined) { return existing; }
    var handle = next++;
    handles.set(node, handle);
    nodes.set(handle, node);
    return handle;
  }

  function nodeOf(handle) { return nodes.get(handle) || null; }

  function attributesOf(node) {
    var result = {};
    for (var i = 0; i < node.attributes.length; i++) {
      var attribute = node.attributes[i];
      result[attribute.name] = attribute.value;
    }
    return result;
  }

  function describe(node) {
    var children = [];
    for (var i = 0; i < node.children.length; i++) {
      children.push(handleOf(node.children[i]));
    }
    return {
      handle: handleOf(node),
      tagName: node.tagName,
      attributes: attributesOf(node),
      innerText: node.innerText === undefined ? (node.textContent || '') : node.innerText,
      innerHtml: node.innerHTML,
      outerHtml: node.outerHTML,
      style: node.getAttribute('style') || '',
      tabIndex: node.tabIndex || 0,
      disabled: !!node.disabled,
      clientWidth: node.clientWidth || 0,
      clientHeight: node.clientHeight || 0,
      offsetLeft: node.offsetLeft || 0,
      offsetTop: node.offsetTop || 0,
      offsetWidth: node.offsetWidth || 0,
      offsetHeight: node.offsetHeight || 0,
      scrollLeft: node.scrollLeft || 0,
      scrollTop: node.scrollTop || 0,
      scrollWidth: node.scrollWidth || 0,
      scrollHeight: node.scrollHeight || 0,
      parent: node.parentElement ? handleOf(node.parentElement) : -1,
      offsetParent: node.offsetParent ? handleOf(node.offsetParent) : -1,
      children: children
    };
  }

  function snapshot() {
    // The registry is rebuilt from the current tree on every snapshot, so handles handed out for a
    // document that has since been replaced cannot resolve to a node in the new one.
    nodes = new Map();
    handles = new WeakMap();
    next = 1;

    var all = [];
    var root = document.documentElement;
    if (root) {
      var walk = [root];
      while (walk.length) {
        var node = walk.shift();
        all.push(describe(node));
        for (var i = 0; i < node.children.length; i++) { walk.push(node.children[i]); }
      }
    }

    var docType = document.doctype ? document.doctype.name : '';
    var body = document.body;
    var active = document.activeElement;

    return JSON.stringify({
      title: document.title || '',
      url: document.URL || '',
      charset: document.characterSet || '',
      cookie: (function () { try { return document.cookie || ''; } catch (e) { return ''; } })(),
      domain: document.domain || '',
      docType: docType,
      backColor: document.body ? (document.body.bgColor || '') : '',
      foreColor: document.body ? (document.body.text || '') : '',
      linkColor: document.body ? (document.body.link || '') : '',
      activeLinkColor: document.body ? (document.body.aLink || '') : '',
      visitedLinkColor: document.body ? (document.body.vLink || '') : '',
      documentElement: root ? handleOf(root) : -1,
      body: body ? handleOf(body) : -1,
      activeElement: active && active.nodeType === 1 ? handleOf(active) : -1,
      nodes: all
    });
  }

  function mutate(request) {
    var node = request.target === -1 ? null : nodeOf(request.target);
    switch (request.kind) {
      case 'SetAttribute': if (node) { node.setAttribute(request.name, request.value); } break;
      case 'SetInnerHtml': if (node) { node.innerHTML = request.value; } break;
      case 'SetInnerText': if (node) { node.innerText = request.value; } break;
      case 'SetOuterHtml': if (node) { node.outerHTML = request.value; } break;
      case 'SetOuterText': if (node) { node.textContent = request.value; } break;
      case 'SetStyle': if (node) { node.setAttribute('style', request.value); } break;
      case 'SetTabIndex': if (node) { node.tabIndex = request.intValue; } break;
      case 'SetDisabled': if (node) { node.disabled = request.flag; } break;
      case 'SetScrollLeft': if (node) { node.scrollLeft = request.intValue; } break;
      case 'SetScrollTop': if (node) { node.scrollTop = request.intValue; } break;
      case 'Focus': if (node) { node.focus(); } break;
      case 'Blur': if (node) { node.blur(); } break;
      case 'ScrollIntoView': if (node) { node.scrollIntoView(request.flag); } break;
      case 'RaiseEvent': if (node) { node.dispatchEvent(new Event(request.name, { bubbles: true, cancelable: true })); } break;
      case 'AppendChild': {
        var child = nodeOf(request.related);
        if (node && child) { node.appendChild(child); }
        break;
      }
      case 'InsertBefore': {
        var inserted = nodeOf(request.related);
        var reference = request.reference === -1 ? null : nodeOf(request.reference);
        if (node && inserted) { node.insertBefore(inserted, reference); }
        break;
      }
      case 'CreateElement': {
        // The host allocated the handle, so register the new node under it rather than under a fresh one.
        var created = document.createElement(request.name);
        handles.set(created, request.target);
        nodes.set(request.target, created);
        break;
      }
      case 'Write': document.write(request.value); break;
      case 'SetDocumentProperty': {
        if (request.name === 'title') { document.title = request.value; }
        else if (request.name === 'cookie') { document.cookie = request.value; }
        else if (document.body) { document.body[request.name] = request.value; }
        break;
      }
    }
  }

  function invoke(handle, member, args) {
    var target = handle === -1 ? window : nodeOf(handle);
    if (!target) { return null; }
    var method = target[member];
    if (typeof method !== 'function') { return null; }
    return method.apply(target, args || []);
  }

  function post(event) {
    var target = event.target && event.target.nodeType === 1 ? event.target : null;
    var related = event.relatedTarget && event.relatedTarget.nodeType === 1 ? event.relatedTarget : null;
    var payload = {
      handle: target ? handleOf(target) : -1,
      type: event.type,
      alt: !!event.altKey,
      ctrl: !!event.ctrlKey,
      shift: !!event.shiftKey,
      keyCode: event.keyCode || 0,
      button: event.button === undefined ? -1 : event.button,
      buttons: event.buttons === undefined ? 0 : event.buttons,
      screenX: event.screenX || 0,
      screenY: event.screenY || 0,
      clientX: event.clientX || 0,
      clientY: event.clientY || 0,
      offsetX: event.offsetX || 0,
      offsetY: event.offsetY || 0,
      from: related ? handleOf(related) : -1,
      to: target ? handleOf(target) : -1
    };
    try {
      window.webkit.messageHandlers." + EventChannel + @".postMessage(JSON.stringify(payload));
    } catch (e) {
      // No channel registered: the host is not listening, which is not the page's problem.
    }
  }

  var delegated = [" + DelegatedEventLiterals + @"];
  for (var i = 0; i < delegated.length; i++) {
    // Capture phase, at the document: an element handler must fire without the page co-operating, and
    // without this bridge attaching a listener per element as the host asks for them.
    document.addEventListener(delegated[i], post, true);
  }

  window." + Global + @" = {
    snapshot: snapshot,
    mutate: mutate,
    invoke: invoke,
    handleOf: handleOf,
    nodeOf: nodeOf
  };
})();";

    private static string DelegatedEventLiterals => "'" + string.Join("', '", DelegatedEvents) + "'";
}
