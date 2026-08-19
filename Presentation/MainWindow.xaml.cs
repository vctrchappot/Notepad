using System;
using System.IO;
using System.Windows;
using Dg.FileUtils;
using Microsoft.Win32;

namespace Notepad;

public partial class MainWindow : Window
{
    private string? _currentFilePath;
    private bool _hasUnsavedChanges = false;

    public MainWindow()
    {
        InitializeComponent();
        MultilineTextBox.TextChanged += (s, e) => OnTextChanged();
        Title = "Notepad - Unbenannt";
    }

    private void OnTextChanged()
    {
        _hasUnsavedChanges = true;
        UpdateTitle();
    }

    private void UpdateTitle()
    {
        var fileName = string.IsNullOrEmpty(_currentFilePath)
            ? "Unbenannt"
            : Path.GetFileName(_currentFilePath);

        var unsaved = _hasUnsavedChanges ? " *" : "";
        Title = $"Notepad - {fileName}{unsaved}";
    }
    
    private void SaveToFile(string filePath, string content)
    {
        try
        {
            FileUtils.SaveToFile(filePath, content);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Fehler beim Speichern: {ex.Message}",
                "Fehler",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void OnClick_File_New(object sender, RoutedEventArgs e)
    {
        if (_hasUnsavedChanges &&
            MessageBox.Show(
                "Änderungen verwerfen?",
                "Neue Datei",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes)
        {
            return;
        }

        MultilineTextBox.Clear();
        _currentFilePath = null;
        _hasUnsavedChanges = false;
        UpdateTitle();
    }

    private void OnClick_File_Open(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            InitialDirectory = Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments),
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                var content = File.ReadAllText(dialog.FileName);
                MultilineTextBox.Text = content;
                _currentFilePath = dialog.FileName;
                _hasUnsavedChanges = false;
                UpdateTitle();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Öffnen: {ex.Message}", "Fehler");
            }
        }
    }

    private void OnClick_File_Save(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_currentFilePath))
        {
            OnClick_File_SaveTo(sender, e);
            return;
        }

        var content = MultilineTextBox.Text;
        FileUtils.SaveToFile(_currentFilePath, content);
    }

    private void OnClick_File_SaveTo(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            InitialDirectory = Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments),
            FileName = "Untitled.txt",
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            DefaultExt = ".txt",
            AddExtension = true
        };

        if (dialog.ShowDialog() == true)
        {
            var content = MultilineTextBox.Text;
            
            FileUtils.SaveToFile(dialog.FileName, content);
        }
    }

    private void OnClick_File_Exit(object sender, RoutedEventArgs e)
    {
        if (_hasUnsavedChanges && 
            MessageBox.Show("Ungespeicherte Änderungen verwerfen und Programm schliessen?", "Beenden", 
                MessageBoxButton.YesNo) == MessageBoxResult.No)
        {
            return;
        }
        
        Close();
    }
}