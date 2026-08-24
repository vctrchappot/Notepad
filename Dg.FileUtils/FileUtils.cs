using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Dg.FileUtils;

public class FileUtils
{
    public static void SaveToFile(string filePath, string content)
    {
        File.WriteAllText(filePath, content);
    }
}