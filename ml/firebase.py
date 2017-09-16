import torch
import argparse
import pyrebase

import torchvision.datasets as datasets
import torchvision.transforms as transforms

from functools import partial
from aae.worker import aae_worker

config = {
    "apiKey": "AIzaSyCz-lRDfhixFe7Rsqis_5GRzwGw2Qk0ySA",
    "authDomain": "vrml-c8ee5.firebaseapp.com",
    "databaseURL": "https://vrml-c8ee5.firebaseio.com/",
    "storageBucket": "vrml-c8ee5.appspot.com",
}

firebase = pyrebase.initialize_app(config)
database = firebase.database()
storage = firebase.storage()


def update_neurons(db, activation_matrix):
    db.child("neurons").child('activations').set(activation_matrix.tolist())


def finding_similar(db, image_ids, coordinates, distance_scores):
    """
    Args:
        db: firebase db object
        store: firebase storage object
        image_id: list of image ids
        coordinates: list of tuples
        distance_scores: list of floats
    """
    zipped_attributes = zip(image_ids, coordinates, distance_scores)
    dict_attributes = list(map(lambda x: {
        'id': 'image_{}.png'.format(x[0]),
        'coord': {
            'x': str(x[1][0]),
            'y': str(x[1][1]),
            'z': str(x[1][2])
        },
        'distance': x[2]}, zipped_attributes)
    )
    db.child("images_similar_100").child('items').set(dict_attributes)


def update_image_storage(store, idx, image_loc):
    store.child('image_{}.png'.format(idx)).put(image_loc)


"""
search: update params of top 100

training: update activations
"""

# Partial functions
visualize_func = partial(update_neurons, database)
search_func = partial(finding_similar, database)
upload_img_func = partial(update_image_storage, storage)


if __name__ == '__main__':
    # Parameters
    parser = argparse.ArgumentParser(description='AAE worker')
    parser.add_argument('--cuda', default='true', type=str)
    parser.add_argument('--rebuild-tree', default='false', type=str)
    parser.add_argument('--checkpoint-loc', required=True, type=str)
    parser.add_argument('--tree-loc', required=True, type=str)
    parser.add_argument('--h5-loc', required=True, type=str)
    args, unknown = parser.parse_known_args()

    cuda = True if 'true' in args.cuda.lower() else False
    rebuild_tree = True if 'true' in args.rebuild_tree.lower() else False

    # Transformers
    transformers = transforms.Compose([
        transforms.Scale((256, 256)),
        transforms.ToTensor(),
    ])

    # dataset
    train_dataset=datasets.ImageFolder(
        './data/',
        transform=transformers
    )
    train_loader=torch.utils.data.DataLoader(
        dataset=train_dataset, batch_size=1, shuffle=True,
        pin_memory=cuda, num_workers=4
    )

    # Our worker
    worker = aae_worker(z_dim=3, h_dim=128, filter_num=64,
                        channel_num=3, lr=0.001, cuda=cuda)
    worker.load_(args.checkpoint_loc)

    if rebuild_tree:
        worker.build_tree(train_loader, args.tree_loc, args.h5_loc, upload_func=upload_img_func)

    # Load our stuff
    worker.load_tree(args.tree_loc, args.h5_loc)

    # Search example
    # worker.search_similar((0, 0, 0), search_func)

    # Training example
    # for e in range(1):
    #     worker.train(train_loader, e, viz_func=visualize_func)
