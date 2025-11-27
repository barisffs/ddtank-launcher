using System.Windows;
using DDTankLauncher.Services;

namespace DDTankLauncher;

/// <summary>
/// DDTank Launcher uygulaması için ana giriş noktası
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Uygulama başlangıcında ayarları yükle
        SettingsService.Instance.LoadSettings();
        
        // Log sistemi başlat
        LogService.Log("DDTank Launcher başlatıldı");
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Uygulama kapanırken ayarları kaydet
        SettingsService.Instance.SaveSettings();
        LogService.Log("DDTank Launcher kapatıldı");
        
        base.OnExit(e);
    }
}
