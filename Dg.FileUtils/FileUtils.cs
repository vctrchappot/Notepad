using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Dg.FileUtils;

public class FileUtils
{
    private void SaveToFile(string filePath)
    {
        try
        {
            File.WriteAllText(filePath, MultilineTextBox.Text);
            _currentFilePath = filePath;
            _hasUnsavedChanges = false;
            UpdateTitle();
            MessageBox.Show("Datei gespeichert!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Speichern: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}