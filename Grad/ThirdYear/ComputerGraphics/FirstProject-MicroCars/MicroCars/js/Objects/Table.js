class Table extends THREE.Object3D{
	constructor(x=0, y=0, z=0, size=BOARD_WIDTH) {
		super();

		this.type = 'Table';
		this.position.set(x, y, z);

		var halfSize = size >> 1;
		var legDistance = size >> 2;
		var thickness = 5;

		new TableTop(this, 0, -thickness, 0, size, thickness);
		new TableLeg(this, -legDistance, -1, -legDistance, halfSize);
		new TableLeg(this, -legDistance, -1, legDistance, halfSize);
		new TableLeg(this, legDistance, -1, legDistance, halfSize);
		new TableLeg(this, legDistance, -1, -legDistance, halfSize);

		scene.add(this);
	}
}

class TableTop extends THREE.Mesh {
	constructor(obj, x, y, z, size, thickness) {
		var geometry = new THREE.CubeGeometry(size, thickness, size);
		super(geometry);
		this.type = 'TableTop';
		createMaterials(this, {color:0xDEB887});

		this.position.set(x, y, z);
		obj.add(this);
	}
}

class TableLeg extends THREE.Mesh  {
	constructor(obj, x, y, z, size) {
		var geometry = new THREE.CubeGeometry(size/10, size, size/10);
		super(geometry);
		createMaterials(this, {color:0xDEB887});

		this.position.set(x, y - (size >> 1), z);
		obj.add(this);
	}
}
