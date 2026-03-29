using UnityEngine;
using System.IO;

// -----------------------------------------------------------------------------
// Назначение файла: PhotoCapture.cs
// Путь: Assets/Scripts/Items/PhotoCapture.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

/// <summary>
/// Реализует компонент `PhotoCapture` и инкапсулирует связанную с ним игровую логику.
/// </summary>
public static class PhotoCapture
{
    private static int _photoCounter = 1;

    /// <summary>
    /// Выполняет операцию `GetPhotoFolder` в рамках обязанностей текущего компонента.
    /// </summary>
    public static string GetPhotoFolder()
    {
        string basePath;
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            basePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures);
        }
        else if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        {
            basePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/Pictures";
        }
        else
        {
            basePath = Application.persistentDataPath;
        }
        string folder = Path.Combine(basePath, "MyGamePhotos");
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);
        return folder;
    }

    /// <summary>
    /// Выполняет операцию `TakeScreenshot` в рамках обязанностей текущего компонента.
    /// </summary>
    public static void TakeScreenshot(Camera camera, int width, int height)
    {
        string folder = GetPhotoFolder();
        string filename = $"Photo{_photoCounter}.png";
        string path = Path.Combine(folder, filename);

        // Создаём временный RenderTexture
        RenderTexture rt = RenderTexture.GetTemporary(width, height, 24);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        
        camera.targetTexture = null;
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt); // освобождаем временный RenderTexture

        byte[] bytes = screenShot.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        Debug.Log($"Фото сохранено: {path}");
        _photoCounter++;
    }
}