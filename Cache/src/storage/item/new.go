package item

func New(length int8) Item {
	return Item{
		data: make([]map[string]interface{}, length),

		length: length,
		index:  0,

		full: false,
	}
}
