package main

import (
	"container/list"
	"math"
)

type Partition struct {
	spheresN []int
}
type Partitioner struct {
	x_min      float32
	size       float32
	count      int
	partitions []Partition
}

func newPartitioner(spheres []Sphere) Partitioner {
	x_min := spheres[0].x
	x_max := spheres[0].x
	r_sum := float32(spheres[0].r)
	for i := 1; i < len(spheres); i++ {
		if spheres[i].x > x_max {
			x_max = spheres[i].x
		}
		if spheres[i].x < x_min {
			x_min = spheres[i].x
		}
		r_sum += spheres[i].r
	}

	avg := float32(r_sum*2) / float32(len(spheres))
	size := avg * 1.1
	count := 1 + int(math.Ceil(float64((x_max-x_min)/size)))
	partitions := make([]Partition, count)
	return Partitioner{x_min, size, count, partitions}

}

func get_value_partition(partitioner Partitioner, value float32) int {
	return int((value - partitioner.x_min) / partitioner.size)
}
func get_sphere_partitions(partitioner Partitioner, sphere Sphere) IntsPair {
	return IntsPair{a: get_value_partition(partitioner, sphere.x-sphere.r), b: get_value_partition(partitioner, sphere.x+sphere.r)}
}

func get_collisions_using_partitions(spheres []Sphere) list.List {
	intersections := list.List{}
	partitioner := newPartitioner(spheres)

	for sphereA_n := 0; sphereA_n < len(spheres); sphereA_n++ {
		// Hash to control wich pairs {a,b} has been added to intersections
		is_B_collided := make(map[int]bool)
		interval := get_sphere_partitions(partitioner, spheres[sphereA_n])
		for partitionN := interval.a; partitionN <= interval.b; partitionN++ {
			spheresN := partitioner.partitions[partitionN].spheresN
			for partition_sphereN_ix := 0; partition_sphereN_ix < len(spheresN); partition_sphereN_ix++ {
				sphereB_n := spheresN[partition_sphereN_ix]
				if intersects(spheres[sphereA_n], spheres[sphereB_n]) && !is_B_collided[sphereB_n] {
					is_B_collided[sphereB_n] = true
					intersections.PushFront(IntsPair{a: sphereA_n, b: sphereB_n})
				}
			}
			partitioner.partitions[partitionN].spheresN = append(spheresN, sphereA_n)
		}
	}
	return intersections
}
