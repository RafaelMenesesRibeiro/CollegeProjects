class Track extends THREE.Object3D {
	constructor(trackWidth) {
		super()
		this.type = 'Track'
		this.lights = new Lamp(this);

		//Sets the position of the curves that compose the track.
		var points = [
			{x:0, z:0}, {x:200, z:0},
			{x:280, z:80}, {x:800, z:80},
			{x:800, z:400}, {x:400, z:800},
			{x:200, z:800}, {x:240, z:600},
			{x:400, z:480}, {x:400, z:200},
			{x:140, z:200}, {x:140, z:540},
			{x:100, z:800}, {x:0, z:800},
			{x:0, z:600}, {x:80, z:480},
			{x:0, z:400}, {x:0, z:200},
			//{x:0, z:0},
		]
		//Offsets the points to be alligned with the camera and converts them
		//to be THREE.Vector3 to be used by THREE.CatmullRomCurve3.
		points = this.pointsOffset(points, 400);
		//Draws the track.
		var track = this.trackCreate(this, points, trackWidth, 0x13294B);
		//Adds the tori on the track.
		this.torusGroup = new THREE.Group();
		this.add(this.torusGroup);
		this.addTorus(this.torusGroup, track.geometry.vertices);
		//Adds all to the scene.
		scene.add(this);
	}

	pointsOffset(points, offset) {
		var p2 = []
		for (var i = 0; i < points.length; i++) {
			var p = points[i]
			p2.push(new THREE.Vector3(p.x - offset, 0, p.z - offset))
		}
		return p2
	}

	trackCreate(obj, points, width, color) {
		//Creates a Curve to be the shape of the track defined by the points.
		var closedSpline = new THREE.CatmullRomCurve3(points)
		closedSpline.type = 'catmullrom'
		closedSpline.closed = true
		width /= 2
		var shape = new THREE.Shape([
			new THREE.Vector2(0, -width),
			new THREE.Vector2(0,  width),
			new THREE.Vector2(1, 0),
		])
		//Extrusion settings.
		//Steps represents the smoothness of the corners.
		//ExtrudePath extrudes along the curve defined above. This is what gives
		//the width to the track (instead of being a line).
		var extrudeSettings = {steps:200, bevelEnabled:false, extrudePath:closedSpline}
		//Creates the track geometry.
		var geometry = new THREE.ExtrudeGeometry(shape, extrudeSettings)
		//Creates the track material.
		var mesh = new THREE.Mesh(geometry)
		createMaterials(mesh, {color:color});
		//Adds the mesh to the track class object.
		obj.add(mesh)
		return mesh
	}

	addTorus(obj, vertices) {
		//Creates the torus material.
		var basicMaterial   = new THREE.MeshBasicMaterial({color:0xAA1111});
		var lambertMaterial = new THREE.MeshLambertMaterial({color:0xAA1111});
		var phongMaterial   = new THREE.MeshPhongMaterial({color:0xAA1111});
		//Step of the for loop
		var step = 2
		//Adds the tires to the track's sides.
		for (var i = 0; i < vertices.length; i += step) {
			var tire = new Tire(obj, vertices[i]);
			tire.userData.initialPosition = vertices[i].clone();
			addMaterials(tire.mesh, basicMaterial, lambertMaterial, phongMaterial);
		}
	}

	resetTorus() {
		var tori = this.torusGroup.children;
		for (var i in tori) {
			var torus = tori[i];
			torus.reset();
		}
	}
}

class Tire extends RigidBody {
	constructor(obj, p=new THREE.Vector3()) {
		super(7)
		this.type = 'Tire'

		//Creates the torus geometry.
		var radius = 2.5
		var tube = 0.8
		var geometry = new THREE.TorusBufferGeometry(radius, tube, 5, 16)

		//Creates the mesh
		this.mesh = new THREE.Mesh(geometry);
		this.add(this.mesh)

		//Positions the torus to be on the track point.
		this.position.copy(p)
		//Rotates the torus to be horizontal.
		this.rotation.set(NINETY_DEGREES, 0, 0)

		// Adding BoundingSphere
		this.bounds = new BoundingSphere(this.mesh, radius+tube)
		this.add(this.bounds)

		//Adds the mesh to the track class object.
		obj.add(this)
		return this
	}

	reset() {
		this.position.copy(this.userData.initialPosition);
		this.velocity = 0;
	}
}
