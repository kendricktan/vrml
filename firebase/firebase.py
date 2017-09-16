import pyrebase
from functools import partial
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

def finding_similar(db, store, input_coordinate, coordinates, images, distance_scores):
    """
    Args:
        db: firebase db object
        store: firebase storage object
        coordinates: list of tuples
        images: list of image paths
        distance_scores: list of floats
    """
    zipped_attributes = list(zip((i for i in range(len(images))), coordinates, images, distance_scores))
    db.child("images_top_100").push(zipped_attributes)
    
    map(lambda idx, image: store.child("{}".format(idx)).put("{}".format(image)), enumerate(images))


visualize_neurons = partial(update_neurons, database)
find_similar = partial(finding_similar, database, storage)