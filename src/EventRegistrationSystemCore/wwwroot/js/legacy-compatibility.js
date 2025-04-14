/**
 * Legacy Compatibility Script
 * 
 * This script provides compatibility between .NET Framework 4.8 and .NET 9.0 applications
 * by implementing functionality that was previously handled by bundling and other legacy features.
 */

// Create a namespace for bundling compatibility
var Bundles = {
    // Simulate the Scripts.Render functionality
    ScriptsRender: function(bundlePath) {
        console.log('Scripts bundle requested:', bundlePath);
        // This is just for logging purposes, actual scripts are loaded via _Layout.cshtml
    },
    
    // Simulate the Styles.Render functionality
    StylesRender: function(bundlePath) {
        console.log('Styles bundle requested:', bundlePath);
        // This is just for logging purposes, actual styles are loaded via _Layout.cshtml
    }
};

// Create a global Scripts object to match legacy pattern
var Scripts = {
    Render: Bundles.ScriptsRender
};

// Create a global Styles object to match legacy pattern
var Styles = {
    Render: Bundles.StylesRender
};

// Modernizr compatibility (minimal implementation of commonly used features)
var Modernizr = Modernizr || {};
Modernizr.touch = 'ontouchstart' in window || navigator.maxTouchPoints > 0;
Modernizr.history = !!(window.history && window.history.pushState);
Modernizr.cssanimations = (function() {
    var el = document.createElement('div');
    var animations = {
        'animation': 'animationend',
        'OAnimation': 'oAnimationEnd',
        'MozAnimation': 'animationend',
        'WebkitAnimation': 'webkitAnimationEnd'
    };
    
    for (var t in animations) {
        if (el.style[t] !== undefined) {
            return true;
        }
    }
    return false;
})();

// Bootstrap 3 to Bootstrap 5 compatibility
$(document).ready(function() {
    // Convert data-toggle attributes to data-bs-toggle
    $('[data-toggle="dropdown"]').attr('data-bs-toggle', 'dropdown').removeAttr('data-toggle');
    $('[data-toggle="tooltip"]').attr('data-bs-toggle', 'tooltip').removeAttr('data-toggle');
    $('[data-toggle="popover"]').attr('data-bs-toggle', 'popover').removeAttr('data-toggle');
    $('[data-toggle="tab"]').attr('data-bs-toggle', 'tab').removeAttr('data-toggle');
    $('[data-toggle="modal"]').each(function() {
        $(this).attr('data-bs-toggle', 'modal');
        var target = $(this).attr('data-target');
        if (target) {
            $(this).attr('data-bs-target', target).removeAttr('data-target');
        }
        $(this).removeAttr('data-toggle');
    });
    
    // Convert data-dismiss attributes to data-bs-dismiss
    $('[data-dismiss="modal"]').attr('data-bs-dismiss', 'modal').removeAttr('data-dismiss');
    $('[data-dismiss="alert"]').attr('data-bs-dismiss', 'alert').removeAttr('data-dismiss');
    
    // Initialize Bootstrap 5 components
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
    
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });
});
