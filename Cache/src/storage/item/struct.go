package item

type Item struct {
	data []map[string]interface{}

	length int8
	index  int8

	full bool
}
