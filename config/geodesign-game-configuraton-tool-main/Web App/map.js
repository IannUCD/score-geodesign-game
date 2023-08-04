// map.js

// JavaScript code for handling the map and updating hidden input fields
const map = L.map('map').setView([51.5074, -0.1278], 10);

L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
}).addTo(map);

// Event listener for form submission
document.getElementById('config-form').addEventListener('submit', function (event) {
    event.preventDefault();
    // Get map center and bounds
    const mapCenter = map.getCenter();
    const mapBounds = map.getBounds();

    // Update the hidden input fields with the map data
    document.getElementById('map-center').value = JSON.stringify({
        lat: mapCenter.lat,
        lng: mapCenter.lng
    });
    document.getElementById('map-bounds').value = JSON.stringify({
        north: mapBounds.getNorth(),
        south: mapBounds.getSouth(),
        east: mapBounds.getEast(),
        west: mapBounds.getWest()
    });

    // Submit the form
    this.submit();
});
