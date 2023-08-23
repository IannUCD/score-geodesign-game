from flask import Flask, render_template, request, jsonify, send_file
import json
import os
import zipfile
from io import BytesIO

app = Flask(__name__)

@app.route('/')
def index():
    return render_template('index.html')

@app.route('/generate_config', methods=['POST'])
def generate_config():
    try:
        # Get form data from the POST request
        city_name = request.form['city-name']
        city_description = request.form['city-description']
        selected_ebas = request.form.getlist('selected-ebas')
        mean_cost = request.form['mean-cost']
        currency = request.form['currency']
        map_center = json.loads(request.form['map-center'])  # Get map center as JSON
        map_bounds = json.loads(request.form['map-bounds'])  # Get map bounds as JSON

        # Get form data from the POST request
        selected_ebas = request.form.getlist('selected-ebas')

        # Construct eba JSON data based on the selected_ebas and your database
        eba_data = []  # Initialize an empty list
        for eba_id in selected_ebas:
        # Fetch EBA details from your database and append to eba_data
        eba_details = {...}  # Fetch details based on eba_id
        eba_data.append(eba_details)


        # Construct globals JSON data
        globals_data = {
            "version": "0.1",
            "center": map_center,
            "game_currency": currency,
            "bounds": map_bounds
        }

        # Construct local JSON data
        local_data = {
            "jurisdiction": [
                {
                    "name": city_name,
                    "description": city_description,
                    "eba_array": selected_ebas,
                    "total_available_budget": 200000,  # Replace with city budget from form data
                    "currency": currency,
                    "logo": "score.com/images/dunlaoghaire.jpg",
                    "center": [map_center['lat'], map_center['lng']]  # Use map center for "local" data
                }
            ]
        }

        # Create a zip file and add the JSON files to it
        zip_file = BytesIO()
        with zipfile.ZipFile(zip_file, 'w') as zipf:
            zipf.writestr('eba.json', json.dumps(eba_data))
            zipf.writestr('globals.json', json.dumps(globals_data))
            zipf.writestr('local.json', json.dumps(local_data))

        # Set the response headers for downloading the zip file
        zip_file.seek(0)
        return send_file(zip_file, attachment_filename='config.zip', as_attachment=True)

    except Exception as e:
        return jsonify({'error': str(e)}), 500

if __name__ == '__main__':
    app.run(debug=True)
