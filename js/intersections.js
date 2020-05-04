const fs = require('fs');

const spheres = readSpheres("../data/spheres.dat");

withLogger("Partitioning", logger => {
  detectUsingPartitions(logger, spheres);
});

console.log("\n");

withLogger("Brute force", logger => {
  for (var n = 0; n < spheres.length; n++) {
    for (var j = n + 1; j < spheres.length; j++) {
      if (intersects(spheres[n], spheres[j])) {
        logger(spheres[n], spheres[j]);
      }
    }
  }
});

function withLogger(title, fx, times = 1) {

  let start = new Date();
  console.log(title);
  let lines = [];
  try {
    for (let ix = 0; ix < times; ix++) {
      let n = 0;

      fx(function logIntersection(a, b) {
        lines.push(`${++n} Sphere ${a.n} collides with ${b.n}`);
      });

    }
  } finally {
    let lapsed = new Date() - start;
    let median = lapsed / times;
    lines.forEach(line => console.log(line));
    console.log({ lapsed, times, median });
  }
}


function readSpheres(path) {
  const data = fs.readFileSync(path, 'utf8').split("\n").map(s => parseFloat(s));
  const spheres_length = Math.floor(data.length / 4);
  const spheres = Array(spheres_length);
  for (var sphereN = 0, ix = 0; sphereN < spheres_length; sphereN++, ix += 4) {
    spheres[sphereN] = {
      // Sphere Identifier
      n: sphereN,
      // Position
      x: data[ix],
      y: data[ix + 1],
      z: data[ix + 2],
      // radius
      r: data[ix + 3],
    };
  }
  return spheres;
}

function detectUsingPartitions(logger, spheres) {
  const partitioner = newPartitioner(spheres);
  const partitions = [...Array(partitioner.getCount())].map(() => ([]));

  spheres.forEach(sphere =>
    detectAndInsert(sphere)
  );

  function detectAndInsert(sphere) {
    let intersected = new Map();
    let { min: minPartIdx, max: maxPartIdx } = partitioner.getSpherePartitionsInterval(sphere);
    for (var partIdx = minPartIdx; partIdx <= maxPartIdx; partIdx++) {
      let partition = partitions[partIdx];
      for (var ix = 0; ix < partition.length; ix++) {
        if (intersects(partition[ix], sphere)) {
          // If the same sphere is present in more than 1 partition, we detect the same intersection repetidely.  The Map is used to avoid duplicates.
          intersected.set(partition[ix].n, partition[ix]);
        }
      }
      // After checking, we add the sphere to the partition 
      partition.push(sphere);
    }
    if (intersected.size !== 0) {
      for (const collided of intersected.values()) {
        logger(sphere, collided);
      }
    }
  };
  function newPartitioner(spheres) {
    const { MIN, N_PARTS, PART_SIZE } = calculatePartitionerParams(spheres);
    return {
      getCount: () => N_PARTS,
      getSpherePartitionsInterval: (sphere) => ({
        min: value2partitionNumber(sphere.x - sphere.r),
        max: value2partitionNumber(sphere.x + sphere.r)
      })
    };
    function calculatePartitionerParams(spheres) {
      let min = spheres[0].x - spheres[0].r;
      let max = spheres[0].x + spheres[0].r;
      let r = spheres[0].r;
      for (ix = 1; ix < spheres.length; ix++) {
        let left = spheres[ix].x - spheres[ix].r;
        if (left < min) {
          min = left;
        }
        let right = spheres[ix].x + spheres[ix].r;
        if (right > max) {
          max = right;
        }
        if (spheres[ix].r > r) {
          r = spheres[ix].r;
        }
      }
      // Bigger sphere width (r*2) is used as partition size.
      let
        N_PARTS = Math.round((max - min) / (r * 2)),
        PART_SIZE = Math.round((max - min) / N_PARTS);
      return {
        MIN: min,
        N_PARTS,
        PART_SIZE
      };
    }
    function value2partitionNumber(x) {
      return Math.floor((x - MIN) / PART_SIZE);
    }
  }
}

function intersects(a, b) {

  const
    d_x = a.x - b.x,
    d_y = a.y - b.y,
    d_z = a.z - b.z,
    s_r = a.r + b.r;

  return d_x * d_x + d_y * d_y + d_z * d_z < s_r * s_r;

}


