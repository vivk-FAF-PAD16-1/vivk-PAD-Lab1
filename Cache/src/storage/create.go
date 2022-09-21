package storage

import "github.com/vivk-FAF-PAD16-1/vivk-PAD-Lab1/src/storage/item"

func (s *Storage) Create(db string, model string, data map[string]interface{}) {
	if s.data[db] == nil {
		s.data[db] = make(map[string]*item.Item)
	}

	if s.data[db][model] == nil {
		i := item.New(s.length)
		s.data[db][model] = &i
	}

	s.data[db][model].Add(data)
}
