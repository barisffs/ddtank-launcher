# 🎮 DDTank Launcher

DDTank PvP Sunucusu için C# WPF Flash Destekli Launcher

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![Platform](https://img.shields.io/badge/Platform-Windows-0078D6)
![License](https://img.shields.io/badge/License-MIT-green)

## 📖 Açıklama

DDTank Launcher, DDTank özel sunucularına (private server) bağlanmak için geliştirilmiş modern bir Windows uygulamasıdır. Flash desteğinin sona ermesinin ardından, Ruffle veya Adobe Flash Player Projector kullanarak SWF oyun dosyalarını çalıştırmanıza olanak tanır.

### ✨ Özellikler

- 🎨 Modern ve şık kullanıcı arayüzü (DDTank temalı)
- 🔌 Sunucu bağlantı durumu kontrolü
- ⚙️ Kolay yapılandırma paneli
- 🎬 Ruffle ve Flash Player desteği
- 📝 Otomatik log sistemi
- 💾 JSON tabanlı ayar yönetimi

## 🛠️ Gereksinimler

### Sistem Gereksinimleri
- **İşletim Sistemi:** Windows 10/11
- **Framework:** .NET 8.0 Runtime

### Ek Gereksinimler
- **Flash Oynatıcı:** Aşağıdakilerden biri:
  - [Ruffle](https://ruffle.rs/) - Önerilen (Rust tabanlı Flash emülatörü)
  - [Adobe Flash Player Projector](https://www.adobe.com/support/flashplayer/debug_downloads.html) - Standalone versiyon

## 📦 Kurulum

### 1. .NET Runtime Kurulumu

.NET 8.0 Runtime yüklü değilse, [buradan](https://dotnet.microsoft.com/download/dotnet/8.0) indirip kurun.

### 2. Uygulamayı Çalıştırma

#### Kaynak Koddan Derleme

```bash
# Repoyu klonlayın
git clone https://github.com/barisffs/ddtank-launcher.git
cd ddtank-launcher/DDTankLauncher

# Projeyi derleyin
dotnet build

# Uygulamayı çalıştırın
dotnet run --project DDTankLauncher
```

#### Release Versiyonunu Kullanma

1. [Releases](https://github.com/barisffs/ddtank-launcher/releases) sayfasından en son sürümü indirin
2. ZIP dosyasını çıkarın
3. `DDTankLauncher.exe` dosyasını çalıştırın

## 📥 Download .exe (Windows)

DDTank Launcher'ın Windows x64 için tek dosyalık (self-contained) çalıştırılabilir sürümünü indirebilirsiniz. Bu sürüm .NET kurulumu gerektirmez.

### Otomatik Build (GitHub Actions)

1. **Manuel Tetikleme:**
   - Repo'nun [Actions](https://github.com/barisffs/ddtank-launcher/actions) sayfasına gidin
   - Sol taraftaki menüden "Build and Publish Windows exe" workflow'unu seçin
   - "Run workflow" butonuna tıklayın ve main branch'i seçerek çalıştırın

2. **Otomatik Tetikleme:**
   - `main` branch'ine her push yapıldığında workflow otomatik olarak çalışır

### İndirme

- **Actions Artifact'tan:** Workflow çalıştıktan sonra Actions sekmesinden ilgili çalıştırmaya tıklayın ve "Artifacts" bölümünden `DDTankLauncher-win-x64.exe` artifact'ını indirin.
- **Releases'tan:** `main` branch'ine push yapıldığında otomatik olarak bir GitHub Release oluşturulur. [Releases](https://github.com/barisffs/ddtank-launcher/releases) sayfasından en son sürümün asset'lerinden `DDTankLauncher-win-x64.exe` dosyasını indirin.

### Çalıştırma

1. İndirilen `DDTankLauncher.exe` veya `DDTankLauncher-win-x64.exe` dosyasını herhangi bir klasöre koyun
2. Dosyaya çift tıklayarak çalıştırın
3. Windows SmartScreen uyarısı çıkarsa "More info" > "Run anyway" seçeneklerini kullanın

> **Not:** Self-contained build olduğu için hedef makinede .NET yüklü olmasına gerek yoktur.

## 🎯 Kullanım Rehberi

### İlk Kurulum

1. **Uygulamayı Başlatın:** DDTankLauncher.exe'yi çalıştırın
2. **Ayarları Açın:** Sağ alttaki "⚙️ Ayarlar" butonuna tıklayın
3. **Sunucu Bilgilerini Girin:**
   - Sunucu IP adresi (örn: `127.0.0.1` veya sunucu IP'si)
   - Sunucu portu (örn: `9001`)
4. **Flash Ayarlarını Yapın:**
   - SWF dosyasını seçin (oyun dosyası)
   - Flash Player veya Ruffle yolunu seçin
5. **Kaydet** butonuna tıklayın

### Oyunu Başlatma

1. Ana ekranda sunucu durumunun "Çevrimiçi" olduğunu kontrol edin
2. **"🎮 OYUNU BAŞLAT"** butonuna tıklayın
3. Oyun Flash Player/Ruffle ile açılacaktır

### Sunucu Durumu

- 🟢 **Çevrimiçi:** Sunucu aktif ve bağlantı kurulabilir
- 🔴 **Çevrimdışı:** Sunucu erişilebilir değil
- 🟡 **Kontrol ediliyor:** Durum kontrolü yapılıyor

## ⚙️ Yapılandırma

Ayarlar dosyası şu konumda saklanır:
```
%APPDATA%\DDTankLauncher\settings.json
```

### Varsayılan Ayarlar

```json
{
  "ServerIP": "127.0.0.1",
  "ServerPort": 9001,
  "SwfPath": "",
  "FlashPlayerPath": "",
  "Language": "tr-TR"
}
```

### Log Dosyaları

Log dosyaları şu konumda bulunur:
```
%APPDATA%\DDTankLauncher\Logs\
```

## 🏗️ Proje Yapısı

```
DDTankLauncher/
├── DDTankLauncher.sln              # Solution dosyası
├── DDTankLauncher/
│   ├── DDTankLauncher.csproj       # Proje dosyası
│   ├── App.xaml                    # Uygulama tanımı
│   ├── App.xaml.cs                 # Uygulama başlangıç kodu
│   ├── MainWindow.xaml             # Ana pencere UI
│   ├── MainWindow.xaml.cs          # Ana pencere kodu
│   ├── Views/
│   │   ├── SettingsWindow.xaml     # Ayarlar penceresi UI
│   │   └── SettingsWindow.xaml.cs  # Ayarlar penceresi kodu
│   ├── Models/
│   │   └── LauncherSettings.cs     # Ayarlar modeli
│   ├── Services/
│   │   ├── SettingsService.cs      # Ayar yönetimi servisi
│   │   ├── ServerService.cs        # Sunucu kontrolü servisi
│   │   ├── FlashService.cs         # Flash yönetimi servisi
│   │   └── LogService.cs           # Log servisi
│   ├── Resources/
│   │   └── Styles.xaml             # UI stilleri
│   └── Assets/                     # Görsel kaynaklar
├── README.md                       # Bu dosya
└── .gitignore                      # Git ignore dosyası
```

## ❓ Sık Sorulan Sorular

### S: "Flash Player bulunamadı" hatası alıyorum
**C:** Ayarlar panelinden Flash Player veya Ruffle yolunu doğru şekilde seçtiğinizden emin olun. Ruffle kullanmanızı öneririz.

### S: SWF dosyası nedir ve nereden bulurum?
**C:** SWF, Flash oyun dosyasıdır. Bu dosyayı sunucu yöneticinizden veya oyun sağlayıcınızdan edinmeniz gerekmektedir.

### S: Sunucu çevrimdışı görünüyor ama sunucu çalışıyor
**C:** 
- Sunucu IP ve port bilgilerinin doğru olduğundan emin olun
- Güvenlik duvarı ayarlarını kontrol edin
- "Durumu Kontrol Et" butonuna tıklayarak tekrar deneyin

### S: Oyun açılıyor ama sunucuya bağlanamıyorum
**C:** 
- Oyun içi sunucu ayarlarını kontrol edin
- SWF dosyasının doğru sunucu için yapılandırıldığından emin olun
- Sunucu yöneticinize danışın

### S: Ruffle mu yoksa Flash Player mı kullanmalıyım?
**C:** Ruffle'ı öneriyoruz çünkü:
- Adobe Flash Player artık desteklenmiyor ve güvenlik güncellemesi almıyor
- Ruffle açık kaynaklı ve aktif olarak geliştiriliyor
- Çoğu SWF dosyasıyla uyumlu çalışıyor

### S: Ayarlarımı nasıl sıfırlarım?
**C:** Ayarlar penceresinde sol alttaki "🔄 Varsayılanlar" butonuna tıklayın.

## 🔧 Geliştirici Bilgileri

### Derleme

```bash
# Debug build
dotnet build

# Release build
dotnet build -c Release

# Publish (self-contained)
dotnet publish -c Release -r win-x64 --self-contained true
```

### Bağımlılıklar

- .NET 8.0 SDK
- System.Text.Json (8.0.0)

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

## 🤝 Katkıda Bulunma

Katkılarınızı bekliyoruz! Pull request göndermeden önce lütfen:

1. Bu repoyu fork edin
2. Feature branch oluşturun (`git checkout -b feature/YeniOzellik`)
3. Değişikliklerinizi commit edin (`git commit -m 'Yeni özellik eklendi'`)
4. Branch'inizi push edin (`git push origin feature/YeniOzellik`)
5. Pull Request açın

## 📞 İletişim

Sorularınız veya önerileriniz için issue açabilirsiniz.

---

**Not:** Bu proje yalnızca eğitim amaçlıdır ve yasal DDTank özel sunucuları ile kullanılmak üzere tasarlanmıştır.
