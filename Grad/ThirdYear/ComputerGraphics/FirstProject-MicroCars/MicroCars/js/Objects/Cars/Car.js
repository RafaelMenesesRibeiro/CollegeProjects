class Car extends MotionBody {
	constructor(x=0, y=0, z=0) {
		super();
		this.type = 'Car';

		// Preparing abstract Car data
		this.direction = 1;
		this.userData.canMoveForward = true;
		this.userData.canMoveBack = true;
		this.userData.colliding = false;

		this.userData.timer = new THREE.Clock(false);
		this.userData.timer_end = 0;

		addHeadLigths(this);

		// Collision function
		this.collide = this.collide.bind(this);

		// Creating mesh
		var carWidth = 20;
		var carLength = 10;
		this.mesh = new CarCustomMesh(carWidth, carLength);
		this.add(this.mesh);

		// Creating Bounds
		/* FIXME: hacked our way through a BoundingSphere for our car */
		var vertices = this.mesh.children[0].geometry.vertices;
		var min = new THREE.Vector3(0, 0, 0);
		var max = new THREE.Vector3(0, 0, 0);
		for (var i = 0; i < vertices.length; i++) {
			var v = vertices[i];
			min.min(v);
			max.max(v);
		}

		var center = max.clone();
		center.sub(min);

		this.bounds = new BoundingSphere(this.mesh, carWidth, center);
		/* FIXME: attempt to remove the code above later */

		this.add(this.bounds);

		// Positioning the car
		this.heading = X_AXIS_HEADING.clone();
		this.acceleration = 0;
		this.position.set(x, y, z);
		this.userData.initialPosition = this.position.clone();

		// Adding to scene graph
		scene.add(this);

		/* Miscellaneous one-time only used functions */
		function addHeadLigths(thisArg) {
			var targetObject = new THREE.Object3D();
			targetObject.position.set(-50, 0, 0);

			var spotLight = new THREE.SpotLight(0xffffff, 5, 200, 35 * TO_RADIANS);
			spotLight.add(targetObject);

			spotLight.position.set(0, 5, 0);
			spotLight.target = spotLight.children[0];
			thisArg.add(spotLight);
			thisArg.headlights1 = spotLight;

			spotLight = spotLight.clone();
			spotLight.position.set(0, 5, 4);
			spotLight.target = spotLight.children[0];
			thisArg.add(spotLight);
			thisArg.headlights2 = spotLight;
		}
	}

	collide(node) {
		if (node == this) { return; }
		if (!this.hasEventListener('collided', this._colliding)) {
			return; // No collision is taking place. Ignoring.
		}

		if (this.intersects(node)) {
			// Fire the main event
			this.dispatchEvent({type: 'collided', body: node});

			// Stop the car if it's a StaticBody
			if (node instanceof StaticBody) {
				var vectorCarToButter = this.getHeading(node);

				/* FIXME: The world direction vector is rotated 90º
				* because it points to the right.
				* I have no idea why this is the case.
				*/
				var direction = this.getWorldDirection();
				var rotatedX = Math.cos(NINETY_DEGREES) * direction.x - Math.sin(NINETY_DEGREES) * direction.z;
				var rotatedZ = Math.sin(NINETY_DEGREES) * direction.x + Math.cos(NINETY_DEGREES) * direction.z;
				var carHeading = new THREE.Vector3(rotatedX, 0, rotatedZ);
				var angleCarButter = carHeading.angleTo(vectorCarToButter) * TO_DEGREES;

				this.velocity = 0;
				this.userData.canMoveForward = angleCarButter >= 90;
				this.userData.canMoveBack = angleCarButter < 90;
			}
			// Respawn the car if it's an Orange
			else if (node instanceof OrangeWrapper) {
				game.playerDied();
				this.respawn();
			}
			else {
				this.userData.canMoveForward = this.userData.canMoveBack = true;
			}
		}
	}

	respawn(position=this.userData.initialPosition, lostLife=true) {
		if (!game.is_gameover) {
			this.position.copy(position);
			this.rotation.set(0, 0, 0);
			this.velocity = 0;

			// Temporarily activates God mode
			this.godmode(lostLife ? 3 : -1);
		} else {
			this.position.set(0, -100, 0); // XXX: just a ridiculous number
		}
	}

	godmode(seconds=0) {
		// Enable/disable collisions depending to the amount of seconds
		this.setColliding(seconds < 0);
		if (seconds > 0) {
			// Changing mesh opacity
			this.mesh.changeOpacity(0.5);

			// Activating our custom timer
			this.timer(seconds, function() {
				this.godmode(-1);
			}.bind(this));
		}
		else if (seconds == 0) {
			// Changing mesh opacity
			this.mesh.changeOpacity(0.5);
		}
		else {
			this.mesh.changeOpacity(1.0);
		}
	}

	timer(seconds, callback=undefined) {
		this.userData.timer_end = seconds;
		this.userData.timer_cb = callback;
		this.userData.timer.start();
	}

	free() {
		queueFree(this);
		car = undefined;
	}

	// Our mandatory update() function
	update(delta) {
		// Handling collisions
		scene.traverseVisible(this.collide);

		// Handling input
		var left  = Input.is_pressed("ArrowLeft");
		var right = Input.is_pressed("ArrowRight");
		var up    = Input.is_pressed("ArrowUp");
		var down  = Input.is_pressed("ArrowDown");

		this.acceleration = 0;
		if (up && !down && this.userData.canMoveForward) {
			this.acceleration = -CAR_ACCELERATION;
		}
		else if (down && !up && this.userData.canMoveBack) {
			this.acceleration = +CAR_ACCELERATION;
		}

		// Updating car motion
		this.velocity += this.acceleration * delta - FRICTION * this.mass * this.velocity;

		// Rotates the mesh
		var angle = 0;
		if (left && !right) {
			angle = -WHEEL_ROTATION;
		}
		if (right && !left) {
			angle = WHEEL_ROTATION;
		}
		if (angle != 0) {
			var sign = this.velocity > 0 ? 1 : -1
			angle *= sign * Math.abs(this.velocity) * TURN_ASSIST;
			this.rotateY(angle);
		}

		// Moving our car
		this.move(this.heading, this.velocity);
		if (objectNeedsRespawn(this)) {
			game.playerDied();
			this.respawn();
		}

		// Handling timer callback
		if (this.userData.timer.running) {
			var elapsed = this.userData.timer.getElapsedTime();
			if (elapsed > this.userData.timer_end) {
				this.userData.timer.stop();
				this.userData.timer_end = 0;
				// Call callback (if available)
				if (this.userData.timer_cb != undefined) {
					this.userData.timer_cb();
				}
			}
		}
	}
}
