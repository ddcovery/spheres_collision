package main

import (
	"container/list"
)

func get_collisions_brute_force(spheres []Sphere) list.List {
	intersections := list.List{}

	for i := 0; i < len(spheres); i++ {
		for j := i + 1; j < len(spheres); j++ {
			if intersects(spheres[i], spheres[j]) {
				intersections.PushFront(IntsPair{a: i, b: j})
			}
		}
	}
	return intersections
}
