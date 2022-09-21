package main

import (
	"github.com/vivk-FAF-PAD16-1/vivk-PAD-Lab1/src/router"
	"github.com/vivk-FAF-PAD16-1/vivk-PAD-Lab1/src/storage"
	"log"
	"net/http"
)

const StorageLength = 32
const Port = ":40500"

func main() {
	s := storage.New(StorageLength)

	r := router.New(&s)

	http.HandleFunc("/", r.Route)

	log.Println("SERVER prepared!")

	err := http.ListenAndServe(Port, nil)
	if err != nil {
		log.Panic(err)
	}
}
