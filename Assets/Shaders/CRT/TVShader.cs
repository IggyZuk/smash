using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]

public class TVShader : MonoBehaviour
{
	public Shader shader;
	private Material _material;
	[Range(2, 64)]
	public float pixle_size = 64.0f;

	protected Material material
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
		if(shader == null) return;
		Material mat = material;
		Graphics.Blit(source, destination, mat);

		mat.SetFloat("_PixelSize", pixle_size);
	}

	void OnDisable()
	{
		if(_material)
		{
			DestroyImmediate(_material);
		}
	}
}
