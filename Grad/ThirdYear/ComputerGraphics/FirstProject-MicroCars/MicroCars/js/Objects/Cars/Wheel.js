class WheelGeometry extends THREE.Geometry {
	constructor(wheelradius, depth, points=360) {
		super();
		var wcc = new THREE.Mesh(new CenterCapGeometry(wheelradius, depth));
		var wt = new THREE.Mesh(new TireGeometry(wheelradius, depth, points));

		wcc.updateMatrix();
		wt.updateMatrix();

		this.merge(wcc.geometry, wcc.matrix);
		this.merge(wt.geometry, wt.matrix);
	}
}

class TireGeometry extends THREE.Geometry {
	constructor(wheelradius, depth, circlePoints=360) {
		super();
		this.innerRadius = wheelradius;
		this.outerRatio = 1.2;
		this.depth = depth;

		//Create vertices
		var outsideRatio = 1.2;
		var degreeSeparation = THREE_HUNDRED_SIXTY_DEGREES / circlePoints;
		var geometry2 = new THREE.Geometry();
		var geometry3 = new THREE.Geometry();
		var geometry4 = new THREE.Geometry();

		for (var side = 0; side < circlePoints * 2; side+=2) {
			var angle = degreeSeparation * side;
			var angle2 = angle + degreeSeparation;
			var x = Math.cos(angle) * this.innerRadius;
			var y = Math.sin(angle) * this.innerRadius;
			var xNext = Math.cos(angle2) * this.innerRadius;
			var yNext = Math.sin(angle2) * this.innerRadius;

			var pointA1 = {x: x, y: y, z: 0}
			var pointB1 = {x: xNext, y: yNext, z: 0}
			var pointC1 = {x: x, y: y, z: -this.depth}
			var pointD1 = {x: xNext, y: yNext, z: -this.depth}
			var pointA2 = {x:pointA1.x*this.outerRatio, y:pointA1.y*this.outerRatio, z:pointA1.z}
			var pointB2 = {x:pointB1.x*this.outerRatio, y:pointB1.y*this.outerRatio, z:pointB1.z}
			var pointC2 = {x:pointC1.x*this.outerRatio, y:pointC1.y*this.outerRatio, z:pointC1.z}
			var pointD2 = {x:pointD1.x*this.outerRatio, y:pointD1.y*this.outerRatio, z:pointD1.z}

			this.vertices.push(new THREE.Vector3(pointA1.x, pointA1.y, pointA1.z)); //0
			this.vertices.push(new THREE.Vector3(pointC1.x, pointC1.y, pointC1.z)); //1
			this.vertices.push(new THREE.Vector3(pointB1.x, pointB1.y, pointB1.z)); //2
			this.vertices.push(new THREE.Vector3(pointD1.x, pointD1.y, pointD1.z)); //3
			this.faces.push(new THREE.Face3(0 + side, 2 + side, 1 + side));
			this.faces.push(new THREE.Face3(1 + side, 2 + side, 3 + side));

			geometry2.vertices.push(new THREE.Vector3(pointA2.x, pointA2.y, pointA2.z)); //0
			geometry2.vertices.push(new THREE.Vector3(pointC2.x, pointC2.y, pointC2.z)); //1
			geometry2.vertices.push(new THREE.Vector3(pointB2.x, pointB2.y, pointB2.z)); //2
			geometry2.vertices.push(new THREE.Vector3(pointD2.x, pointD2.y, pointD2.z)); //3
			geometry2.faces.push(new THREE.Face3(0 + side, 1 + side, 2 + side));
			geometry2.faces.push(new THREE.Face3(2 + side, 1 + side, 3 + side));

			geometry3.vertices.push(new THREE.Vector3(pointA1.x, pointA1.y, pointA1.z)); //0
			geometry3.vertices.push(new THREE.Vector3(pointB1.x, pointB1.y, pointB1.z)); //1
			geometry3.vertices.push(new THREE.Vector3(pointA2.x, pointA2.y, pointA2.z)); //2
			geometry3.vertices.push(new THREE.Vector3(pointB2.x, pointB2.y, pointB2.z)); //3
			geometry3.faces.push(new THREE.Face3(0 + side, 2 + side, 1 + side));
			geometry3.faces.push(new THREE.Face3(1 + side, 2 + side, 3 + side));

			geometry4.vertices.push(new THREE.Vector3(pointC1.x, pointC1.y, pointC1.z)); //0
			geometry4.vertices.push(new THREE.Vector3(pointD1.x, pointD1.y, pointD1.z)); //1
			geometry4.vertices.push(new THREE.Vector3(pointC2.x, pointC2.y, pointC2.z)); //2
			geometry4.vertices.push(new THREE.Vector3(pointD2.x, pointD2.y, pointD2.z)); //3
			geometry4.faces.push(new THREE.Face3(0 + side, 1 + side, 3 + side));
			geometry4.faces.push(new THREE.Face3(0 + side, 3 + side, 2 + side));

			if (side > 2) {
				geometry3.faces.push(new THREE.Face3(0 + side, -1 + side, 2 + side));
				geometry3.faces.push(new THREE.Face3(0 + side, -3 + side, -1 + side));
				geometry4.faces.push(new THREE.Face3(0 + side, -3 + side, -2 + side));
				geometry4.faces.push(new THREE.Face3(0 + side, -1 + side, -3 + side));
			}
		}

		this.merge(geometry2);
		this.merge(geometry3);
		this.merge(geometry4);

		this.mergeVertices();
		this.computeFaceNormals();
		this.computeVertexNormals();
	}
}

