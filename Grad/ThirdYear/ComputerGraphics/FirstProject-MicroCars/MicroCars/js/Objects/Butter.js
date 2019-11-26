class Butter extends StaticBody {
	constructor(name=undefined, x=0, y=0, z=0, angle=undefined) {
		super();
		var size = new THREE.Vector3(7, 10, 20);

		this.type = "Butter";
		this.name = name || "Butter" + this.id;
		this.mesh = new ButterBox(this, size);

		// Places it in a given position
		this.position.set(x, y+size.y, z);
		this.rotateY(angle || Math.random() * 360 * TO_RADIANS);
		this.scale.set(2, 2, 2);

		// Adding our Bounds
		this.bounds = new BoundingSphere(this.mesh);
		this.add(this.bounds);

		scene.add(this);

		this.updateMatrix(); // Necessary *once* since this is a StaticBody
	}
}

class ButterBox extends THREE.Mesh {
	constructor(obj, size) {
		var geometry = new THREE.BoxGeometry(size.x, size.y, size.z);

		super(geometry);
		var tex = RemoteTextures.load('https://i.imgur.com/4jH2zMv.png');
		createMaterials(this, { map: tex });

		obj.add(this);
	}
}
