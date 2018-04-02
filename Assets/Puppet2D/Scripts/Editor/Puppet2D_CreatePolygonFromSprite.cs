using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Puppet2D_Poly2Tri;
//using Poly2Tri.Triangulation;
//using Poly2Tri.Triangulation.Delaunay;
//using Poly2Tri.Triangulation.Delaunay.Sweep;
//using Poly2Tri.Triangulation.Polygon;
using System.Linq;
using System.IO;

namespace Puppet2D
{
	public class Puppet2D_CreatePolygonFromSprite : Editor
	{

		private GameObject MeshedSprite;
		private MeshFilter mf;
		private MeshRenderer mr;
		private Mesh mesh;
		public Sprite mysprite;

		private Vector3[] finalVertices = { };
		private int[] finalTriangles = { };
		private Vector2[] finalUvs = { };
		private Vector3[] finalNormals = { };

		List<Vector3> results = new List<Vector3>();
		List<int> resultsTriIndexes = new List<int>();
		List<int> resultsTriIndexesReversed = new List<int>();
		List<Vector2> uvs = new List<Vector2>();
		List<Vector3> normals = new List<Vector3>();


		//public bool ReverseNormals;

		public GameObject Run(Transform transform, bool ReverseNormals, int triangleIndex)
		{
			Vector3 CurrentScale = transform.localScale;
			transform.localScale = Vector3.one;

			PolygonCollider2D polygonCollider = transform.GetComponent<PolygonCollider2D>();

			//for(int path =0;path<polygonCollider.pathCount;path++)
			//{
			int path = 0;
			bool overwrite = false;
			MeshedSprite = new GameObject();
			Undo.RegisterCreatedObjectUndo(MeshedSprite, "Created Mesh");
			mf = MeshedSprite.AddComponent<MeshFilter>();
			mr = MeshedSprite.AddComponent<MeshRenderer>();
			mesh = new Mesh();

			if (!Directory.Exists(Puppet2D_Editor._puppet2DPath + "/Models"))
				Directory.CreateDirectory(Puppet2D_Editor._puppet2DPath + "/Models");

			if (AssetDatabase.LoadAssetAtPath(Puppet2D_Editor._puppet2DPath + "/Models/" + transform.name + "_MESH.asset", typeof(Mesh)))
			{
				if (EditorUtility.DisplayDialog("Overwrite Asset?", "Do you want to overwrite the current Mesh & Material?", "Yes, Overwrite", "No, Create New Mesh & Material"))
				{
					//mf.mesh = AssetDatabase.LoadAssetAtPath(Puppet2D_Editor._puppet2DPath+"/Models/"+transform.name+"_MESH.asset",typeof(Mesh))as Mesh;

					string meshPath = (Puppet2D_Editor._puppet2DPath + "/Models/" + transform.name + "_MESH.asset");
					AssetDatabase.CreateAsset(mesh, meshPath);
					overwrite = true;
				}
				else
				{
					string meshPath = AssetDatabase.GenerateUniqueAssetPath(Puppet2D_Editor._puppet2DPath + "/Models/" + transform.name + "_MESH.asset");
					AssetDatabase.CreateAsset(mesh, meshPath);
				}
			}
			else
			{
				string meshPath = AssetDatabase.GenerateUniqueAssetPath(Puppet2D_Editor._puppet2DPath + "/Models/" + transform.name + "_MESH.asset");
				AssetDatabase.CreateAsset(mesh, meshPath);
			}

			Vector2[] vertsToCopy = polygonCollider.GetPath(path);

			CreateMesh(vertsToCopy, transform, triangleIndex);


			mesh.vertices = finalVertices;
			mesh.uv = finalUvs;
			mesh.normals = finalNormals;
			mesh.triangles = finalTriangles;
			mesh.RecalculateBounds();
			mesh = calculateMeshTangents(mesh);


			mf.mesh = mesh;

			results.Clear();
			resultsTriIndexes.Clear();
			resultsTriIndexesReversed.Clear();
			uvs.Clear();
			normals.Clear();

			if (!Directory.Exists(Puppet2D_Editor._puppet2DPath + "/Models/Materials"))
				Directory.CreateDirectory(Puppet2D_Editor._puppet2DPath + "/Models/Materials");
			if (overwrite)
			{
				mr.material = AssetDatabase.LoadAssetAtPath(Puppet2D_Editor._puppet2DPath + "/Models/Materials/" + transform.name + "_MAT.mat", typeof(Material)) as Material;
			}
			else
			{

				Material newMat = new Material(Shader.Find("Unlit/Transparent"));
				string materialPath = AssetDatabase.GenerateUniqueAssetPath(Puppet2D_Editor._puppet2DPath + "/Models/Materials/" + transform.name + "_MAT.mat");
				AssetDatabase.CreateAsset(newMat, materialPath);
				mr.material = newMat;
			}

			MeshedSprite.transform.localScale = CurrentScale;


			return MeshedSprite;

		}

