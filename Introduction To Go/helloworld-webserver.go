package main

import (
	"fmt"
	"net/http"
)

func main() {
	http.HandleFunc("/", func(res http.ResponseWriter, req *http.Request) {
		res.Write([]byte("Hello World!"))
	})

	if err := http.ListenAndServe(":8080", nil); err != nil {
		fmt.Printf("fatal: %v", err)
	}
}
