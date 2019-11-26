class Game {
	constructor(numberOfLives=5, boardSize=BOARD_WIDTH) {
		// Setting pause value
		this.is_paused = false;
		this.is_gameover = false;

		// Defining immutable maximum values
		Object.defineProperty(this, "maximumLives", {
			value: numberOfLives-1, // XXX: show off one fewer life, and yet still living it
			enumerable: true,
			configurable: false,
		});

		// Setting the maximum amount of Car meshes as lives, while putting
		// them invisible first
		var carWidth = 20;
		var carLength = 10;
		var pos = {x:boardSize/2 + 500, y:0, z:-boardSize/2 + 100}

		this.liveReps = []
		for (var i = 0; i < this.maximumLives; i++) {
			var mesh = new CarCustomMesh(carWidth, carLength, true);
			mesh.position.set(pos.x + 30*i, pos.y, pos.z);
			mesh.rotation.set(0, NINETY_DEGREES, 0);
			mesh.visible = false;
			mesh.material = mesh.userData.basicMaterial;
			delete mesh.userData.previousMaterial;

			this.liveReps.push(mesh);
			scene.add(mesh);
		}

		// Showing off our lives
		this.numberOfLives = 0;
		this.resetLives(numberOfLives);

		// Adding main MessageBox
		msgBox = new MessageBox(512, 128);
		msgBox.add('text_pause.png');
		msgBox.add('text_gameover.png');
		// Adding subBox
		subBox = new MessageBox(256, 64, 0, -100);
		subBox.add('text_start.png');
		subBox.userData.persp.setY(-0.15); // FIXME: calculate appropriate value
	}

	limitNumber(number) {
		return Math.min(this.maximumLives, Math.max(0, number));
	}

	/** @function resetLives
	** @param number: the exact number of lives to show
	*/
	resetLives(number) {
		number = this.limitNumber(this.numberOfLives + number);
		for (var i = 0; i < this.liveReps.length; i++) {
			this.liveReps[i].visible = i < number;
		}
		this.numberOfLives = number;
	}

	/** @function addLives
	** @param amount: the amount of lives to add upon our current numberOfLives
	*/
	addLives(amount) {
		this.resetLives(amount);
	}

	/** @function removeLives
	** @param amount: the amount of lives to remove upon our current numberOfLives
	*/
	removeLives(amount) {
		this.resetLives(-amount);
	}

	/** @function restart
	*/
	restart() {
		msgBox.visible = false;
		this.is_gameover = false;
		this.is_paused = false;
		this.resetLives(this.maximumLives);

		msgBox.visible = subBox.visible = false;

		reloadScene();
	}

	getCurrentLives() { return this.numberOfLives; }
	getMaxLives() { return this.maximumLives; }

	playerDied() {
		if (this.gameOver()) { return true; }
		this.removeLives(1);
	}

	gameOver() {
		if (this.numberOfLives <= 0) {
			cameraManager.changeToOrthographic();
			this.is_gameover = true;
			this.togglePause();

			msgBox.apply('text_gameover.png');
			subBox.apply('text_start.png');
			msgBox.visible = subBox.visible = true;

			return true;
		}
		return false;
	}

	togglePause() {
		msgBox.apply('text_pause.png');
		this.is_paused = !this.is_paused;
		msgBox.visible = this.is_paused;
	}
}