		public void CreateMesh(Vector2[] vertsToCopy, Transform transform, int triangleIndex)
		{
			List<Vector3> resultsLocal = new List<Vector3>();
			List<int> resultsTriIndexesLocal = new List<int>();
			List<int> resultsTriIndexesReversedLocal = new List<int>();
			List<Vector2> uvsLocal = new List<Vector2>();
			List<Vector3> normalsLocal = new List<Vector3>();


			Sprite spr = transform.GetComponent<SpriteRenderer>().sprite;
			Rect rec = spr.rect;
			Vector3 bound = transform.GetComponent<Renderer>().bounds.max - transform.GetComponent<Renderer>().bounds.min;

			TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(spr)) as TextureImporter;

			List<PolygonPoint> p2 = new List<PolygonPoint>();

			if (triangleIndex > 0)
			{
				vertsToCopy = CreateSubVertPoints(spr.bounds, vertsToCopy.ToList(), triangleIndex).ToArray();
			}

			int i = 0;
			for (i = 0; i < vertsToCopy.Count(); i++)
			{
				p2.Add(new PolygonPoint(vertsToCopy[i].x, vertsToCopy[i].y));
			}

			Polygon _polygon = new Polygon(p2);

			if (triangleIndex > 0)
			{
				List<TriangulationPoint> triPoints = GenerateGridPoints(spr.bounds, triangleIndex, _polygon);
				_polygon.AddSteinerPoints(triPoints);
			}

			P2T.Triangulate(_polygon);


			int idx = 0;

			foreach (DelaunayTriangle triangle in _polygon.Triangles)
			{
				Vector3 v = new Vector3();
				foreach (TriangulationPoint p in triangle.Points)
				{
					v = new Vector3((float)p.X, (float)p.Y, 0);
					if (!resultsLocal.Contains(v))
					{
						resultsLocal.Add(v);
						resultsTriIndexesLocal.Add(idx);

						Vector2 newUv = new Vector2((v.x / bound.x) + 0.5f, (v.y / bound.y) + 0.5f);

						newUv.x *= rec.width / spr.texture.width;
						newUv.y *= rec.height / spr.texture.height;

						newUv.x += (rec.x) / spr.texture.width;
						newUv.y += (rec.y) / spr.texture.height;

						SpriteMetaData[] smdArray = textureImporter.spritesheet;
						Vector2 pivot = new Vector2(.0f, .0f); ;

						for (int k = 0; k < smdArray.Length; k++)
						{
							if (smdArray[k].name == spr.name)
							{
								switch (smdArray[k].alignment)
								{
									case (0):
										smdArray[k].pivot = Vector2.zero;
										break;
									case (1):
										smdArray[k].pivot = new Vector2(0f, 1f) - new Vector2(.5f, .5f);
										break;
									case (2):
										smdArray[k].pivot = new Vector2(0.5f, 1f) - new Vector2(.5f, .5f);
										break;
									case (3):
										smdArray[k].pivot = new Vector2(1f, 1f) - new Vector2(.5f, .5f);
										break;
									case (4):
										smdArray[k].pivot = new Vector2(0f, .5f) - new Vector2(.5f, .5f);
										break;
									case (5):
										smdArray[k].pivot = new Vector2(1f, .5f) - new Vector2(.5f, .5f);
										break;
									case (6):
										smdArray[k].pivot = new Vector2(0f, 0f) - new Vector2(.5f, .5f);
										break;
									case (7):
										smdArray[k].pivot = new Vector2(0.5f, 0f) - new Vector2(.5f, .5f);
										break;
									case (8):
										smdArray[k].pivot = new Vector2(1f, 0f) - new Vector2(.5f, .5f);
										break;
									case (9):
										smdArray[k].pivot -= new Vector2(.5f, .5f);
										break;
								}
								pivot = smdArray[k].pivot;
							}
						}
						if (textureImporter.spriteImportMode == SpriteImportMode.Single)
							pivot = textureImporter.spritePivot - new Vector2(.5f, .5f);
						newUv.x += ((pivot.x) * rec.width) / spr.texture.width;
						newUv.y += ((pivot.y) * rec.height) / spr.texture.height;


						uvsLocal.Add(newUv);
						normalsLocal.Add(new Vector3(0, 0, -1));
						idx++;
					}
					else
					{
						resultsTriIndexesLocal.Add(resultsLocal.LastIndexOf(v));
					}


				}
			}



