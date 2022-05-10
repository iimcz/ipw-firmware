using UnityEngine;
using emt_sdk.Events;
using emt_sdk.Scene;
using System.IO;
using System;


public class PanoComponent : MonoBehaviour {
	Renderer m_Renderer;

	[SerializeField] 
	private Material _panoMat;

	//Adjusting 'lines' affects the triangle count of the sphere
	[SerializeField] 
	int lines = 64;

	[SerializeField] 
	int radius = 10;
	
	public void SetPanorama(Texture2D tex){
		m_Renderer.material.mainTexture = tex;
	}
	
	public void LoadPanorama(string path){
		byte[] fileData = File.ReadAllBytes(path);
		Texture2D tex = new Texture2D(2, 2);
		tex.LoadImage(fileData);
		
		SetPanorama(tex);
	}

	void SetupMesh(){
		m_Renderer = gameObject.AddComponent<MeshRenderer>();
		m_Renderer.sharedMaterial = _panoMat;

		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

		Mesh mesh = new Mesh();

		int vertexCount = (lines + 1) * (lines + 1);

		Vector3[] vertices = new Vector3[vertexCount];
		Vector2[] uvs = new Vector2[vertexCount];

		double lat_step = Math.PI / lines;
		double long_step = 2 * Math.PI / lines;

		int processed = 0;
		for(int i = 0; i < lines + 1; i++){
			float y = (float) Math.Cos(i * lat_step) * radius;
			double y_slice_r = Math.Sin(i * lat_step) * radius;

			//The first and last vertex on the y slice circle are in the same place
			for(int j = 0; j < lines + 1; j++){
				float x = (float) (Math.Sin(j * long_step) * y_slice_r);
				float z = (float) (Math.Cos(j * long_step) * y_slice_r);
				vertices[processed] = new Vector3(x, y, z);

				float u = 1 - ((float) j / lines);
				float v = 1 - ((float) i / lines);
				uvs[processed] = new Vector2(u, v);
				processed++;
			}
		}

		/* loops iterate lines * lines times, each iteration adds two triangles,
		 * except the very first and very last, which add only one. Each triangle
		 * has 3 indices
		 */
		int[] tris = new int[(lines * lines * 2 - 2) * 3];

		processed = 0;
		for(int i = 0; i < lines; i++){
			int slice_idx = i * (lines + 1);
			int next_slice_idx = (i + 1) * (lines + 1);

			for(int j = 0; j < lines; j++){
				//Since the top and bottom slices are circles with radius 0,
				//we only need one triangle for those
				if(i != lines - 1){
					tris[processed++] = slice_idx + j + 1;
					tris[processed++] = next_slice_idx + j;
					tris[processed++] = next_slice_idx + j + 1;
				}

				if(i != 0){
					tris[processed++] = slice_idx + j + 1;
					tris[processed++] = slice_idx + j;
					tris[processed++] = next_slice_idx + j;
				}
			}
		}

		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = tris;

		meshFilter.mesh = mesh;
	}

	void Start () {
		SetupMesh();

		var settings = ExhibitConnectionComponent.ActivePackage.Parameters.Settings;
		var fileName = settings.FileName;
		var basePath = ExhibitConnectionComponent.ActivePackage.DataRoot;

		var filePath = Path.Combine(basePath, fileName);

		LoadPanorama(filePath);
	}
}
