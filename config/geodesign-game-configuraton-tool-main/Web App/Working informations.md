Alright, based on the files and their contents provided, here is a `README.md` for your folder:

---

# Geodesign Game Configuration Generator

This set of tools and interfaces allow for the generation of configurations for a Geodesign game. It consists of a web interface to provide the game settings, and the backend is managed using Flask. The generated configurations are packed into a `.zip` file for easy downloading.

## Files in this Folder:

### 1. `Configuration tool interface.html`

This is the main web interface where the game administrator can:

- Input the city configuration: including city name, description, currency, and budget.
- Configure the map's center and bounds using a provided interactive map interface.
- Choose Environment-based Actions (EBAs) from a list.

The interface uses the Leaflet library for map functionalities and connects to Mapbox for map tiles. 

### 2. `app.py`

This is the Flask application which provides the backend functionality. The following routes and functionalities are defined:

- `/`: Renders the main web interface.
- `/generate_config`: Handles POST requests from the web interface, processes the form data, fetches additional details from a SQLite database, and returns the generated configuration files packed into a `.zip` file.

### 3. `map.js`

A JavaScript file that offers functionalities for:

- Initializing the Leaflet map.
- Configuring the map to use Mapbox tiles.
- Handling the form submission and reading the map's current configuration.

Note: Please ensure you have the correct Mapbox access token when deploying.

## Dependencies:

- **Leaflet**: For map functionalities.
- **Mapbox**: Source of map tiles.
- **Flask**: For backend functionalities.
- **SQLite**: As the database solution for fetching EBA details.
