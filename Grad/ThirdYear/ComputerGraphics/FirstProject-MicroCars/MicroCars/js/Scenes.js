/*******************************************************************************
* Scene related - Helper methods
*******************************************************************************/
var scene;

/**
* Instanciates the Scene with Axis & Grid helpers to assist devs with coordinates.
* Generates the board, representing the game world where other Oject3D objects are held.
*	Instanciates and adds edibles to the track which debuff the player when touched
* Object3D of subtype Car to a dictionary
*/
function createScene() {
	scene = new THREE.Scene();
	scene.background = new THREE.Color(0xAEEEEE);
	scene.add(new THREE.AxesHelper(HALF_BOARD_WIDTH));

	var size = BOARD_WIDTH;
	var divisions = BOARD_WIDTH / 10;
	// The X axis is red, Y is green and Z is blue.
	var table = new Table(0, -1.1, 0, size);
	gameBoard = new Board(0, -1, 0, size);
	raceTrack = new Track(45);

	// Adding oranges
	createEdible(OrangeWrapper, "Orange1", 0, 0, 0);
	createEdible(OrangeWrapper, "Orange2", 0, 0, 0);
	createEdible(OrangeWrapper, "Orange3", 0, 0, 0);
	// Adding apples. I mean, butters.
	createEdible(Butter, "Butter1", 100, 0, -100);
	createEdible(Butter, "Butter2", -100, 0, -100);
	createEdible(Butter, "Butter3", 50, 0, 70);
	createEdible(Butter, "Butter4", 150, 0, 90);
	createEdible(Butter, "Butter5", -50, 0, 70);
}

function reloadScene() {
	// Setting Car
	if (car == undefined) {
		car = new Car(100, 0, -325);
		var camera = cameraManager.cameras[3];
		cameraManager.attachCameraTo(camera, car);
		camera.position.set(40, 30, 2); // FIXME: fix Car's position, THEN come back here and remove this line
		camera.lookAt(car.bounds.position); // FIXME
	}
	else {
		car.respawn(new THREE.Vector3(100, 0, -325), false);
	}

	// Setting Track
	raceTrack.resetTorus();

	// Setting Oranges
	scene.traverse(function(orange) {
		if (orange instanceof OrangeWrapper) {
			orange.reset();
		}
	});
}
