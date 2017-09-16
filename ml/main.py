import numpy as np

from aae.train import trainer, transformers


if __name__ == '__main__':
    trainer.load('./checkpoints/best_aae.path.tar')
