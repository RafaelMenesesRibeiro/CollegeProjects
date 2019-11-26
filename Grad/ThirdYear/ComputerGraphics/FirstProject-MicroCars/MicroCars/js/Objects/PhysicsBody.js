/*******************************************************************************
*	PhysicsBody classes
* Place where all types of bodies will reside for further implementation
*******************************************************************************/

// Abstract class for physics bodies.
class PhysicsBody extends THREE.Object3D {
	constructor() {
		super();
		this.type = "PhysicsBody";
		Object.defineProperty(this, "isPhysicsBody", { value: true });

		this.bounds = undefined;
		this.heading = new THREE.Vector3();
	}

	/* Method for collision calculation and sharing. Called via EventDispatcher */
	_colliding(event) { /* do nothing */ }

	/* Enables/disables collision events */
	setColliding(value) {
		if (value) {
			this.addEventListener('collided', this._colliding);
		} else {
			this.removeEventListener('collided', this._colliding);
		}
	}

	// Callback for every frame
	update(delta) { /* do nothing */ }

	/**
	* @method getHeading: Gets the Vector direction where Point A points to Point B.
	* @param b: Object3D of point B
	* @param n: given Vector3 to fill data in. If undefined, function will return a new one
	*/
	getHeading(b, vec=undefined) {
		var n = vec || new THREE.Vector3();
		n.copy(b.getWorldPosition());
		n.sub(this.getWorldPosition());
		n.normalize();
		return n;
	}

	// Checks for intersection via a formula
	intersects(body) {
		if (!(body instanceof PhysicsBody)) { return false; }
		if (this.bounds == undefined || body.bounds == undefined) {
			return false;
		}

		return this.bounds.intersects(body.bounds)
	}
}


// Complately static, non-moving objects. Useful for Butters
class StaticBody extends PhysicsBody {
	constructor() {
		super();
		this.type = "StaticBody";
		Object.defineProperty(this, "isStaticBody", { value: true });
		this.matrixAutoUpdate = false; // Object is static, no update is necessary
	}
}


// Weighted, non-deformable bodies. Useful for Oranges
class RigidBody extends PhysicsBody {
	constructor(mass = 1) {
		super();
		this.type = "RigidBody";
		Object.defineProperty(this, "isRigidBody", { value: true });
		this.mass = mass;
		this.velocity = 0;

		// Creating event for collision
		this.addEventListener('collided', this._colliding);
		this.collide = this.collide.bind(this);
	}

	/* Method for collision calculation and sharing. Called via EventDispatcher */
	_colliding(event) {
		var v = event.body.heading;
		// if the colliding body is a StaticBody, bounce against it
		if (event.body.isStaticBody) {
			this.getHeading(event.body, v);
			v.set(v.x, v.z, 0);
			this.velocity = -this.velocity;
		}
		// if the colliding body is a RigidBody, share velocity with this one
		else if (event.body.isRigidBody) {
			this.getHeading(event.body, v);
			v.set(v.x, v.z, 0);
			event.body.velocity = Math.abs(this.velocity);
		}
	}

	collide(node) {
		if (node == this) { return; }
		if (!node.isPhysicsBody) { return; }
		if (node.isMotionBody) { return; }

		if (this.intersects(node)) {
			this.dispatchEvent({type: 'collided', body: node});
		}
	}

	update(delta) {
		this.velocity -= FRICTION * this.mass * this.velocity;
		this.translateOnAxis(this.heading, this.velocity);

		if (this.velocity > 0.01) {
			scene.traverseVisible(this.collide);
		}
	}
}


// Bodies that move and can be animated. Useful for Cars.
class MotionBody extends PhysicsBody {
	constructor(mass = 1) {
		super();
		this.type = "MotionBody";
		Object.defineProperty(this, "isMotionBody", { value: true });
		this.mass = mass;
		this.velocity = 0;

		// Creating event for collision
		this.setColliding(true);
	}

	/* Method for collision calculation and sharing. Called via EventDispatcher */
	_colliding(event) {
		// if the colliding body is a RigidBody, share velocity with this one
		if (event.body.isRigidBody) {
			var v = event.body.heading;
			this.getHeading(event.body, v);
			v.set(v.x, v.z, 0);
			event.body.velocity = Math.abs(this.velocity);
		}
	}

	/**
	* @method move: Translate object according to previously calculated or collision values.
	* @param axis: representing the direction of movement_direction
	* @param distance: how far should the body travel
	*/
	move(axis, distance) {
		this.translateOnAxis(axis, distance);
	}
}
