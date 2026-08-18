using System;
using System.IO;
using System.Windows;
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
        string fileName = string.IsNullOrEmpty(_currentFilePath)
            ? "Unbenannt"
            : Path.GetFileName(_currentFilePath);

        string unsaved = _hasUnsavedChanges ? " *" : "";
        Title = $"Notepad - {fileName}{unsaved}";
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
                string content = File.ReadAllText(dialog.FileName);
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

    private void OnClick_File_Save_Save(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_currentFilePath))
        {
            OnClick_File_Save_SaveTo(sender, e);
            return;
        }

        SaveToFile(_currentFilePath);
    }

    private void OnClick_File_Save_SaveTo(object sender, RoutedEventArgs e)
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
            SaveToFile(dialog.FileName);
        }
    }

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

    private void OnClick_File_Exit(object sender, RoutedEventArgs e)
    {
        if (_hasUnsavedChanges && 
            MessageBox.Show("Ungespeicherte Änderungen verwerfen?", "Beenden", 
                MessageBoxButton.YesNo) == MessageBoxResult.No)
        {
            return;
        }
        Close();
    }
}