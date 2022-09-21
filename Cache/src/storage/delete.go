package storage

func (s *Storage) Delete(db string, model string) {
	if s.data[db] == nil {
		return
	}

	if s.data[db][model] == nil {
		return
	}

	delete(s.data[db], model)
}
