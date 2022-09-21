package item

func (i *Item) Get() []map[string]interface{} {
	if !i.full {
		return i.data[0:i.index]
	}

	return i.data
}
