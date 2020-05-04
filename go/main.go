package main

import (
	"container/list"
	"fmt"
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

type collisions_detector func([]Sphere) list.List

func main() {
	spheres := readFile("../data/spheres.dat")
	log.Printf("%d spheres \n", len(spheres))
	log.Printf("\n")
	logAlgorithm("Brute force", get_collisions_brute_force, spheres)
	log.Printf("\n")
	logAlgorithm("Partitioning", get_collisions_using_partitions, spheres)

}

func logAlgorithm(title string, fn collisions_detector, spheres []Sphere) {

	log.Printf("%s\n", title)
	start := time.Now()
	intersections := fn(spheres)
	tend := time.Since(start)

	res := ""
	for e := intersections.Front(); e != nil; e = e.Next() {
		pair := e.Value.(IntsPair)
		res += fmt.Sprintf("{%d,%d}", pair.a, pair.b)
	}
	log.Printf("%s", res)
	log.Printf("Intersections %d\n", intersections.Len())
	log.Printf("Process took %s \n", tend)
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
