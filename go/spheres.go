package main

import (
	"io/ioutil"
	"log"
	"strconv"
	"strings"
	"time"
)

func check(e error) {
	if e != nil {
		panic(e)
	}
}

type Sphere struct {
	x float32
	y float32
	z float32
	r float32
}

func main() {
	spheres := readFile("../data/spheres.dat")
	log.Printf("%d spheres \n", len(spheres))

	start := time.Now()
	for i := 0; i < len(spheres); i++ {
		for j := i + 1; j < len(spheres); j++ {
			if intersects(spheres[i], spheres[j]) {
				log.Printf("%d intersects with %d \n", i, j)
			}
		}
	}

	log.Printf("Process took %s \n", time.Since(start))

}

func readFile(path string) []Sphere {
	data, err := ioutil.ReadFile(path)
	check(err)
	s := strings.Split(string(data), "\r\n")
	spheres_count := len(s) / 4
	spheres := make([]Sphere, spheres_count)

	for i, sphere_i := 0, 0; sphere_i < spheres_count; i, sphere_i = i+4, sphere_i+1 {

		x, err := strconv.ParseFloat(s[i], 32)
		check(err)
		y, err := strconv.ParseFloat(s[i+1], 32)
		check(err)
		z, err := strconv.ParseFloat(s[i+2], 32)
		check(err)
		r, err := strconv.ParseFloat(s[i+3], 32)
		check(err)
		spheres[sphere_i] = Sphere{x: float32(x), y: float32(y), z: float32(z), r: float32(r)}
	}
	return spheres
}

func intersects(a Sphere, b Sphere) bool {
	var dx = a.x - b.x
	var dy = a.y - b.y
	var dz = a.z - b.z
	var sr = a.r + b.r
	return dx*dx+dy*dy+dz*dz < sr*sr
}
