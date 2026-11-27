# YMH219-SpeechApp

Windows Forms uygulaması, yazılı metni sesli okur ve mikrofondan alınan konuşmayı metne dönüştürür.

## Kullanılan teknolojiler

- C# ve .NET 10
- Windows Forms
- Windows konuşma servisleri için `System.Speech`

## Özellikler

- **Metinden Sese:** Metin yazma, sesli okumayı başlatma/devam ettirme, duraklatma, durdurma ve metni temizleme.
- **Sesten Metne:** Varsayılan mikrofonla konuşmayı tanıma, sonuçları zaman damgasıyla listeleme, dinlemeyi durdurma ve sonuçları temizleme.
- Aktif işlem durumu ve mikrofon/konuşma hizmeti hataları için anlaşılır kullanıcı mesajları.

## Çalıştırma

1. Windows 10/11 ve .NET 10 SDK kurulu olmalıdır.
2. Depoyu klonlayıp `YMH219-SpeechApp.slnx` dosyasını Visual Studio 2026 ile açın.
3. NuGet paketlerinin geri yüklenmesini bekleyin.
4. Bilgisayarınıza bağlı bir mikrofon kullanın; Windows gizlilik ayarlarından masaüstü uygulamalarının mikrofon erişimine izin verin.
5. Visual Studio'da **Başlat (F5)** seçeneğiyle uygulamayı çalıştırın.

Konuşma tanıma için Windows'ta uygun bir konuşma dili/speech recognition bileşeninin kurulu ve kullanılabilir olması gerekir. Bu bileşen veya varsayılan mikrofon yoksa uygulama kullanıcıya hata mesajı gösterir.

## Ekran görüntüleri

Uygulamayı çalıştırdıktan sonra en az iki gerçek ekran görüntüsü alın ve `screenshots` klasörüne aşağıdaki adlarla ekleyin:

1. `screenshots/main-window.png` — uygulamanın iki bölümlü ana penceresi.
2. `screenshots/speech-recognition.png` — konuşma tanıma sonucu metin alanında görünürken.

![Uygulamanın ana penceresi](screenshots/main-window.png)

![Konuşma tanıma sonucu](screenshots/speech-recognition.png)

## Depo notları

Derleme çıktıları ve Visual Studio geçici dosyaları (`bin/`, `obj/`, `.vs/`) `.gitignore` tarafından dışlanır.
