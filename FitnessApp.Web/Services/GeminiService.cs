using FitnessApp.Web.ViewModels;
using System.Text;
using System.Text.Json;

namespace FitnessApp.Web.Services;

public class GeminiService : IAIService
{
    private readonly string? _apiKey;
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeminiService> _logger;

    public GeminiService(IConfiguration configuration, ILogger<GeminiService> logger)
    {
        _apiKey = configuration["Gemini:ApiKey"];
        _httpClient = new HttpClient();
        _logger = logger;
    }

    public async Task<string> GeneratePlanAsync(UserStatsViewModel stats)
    {
        // Eğer API Key yoksa veya varsayılan değerse Mock data dön
        if (string.IsNullOrEmpty(_apiKey) || _apiKey == "YOUR_GEMINI_API_KEY_HERE")
        {
            _logger.LogWarning("Gemini API anahtarı yapılandırılmamış. Mock plan döndürülüyor.");
            return GenerateMockPlan(stats);
        }

        try
        {
            return await CallGeminiApiAsync(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gemini API çağrısı başarısız oldu. Mock plana geri dönülüyor.");
            return GenerateMockPlan(stats);
        }
    }

    private async Task<string> CallGeminiApiAsync(UserStatsViewModel stats)
    {
        var prompt = $@"Sen bir fitness ve beslenme uzmanısın. Aşağıdaki kullanıcı bilgilerine göre Türkçe olarak kişiselleştirilmiş bir fitness ve beslenme planı oluştur.

Kullanıcı Bilgileri:
- Yaş: {stats.Age}
- Boy: {stats.Height} cm
- Kilo: {stats.Weight} kg
- Cinsiyet: {stats.Gender}
- Aktivite Seviyesi: {stats.ActivityLevel}
- Hedef: {stats.Goal}

Lütfen şunları içeren detaylı bir plan oluştur:
1. Günlük kalori hedefi ve makro besin dağılımı
2. Haftalık antrenman programı (hangi günler hangi egzersizler)
3. Beslenme önerileri ve örnek öğünler
4. Başarı için pratik ipuçları

Yanıtı Markdown formatında ver.";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.7,
                maxOutputTokens = 2048
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent?key={_apiKey}";
        
        var response = await _httpClient.PostAsync(url, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Gemini API hatası: {StatusCode} - {Response}", response.StatusCode, responseBody);
            throw new HttpRequestException($"Gemini API error: {response.StatusCode}");
        }

        // Parse response
        using var doc = JsonDocument.Parse(responseBody);
        var root = doc.RootElement;
        
        if (root.TryGetProperty("candidates", out var candidates) && 
            candidates.GetArrayLength() > 0)
        {
            var firstCandidate = candidates[0];
            if (firstCandidate.TryGetProperty("content", out var contentElement) &&
                contentElement.TryGetProperty("parts", out var parts) &&
                parts.GetArrayLength() > 0)
            {
                var text = parts[0].GetProperty("text").GetString();
                return text ?? GenerateMockPlan(stats);
            }
        }

        return GenerateMockPlan(stats);
    }

    private string GenerateMockPlan(UserStatsViewModel stats)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"# Kişiselleştirilmiş Fitness Planı ({stats.Goal})");
        sb.AppendLine($"**Profil:** {stats.Age} yaş, {stats.Weight}kg, {stats.Height}cm, {stats.Gender}");
        sb.AppendLine($"**Aktivite Seviyesi:** {stats.ActivityLevel}");
        sb.AppendLine();

        sb.AppendLine("## 🥗 Beslenme Önerileri");
        if (stats.Goal == "Kilo Verme")
        {
            sb.AppendLine("- **Kalori Hedefi:** Günlük ~1800-2000 kalori.");
            sb.AppendLine("- **Protein:** Yüksek proteinli kahvaltılar (yumurta, lor peyniri).");
            sb.AppendLine("- **Karbonhidrat:** Akşam 18:00'den sonra karbonhidratı azaltın.");
            sb.AppendLine("- **Su:** Günde en az 3 litre su tüketin.");
        }
        else if (stats.Goal == "Kas Kazanma")
        {
            sb.AppendLine("- **Kalori Hedefi:** Günlük ~2500-3000 kalori (Fazla kalori alımı şart).");
            sb.AppendLine("- **Protein:** Kilo başına 2g protein (Tavuk, Balık, Kırmızı Et).");
            sb.AppendLine("- **Karbonhidrat:** Antrenman öncesi ve sonrası kompleks karbonhidratlar (Pirinç, Yulaf).");
        }
        else
        {
            sb.AppendLine("- **Kalori Hedefi:** Günlük ihtiyacınızı koruyun.");
            sb.AppendLine("- **Denge:** Protein, yağ ve karbonhidrat dengesini koruyun.");
        }

        sb.AppendLine();
        sb.AppendLine("## 🏋️‍♂️ Antrenman Programı");
        if (stats.ActivityLevel == "Düşük")
        {
            sb.AppendLine("Başlangıç seviyesi olduğunuz için haftada 3 gün tüm vücut (Full Body) antrenmanı öneriyoruz.");
            sb.AppendLine("- **Pazartesi:** Full Body (Squat, Push-up, Row)");
            sb.AppendLine("- **Çarşamba:** Kardiyo (30 dk yürüyüş) + Karın egzersizleri");
            sb.AppendLine("- **Cuma:** Full Body (Lunge, Shoulder Press, Plank)");
        }
        else
        {
            sb.AppendLine("Orta/Yüksek seviye için haftada 4-5 gün bölgesel antrenman (Split) öneriyoruz.");
            sb.AppendLine("- **Gün 1:** Göğüs & Arka Kol");
            sb.AppendLine("- **Gün 2:** Sırt & Ön Kol");
            sb.AppendLine("- **Gün 3:** Dinlenme veya Hafif Kardiyo");
            sb.AppendLine("- **Gün 4:** Bacak & Omuz");
            sb.AppendLine("- **Gün 5:** Full Body veya Eksik Bölgeler");
        }

        sb.AppendLine();
        sb.AppendLine("> [!NOTE]");
        sb.AppendLine("> Bu plan yapay zeka tarafından genel öneri olarak oluşturulmuştur. Herhangi bir sağlık sorununuz varsa lütfen önce doktorunuza danışın.");

        return sb.ToString();
    }
}
