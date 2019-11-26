/*******************************************************************************
* Car variables
*******************************************************************************/
const CAR_ACCELERATION = 2;
const MAX_ORANGE_VELOCITY = 200;
const ORANGE_VELOCITY = 20;
const ORANGE_ACCELERATION = 2;
const TURN_ASSIST = CAR_ACCELERATION / 32;
const WHEEL_ROTATION = Math.PI / 8;
/*******************************************************************************
* Board variables
*******************************************************************************/
const BOARD_WIDTH = 1000;
const BOARD_LENGTH = 1000;
const HALF_BOARD_WIDTH  = BOARD_WIDTH  >> 1;
const HALF_BOARD_LENGTH = BOARD_LENGTH >> 1;
const QUARTER_BOARD_WIDTH  = BOARD_WIDTH  >> 2;
const QUARTER_BOARD_LENGTH = BOARD_LENGTH >> 2;
const FRICTION = 0.02;
/*******************************************************************************
* Directional variables
*******************************************************************************/
const X_AXIS_HEADING = new THREE.Vector3(1, 0, 0);
const Y_AXIS_HEADING = new THREE.Vector3(0, 1, 0);
const Z_AXIS_HEADING = new THREE.Vector3(0, 0, 1);
const MX_AXIS_HEADING = new THREE.Vector3(-1, 0, 0);
const MZ_AXIS_HEADING = new THREE.Vector3(0, 0, -1);
const XPZP_AXIS_HEADING = new THREE.Vector3(1, 0, 1).normalize();
const XPZM_AXIS_HEADING = new THREE.Vector3(-1, 0, -1).normalize();
const XMZP_AXIS_HEADING = new THREE.Vector3(-1, 0, 1).normalize();
const XMZM_AXIS_HEADING = new THREE.Vector3(-1, 0, 1).normalize();
const HEADING_ARRAY = [ X_AXIS_HEADING, Z_AXIS_HEADING,
                        MX_AXIS_HEADING, MZ_AXIS_HEADING,
                        XPZP_AXIS_HEADING, XPZM_AXIS_HEADING,
                        XMZP_AXIS_HEADING, XMZM_AXIS_HEADING ];
/*******************************************************************************
* Trignometric variables
*******************************************************************************/
const TO_DEGREES = 180 / Math.PI;
const TO_RADIANS = Math.PI / 180;
const NINETY_DEGREES = Math.PI / 2;
const THREE_HUNDRED_SIXTY_DEGREES = 2 * Math.PI;
/*******************************************************************************
* Lighting variables
*******************************************************************************/
const NUMBER_OF_POINT_LIGHTS = 6;
const POINT_LIGHT_INTENSITY = 2;
const POINT_LIGHT_DISTANCE = 1000;
const POINT_LIGHT_REAL = 2;

/*******************************************************************************
* Textures
*******************************************************************************/
var LocalTextures = new THREE.TextureLoader();
LocalTextures.setPath('textures/');
var RemoteTextures = new THREE.TextureLoader();
RemoteTextures.crossOrigin = 'anonymous';

/*******************************************************************************
* Helper methods
*******************************************************************************/

/**
* @method createMaterials Creates the appropriate materials for a given mesh
* @param mesh: The mesh to associate the materials with
* @param parameters: THREE.Material parameters
*/
function createMaterials(mesh, parameters={}) {
	parameters.transparent = true;
	addMaterials(mesh,
		new THREE.MeshBasicMaterial(parameters),
		new THREE.MeshLambertMaterial(parameters),
		new THREE.MeshPhongMaterial(parameters),
	);
}

/**
* @method createMaterials Creates the appropriate materials for a given mesh
* @param mesh: The mesh to associate the materials with
* @param parameters: THREE.Material parameters distinguished by inheritance
*/
function createMaterialsTwo(mesh, basicParam={}, phongParam={}, lambertParam={}) {
	basicParam.transparent   = true;
	phongParam.transparent   = true;
	lambertParam.transparent = true;
	addMaterials(mesh,
		new THREE.MeshBasicMaterial(basicParam),
		new THREE.MeshLambertMaterial(lambertParam),
		new THREE.MeshPhongMaterial(phongParam),
	);
}

