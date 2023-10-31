using UnityEditor;

public class MenuAppBuilder
{
    #region Test

    [MenuItem("MobaVR/Build/Test/Android Client")]
    public static void BuildAndroidClient_Test()
    {
        AppBuilder.Build("ARMA", "android", false);
    }
    
    [MenuItem("MobaVR/Build/Test/Android Admin")]
    public static void BuildAndroidAdmin_Test()
    {
        AppBuilder.Build("ARMA", "android", true);
    }

    [MenuItem("MobaVR/Build/Test/Meta")]
    public static void BuildMeta_Test()
    {
        AppBuilder.Build("ARMA", "meta", false);
    }

    [MenuItem("MobaVR/Build/Test/Pico")]
    public static void BuildPico_Test()
    {
        AppBuilder.Build("ARMA", "pico", false);
    }

    [MenuItem("MobaVR/Build/Test/Windows Client")]
    public static void BuildWindowsClient_Test()
    {
        AppBuilder.Build("ARMA", "windows", false);
    }
    
    [MenuItem("MobaVR/Build/Test/Windows Admin")]
    public static void BuildWindowsAdmin_Test()
    {
        AppBuilder.Build("ARMA", "windows", true);
    }

    #endregion


    #region Arma

    [MenuItem("MobaVR/Build/Android/Build Arma")]
    public static void BuildAndroid_ARMA()
    {
        BuildCommon.BuildForCity("Arma", BuildTarget.Android, ".apk");
    }

    [MenuItem("MobaVR/Build/Meta Quest 2/Build Arma")]
    public static void BuildMeta_ARMA()
    {
        BuildCommon.BuildForCity("Arma", BuildTarget.Android, ".apk");
    }

    [MenuItem("MobaVR/Build/Pico/Build Arma")]
    public static void BuildPico_ARMA()
    {
        BuildCommon.BuildForCity("Arma", BuildTarget.Android, ".apk");
    }


    [MenuItem("MobaVR/Build/Windows64/Build Arma")]
    public static void BuildWindows64_ARMA()
    {
        BuildCommon.BuildForCity("Arma", BuildTarget.StandaloneWindows64, ".exe");
    }

    #endregion

    #region China

    [MenuItem("MobaVR/Build/Android/Build China")]
    public static void BuildAndroid_China()
    {
        BuildCommon.BuildForCity("China", BuildTarget.Android, ".apk");
    }

    [MenuItem("MobaVR/Build/Meta Quest 2/Build China")]
    public static void BuildMeta_China()
    {
        BuildCommon.BuildForCity("China", BuildTarget.Android, ".apk");
    }

    [MenuItem("MobaVR/Build/Pico/Build China")]
    public static void BuildPico_China()
    {
        BuildCommon.BuildForCity("China", BuildTarget.Android, ".apk");
    }

    [MenuItem("MobaVR/Build/Windows64/Build China")]
    public static void BuildExeForChina()
    {
        BuildCommon.BuildForCity("China", BuildTarget.StandaloneWindows64, ".exe");
    }

    #endregion
}