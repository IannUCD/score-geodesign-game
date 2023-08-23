from flask import Flask, render_template, request, jsonify, send_file
import json
import os
import zipfile
from io import BytesIO
import sqlite3

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
        map_center = json.loads(request.form['map-center'])
        map_bounds = json.loads(request.form['map-bounds'])

        # Connect to your SQLite database
        conn = sqlite3.connect('your_database_path_here.db')  # <-- Update this with your actual database path
        c = conn.cursor()
        
        eba_data = []

        for eba_id in selected_ebas:
            c.execute("SELECT * FROM ebas WHERE id=?", (eba_id, ))
            eba_details = c.fetchone()
            eba_dict = {
                'id': eba_details[0],
                'name': eba_details[1],
                'description': eba_details[2],
                'icon': eba_details[3],
                'cost': eba_details[4]
            }
            eba_data.append(eba_dict)

        # Close the database connection
        conn.close()

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

