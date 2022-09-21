package router

import "github.com/vivk-FAF-PAD16-1/vivk-PAD-Lab1/src/storage"

func New(storage storage.IStorage) Router {
	return Router{
		storage: storage,
	}
}
