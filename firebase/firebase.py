import pyrebase
from functools import partial
#from aee.... import worker

config = {
    "databaseURL": "https://vrml-c8ee5.firebaseio.com/",
}

firebase = pyrebase.initialize_app(config)
database = firebase.database()
storage = firebase.storage()

def stream_handler(message):
    print(message['event'])
    print(message['data'])
    
def update_neurons(db, neural_number, activation_matrix):
    db.child("neurons").push({neural_number: neural_number, activation_matrix: activation_matrix})

def finding_similar(db, store, input_coordinate, coordinates, images, distance_scores):
    """
    Args:
        db: firebase db object
        store: firebase storage object
        coordinates: list of tuples
        images: list of image paths
        distance_scores: list of floats
    """
    zipped_attributes = zip(coordinates, images, distance_scores)
    db.child("images_top_100").push(zipped_attributes)



visualize_neurons = partial(update_neurons, database)
find_similar = partial(finding_neurons, database)