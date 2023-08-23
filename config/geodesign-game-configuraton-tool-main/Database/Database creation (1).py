#!/usr/bin/env python
# coding: utf-8

# # Create the database

# ## Librairies

# In[1]:


pip install Flask


# In[2]:


from flask import Flask, render_template, request
import sqlite3


# ### Initialization

# In[3]:


app = Flask(__name__)


# ## Define the database

# In[4]:


def create_db():
    conn = sqlite3.connect('config_generator.db')
    c = conn.cursor()
    # Create the 'cities' table
    c.execute('''CREATE TABLE IF NOT EXISTS cities (
                 id INTEGER PRIMARY KEY,
                 name TEXT NOT NULL,
                 description TEXT NOT NULL,
                 ebas TEXT NOT NULL,
                 mean_cost INTEGER NOT NULL,
                 currency TEXT NOT NULL,
                 location TEXT NOT NULL,
                 budget INTEGER NOT NULL
                 )''')
    # Create the 'ebas' table
    c.execute('''CREATE TABLE IF NOT EXISTS ebas (
                 id INTEGER PRIMARY KEY,
                 name TEXT NOT NULL,
                 description TEXT NOT NULL,
                 icon TEXT NOT NULL,
                 cost INTEGER NOT NULL
                 )''')
    conn.commit()
    conn.close()


# ### Initial EBAs

# In[ ]:


def insert_initial_ebas():
    conn = sqlite3.connect('config_generator.db')
    c = conn.cursor()
    # Insert sample EBA data
    ebas_data = [
        ('6c3c338f-5587-4d93-8e6c-a0b23cc78989', 'Dune Stabilization', 'Dune stabilization refers to the process of managing and preserving sand dunes to prevent erosion and maintain their ecological and environmental functions...', 'Dune', 70000),
        ('6c3c338f-5587-4d93-8e6c-a0b23cc78989', 'Tree plantation', 'Tree plantation is the process of planting and establishing trees in various locations, including forests, urban areas, and agricultural lands, with the aim of improving the environment and achieving multiple benefits...', 'Green', 70000),
        ('6c3c338f-5587-4d93-8e6c-a0b23cc78989', 'Grasslands', 'Protection and restoration of areas dominated by a continuous cover of grasses, without the presence of taller plants such as trees or shrubs...', 'Grass', 70000),
        ('6c3c338f-5587-4d93-8e6c-a0b23cc78989', 'Filtering water', 'Grass drainage, designed to absorb water and reduce permanently drained and often impermeable areas...', 'Filter', 70000),
        ('6c3c338f-5587-4d93-8e6c-a0b23cc78989', 'Flood plain enlargement/restoration', 'Enlargement and restoration of river flood plains, consisting out of lowering the level and/or increasing the width of the flood plain area next to a riverbed...', 'Flood', 70000),
        ('6c3c338f-5587-4d93-8e6c-a0b23cc78989', "Filtering water from the city's rooftops and parkings", "Multi-levelled gardens or parks with shallow depressions that use a special mix of sand and compost that allow water to soak in rapidly...", 'Filter', 70000),
        ('6c3c338f-5587-4d93-8e6c-a0b23cc78989', 'Cope with storms and floods', 'Landscape designed to catch water from a higher elevation and permanently hold run-off from melted water and precipitation...', 'Storm', 70000),
        ('6c3c338f-5587-4d93-8e6c-a0b23cc78989', 'Introduction and/or restoration of Bioswale', 'Landscape depression that receives and filters water runoffs back to the sewer water system through water-holding vegetation and organic materials...', 'Bioswale', 70000),
        ('6c3c338f-5587-4d93-8e6c-a0b23cc78989', 'Marram grass planting', 'Planting vegetation on sand dunes for coastal protection...', 'Grass', 70000),
        ('6c3c338f-5587-4d93-8e6c-a0b23cc78989', 'Peatland restoration', 'Restoration of the original peatland by rewetting the surface area through blocking canals, restoration of peat vegetation and planting of (resilient) peat species...', 'Peat', 70000),
        ('6c3c338f-5587-4d93-8e6c-a0b23cc78989', 'Seagrass meadow restoration', 'Seagrass meadows are underwater ecosystems formed by saltwater plants with roots and rhizomes anchored in the seafloor sand...', 'Seagrass', 70000),
        ('6c3c338f-5587-4d93-8e6c-a0b23cc78989', 'Estuaries protection and restoration', 'Protection and restoration of estuaries by providing stabilisation, reduce the force of incoming waves, while constituting an important source of carbon sequestration in coastal areas...', 'Estuary', 70000),
        ('6c3c338f-5587-4d93-8e6c-a0b23cc78989', 'River bed deepening', 'The lowering and deepening of the riverbed in order to accommodate greater depths of water to prevent overflow and thus flooding...', 'River', 70000),
        ('6c3c338f-5587-4d93-8e6c-a0b23cc78989', 'Dune restoration', 'Planting indigenous climate-resilient dune plants that pioneer revegetation, and will facilitate to naturally regenerate the dune ridge...', 'Dune', 70000),
        ('6c3c338f-5587-4d93-8e6c-a0b23cc78989', 'Linking ecosystems', 'Roads and paths (for motorized and non-motorized traffic respectively) aligned/bordered/surrounded with significant presence of vegetation, trees, and plants...', 'Link', 70000),
        ('6c3c338f-5587-4d93-8e6c-a0b23cc78989', 'Cultivate in town', 'Urban agriculture practices to grow crops or animals for personal consumption or to sell locally within and around cities...', 'Urban', 70000),
        ('6c3c338f-5587-4d93-8e6c-a0b23cc78989', 'Saltmarsh and mudflat management and restoration', 'Management and restoration of areas through hydrologic restoration/re-establishment of tidal hydrodynamics in saltmarshers and mudflats...', 'Saltmarsh', 70000),
        ('6c3c338f-5587-4d93-8e6c-a0b23cc78989', 'Promote water infiltration', 'Land depressions designed to have water-storage capacity and manage surface runoff water during rainfall events...', 'Infiltration', 70000),
        
        # Add more EBA data as needed
    ]
    c.executemany('''INSERT INTO ebas (id, name, description, icon, cost)
                     VALUES (?, ?, ?, ?, ?)''', ebas_data)
    conn.commit()
    conn.close()


# ## Create the route

# In[5]:


@app.route('/generate_config', methods=['POST'])
def generate_config():
    # Retrieve form data
    city_name = request.form['city-name']
    city_description = request.form['city-description']
    selected_ebas = request.form.getlist('selected-ebas')
    mean_cost = int(request.form['mean-cost'])
    currency = request.form['currency']
    map_center = request.form['map-center']
    city_budget = int(request.form['city-budget'])

    # Convert the list of selected ebas to a string for storage in the database
    ebas_str = ','.join(selected_ebas)

    # Insert data into the database
    conn = sqlite3.connect('config_generator.db')
    c = conn.cursor()
    c.execute('''INSERT INTO cities (name, description, ebas, mean_cost, currency, location, budget)
                 VALUES (?, ?, ?, ?, ?, ?, ?)''',
              (city_name, city_description, ebas_str, mean_cost, currency, map_center, city_budget))
    conn.commit()
    conn.close()

    # Return a response or redirect to another page
    return 'Config generated successfully!'


# ### Running the app

# In[6]:


if __name__ == '__main__':
    create_db()  # Create the database and tables when the app starts
    insert_initial_ebas()  # Insert initial EBA data
    app.run(debug=True)

