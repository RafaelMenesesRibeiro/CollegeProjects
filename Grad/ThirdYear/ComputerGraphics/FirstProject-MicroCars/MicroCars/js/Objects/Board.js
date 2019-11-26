class Board extends THREE.Mesh {
	constructor(x=0, y=0, z=0, size=BOARD_WIDTH) {
		var grassTexture;
		var geometry      = new THREE.PlaneGeometry(size, size)
		var textureLoader = new THREE.TextureLoader();
		textureLoader.crossOrigin = 'anonymous'
		grassTexture  = textureLoader.load('https://i.imgur.com/J4sDs4b.jpg')

		super(geometry);
		this.type = 'Board';

		var basicMat = {map:grassTexture, side: THREE.DoubleSide};
		var phongMat = {map:grassTexture, side: THREE.DoubleSide, shininess: 5, specular: new THREE.Color("rgb(5%, 5%, 5%)")};
		var lambertMat = {map:grassTexture, side: THREE.DoubleSide, emissive: 0x002200, emissiveIntensity: 0.5};

		createMaterialsTwo(this, basicMat, phongMat, lambertMat);

		this.position.set(x, y, z);
		this.rotateX(90 * TO_RADIANS);

		scene.add(this);
	}
}
