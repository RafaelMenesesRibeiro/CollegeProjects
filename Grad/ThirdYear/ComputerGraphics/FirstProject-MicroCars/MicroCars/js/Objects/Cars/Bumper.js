class TriangularPrismGeometry extends THREE.Geometry {
	/**
	* Creates an Extruded Triangle (Prism is more accurate)
	* of a given width, height and depth
	* This borrows logic from Cylinder construction, only modified to build a Prism.
	*/
	constructor(width=1, height=1, depth=1, thetaStart=0, xOrig=0, yOrig=0, zOrig=0) {
		super();
		var halfDepth = depth / 2;

		var radialSegments = 3; // it's a triangle, after all
		var heightSegments = 1; // we don't need more segments to extrude our triangle
		var thetaLength = 6.3;  // necessary for accurate triangulation
		thetaStart = thetaStart > thetaLength ? thetaLength : thetaStart;

		this.radius = Math.sqrt(width*width + height*height);

		var x, y;

		// Creating vertices
		var indexes = [];
		for (y = 0; y <= heightSegments; y++) {
			var indexRow = [];
			var v = y / heightSegments;

			for (x = 0; x <= radialSegments; x++) {
				var u = x / radialSegments;
				var theta = u * thetaLength + thetaStart;
				var sinTheta = Math.sin(theta);
				var cosTheta = Math.cos(theta);

				var vertex = new THREE.Vector3();
				vertex.x = xOrig + this.radius * sinTheta;
				vertex.y = yOrig - v * depth + halfDepth;
				vertex.z = zOrig + this.radius * cosTheta;
				this.vertices.push(vertex);

				indexRow.push(this.vertices.length-1);
			}

			indexes.push(indexRow);
		}

		// Creating side faces
		for (y = 0; y < heightSegments; y++) {
			for (x = 0; x < radialSegments; x++) {
				var a = indexes[y][x];
				var b = indexes[y + 1][x];
				var c = indexes[y + 1][x + 1];
				var d = indexes[y][x + 1];

				this.faces.push(new THREE.Face3(a, b, d));
				this.faces.push(new THREE.Face3(b, c, d));
			}
		}

		// XXX: Get ready for some old-fashioned hard-code
		var a, b, c;
		// top face
		a = indexes[0][0];
		b = indexes[0][1];
		c = indexes[0][2];
		this.faces.push(new THREE.Face3(a, b, c));

		// bottom face
		a = indexes[1][0];
		b = indexes[1][1];
		c = indexes[1][2];
		this.faces.push(new THREE.Face3(c, b, a));

		// Update our Geometry
		this.mergeVertices();
		this.computeFaceNormals();
		this.computeVertexNormals();
	}

	rotateToVertical() {
		this.rotateY(125 * TO_RADIANS);
		this.rotateX(90 * TO_RADIANS);
		this.rotateY(90 * TO_RADIANS);
		this.rotateX(30 * TO_RADIANS);
	}
}

class BoxGeometry extends THREE.Geometry {
	constructor(width, height, depth, xOrig=0, yOrig=0, zOrig=0) {
		super();

		// Arguments to send to createPlane()
		var attributes = [
			[ 'x', 'y', 'z', -1, -1, width, height, -depth  ], // ABCD - front
			[ 'x', 'y', 'z',  1, -1, width, height,  depth  ], // BHGC - back
			[ 'x', 'z', 'y',  1,  1, width,  depth,  height ], // AEHB - top
			[ 'x', 'z', 'y',  1, -1, width,  depth, -height ], // DMGC - bottom
			[ 'z', 'y', 'x',  1, -1, depth, height, -width  ], // AEFD - side left
			[ 'z', 'y', 'x', -1, -1, depth, height,  width  ], // EGHM - side right
		];

		for (var i in attributes) {
			/* Creating planes with the arguments defined above + (x, y, z) */
			this.createPlane.apply(this, attributes[i].concat([xOrig, yOrig, zOrig]));
		}

		// Update our Geometry
		this.mergeVertices();
		this.computeFaceNormals();
		this.computeVertexNormals();
	}

	/**
	* @method createPlane: Creates a plane of a given width, height and depth
	* @argument u:
	* @argument v:
	* @argument w:
	* @argument udir:
	* @argument vdir:
	*/
	createPlane(u, v, w, udir, vdir, width, height, depth, xOrig=0, yOrig=0, zOrig=0) {
		var halfWidth  = width / 2;
		var halfHeight = height / 2;
		var halfDepth  = depth / 2;

		// Creating vertices
		for (var iy = 0; iy < 2; iy++) { // 2 = number of height segments
			var y = iy * height - halfHeight;

			for (var ix = 0; ix < 2; ix++) { // 2 = number of width segments
				var x = ix * width - halfWidth;

				var vertex = new THREE.Vector3();
				/* Defining box's dimensions */
				vertex[u] = x * udir;
				vertex[v] = y * vdir;
				vertex[w] = halfDepth;

				/* Defining box's position */
				vertex.x += xOrig;
				vertex.y += yOrig;
				vertex.z += zOrig;
				this.vertices.push(vertex);
			}
		}

		// Creating faces
		var numVertices = this.vertices.length;
		var a = numVertices - 1;
		var d = numVertices - 2;
		var b = numVertices - 3;
		var c = numVertices - 4;

		this.faces.push(new THREE.Face3(a, b, d));
		this.faces.push(new THREE.Face3(b, c, d));
	}
}

class BumperGeometry extends THREE.Geometry {
	constructor(width=1, height=1, depth=1) {
		super();
		this.type = 'BumperGeometry';

		/* The objective of these custom faces is to interpolate between vertices. */
		var halfWidth = width / 2;
		var halfHeight = height / 2;
		var halfDepth = depth / 2;
		var quarterDepth = depth / 4;

		// Creating boxes
		var boxes = [];
		boxes.push(new BoxGeometry(width, height, quarterDepth)); // main body
		boxes.push(new BoxGeometry(width*0.05, height, depth, -halfWidth, 0, -quarterDepth)); // left wing
		boxes.push(new BoxGeometry(width*0.05, height, depth,  halfWidth, 0, -quarterDepth)); // right wing
		for (var i in boxes) {
			this.merge(boxes[i]);
		}

		// Adding triangular prisms
		var triprism = new TriangularPrismGeometry(quarterDepth, halfHeight, halfDepth);
		triprism.rotateToVertical();
		var prismPositions = [-(halfWidth - halfDepth), 0, halfWidth - halfDepth];
		for (var i = 0; i < 3; i++) {
			var prism = triprism.clone();
			prism.translate(prismPositions[i], 0, quarterDepth);
			this.merge(prism);
		}

		// Update our Geometry
		this.mergeVertices();
		this.computeFaceNormals();
		this.computeVertexNormals();
	}
}
