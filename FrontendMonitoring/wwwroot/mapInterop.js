let currentMap = null;
let currentMarkers = [];
let markerClusterGroup = null;

window.initLeafletMap = (mapData) => {
    console.log("initLeafletMap function called!");
    console.log("Received mapData:", mapData);

    const mapContainer = document.getElementById('map');
    if (!mapContainer) return;

    if (mapContainer._leaflet_id) return;

    const map = L.map('map').setView([51.5719, 4.7683], 14); // Breda, Netherlands
    currentMap = map;

    // Add OpenStreetMap
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap contributors',
        maxZoom: 19
    }).addTo(map);

    // Initialize marker cluster group
    if (typeof L.markerClusterGroup !== 'undefined') {
        markerClusterGroup = L.markerClusterGroup({
            chunkedLoading: true,
            maxClusterRadius: 80
        });
        map.addLayer(markerClusterGroup);
    }

    if (mapData && mapData.length > 0) {
        mapData.forEach(item => {
            console.log(`Attempting to add marker at: ${item.lat}, ${item.lng}`);
            const marker = L.marker([item.lat, item.lng]).addTo(map);
            marker.bindPopup(`<b>Trash Type:</b> ${item.type}`);
            currentMarkers.push(marker);
        });
    }

    setTimeout(() => map.invalidateSize(), 500);
};

window.updateAdvancedMapMarkers = (mapData, useClusterView) => {
    console.log("updateAdvancedMapMarkers called with", mapData.length, "items");
    
    if (!currentMap) {
        // Initialize map if it doesn't exist
        const mapContainer = document.getElementById('map');
        if (!mapContainer) return;

        currentMap = L.map('map').setView([51.5719, 4.7683], 14); // Breda, Netherlands
        
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '© OpenStreetMap contributors',
            maxZoom: 19
        }).addTo(currentMap);

        // Initialize marker cluster group if available
        if (typeof L.markerClusterGroup !== 'undefined') {
            markerClusterGroup = L.markerClusterGroup({
                chunkedLoading: true,
                maxClusterRadius: 80,
                iconCreateFunction: function(cluster) {
                    const count = cluster.getChildCount();
                    let className = 'marker-cluster-small';
                    if (count > 10) className = 'marker-cluster-medium';
                    if (count > 100) className = 'marker-cluster-large';
                    
                    return new L.DivIcon({
                        html: '<div><span>' + count + '</span></div>',
                        className: 'marker-cluster ' + className,
                        iconSize: new L.Point(40, 40)
                    });
                }
            });
        }

        setTimeout(() => currentMap.invalidateSize(), 500);
    }

    // Clear existing markers
    currentMarkers.forEach(marker => currentMap.removeLayer(marker));
    currentMarkers = [];
    
    if (markerClusterGroup) {
        markerClusterGroup.clearLayers();
        currentMap.removeLayer(markerClusterGroup);
    }

    // Add new markers
    if (mapData && mapData.length > 0) {
        mapData.forEach(item => {
            const color = getMarkerColor(item);
            const icon = createCustomIcon(color, item.type);
            
            const marker = L.marker([item.lat, item.lng], { icon: icon });
            
            const popupContent = `
                <div class="custom-popup">
                    <h6 style="margin: 0 0 8px 0; color: #333; font-weight: 600;">${item.type}</h6>
                    <div style="font-size: 12px; color: #666;">
                        <div><strong>Locatie:</strong> ${item.location}</div>
                        <div><strong>Datum:</strong> ${item.time}</div>
                        <div><strong>Nauwkeurigheid:</strong> ${(item.confidence * 100).toFixed(1)}%</div>
                        <div><strong>Temperatuur:</strong> ${item.temperature}°C</div>
                        <div><strong>Status:</strong> 
                            <span style="color: ${item.cleaned ? '#4caf50' : '#f44336'};">
                                ${item.cleaned ? 'Opgeruimd' : 'Niet opgeruimd'}
                            </span>
                        </div>
                        ${item.verified ? '<div style="color: #2196f3;"><strong>✓ Geverifieerd</strong></div>' : ''}
                        ${item.cleanedTime ? `<div><strong>Opgeruimd op:</strong> ${item.cleanedTime}</div>` : ''}
                    </div>
                </div>
            `;
            
            marker.bindPopup(popupContent);
            
            if (useClusterView && markerClusterGroup) {
                markerClusterGroup.addLayer(marker);
            } else {
                marker.addTo(currentMap);
            }
            
            currentMarkers.push(marker);
        });

        if (useClusterView && markerClusterGroup) {
            currentMap.addLayer(markerClusterGroup);
        }

        // Fit map to show all markers if there are any
        if (currentMarkers.length > 0) {
            const group = new L.featureGroup(currentMarkers);
            currentMap.fitBounds(group.getBounds().pad(0.1));
        }
    }
};

function getMarkerColor(item) {
    if (item.cleaned) return '#4caf50'; // Green for cleaned
    if (item.verified) return '#2196f3'; // Blue for verified
    if (item.confidence > 0.8) return '#ff9800'; // Orange for high confidence
    return '#f44336'; // Red for default/low confidence
}

function createCustomIcon(color, type) {
    const iconHtml = `
        <div style="
            background-color: ${color};
            width: 20px;
            height: 20px;
            border-radius: 50%;
            border: 2px solid white;
            box-shadow: 0 2px 4px rgba(0,0,0,0.3);
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 10px;
            color: white;
            font-weight: bold;
        ">
            ${getTypeIcon(type)}
        </div>
    `;
    
    return new L.DivIcon({
        html: iconHtml,
        className: 'custom-marker',
        iconSize: [20, 20],
        iconAnchor: [10, 10],
        popupAnchor: [0, -10]
    });
}

function getTypeIcon(type) {
    switch (type?.toLowerCase()) {
        case 'plastic': return '♻';
        case 'glass': return '🥃';
        case 'paper': return '📄';
        case 'metal': return '🔧';
        case 'organic': return '🍃';
        default: return '🗑';
    }
}

window.fitMapToMarkers = () => {
    if (currentMap && currentMarkers.length > 0) {
        const group = new L.featureGroup(currentMarkers);
        currentMap.fitBounds(group.getBounds().pad(0.1));
    }
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
            const map = L.map('map').setView([51.5719, 4.7683], 14); // Breda, Netherlands

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
