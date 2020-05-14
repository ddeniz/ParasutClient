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
    /// SalesInvoiceAttributes
    /// </summary>
    [DataContract]
    public partial class SalesInvoiceAttributes :  IEquatable<SalesInvoiceAttributes>, IValidatableObject
    {
        /// <summary>
        /// Tahsilat durumu
        /// </summary>
        /// <value>Tahsilat durumu</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum PaymentStatusEnum
        {
            
            /// <summary>
            /// Enum Paid for value: paid
            /// </summary>
            [EnumMember(Value = "paid")]
            Paid = 1,
            
            /// <summary>
            /// Enum Overdue for value: overdue
            /// </summary>
            [EnumMember(Value = "overdue")]
            Overdue = 2,
            
            /// <summary>
            /// Enum Unpaid for value: unpaid
            /// </summary>
            [EnumMember(Value = "unpaid")]
            Unpaid = 3,
            
            /// <summary>
            /// Enum Partiallypaid for value: partially_paid
            /// </summary>
            [EnumMember(Value = "partially_paid")]
            Partiallypaid = 4
        }

        /// <summary>
        /// Tahsilat durumu
        /// </summary>
        /// <value>Tahsilat durumu</value>
        [DataMember(Name="payment_status", EmitDefaultValue=false)]
        public PaymentStatusEnum? PaymentStatus { get; set; }
        /// <summary>
        /// Fatura türü
        /// </summary>
        /// <value>Fatura türü</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ItemTypeEnum
        {
            
            /// <summary>
            /// Enum Invoice for value: invoice
            /// </summary>
            [EnumMember(Value = "invoice")]
            Invoice = 1,
            
            /// <summary>
            /// Enum Estimate for value: estimate
            /// </summary>
            [EnumMember(Value = "estimate")]
            Estimate = 2,
            
            /// <summary>
            /// Enum Cancelled for value: cancelled
            /// </summary>
            [EnumMember(Value = "cancelled")]
            Cancelled = 3,
            
            /// <summary>
            /// Enum Recurringinvoice for value: recurring_invoice
            /// </summary>
            [EnumMember(Value = "recurring_invoice")]
            Recurringinvoice = 4,
            
            /// <summary>
            /// Enum Recurringestimate for value: recurring_estimate
            /// </summary>
            [EnumMember(Value = "recurring_estimate")]
            Recurringestimate = 5,
            
            /// <summary>
            /// Enum Refund for value: refund
            /// </summary>
            [EnumMember(Value = "refund")]
            Refund = 6
        }

        /// <summary>
        /// Fatura türü
        /// </summary>
        /// <value>Fatura türü</value>
        [DataMember(Name="item_type", EmitDefaultValue=false)]
        public ItemTypeEnum ItemType { get; set; }
        /// <summary>
        /// Döviz tipi
        /// </summary>
        /// <value>Döviz tipi</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum CurrencyEnum
        {
            
            /// <summary>
            /// Enum TRL for value: TRL
            /// </summary>
            [EnumMember(Value = "TRL")]
            TRL = 1,
            
            /// <summary>
            /// Enum USD for value: USD
            /// </summary>
            [EnumMember(Value = "USD")]
            USD = 2,
            
            /// <summary>
            /// Enum EUR for value: EUR
            /// </summary>
            [EnumMember(Value = "EUR")]
            EUR = 3,
            
            /// <summary>
            /// Enum GBP for value: GBP
            /// </summary>
            [EnumMember(Value = "GBP")]
            GBP = 4
        }

        /// <summary>
        /// Döviz tipi
        /// </summary>
        /// <value>Döviz tipi</value>
        [DataMember(Name="currency", EmitDefaultValue=false)]
        public CurrencyEnum? Currency { get; set; }
        /// <summary>
        /// Defines InvoiceDiscountType
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum InvoiceDiscountTypeEnum
        {
            
            /// <summary>
            /// Enum Percentage for value: percentage
            /// </summary>
            [EnumMember(Value = "percentage")]
            Percentage = 1,
            
            /// <summary>
            /// Enum Amount for value: amount
            /// </summary>
            [EnumMember(Value = "amount")]
            Amount = 2
        }

        /// <summary>
        /// Gets or Sets InvoiceDiscountType
        /// </summary>
        [DataMember(Name="invoice_discount_type", EmitDefaultValue=false)]
        public InvoiceDiscountTypeEnum? InvoiceDiscountType { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="SalesInvoiceAttributes" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected SalesInvoiceAttributes() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SalesInvoiceAttributes" /> class.
        /// </summary>
        /// <param name="itemType">Fatura türü (required).</param>
        /// <param name="description">Fatura açıklaması.</param>
        /// <param name="issueDate">Düzenleme tarihi (required).</param>
        /// <param name="dueDate">Son tahsilat tarihi.</param>
        /// <param name="invoiceSeries">Fatura seri.</param>
        /// <param name="invoiceId">Fatura sıra.</param>
        /// <param name="currency">Döviz tipi.</param>
        /// <param name="exchangeRate">Döviz kuru.</param>
        /// <param name="withholdingRate">Stopaj oranı.</param>
        /// <param name="vatWithholdingRate">Tevkifat oranı.</param>
        /// <param name="invoiceDiscountType">invoiceDiscountType.</param>
        /// <param name="invoiceDiscount">invoiceDiscount.</param>
        /// <param name="billingAddress">Gönderim adresi.</param>
        /// <param name="billingPhone">Gönderim adresi telefonu.</param>
        /// <param name="billingFax">Gönderim adresi faksı.</param>
        /// <param name="taxOffice">Müşteri vergi dairesi.</param>
        /// <param name="taxNumber">Müşteri vergi numarası.</param>
        /// <param name="city">city.</param>
        /// <param name="district">district.</param>
        /// <param name="isAbroad">isAbroad.</param>
        /// <param name="orderNo">orderNo.</param>
        /// <param name="orderDate">orderDate.</param>
        /// <param name="shipmentAddres">shipmentAddres.</param>
        /// <param name="shipmentIncluded">İrsaliyeli fatura.</param>
        public SalesInvoiceAttributes(ItemTypeEnum itemType = default(ItemTypeEnum), string description = default(string), DateTime? issueDate = default(DateTime?), DateTime? dueDate = default(DateTime?), string invoiceSeries = default(string), int? invoiceId = default(int?), CurrencyEnum? currency = default(CurrencyEnum?), decimal? exchangeRate = default(decimal?), decimal? withholdingRate = default(decimal?), decimal? vatWithholdingRate = default(decimal?), InvoiceDiscountTypeEnum? invoiceDiscountType = default(InvoiceDiscountTypeEnum?), decimal? invoiceDiscount = default(decimal?), string billingAddress = default(string), string billingPhone = default(string), string billingFax = default(string), string taxOffice = default(string), string taxNumber = default(string), string city = default(string), string district = default(string), string isAbroad = default(string), string orderNo = default(string), DateTime? orderDate = default(DateTime?), string shipmentAddres = default(string), bool? shipmentIncluded = default(bool?))
        {
            // to ensure "itemType" is required (not null)
            if (itemType == null)
            {
                throw new InvalidDataException("itemType is a required property for SalesInvoiceAttributes and cannot be null");
            }
            else
            {
                this.ItemType = itemType;
            }
            // to ensure "issueDate" is required (not null)
            if (issueDate == null)
            {
                throw new InvalidDataException("issueDate is a required property for SalesInvoiceAttributes and cannot be null");
            }
            else
            {
                this.IssueDate = issueDate;
            }
            this.Description = description;
            this.DueDate = dueDate;
            this.InvoiceSeries = invoiceSeries;
            this.InvoiceId = invoiceId;
            this.Currency = currency;
            this.ExchangeRate = exchangeRate;
            this.WithholdingRate = withholdingRate;
            this.VatWithholdingRate = vatWithholdingRate;
            this.InvoiceDiscountType = invoiceDiscountType;
            this.InvoiceDiscount = invoiceDiscount;
            this.BillingAddress = billingAddress;
            this.BillingPhone = billingPhone;
            this.BillingFax = billingFax;
            this.TaxOffice = taxOffice;
            this.TaxNumber = taxNumber;
            this.City = city;
            this.District = district;
            this.IsAbroad = isAbroad;
            this.OrderNo = orderNo;
            this.OrderDate = orderDate;
            this.ShipmentAddres = shipmentAddres;
            this.ShipmentIncluded = shipmentIncluded;
        }
        
        /// <summary>
        /// Gets or Sets Archived
        /// </summary>
        [DataMember(Name="archived", EmitDefaultValue=false)]
        public bool? Archived { get; private set; }

        /// <summary>
        /// Fatura no
        /// </summary>
        /// <value>Fatura no</value>
        [DataMember(Name="invoice_no", EmitDefaultValue=false)]
        public string InvoiceNo { get; private set; }

        /// <summary>
        /// Genel Toplam
        /// </summary>
        /// <value>Genel Toplam</value>
        [DataMember(Name="net_total", EmitDefaultValue=false)]
        public decimal? NetTotal { get; private set; }

        /// <summary>
        /// Ara toplam
        /// </summary>
        /// <value>Ara toplam</value>
        [DataMember(Name="gross_total", EmitDefaultValue=false)]
        public decimal? GrossTotal { get; private set; }

        /// <summary>
        /// Stopaj
        /// </summary>
        /// <value>Stopaj</value>
        [DataMember(Name="withholding", EmitDefaultValue=false)]
        public decimal? Withholding { get; private set; }

        /// <summary>
        /// Gets or Sets TotalExciseDuty
        /// </summary>
        [DataMember(Name="total_excise_duty", EmitDefaultValue=false)]
        public decimal? TotalExciseDuty { get; private set; }

        /// <summary>
        /// Gets or Sets TotalCommunicationsTax
        /// </summary>
        [DataMember(Name="total_communications_tax", EmitDefaultValue=false)]
        public decimal? TotalCommunicationsTax { get; private set; }

        /// <summary>
        /// Toplam KDV
        /// </summary>
        /// <value>Toplam KDV</value>
        [DataMember(Name="total_vat", EmitDefaultValue=false)]
        public decimal? TotalVat { get; private set; }

        /// <summary>
        /// Tevkifat
        /// </summary>
        /// <value>Tevkifat</value>
        [DataMember(Name="vat_withholding", EmitDefaultValue=false)]
        public decimal? VatWithholding { get; private set; }

        /// <summary>
        /// Toplam indirim
        /// </summary>
        /// <value>Toplam indirim</value>
        [DataMember(Name="total_discount", EmitDefaultValue=false)]
        public decimal? TotalDiscount { get; private set; }

        /// <summary>
        /// Gets or Sets TotalInvoiceDiscount
        /// </summary>
        [DataMember(Name="total_invoice_discount", EmitDefaultValue=false)]
        public decimal? TotalInvoiceDiscount { get; private set; }

        /// <summary>
        /// Vergiler Hariç Toplam
        /// </summary>
        /// <value>Vergiler Hariç Toplam</value>
        [DataMember(Name="before_taxes_total", EmitDefaultValue=false)]
        public decimal? BeforeTaxesTotal { get; private set; }

        /// <summary>
        /// Ödenmemiş tutar
        /// </summary>
        /// <value>Ödenmemiş tutar</value>
        [DataMember(Name="remaining", EmitDefaultValue=false)]
        public decimal? Remaining { get; private set; }

        /// <summary>
        /// Gets or Sets RemainingInTrl
        /// </summary>
        [DataMember(Name="remaining_in_trl", EmitDefaultValue=false)]
        public decimal? RemainingInTrl { get; private set; }


        /// <summary>
        /// Kayıt tarihi
        /// </summary>
        /// <value>Kayıt tarihi</value>
        [DataMember(Name="created_at", EmitDefaultValue=false)]
        [JsonConverter(typeof(SwaggerDateConverter))]
        public DateTime? CreatedAt { get; private set; }

        /// <summary>
        /// Son güncelleme tarihi
        /// </summary>
        /// <value>Son güncelleme tarihi</value>
        [DataMember(Name="updated_at", EmitDefaultValue=false)]
        [JsonConverter(typeof(SwaggerDateConverter))]
        public DateTime? UpdatedAt { get; private set; }


        /// <summary>
        /// Fatura açıklaması
        /// </summary>
        /// <value>Fatura açıklaması</value>
        [DataMember(Name="description", EmitDefaultValue=false)]
        public string Description { get; set; }

        /// <summary>
        /// Düzenleme tarihi
        /// </summary>
        /// <value>Düzenleme tarihi</value>
        [DataMember(Name="issue_date", EmitDefaultValue=false)]
        [JsonConverter(typeof(SwaggerDateConverter))]
        public DateTime? IssueDate { get; set; }

        /// <summary>
        /// Son tahsilat tarihi
        /// </summary>
        /// <value>Son tahsilat tarihi</value>
        [DataMember(Name="due_date", EmitDefaultValue=false)]
        [JsonConverter(typeof(SwaggerDateConverter))]
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Fatura seri
        /// </summary>
        /// <value>Fatura seri</value>
        [DataMember(Name="invoice_series", EmitDefaultValue=false)]
        public string InvoiceSeries { get; set; }

        /// <summary>
        /// Fatura sıra
        /// </summary>
        /// <value>Fatura sıra</value>
        [DataMember(Name="invoice_id", EmitDefaultValue=false)]
        public int? InvoiceId { get; set; }


        /// <summary>
        /// Döviz kuru
        /// </summary>
        /// <value>Döviz kuru</value>
        [DataMember(Name="exchange_rate", EmitDefaultValue=false)]
        public decimal? ExchangeRate { get; set; }

        /// <summary>
        /// Stopaj oranı
        /// </summary>
        /// <value>Stopaj oranı</value>
        [DataMember(Name="withholding_rate", EmitDefaultValue=false)]
        public decimal? WithholdingRate { get; set; }

        /// <summary>
        /// Tevkifat oranı
        /// </summary>
        /// <value>Tevkifat oranı</value>
        [DataMember(Name="vat_withholding_rate", EmitDefaultValue=false)]
        public decimal? VatWithholdingRate { get; set; }


        /// <summary>
        /// Gets or Sets InvoiceDiscount
        /// </summary>
        [DataMember(Name="invoice_discount", EmitDefaultValue=false)]
        public decimal? InvoiceDiscount { get; set; }

        /// <summary>
        /// Gönderim adresi
        /// </summary>
        /// <value>Gönderim adresi</value>
        [DataMember(Name="billing_address", EmitDefaultValue=false)]
        public string BillingAddress { get; set; }

        /// <summary>
        /// Gönderim adresi telefonu
        /// </summary>
        /// <value>Gönderim adresi telefonu</value>
        [DataMember(Name="billing_phone", EmitDefaultValue=false)]
        public string BillingPhone { get; set; }

        /// <summary>
        /// Gönderim adresi faksı
        /// </summary>
        /// <value>Gönderim adresi faksı</value>
        [DataMember(Name="billing_fax", EmitDefaultValue=false)]
        public string BillingFax { get; set; }

        /// <summary>
        /// Müşteri vergi dairesi
        /// </summary>
        /// <value>Müşteri vergi dairesi</value>
        [DataMember(Name="tax_office", EmitDefaultValue=false)]
        public string TaxOffice { get; set; }

        /// <summary>
        /// Müşteri vergi numarası
        /// </summary>
        /// <value>Müşteri vergi numarası</value>
        [DataMember(Name="tax_number", EmitDefaultValue=false)]
        public string TaxNumber { get; set; }

        /// <summary>
        /// Gets or Sets City
        /// </summary>
        [DataMember(Name="city", EmitDefaultValue=false)]
        public string City { get; set; }

        /// <summary>
        /// Gets or Sets District
        /// </summary>
        [DataMember(Name="district", EmitDefaultValue=false)]
        public string District { get; set; }

        /// <summary>
        /// Gets or Sets IsAbroad
        /// </summary>
        [DataMember(Name="is_abroad", EmitDefaultValue=false)]
        public string IsAbroad { get; set; }

        /// <summary>
        /// Gets or Sets OrderNo
        /// </summary>
        [DataMember(Name="order_no", EmitDefaultValue=false)]
        public string OrderNo { get; set; }

        /// <summary>
        /// Gets or Sets OrderDate
        /// </summary>
        [DataMember(Name="order_date", EmitDefaultValue=false)]
        [JsonConverter(typeof(SwaggerDateConverter))]
        public DateTime? OrderDate { get; set; }

        /// <summary>
        /// Gets or Sets ShipmentAddres
        /// </summary>
        [DataMember(Name="shipment_addres", EmitDefaultValue=false)]
        public string ShipmentAddres { get; set; }

        /// <summary>
        /// İrsaliyeli fatura
        /// </summary>
        /// <value>İrsaliyeli fatura</value>
        [DataMember(Name="shipment_included", EmitDefaultValue=false)]
        public bool? ShipmentIncluded { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SalesInvoiceAttributes {\n");
            sb.Append("  Archived: ").Append(Archived).Append("\n");
            sb.Append("  InvoiceNo: ").Append(InvoiceNo).Append("\n");
            sb.Append("  NetTotal: ").Append(NetTotal).Append("\n");
            sb.Append("  GrossTotal: ").Append(GrossTotal).Append("\n");
            sb.Append("  Withholding: ").Append(Withholding).Append("\n");
            sb.Append("  TotalExciseDuty: ").Append(TotalExciseDuty).Append("\n");
            sb.Append("  TotalCommunicationsTax: ").Append(TotalCommunicationsTax).Append("\n");
            sb.Append("  TotalVat: ").Append(TotalVat).Append("\n");
            sb.Append("  VatWithholding: ").Append(VatWithholding).Append("\n");
            sb.Append("  TotalDiscount: ").Append(TotalDiscount).Append("\n");
            sb.Append("  TotalInvoiceDiscount: ").Append(TotalInvoiceDiscount).Append("\n");
            sb.Append("  BeforeTaxesTotal: ").Append(BeforeTaxesTotal).Append("\n");
            sb.Append("  Remaining: ").Append(Remaining).Append("\n");
            sb.Append("  RemainingInTrl: ").Append(RemainingInTrl).Append("\n");
            sb.Append("  PaymentStatus: ").Append(PaymentStatus).Append("\n");
            sb.Append("  CreatedAt: ").Append(CreatedAt).Append("\n");
            sb.Append("  UpdatedAt: ").Append(UpdatedAt).Append("\n");
            sb.Append("  ItemType: ").Append(ItemType).Append("\n");
            sb.Append("  Description: ").Append(Description).Append("\n");
            sb.Append("  IssueDate: ").Append(IssueDate).Append("\n");
            sb.Append("  DueDate: ").Append(DueDate).Append("\n");
            sb.Append("  InvoiceSeries: ").Append(InvoiceSeries).Append("\n");
            sb.Append("  InvoiceId: ").Append(InvoiceId).Append("\n");
            sb.Append("  Currency: ").Append(Currency).Append("\n");
            sb.Append("  ExchangeRate: ").Append(ExchangeRate).Append("\n");
            sb.Append("  WithholdingRate: ").Append(WithholdingRate).Append("\n");
            sb.Append("  VatWithholdingRate: ").Append(VatWithholdingRate).Append("\n");
            sb.Append("  InvoiceDiscountType: ").Append(InvoiceDiscountType).Append("\n");
            sb.Append("  InvoiceDiscount: ").Append(InvoiceDiscount).Append("\n");
            sb.Append("  BillingAddress: ").Append(BillingAddress).Append("\n");
            sb.Append("  BillingPhone: ").Append(BillingPhone).Append("\n");
            sb.Append("  BillingFax: ").Append(BillingFax).Append("\n");
            sb.Append("  TaxOffice: ").Append(TaxOffice).Append("\n");
            sb.Append("  TaxNumber: ").Append(TaxNumber).Append("\n");
            sb.Append("  City: ").Append(City).Append("\n");
            sb.Append("  District: ").Append(District).Append("\n");
            sb.Append("  IsAbroad: ").Append(IsAbroad).Append("\n");
            sb.Append("  OrderNo: ").Append(OrderNo).Append("\n");
            sb.Append("  OrderDate: ").Append(OrderDate).Append("\n");
            sb.Append("  ShipmentAddres: ").Append(ShipmentAddres).Append("\n");
            sb.Append("  ShipmentIncluded: ").Append(ShipmentIncluded).Append("\n");
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
            return this.Equals(input as SalesInvoiceAttributes);
        }

        /// <summary>
        /// Returns true if SalesInvoiceAttributes instances are equal
        /// </summary>
        /// <param name="input">Instance of SalesInvoiceAttributes to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SalesInvoiceAttributes input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Archived == input.Archived ||
                    (this.Archived != null &&
                    this.Archived.Equals(input.Archived))
                ) && 
                (
                    this.InvoiceNo == input.InvoiceNo ||
                    (this.InvoiceNo != null &&
                    this.InvoiceNo.Equals(input.InvoiceNo))
                ) && 
                (
                    this.NetTotal == input.NetTotal ||
                    (this.NetTotal != null &&
                    this.NetTotal.Equals(input.NetTotal))
                ) && 
                (
                    this.GrossTotal == input.GrossTotal ||
                    (this.GrossTotal != null &&
                    this.GrossTotal.Equals(input.GrossTotal))
                ) && 
                (
                    this.Withholding == input.Withholding ||
                    (this.Withholding != null &&
                    this.Withholding.Equals(input.Withholding))
                ) && 
                (
                    this.TotalExciseDuty == input.TotalExciseDuty ||
                    (this.TotalExciseDuty != null &&
                    this.TotalExciseDuty.Equals(input.TotalExciseDuty))
                ) && 
                (
                    this.TotalCommunicationsTax == input.TotalCommunicationsTax ||
                    (this.TotalCommunicationsTax != null &&
                    this.TotalCommunicationsTax.Equals(input.TotalCommunicationsTax))
                ) && 
                (
                    this.TotalVat == input.TotalVat ||
                    (this.TotalVat != null &&
                    this.TotalVat.Equals(input.TotalVat))
                ) && 
                (
                    this.VatWithholding == input.VatWithholding ||
                    (this.VatWithholding != null &&
                    this.VatWithholding.Equals(input.VatWithholding))
                ) && 
                (
                    this.TotalDiscount == input.TotalDiscount ||
                    (this.TotalDiscount != null &&
                    this.TotalDiscount.Equals(input.TotalDiscount))
                ) && 
                (
                    this.TotalInvoiceDiscount == input.TotalInvoiceDiscount ||
                    (this.TotalInvoiceDiscount != null &&
                    this.TotalInvoiceDiscount.Equals(input.TotalInvoiceDiscount))
                ) && 
                (
                    this.BeforeTaxesTotal == input.BeforeTaxesTotal ||
                    (this.BeforeTaxesTotal != null &&
                    this.BeforeTaxesTotal.Equals(input.BeforeTaxesTotal))
                ) && 
                (
                    this.Remaining == input.Remaining ||
                    (this.Remaining != null &&
                    this.Remaining.Equals(input.Remaining))
                ) && 
                (
                    this.RemainingInTrl == input.RemainingInTrl ||
                    (this.RemainingInTrl != null &&
                    this.RemainingInTrl.Equals(input.RemainingInTrl))
                ) && 
                (
                    this.PaymentStatus == input.PaymentStatus ||
                    (this.PaymentStatus != null &&
                    this.PaymentStatus.Equals(input.PaymentStatus))
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
                    this.ItemType == input.ItemType ||
                    (this.ItemType != null &&
                    this.ItemType.Equals(input.ItemType))
                ) && 
                (
                    this.Description == input.Description ||
                    (this.Description != null &&
                    this.Description.Equals(input.Description))
                ) && 
                (
                    this.IssueDate == input.IssueDate ||
                    (this.IssueDate != null &&
                    this.IssueDate.Equals(input.IssueDate))
                ) && 
                (
                    this.DueDate == input.DueDate ||
                    (this.DueDate != null &&
                    this.DueDate.Equals(input.DueDate))
                ) && 
                (
                    this.InvoiceSeries == input.InvoiceSeries ||
                    (this.InvoiceSeries != null &&
                    this.InvoiceSeries.Equals(input.InvoiceSeries))
                ) && 
                (
                    this.InvoiceId == input.InvoiceId ||
                    (this.InvoiceId != null &&
                    this.InvoiceId.Equals(input.InvoiceId))
                ) && 
                (
                    this.Currency == input.Currency ||
                    (this.Currency != null &&
                    this.Currency.Equals(input.Currency))
                ) && 
                (
                    this.ExchangeRate == input.ExchangeRate ||
                    (this.ExchangeRate != null &&
                    this.ExchangeRate.Equals(input.ExchangeRate))
                ) && 
                (
                    this.WithholdingRate == input.WithholdingRate ||
                    (this.WithholdingRate != null &&
                    this.WithholdingRate.Equals(input.WithholdingRate))
                ) && 
                (
                    this.VatWithholdingRate == input.VatWithholdingRate ||
                    (this.VatWithholdingRate != null &&
                    this.VatWithholdingRate.Equals(input.VatWithholdingRate))
                ) && 
                (
                    this.InvoiceDiscountType == input.InvoiceDiscountType ||
                    (this.InvoiceDiscountType != null &&
                    this.InvoiceDiscountType.Equals(input.InvoiceDiscountType))
                ) && 
                (
                    this.InvoiceDiscount == input.InvoiceDiscount ||
                    (this.InvoiceDiscount != null &&
                    this.InvoiceDiscount.Equals(input.InvoiceDiscount))
                ) && 
                (
                    this.BillingAddress == input.BillingAddress ||
                    (this.BillingAddress != null &&
                    this.BillingAddress.Equals(input.BillingAddress))
                ) && 
                (
                    this.BillingPhone == input.BillingPhone ||
                    (this.BillingPhone != null &&
                    this.BillingPhone.Equals(input.BillingPhone))
                ) && 
                (
                    this.BillingFax == input.BillingFax ||
                    (this.BillingFax != null &&
                    this.BillingFax.Equals(input.BillingFax))
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
                    this.City == input.City ||
                    (this.City != null &&
                    this.City.Equals(input.City))
                ) && 
                (
                    this.District == input.District ||
                    (this.District != null &&
                    this.District.Equals(input.District))
                ) && 
                (
                    this.IsAbroad == input.IsAbroad ||
                    (this.IsAbroad != null &&
                    this.IsAbroad.Equals(input.IsAbroad))
                ) && 
                (
                    this.OrderNo == input.OrderNo ||
                    (this.OrderNo != null &&
                    this.OrderNo.Equals(input.OrderNo))
                ) && 
                (
                    this.OrderDate == input.OrderDate ||
                    (this.OrderDate != null &&
                    this.OrderDate.Equals(input.OrderDate))
                ) && 
                (
                    this.ShipmentAddres == input.ShipmentAddres ||
                    (this.ShipmentAddres != null &&
                    this.ShipmentAddres.Equals(input.ShipmentAddres))
                ) && 
                (
                    this.ShipmentIncluded == input.ShipmentIncluded ||
                    (this.ShipmentIncluded != null &&
                    this.ShipmentIncluded.Equals(input.ShipmentIncluded))
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
                if (this.Archived != null)
                    hashCode = hashCode * 59 + this.Archived.GetHashCode();
                if (this.InvoiceNo != null)
                    hashCode = hashCode * 59 + this.InvoiceNo.GetHashCode();
                if (this.NetTotal != null)
                    hashCode = hashCode * 59 + this.NetTotal.GetHashCode();
                if (this.GrossTotal != null)
                    hashCode = hashCode * 59 + this.GrossTotal.GetHashCode();
                if (this.Withholding != null)
                    hashCode = hashCode * 59 + this.Withholding.GetHashCode();
                if (this.TotalExciseDuty != null)
                    hashCode = hashCode * 59 + this.TotalExciseDuty.GetHashCode();
                if (this.TotalCommunicationsTax != null)
                    hashCode = hashCode * 59 + this.TotalCommunicationsTax.GetHashCode();
                if (this.TotalVat != null)
                    hashCode = hashCode * 59 + this.TotalVat.GetHashCode();
                if (this.VatWithholding != null)
                    hashCode = hashCode * 59 + this.VatWithholding.GetHashCode();
                if (this.TotalDiscount != null)
                    hashCode = hashCode * 59 + this.TotalDiscount.GetHashCode();
                if (this.TotalInvoiceDiscount != null)
                    hashCode = hashCode * 59 + this.TotalInvoiceDiscount.GetHashCode();
                if (this.BeforeTaxesTotal != null)
                    hashCode = hashCode * 59 + this.BeforeTaxesTotal.GetHashCode();
                if (this.Remaining != null)
                    hashCode = hashCode * 59 + this.Remaining.GetHashCode();
                if (this.RemainingInTrl != null)
                    hashCode = hashCode * 59 + this.RemainingInTrl.GetHashCode();
                if (this.PaymentStatus != null)
                    hashCode = hashCode * 59 + this.PaymentStatus.GetHashCode();
                if (this.CreatedAt != null)
                    hashCode = hashCode * 59 + this.CreatedAt.GetHashCode();
                if (this.UpdatedAt != null)
                    hashCode = hashCode * 59 + this.UpdatedAt.GetHashCode();
                if (this.ItemType != null)
                    hashCode = hashCode * 59 + this.ItemType.GetHashCode();
                if (this.Description != null)
                    hashCode = hashCode * 59 + this.Description.GetHashCode();
                if (this.IssueDate != null)
                    hashCode = hashCode * 59 + this.IssueDate.GetHashCode();
                if (this.DueDate != null)
                    hashCode = hashCode * 59 + this.DueDate.GetHashCode();
                if (this.InvoiceSeries != null)
                    hashCode = hashCode * 59 + this.InvoiceSeries.GetHashCode();
                if (this.InvoiceId != null)
                    hashCode = hashCode * 59 + this.InvoiceId.GetHashCode();
                if (this.Currency != null)
                    hashCode = hashCode * 59 + this.Currency.GetHashCode();
                if (this.ExchangeRate != null)
                    hashCode = hashCode * 59 + this.ExchangeRate.GetHashCode();
                if (this.WithholdingRate != null)
                    hashCode = hashCode * 59 + this.WithholdingRate.GetHashCode();
                if (this.VatWithholdingRate != null)
                    hashCode = hashCode * 59 + this.VatWithholdingRate.GetHashCode();
                if (this.InvoiceDiscountType != null)
                    hashCode = hashCode * 59 + this.InvoiceDiscountType.GetHashCode();
                if (this.InvoiceDiscount != null)
                    hashCode = hashCode * 59 + this.InvoiceDiscount.GetHashCode();
                if (this.BillingAddress != null)
                    hashCode = hashCode * 59 + this.BillingAddress.GetHashCode();
                if (this.BillingPhone != null)
                    hashCode = hashCode * 59 + this.BillingPhone.GetHashCode();
                if (this.BillingFax != null)
                    hashCode = hashCode * 59 + this.BillingFax.GetHashCode();
                if (this.TaxOffice != null)
                    hashCode = hashCode * 59 + this.TaxOffice.GetHashCode();
                if (this.TaxNumber != null)
                    hashCode = hashCode * 59 + this.TaxNumber.GetHashCode();
                if (this.City != null)
                    hashCode = hashCode * 59 + this.City.GetHashCode();
                if (this.District != null)
                    hashCode = hashCode * 59 + this.District.GetHashCode();
                if (this.IsAbroad != null)
                    hashCode = hashCode * 59 + this.IsAbroad.GetHashCode();
                if (this.OrderNo != null)
                    hashCode = hashCode * 59 + this.OrderNo.GetHashCode();
                if (this.OrderDate != null)
                    hashCode = hashCode * 59 + this.OrderDate.GetHashCode();
                if (this.ShipmentAddres != null)
                    hashCode = hashCode * 59 + this.ShipmentAddres.GetHashCode();
                if (this.ShipmentIncluded != null)
                    hashCode = hashCode * 59 + this.ShipmentIncluded.GetHashCode();
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
