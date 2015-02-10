package main

import (
	"fmt"
	"math/rand"
	"time"
)

// START OMIT
func main() {
	count("robert") // HL
	count("tomas")  // HL
}

func count(name string) {
	for i := 0; i < 10; i++ {
		fmt.Printf("%v: %v\n", name, i)
		time.Sleep(time.Duration(rand.Float64())) * time.Second)
	}
}

// END OMIT