			for (int j = resultsTriIndexesLocal.Count - 1; j >= 0; j--)
			{
				resultsTriIndexesReversedLocal.Add(resultsTriIndexesLocal[j]);
			}

			results.AddRange(resultsLocal);
			resultsTriIndexes.AddRange(resultsTriIndexesLocal);
			resultsTriIndexesReversed.AddRange(resultsTriIndexesReversedLocal);
			uvs.AddRange(uvsLocal);
			normals.AddRange(normalsLocal);

			resultsLocal.Clear();
			resultsTriIndexesLocal.Clear();
			resultsTriIndexesReversedLocal.Clear();
			uvsLocal.Clear();
			normalsLocal.Clear();

			finalVertices = results.ToArray();

			finalNormals = normals.ToArray();
			finalUvs = uvs.ToArray();

			finalTriangles = resultsTriIndexesReversed.ToArray();
		}
		public List<Vector2> CreateSubVertPoints(Bounds bounds, List<Vector2> vertsToCopy, float subdivLevel)
		{
			List<Vector2> returnList = new List<Vector2>();

			float numberDivisions = 6;
			float width = bounds.max.x - bounds.min.x;
			float subdivWidth = width / (subdivLevel * numberDivisions);

			vertsToCopy.Add(vertsToCopy[0]);
			returnList.Add(vertsToCopy[0]);

			for (int i = 1; i < vertsToCopy.Count; i++)
			{

				float distanceBetweenVerts = Vector2.Distance(vertsToCopy[i], vertsToCopy[i - 1]);
				int numberOfNewVerts = Mathf.RoundToInt(distanceBetweenVerts / subdivWidth);

				// add new verts
				if (numberOfNewVerts >= 1)
				{
					for (int j = 1; j < numberOfNewVerts; j++)
					{
						Vector2 newLengthVector = (vertsToCopy[i] - vertsToCopy[i - 1]) / numberOfNewVerts;
						Vector2 vert = vertsToCopy[i - 1] + newLengthVector * j;
						returnList.Add(vert);
					}
				}
				if (i < vertsToCopy.Count - 1)
					returnList.Add(vertsToCopy[i]);

			}


			return returnList;
		}
		public List<TriangulationPoint> GenerateGridPoints(Bounds bounds, float subdivLevel, Polygon _polygon)
		{
			List<TriangulationPoint> GridPoints = new List<TriangulationPoint>();
			float numberDivisions = 6;

			float width = bounds.max.x - bounds.min.x;
			float height = bounds.max.y - bounds.min.y;

			float subdivWidth = width / (subdivLevel * numberDivisions);
			float subdivHeight = height / ((subdivLevel * numberDivisions));

			float averagedLength = (subdivWidth + subdivHeight) / 2;
			float widthHeight = (width + height) / 2;



			for (int i = 1; i < (subdivLevel * numberDivisions / widthHeight) * width; i++)
			{
				for (int j = 1; j < (subdivLevel * numberDivisions / widthHeight) * height; j++)
				{
					float xPos = (i * averagedLength) + bounds.min.x;
					float yPos = (j * averagedLength) + bounds.min.y;
					TriangulationPoint t = new TriangulationPoint(xPos, yPos);
					if (_polygon.IsPointInside(t))
						GridPoints.Add(t);

				}
			}
			return GridPoints;
		}

