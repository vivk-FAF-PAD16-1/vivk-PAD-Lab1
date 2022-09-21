package httpUtilities

import (
	"encoding/json"
	"log"
	"net/http"
)

func WriteNotFound(w http.ResponseWriter) {
	w.WriteHeader(http.StatusNotFound)
	w.Header().Set("Content-Type", "application/json")

	resp := make(map[string]string)
	resp["message"] = "404 Not Found"

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
