class LightManager {
	constructor() {
		this.lightsNeedUpdate = true;
		this.directionalLight = this.newDirectionalLight();
		this.ambientLight = this.newAmbientLight();
		this.horizonLight = this.newHorizonLight();
	}

	/**
	* A light that uses parallel rays allowing to simulate sunlight, it is the only
	* light used here that allows shadow casting.
	*/
	newDirectionalLight() {
		var directionalLight = new THREE.DirectionalLight(0xFFFFE0);
		var frustumSize = cameraManager.frustumSize;
		var halfFrustum = frustumSize / 2;

		directionalLight.name = 'sunLight';
		directionalLight.position.set(0, 500, 0);
		directionalLight.castShadow = true;

		directionalLight.shadow.mapSize.width = HALF_BOARD_WIDTH;
		directionalLight.shadow.mapSize.height = HALF_BOARD_LENGTH;
		directionalLight.shadow.camera.near = 1;
		directionalLight.shadow.camera.far = frustumSize;
		directionalLight.shadow.camera.left = - halfFrustum;
		directionalLight.shadow.camera.right = halfFrustum;
		directionalLight.shadow.camera.top = halfFrustum;
		directionalLight.shadow.camera.bottom = halfFrustum;

		// scene.add(new THREE.CameraHelper(this.directionalLight.shadow.camera));
		scene.add(directionalLight);

		return directionalLight;
	}

	/**
	* A light source positioned directly above the scene, color fades from the sky
	* towards the ground. Simulates horizon.
	*/
	newHorizonLight() {
		var horizonLight = new THREE.HemisphereLight(0xffffff, 0xffffff, 0.35);

		horizonLight.name = 'horizonLight';
		horizonLight.position.set( 0, cameraManager.frustumSize, 0 );

		scene.add(horizonLight);

	return horizonLight;
	}

	newAmbientLight() {
		var ambientLight = new THREE.AmbientLight(0x404040);
		ambientLight.name = 'ambientLight';

		scene.add(ambientLight);
		return ambientLight;
	}

	update() {
		// While lightsNeedUpdate == false, we shall ignore any of our relevant input
		// until one of them has been released
		if (!this.lightsNeedUpdate) {
			this.lightsNeedUpdate = !(
				Input.is_pressed("c") || Input.is_pressed("g") ||
				Input.is_pressed("h") || Input.is_pressed("l") ||
				Input.is_pressed("n")
			);
			return;
		}

		var toggled = {
			switchPointLights      : Input.is_pressed("c"),
			switchMaterials        : Input.is_pressed("g"),
			switchHeadlights       : Input.is_pressed("h"),
			disableLightUpdates    : Input.is_pressed("l"),
			switchDirectionalLight : Input.is_pressed("n"),
		};

		// Iterate our toggled keypresses
		for (var key in toggled) {
			if (toggled[key]) {
				this.lightsNeedUpdate = false;
				// if the given key was pressed, call the function with key's name
				this[key]();
			}
		}
	}

	switchDirectionalLight() {
		this.directionalLight.visible = !this.directionalLight.visible;
		this.ambientLight.visible = !this.ambientLight.visible;
		this.horizonLight.visible = !this.horizonLight.visible;
	}

	disableLightUpdates() {
		scene.traverse(function (node) {
			if (isMultiMaterial(node)) {
				if (!(node.material instanceof THREE.MeshBasicMaterial)) {
					node.userData.previousMaterial = node.material;
					node.material = node.userData.basicMaterial;
				} else {
					node.material = node.userData.previousMaterial;
				}
			}
		});
	}

	switchPointLights() {
		var lights = raceTrack.lights.getLightsArray();
		for (var i in lights) {
			var light = lights[i];
			light.intensity = (light.intensity == 0) ? POINT_LIGHT_INTENSITY : 0;
		}
		raceTrack.lights.lampsOn();
	}

	switchHeadlights() {
		// FIXME: FIX THIS CODE
		if (car.headlights1.power != 0) {
			car.headlights1.power = car.headlights2.power = 0;
		} else {
			car.headlights1.power = car.headlights2.power = 5 * Math.PI;
		}
	}

	switchMaterials() {
		scene.traverse(function (node) {
			if (isMultiMaterial(node)) {
				if (node.material instanceof THREE.MeshLambertMaterial) {
					node.userData.previousMaterial = node.material;
					node.material = node.userData.phongMaterial;
				} else if (node.material instanceof THREE.MeshPhongMaterial) {
					node.userData.previousMaterial = node.material;
					node.material = node.userData.lambertMaterial;
				} else {
					node.material = node.userData.previousMaterial;
				}
			}
		});
	}

}
