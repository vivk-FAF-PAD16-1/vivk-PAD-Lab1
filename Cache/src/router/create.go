package router

import (
	"encoding/json"
	"github.com/vivk-FAF-PAD16-1/vivk-PAD-Lab1/src/httpUtilities"
	"io"
	"log"
	"net/http"
)

func (r Router) create(w http.ResponseWriter, req *http.Request, db string, model string) {
	if req.ContentLength == 0 {
		return
	}

	defer func(Body io.ReadCloser) {
		err := Body.Close()
		if err != nil {
			httpUtilities.WriteInternalServerError(w)
			log.Panic(err)
			return
		}
	}(req.Body)

	body, err := io.ReadAll(req.Body)
	if err != nil {
		httpUtilities.WriteInternalServerError(w)
		log.Panic(err)
		return
	}

	var data []map[string]interface{}
	err = json.Unmarshal(body, &data)
	if err != nil {
		httpUtilities.WriteInternalServerError(w)
		log.Panic(err)
		return
	}

	ok := data != nil && len(data) > 0
	if !ok {
		return
	}

	r.storage.Create(db, model, data)
}
