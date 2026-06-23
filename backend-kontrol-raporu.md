# e-iskele Backend Kontrol Raporu

Tarih: 23 Haziran 2026  
Kapsam: `eiskele/backend` altındaki Entity, Business/Application/Infrastructure servisleri, EF Core/Persistence ve veritabani baglanti yapisi.  
Not: Uygulama koduna mudahale edilmedi; yalnizca bu rapor dosyasi olusturuldu.

## Genel Degerlendirme

Backend genel olarak katmanli mimariye uygun bir iskelete sahip: `Domain`, `Application`, `Infrastructure`, `Persistence`, `Api` ve `Shared` projeleri ayrilmis. Entity'ler Domain katmaninda, DTO/interface/validator yapilari Application katmaninda, servis implementasyonlari Infrastructure katmaninda, EF Core DbContext ve configuration/migration dosyalari Persistence katmaninda tutuluyor.

Ancak standartlara tam uygun sayilmasi icin ozellikle guvenlik, EF configuration kapsami, entity modelleme tutarliligi ve servis sorumluluklari tarafinda iyilestirme gerekiyor. En onemli risk, gelistirme `appsettings.json` dosyasinda gercek/veritabani benzeri baglanti bilgisi ve JWT secret bulunmasi.

## Olumlu Noktalar

- Proje ayrimi Clean Architecture'a yakin: Domain, Application, Infrastructure, Persistence ve Api ayrimi dogru yonde.
- `EIskeleDbContext`, `IdentityDbContext<ApplicationUser, ApplicationRole, Guid>` uzerine kurulmus; Identity entegrasyonu tutarli gorunuyor.
- `ApplyConfigurationsFromAssembly(typeof(EIskeleDbContext).Assembly)` kullanildigi icin EF configuration siniflari otomatik yukleniyor.
- `SaveChangesAsync` icinde audit alanlari (`CreatedAt`, `UpdatedAt`) ve soft delete davranisi merkezi olarak ele alinmis.
- Bircok kritik iliskide `DeleteBehavior.Restrict` kullanilarak rezervasyon/odeme gibi kayitlarin yanlis cascade delete ile silinmesi engellenmis.
- Decimal para alanlarinin bir kismi `decimal(18,2)` olarak tanimlanmis.
- Docker Compose tarafinda production connection string ve JWT secret environment variable ile veriliyor; bu dogru yaklasim.

## Kritik Bulgular

### 1. Hassas bilgiler `appsettings.json` icinde tutuluyor

`src/EIskele.Api/appsettings.json` icinde `DefaultConnection` alaninda sunucu, kullanici adi ve parola; `Jwt:Secret` alaninda da imzalama anahtari var. Bu bilgiler kaynak kod deposunda tutulmamalidir.

Risk:
- Repository paylasilirsa veritabani ve token imzalama anahtari aciga cikar.
- JWT secret degismeden kalirsa uretilmis tokenlarin guvenilirligi zayiflar.

Oneri:
- Bu degerleri hemen rotate edin: DB parolasi ve JWT secret yenilenmeli.
- `appsettings.json` icinden gercek secret'lar kaldirilmali; local icin `User Secrets`, production icin environment variable / secret manager kullanilmali.
- `appsettings.json` sadece bos veya dummy placeholder degerler icermeli.

### 2. Bazi entity'lerin explicit EF configuration'i yok

DbContext'te 31 entity DbSet olarak tanimli. Configuration taramasinda su entity'ler icin ayri `IEntityTypeConfiguration<T>` bulunmuyor:

- `ApplicationRole`
- `AvailabilitySlot`
- `Company`
- `Payout`
- `ReviewReply`
- `ReviewReport`
- `UserActiveSession`
- `UserLegalAgreement`
- `UserNotificationPreference`
- `UserSecurityEvent`

EF Core convention ile tablo uretebilir, fakat standart ve surdurulebilir modelleme icin her aggregate/entity icin explicit configuration onerilir.

Risk:
- String alanlari `nvarchar(max)` olarak kalabilir.
- Index eksikleri performans sorununa donusebilir.
- Delete behavior convention'a kalabilir.
- Unique constraint gerektiren alanlar guvence altina alinmayabilir.

Oneri:
- Her entity icin ayri configuration dosyasi olusturun.
- Ozellikle `AvailabilitySlot` icin `BoatId`, tarih araligi ve status indexleri; `Payout` icin `PayoutNo`, `CaptainId`, `Status`, tarih alanlari; user alt kayitlari icin `UserId` ve gerekirse composite unique indexler tanimlanmali.

