import numpy as np

from aae.train import worker, transformers


if __name__ == '__main__':
    worker.load_('./aae/epoch0_aae.path.tar')
    print(worker.encoder.z_dim)
