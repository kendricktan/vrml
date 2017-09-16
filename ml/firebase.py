import pyrebase
from functools import partial
from aae.worker import aae_worker

#from aee.... import worker

config = {
    "apiKey": "AIzaSyCz-lRDfhixFe7Rsqis_5GRzwGw2Qk0ySA",
    "authDomain": "vrml-c8ee5.firebaseapp.com",
    "databaseURL": "https://vrml-c8ee5.firebaseio.com/",
    "storageBucket": "vrml-c8ee5.appspot.com/",
}

firebase = pyrebase.initialize_app(config)
database = firebase.database()
storage = firebase.storage()

def stream_handler(message):
    print(message['event'])
    print(message['data'])
    
def update_neurons(db, neural_number, activation_matrix):
    db.child("neurons").push({neural_number: neural_number, activation_matrix: activation_matrix})

def finding_similar(db, store, input_coordinate, image_ids, coordinates, distance_scores):
    """
    Args:
        db: firebase db object
        store: firebase storage object
        image_id: list of image ids
        coordinates: list of tuples
        distance_scores: list of floats
    """
    zipped_attributes = list(image_ids, coordinates, distance_scores))
    db.child("image_ids_top_100").push(zipped_attributes)

"""
build tree: update images
search: update params of top 100

training: update activations
"""
    

visualize_neurons = partial(update_neurons, database)
search = partial(finding_similar, database, storage)