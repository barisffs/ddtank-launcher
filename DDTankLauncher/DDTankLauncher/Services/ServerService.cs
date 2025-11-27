using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace DDTankLauncher.Services;

/// <summary>
/// Sunucu bağlantı kontrolü ve durum yönetimi servisi
/// </summary>
public class ServerService
{
    /// <summary>
    /// Sunucu durumu
    /// </summary>
    public enum ServerStatus
    {
        Unknown,
        Online,
        Offline,
        Checking
    }

    /// <summary>
    /// Sunucunun çevrimiçi olup olmadığını kontrol eder
    /// </summary>
    /// <param name="host">Sunucu IP adresi</param>
    /// <param name="port">Sunucu portu</param>
    /// <param name="timeoutMs">Zaman aşımı (milisaniye)</param>
    /// <returns>Sunucu durumu</returns>
    public static async Task<ServerStatus> CheckServerStatusAsync(string host, int port, int timeoutMs = 5000)
    {
        try
        {
            LogService.Log($"Sunucu durumu kontrol ediliyor: {host}:{port}");

            using var client = new TcpClient();
            using var cts = new CancellationTokenSource(timeoutMs);
            
            await client.ConnectAsync(host, port, cts.Token);
            
            if (client.Connected)
            {
                LogService.Log("Sunucu çevrimiçi");
                return ServerStatus.Online;
            }
            
            return ServerStatus.Offline;
        }
        catch (OperationCanceledException)
        {
            LogService.Log("Sunucu bağlantısı zaman aşımına uğradı");
            return ServerStatus.Offline;
        }
        catch (SocketException ex)
        {
            LogService.Log($"Sunucu bağlantı hatası: {ex.Message}");
            return ServerStatus.Offline;
        }
        catch (Exception ex)
        {
            LogService.Log($"Beklenmeyen hata: {ex.Message}");
            return ServerStatus.Unknown;
        }
    }

    /// <summary>
    /// Sunucuya ping atar
    /// </summary>
    /// <param name="host">Sunucu IP adresi</param>
    /// <param name="timeoutMs">Zaman aşımı (milisaniye)</param>
    /// <returns>Ping süresi (ms) veya -1 (başarısız)</returns>
    public static async Task<long> PingServerAsync(string host, int timeoutMs = 5000)
    {
        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(host, timeoutMs);
            
            if (reply.Status == IPStatus.Success)
            {
                LogService.Log($"Ping başarılı: {reply.RoundtripTime}ms");
                return reply.RoundtripTime;
            }
            
            LogService.Log($"Ping başarısız: {reply.Status}");
            return -1;
        }
        catch (Exception ex)
        {
            LogService.Log($"Ping hatası: {ex.Message}");
            return -1;
        }
    }

    /// <summary>
    /// Sunucu bilgilerini döndürür (gelecekte genişletilebilir)
    /// </summary>
    public static string GetServerInfo(string host, int port)
    {
        return $"Sunucu: {host}:{port}";
    }
}