### 3. Entity'lerde string status/type alanlari ile enum kullanimi karisik

Bazi alanlarda enum kullanilmis (`BoatStatus`, `TourPackageStatus`, `PaymentStatus`, `ReservationStatus`), bazi alanlarda string status/type kullaniliyor:

- `Captain.Status`, `Captain.AccountStatus`, `Captain.ApplicationType`
- `AvailabilitySlot.Status`
- `BoatFeature.Status`
- `PackageInclude.Status`
- `Notification.Channel`, `Notification.Type`, `Notification.Status`
- `SystemSetting.ValueType`
- `StoredFile.FileType`, `StoredFile.Status`

Risk:
- Yazim hatalari veritabanina gecer.
- Business rule kontrolu dagilir.
- Filtreleme ve raporlama tarafinda tutarsizlik olusur.

Oneri:
- Domain'de anlamli sabit degerler enum veya value object olarak modellenmeli.
- EF tarafinda enumlar icin bilincli karar verilmeli: string saklanacaksa `HasConversion<string>()` ve max length; int saklanacaksa migration etkisi dokumante edilmeli.

## Entity Modelleme Degerlendirmesi

Entity'ler su an dosya boyutu olarak cok buyuk degil; bu nedenle fiziksel olarak parcali class dosyalarina bolmek acil bir ihtiyac degil. Ancak modelleme acisindan bazi entity'lerde alan gruplari ayri owned type/value object olarak dusunulebilir.

### Bolunmesi Dusunulebilecek Alanlar

- `ApplicationUser`: profil, guvenlik, dogrulama, oturum ve bildirim iliskilerini tek entity uzerinde tasiyor. Iliskisel koleksiyonlar ayri entity olarak zaten var; fakat profil/dogrulama bilgileri value object veya daha net alt modellerle ayrilabilir.
- `Location`: SEO, koordinat ve icerik alanlari ayni entity icinde. `LocationSeoInfo`, `LocationCoordinateInfo`, `LocationContentInfo` gibi owned type'lar okunabilirligi artirabilir.
- `TourPackage`: fiyatlandirma, kapasite, iptal politikasi ve tur metadatasi ayni modelde. `Pricing`, `CapacityRule`, `CancellationPolicy` gibi owned type'lar uygun olabilir.
- `Payment`: tutar kirilimlari ve provider bilgileri ayrilabilir. Ornegin `PaymentAmounts` ve `ProviderInfo`.
- `StoredFile`: dosya kimligi, storage bilgisi ve iliskili entity referansi ayrilabilir.

Karar: Entity'leri sirf satir sayisi icin bolmeye gerek yok. Bolme, domain kavramlarini netlestirmek ve invariant'lari tek yerde toplamak icin yapilmali.

## Persistence / Veritabani Baglantisi

### Dogru Olanlar

- `AddDbContext<EIskeleDbContext>` ile SQL Server kullanimi merkezi DI extension icinde.
- Production config dosyasinda connection string ve JWT secret bos birakilmis; Docker Compose bunlari environment variable ile geciyor.
- Migration dosyalari mevcut ve DbContext snapshot uretilmis.

### Iyilestirilmesi Gerekenler

- `configuration.GetConnectionString("DefaultConnection")` null/bos geldiginde uygulama daha acik hata vermeli. Production'da eksik environment variable ile baslamayi erken durdurmak iyi olur.
- Health check sadece `/health` endpoint'i gibi gorunuyor; SQL Server baglantisini da kontrol eden `AddSqlServer`/EF health check eklenmeli.
- Uygulama acilisinda seed calisiyor. Rol/template seed kabul edilebilir, fakat buyudukce idempotent, log'lu ve migration stratejisinden ayrilmis hale getirilmeli.
- `TrustServerCertificate=True` gelistirme icin kullanilabilir; production connection string'de sertifika dogrulama stratejisi netlestirilmeli.

## Business / Servis Katmani

Servisler Infrastructure altinda interface implementasyonlari olarak duruyor. Bu yapi calisir, fakat servislerin bir kismi oldukca buyumus:

- `CaptainService.Admin.cs`: 375 satir
- `AdminUserService.cs`: 352 satir
- `TourPackageService.Captain.cs`: 344 satir
- `PaymentService.Admin.cs`: 332 satir
- `ReservationService.Admin.cs`: 299 satir
- `CaptainDocumentsService.cs`: 284 satir

