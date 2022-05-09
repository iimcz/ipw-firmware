using UnityEngine;
using emt_sdk.Events;
using emt_sdk.Generated.ScenePackage;
using emt_sdk.Scene;
using System.IO;


public class PanoComponent : MonoBehaviour {
	Renderer m_Renderer;
	
	public void SetPanorama(Texture2D tex){
		m_Renderer.material.mainTexture = tex;
	}
	
	public void LoadPanorama(string path){
		byte[] fileData = File.ReadAllBytes(path);
		Texture2D tex = new Texture2D(2, 2);
		tex.LoadImage(fileData);
		
		SetPanorama(tex);
	}
	
	void Start () {
		//Fetch the Renderer from the GameObject
		m_Renderer = GetComponent<Renderer> ();

		var settings = ExhibitConnectionComponent.ActivePackage.Parameters.Settings;
		var fileName = settings.FileName;
		var basePath = ExhibitConnectionComponent.ActivePackage.DataRoot;

		var filePath = Path.Combine(basePath, fileName);
		
		LoadPanorama(filePath);
	}
}