		public GameObject MakeFromVerts(bool ReverseNormals, Vector3[] vertsToCopy, List<int> pathSplitIds, GameObject FFDGameObject)
		{
			bool overwrite = false;


			MeshedSprite = new GameObject();
			Undo.RegisterCreatedObjectUndo(MeshedSprite, "Created Mesh");

			mf = MeshedSprite.AddComponent<MeshFilter>();
			mr = MeshedSprite.AddComponent<MeshRenderer>();
			mesh = new Mesh();

			if (AssetDatabase.LoadAssetAtPath(Puppet2D_Editor._puppet2DPath + "/Models/" + FFDGameObject.transform.name + "_MESH.asset", typeof(Mesh)))
			{
				if (EditorUtility.DisplayDialog("Overwrite Asset?", "Do you want to overwrite the current Mesh & Material?", "Yes, Overwrite", "No, Create New Mesh & Material"))
				{
					//mf.mesh = AssetDatabase.LoadAssetAtPath(Puppet2D_Editor._puppet2DPath+"/Models/"+transform.name+"_MESH.asset",typeof(Mesh))as Mesh;
					string meshPath = (Puppet2D_Editor._puppet2DPath + "/Models/" + FFDGameObject.transform.name + "_MESH.asset");
					AssetDatabase.CreateAsset(mesh, meshPath);
					overwrite = true;
				}
				else
				{
					string meshPath = AssetDatabase.GenerateUniqueAssetPath(Puppet2D_Editor._puppet2DPath + "/Models/" + FFDGameObject.transform.name + "_MESH.asset");
					AssetDatabase.CreateAsset(mesh, meshPath);
				}
			}
			else
			{
				string meshPath = AssetDatabase.GenerateUniqueAssetPath(Puppet2D_Editor._puppet2DPath + "/Models/" + FFDGameObject.transform.name + "_MESH.asset");
				AssetDatabase.CreateAsset(mesh, meshPath);
			}

			mesh = CreateMeshFromVerts(vertsToCopy, mesh, pathSplitIds, FFDGameObject.transform);


			mf.mesh = mesh;

			results.Clear();
			resultsTriIndexes.Clear();
			resultsTriIndexesReversed.Clear();
			uvs.Clear();
			normals.Clear();


			if (overwrite)
			{
				mr.material = AssetDatabase.LoadAssetAtPath(Puppet2D_Editor._puppet2DPath + "/Models/Materials/" + FFDGameObject.transform.name + "_MAT.mat", typeof(Material)) as Material;
			}
			else
			{

				Material newMat = new Material(Shader.Find("Unlit/Transparent"));
				string materialPath = AssetDatabase.GenerateUniqueAssetPath(Puppet2D_Editor._puppet2DPath + "/Models/Materials/" + FFDGameObject.transform.name + "_MAT.mat");
				AssetDatabase.CreateAsset(newMat, materialPath);
				mr.material = newMat;
			}




			return MeshedSprite;
		}
		public Mesh CreateMeshFromVerts(Vector3[] vertsToCopy, Mesh mesh, List<int> pathSplitIds, Transform SpriteGO = null)
		{
			List<Vector3> resultsLocal = new List<Vector3>();
			List<int> resultsTriIndexesLocal = new List<int>();
			List<int> resultsTriIndexesReversedLocal = new List<int>();
			List<Vector2> uvsLocal = new List<Vector2>();
			List<Vector3> normalsLocal = new List<Vector3>();


			Sprite spr = null;
			Rect rec = new Rect();
			Vector3 bound = Vector3.zero;
			TextureImporter textureImporter = new TextureImporter();

			if (SpriteGO != null && SpriteGO.GetComponent<SpriteRenderer>() && SpriteGO.GetComponent<SpriteRenderer>().sprite)
			{

				spr = SpriteGO.GetComponent<SpriteRenderer>().sprite;
				rec = spr.rect;
				bound = SpriteGO.GetComponent<Renderer>().bounds.max - SpriteGO.GetComponent<Renderer>().bounds.min;
				textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(spr)) as TextureImporter;


			}

