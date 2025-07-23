# 🧩 CRM Yönetim Paneli

Bu proje, küçük ve orta ölçekli işletmelerin müşteri ilişkilerini daha verimli yönetmelerini sağlamak amacıyla geliştirilmiş bir **CRM (Customer Relationship Management)** sistemidir. ASP.NET Core MVC ve Entity Framework Core kullanılarak geliştirilmiştir.

---

## 🚀 Özellikler

### 👤 Müşteri Yönetimi
- Müşteri oluşturma, güncelleme ve silme işlemleri.
- Her müşteri için:
  - Telefon, e-posta, adres, kayıt tarihi.
  - Sorumlu kullanıcı seçimi.
- Soft delete (silinen müşteriler veritabanında kalır, görünmez).

### 🏷️ Etiketleme Sistemi
- Müşterilere çoklu etiket atanabilir (örn. VIP, Sorunlu, Yeni).
- Etikete göre müşteri filtreleme desteği.
- Etiket yönetim ekranı (Ekle / Güncelle / Sil).
- Etiket silinmek istendiğinde:
  - Eğer bağlı müşteri varsa uyarı verilir.
  - Onayla seçeneğiyle ilişkili etiket bağlantıları kaldırılarak silinir.

### 📋 Görev Takibi
- Müşterilere görev atanabilir.
- Görev bilgileri:
  - Açıklama, sorumlu kullanıcı, kişi, müşteri, durum, son tarih.
- Görev durumları:
  - Bekliyor, Devam Ediyor, Tamamlandı, İptal Edildi.
- Kişiye aynı tarihte aynı görev atanamaz kontrolü.
- Rol bazlı görev listesi:
  - Admin: tüm görevleri görebilir.
  - Kullanıcı: sadece kendisiyle ilişkili görevleri görebilir.

### 💼 Fırsat Takibi
- Müşterilere özel satış fırsatları oluşturulabilir.
- Aşamalar, tahmini gelir, tarih bilgileri.

### 🗓️ Takvim Görünümü
- Haftalık görev görünümü (Pazartesi–Cuma).
- Görevler günlere göre listelenir.
- Duruma göre renkli etiketler:
  - 🔴 Gecikti, 🟡 Bekliyor, ✅ Tamamlandı.
- Göreve tıklanınca görev detayları modal popup olarak gösterilir.

### 🔐 Rol Bazlı Yetkilendirme
- Giriş zorunlu (`[Authorize]`).
- Admin rolü:
  - Tüm müşterileri, görevleri, etiketleri, fırsatları görebilir ve düzenleyebilir.
- Kullanıcı rolü:
  - Sadece kendisine atanan müşteri/görevleri görebilir.
  - Etiketleri sadece görüntüleyebilir, yönetemez.

### 🕵️‍♂️ İşlem Geçmişi (Audit Log) _(opsiyonel olarak eklenebilir)_
- Müşteri, görev, fırsat, not gibi işlemler kayıt altına alınabilir.
- Gelişmiş izleme için `IslemLog` tablosu üzerinden tüm işlemler takip edilebilir.

---

## 🧱 Teknolojiler

| Teknoloji | Açıklama |
|----------|----------|
| ASP.NET Core MVC | Backend ve frontend çatısı |
| Entity Framework Core | ORM ve veritabanı işlemleri |
| SQL Server | Veritabanı |
| Bootstrap / CSS | UI tasarımı |
| AutoMapper | DTO ↔ Entity dönüşümleri |
| Authorization | Rol bazlı yetki yönetimi |

## 🛠️ Kurulum

1. **Veritabanı Oluşturma**
   
   
   script atıldı 
Scaffold-DbContext "Server=.;Database=crmDB;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -force
