# Spor Salonu Yönetim Sistemi (FitnessApp)

Bu proje, Web Programlama dersi kapsamında geliştirilen, kapsamlı bir **Spor Salonu Yönetim ve Randevu Sistemidir**. ASP.NET Core MVC mimarisi kullanılarak, Entity Framework Core ile veritabanı yönetimi ve Identity kütüphanesi ile kullanıcı yetkilendirmesi sağlanmıştır.

## 🚀 Özellikler

### 👤 Üye Paneli
- **Kayıt & Giriş:** Güvenli üyelik sistemi.
- **Randevu Alma:** Eğitmenlerin müsaitlik durumuna göre dinamik randevu oluşturma.
- **Randevu Geçmişi:** Bekleyen, onaylanan ve geçmiş randevuları görüntüleme ve iptal etme.
- **Akıllı Asistan (AI):** Vücut kitle indeksi ve hedeflere göre kişiselleştirilmiş egzersiz/diyet programı oluşturma.
- **Eğitmen & Hizmet İnceleme:** Detaylı eğitmen profilleri ve hizmet açıklamaları.

### 🛠 Yönetici (Admin) Paneli
- **Dashboard:** Genel istatistikler ve hızlı erişim.
- **Salon & Hizmet Yönetimi:** Salon bilgileri ve hizmet kategorilerinin CRUD işlemleri.
- **Eğitmen Yönetimi:** Eğitmen ekleme, fotoğraf yükleme ve uzmanlık alanı atama.
- **Çalışma Saatleri:** Eğitmenler için haftalık çalışma programı ve çakışma kontrolü.
- **Randevu Onayı:** Üyelerden gelen randevu taleplerini onaylama veya reddetme.

### 🔌 Teknik Özellikler
- **Mimari:** ASP.NET Core MVC 9.0
- **Veritabanı:** MS SQL Server (Entity Framework Core Code-First)
- **Yetkilendirme:** ASP.NET Core Identity (Role-Based: Admin, Member)
- **API:** RESTful API endpoints (Swagger UI ile dokümante edilmiş).
- **Validasyon:** Server-side (Data Annotations) ve Client-side (jQuery Validation).
- **Localization:** Türkçe (tr-TR) kültür desteği.

## ⚙️ Kurulum ve Çalıştırma

1.  **Gereksinimler:**
    -   .NET 9.0 SDK
    -   SQL Server (LocalDB veya Full)

2.  **Veritabanı Oluşturma:**
    Terminali proje dizininde (`FitnessApp.Web`) açın ve aşağıdaki komutu çalıştırın:
    ```bash
    dotnet ef database update
    ```
    *Bu komut veritabanını oluşturacak ve başlangıç verilerini (Admin kullanıcısı vb.) ekleyecektir.*

3.  **Projeyi Başlatma:**
    ```bash
    dotnet run
    ```
    Tarayıcınızda `http://localhost:5277` adresine gidin.

4.  **Giriş Bilgileri:**
    -   **Admin:** `ogrencinumarasi@sakarya.edu.tr` / Şifre: `sau`
    -   **Üye:** Kayıt olabilir veya `member@test.com` / `sau` (eğer seed edildiyse) kullanabilirsiniz.

## 📚 API Dokümantasyonu
Proje çalışırken `/swagger` adresine giderek API endpoint'lerini test edebilirsiniz.
-   `GET /api/Trainers`: Eğitmen listesi.
-   `GET /api/Appointments/my-history`: Üye randevu geçmişi (Auth gerektirir).

## 🤖 Akıllı Asistan
Üyeler, "Akıllı Asistan" menüsünden yaş, kilo, boy ve hedeflerini girerek yapay zeka destekli (Mock/Demo modunda) beslenme ve antrenman tavsiyeleri alabilirler.