Kismi class kullanimi dosya bolumlemesi sagliyor, fakat domain davranisi halen servislerin icinde yogunlasiyor.

Oneri:
- Okuma ve yazma use-case'leri ayrilabilir: query service / command service veya MediatR benzeri use-case handler yapisi.
- Tekrarlayan `Include` zincirleri projection tabanli sorgulara cevrilmeli. Liste ekranlarinda entity graph yuklemek yerine direkt DTO select kullanimi performansi artirir.
- Business rule'lar servis icinde dagilmak yerine domain service veya policy siniflarina tasinabilir. Ornek: rezervasyon uygunlugu, odeme komisyon hesaplari, kaptan onay akisi.
- Kritik islemlerde transaction siniri gozden gecirilmeli. Rezervasyon + odeme + bildirim gibi coklu yazim yapan akislarda explicit transaction gerekebilir.

## Standartlara Uygunluk

### Uygun

- Nullable reference types acik.
- FluentValidation Application katmaninda kayitli.
- Global exception middleware var.
- API response standardizasyonu icin Shared response tipleri kullaniliyor.
- Katmanlar proje bazinda ayrilmis.

### Eksik / Gelistirilmeli

- Tum entity'lerde consistent max length, required, index ve delete behavior tanimlari yok.
- Domain entity'leri cogunlukla anemic model; davranis ve invariant'lar servislerde kalmis.
- `CreatedBy`, `UpdatedBy`, `DeletedBy` alanlari merkezi SaveChanges icinde doldurulmuyor; sadece tarih alanlari set ediliyor.
- Soft delete global filter tum soft deletable entity'lere otomatik uygulanmiyor; tek tek configuration ile uygulanmis. Yeni entity eklenince unutulma riski var.
- Identity password policy cok gevsek: digit/lowercase/uppercase/non-alphanumeric kapali ve minimum length 6.
- `Program.cs` icinde JWT config okuma alanlarinda null guvenligi zayif; build sirasinda CS8602 uyarilari goruldu.

## Oncelikli Aksiyon Listesi

1. `appsettings.json` icindeki DB connection string ve JWT secret'i kaldirin, mevcut secret'lari rotate edin.
2. Explicit configuration'i olmayan entity'ler icin configuration dosyalari ekleyin.
3. String status/type alanlarini enum/value object standardina cekin.
4. Para ve oran alanlarini tek tek kontrol edin; tum decimal alanlarda precision tanimlayin (`Payment.GrossTourAmount`, `DepositAmount`, `RemainingAmount`, `ServiceFeeAmount`, `RefundedAmount`, `Payout.Amount` vb.).
5. SQL Server health check ekleyin ve bos connection string/JWT secret icin startup validation yapin.
6. `CreatedBy`, `UpdatedBy`, `DeletedBy` alanlarini current user context ile merkezi dolduracak bir audit mekanizmasi ekleyin.
7. Soft delete query filter'larini merkezi modelBuilder extension ile otomatik uygulamayi degerlendirin.
8. Buyuyen servisleri use-case bazli command/query siniflarina veya daha kucuk application service'lere ayirin.
9. Kritik akislara transaction stratejisi ekleyin.
10. Password policy ve auth ayarlarini production beklentilerine gore sertlestirin.

## Derleme / Dogrulama Notu

`dotnet build EIskele.sln --no-restore` calistirildi. Domain, Application, Persistence ve Infrastructure projeleri derleme asamasini gecti; Api projesinde kopyalama adiminda build tamamlanamadi. Sebep kod hatasi degil, `EIskele.Api (PID 17596)` ve Visual Studio `devenv (PID 6224)` sureclerinin `bin/Debug/net9.0` altindaki DLL/PDB dosyalarini kilitlemesi.

Build sirasinda ayrica `Program.cs` icin iki nullable warning goruldu:

- `Program.cs(87,29)`: olasi null basvuru
- `Program.cs(91,31)`: olasi null basvuru

Bu warning'ler OpenAPI security components/requirements kurgusunda null guard ihtiyacina isaret ediyor.

## Sonuc

Backend temeli dogru yonde ve buyumeye acik. En kritik problem mimari degil, guvenlik ve persistence standardizasyonu. Entity'leri fiziksel olarak hemen bolmek yerine, once EF configuration eksiklerini tamamlamak, status/type alanlarini standardize etmek, secret yonetimini duzeltmek ve buyuyen servisleri use-case odakli sadeleştirmek daha yuksek fayda saglar.
