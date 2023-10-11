using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildCommon
{
    /// <summary>
    /// Создание билда для указанного города.
    /// </summary>
    /// <param name="cityName">Имя города для билда.</param>
    /// <param name="target">Платформа для билда (Android, Windows и т.д.)</param>
    /// <param name="extension">Расширение файла билда (.apk, .exe и т.д.)</param>
    public static void BuildForCity(string cityName, BuildTarget target, string extension)
    {
        // Запрашиваем у пользователя папку для сохранения билда
        string folderPath = EditorUtility.SaveFolderPanel($"Выберите папку для билда {cityName}", "", "");

        if (string.IsNullOrEmpty(folderPath))
        {
            return; // Пользователь отменил выбор папки
        }

        // Загрузка и установка значения в AppSettings 
        AppSetting settings = AssetDatabase.LoadAssetAtPath<AppSetting>("Assets/_App/Scripts/UI/SceneManagerSity/AppSettingSity.asset");
        if (settings == null)
        {
            Debug.LogError("Не удалось загрузить AppSettings. Убедитесь, что он создан и путь указан верно.");
            return;
        }
        settings.City = cityName;
        EditorUtility.SetDirty(settings); // Помечаем объект как измененный
        AssetDatabase.SaveAssets(); // Сохраняем изменения

        string[] cityScenes = GetScenesForCity(cityName);
        string[] universalScenes = 
        {
            "Assets/_App/Scenes/StartGame.unity",
            "Assets/_App/Scenes/Lobby.unity"
        };

        string[] allScenes = new string[cityScenes.Length + universalScenes.Length];
        universalScenes.CopyTo(allScenes, 0); // Сначала добавляем универсальные сцены
        cityScenes.CopyTo(allScenes, universalScenes.Length); // Затем добавляем сцены города

        if (allScenes.Length == 0)
        {
            Debug.LogError("Сцены для билда не найдены");
            return;
        }

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = allScenes,
            locationPathName = $"{folderPath}/{cityName}{extension}", // Используем путь, выбранный пользователем
            target = target,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Билд для {cityName} успешно создан");
        }
    }

    /// <summary>
    /// Получение списка сцен для указанного города.
    /// </summary>
    /// <param name="cityName">Имя города.</param>
    /// <returns>Массив с путями к сценам города.</returns>
    private static string[] GetScenesForCity(string cityName)
    {
        // Список всех карт, которые могут быть в любом городе.
        string[] baseMapNames = { "Taverna", "SkyArena", "Necropolis", "Tower", "Dungeon" };

        // Создаём массив строк, который будет содержать полные пути к сценам.
        string[] scenes = new string[baseMapNames.Length];

        for (int i = 0; i < baseMapNames.Length; i++)
        {
            // Формируем полный путь к сцене с использованием базового имени карты и имени города.
            scenes[i] = $"Assets/_App/Scenes/City/{cityName}/{baseMapNames[i]}_{cityName}.unity";
        }

        return scenes;
    }
}
