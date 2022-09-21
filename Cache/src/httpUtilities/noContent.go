package httpUtilities

import (
	"net/http"
)

func WriteNoContent(w http.ResponseWriter) {
	w.WriteHeader(http.StatusNoContent)
	return
}
