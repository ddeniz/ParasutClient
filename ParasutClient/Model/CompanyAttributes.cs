/* 
 * Paraşüt - API V4
 *
 * # GİRİŞ  ## API Hakkında  Paraşüt API'yi kullanmak veya görüşlerinizi bizimle paylaşmak isterseniz lütfen bizimle destek@parasut.com adresi üzerinden iletişime geçiniz.  API'yi kullanarak Paraşüt verilerine ulaşabilir ve kendi yazdığınız uygulamalar ile entegre edebilirsiniz. API vasıtasıyla Paraşüt Web arayüzü ile yapılan hemen her işlemi gerçekleştirebilirsiniz.   - API geliştirmesinde çoğunlukla JSONAPI (http://jsonapi.org/) standartlarına uymaya çalıştık.  - Dökümantasyon oluşturulmasında ise OpenAPI-Swagger 2.0 kullandık.  - API hizmetimizin `BASE_URL`i `https://api.parasut.com` şeklindedir.  - V4 endpointlerine ulaşmak için `https://api.parasut.com/v4` şeklinde kullanabilirsiniz.  ## Genel Bilgiler  - API metodlarına erişmek için baz URL olarak `https://api.parasut.com/v4/firma_no` adresi kullanılır.   - Bu yapıda kullanılan `firma_no` parametresi bilgisine erişilmek istenin firmanın Paraşüt veritabanındaki kayıt numarasıdır.   - Örneğin 115 numaralı firmanın müşteri/tedarikçi listesine erişmek için `https://api.parasut.com/v4/115/contacts` adresi kullanılır. - İstekleri gönderirken `Content-Type` header'ı olarak `application/json` veya `application/vnd.api+json` göndermelisiniz. - Yeni bir kayıt oluştururken **ilgili** kaydın `ID` parametresini boş göndermeli veya hiç göndermemelisiniz.   - Örnek: Satış faturası oluştururken `data->id` boş olmalı, ama `relationships->contact->data->id` dolu olmalı, çünkü gönderdiğiniz müşterinizin ID'si daha önceden elinizde bulunmalıdır. Aynı şekilde `relationships->details->data` içerisinde tanımladığınız ID'ler de boş olmalı, çünkü henüz fatura kalemi yaratmadınız. - API endpointlerine ulaşmak için, aldığınız `access_token`'ı sorgulara `Authorization` header'ı olarak `Bearer access_token` şeklinde göndermelisiniz. - Dakikada 60 adet istek gönderebilirsiniz.  # Authentication  <!- - ReDoc-Inject: <security-definitions> - ->  Paraşüt API kimlik doğrulama için oAuth2 kullanmaktadır. Bu protokolü destekleyen istemci kütüphanelerini kullanarak oturum açabilir ve API'yi kullanabilirsiniz.  Gerekli CLIENT_ID, CLIENT_SECRET ve REDIRECT_URL bilgilerini almak için destek@parasut.com adresine mail atabilirsiniz.  Kimlik doğrulama işleminin başarılı olması durumunda bir adet kimlik jetonu (authentication token) ve bir adet de yenileme jetonu (refresh token) gönderilecektir. Kimlik jetonu 2 saat süreyle geçerlidir ve her istekte http başlık bilgilerinin içerisinde gönderilmelidir. Bu sürenin sonunda kimlik jetonu geçerliliğini yitirecektir ve yenileme jetonu kullanılarak tekrar üretilmesi gerekmektedir.  ## access_token almak:  access_token almanız için iki farklı seçenek bulunmaktadır.  Kullanım şeklinize bağlı olarak iki yöntemden birini tercih etmelisiniz.  ### 1. grant_type=authorization_code  Bu yöntemi kullanabilmek için öncelikle aşağıda belirtildiği gibi kullanıcıyı başarılı authentication işleminin ardından yönlendirmek istediğiniz `REDIRECT_URL`'i bize ulaşarak kayıt ettirmeniz gerekmektedir. `REDIRECT_URL` varsayılan olarak `urn:ietf:wg:oauth:2.0:oob` gelmektedir.  Size özel bir REDIRECT_URL tanımlamak isterseniz destek@parasut.com adresine mail atabilirsiniz.  1. Kullanıcıyı şu adrese yönlendirin:    ```   BASE_URL/oauth/authorize?client_id=CLIENT_ID&redirect_uri=REDIRECT_URL&response_type=code   ```  2. Oturum açmışsa ve uygulamayı kabul ederse, kullanıcı sizin tanımladığınız REDIRECT_URL'e şu şekilde gelmesi gerekiyor:   `REDIRECT_URL?code=xxxxxxx`  3. Burada size gelen \"code\" parametresi ile access token almalısınız.  ```bash curl -F grant_type=authorization_code \\ -F client_id=CLIENT_ID \\ -F client_secret=CLIENT_SECRET \\ -F code=RETURNED_CODE \\ -F redirect_uri=REDIRECT_URL \\ -X POST BASE_URL/oauth/token ```  ### 2. grant_type=password  E-posta ve şifre ile access_token almanız için aşağıdaki istekte size özel alanları doldurarak POST isteği atmanız gerekmektedir.  ```bash curl -F grant_type=password \\ -F client_id=CLIENT_ID \\ -F client_secret=CLIENT_SECRET \\ -F username=YOUREMAIL \\ -F password=YOURPASSWORD \\ -F redirect_uri=urn:ietf:wg:oauth:2.0:oob \\ -X POST BASE_URL/oauth/token ```  ### Sonuç  Her iki yöntem sonucunda size aşağıdaki gibi bir sonuç dönecektir:  ```json {  \"access_token\": \"XYZXYZXYZ\",  \"token_type\": \"bearer\",  \"expires_in\": 7200,  \"refresh_token\": \"ABCABCABC\" } ```  Burada dönen `access_token`'ı API endpointlerine ulaşmak için gönderdiğiniz sorgulara `Authorization` header'ı olarak `Bearer XYZXYZXYZ` şeklinde eklemeniz gerekiyor.   #### Refresh token ile yeni access_token alma örneği:  `access_token` geçerliliğini 2 saat içerisinde yitirdiği için `refresh_token` ile yeni token alabilirsiniz.  ```bash curl -F grant_type=refresh_token \\ -F client_id=CLIENT_ID \\ -F client_secret=CLIENT_SECRET \\ -F refresh_token=REFRESH_TOKEN \\ -X POST BASE_URL/oauth/token ```  `refresh_token` ile yeni bir `access_token` alırken aynı zamanda yeni bir `refresh_token` da almaktasınız. Dolayısıyla, daha sonra yeniden bir `access_token` alma isteğinizde size dönen yeni `refresh_token`ı kullanmalısınız.  # SIK KULLANILAN İŞLEMLER  ## Kullanıcı Bilgisi  `access_token` aldığınız kullanıcının genel bilgilerini görmek için [/me](/#operation/showMe) adresini kullanabilirsiniz.  ## Satış Faturası Oluşturma  Satış faturası oluşturmak için bir müşteri (`contact`) `id`'si ve bir veya birden fazla ürün (`product`) `id`'sine ihtiyacınız vardır.  ### Müşteri  ##### Yeni bir müşteri ile  Eğer ihtiyaç duyduğunuz müşteri bilgisi henüz yoksa, öncelikle müşteri oluşturmanız gereklidir. Bunun için [Müşteri oluşturma](/#operation/createContact) endpoint'ini kullanmalısınız. Başarılı bir şekilde müşteri oluşturulursa size dönecek olan yanıt ihtiyaç duyacağınız müşteri `id`'sini içerir.  ##### Mevcut bir müşteri ile  Eğer daha önceden zaten oluşturduğunuz bir müşteri ile ilişkili bir satış faturası oluşturacaksanız öncelikle o müşterinin `id`'sini öğrenmeniz gerekir. Bunun için [Müşteri listesi](/#operation/listContacts) endpoint'ini kullanabilirsiniz. Müşteri listesi endpoint'i isim, e-posta, vergi numarası gibi çeşitli filtreleri destekler. Bunları kullanarak aradığınız müşteriyi bulabilirsiniz.  ### Ürün  ##### Yeni bir ürün ile  Eğer ihtiyaç duyduğunuz ürün bilgisi henüz yoksa, öncelikle ürün oluşturmanız gereklidir. Bunun için [Ürün oluşturma](/#operation/createProduct) endpoint'ini kullanmalısınız. Başarılı bir şekilde ürün oluşturulursa size dönecek olan yanıt ihtiyaç duyacağınız ürün `id`'sini içerir.  ##### Mevcut bir ürün ile  Eğer daha önceden oluşturduğunuz bir ürünü kullanarak bir satış faturası oluşturacaksanız öncelikle o ürünün `id`'sini öğrenmeniz gerekir. Bunun için [Ürün listesi](/#operation/listProducts) endpoint'ini kullanabilirsiniz. Ürün listesi endpoint'i isim, kod gibi çeşitli filtreleri destekler. Bunları kullanarak aradığınız ürünü bulabilirsiniz.  - --  İhtiyaç duyduğunuz müşteri ve ürün `id`'lerini aldıktan sonra [Satış Faturası Oluşturma](/#operation/createSalesInvoice) endpoint'i ile satış faturası oluşturabilirsiniz. Endpoint'in tanımında sağ tarafta beklediğimiz veri şekli bulunmaktadır, aşağıdaki bilgileri verinin şekli ile kıyaslamak daha açıklayıcı olabilir.  Dikkat edilecek noktalar: * `relationships` altındaki `contact`'te bulunan `id` alanına müşteri `id`'sini girmeniz gereklidir. * `relationships` altındaki `details` kısmı bir listedir (`array`) ve fatura kalemlerini temsil eder. Bu listenin her elemanının ilişkili olduğu bir ürün vardır. Yani `details` altındaki her elemanın kendine ait bir `relationships` kısmı mevcuttur. Buradaki `product` `id` alanı üstteki ürün adımlarında elde ettiğiniz `id`'yi koymanız gereken yerdir.  ## Satış Faturasına Tahsilat Ekleme  [Tahsilat ekleme](/#operation/paySalesInvoice) kısmındaki ilgili alanları doldurarak satış faturasına tahsilat ekleyebilirsiniz.  ## Satış Faturasının Tahsilatını Silme  Bir satış faturasının tahsilatını silmek aslında o tahsilatı oluşturan para akış işlemini silmek demektir. Bir işlemi silmeden önce o işlemin `id`'sine ihtiyacınız vardır.  Bir satış faturasına ait tahsilatları almak için [Satış faturası bilgilerini alma (show)](/#operation/showSalesInvoice) endpoint'ine istek atarken `?include=payments` parametresini de eklemelisiniz. Bu size satış faturası bilgilerine ilave olarak tahsilatları da verir.  Tahsilatlar ile birlikte o tahsilatları oluşturan işlemleri de almak için yine aynı endpoint'e `?include=payments.transaction` parametresini ekleyerek istek yapmanız gerekir. Bu size hem satış faturası bilgilerini, hem tahsilat bilgilerini hem de tahsilatı oluşturan işlemlerin bilgilerini verir.  `?include=payments.transaction` parametresini kullanarak yaptığınız istek ile işlem (`transaction`) `id`'sini aldıktan sonra [işlem silme](/#operation/deleteTransaction) endpoint'inde bu `id`'yi kullanarak silme işlemini yapabilirsiniz.  ## Satış Faturası Resmileştirme  Oluşturduğunuz bir satış faturası varsa onu e-Arşiv veya e-Fatura olarak resmileştirmek için aşağıdakileri yapmanız gereklidir.  1. Öncelikle müşterinizin e-Fatura kullanıcısı olup olmadığını öğrenmelisiniz. Bunun için müşterinizin e-Fatura gelen kutusu olup olmadığına bakmak gereklidir. [e-Fatura gelen kutusu](/#operation/listEInvoiceInboxes) endpoint'ine müşterinin vkn'sini kullanarak bir istek yaptığınızda eğer bir gelen kutusu olduğuna dair yanıt alıyorsanız müşteri e-Fatura kullanıcısıdır. Müşteri e-Fatura kullanıcısı ise resmileştirme için e-Fatura oluşturmak, e-Fatura kullanıcısı değilse e-Arşiv oluşturmak gereklidir. Oluşturduğunuz e-Fatura, e-Arşiv ve e-Smm’nin düzenleme tarihi e-Fatura’ya geçiş sağladığınız aktivasyon tarihinden sonra olmalıdır. Aynı zamanda oluşturduğunuz e-Fatura’nın düzenleme tarihi alıcının etiketi kullanmaya başladığı tarihten de önce olamaz. Alıcının etiketi kullanmaya başladığı tarihi e-Fatura gelen kutusunu çekerek görüntüleyebilirsiniz. 2. e-Fatura veya e-Arşiv oluşturma:    * Önceki adımda müşterinin e-Fatura kullanıcısı olduğu öğrenildiyse,  [e-Fatura oluşturma endpoint'i](/#operation/createEInvoice) kullanılarak e-Fatura oluşturmak gereklidir.    * Önceki adımda müşterinin e-Arşiv kullanıcısı olduğu öğrenildiyse,  [e-Arşiv oluşturma endpoint'i](/#operation/createEArchive) kullanılarak e-Arşiv oluşturmak gereklidir.     e-Fatura ve e-Arşiv oluşturma işlemi synchronous değildir. Yani istek arka planda yerine getirilir. Bu yüzden e-Fatura veya e-Arşiv oluşturma endpoint'leri cevap olarak oluşturma işleminin durumunu takip edebileceğiniz bir işlem `id`'si döner. Bu işlem `id`'sini [sorgulama](/#tag/TrackableJobs) endpoint'inde belirli aralıklarla kullanıp oluşturma işleminin durumunu takip etmeniz gerekmektedir. İşlem durumu ile ilgili aşağıdaki yanıtları alabilirsiniz:    * `status: \"pending\"` işlemin sırada olduğunu, henüz başlamadığını gösterir.    * `status: \"running\"` işlemin yapılmakta olduğunu ancak henüz sonuçlanmadığını gösterir.    * `status: \"error\"` işlemde bir hata olduğu anlamına gelir. Dönen yanıtta hata mesajını inceleyebilirsiniz.    * `status: \"done\"` işlemin başarılı bir şekilde sonuçlandığını gösterir. 4. e-Fatura / e-Arşiv işleminin başarılı bir şekilde sonuçlandığını gördükten sonra e-Fatura / e-Arşiv bilgilerini almak için [Satış faturası bilgilerini alma (show)](/#operation/showSalesInvoice) endpoint'ine `?include=active_e_document` parametresi ile istek yapmanız gerekmektedir. Buradan sıradaki adımda ihtiyaç duyacağınız e-Fatura / e-Arşiv `id`'lerini ve başka bilgileri de alabilirsiniz. 5. e-Fatura / e-Arşiv başarılı bir resmileştirildikten sonra müşterilerinize PDF olarak göndermek isteyebilirsiniz. Bunun için:    * e-Arşiv için, 4. adımda elde edeceğiniz e-Arşiv `id`'sini kullanarak [e-Arşiv PDF](/#operation/showEArchivePdf) endpoint'ine istek atabilirsiniz. Bu endpoint PDF henüz yoksa boş bir yanıt ile birlikte 204 döner. Yani 204 almayana kadar belirli aralıklarla bu endpoint'e istek yapmanız gerekmektedir. Geçerli yanıt aldığınızda size dönecek olan PDF URL 1 saat için geçerlidir. Bu yüzden bu linki direk olarak müşterinizle **paylaşmamalısınız**. İndirip müşterinize kendiniz göndermelisiniz.    * e-Arşiv için anlatılan senaryonun aynısı e-Fatura için de geçerlidir. Tek farklı kısım isteği yapacağınız endpoint'dir: [e-Fatura PDF](/#operation/showEInvoicePdf)  ## İrsaliye Oluşturma  İrsaliye oluşturmak için bir müşteri/tedarikçi (`contact`) `id`'si ve bir veya birden fazla ürün (`product`) `id`'sine ihtiyacınız vardır.  ### Müşteri/Tedarikçi  ##### Yeni bir müşteri/tedarikçi ile  Eğer ihtiyaç duyduğunuz müşteri/tedarikçi bilgisi henüz yoksa, öncelikle müşteri/tedarikçi oluşturmanız gereklidir. Bunun için [Müşteri/Tedarikçi oluşturma](/#operation/createContact) endpoint'ini kullanmalısınız. Başarılı bir şekilde müşteri/tedarikçi oluşturulursa size dönecek olan yanıt ihtiyaç duyacağınız müşteri/tedarikçi `id`'sini içerir.  ##### Mevcut bir müşteri/tedarikçi ile  Eğer daha önceden zaten oluşturduğunuz bir müşteri/tedarikçi ile ilişkili bir irsaliye oluşturacaksanız öncelikle o müşteri/tedarikçinin `id`'sini öğrenmeniz gerekir. Bunun için [Müşteri/tedarikçi listesi](/#operation/listContacts) endpoint'ini kullanabilirsiniz. Müşteri/tedarikçi listesi endpoint'i isim, e-posta, vergi numarası gibi çeşitli filtreleri destekler. Bunları kullanarak aradığınız müşteri/tedarikçiyi bulabilirsiniz.  ### Ürün  ##### Yeni bir ürün ile  Eğer ihtiyaç duyduğunuz ürün bilgisi henüz yoksa, öncelikle ürün oluşturmanız gereklidir. Bunun için [Ürün oluşturma](/#operation/createProduct) endpoint'ini kullanmalısınız. Başarılı bir şekilde ürün oluşturulursa size dönecek olan yanıt ihtiyaç duyacağınız ürün `id`'sini içerir.  ##### Mevcut bir ürün ile  Eğer daha önceden oluşturduğunuz bir ürünü kullanarak bir irsaliye oluşturacaksanız öncelikle o ürünün `id`'sini öğrenmeniz gerekir. Bunun için [Ürün listesi](/#operation/listProducts) endpoint'ini kullanabilirsiniz. Ürün listesi endpoint'i isim, kod gibi çeşitli filtreleri destekler. Bunları kullanarak aradığınız ürünü bulabilirsiniz.  - --  İhtiyaç duyduğunuz müşteri/tedarikçi ve ürün `id`'lerini aldıktan sonra [İrsaliye Oluşturma](/#operation/createShipmentDocument) endpoint'i ile irsaliye oluşturabilirsiniz. Endpoint'in tanımında sağ tarafta beklediğimiz veri şekli bulunmaktadır, aşağıdaki bilgileri verinin şekli ile kıyaslamak daha açıklayıcı olabilir.  Dikkat edilecek noktalar: * `relationships` altındaki `contact`'te bulunan `id` alanına müşteri/tedarikçi `id`'sini girmeniz gereklidir. * `relationships` altındaki `stock_movements` kısmı bir listedir (`array`) ve irsaliye kalemlerini temsil eder. Bu listenin her elemanının ilişkili olduğu bir ürün vardır. Yani `stock_movements` altındaki her elemanın kendine ait bir `relationships` kısmı mevcuttur. Buradaki `product` `id` alanı üstteki ürün adımlarında elde ettiğiniz `id`'yi koymanız gereken yerdir. 
 *
 * OpenAPI spec version: 4.0.0
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using SwaggerDateConverter = ParasutClient.Client.SwaggerDateConverter;

namespace ParasutClient.Model
{
    /// <summary>
    /// CompanyAttributes
    /// </summary>
    [DataContract]
    public partial class CompanyAttributes :  IEquatable<CompanyAttributes>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyAttributes" /> class.
        /// </summary>
        /// <param name="logo">logo.</param>
        public CompanyAttributes(Object logo = default(Object))
        {
            this.Logo = logo;
        }
        
        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [DataMember(Name="name", EmitDefaultValue=false)]
        public string Name { get; private set; }

        /// <summary>
        /// Gets or Sets ValidUntil
        /// </summary>
        [DataMember(Name="valid_until", EmitDefaultValue=false)]
        [JsonConverter(typeof(SwaggerDateConverter))]
        public DateTime? ValidUntil { get; private set; }

        /// <summary>
        /// Gets or Sets SubscriptionStatus
        /// </summary>
        [DataMember(Name="subscription_status", EmitDefaultValue=false)]
        public string SubscriptionStatus { get; private set; }

        /// <summary>
        /// Gets or Sets TrialExpirationAt
        /// </summary>
        [DataMember(Name="trial_expiration_at", EmitDefaultValue=false)]
        public DateTime? TrialExpirationAt { get; private set; }

        /// <summary>
        /// Gets or Sets AllowedInspectionAt
        /// </summary>
        [DataMember(Name="allowed_inspection_at", EmitDefaultValue=false)]
        public DateTime? AllowedInspectionAt { get; private set; }

        /// <summary>
        /// Gets or Sets AppUrl
        /// </summary>
        [DataMember(Name="app_url", EmitDefaultValue=false)]
        public string AppUrl { get; private set; }

        /// <summary>
        /// Gets or Sets LegalName
        /// </summary>
        [DataMember(Name="legal_name", EmitDefaultValue=false)]
        public string LegalName { get; private set; }

        /// <summary>
        /// Gets or Sets OccupationField
        /// </summary>
        [DataMember(Name="occupation_field", EmitDefaultValue=false)]
        public string OccupationField { get; private set; }

        /// <summary>
        /// Gets or Sets District
        /// </summary>
        [DataMember(Name="district", EmitDefaultValue=false)]
        public string District { get; private set; }

        /// <summary>
        /// Gets or Sets City
        /// </summary>
        [DataMember(Name="city", EmitDefaultValue=false)]
        public string City { get; private set; }

        /// <summary>
        /// Gets or Sets TaxOffice
        /// </summary>
        [DataMember(Name="tax_office", EmitDefaultValue=false)]
        public string TaxOffice { get; private set; }

        /// <summary>
        /// Gets or Sets TaxNumber
        /// </summary>
        [DataMember(Name="tax_number", EmitDefaultValue=false)]
        public string TaxNumber { get; private set; }

        /// <summary>
        /// Gets or Sets MersisNo
        /// </summary>
        [DataMember(Name="mersis_no", EmitDefaultValue=false)]
        public string MersisNo { get; private set; }

        /// <summary>
        /// Gets or Sets TotalUnusedBonusMonths
        /// </summary>
        [DataMember(Name="total_unused_bonus_months", EmitDefaultValue=false)]
        public decimal? TotalUnusedBonusMonths { get; private set; }

        /// <summary>
        /// Gets or Sets SubscriptionStartedAt
        /// </summary>
        [DataMember(Name="subscription_started_at", EmitDefaultValue=false)]
        [JsonConverter(typeof(SwaggerDateConverter))]
        public DateTime? SubscriptionStartedAt { get; private set; }

        /// <summary>
        /// Gets or Sets SubscriptionRenewedAt
        /// </summary>
        [DataMember(Name="subscription_renewed_at", EmitDefaultValue=false)]
        [JsonConverter(typeof(SwaggerDateConverter))]
        public DateTime? SubscriptionRenewedAt { get; private set; }

        /// <summary>
        /// Gets or Sets SubscriptionValue
        /// </summary>
        [DataMember(Name="subscription_value", EmitDefaultValue=false)]
        public decimal? SubscriptionValue { get; private set; }

        /// <summary>
        /// Gets or Sets PrimaryJob
        /// </summary>
        [DataMember(Name="primary_job", EmitDefaultValue=false)]
        public string PrimaryJob { get; private set; }

        /// <summary>
        /// Gets or Sets IsActive
        /// </summary>
        [DataMember(Name="is_active", EmitDefaultValue=false)]
        public bool? IsActive { get; private set; }

        /// <summary>
        /// Gets or Sets Accessible
        /// </summary>
        [DataMember(Name="accessible", EmitDefaultValue=false)]
        public bool? Accessible { get; private set; }

        /// <summary>
        /// Gets or Sets Inspectable
        /// </summary>
        [DataMember(Name="inspectable", EmitDefaultValue=false)]
        public bool? Inspectable { get; private set; }

        /// <summary>
        /// Gets or Sets IsInGracePeriod
        /// </summary>
        [DataMember(Name="is_in_grace_period", EmitDefaultValue=false)]
        public bool? IsInGracePeriod { get; private set; }

        /// <summary>
        /// Gets or Sets SubscriptionStatusForAnalytics
        /// </summary>
        [DataMember(Name="subscription_status_for_analytics", EmitDefaultValue=false)]
        public string SubscriptionStatusForAnalytics { get; private set; }

        /// <summary>
        /// Gets or Sets EndOfGracePeriodAt
        /// </summary>
        [DataMember(Name="end_of_grace_period_at", EmitDefaultValue=false)]
        [JsonConverter(typeof(SwaggerDateConverter))]
        public DateTime? EndOfGracePeriodAt { get; private set; }

        /// <summary>
        /// Gets or Sets InventoryEnabled
        /// </summary>
        [DataMember(Name="inventory_enabled", EmitDefaultValue=false)]
        public bool? InventoryEnabled { get; private set; }

        /// <summary>
        /// Gets or Sets IsInTrialPeriod
        /// </summary>
        [DataMember(Name="is_in_trial_period", EmitDefaultValue=false)]
        public bool? IsInTrialPeriod { get; private set; }

        /// <summary>
        /// Gets or Sets HasIyzicoIntegration
        /// </summary>
        [DataMember(Name="has_iyzico_integration", EmitDefaultValue=false)]
        public bool? HasIyzicoIntegration { get; private set; }

        /// <summary>
        /// Gets or Sets HasActiveSubscription
        /// </summary>
        [DataMember(Name="has_active_subscription", EmitDefaultValue=false)]
        public bool? HasActiveSubscription { get; private set; }

        /// <summary>
        /// Gets or Sets Logo
        /// </summary>
        [DataMember(Name="logo", EmitDefaultValue=false)]
        public Object Logo { get; set; }

        /// <summary>
        /// Gets or Sets SubscriptionInfoText
        /// </summary>
        [DataMember(Name="subscription_info_text", EmitDefaultValue=false)]
        public string SubscriptionInfoText { get; private set; }

        /// <summary>
        /// Gets or Sets SubscriptionInfoTitle
        /// </summary>
        [DataMember(Name="subscription_info_title", EmitDefaultValue=false)]
        public string SubscriptionInfoTitle { get; private set; }

        /// <summary>
        /// Gets or Sets SubscriptionUrl
        /// </summary>
        [DataMember(Name="subscription_url", EmitDefaultValue=false)]
        public string SubscriptionUrl { get; private set; }

        /// <summary>
        /// Gets or Sets ReferralUrl
        /// </summary>
        [DataMember(Name="referral_url", EmitDefaultValue=false)]
        public string ReferralUrl { get; private set; }

        /// <summary>
        /// Gets or Sets SubscriptionPlanDuration
        /// </summary>
        [DataMember(Name="subscription_plan_duration", EmitDefaultValue=false)]
        public decimal? SubscriptionPlanDuration { get; private set; }

        /// <summary>
        /// Gets or Sets SubscriptionPlanName
        /// </summary>
        [DataMember(Name="subscription_plan_name", EmitDefaultValue=false)]
        public string SubscriptionPlanName { get; private set; }

        /// <summary>
        /// Gets or Sets EInvoicingActivatedAt
        /// </summary>
        [DataMember(Name="e_invoicing_activated_at", EmitDefaultValue=false)]
        public DateTime? EInvoicingActivatedAt { get; private set; }

        /// <summary>
        /// Gets or Sets HasSelectedPlan
        /// </summary>
        [DataMember(Name="has_selected_plan", EmitDefaultValue=false)]
        public bool? HasSelectedPlan { get; private set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class CompanyAttributes {\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  ValidUntil: ").Append(ValidUntil).Append("\n");
            sb.Append("  SubscriptionStatus: ").Append(SubscriptionStatus).Append("\n");
            sb.Append("  TrialExpirationAt: ").Append(TrialExpirationAt).Append("\n");
            sb.Append("  AllowedInspectionAt: ").Append(AllowedInspectionAt).Append("\n");
            sb.Append("  AppUrl: ").Append(AppUrl).Append("\n");
            sb.Append("  LegalName: ").Append(LegalName).Append("\n");
            sb.Append("  OccupationField: ").Append(OccupationField).Append("\n");
            sb.Append("  District: ").Append(District).Append("\n");
            sb.Append("  City: ").Append(City).Append("\n");
            sb.Append("  TaxOffice: ").Append(TaxOffice).Append("\n");
            sb.Append("  TaxNumber: ").Append(TaxNumber).Append("\n");
            sb.Append("  MersisNo: ").Append(MersisNo).Append("\n");
            sb.Append("  TotalUnusedBonusMonths: ").Append(TotalUnusedBonusMonths).Append("\n");
            sb.Append("  SubscriptionStartedAt: ").Append(SubscriptionStartedAt).Append("\n");
            sb.Append("  SubscriptionRenewedAt: ").Append(SubscriptionRenewedAt).Append("\n");
            sb.Append("  SubscriptionValue: ").Append(SubscriptionValue).Append("\n");
            sb.Append("  PrimaryJob: ").Append(PrimaryJob).Append("\n");
            sb.Append("  IsActive: ").Append(IsActive).Append("\n");
            sb.Append("  Accessible: ").Append(Accessible).Append("\n");
            sb.Append("  Inspectable: ").Append(Inspectable).Append("\n");
            sb.Append("  IsInGracePeriod: ").Append(IsInGracePeriod).Append("\n");
            sb.Append("  SubscriptionStatusForAnalytics: ").Append(SubscriptionStatusForAnalytics).Append("\n");
            sb.Append("  EndOfGracePeriodAt: ").Append(EndOfGracePeriodAt).Append("\n");
            sb.Append("  InventoryEnabled: ").Append(InventoryEnabled).Append("\n");
            sb.Append("  IsInTrialPeriod: ").Append(IsInTrialPeriod).Append("\n");
            sb.Append("  HasIyzicoIntegration: ").Append(HasIyzicoIntegration).Append("\n");
            sb.Append("  HasActiveSubscription: ").Append(HasActiveSubscription).Append("\n");
            sb.Append("  Logo: ").Append(Logo).Append("\n");
            sb.Append("  SubscriptionInfoText: ").Append(SubscriptionInfoText).Append("\n");
            sb.Append("  SubscriptionInfoTitle: ").Append(SubscriptionInfoTitle).Append("\n");
            sb.Append("  SubscriptionUrl: ").Append(SubscriptionUrl).Append("\n");
            sb.Append("  ReferralUrl: ").Append(ReferralUrl).Append("\n");
            sb.Append("  SubscriptionPlanDuration: ").Append(SubscriptionPlanDuration).Append("\n");
            sb.Append("  SubscriptionPlanName: ").Append(SubscriptionPlanName).Append("\n");
            sb.Append("  EInvoicingActivatedAt: ").Append(EInvoicingActivatedAt).Append("\n");
            sb.Append("  HasSelectedPlan: ").Append(HasSelectedPlan).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as CompanyAttributes);
        }

        /// <summary>
        /// Returns true if CompanyAttributes instances are equal
        /// </summary>
        /// <param name="input">Instance of CompanyAttributes to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(CompanyAttributes input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Name == input.Name ||
                    (this.Name != null &&
                    this.Name.Equals(input.Name))
                ) && 
                (
                    this.ValidUntil == input.ValidUntil ||
                    (this.ValidUntil != null &&
                    this.ValidUntil.Equals(input.ValidUntil))
                ) && 
                (
                    this.SubscriptionStatus == input.SubscriptionStatus ||
                    (this.SubscriptionStatus != null &&
                    this.SubscriptionStatus.Equals(input.SubscriptionStatus))
                ) && 
                (
                    this.TrialExpirationAt == input.TrialExpirationAt ||
                    (this.TrialExpirationAt != null &&
                    this.TrialExpirationAt.Equals(input.TrialExpirationAt))
                ) && 
                (
                    this.AllowedInspectionAt == input.AllowedInspectionAt ||
                    (this.AllowedInspectionAt != null &&
                    this.AllowedInspectionAt.Equals(input.AllowedInspectionAt))
                ) && 
                (
                    this.AppUrl == input.AppUrl ||
                    (this.AppUrl != null &&
                    this.AppUrl.Equals(input.AppUrl))
                ) && 
                (
                    this.LegalName == input.LegalName ||
                    (this.LegalName != null &&
                    this.LegalName.Equals(input.LegalName))
                ) && 
                (
                    this.OccupationField == input.OccupationField ||
                    (this.OccupationField != null &&
                    this.OccupationField.Equals(input.OccupationField))
                ) && 
                (
                    this.District == input.District ||
                    (this.District != null &&
                    this.District.Equals(input.District))
                ) && 
                (
                    this.City == input.City ||
                    (this.City != null &&
                    this.City.Equals(input.City))
                ) && 
                (
                    this.TaxOffice == input.TaxOffice ||
                    (this.TaxOffice != null &&
                    this.TaxOffice.Equals(input.TaxOffice))
                ) && 
                (
                    this.TaxNumber == input.TaxNumber ||
                    (this.TaxNumber != null &&
                    this.TaxNumber.Equals(input.TaxNumber))
                ) && 
                (
                    this.MersisNo == input.MersisNo ||
                    (this.MersisNo != null &&
                    this.MersisNo.Equals(input.MersisNo))
                ) && 
                (
                    this.TotalUnusedBonusMonths == input.TotalUnusedBonusMonths ||
                    (this.TotalUnusedBonusMonths != null &&
                    this.TotalUnusedBonusMonths.Equals(input.TotalUnusedBonusMonths))
                ) && 
                (
                    this.SubscriptionStartedAt == input.SubscriptionStartedAt ||
                    (this.SubscriptionStartedAt != null &&
                    this.SubscriptionStartedAt.Equals(input.SubscriptionStartedAt))
                ) && 
                (
                    this.SubscriptionRenewedAt == input.SubscriptionRenewedAt ||
                    (this.SubscriptionRenewedAt != null &&
                    this.SubscriptionRenewedAt.Equals(input.SubscriptionRenewedAt))
                ) && 
                (
                    this.SubscriptionValue == input.SubscriptionValue ||
                    (this.SubscriptionValue != null &&
                    this.SubscriptionValue.Equals(input.SubscriptionValue))
                ) && 
                (
                    this.PrimaryJob == input.PrimaryJob ||
                    (this.PrimaryJob != null &&
                    this.PrimaryJob.Equals(input.PrimaryJob))
                ) && 
                (
                    this.IsActive == input.IsActive ||
                    (this.IsActive != null &&
                    this.IsActive.Equals(input.IsActive))
                ) && 
                (
                    this.Accessible == input.Accessible ||
                    (this.Accessible != null &&
                    this.Accessible.Equals(input.Accessible))
                ) && 
                (
                    this.Inspectable == input.Inspectable ||
                    (this.Inspectable != null &&
                    this.Inspectable.Equals(input.Inspectable))
                ) && 
                (
                    this.IsInGracePeriod == input.IsInGracePeriod ||
                    (this.IsInGracePeriod != null &&
                    this.IsInGracePeriod.Equals(input.IsInGracePeriod))
                ) && 
                (
                    this.SubscriptionStatusForAnalytics == input.SubscriptionStatusForAnalytics ||
                    (this.SubscriptionStatusForAnalytics != null &&
                    this.SubscriptionStatusForAnalytics.Equals(input.SubscriptionStatusForAnalytics))
                ) && 
                (
                    this.EndOfGracePeriodAt == input.EndOfGracePeriodAt ||
                    (this.EndOfGracePeriodAt != null &&
                    this.EndOfGracePeriodAt.Equals(input.EndOfGracePeriodAt))
                ) && 
                (
                    this.InventoryEnabled == input.InventoryEnabled ||
                    (this.InventoryEnabled != null &&
                    this.InventoryEnabled.Equals(input.InventoryEnabled))
                ) && 
                (
                    this.IsInTrialPeriod == input.IsInTrialPeriod ||
                    (this.IsInTrialPeriod != null &&
                    this.IsInTrialPeriod.Equals(input.IsInTrialPeriod))
                ) && 
                (
                    this.HasIyzicoIntegration == input.HasIyzicoIntegration ||
                    (this.HasIyzicoIntegration != null &&
                    this.HasIyzicoIntegration.Equals(input.HasIyzicoIntegration))
                ) && 
                (
                    this.HasActiveSubscription == input.HasActiveSubscription ||
                    (this.HasActiveSubscription != null &&
                    this.HasActiveSubscription.Equals(input.HasActiveSubscription))
                ) && 
                (
                    this.Logo == input.Logo ||
                    (this.Logo != null &&
                    this.Logo.Equals(input.Logo))
                ) && 
                (
                    this.SubscriptionInfoText == input.SubscriptionInfoText ||
                    (this.SubscriptionInfoText != null &&
                    this.SubscriptionInfoText.Equals(input.SubscriptionInfoText))
                ) && 
                (
                    this.SubscriptionInfoTitle == input.SubscriptionInfoTitle ||
                    (this.SubscriptionInfoTitle != null &&
                    this.SubscriptionInfoTitle.Equals(input.SubscriptionInfoTitle))
                ) && 
                (
                    this.SubscriptionUrl == input.SubscriptionUrl ||
                    (this.SubscriptionUrl != null &&
                    this.SubscriptionUrl.Equals(input.SubscriptionUrl))
                ) && 
                (
                    this.ReferralUrl == input.ReferralUrl ||
                    (this.ReferralUrl != null &&
                    this.ReferralUrl.Equals(input.ReferralUrl))
                ) && 
                (
                    this.SubscriptionPlanDuration == input.SubscriptionPlanDuration ||
                    (this.SubscriptionPlanDuration != null &&
                    this.SubscriptionPlanDuration.Equals(input.SubscriptionPlanDuration))
                ) && 
                (
                    this.SubscriptionPlanName == input.SubscriptionPlanName ||
                    (this.SubscriptionPlanName != null &&
                    this.SubscriptionPlanName.Equals(input.SubscriptionPlanName))
                ) && 
                (
                    this.EInvoicingActivatedAt == input.EInvoicingActivatedAt ||
                    (this.EInvoicingActivatedAt != null &&
                    this.EInvoicingActivatedAt.Equals(input.EInvoicingActivatedAt))
                ) && 
                (
                    this.HasSelectedPlan == input.HasSelectedPlan ||
                    (this.HasSelectedPlan != null &&
                    this.HasSelectedPlan.Equals(input.HasSelectedPlan))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.Name != null)
                    hashCode = hashCode * 59 + this.Name.GetHashCode();
                if (this.ValidUntil != null)
                    hashCode = hashCode * 59 + this.ValidUntil.GetHashCode();
                if (this.SubscriptionStatus != null)
                    hashCode = hashCode * 59 + this.SubscriptionStatus.GetHashCode();
                if (this.TrialExpirationAt != null)
                    hashCode = hashCode * 59 + this.TrialExpirationAt.GetHashCode();
                if (this.AllowedInspectionAt != null)
                    hashCode = hashCode * 59 + this.AllowedInspectionAt.GetHashCode();
                if (this.AppUrl != null)
                    hashCode = hashCode * 59 + this.AppUrl.GetHashCode();
                if (this.LegalName != null)
                    hashCode = hashCode * 59 + this.LegalName.GetHashCode();
                if (this.OccupationField != null)
                    hashCode = hashCode * 59 + this.OccupationField.GetHashCode();
                if (this.District != null)
                    hashCode = hashCode * 59 + this.District.GetHashCode();
                if (this.City != null)
                    hashCode = hashCode * 59 + this.City.GetHashCode();
                if (this.TaxOffice != null)
                    hashCode = hashCode * 59 + this.TaxOffice.GetHashCode();
                if (this.TaxNumber != null)
                    hashCode = hashCode * 59 + this.TaxNumber.GetHashCode();
                if (this.MersisNo != null)
                    hashCode = hashCode * 59 + this.MersisNo.GetHashCode();
                if (this.TotalUnusedBonusMonths != null)
                    hashCode = hashCode * 59 + this.TotalUnusedBonusMonths.GetHashCode();
                if (this.SubscriptionStartedAt != null)
                    hashCode = hashCode * 59 + this.SubscriptionStartedAt.GetHashCode();
                if (this.SubscriptionRenewedAt != null)
                    hashCode = hashCode * 59 + this.SubscriptionRenewedAt.GetHashCode();
                if (this.SubscriptionValue != null)
                    hashCode = hashCode * 59 + this.SubscriptionValue.GetHashCode();
                if (this.PrimaryJob != null)
                    hashCode = hashCode * 59 + this.PrimaryJob.GetHashCode();
                if (this.IsActive != null)
                    hashCode = hashCode * 59 + this.IsActive.GetHashCode();
                if (this.Accessible != null)
                    hashCode = hashCode * 59 + this.Accessible.GetHashCode();
                if (this.Inspectable != null)
                    hashCode = hashCode * 59 + this.Inspectable.GetHashCode();
                if (this.IsInGracePeriod != null)
                    hashCode = hashCode * 59 + this.IsInGracePeriod.GetHashCode();
                if (this.SubscriptionStatusForAnalytics != null)
                    hashCode = hashCode * 59 + this.SubscriptionStatusForAnalytics.GetHashCode();
                if (this.EndOfGracePeriodAt != null)
                    hashCode = hashCode * 59 + this.EndOfGracePeriodAt.GetHashCode();
                if (this.InventoryEnabled != null)
                    hashCode = hashCode * 59 + this.InventoryEnabled.GetHashCode();
                if (this.IsInTrialPeriod != null)
                    hashCode = hashCode * 59 + this.IsInTrialPeriod.GetHashCode();
                if (this.HasIyzicoIntegration != null)
                    hashCode = hashCode * 59 + this.HasIyzicoIntegration.GetHashCode();
                if (this.HasActiveSubscription != null)
                    hashCode = hashCode * 59 + this.HasActiveSubscription.GetHashCode();
                if (this.Logo != null)
                    hashCode = hashCode * 59 + this.Logo.GetHashCode();
                if (this.SubscriptionInfoText != null)
                    hashCode = hashCode * 59 + this.SubscriptionInfoText.GetHashCode();
                if (this.SubscriptionInfoTitle != null)
                    hashCode = hashCode * 59 + this.SubscriptionInfoTitle.GetHashCode();
                if (this.SubscriptionUrl != null)
                    hashCode = hashCode * 59 + this.SubscriptionUrl.GetHashCode();
                if (this.ReferralUrl != null)
                    hashCode = hashCode * 59 + this.ReferralUrl.GetHashCode();
                if (this.SubscriptionPlanDuration != null)
                    hashCode = hashCode * 59 + this.SubscriptionPlanDuration.GetHashCode();
                if (this.SubscriptionPlanName != null)
                    hashCode = hashCode * 59 + this.SubscriptionPlanName.GetHashCode();
                if (this.EInvoicingActivatedAt != null)
                    hashCode = hashCode * 59 + this.EInvoicingActivatedAt.GetHashCode();
                if (this.HasSelectedPlan != null)
                    hashCode = hashCode * 59 + this.HasSelectedPlan.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}
