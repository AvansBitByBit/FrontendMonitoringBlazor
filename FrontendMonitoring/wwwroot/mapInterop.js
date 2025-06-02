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
