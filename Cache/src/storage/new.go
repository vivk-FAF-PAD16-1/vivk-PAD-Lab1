package storage

import "github.com/vivk-FAF-PAD16-1/vivk-PAD-Lab1/src/storage/item"

func New(length int8) Storage {
	return Storage{
		data: make(map[string]map[string]*item.Item),

		length: length,
	}
}
