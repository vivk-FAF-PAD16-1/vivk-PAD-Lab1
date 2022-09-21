package router

import (
	"net/http"
)

func (r Router) delete(_ http.ResponseWriter, _ *http.Request, db string, model string) {
	r.storage.Delete(db, model)
}
