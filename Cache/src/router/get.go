package router

import (
	"encoding/json"
	"github.com/vivk-FAF-PAD16-1/vivk-PAD-Lab1/src/httpUtilities"
	"log"
	"net/http"
)

func (r Router) get(w http.ResponseWriter, _ *http.Request, db string, model string) {
	data := r.storage.Get(db, model)
	if data == nil {
		httpUtilities.WriteNoContent(w)
		return
	}

	if len(data) == 0 {
		httpUtilities.WriteNoContent(w)
		return
	}

	jsonResp, err := json.Marshal(data)
	if err != nil {
		httpUtilities.WriteInternalServerError(w)
		log.Panic(err)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	_, err = w.Write(jsonResp)

	if err != nil {
		httpUtilities.WriteInternalServerError(w)
		log.Panic(err)
		return
	}
}
