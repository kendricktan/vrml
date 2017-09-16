import os
import sys
import argparse
import torch

sys.path.append(os.path.dirname(__file__))

import torchvision.datasets as datasets
import torchvision.transforms as transforms

from worker import aae_worker


# Params
parser = argparse.ArgumentParser(description='AAE worker')
parser.add_argument('--epoch', default=500, type=int)
parser.add_argument('--lr', default=0.001, type=float)
parser.add_argument('--cuda', default='true', type=str)
parser.add_argument('--resume', default='', type=str)
args, unknown = parser.parse_known_args()

cuda = True if 'true' in args.cuda.lower() else False
# cuda = True

transformers = transforms.Compose([
    transforms.Scale((256, 256)),
    transforms.ToTensor(),
])

# Gan trainer
worker = aae_worker(z_dim=3, h_dim=128, filter_num=64, channel_num=3, lr=args.lr, cuda=cuda)

if __name__ == '__main__':
    if args.resume:
        worker.load_(args.resume)

    # dataset
    train_dataset = datasets.ImageFolder(
        '../data/',
        transform=transformers
    )
    train_loader = torch.utils.data.DataLoader(
        dataset=train_dataset, batch_size=8, shuffle=True,
        pin_memory=cuda, num_workers=4
    )

    for e in range(worker.start_epoch, args.epoch):
        worker.train(train_loader, e)
        worker.save_(e)
