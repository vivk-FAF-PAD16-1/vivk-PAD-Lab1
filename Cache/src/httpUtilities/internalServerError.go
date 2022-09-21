package httpUtilities

import (
	"encoding/json"
	"log"
	"net/http"
)

func WriteInternalServerError(w http.ResponseWriter) {
	w.WriteHeader(http.StatusInternalServerError)
	w.Header().Set("Content-Type", "application/json")

	resp := make(map[string]string)
	resp["message"] = "500 Internal Server Error"

	jsonResp, err := json.Marshal(resp)
	if err != nil {
		log.Panic(err)
	}

	_, err = w.Write(jsonResp)
	if err != nil {
		log.Panic(err)
	}

	return
}
