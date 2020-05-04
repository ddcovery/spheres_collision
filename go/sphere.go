package main

type Sphere struct {
	x float32
	y float32
	z float32
	r float32
}

func intersects(a Sphere, b Sphere) bool {
	var dx = a.x - b.x
	var dy = a.y - b.y
	var dz = a.z - b.z
	var sr = a.r + b.r
	return dx*dx+dy*dy+dz*dz < sr*sr
}
