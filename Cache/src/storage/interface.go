package storage

type IStorage interface {
	Get(db string, model string) []map[string]interface{}
	Create(db string, model string, data map[string]interface{})
	Delete(db string, model string)
}
