class CameraManager {
	constructor() {
		// Main CameraManager data
		this.windowWidth = window.innerWidth;
		this.windowHeight = window.innerHeight;
		this.aspectRatio = this.windowWidth / this.windowHeight;
		this.near = 0.1;
		this.far = 2000;
		this.frustumSize = BOARD_WIDTH;

		// Creating ALL cameras now
		this.cameras = [];
		var camera;

		// Creating debug camera: Orthographic + OrbitControls
		camera = this.createOrthographicCamera("Orbit Camera");
		camera.position.set(0, BOARD_WIDTH, 0);
		camera.lookAt(scene.position);
		controls = new THREE.OrbitControls(camera);
		controls.enableKeys = false;
		this.cameras.push(camera);

		// Creating 1st camera: Orthographic Top
		camera = this.createOrthographicCamera("Top Camera");
		camera.position.set(0, BOARD_WIDTH, 0);
		camera.lookAt(scene.position);
		this.cameras.push(camera);

		// Creating 2nd camera: Perspective World
		camera = this.createPerspectiveCamera("Table Camera");
		camera.position.set(0, 600, 550);
		camera.lookAt(scene.position);
		this.cameras.push(camera);

		// Creating 3rd camera: Perspective Chase
		camera = this.createPerspectiveCamera("Chase Camera");
		this.cameras.push(camera);

		// Final settings for all cameras
		var cameraColor = new THREE.Color(102, 152, 255);
		for (var i in this.cameras) {
			camera = this.cameras[i];
			camera.userData.background = cameraColor;
			camera.userData.alpha = 1;
			camera.userData.view = {
				x: 0, y:0,
				width: 1.0, height: 1.0
			}
			scene.add(camera);
		}

		// Preparing Heads-Up Display
		var hud = this.createOrthographicCamera("HUD", -mapWidth / 2, mapWidth / 2, mapHeight / 2, -mapHeight / 2);
		var cameraX = HALF_BOARD_WIDTH + 550, cameraY = 200, cameraZ = -HALF_BOARD_WIDTH + 90;
		hud.position.set(cameraX, cameraY, cameraZ);
		hud.lookAt(cameraX, 0, cameraZ);
		hud.userData.background = cameraColor;
		hud.userData.alpha = 1;
		hud.userData.view = {
			x: 0, y: 0,
			width: 0.2, height: 0.1
		}
		hud.userData.aspect = mapWidth/mapHeight;

		// Assembling viewports
		this.viewports = {
			camera : this.cameras[1],
			hud    : hud,
		};

		// Final touches
		if (msgBox != undefined) { this.cameras[1].add(msgBox); }
		if (subBox != undefined) { this.cameras[1].add(subBox); }
	}

	createPerspectiveCamera(name="", fov=75) {
		var cam = new THREE.PerspectiveCamera(fov, this.aspectRatio, this.near, this.far);
		cam.name = name;
		return cam;
	}

	createOrthographicCamera(name="", left=0, right=0, top=0, bottom=0) {
		var cam = new THREE.OrthographicCamera(left, right, top, bottom, this.near, this.far);
		cam.name = name;
		return cam;
	}

	attachCameraTo(camera, obj) {
		obj.add(camera);
		camera.position.set(0, 30, 40);
		camera.rotation.set(0, 0, 0);
		camera.lookAt(obj.bounds.position);
	}

	getCurrentCamera() {
		return this.viewports.camera;
	}

	updateValues(w, h, aspect) {
		this.windowWidth = w;
		this.windowHeight = h;
		this.aspectRatio = aspect;
	}

	// Updates
	updateCamera() {
		for (var view in this.viewports) {
			var camera = this.viewports[view];

			if (camera.name == "Top Camera" || camera.name == "Orbit Camera") {
				if (this.windowHeight > this.windowWidth) {
					camera.left   = - this.frustumSize / 2;
					camera.right  =   this.frustumSize / 2;
					camera.top    =   this.frustumSize / this.aspectRatio / 2;
					camera.bottom = - this.frustumSize / this.aspectRatio / 2;
				}
				else {
					camera.left   = - this.frustumSize * this.aspectRatio / 2;
					camera.right  =   this.frustumSize * this.aspectRatio / 2;
					camera.top    =   this.frustumSize / 2;
					camera.bottom = - this.frustumSize / 2;
				}
			}

			else if (camera instanceof THREE.PerspectiveCamera){
				camera.aspect = renderer.getSize().width / renderer.getSize().height;
			}

			else if (camera.name == "HUD") {
				var aspectRatio = this.aspectRatio * 4;
				camera.left   = - mapWidth / 2;
				camera.right  =   mapWidth / 2;
				camera.top    =   mapWidth / aspectRatio;
				camera.bottom = - mapWidth / aspectRatio;
			}

			// Updating projection matrix (absolutely required)
			camera.updateProjectionMatrix();
		}
	}

	update() {
		var toggled = {
			changeToOrthographic      : Input.is_pressed("1"),
			changeToPerspectiveWorld  : Input.is_pressed("2"),
			changeToPerspectiveFollow : Input.is_pressed("3"),
			changeToOrbit             : Input.is_pressed("0"),
		};

		// Iterate our toggled keypresses
		for (var key in toggled) {
			if (toggled[key]) {
				// if the given key was pressed, call the function with key's name
				this[key]();
			}
		}
	}

	changeTo(index) {
		if (0 <= index && index < this.cameras.length) {
			controls.enabled = index == 0;
			this.viewports.camera = this.cameras[index];
			this.updateCamera();
		}

		msgBox.switchCamera(this.viewports.camera);
		subBox.switchCamera(this.viewports.camera);
	}

	changeToOrbit() {
		this.changeTo(0);
	}

	changeToOrthographic() {
		this.changeTo(1);
	}

	changeToPerspectiveWorld() {
		this.changeTo(2);
	}

	changeToPerspectiveFollow() {
		if (!game.is_gameover) {
			this.changeTo(3);
		}
	}
}
