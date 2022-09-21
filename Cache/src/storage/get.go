package storage

func (s *Storage) Get(db string, model string) []map[string]interface{} {
	if s.data[db] == nil {
		return nil
	}

	i, ok := s.data[db][model]
	if !ok {
		return nil
	}

	result := i.Get()
	return result
}
