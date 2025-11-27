using System.Diagnostics;
using System.IO;

namespace DDTankLauncher.Services;

/// <summary>
/// Flash SWF dosyalarını çalıştırmak için servis sınıfı
/// Ruffle veya standalone Flash Projector desteği sağlar
/// </summary>
public class FlashService
{
    /// <summary>
    /// Flash Player türleri
    /// </summary>
    public enum FlashPlayerType
    {
        Unknown,
        Ruffle,
        StandaloneFlashPlayer,
        Browser
    }

    /// <summary>
    /// Oyunu başlatır
    /// </summary>
    /// <param name="swfPath">SWF dosyası yolu</param>
    /// <param name="flashPlayerPath">Flash Player/Ruffle yolu</param>
    /// <param name="serverIP">Sunucu IP adresi</param>
    /// <param name="serverPort">Sunucu portu</param>
    /// <returns>İşlem başarılı mı</returns>
    public static async Task<(bool Success, string Message)> LaunchGameAsync(
        string swfPath, 
        string flashPlayerPath,
        string serverIP,
        int serverPort)
    {
        try
        {
            // Dosya kontrolleri
            if (string.IsNullOrWhiteSpace(swfPath))
            {
                return (false, "SWF dosya yolu belirtilmedi. Lütfen ayarlardan SWF dosyasını seçin.");
            }

            if (!File.Exists(swfPath))
            {
                return (false, $"SWF dosyası bulunamadı: {swfPath}");
            }

            if (string.IsNullOrWhiteSpace(flashPlayerPath))
            {
                return (false, "Flash Player/Ruffle yolu belirtilmedi. Lütfen ayarlardan seçin.");
            }

            if (!File.Exists(flashPlayerPath))
            {
                return (false, $"Flash Player bulunamadı: {flashPlayerPath}");
            }

            LogService.Log($"Oyun başlatılıyor - SWF: {swfPath}");
            LogService.Log($"Flash Player: {flashPlayerPath}");
            LogService.Log($"Sunucu: {serverIP}:{serverPort}");

            // Flash Player türünü belirle
            var playerType = DetectFlashPlayerType(flashPlayerPath);
            LogService.Log($"Flash Player türü: {playerType}");

            // Oyunu başlat
            var processInfo = new ProcessStartInfo
            {
                FileName = flashPlayerPath,
                Arguments = BuildArguments(swfPath, serverIP, serverPort, playerType),
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(swfPath) ?? Environment.CurrentDirectory
            };

            var process = Process.Start(processInfo);
            
            if (process != null)
            {
                LogService.Log($"Oyun başlatıldı (PID: {process.Id})");
                return (true, "Oyun başarıyla başlatıldı!");
            }
            
            return (false, "Oyun başlatılamadı.");
        }
        catch (Exception ex)
        {
            LogService.Log($"Oyun başlatma hatası: {ex.Message}");
            return (false, $"Oyun başlatılırken hata oluştu: {ex.Message}");
        }
    }

    /// <summary>
    /// Flash Player türünü dosya adından tespit eder
    /// </summary>
    private static FlashPlayerType DetectFlashPlayerType(string playerPath)
    {
        string fileName = Path.GetFileName(playerPath).ToLowerInvariant();
        
        if (fileName.Contains("ruffle"))
            return FlashPlayerType.Ruffle;
        
        if (fileName.Contains("flashplayer") || fileName.Contains("flash player"))
            return FlashPlayerType.StandaloneFlashPlayer;
        
        return FlashPlayerType.Unknown;
    }

    /// <summary>
    /// Player türüne göre komut satırı argümanları oluşturur
    /// </summary>
    private static string BuildArguments(string swfPath, string serverIP, int serverPort, FlashPlayerType playerType)
    {
        // Temel argüman - SWF dosyası
        string args = $"\"{swfPath}\"";
        
        // Ruffle için ek parametreler - temel kullanım, tüm Ruffle sürümleriyle uyumlu
        // Not: Ek parametreler gerekirse SettingsService üzerinden yapılandırılabilir
        if (playerType == FlashPlayerType.Ruffle)
        {
            // Sadece SWF dosyası argümanı kullan - en uyumlu yaklaşım
            // Ruffle otomatik olarak doğru runtime'ı seçer
        }
        
        return args;
    }

    /// <summary>
    /// SWF dosyasının geçerli olup olmadığını kontrol eder
    /// </summary>
    public static bool ValidateSwfFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            return false;

        try
        {
            // SWF dosyaları "FWS" veya "CWS" (sıkıştırılmış) magic bytes ile başlar
            byte[] buffer = new byte[3];
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            fs.Read(buffer, 0, 3);
            
            string signature = System.Text.Encoding.ASCII.GetString(buffer);
            bool isValid = signature == "FWS" || signature == "CWS" || signature == "ZWS";
            
            if (isValid)
                LogService.Log($"SWF dosyası doğrulandı: {path}");
            else
                LogService.Log($"Geçersiz SWF dosyası: {path}");
            
            return isValid;
        }
        catch (Exception ex)
        {
            LogService.Log($"SWF doğrulama hatası: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Yaygın Flash Player konumlarını arar
    /// </summary>
    public static List<string> FindFlashPlayers()
    {
        var players = new List<string>();
        var searchPaths = new[]
        {
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"),
            Environment.CurrentDirectory
        };

        var searchPatterns = new[] { "ruffle*.exe", "flashplayer*.exe", "flash*.exe" };

        foreach (var basePath in searchPaths)
        {
            if (string.IsNullOrEmpty(basePath) || !Directory.Exists(basePath))
                continue;

            try
            {
                foreach (var pattern in searchPatterns)
                {
                    var files = Directory.GetFiles(basePath, pattern, SearchOption.AllDirectories);
                    players.AddRange(files);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Bu dizine erişim izni yok - sessizce atla
            }
            catch (DirectoryNotFoundException)
            {
                // Dizin bulunamadı - sessizce atla
            }
            catch (Exception ex)
            {
                // Diğer hatalar için debug log
                System.Diagnostics.Debug.WriteLine($"[FlashService] Flash Player aranırken hata ({basePath}): {ex.Message}");
            }
        }

        return players.Distinct().ToList();
    }
}
