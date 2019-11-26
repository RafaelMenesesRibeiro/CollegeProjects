/*******************************************************************************
* EventHandler - and some Input too
*******************************************************************************/

/**
*	InputServer
* A singleton (FIXME: not yet, but HEY, it works like this) accessed via Input.
* Creates scoping for Input, to allow more strict OOP and allow less clutter code
* all-in-one file.
*
* Check keyName values at:
* https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent/key/Key_Values
*/
class InputServer {
	constructor() {
		this.pressed = [];
		this.keys = {};
		for (var i = 0; i < 256; i++) {
			this.pressed[i] = false;
		}

		Object.seal(this);
	}

	is_key_pressed(keyCode) {
		return this.pressed[keyCode];
	}

	is_pressed(keyName) {
		return this.keys[keyName] != undefined;
	}

	is_echo(keyName) {
		return this.keys[keyName].repeat;
	}

	press(e) {
		this.pressed[e.keyCode] = true;
		this.keys[e.key] = e;
	}

	release(e) {
		this.pressed[e.keyCode] = false;
		delete this.keys[e.key];
	}
}
const Input = new InputServer();

/**
*	Keypresses Events (keydown, keyup, keypress)
*/
function onKeyDown(e) {
	// Setting global input (if any)
	switch(e.keyCode) {
		case 65: // A
		case 97: // a
			scene.traverse(function (node) {
				if (isMultiMaterial(node)) {
					toggleWireframe(node);
				}
			});
			break;

		case 73:  // I
		case 105: // i
			// TODO: toggle info window
			var help = document.getElementById("help");
		    if (help.style.display === "none") {
		        help.style.display = "block";
		    } else {
		        help.style.display = "none";
		    }
			break;

		case 81:  // Q
		case 113: // q
			scene.traverse(function (node) {
				if (node instanceof BoundingSphere) {
					node.toggleVisibility();
				}
			});
			break;

		case 82:  // R
		case 114: // r
			game.restart();
			break;

		case 83:  // S
		case 115: // s
			if (!game.is_gameover) {
				game.togglePause();
			}
			break;
	}
}

function onKeyUp(e) {
	// Setting global input (if any)
}

/**
*	Other EventHandlers
*/
function onWindowResize() {
	var windowWidth = window.innerWidth;
	var windowHeight = window.innerHeight;
	var aspectRatio = windowWidth / windowHeight;
	if (windowWidth > 0 && windowHeight > 0) {
		renderer.setSize(windowWidth, windowHeight);
		renderer.setViewport (0, 0, windowWidth, windowHeight);
	} else {
		console.log("Error on window resize. Negative size values were detected.");
		return -1;
	}
	cameraManager.updateValues(windowWidth, windowHeight, aspectRatio);
	cameraManager.updateCamera();
}
