class CarCustomMesh extends THREE.Mesh {
	constructor(carWidth, carLength, basic=false) {
		// Initializing CarCustomMesh with a CarBody
		super(new CarBody(carWidth, carLength));
		createMaterials(this, {color : 0xff0000});

		// Wheels
		var pneuWidth = 2;
		var geometry = new WheelGeometry(3, pneuWidth, 45);
		var wheels = [];
		for (var i = 0; i < 4; i++) {
			wheels.push(new THREE.Mesh(geometry));
			if (basic) {
				wheels[i].material = new THREE.MeshBasicMaterial({color: 0x001111});
			} else {
				createMaterials(wheels[i], {color: 0x001111});
			}
			this.add(wheels[i]);
		}
		wheels[0].position.set(0, 5, 0);
		wheels[1].position.set(0, 5, carLength + pneuWidth);
		wheels[2].position.set(carWidth, 5, 0);
		wheels[3].position.set(carWidth, 5, carLength + pneuWidth);

		// Bumper
		var bumper = new THREE.Mesh(new BumperGeometry(carLength, carLength/3, carLength/3));
		if (basic) {
			bumper.material = new THREE.MeshBasicMaterial({color: 0xFFFF000});
		} else {
			createMaterials(bumper, {color: 0xFFFF000});
		}
		bumper.rotation.set(0, -Math.PI/2, 0);
		bumper.position.set(0, 5, 5);
		this.add(bumper);

		// All meshes
		for (var i in this.children) {
			this.children[i].updateMatrix();
		}
		this.position.set(0, 0, -2)

		// Adding custom opacity changing method for this crazy mesh.
		this.changeOpacity = function(opacity=1.0) {
			this.traverse(function (opacity, node) {
				if (node instanceof THREE.Mesh) {
					changeOpacity(node, opacity);
				}
			}.bind(this, opacity));
		}.bind(this);
	}
}
