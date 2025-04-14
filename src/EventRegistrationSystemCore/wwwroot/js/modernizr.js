/*!
 * Modernizr Core Functionality (based on v2.8.3)
 * Lightweight version for .NET 9.0 application
 * 
 * Original copyright:
 * Modernizr v2.8.3
 * www.modernizr.com
 * Copyright (c) Faruk Ates, Paul Irish, Alex Sexton
 * Available under the BSD and MIT licenses: www.modernizr.com/license/
 */

(function(window, document, undefined) {
    // Create Modernizr namespace if it doesn't exist
    // This allows our implementation to coexist with the minimal one in legacy-compatibility.js
    var Modernizr = window.Modernizr || {};
    
    // Core variables
    var docElement = document.documentElement;
    var mod = 'modernizr';
    var modElem = document.createElement(mod);
    var mStyle = modElem.style;
    var inputElem = document.createElement('input');
    var toString = {}.toString;
    
    // CSS prefixes
    var prefixes = ' -webkit- -moz- -o- -ms- '.split(' ');
    var omPrefixes = 'Webkit Moz O ms';
    var cssomPrefixes = omPrefixes.split(' ');
    var domPrefixes = omPrefixes.toLowerCase().split(' ');
    
    // Storage for test results
    var tests = {};
    var inputs = {};
    var attrs = {};
    
    // Helper functions
    function setCss(str) {
        mStyle.cssText = str;
    }
    
    function setCssAll(str1, str2) {
        return setCss(prefixes.join(str1 + ';') + (str2 || ''));
    }
    
    function is(obj, type) {
        return typeof obj === type;
    }
    
    function contains(str, substr) {
        return !!~('' + str).indexOf(substr);
    }
    
    function testProps(props, prefixed) {
        for (var i in props) {
            var prop = props[i];
            if (!contains(prop, "-") && mStyle[prop] !== undefined) {
                return prefixed == 'pfx' ? prop : true;
            }
        }
        return false;
    }
    
    function testDOMProps(props, obj, elem) {
        for (var i in props) {
            var item = obj[props[i]];
            if (item !== undefined) {
                if (elem === false) return props[i];
                if (is(item, 'function')) {
                    return item.bind(elem || obj);
                }
                return item;
            }
        }
        return false;
    }
    
    function testPropsAll(prop, prefixed, elem) {
        var ucProp = prop.charAt(0).toUpperCase() + prop.slice(1);
        var props = (prop + ' ' + cssomPrefixes.join(ucProp + ' ') + ucProp).split(' ');
        
        if (is(prefixed, "string") || is(prefixed, "undefined")) {
            return testProps(props, prefixed);
        } else {
            props = (prop + ' ' + (domPrefixes).join(ucProp + ' ') + ucProp).split(' ');
            return testDOMProps(props, prefixed, elem);
        }
    }
    
    // Test for CSS features
    tests['flexbox'] = function() {
        return testPropsAll('flexWrap');
    };
    
    tests['flexboxlegacy'] = function() {
        return testPropsAll('boxDirection');
    };
    
    tests['canvas'] = function() {
        var elem = document.createElement('canvas');
        return !!(elem.getContext && elem.getContext('2d'));
    };
    
    tests['canvastext'] = function() {
        return !!(Modernizr['canvas'] && is(document.createElement('canvas').getContext('2d').fillText, 'function'));
    };
    
    tests['webgl'] = function() {
        return !!window.WebGLRenderingContext;
    };
    
    tests['touch'] = function() {
        var bool;
        
        if (('ontouchstart' in window) || window.DocumentTouch && document instanceof DocumentTouch) {
            bool = true;
        } else {
            bool = false;
        }
        
        return bool;
    };
    
    tests['geolocation'] = function() {
        return 'geolocation' in navigator;
    };
    
    tests['postmessage'] = function() {
        return !!window.postMessage;
    };
    
    tests['websqldatabase'] = function() {
        return !!window.openDatabase;
    };
    
    tests['indexedDB'] = function() {
        return !!testPropsAll("indexedDB", window);
    };
    
    tests['hashchange'] = function() {
        return ('onhashchange' in window) && (document.documentMode === undefined || document.documentMode > 7);
    };
    
    tests['history'] = function() {
        return !!(window.history && history.pushState);
    };
    
    tests['draganddrop'] = function() {
        var div = document.createElement('div');
        return ('draggable' in div) || ('ondragstart' in div && 'ondrop' in div);
    };
    
    tests['websockets'] = function() {
        return 'WebSocket' in window || 'MozWebSocket' in window;
    };
    
    tests['rgba'] = function() {
        setCss('background-color:rgba(150,255,150,.5)');
        return contains(mStyle.backgroundColor, 'rgba');
    };
    
    tests['hsla'] = function() {
        setCss('background-color:hsla(120,40%,100%,.5)');
        return contains(mStyle.backgroundColor, 'rgba') || contains(mStyle.backgroundColor, 'hsla');
    };
    
    tests['multiplebgs'] = function() {
        setCss('background:url(https://),url(https://),red url(https://)');
        return (/(url\s*\(.*?){3}/).test(mStyle.background);
    };
    
    tests['backgroundsize'] = function() {
        return testPropsAll('backgroundSize');
    };
    
    tests['borderimage'] = function() {
        return testPropsAll('borderImage');
    };
    
    tests['borderradius'] = function() {
        return testPropsAll('borderRadius');
    };
    
    tests['boxshadow'] = function() {
        return testPropsAll('boxShadow');
    };
    
    tests['textshadow'] = function() {
        return document.createElement('div').style.textShadow === '';
    };
    
    tests['opacity'] = function() {
        setCssAll('opacity:.55');
        return (/^0.55$/).test(mStyle.opacity);
    };
    
    tests['cssanimations'] = function() {
        return testPropsAll('animationName');
    };
    
    tests['csscolumns'] = function() {
        return testPropsAll('columnCount');
    };
    
    tests['cssgradients'] = function() {
        var str1 = 'background-image:';
        var str2 = 'gradient(linear,left top,right bottom,from(#9f9),to(white));';
        var str3 = 'linear-gradient(left top,#9f9, white);';
        
        setCss(
            (str1 + '-webkit- '.split(' ').join(str2 + str1) +
            prefixes.join(str3 + str1)).slice(0, -str1.length)
        );
        
        return contains(mStyle.backgroundImage, 'gradient');
    };
    
    tests['cssreflections'] = function() {
        return testPropsAll('boxReflect');
    };
    
    tests['csstransforms'] = function() {
        return !!testPropsAll('transform');
    };
    
    tests['csstransforms3d'] = function() {
        var ret = !!testPropsAll('perspective');
        return ret;
    };
    
    tests['csstransitions'] = function() {
        return testPropsAll('transition');
    };
    
    tests['localstorage'] = function() {
        try {
            localStorage.setItem(mod, mod);
            localStorage.removeItem(mod);
            return true;
        } catch(e) {
            return false;
        }
    };
    
    tests['sessionstorage'] = function() {
        try {
            sessionStorage.setItem(mod, mod);
            sessionStorage.removeItem(mod);
            return true;
        } catch(e) {
            return false;
        }
    };
    
    tests['webworkers'] = function() {
        return !!window.Worker;
    };
    
    tests['applicationcache'] = function() {
        return !!window.applicationCache;
    };
    
    tests['svg'] = function() {
        return !!document.createElementNS && !!document.createElementNS('http://www.w3.org/2000/svg', 'svg').createSVGRect;
    };
    
    tests['inlinesvg'] = function() {
        var div = document.createElement('div');
        div.innerHTML = '<svg/>';
        return (div.firstChild && div.firstChild.namespaceURI) == 'http://www.w3.org/2000/svg';
    };
    
    tests['smil'] = function() {
        return !!document.createElementNS && /SVGAnimate/.test(toString.call(document.createElementNS('http://www.w3.org/2000/svg', 'animate')));
    };
    
    tests['svgclippaths'] = function() {
        return !!document.createElementNS && /SVGClipPath/.test(toString.call(document.createElementNS('http://www.w3.org/2000/svg', 'clipPath')));
    };
    
    // Run all tests and populate Modernizr object
    for (var feature in tests) {
        if (tests.hasOwnProperty(feature)) {
            var featureName = feature.toLowerCase();
            Modernizr[featureName] = tests[feature]();
        }
    }
    
    // Add prefixed method
    Modernizr.prefixed = function(prop, obj, elem) {
        if (!obj) {
            return testPropsAll(prop, 'pfx');
        } else {
            return testPropsAll(prop, obj, elem);
        }
    };
    
    // Expose Modernizr
    window.Modernizr = Modernizr;
    
})(window, document);
