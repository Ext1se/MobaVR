using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AppCommandLine
{
    private const string TAG = nameof(AppCommandLine);

    public static int HelloCmd()
    {
        Console.WriteLine("Hello from Unity: Console1");
        Directory.CreateDirectory(@"C:\Projects\Unity\VR\VRIF\Builds\Scripts\NewFolder1");
        Console.WriteLine("12");
        System.Diagnostics.Debug.WriteLine("This is a log");
        Debug.Log("Hello from Unity: Debug");
        Console.Write("Hello from Unity: Console");

        return -1;
    }
}