/**
* @method addMaterials Adds the given materials to a given mesh
* @param mesh: The mesh to associate the materials with
* @param basicMaterial:
* @param lambertMaterial:
* @param phongMaterial:
*/
function addMaterials(mesh, basicMaterial, lambertMaterial, phongMaterial) {
	mesh.material                  = phongMaterial;
	mesh.userData.previousMaterial = phongMaterial;
	mesh.userData.basicMaterial    = basicMaterial;
	mesh.userData.lambertMaterial  = lambertMaterial;
	mesh.userData.phongMaterial    = phongMaterial;
}

/**
* @method isMultiMaterial Checks if given mesh is multi-material
* @param mesh: The mesh to examine
*/
function isMultiMaterial(mesh) {
	return mesh instanceof THREE.Mesh &&
		!(mesh instanceof BoundingGeometry) &&
		mesh.userData.hasOwnProperty("previousMaterial");
}

function changeOpacity(mesh, opacity=1.0) {
	mesh.material.opacity                  = opacity;
	mesh.userData.previousMaterial.opacity = opacity;
	mesh.userData.basicMaterial.opacity    = opacity;
	mesh.userData.lambertMaterial.opacity  = opacity;
	mesh.userData.phongMaterial.opacity    = opacity;
}

function toggleWireframe(mesh) {
	var value = !mesh.material.wireframe;
	mesh.material.wireframe                  = value;
	mesh.userData.previousMaterial.wireframe = value;
	mesh.userData.basicMaterial.wireframe    = value;
	mesh.userData.lambertMaterial.wireframe  = value;
	mesh.userData.phongMaterial.wireframe    = value;
}

/**
* objectNeedsRespawn verifies if an object is within the boundaries of the board
* @param x: x position of the object subject to verification.
* @param z: z position of the object subject to verification.
* @return: Boolean value True if orange is outside of the board. False otherwise
*/
function objectNeedsRespawn(obj) {
	var x = obj.getWorldPosition().x;
	var z = obj.getWorldPosition().z;
	return x <= -HALF_BOARD_WIDTH  || x >= HALF_BOARD_WIDTH
		|| z <= -HALF_BOARD_LENGTH || z >= HALF_BOARD_LENGTH;
}

/** generateSpawnLocation(min, max)
@param min: used to calculate a random coordinate in inverval [min, max]
@param max: used to calculate a random coordinate in inverval [min, max]
@var x: X coordinate (value calculated randomly based on min-max values)
@var y: Y coordiante (default is 0 - orange stay on top of the board at all times)
@var z: Z coordinate (value calculated randomly based on min-max values)
@return: Vector3D that defines a new spawn location after orange falls from the table.
*/
function generateSpawnLocation(min = -HALF_BOARD_WIDTH, max = HALF_BOARD_WIDTH) {
	var x = Math.floor(Math.random() * (max - min + 1)) + min;
	var z = Math.floor(Math.random() * (max - min + 1)) + min;
	return new THREE.Vector3(x, 0, z);
}

/** respawnObject(spawnLocation, axis, distance)
* @param obj: object that is being respawned
*/
function respawnObject(obj, timed=true) {
	obj.visible = false;

	var position = generateSpawnLocation();
	position.setY(obj.bounds.radius);

	var rand = Math.random();
	var x = rand < 0.5 ? -1 * rand : 1 * rand;
	rand = Math.random();
	var z = rand >= 0.5 ? -1 * rand : 1 * rand;

	var heading = new THREE.Vector3(x, 0, z);

	setTimeout(function() {
		obj.position.copy(position);
		obj.mesh.rotation.set(0, 0, 0);
		obj.heading = heading.normalize();
		obj.visible = true;
	}, timed ? 2000 : 0);

}

/*	Safe Freeing solution
** scene.remove() usually goes wrong if it's called before a render() call if
** the object that is being removed is too heavy.
** Therefore, this queue of objects to be freed *after* a render() has taken place
** is a solution to that problem.
*/
var to_delete = [];

/** @function queueFree
* @param obj: object to put to a queue
*/
function queueFree(obj) {
	obj.visible = false; // Putting it invisible for it not to be drawn in the next frame
	if (to_delete.indexOf(obj) == -1) {
		to_delete.push(obj);
	}
}

function cleanQueue() {
	var length = to_delete.length;
	for (var i = 0; i < length; i++) {
		var obj = to_delete.pop();
		scene.remove(obj);
	}
}
