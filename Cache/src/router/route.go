package router

import (
	"github.com/vivk-FAF-PAD16-1/vivk-PAD-Lab1/src/httpUtilities"
	"net/http"
	"strings"
)

func (r Router) Route(w http.ResponseWriter, req *http.Request) {
	requestURI := strings.Trim(req.RequestURI, "/")
	segments := strings.Split(requestURI, "/")

	if len(segments) != 2 {
		httpUtilities.WriteNotFound(w)
		return
	}

	db := segments[0]
	model := segments[1]

	switch req.Method {
	case "POST":
		r.create(w, req, db, model)
	case "GET":
		r.get(w, req, db, model)
	case "DELETE":
		r.delete(w, req, db, model)
	default:
		httpUtilities.WriteNotFound(w)
	}
}
