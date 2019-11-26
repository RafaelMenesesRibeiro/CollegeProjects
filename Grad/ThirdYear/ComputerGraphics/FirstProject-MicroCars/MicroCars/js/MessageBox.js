/**
* An object to which we apply a texture.
*/
class MessageBox extends THREE.Mesh {
	constructor(width=1, height=1, x=0, y=0) {
		var geometry = new THREE.PlaneGeometry(width, height);
		super(geometry);

		// Setting initial data
		this.visible = false;
		this.position.set(x, y, -1); // z = -1 to be in front of the camera
		this.userData.ortho = this.position.clone();
		this.userData.persp = this.position.clone();

		// Setting texture map
		this.material = new THREE.MeshBasicMaterial( { side: THREE.DoubleSide } );
		this.textures = {};
	}

	add(path) {
		// FIXME: use remote texture
		return this.textures[path] = LocalTextures.load(path);
	}

	apply(path) {
		// if path was already added, use it; else, load it to memory.
		var texture = path in this.textures ? this.textures[path] : this.add(path);
		this.material.map = texture;
		this.needsUpdate = true;
	}

	switchCamera(camera) {
		var oldCamera = this.parent;
		if (oldCamera != undefined) {
			oldCamera.remove(this);
		}
		camera.add(this);

		if (camera instanceof THREE.PerspectiveCamera) {
			this.scale.set(0.0015, 0.0015, 0.0015); // FIXME: calculate values
			this.position.copy(this.userData.persp);
		} else if (camera instanceof THREE.OrthographicCamera) {
			this.scale.set(1, 1, 1);
			this.position.copy(this.userData.ortho);
		}
	}
}
