package item

func (i *Item) Add(data map[string]interface{}) {
	i.data[i.index] = data
	i.index = (i.index + 1) % i.length
	if i.index == 0 {
		i.full = true
	}
}