class CenterCapGeometry extends THREE.Geometry {
	constructor(wheelradius, depth) {
		super();
		this.wheelRadius      = wheelradius;
		this.pcr              = wheelradius * 0.25;
		this.bladeLenght      = this.pcr * 0.75;
		this.bladeWidth       = wheelradius * 0.166;
		this.capInnerLenght   = Math.sqrt(this.bladeWidth*this.bladeWidth);

		var hBW = this.bladeWidth / 2;
		var cil = this.capInnerLenght;
		var pcr = this.pcr;
		var hD  = depth / 2;
		var wr  = wheelradius;
		  // Defining the geometry's vertices
		var vertices = [
			// Define wheel's centercap points and spike point
			[-cil, 0  , 0],       // 0
			[ cil, 0  , 0],       // 1
			[ 0  , cil, 0],       // 2
			[ 0  ,-cil, 0],       // 3
			[-cil, 0  , -depth],  // 4
			[ cil, 0  , -depth],  // 5
			[ 0  , cil, -depth],  // 6
			[ 0  ,-cil, -depth],  // 7
			[ 0  , 0  , cil / 2], // 8

			[-hBW, pcr, 0],       // 9   (C)
			[-hBW, pcr, -depth],  // 10  (C')
			[ hBW, pcr, 0],       // 11  (D)
			[ hBW, pcr, -depth],  // 12  (D')

			[ pcr, hBW, 0],       // 13  (E)
			[ pcr, hBW, -depth],  // 14  (E')
			[ pcr,-hBW, 0],       // 15  (F)
			[ pcr,-hBW, -depth],  // 16  (F')

			[ hBW,-pcr, 0],       // 17  (G)
			[ hBW,-pcr, -depth],  // 18  (G')
			[-hBW,-pcr, 0],       // 19  (H)
			[-hBW,-pcr, -depth],  // 20  (H')

			[-pcr,-hBW, 0],       // 21  (I)
			[-pcr,-hBW, -depth],  // 22  (I')
			[-pcr, hBW, 0],       // 23  (J)
			[-pcr, hBW, -depth],   // 24  (J')

			[ 0  , wr ,-hD],       // 25  (K)
			[ wr , 0  ,-hD],       // 26  (L)
			[ 0  ,-wr ,-hD],       // 27  (M)
			[-wr , 0  ,-hD]        // 28  (N)
		];

		// Pushing the vertices in
		for (var i in vertices) {
			var vertex = vertices[i];
			this.vertices.push(new THREE.Vector3(vertex[0], vertex[1], vertex[2]));
		}

		// Defining faces
		var faces = [
			// Define inner center cap front faces, which stand behind the spikes (+z) => (counter-clock wise)
			//[0, 1, 2],        // A - Hidden face
			//[0, 3, 1],        // B - Hidden face
			// Define inner center cap back faces, which have no spikes (-z) => (clock wise)
			[4, 6, 5],        // C
			[4, 5, 7],        // D
			// Define inner center cap left side faces (-x) => (ccw)
			[4, 0, 2],        // E
			[4, 2, 6],        // F
			[4, 7, 3],        // G
			[4, 3, 0],        // H
			// Define inner center cap right side faces (+x) => (ccw)
			[6, 2, 1],        // I
			[6, 1, 5],        // J
			[1, 3, 5],        // K
			[3, 7, 5],         // L
			// Define spike faces, faces are in -x and +x => (ccw)
			[8, 1, 2],        // M
			[8, 3, 1],        // N
			[8, 0, 3],        // O
			[8, 2, 0],         // P
			// Define outer cap triangles
			// Top (Z 1, Z' 2, C 13, C' 14, D 15, D' 16)
			[9, 2, 11],
			[10, 12, 6],
			[2, 12, 11],
			[2, 6, 12],
			[2, 9, 6],
			[9, 10, 6],
			// Right (W 1, W' 5, E 13, E' 14, F 15, F' 16)
			[1, 15, 13],
			[5, 14, 16],
			[1, 14, 5],
			[1, 13, 14],
			[1, 16, 15],
			[1, 5, 16],
			// Bottom (X 3, X' 7, G 17, G' 18, H 19, H' 20)
			[3, 19, 17],
			[7, 18, 20],
			[7, 3, 17],
			[7, 17, 18],
			[3, 7, 20],
			[3, 20, 19],
			// Left (Y 0, Y' 4, I 21, I' 22, J 23, J' 24)
			[0, 23, 21],
			[4, 22, 24],
			[0, 4, 24],
			[0, 24, 23],
			[0, 22, 4],
			[0, 21, 22],
			// Define wheel Blades
			// Top Blade (K 25, C 9, C' 10, D 11, D' 12)
			[9, 11, 25],
			[11, 12, 25],
			[12, 10, 25],
			[10, 9, 25],
			// Right Blade (L 26, E 13, E' 14, F 15, F' 16)
			[13, 15, 26],
			[15, 16, 26],
			[16, 14, 26],
			[14, 13, 26],
			// Bottom Blade (M 27, G 17, G' 18, H 19, H' 20)
			[17, 19, 27],
			[19, 20, 27],
			[20, 18, 27],
			[18, 17, 27],
			// Left Blade (N 28, I 21, I' 22, J 23, J' 24)
			[21, 23, 28],
			[23, 24, 28],
			[24, 22, 28],
			[22, 21, 28]
		];

		// Pushing the faces in
		for (var i in faces) {
			var face = faces[i];
			this.faces.push(new THREE.Face3(face[0], face[1], face[2]));
		}

		this.mergeVertices();
		this.computeFaceNormals();
		this.computeVertexNormals();
	}
}
