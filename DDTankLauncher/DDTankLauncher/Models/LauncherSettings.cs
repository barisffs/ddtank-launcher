using System.Text.Json.Serialization;

namespace DDTankLauncher.Models;

/// <summary>
/// Launcher ayarlarını temsil eden model sınıfı
/// </summary>
public class LauncherSettings
{
    /// <summary>
    /// Sunucu IP adresi
    /// </summary>
    [JsonPropertyName("ServerIP")]
    public string ServerIP { get; set; } = "127.0.0.1";

    /// <summary>
    /// Sunucu port numarası
    /// </summary>
    [JsonPropertyName("ServerPort")]
    public int ServerPort { get; set; } = 9001;

    /// <summary>
    /// SWF oyun dosyası yolu
    /// </summary>
    [JsonPropertyName("SwfPath")]
    public string SwfPath { get; set; } = string.Empty;

    /// <summary>
    /// Flash Player veya Ruffle çalıştırıcı yolu
    /// </summary>
    [JsonPropertyName("FlashPlayerPath")]
    public string FlashPlayerPath { get; set; } = string.Empty;

    /// <summary>
    /// Uygulama dili
    /// </summary>
    [JsonPropertyName("Language")]
    public string Language { get; set; } = "tr-TR";

    /// <summary>
    /// Varsayılan ayarları oluşturur
    /// </summary>
    public static LauncherSettings CreateDefault()
    {
        return new LauncherSettings
        {
            ServerIP = "127.0.0.1",
            ServerPort = 9001,
            SwfPath = string.Empty,
            FlashPlayerPath = string.Empty,
            Language = "tr-TR"
        };
    }

    /// <summary>
    /// Ayarları başka bir ayar nesnesinden kopyalar
    /// </summary>
    public void CopyFrom(LauncherSettings other)
    {
        ServerIP = other.ServerIP;
        ServerPort = other.ServerPort;
        SwfPath = other.SwfPath;
        FlashPlayerPath = other.FlashPlayerPath;
        Language = other.Language;
    }

    /// <summary>
    /// Ayarların klonunu oluşturur
    /// </summary>
    public LauncherSettings Clone()
    {
        return new LauncherSettings
        {
            ServerIP = ServerIP,
            ServerPort = ServerPort,
            SwfPath = SwfPath,
            FlashPlayerPath = FlashPlayerPath,
            Language = Language
        };
    }
}
