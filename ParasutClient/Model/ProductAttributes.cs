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
    /// ProductAttributes
    /// </summary>
    [DataContract]
    public partial class ProductAttributes :  IEquatable<ProductAttributes>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductAttributes" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected ProductAttributes() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductAttributes" /> class.
        /// </summary>
        /// <param name="code">Ürün/hizmet kodu.</param>
        /// <param name="name">Ürün/hizmet ismi (required).</param>
        /// <param name="vatRate">KDV oranı.</param>
        /// <param name="salesExciseDuty">Satış ÖTV.</param>
        /// <param name="salesExciseDutyType">Satış ÖTV tipi.</param>
        /// <param name="purchaseExciseDuty">Alış ÖTV.</param>
        /// <param name="purchaseExciseDutyType">Alış ÖTV tipi.</param>
        /// <param name="unit">Birim.</param>
        /// <param name="communicationsTaxRate">ÖİV oranı.</param>
        /// <param name="archived">archived.</param>
        /// <param name="listPrice">Satış fiyatı.</param>
        /// <param name="currency">Satış döviz.</param>
        /// <param name="buyingPrice">Alış fiyatı.</param>
        /// <param name="buyingCurrency">Alış döviz.</param>
        /// <param name="inventoryTracking">inventoryTracking.</param>
        /// <param name="initialStockCount">Başlangıç Stok Miktarı.</param>
        public ProductAttributes(string code = default(string), string name = default(string), decimal? vatRate = default(decimal?), decimal? salesExciseDuty = default(decimal?), string salesExciseDutyType = default(string), decimal? purchaseExciseDuty = default(decimal?), string purchaseExciseDutyType = default(string), string unit = default(string), decimal? communicationsTaxRate = default(decimal?), bool? archived = default(bool?), decimal? listPrice = default(decimal?), string currency = default(string), decimal? buyingPrice = default(decimal?), string buyingCurrency = default(string), bool? inventoryTracking = default(bool?), decimal? initialStockCount = default(decimal?))
        {
            // to ensure "name" is required (not null)
            if (name == null)
            {
                throw new InvalidDataException("name is a required property for ProductAttributes and cannot be null");
            }
            else
            {
                this.Name = name;
            }
            this.Code = code;
            this.VatRate = vatRate;
            this.SalesExciseDuty = salesExciseDuty;
            this.SalesExciseDutyType = salesExciseDutyType;
            this.PurchaseExciseDuty = purchaseExciseDuty;
            this.PurchaseExciseDutyType = purchaseExciseDutyType;
            this.Unit = unit;
            this.CommunicationsTaxRate = communicationsTaxRate;
            this.Archived = archived;
            this.ListPrice = listPrice;
            this.Currency = currency;
            this.BuyingPrice = buyingPrice;
            this.BuyingCurrency = buyingCurrency;
            this.InventoryTracking = inventoryTracking;
            this.InitialStockCount = initialStockCount;
        }
        
        /// <summary>
        /// Gets or Sets SalesExciseDutyCode
        /// </summary>
        [DataMember(Name="sales_excise_duty_code", EmitDefaultValue=false)]
        public string SalesExciseDutyCode { get; private set; }

        /// <summary>
        /// Gets or Sets SalesInvoiceDetailsCount
        /// </summary>
        [DataMember(Name="sales_invoice_details_count", EmitDefaultValue=false)]
        public int? SalesInvoiceDetailsCount { get; private set; }

        /// <summary>
        /// Gets or Sets PurchaseInvoiceDetailsCount
        /// </summary>
        [DataMember(Name="purchase_invoice_details_count", EmitDefaultValue=false)]
        public int? PurchaseInvoiceDetailsCount { get; private set; }

        /// <summary>
        /// Gets or Sets ListPriceInTrl
        /// </summary>
        [DataMember(Name="list_price_in_trl", EmitDefaultValue=false)]
        public decimal? ListPriceInTrl { get; private set; }

        /// <summary>
        /// Gets or Sets BuyingPriceInTrl
        /// </summary>
        [DataMember(Name="buying_price_in_trl", EmitDefaultValue=false)]
        public decimal? BuyingPriceInTrl { get; private set; }

        /// <summary>
        /// Stok Miktarı
        /// </summary>
        /// <value>Stok Miktarı</value>
        [DataMember(Name="stock_count", EmitDefaultValue=false)]
        public decimal? StockCount { get; private set; }

        /// <summary>
        /// Gets or Sets CreatedAt
        /// </summary>
        [DataMember(Name="created_at", EmitDefaultValue=false)]
        [JsonConverter(typeof(SwaggerDateConverter))]
        public DateTime? CreatedAt { get; private set; }

        /// <summary>
        /// Gets or Sets UpdatedAt
        /// </summary>
        [DataMember(Name="updated_at", EmitDefaultValue=false)]
        [JsonConverter(typeof(SwaggerDateConverter))]
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Ürün/hizmet kodu
        /// </summary>
        /// <value>Ürün/hizmet kodu</value>
        [DataMember(Name="code", EmitDefaultValue=false)]
        public string Code { get; set; }

        /// <summary>
        /// Ürün/hizmet ismi
        /// </summary>
        /// <value>Ürün/hizmet ismi</value>
        [DataMember(Name="name", EmitDefaultValue=false)]
        public string Name { get; set; }

        /// <summary>
        /// KDV oranı
        /// </summary>
        /// <value>KDV oranı</value>
        [DataMember(Name="vat_rate", EmitDefaultValue=false)]
        public decimal? VatRate { get; set; }

        /// <summary>
        /// Satış ÖTV
        /// </summary>
        /// <value>Satış ÖTV</value>
        [DataMember(Name="sales_excise_duty", EmitDefaultValue=false)]
        public decimal? SalesExciseDuty { get; set; }

        /// <summary>
        /// Satış ÖTV tipi
        /// </summary>
        /// <value>Satış ÖTV tipi</value>
        [DataMember(Name="sales_excise_duty_type", EmitDefaultValue=false)]
        public string SalesExciseDutyType { get; set; }

        /// <summary>
        /// Alış ÖTV
        /// </summary>
        /// <value>Alış ÖTV</value>
        [DataMember(Name="purchase_excise_duty", EmitDefaultValue=false)]
        public decimal? PurchaseExciseDuty { get; set; }

        /// <summary>
        /// Alış ÖTV tipi
        /// </summary>
        /// <value>Alış ÖTV tipi</value>
        [DataMember(Name="purchase_excise_duty_type", EmitDefaultValue=false)]
        public string PurchaseExciseDutyType { get; set; }

        /// <summary>
        /// Birim
        /// </summary>
        /// <value>Birim</value>
        [DataMember(Name="unit", EmitDefaultValue=false)]
        public string Unit { get; set; }

        /// <summary>
        /// ÖİV oranı
        /// </summary>
        /// <value>ÖİV oranı</value>
        [DataMember(Name="communications_tax_rate", EmitDefaultValue=false)]
        public decimal? CommunicationsTaxRate { get; set; }

        /// <summary>
        /// Gets or Sets Archived
        /// </summary>
        [DataMember(Name="archived", EmitDefaultValue=false)]
        public bool? Archived { get; set; }

        /// <summary>
        /// Satış fiyatı
        /// </summary>
        /// <value>Satış fiyatı</value>
        [DataMember(Name="list_price", EmitDefaultValue=false)]
        public decimal? ListPrice { get; set; }

        /// <summary>
        /// Satış döviz
        /// </summary>
        /// <value>Satış döviz</value>
        [DataMember(Name="currency", EmitDefaultValue=false)]
        public string Currency { get; set; }

        /// <summary>
        /// Alış fiyatı
        /// </summary>
        /// <value>Alış fiyatı</value>
        [DataMember(Name="buying_price", EmitDefaultValue=false)]
        public decimal? BuyingPrice { get; set; }

        /// <summary>
        /// Alış döviz
        /// </summary>
        /// <value>Alış döviz</value>
        [DataMember(Name="buying_currency", EmitDefaultValue=false)]
        public string BuyingCurrency { get; set; }

        /// <summary>
        /// Gets or Sets InventoryTracking
        /// </summary>
        [DataMember(Name="inventory_tracking", EmitDefaultValue=false)]
        public bool? InventoryTracking { get; set; }

        /// <summary>
        /// Başlangıç Stok Miktarı
        /// </summary>
        /// <value>Başlangıç Stok Miktarı</value>
        [DataMember(Name="initial_stock_count", EmitDefaultValue=false)]
        public decimal? InitialStockCount { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ProductAttributes {\n");
            sb.Append("  SalesExciseDutyCode: ").Append(SalesExciseDutyCode).Append("\n");
            sb.Append("  SalesInvoiceDetailsCount: ").Append(SalesInvoiceDetailsCount).Append("\n");
            sb.Append("  PurchaseInvoiceDetailsCount: ").Append(PurchaseInvoiceDetailsCount).Append("\n");
            sb.Append("  ListPriceInTrl: ").Append(ListPriceInTrl).Append("\n");
            sb.Append("  BuyingPriceInTrl: ").Append(BuyingPriceInTrl).Append("\n");
            sb.Append("  StockCount: ").Append(StockCount).Append("\n");
            sb.Append("  CreatedAt: ").Append(CreatedAt).Append("\n");
            sb.Append("  UpdatedAt: ").Append(UpdatedAt).Append("\n");
            sb.Append("  Code: ").Append(Code).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  VatRate: ").Append(VatRate).Append("\n");
            sb.Append("  SalesExciseDuty: ").Append(SalesExciseDuty).Append("\n");
            sb.Append("  SalesExciseDutyType: ").Append(SalesExciseDutyType).Append("\n");
            sb.Append("  PurchaseExciseDuty: ").Append(PurchaseExciseDuty).Append("\n");
            sb.Append("  PurchaseExciseDutyType: ").Append(PurchaseExciseDutyType).Append("\n");
            sb.Append("  Unit: ").Append(Unit).Append("\n");
            sb.Append("  CommunicationsTaxRate: ").Append(CommunicationsTaxRate).Append("\n");
            sb.Append("  Archived: ").Append(Archived).Append("\n");
            sb.Append("  ListPrice: ").Append(ListPrice).Append("\n");
            sb.Append("  Currency: ").Append(Currency).Append("\n");
            sb.Append("  BuyingPrice: ").Append(BuyingPrice).Append("\n");
            sb.Append("  BuyingCurrency: ").Append(BuyingCurrency).Append("\n");
            sb.Append("  InventoryTracking: ").Append(InventoryTracking).Append("\n");
            sb.Append("  InitialStockCount: ").Append(InitialStockCount).Append("\n");
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
            return this.Equals(input as ProductAttributes);
        }

        /// <summary>
        /// Returns true if ProductAttributes instances are equal
        /// </summary>
        /// <param name="input">Instance of ProductAttributes to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ProductAttributes input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.SalesExciseDutyCode == input.SalesExciseDutyCode ||
                    (this.SalesExciseDutyCode != null &&
                    this.SalesExciseDutyCode.Equals(input.SalesExciseDutyCode))
                ) && 
                (
                    this.SalesInvoiceDetailsCount == input.SalesInvoiceDetailsCount ||
                    (this.SalesInvoiceDetailsCount != null &&
                    this.SalesInvoiceDetailsCount.Equals(input.SalesInvoiceDetailsCount))
                ) && 
                (
                    this.PurchaseInvoiceDetailsCount == input.PurchaseInvoiceDetailsCount ||
                    (this.PurchaseInvoiceDetailsCount != null &&
                    this.PurchaseInvoiceDetailsCount.Equals(input.PurchaseInvoiceDetailsCount))
                ) && 
                (
                    this.ListPriceInTrl == input.ListPriceInTrl ||
                    (this.ListPriceInTrl != null &&
                    this.ListPriceInTrl.Equals(input.ListPriceInTrl))
                ) && 
                (
                    this.BuyingPriceInTrl == input.BuyingPriceInTrl ||
                    (this.BuyingPriceInTrl != null &&
                    this.BuyingPriceInTrl.Equals(input.BuyingPriceInTrl))
                ) && 
                (
                    this.StockCount == input.StockCount ||
                    (this.StockCount != null &&
                    this.StockCount.Equals(input.StockCount))
                ) && 
                (
                    this.CreatedAt == input.CreatedAt ||
                    (this.CreatedAt != null &&
                    this.CreatedAt.Equals(input.CreatedAt))
                ) && 
                (
                    this.UpdatedAt == input.UpdatedAt ||
                    (this.UpdatedAt != null &&
                    this.UpdatedAt.Equals(input.UpdatedAt))
                ) && 
                (
                    this.Code == input.Code ||
                    (this.Code != null &&
                    this.Code.Equals(input.Code))
                ) && 
                (
                    this.Name == input.Name ||
                    (this.Name != null &&
                    this.Name.Equals(input.Name))
                ) && 
                (
                    this.VatRate == input.VatRate ||
                    (this.VatRate != null &&
                    this.VatRate.Equals(input.VatRate))
                ) && 
                (
                    this.SalesExciseDuty == input.SalesExciseDuty ||
                    (this.SalesExciseDuty != null &&
                    this.SalesExciseDuty.Equals(input.SalesExciseDuty))
                ) && 
                (
                    this.SalesExciseDutyType == input.SalesExciseDutyType ||
                    (this.SalesExciseDutyType != null &&
                    this.SalesExciseDutyType.Equals(input.SalesExciseDutyType))
                ) && 
                (
                    this.PurchaseExciseDuty == input.PurchaseExciseDuty ||
                    (this.PurchaseExciseDuty != null &&
                    this.PurchaseExciseDuty.Equals(input.PurchaseExciseDuty))
                ) && 
                (
                    this.PurchaseExciseDutyType == input.PurchaseExciseDutyType ||
                    (this.PurchaseExciseDutyType != null &&
                    this.PurchaseExciseDutyType.Equals(input.PurchaseExciseDutyType))
                ) && 
                (
                    this.Unit == input.Unit ||
                    (this.Unit != null &&
                    this.Unit.Equals(input.Unit))
                ) && 
                (
                    this.CommunicationsTaxRate == input.CommunicationsTaxRate ||
                    (this.CommunicationsTaxRate != null &&
                    this.CommunicationsTaxRate.Equals(input.CommunicationsTaxRate))
                ) && 
                (
                    this.Archived == input.Archived ||
                    (this.Archived != null &&
                    this.Archived.Equals(input.Archived))
                ) && 
                (
                    this.ListPrice == input.ListPrice ||
                    (this.ListPrice != null &&
                    this.ListPrice.Equals(input.ListPrice))
                ) && 
                (
                    this.Currency == input.Currency ||
                    (this.Currency != null &&
                    this.Currency.Equals(input.Currency))
                ) && 
                (
                    this.BuyingPrice == input.BuyingPrice ||
                    (this.BuyingPrice != null &&
                    this.BuyingPrice.Equals(input.BuyingPrice))
                ) && 
                (
                    this.BuyingCurrency == input.BuyingCurrency ||
                    (this.BuyingCurrency != null &&
                    this.BuyingCurrency.Equals(input.BuyingCurrency))
                ) && 
                (
                    this.InventoryTracking == input.InventoryTracking ||
                    (this.InventoryTracking != null &&
                    this.InventoryTracking.Equals(input.InventoryTracking))
                ) && 
                (
                    this.InitialStockCount == input.InitialStockCount ||
                    (this.InitialStockCount != null &&
                    this.InitialStockCount.Equals(input.InitialStockCount))
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
                if (this.SalesExciseDutyCode != null)
                    hashCode = hashCode * 59 + this.SalesExciseDutyCode.GetHashCode();
                if (this.SalesInvoiceDetailsCount != null)
                    hashCode = hashCode * 59 + this.SalesInvoiceDetailsCount.GetHashCode();
                if (this.PurchaseInvoiceDetailsCount != null)
                    hashCode = hashCode * 59 + this.PurchaseInvoiceDetailsCount.GetHashCode();
                if (this.ListPriceInTrl != null)
                    hashCode = hashCode * 59 + this.ListPriceInTrl.GetHashCode();
                if (this.BuyingPriceInTrl != null)
                    hashCode = hashCode * 59 + this.BuyingPriceInTrl.GetHashCode();
                if (this.StockCount != null)
                    hashCode = hashCode * 59 + this.StockCount.GetHashCode();
                if (this.CreatedAt != null)
                    hashCode = hashCode * 59 + this.CreatedAt.GetHashCode();
                if (this.UpdatedAt != null)
                    hashCode = hashCode * 59 + this.UpdatedAt.GetHashCode();
                if (this.Code != null)
                    hashCode = hashCode * 59 + this.Code.GetHashCode();
                if (this.Name != null)
                    hashCode = hashCode * 59 + this.Name.GetHashCode();
                if (this.VatRate != null)
                    hashCode = hashCode * 59 + this.VatRate.GetHashCode();
                if (this.SalesExciseDuty != null)
                    hashCode = hashCode * 59 + this.SalesExciseDuty.GetHashCode();
                if (this.SalesExciseDutyType != null)
                    hashCode = hashCode * 59 + this.SalesExciseDutyType.GetHashCode();
                if (this.PurchaseExciseDuty != null)
                    hashCode = hashCode * 59 + this.PurchaseExciseDuty.GetHashCode();
                if (this.PurchaseExciseDutyType != null)
                    hashCode = hashCode * 59 + this.PurchaseExciseDutyType.GetHashCode();
                if (this.Unit != null)
                    hashCode = hashCode * 59 + this.Unit.GetHashCode();
                if (this.CommunicationsTaxRate != null)
                    hashCode = hashCode * 59 + this.CommunicationsTaxRate.GetHashCode();
                if (this.Archived != null)
                    hashCode = hashCode * 59 + this.Archived.GetHashCode();
                if (this.ListPrice != null)
                    hashCode = hashCode * 59 + this.ListPrice.GetHashCode();
                if (this.Currency != null)
                    hashCode = hashCode * 59 + this.Currency.GetHashCode();
                if (this.BuyingPrice != null)
                    hashCode = hashCode * 59 + this.BuyingPrice.GetHashCode();
                if (this.BuyingCurrency != null)
                    hashCode = hashCode * 59 + this.BuyingCurrency.GetHashCode();
                if (this.InventoryTracking != null)
                    hashCode = hashCode * 59 + this.InventoryTracking.GetHashCode();
                if (this.InitialStockCount != null)
                    hashCode = hashCode * 59 + this.InitialStockCount.GetHashCode();
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
