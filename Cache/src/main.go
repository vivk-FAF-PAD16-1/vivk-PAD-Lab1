package main

import (
	"log"
	"net/http"
)

const PORT = ":40500"

func indexRoute(w http.ResponseWriter, req *http.Request) {
	log.Println(req.Host)

	_, err := w.Write([]byte(req.Host))
	if err != nil {
		log.Panic(err)
		return
	}
}

func main() {
	http.HandleFunc("/", indexRoute)

	log.Println("Prepare server!")

	err := http.ListenAndServe(PORT, nil)
	if err != nil {
		log.Panic(err)
	}

}
