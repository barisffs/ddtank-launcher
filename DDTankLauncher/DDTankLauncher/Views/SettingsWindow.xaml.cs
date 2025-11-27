using System.Windows;
using System.Windows.Input;
using DDTankLauncher.Models;
using DDTankLauncher.Services;
using Microsoft.Win32;

namespace DDTankLauncher.Views;

/// <summary>
/// Launcher ayarları penceresi
/// </summary>
public partial class SettingsWindow : Window
{
    private LauncherSettings _originalSettings;

    public SettingsWindow()
    {
        InitializeComponent();
        
        // Mevcut ayarları yükle
        _originalSettings = SettingsService.Instance.Settings.Clone();
        LoadSettingsToUI();
    }

    /// <summary>
    /// Pencereyi sürüklemek için
    /// </summary>
    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    /// <summary>
    /// Kapat butonu
    /// </summary>
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    /// <summary>
    /// SWF dosyası seçme butonu
    /// </summary>
    private void BrowseSwfButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "SWF Oyun Dosyası Seç",
            Filter = "Flash Dosyaları (*.swf)|*.swf|Tüm Dosyalar (*.*)|*.*",
            CheckFileExists = true
        };

        if (dialog.ShowDialog() == true)
        {
            // SWF dosyasını doğrula
            if (FlashService.ValidateSwfFile(dialog.FileName))
            {
                SwfPathTextBox.Text = dialog.FileName;
            }
            else
            {
                MessageBox.Show(
                    "Seçilen dosya geçerli bir SWF dosyası değil.",
                    "Hata",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }
    }

    /// <summary>
    /// Flash Player seçme butonu
    /// </summary>
    private void BrowseFlashPlayerButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Flash Player veya Ruffle Seç",
            Filter = "Çalıştırılabilir Dosyalar (*.exe)|*.exe|Tüm Dosyalar (*.*)|*.*",
            CheckFileExists = true
        };

        if (dialog.ShowDialog() == true)
        {
            FlashPlayerPathTextBox.Text = dialog.FileName;
        }
    }

    /// <summary>
    /// Varsayılanlara sıfırla butonu
    /// </summary>
    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Tüm ayarlar varsayılan değerlere sıfırlanacak. Devam etmek istiyor musunuz?",
            "Ayarları Sıfırla",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question
        );

        if (result == MessageBoxResult.Yes)
        {
            var defaults = LauncherSettings.CreateDefault();
            ServerIPTextBox.Text = defaults.ServerIP;
            ServerPortTextBox.Text = defaults.ServerPort.ToString();
            SwfPathTextBox.Text = defaults.SwfPath;
            FlashPlayerPathTextBox.Text = defaults.FlashPlayerPath;
            
            LogService.Log("Ayarlar varsayılanlara sıfırlandı (henüz kaydedilmedi)");
        }
    }

    /// <summary>
    /// İptal butonu
    /// </summary>
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    /// <summary>
    /// Kaydet butonu
    /// </summary>
    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        // Girişleri doğrula
        if (!ValidateInputs())
        {
            return;
        }

        try
        {
            // Ayarları güncelle
            var newSettings = new LauncherSettings
            {
                ServerIP = ServerIPTextBox.Text.Trim(),
                ServerPort = int.Parse(ServerPortTextBox.Text.Trim()),
                SwfPath = SwfPathTextBox.Text.Trim(),
                FlashPlayerPath = FlashPlayerPathTextBox.Text.Trim(),
                Language = "tr-TR"
            };

            SettingsService.Instance.UpdateSettings(newSettings);
            
            LogService.Log("Ayarlar kaydedildi");
            
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            LogService.LogError($"Ayarlar kaydedilemedi: {ex.Message}");
            MessageBox.Show(
                $"Ayarlar kaydedilirken hata oluştu: {ex.Message}",
                "Hata",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }

    /// <summary>
    /// Girişleri doğrular
    /// </summary>
    private bool ValidateInputs()
    {
        // IP adresi kontrolü
        if (string.IsNullOrWhiteSpace(ServerIPTextBox.Text))
        {
            ShowValidationError("Sunucu IP adresi boş olamaz.");
            ServerIPTextBox.Focus();
            return false;
        }

        // Port kontrolü
        if (!int.TryParse(ServerPortTextBox.Text, out int port) || port < 1 || port > 65535)
        {
            ShowValidationError("Geçersiz port numarası. Port 1-65535 arasında olmalıdır.");
            ServerPortTextBox.Focus();
            return false;
        }

        return true;
    }

    /// <summary>
    /// Doğrulama hatası mesajı gösterir
    /// </summary>
    private void ShowValidationError(string message)
    {
        MessageBox.Show(message, "Doğrulama Hatası", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    /// <summary>
    /// Ayarları UI'a yükler
    /// </summary>
    private void LoadSettingsToUI()
    {
        ServerIPTextBox.Text = _originalSettings.ServerIP;
        ServerPortTextBox.Text = _originalSettings.ServerPort.ToString();
        SwfPathTextBox.Text = _originalSettings.SwfPath;
        FlashPlayerPathTextBox.Text = _originalSettings.FlashPlayerPath;
    }
}
