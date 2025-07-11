using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.SceneManagement;

public class CanvasExporter : MonoBehaviour
{
    public Camera uiCamera;
    public string fileName = "StatsExport.png";

    public Canvas exportCanvas;
    public Camera exportCamera;

    public int textureWidth = 1920;
    public int textureHeight = 1080;

    public void ExportCanvasToPNG()
    {
        RenderTexture renderTexture = new RenderTexture(textureWidth, textureHeight, 0, RenderTextureFormat.ARGB32);
        renderTexture.Create();

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        uiCamera.targetTexture = renderTexture;
        uiCamera.Render();

        Texture2D image = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        image.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        image.Apply();

        byte[] bytes = image.EncodeToPNG();
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(path, bytes);

        Debug.Log("Canvas exported to: " + path);

        RenderTexture.active = currentRT;
        uiCamera.targetTexture = null;

        Destroy(image);
        renderTexture.Release();
        Destroy(renderTexture);
    }

    public void ExportStatsToPNG()
    {
        StartCoroutine(ExportStatsCoroutine());
    }

    public IEnumerator ExportStatsCoroutine()
    {
        exportCanvas.gameObject.SetActive(true);
        exportCamera.gameObject.SetActive(true);

        yield return null;

        Canvas.ForceUpdateCanvases();

        RenderTexture rt = new RenderTexture(textureWidth, textureHeight, 24);
        exportCamera.targetTexture = rt;

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = rt;

        exportCamera.Render();

        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(Application.persistentDataPath, fileName), bytes);

        Debug.Log("Exported to: " + Path.Combine(Application.persistentDataPath, fileName));

        RenderTexture.active = currentRT;
        exportCamera.targetTexture = null;
        Object.Destroy(tex);
        Object.Destroy(rt);

        exportCanvas.gameObject.SetActive(false);
        exportCamera.gameObject.SetActive(false);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