			List<PolygonPoint> p2 = new List<PolygonPoint>();
			List<TriangulationPoint> extraPoints = new List<TriangulationPoint>();

			int i = 0;
			for (i = 0; i < vertsToCopy.Count(); i++)
			{
				if (i < pathSplitIds[0])
					p2.Add(new PolygonPoint(vertsToCopy[i].x, vertsToCopy[i].y));
				else
					extraPoints.Add(new TriangulationPoint(vertsToCopy[i].x, vertsToCopy[i].y));
			}

			Polygon _polygon = new Polygon(p2);

			// this is how to add more points
			_polygon.AddSteinerPoints(extraPoints);

			P2T.Triangulate(_polygon);

			if (spr == null)
			{
				bound = new Vector3((float)(_polygon.Bounds.MaxX - _polygon.Bounds.MinX), (float)(_polygon.Bounds.MaxY - _polygon.Bounds.MinY), 0);
			}

			int idx = 0;

			foreach (DelaunayTriangle triangle in _polygon.Triangles)
			{
				Vector3 v = new Vector3();
				foreach (TriangulationPoint p in triangle.Points)
				{
					v = new Vector3((float)p.X, (float)p.Y, 0);
					if (!resultsLocal.Contains(v))
					{
						resultsLocal.Add(v);
						resultsTriIndexesLocal.Add(idx);



						Vector2 newUv = new Vector2(((v.x - (float)_polygon.Bounds.MinX) / bound.x), ((v.y - (float)_polygon.Bounds.MinY) / bound.y));
						if (spr != null)
						{
							newUv = new Vector2((v.x / bound.x) + 0.5f, (v.y / bound.y) + 0.5f);
							newUv.x *= rec.width / spr.texture.width;
							newUv.y *= rec.height / spr.texture.height;

							newUv.x += (rec.x) / spr.texture.width;
							newUv.y += (rec.y) / spr.texture.height;


							SpriteMetaData[] smdArray = textureImporter.spritesheet;
							Vector2 pivot = new Vector2(.0f, .0f); ;

							for (int k = 0; k < smdArray.Length; k++)
							{
								if (smdArray[k].name == spr.name)
								{
									switch (smdArray[k].alignment)
									{
										case (0):
											smdArray[k].pivot = Vector2.zero;
											break;
										case (1):
											smdArray[k].pivot = new Vector2(0f, 1f) - new Vector2(.5f, .5f);
											break;
										case (2):
											smdArray[k].pivot = new Vector2(0.5f, 1f) - new Vector2(.5f, .5f);
											break;
										case (3):
											smdArray[k].pivot = new Vector2(1f, 1f) - new Vector2(.5f, .5f);
											break;
										case (4):
											smdArray[k].pivot = new Vector2(0f, .5f) - new Vector2(.5f, .5f);
											break;
										case (5):
											smdArray[k].pivot = new Vector2(1f, .5f) - new Vector2(.5f, .5f);
											break;
										case (6):
											smdArray[k].pivot = new Vector2(0f, 0f) - new Vector2(.5f, .5f);
											break;
										case (7):
											smdArray[k].pivot = new Vector2(0.5f, 0f) - new Vector2(.5f, .5f);
											break;
										case (8):
											smdArray[k].pivot = new Vector2(1f, 0f) - new Vector2(.5f, .5f);
											break;
										case (9):
											smdArray[k].pivot -= new Vector2(.5f, .5f);
											break;
									}
									pivot = smdArray[k].pivot;
								}
							}
							if (textureImporter.spriteImportMode == SpriteImportMode.Single)
								pivot = textureImporter.spritePivot - new Vector2(.5f, .5f);
							newUv.x += ((pivot.x) * rec.width) / spr.texture.width;
							newUv.y += ((pivot.y) * rec.height) / spr.texture.height;
						}

						uvsLocal.Add(newUv);
						normalsLocal.Add(new Vector3(0, 0, -1));
						idx++;
					}
					else
					{
						resultsTriIndexesLocal.Add(resultsLocal.LastIndexOf(v));
					}


				}
			}



