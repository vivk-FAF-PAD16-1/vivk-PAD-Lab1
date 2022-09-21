package storage

import "github.com/vivk-FAF-PAD16-1/vivk-PAD-Lab1/src/storage/item"

type Storage struct {
	data map[string]map[string]*item.Item

	length int8
}
