using System.IO;

namespace DDTankLauncher.Services;

/// <summary>
/// Log kayıtları için basit servis sınıfı
/// </summary>
public static class LogService
{
    private static readonly object _lock = new();
    private static readonly string _logDirectory;
    private static readonly string _logFilePath;
    private static bool _isInitialized;

    static LogService()
    {
        _logDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DDTankLauncher",
            "Logs"
        );
        
        // Her gün için ayrı log dosyası
        string fileName = $"launcher_{DateTime.Now:yyyy-MM-dd}.log";
        _logFilePath = Path.Combine(_logDirectory, fileName);
    }

    /// <summary>
    /// Log sistemini başlatır
    /// </summary>
    private static void Initialize()
    {
        if (_isInitialized) return;

        lock (_lock)
        {
            if (_isInitialized) return;

            try
            {
                if (!Directory.Exists(_logDirectory))
                {
                    Directory.CreateDirectory(_logDirectory);
                }
                _isInitialized = true;
            }
            catch
            {
                // Log dizini oluşturulamazsa sessizce devam et
            }
        }
    }

    /// <summary>
    /// Log mesajı yazar
    /// </summary>
    /// <param name="message">Log mesajı</param>
    /// <param name="level">Log seviyesi</param>
    public static void Log(string message, LogLevel level = LogLevel.Info)
    {
        Initialize();

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string logMessage = $"[{timestamp}] [{level}] {message}";

        // Konsola yaz (debug modunda)
        System.Diagnostics.Debug.WriteLine(logMessage);

        // Dosyaya yaz
        try
        {
            lock (_lock)
            {
                File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
            }
        }
        catch
        {
            // Dosya yazma hatası - sessizce devam et
        }
    }

    /// <summary>
    /// Hata loglar
    /// </summary>
    public static void LogError(string message) => Log(message, LogLevel.Error);

    /// <summary>
    /// Uyarı loglar
    /// </summary>
    public static void LogWarning(string message) => Log(message, LogLevel.Warning);

    /// <summary>
    /// Debug loglar
    /// </summary>
    public static void LogDebug(string message) => Log(message, LogLevel.Debug);

    /// <summary>
    /// Eski log dosyalarını temizler (30 günden eski)
    /// </summary>
    public static void CleanOldLogs(int daysToKeep = 30)
    {
        Initialize();

        try
        {
            if (!Directory.Exists(_logDirectory)) return;

            var files = Directory.GetFiles(_logDirectory, "launcher_*.log");
            var cutoffDate = DateTime.Now.AddDays(-daysToKeep);

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.CreationTime < cutoffDate)
                {
                    File.Delete(file);
                    Log($"Eski log dosyası silindi: {fileInfo.Name}");
                }
            }
        }
        catch (Exception ex)
        {
            Log($"Log temizleme hatası: {ex.Message}", LogLevel.Warning);
        }
    }

    /// <summary>
    /// Log dizin yolunu döndürür
    /// </summary>
    public static string GetLogDirectory() => _logDirectory;
}

/// <summary>
/// Log seviyeleri
/// </summary>
public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error
}
