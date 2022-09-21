package router

import "net/http"

type IRouter interface {
	Route(w http.ResponseWriter, req *http.Request)
}
