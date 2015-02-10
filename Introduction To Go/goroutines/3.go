package main

import (
	"fmt"
	"math/rand"
	"time"
)

// START OMIT
func main() {
	go count("robert") // HL
	go count("tomas")  // HL

	time.Sleep(10 * time.Second)
}

func count(name string) {
	for i := 0; i < 10; i++ {
		fmt.Printf("%v: %v\n", name, i)
        time.Sleep(time.Duration(rand.Float64())) * time.Second)
	}
}

// END OMIT
