window.initMap = () => {
    console.log("Initializing map...");

    const mapContainer = document.getElementById('map');
    if (!mapContainer) {
        console.error("Map container not found!");
        return;
    }

    // Prevent re-initialization
    if (mapContainer._leaflet_id) {
        console.log("Map already initialized.");
        return;
    }

    setTimeout(() => {
        try {
            const map = L.map('map').setView([51.5893, 4.7750], 13);

            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '© OpenStreetMap contributors',
                maxZoom: 19,
            }).addTo(map);

            setTimeout(() => {
                map.invalidateSize();
                console.log("Map size invalidated.");
            }, 500);
        } catch (error) {
            console.error("Error initializing Leaflet map:", error);
        }
    }, 300);
};

window.hasAuthToken = () => {
    return localStorage.getItem('authToken') !== null;
};

window.clearAuthToken = () => {
    localStorage.removeItem('authToken');
};

window.downloadCsv = (filename, csvContent) => {
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = filename;
    link.click();
    URL.revokeObjectURL(link.href);
};

// Mobile-specific fixes for better touch handling
window.initMobileFixes = () => {
    console.log("Initializing mobile fixes...");
    
    // Prevent zoom on double tap for iOS Safari
    let lastTouchEnd = 0;
    document.addEventListener('touchend', function (event) {
        const now = (new Date()).getTime();
        if (now - lastTouchEnd <= 300) {
            event.preventDefault();
        }
        lastTouchEnd = now;
    }, false);
    
    // Improve button touch handling
    const addTouchClass = (element) => {
        element.addEventListener('touchstart', function() {
            this.classList.add('mud-touch-active');
        });
        
        element.addEventListener('touchend', function() {
            setTimeout(() => {
                this.classList.remove('mud-touch-active');
            }, 150);
        });
        
        element.addEventListener('touchcancel', function() {
            this.classList.remove('mud-touch-active');
        });
    };
    
    // Apply to all buttons and clickable elements
    const observer = new MutationObserver(function(mutations) {
        mutations.forEach(function(mutation) {
            mutation.addedNodes.forEach(function(node) {
                if (node.nodeType === 1) { // Element node
                    // Find all clickable elements
                    const clickableElements = node.querySelectorAll('.mud-button, .mud-icon-button, .mud-list-item, .mud-nav-link, .mud-checkbox, .mud-radio');
                    clickableElements.forEach(addTouchClass);
                    
                    // Also apply to the node itself if it's clickable
                    if (node.matches && node.matches('.mud-button, .mud-icon-button, .mud-list-item, .mud-nav-link, .mud-checkbox, .mud-radio')) {
                        addTouchClass(node);
                    }
                }
            });
        });
    });
    
    observer.observe(document.body, {
        childList: true,
        subtree: true
    });
    
    // Initial application to existing elements
    document.querySelectorAll('.mud-button, .mud-icon-button, .mud-list-item, .mud-nav-link, .mud-checkbox, .mud-radio').forEach(addTouchClass);
};

// Initialize mobile fixes when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', window.initMobileFixes);
} else {
    window.initMobileFixes();
}

// Re-initialize mobile fixes after Blazor reconnection
window.addEventListener('load', () => {
    setTimeout(window.initMobileFixes, 1000);
});
