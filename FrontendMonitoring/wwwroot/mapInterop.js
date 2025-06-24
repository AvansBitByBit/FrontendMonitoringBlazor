window.initLeafletMap = (mapData) => {
    console.log("initLeafletMap function called!");

    // ik log ff alles omdat ik niet snap wwaar het misgaat met die stomme pins
    console.log("Received mapData:", mapData);

    const mapContainer = document.getElementById('map');
    if (!mapContainer) return;

    if (mapContainer._leaflet_id) return;

    const map = L.map('map').setView([51.5893, 4.7750], 13); // Set initial map center

    // Add OpenStreetMap
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap contributors',
        maxZoom: 19
    }).addTo(map);

    mapData.forEach(item => {
        console.log(`Attempting to add marker at: ${item.lat}, ${item.lng}`); // ik zie deze nooit in console maar waarom idk
        const marker = L.marker([item.lat, item.lng]).addTo(map);
        marker.bindPopup(`<b>Trash Type:</b> ${item.type}`);
    });

    setTimeout(() => map.invalidateSize(), 500);
};

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
