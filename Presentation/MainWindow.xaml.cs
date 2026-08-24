using System;
using System.IO;
using System.Windows;
using Dg.FileUtils;
using Microsoft.Win32;

namespace Notepad;

public partial class MainWindow : Window
{
    private string? currentFilePath;
    private bool hasUnsavedChanges = false;

    public MainWindow()
    {
        InitializeComponent();
        MultilineTextBox.TextChanged += (s, e) => OnTextChanged();
        Title = "Notepad - Unbenannt";
    }

    private void OnTextChanged()
    {
        hasUnsavedChanges = true;
        UpdateTitle();
    }

    private void UpdateTitle()
    {
        var fileName = string.IsNullOrEmpty(currentFilePath)
            ? "Unbenannt"
            : Path.GetFileName(currentFilePath);

        var unsaved = hasUnsavedChanges ? " *" : "";

        Title = $"Notepad - {fileName}{unsaved}";
    }
    
    private void OnClick_File_New(object sender, RoutedEventArgs e)
    {
        if (hasUnsavedChanges &&
            MessageBox.Show(
                "Änderungen verwerfen?",
                "Neue Datei",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes)
        {
            return;
        }

        MultilineTextBox.Clear();
        currentFilePath = null;
        hasUnsavedChanges = false;
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

        if (dialog.ShowDialog() != true) return;
        try
        {
            var content = File.ReadAllText(dialog.FileName);
            MultilineTextBox.Text = content;
            currentFilePath = dialog.FileName;
            hasUnsavedChanges = false;
            UpdateTitle();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Öffnen: {ex.Message}", "Fehler");
        }
    }

    private void OnClick_File_Save(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(currentFilePath))
        {
            OnClick_File_SaveTo(sender, e);
            return;
        }

        var content = MultilineTextBox.Text;
        FileUtils.SaveToFile(currentFilePath, content);
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

        if (dialog.ShowDialog() != true) return;
        var content = MultilineTextBox.Text;
            
        FileUtils.SaveToFile(dialog.FileName, content);
    }

    private void OnClick_File_Exit(object sender, RoutedEventArgs e)
    {
        if (hasUnsavedChanges && 
            MessageBox.Show("Ungespeicherte Änderungen verwerfen und Programm schliessen?", "Beenden", 
                MessageBoxButton.YesNo) == MessageBoxResult.No)
        {
            return;
        }
        
        Close();
    }
}