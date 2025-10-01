/**
 * Dynamic Color Detection for Bootstrap Breadcrumb3
 * Automatically extracts colors from Bootstrap text-bg-* classes
 * and updates CSS custom properties for breadcrumb3 components
 * 
 * Author: Mark.Pelf
 */

(function() {
    'use strict';
    
    /**
     * Extracts the computed color from a Bootstrap text-bg class
     * @param {string} className - The Bootstrap class name (e.g., 'text-bg-primary')
     * @returns {string} The computed color value
     */
    function getTextBgColor(className) {
        // Create a temporary element with the Bootstrap class
        const tempElement = document.createElement('div');
        tempElement.className = className;
        tempElement.style.position = 'absolute';
        tempElement.style.visibility = 'hidden';
        tempElement.style.pointerEvents = 'none';
        tempElement.style.top = '-9999px';
        
        // Add to DOM temporarily to compute styles
        document.body.appendChild(tempElement);
        
        // Get the computed color
        const computedStyle = window.getComputedStyle(tempElement);
        const color = computedStyle.color;
        
        // Remove the temporary element
        document.body.removeChild(tempElement);
        
        return color;
    }
    
    /**
     * Updates CSS custom properties based on Bootstrap text-bg classes
     */
    function updateBreadcrumbColors() {
        try {
            // All 8 basic Bootstrap color variants with their corresponding CSS properties
            const colorVariants = [
                { class: 'text-bg-primary', property: '--_bs-primary-text-dynamic', fallback: '#fff' },
                { class: 'text-bg-secondary', property: '--_bs-secondary-text-dynamic', fallback: '#fff' },
                { class: 'text-bg-success', property: '--_bs-success-text-dynamic', fallback: '#fff' },
                { class: 'text-bg-info', property: '--_bs-info-text-dynamic', fallback: '#000' },
                { class: 'text-bg-warning', property: '--_bs-warning-text-dynamic', fallback: '#000' },
                { class: 'text-bg-danger', property: '--_bs-danger-text-dynamic', fallback: '#fff' },
                { class: 'text-bg-light', property: '--_bs-light-text-dynamic', fallback: '#000' },
                { class: 'text-bg-dark', property: '--_bs-dark-text-dynamic', fallback: '#fff' }
            ];
            
            const detectedColors = {};
            
            colorVariants.forEach(variant => {
                try {
                    const color = getTextBgColor(variant.class);
                    document.documentElement.style.setProperty(variant.property, color);
                    detectedColors[variant.class] = color;
                } catch (error) {
                    console.warn(`Breadcrumb3: Could not detect color for ${variant.class}, using fallback ${variant.fallback}`);
                    document.documentElement.style.setProperty(variant.property, variant.fallback);
                    detectedColors[variant.class] = variant.fallback;
                }
            });
            
            console.log('Breadcrumb3: Dynamic colors updated successfully');
            console.log('Detected colors:', detectedColors);
            
            return detectedColors;
            
        } catch (error) {
            console.warn('Breadcrumb3: Could not detect Bootstrap colors, using fallbacks', error);
            return null;
        }
    }
    
    /**
     * Initialize color detection when DOM is ready
     */
    function init() {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', updateBreadcrumbColors);
        } else {
            // DOM already loaded
            updateBreadcrumbColors();
        }
        
        // Also update when window loads (in case Bootstrap CSS loads asynchronously)
        window.addEventListener('load', updateBreadcrumbColors);
    }
    
    /**
     * Public API to manually refresh colors (useful when switching themes dynamically)
     */
    window.Breadcrumb3 = window.Breadcrumb3 || {};
    window.Breadcrumb3.refreshColors = updateBreadcrumbColors;
    window.Breadcrumb3.getDetectedColors = function() {
        return updateBreadcrumbColors();
    };
    
    // Initialize
    init();
    
})();