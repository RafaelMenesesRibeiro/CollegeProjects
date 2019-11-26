class TrapezoidGeometry extends THREE.Geometry {
	constructor(widthTop=1, widthBottom=1, height=1, depth=1, xOrig=0, yOrig=0, zOrig=0) {
		super();
		var halfHeight = height / 2;
		var halfDepth = depth / 2;

		var radialSegments = 4; // a 2D trapezoid has 4 sides
		var heightSegments = 1; // we don't need more segments to extrude our triangle
		var thetaLength = Math.PI * 2;  // necessary for accurate triangulation
		var thetaStart = Math.PI/4; // Rotating our geometry so its sides are aligned with XZ axis

		var radiusTop = widthBottom;
		var radiusBottom = widthTop;

		var x, y;

		// Creating vertices
		var indexes = [];
		for (y = 0; y <= heightSegments; y++) {
			var indexRow = [];
			var v = y / heightSegments;
			var radius = v * (radiusBottom - radiusTop) + radiusTop;

			for (x = 0; x <= radialSegments; x++) {
				var u = x / radialSegments;
				var theta = u * thetaLength + thetaStart;
				var sinTheta = Math.sin(theta);
				var cosTheta = Math.cos(theta);

				var vertex = new THREE.Vector3();
				vertex.x = xOrig + radius * sinTheta;
				vertex.y = yOrig - v * height + halfHeight;
				vertex.z = zOrig + radiusTop * cosTheta; // FIXME: check this value
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

		// Creating top and bottom faces
		// XXX: Brace yourselves for some hard code
		for (y = 0; y <= heightSegments; y++) {
			var a, b, c, d;
		 	if (y==0) {
				a = indexes[y][0];
				b = indexes[y][1];
				c = indexes[y][2];
				d = indexes[y][3];
			} else {
				a = indexes[y][4];
				b = indexes[y][3];
				c = indexes[y][2];
				d = indexes[y][1];
			}

			this.faces.push(new THREE.Face3(a, b, d));
			this.faces.push(new THREE.Face3(b, c, d));
		}

		// Update our Geometry
		this.mergeVertices();
		this.computeFaceNormals();
		this.computeVertexNormals();
	}
}

class RoofGeometry extends THREE.Geometry {
	constructor(width=1, height=1, depth=1) {
		super();

		// Dimensions
		var baseHeight = height * 0.8;
		var smallHeight = height - baseHeight;
		var smallWidth = width * 0.2;

		// Base body
		var baseTrapezoid = new TrapezoidGeometry(width, width*0.8, baseHeight, depth);
		var upperLeftTrapezoid = new TrapezoidGeometry(smallWidth, smallWidth*0.8, smallHeight, depth);
		upperLeftTrapezoid.translate(0, baseHeight, 0);
		/*
		//upperLeftTrapezoid.clone();
		*/
		// Merging
		this.merge(baseTrapezoid);
		this.merge(upperLeftTrapezoid);

		// Update our Geometry
		this.mergeVertices();
		this.computeFaceNormals();
		this.computeVertexNormals();
	}
}
