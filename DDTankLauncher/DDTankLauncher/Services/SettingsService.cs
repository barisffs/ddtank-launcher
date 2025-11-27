using System.IO;
using System.Text.Json;
using DDTankLauncher.Models;

namespace DDTankLauncher.Services;

/// <summary>
/// Launcher ayarlarını yöneten servis sınıfı (Singleton pattern)
/// </summary>
public sealed class SettingsService
{
    private static readonly Lazy<SettingsService> _instance = new(() => new SettingsService());
    private readonly string _settingsPath;
    private LauncherSettings _settings;

    /// <summary>
    /// SettingsService singleton instance
    /// </summary>
    public static SettingsService Instance => _instance.Value;

    /// <summary>
    /// Geçerli ayarlar
    /// </summary>
    public LauncherSettings Settings => _settings;

    private SettingsService()
    {
        // Ayarlar dosyası uygulama dizininde saklanır
        string appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DDTankLauncher"
        );
        
        // Dizin yoksa oluştur
        if (!Directory.Exists(appDataPath))
        {
            Directory.CreateDirectory(appDataPath);
        }

        _settingsPath = Path.Combine(appDataPath, "settings.json");
        _settings = LauncherSettings.CreateDefault();
    }

    /// <summary>
    /// Ayarları dosyadan yükler
    /// </summary>
    public void LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                string json = File.ReadAllText(_settingsPath);
                var loadedSettings = JsonSerializer.Deserialize<LauncherSettings>(json);
                
                if (loadedSettings != null)
                {
                    _settings = loadedSettings;
                    LogService.Log("Ayarlar başarıyla yüklendi");
                }
            }
            else
            {
                LogService.Log("Ayarlar dosyası bulunamadı, varsayılan ayarlar kullanılıyor");
                SaveSettings(); // Varsayılan ayarları kaydet
            }
        }
        catch (Exception ex)
        {
            LogService.Log($"Ayarlar yüklenirken hata oluştu: {ex.Message}");
            _settings = LauncherSettings.CreateDefault();
        }
    }

    /// <summary>
    /// Ayarları dosyaya kaydeder
    /// </summary>
    public void SaveSettings()
    {
        try
        {
            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true 
            };
            
            string json = JsonSerializer.Serialize(_settings, options);
            File.WriteAllText(_settingsPath, json);
            LogService.Log("Ayarlar başarıyla kaydedildi");
        }
        catch (Exception ex)
        {
            LogService.Log($"Ayarlar kaydedilirken hata oluştu: {ex.Message}");
        }
    }

    /// <summary>
    /// Ayarları günceller ve kaydeder
    /// </summary>
    public void UpdateSettings(LauncherSettings newSettings)
    {
        _settings.CopyFrom(newSettings);
        SaveSettings();
    }

    /// <summary>
    /// Ayarları varsayılanlara sıfırlar
    /// </summary>
    public void ResetToDefaults()
    {
        _settings = LauncherSettings.CreateDefault();
        SaveSettings();
        LogService.Log("Ayarlar varsayılanlara sıfırlandı");
    }
}
