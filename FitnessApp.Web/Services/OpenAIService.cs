using FitnessApp.Web.ViewModels;
using System.Text;

namespace FitnessApp.Web.Services;

public class OpenAIService : IAIService
{
    private readonly string? _apiKey;
    private readonly HttpClient _httpClient;

    public OpenAIService(IConfiguration configuration)
    {
        _apiKey = configuration["OpenAI:ApiKey"];
        _httpClient = new HttpClient();
    }

    public async Task<string> GeneratePlanAsync(UserStatsViewModel stats)
    {
        // Eğer API Key yoksa veya boşsa Mock data dön
        if (string.IsNullOrEmpty(_apiKey) || _apiKey == "YOUR_API_KEY_HERE")
        {
            return GenerateMockPlan(stats);
        }

        // Burada gerçek API çağrısı yapılabilir (Şimdilik demo için mock kullanıyoruz)
        // Gerçek implementasyon için OpenAI API endpoint'ine istek atılmalı.
        // Güvenlik ve maliyet nedeniyle bu aşamada Mock tercih ediyoruz.
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
