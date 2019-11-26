class CarMesh extends THREE.Group {
	constructor(x=0, y=0, z=0) {
		super();

		/* Creates the body panels */
		//Extrude setting that define the car width.
		var extrSettings = {amount:8, bevelEnabled:false};

		// Main mesh to merge all CarSquare and CarTriangle meshes
		var square = new THREE.Mesh(new THREE.Geometry());
		createMaterials(square, { color:0x2194ce, /*Blue*/ specular: 0x111111, shininess: 10 });
		this.add(square);

		var c1 = new CarSquare(square, extrSettings,  3, 5,  0, 0);
		var c2 = new CarSquare(square, extrSettings,  8, 3,  3, 2);
		var c3 = new CarSquare(square, extrSettings,  7, 2, 11, 0);
		var c4 = new CarSquare(square, extrSettings, 12, 6, 11, 2);

		var t1 = new CarTriangle(square, extrSettings,  -2, 2.5, 0,  0, 2.5, 0,  0,   5, 0);
		var t2 = new CarTriangle(square, extrSettings,  -2, 2.5, 0,  0,   0, 0,  0, 2.5, 0);
		var t3 = new CarTriangle(square, extrSettings,   3,   0, 0,  4,   2, 0,  3,   2, 0);
		var t4 = new CarTriangle(square, extrSettings,   9,   2, 0, 11,   0, 0, 11,   2, 0);
		var t5 = new CarTriangle(square, extrSettings, 8.5,   5, 0, 11,   5, 0, 11,   8, 0);
		var t6 = new CarTriangle(square, extrSettings,  18,   0, 0, 19,   2, 0, 18,   2, 0);
		var t7 = new CarTriangle(square, extrSettings,  22,   2, 0, 23,   0, 0, 23,   2, 0);
		var t8 = new CarTriangle(square, extrSettings,  23,   0, 0, 24,   5, 0, 23,   5, 0);
		var t9 = new CarTriangle(square, extrSettings,  23,   5, 0, 24,   5, 0, 23,   8, 0);

		/* Creates the wheels */
		// Contrary to the others, these MUST be separated (to some degree)
		var rfw = new CarTorus(this, 2.5, 1.7, 10, 16,  5.5, -1, -2);
		var lfw = new CarTorus(this, 2.5, 1.7, 10, 16,  5.5, -1, 10);
		var rrw = new CarTorus(this, 2.5, 1.7, 10, 16, 20.5, -1, -2);
		var lrw = new CarTorus(this, 2.5, 1.7, 10, 16, 20.5, -1, 10);

		/* Creates the axles */
		//Creates the material for the axles.
		var cylinders = new THREE.Mesh(new THREE.Geometry());
		createMaterials(cylinders, {color:0x960101});
		this.add(cylinders);

		var cy1 = new CarCylinder(cylinders, 0.5, 12, 8, 1,  5.5,  -1, 4, 90, 0,  0);
		var cy2 = new CarCylinder(cylinders, 0.5, 12, 8, 1, 20.5,  -1, 4, 90, 0,  0);
		var cy3 = new CarCylinder(cylinders, 0.5,  3, 8, 1,  5.5, 0.5, 4,  0, 0,  0);
		var cy4 = new CarCylinder(cylinders, 0.5,  3, 8, 1, 20.5, 0.5, 4,  0, 0,  0);
		var cy5 = new CarCylinder(cylinders, 0.5, 15, 8, 1,   13,  -1, 4,  0, 0, 90);

		//Scales the car.
		this.scale.set(0.5, 0.5, 0.5);
	}

	changeOpacity (opacity=1.0) {
		this.traverse(function (opacity, node) {
			if (node instanceof THREE.Mesh) {
				changeOpacity(node, opacity);
			}
		}.bind(this, opacity));
	}
}

class CarTriangle {
	constructor(obj, extrSettings, x1, y1, z1, x2, y2, z2, x3, y3, z3) {
		//Creates the side panel's geometry - triangle.
		var geometry = new THREE.Shape()
		geometry.moveTo(x1, y1)
		geometry.lineTo(x2, y2)
		geometry.lineTo(x3, y3)
		geometry.lineTo(x1, y1)
		//Extrudes the side panel to create volume.
		var geometry = new THREE.ExtrudeGeometry(geometry, extrSettings)
		//Creates the side panel's mesh.
		var mesh = new THREE.Mesh(geometry)
		obj.geometry.mergeMesh(mesh)
		return mesh
	}
}
class CarSquare {
	constructor(obj, extrSettings, cubeL1, cubeL2, x, y) {
		//Creates the side panel's geometry - square.
		var geometry = new THREE.Shape()
		geometry.moveTo(x, y)
		geometry.lineTo(x, y + cubeL2)
		geometry.lineTo(x + cubeL1, y + cubeL2)
		geometry.lineTo(x + cubeL1, y)
		geometry.lineTo(x, y)
		//Extrudes the side panel to create volume.
		var extrudeGeometry = new THREE.ExtrudeGeometry(geometry, extrSettings)
		//Creates the side panel's mesh.
		var mesh = new THREE.Mesh(extrudeGeometry)
		obj.geometry.mergeMesh(mesh)
		return mesh
	}
}
class CarTorus {
	constructor(obj, radius, tube, rSeg, tSeg, x, y, z) {
		//Creates the wheel's geometry - torus.
		var geometry = new THREE.TorusGeometry(radius, tube, rSeg, tSeg)
		//Creates the wheel's mesh.
		var mesh = new THREE.Mesh(geometry);
		createMaterials(mesh, {color:0x222222});
		//Positions the wheel.
		mesh.position.set(x, y, z)
		obj.add(mesh)
		return mesh
	}
}
class CarCylinder {
	constructor(obj, radius, h, rSeg, hSeg, x, y, z, rotx, roty, rotz) {
		//Creates the axel's geometry - cylinder.
		var geometry = new THREE.CylinderGeometry(radius, radius, h, rSeg, hSeg)
		//Creates the axel's mesh.
		var mesh = new THREE.Mesh(geometry)
		//Rotates the axle.
		mesh.rotation.set(rotx * TO_RADIANS, roty * TO_RADIANS, rotz * TO_RADIANS)
		//Positions the axle.
		mesh.position.set(x, y, z)
		obj.geometry.mergeMesh(mesh)
		return mesh
	}
}
