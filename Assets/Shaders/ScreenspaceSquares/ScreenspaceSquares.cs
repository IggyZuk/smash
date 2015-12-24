using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]

public class ScreenspaceSquares : MonoBehaviour
{
	public Shader shader;

	public Texture2D BackgroundTex;

	public Color BackgroundColor;
	public Color Color1;
	public Color Color2;

	[Range(2, 64)]
	public int Scale = 32;

	[Range(1, 128)]
	public int TextureScale = 1;

	private Material _material;

	void Start()
	{
		OnRenderImage(null, RenderTexture.active);
	}

	protected Material Material
	{
		get
		{
			if(_material == null)
			{
				_material = new Material(shader);
				_material.hideFlags = HideFlags.HideAndDontSave;
			}
			return _material;
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if(shader)
		{
			Material mat = Material;

			Graphics.Blit(new RenderTexture(0,0,0), destination, mat);

			mat.SetTexture("_BackgroundTex", BackgroundTex);

			mat.SetColor("_BackgroundColor", BackgroundColor);
			mat.SetColor("_Color1", Color1);
			mat.SetColor("_Color2", Color2);

			mat.SetFloat("_Scale", Scale);
			mat.SetFloat("_TexScale", TextureScale);
			mat.SetFloat("_Offset", Camera.main.transform.position.y / 25f);
		}
	}
}
