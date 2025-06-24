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