			for (int j = resultsTriIndexesLocal.Count - 1; j >= 0; j--)
			{
				resultsTriIndexesReversedLocal.Add(resultsTriIndexesLocal[j]);
			}

			results.AddRange(resultsLocal);
			resultsTriIndexes.AddRange(resultsTriIndexesLocal);
			resultsTriIndexesReversed.AddRange(resultsTriIndexesReversedLocal);

			uvs.AddRange(uvsLocal);
			normals.AddRange(normalsLocal);

			resultsLocal.Clear();
			resultsTriIndexesLocal.Clear();
			resultsTriIndexesReversedLocal.Clear();
			uvsLocal.Clear();
			normalsLocal.Clear();

			finalVertices = results.ToArray();

			finalNormals = normals.ToArray();
			finalUvs = uvs.ToArray();

			finalTriangles = resultsTriIndexesReversed.ToArray();

			mesh.vertices = finalVertices;
			mesh.triangles = finalTriangles;
			mesh.uv = finalUvs;
			mesh.normals = finalNormals;
			mesh = calculateMeshTangents(mesh);
			return mesh;
		}


		List<Vector2> randomizeArray(List<Vector2> arr)
		{
			int counter = arr.Count;
			List<Vector2> reArr = new List<Vector2>();

			while (counter-- >= 1)
			{
				int rndM = Random.Range(0, arr.Count - 1);
				reArr.Add(arr[rndM]);
				arr.RemoveAt(rndM);
			}

			return reArr;
		}
		public static Mesh calculateMeshTangents(Mesh mesh)
		{
			//speed up math by copying the mesh arrays
			int[] triangles = mesh.triangles;
			Vector3[] vertices = mesh.vertices;
			Vector2[] uv = mesh.uv;
			Vector3[] normals = mesh.normals;

			//variable definitions
			int triangleCount = triangles.Length;
			int vertexCount = vertices.Length;

			Vector3[] tan1 = new Vector3[vertexCount];
			Vector3[] tan2 = new Vector3[vertexCount];

			Vector4[] tangents = new Vector4[vertexCount];

			for (long a = 0; a < triangleCount; a += 3)
			{
				long i1 = triangles[a + 0];
				long i2 = triangles[a + 1];
				long i3 = triangles[a + 2];

				Vector3 v1 = vertices[i1];
				Vector3 v2 = vertices[i2];
				Vector3 v3 = vertices[i3];

				Vector2 w1 = uv[i1];
				Vector2 w2 = uv[i2];
				Vector2 w3 = uv[i3];

				float x1 = v2.x - v1.x;
				float x2 = v3.x - v1.x;
				float y1 = v2.y - v1.y;
				float y2 = v3.y - v1.y;
				float z1 = v2.z - v1.z;
				float z2 = v3.z - v1.z;

				float s1 = w2.x - w1.x;
				float s2 = w3.x - w1.x;
				float t1 = w2.y - w1.y;
				float t2 = w3.y - w1.y;

				float r = 1.0f / (s1 * t2 - s2 * t1);

				Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
				Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

				tan1[i1] += sdir;
				tan1[i2] += sdir;
				tan1[i3] += sdir;

				tan2[i1] += tdir;
				tan2[i2] += tdir;
				tan2[i3] += tdir;
			}


			for (long a = 0; a < vertexCount; ++a)
			{
				Vector3 n = normals[a];
				Vector3 t = tan1[a];

				//Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
				//tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
				Vector3.OrthoNormalize(ref n, ref t);
				tangents[a].x = t.x;
				tangents[a].y = t.y;
				tangents[a].z = t.z;

				tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
			}

			mesh.tangents = tangents;
			return mesh;
		}
	}
}