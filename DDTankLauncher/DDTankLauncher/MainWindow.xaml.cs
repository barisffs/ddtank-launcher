using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DDTankLauncher.Models;
using DDTankLauncher.Services;
using DDTankLauncher.Views;

namespace DDTankLauncher;

/// <summary>
/// DDTank Launcher ana penceresi
/// </summary>
public partial class MainWindow : Window
{
    private LauncherSettings _settings;
    private bool _isChecking;

    public MainWindow()
    {
        InitializeComponent();
        _settings = SettingsService.Instance.Settings;
        
        // UI'ı başlat
        UpdateServerInfoDisplay();
        
        // Sunucu durumunu kontrol et
        Loaded += MainWindow_Loaded;
    }

    /// <summary>
    /// Pencere yüklendiğinde çalışır
    /// </summary>
    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await CheckServerStatusAsync();
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
    /// Küçült butonu
    /// </summary>
    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    /// <summary>
    /// Kapat butonu
    /// </summary>
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    /// <summary>
    /// Oyunu Başlat butonu
    /// </summary>
    private async void PlayButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            PlayButton.IsEnabled = false;
            ShowLoading("Oyun başlatılıyor...");

            var (success, message) = await FlashService.LaunchGameAsync(
                _settings.SwfPath,
                _settings.FlashPlayerPath,
                _settings.ServerIP,
                _settings.ServerPort
            );

            HideLoading();

            if (success)
            {
                LogService.Log("Oyun başarıyla başlatıldı");
                // Başarı mesajı göster
                ShowMessage("Başarılı", message, MessageBoxImage.Information);
            }
            else
            {
                LogService.LogError($"Oyun başlatılamadı: {message}");
                ShowMessage("Hata", message, MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            HideLoading();
            LogService.LogError($"Beklenmeyen hata: {ex.Message}");
            ShowMessage("Hata", $"Beklenmeyen bir hata oluştu: {ex.Message}", MessageBoxImage.Error);
        }
        finally
        {
            PlayButton.IsEnabled = true;
        }
    }

    /// <summary>
    /// Durum kontrol butonu
    /// </summary>
    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        await CheckServerStatusAsync();
    }

    /// <summary>
    /// Ayarlar butonu
    /// </summary>
    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow();
        settingsWindow.Owner = this;
        
        if (settingsWindow.ShowDialog() == true)
        {
            // Ayarlar güncellendi, UI'ı güncelle
            _settings = SettingsService.Instance.Settings;
            UpdateServerInfoDisplay();
            
            // Sunucu durumunu tekrar kontrol et
            _ = CheckServerStatusAsync();
        }
    }

    /// <summary>
    /// Sunucu durumunu kontrol eder
    /// </summary>
    private async Task CheckServerStatusAsync()
    {
        if (_isChecking) return;
        
        try
        {
            _isChecking = true;
            RefreshButton.IsEnabled = false;
            
            // Kontrol ediliyor durumuna geç
            SetServerStatus(ServerService.ServerStatus.Checking);
            
            // Sunucu durumunu kontrol et
            var status = await ServerService.CheckServerStatusAsync(
                _settings.ServerIP, 
                _settings.ServerPort
            );
            
            // Ping değerini al
            long ping = -1;
            if (status == ServerService.ServerStatus.Online)
            {
                ping = await ServerService.PingServerAsync(_settings.ServerIP);
            }
            
            // UI'ı güncelle
            SetServerStatus(status, ping);
        }
        catch (Exception ex)
        {
            LogService.LogError($"Durum kontrolü hatası: {ex.Message}");
            SetServerStatus(ServerService.ServerStatus.Unknown);
        }
        finally
        {
            _isChecking = false;
            RefreshButton.IsEnabled = true;
        }
    }

    /// <summary>
    /// Sunucu durumu göstergesini günceller
    /// </summary>
    private void SetServerStatus(ServerService.ServerStatus status, long ping = -1)
    {
        switch (status)
        {
            case ServerService.ServerStatus.Online:
                StatusIndicator.Fill = (SolidColorBrush)FindResource("SuccessGreenBrush");
                StatusText.Text = "Çevrimiçi";
                StatusText.Foreground = (SolidColorBrush)FindResource("SuccessGreenBrush");
                PlayButton.IsEnabled = true;
                if (ping >= 0)
                {
                    PingText.Text = $"Ping: {ping}ms";
                }
                break;
                
            case ServerService.ServerStatus.Offline:
                StatusIndicator.Fill = (SolidColorBrush)FindResource("ErrorRedBrush");
                StatusText.Text = "Çevrimdışı";
                StatusText.Foreground = (SolidColorBrush)FindResource("ErrorRedBrush");
                PlayButton.IsEnabled = false;
                PingText.Text = "";
                break;
                
            case ServerService.ServerStatus.Checking:
                StatusIndicator.Fill = (SolidColorBrush)FindResource("WarningYellowBrush");
                StatusText.Text = "Kontrol ediliyor...";
                StatusText.Foreground = (SolidColorBrush)FindResource("WarningYellowBrush");
                PingText.Text = "";
                break;
                
            default:
                StatusIndicator.Fill = (SolidColorBrush)FindResource("TextSecondaryBrush");
                StatusText.Text = "Bilinmiyor";
                StatusText.Foreground = (SolidColorBrush)FindResource("TextSecondaryBrush");
                PlayButton.IsEnabled = true;
                PingText.Text = "";
                break;
        }
    }

    /// <summary>
    /// Sunucu bilgisi metnini günceller
    /// </summary>
    private void UpdateServerInfoDisplay()
    {
        ServerInfoText.Text = $"Sunucu: {_settings.ServerIP}:{_settings.ServerPort}";
    }

    /// <summary>
    /// Yükleniyor göstergesini gösterir
    /// </summary>
    private void ShowLoading(string message)
    {
        LoadingText.Text = message;
        LoadingOverlay.Visibility = Visibility.Visible;
    }

    /// <summary>
    /// Yükleniyor göstergesini gizler
    /// </summary>
    private void HideLoading()
    {
        LoadingOverlay.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Mesaj kutusu gösterir
    /// </summary>
    private void ShowMessage(string title, string message, MessageBoxImage icon)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, icon);
    }
}
