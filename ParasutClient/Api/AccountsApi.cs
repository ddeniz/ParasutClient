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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RestSharp;
using ParasutClient.Client;
using ParasutClient.Model;

namespace ParasutClient.Api
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IAccountsApi : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// Create
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="accountForm"></param>
        /// <returns>InlineResponse201</returns>
        InlineResponse201 CreateAccount (int? companyId, AccountForm accountForm);

        /// <summary>
        /// Create
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="accountForm"></param>
        /// <returns>ApiResponse of InlineResponse201</returns>
        ApiResponse<InlineResponse201> CreateAccountWithHttpInfo (int? companyId, AccountForm accountForm);
        /// <summary>
        /// Credit Transaction
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kasa/Banka Hesabı ID</param>
        /// <param name="transactionForm"></param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account, payments* (optional)</param>
        /// <returns>InlineResponse2011</returns>
        InlineResponse2011 CreateCreditTransaction (int? companyId, int? id, TransactionForm1 transactionForm, string include = null);

        /// <summary>
        /// Credit Transaction
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kasa/Banka Hesabı ID</param>
        /// <param name="transactionForm"></param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account, payments* (optional)</param>
        /// <returns>ApiResponse of InlineResponse2011</returns>
        ApiResponse<InlineResponse2011> CreateCreditTransactionWithHttpInfo (int? companyId, int? id, TransactionForm1 transactionForm, string include = null);
        /// <summary>
        /// Debit Transaction
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kasa/Banka Hesabı ID</param>
        /// <param name="transactionForm"></param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account, payments* (optional)</param>
        /// <returns>InlineResponse2011</returns>
        InlineResponse2011 CreateDebitTransaction (int? companyId, int? id, TransactionForm transactionForm, string include = null);

        /// <summary>
        /// Debit Transaction
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kasa/Banka Hesabı ID</param>
        /// <param name="transactionForm"></param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account, payments* (optional)</param>
        /// <returns>ApiResponse of InlineResponse2011</returns>
        ApiResponse<InlineResponse2011> CreateDebitTransactionWithHttpInfo (int? companyId, int? id, TransactionForm transactionForm, string include = null);
        /// <summary>
        /// Delete
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <returns>Object</returns>
        Object DeleteAccount (int? companyId, int? id);

        /// <summary>
        /// Delete
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <returns>ApiResponse of Object</returns>
        ApiResponse<Object> DeleteAccountWithHttpInfo (int? companyId, int? id);
        /// <summary>
        /// Transactions
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <param name="filterDate"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id* (optional, default to id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account* (optional)</param>
        /// <returns>InlineResponse2001</returns>
        InlineResponse2001 ListAccountTransactions (int? companyId, int? id, string filterDate = null, string sort = null, int? pageNumber = null, int? pageSize = null, string include = null);

        /// <summary>
        /// Transactions
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <param name="filterDate"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id* (optional, default to id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account* (optional)</param>
        /// <returns>ApiResponse of InlineResponse2001</returns>
        ApiResponse<InlineResponse2001> ListAccountTransactionsWithHttpInfo (int? companyId, int? id, string filterDate = null, string sort = null, int? pageNumber = null, int? pageSize = null, string include = null);
        /// <summary>
        /// Index
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="filterName"> (optional)</param>
        /// <param name="filterCurrency"> (optional)</param>
        /// <param name="filterBankName"> (optional)</param>
        /// <param name="filterBankBranch"> (optional)</param>
        /// <param name="filterAccountType"> (optional)</param>
        /// <param name="filterIban"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id, balance, balance_in_trl* (optional, default to -id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <returns>InlineResponse200</returns>
        InlineResponse200 ListAccounts (int? companyId, string filterName = null, string filterCurrency = null, string filterBankName = null, string filterBankBranch = null, string filterAccountType = null, string filterIban = null, string sort = null, int? pageNumber = null, int? pageSize = null);

        /// <summary>
        /// Index
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="filterName"> (optional)</param>
        /// <param name="filterCurrency"> (optional)</param>
        /// <param name="filterBankName"> (optional)</param>
        /// <param name="filterBankBranch"> (optional)</param>
        /// <param name="filterAccountType"> (optional)</param>
        /// <param name="filterIban"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id, balance, balance_in_trl* (optional, default to -id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <returns>ApiResponse of InlineResponse200</returns>
        ApiResponse<InlineResponse200> ListAccountsWithHttpInfo (int? companyId, string filterName = null, string filterCurrency = null, string filterBankName = null, string filterBankBranch = null, string filterAccountType = null, string filterIban = null, string sort = null, int? pageNumber = null, int? pageSize = null);
        /// <summary>
        /// Show
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <returns>InlineResponse201</returns>
        InlineResponse201 ShowAccount (int? companyId, int? id);

        /// <summary>
        /// Show
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <returns>ApiResponse of InlineResponse201</returns>
        ApiResponse<InlineResponse201> ShowAccountWithHttpInfo (int? companyId, int? id);
        /// <summary>
        /// Edit
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <param name="accountForm"></param>
        /// <returns>InlineResponse201</returns>
        InlineResponse201 UpdateAccount (int? companyId, int? id, AccountForm1 accountForm);

        /// <summary>
        /// Edit
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <param name="accountForm"></param>
        /// <returns>ApiResponse of InlineResponse201</returns>
        ApiResponse<InlineResponse201> UpdateAccountWithHttpInfo (int? companyId, int? id, AccountForm1 accountForm);
        #endregion Synchronous Operations
        #region Asynchronous Operations
        /// <summary>
        /// Create
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="accountForm"></param>
        /// <returns>Task of InlineResponse201</returns>
        System.Threading.Tasks.Task<InlineResponse201> CreateAccountAsync (int? companyId, AccountForm accountForm);

        /// <summary>
        /// Create
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="accountForm"></param>
        /// <returns>Task of ApiResponse (InlineResponse201)</returns>
        System.Threading.Tasks.Task<ApiResponse<InlineResponse201>> CreateAccountAsyncWithHttpInfo (int? companyId, AccountForm accountForm);
        /// <summary>
        /// Credit Transaction
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kasa/Banka Hesabı ID</param>
        /// <param name="transactionForm"></param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account, payments* (optional)</param>
        /// <returns>Task of InlineResponse2011</returns>
        System.Threading.Tasks.Task<InlineResponse2011> CreateCreditTransactionAsync (int? companyId, int? id, TransactionForm1 transactionForm, string include = null);

        /// <summary>
        /// Credit Transaction
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kasa/Banka Hesabı ID</param>
        /// <param name="transactionForm"></param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account, payments* (optional)</param>
        /// <returns>Task of ApiResponse (InlineResponse2011)</returns>
        System.Threading.Tasks.Task<ApiResponse<InlineResponse2011>> CreateCreditTransactionAsyncWithHttpInfo (int? companyId, int? id, TransactionForm1 transactionForm, string include = null);
        /// <summary>
        /// Debit Transaction
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kasa/Banka Hesabı ID</param>
        /// <param name="transactionForm"></param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account, payments* (optional)</param>
        /// <returns>Task of InlineResponse2011</returns>
        System.Threading.Tasks.Task<InlineResponse2011> CreateDebitTransactionAsync (int? companyId, int? id, TransactionForm transactionForm, string include = null);

        /// <summary>
        /// Debit Transaction
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kasa/Banka Hesabı ID</param>
        /// <param name="transactionForm"></param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account, payments* (optional)</param>
        /// <returns>Task of ApiResponse (InlineResponse2011)</returns>
        System.Threading.Tasks.Task<ApiResponse<InlineResponse2011>> CreateDebitTransactionAsyncWithHttpInfo (int? companyId, int? id, TransactionForm transactionForm, string include = null);
        /// <summary>
        /// Delete
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <returns>Task of Object</returns>
        System.Threading.Tasks.Task<Object> DeleteAccountAsync (int? companyId, int? id);

        /// <summary>
        /// Delete
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <returns>Task of ApiResponse (Object)</returns>
        System.Threading.Tasks.Task<ApiResponse<Object>> DeleteAccountAsyncWithHttpInfo (int? companyId, int? id);
        /// <summary>
        /// Transactions
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <param name="filterDate"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id* (optional, default to id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account* (optional)</param>
        /// <returns>Task of InlineResponse2001</returns>
        System.Threading.Tasks.Task<InlineResponse2001> ListAccountTransactionsAsync (int? companyId, int? id, string filterDate = null, string sort = null, int? pageNumber = null, int? pageSize = null, string include = null);

        /// <summary>
        /// Transactions
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <param name="filterDate"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id* (optional, default to id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account* (optional)</param>
        /// <returns>Task of ApiResponse (InlineResponse2001)</returns>
        System.Threading.Tasks.Task<ApiResponse<InlineResponse2001>> ListAccountTransactionsAsyncWithHttpInfo (int? companyId, int? id, string filterDate = null, string sort = null, int? pageNumber = null, int? pageSize = null, string include = null);
        /// <summary>
        /// Index
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="filterName"> (optional)</param>
        /// <param name="filterCurrency"> (optional)</param>
        /// <param name="filterBankName"> (optional)</param>
        /// <param name="filterBankBranch"> (optional)</param>
        /// <param name="filterAccountType"> (optional)</param>
        /// <param name="filterIban"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id, balance, balance_in_trl* (optional, default to -id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <returns>Task of InlineResponse200</returns>
        System.Threading.Tasks.Task<InlineResponse200> ListAccountsAsync (int? companyId, string filterName = null, string filterCurrency = null, string filterBankName = null, string filterBankBranch = null, string filterAccountType = null, string filterIban = null, string sort = null, int? pageNumber = null, int? pageSize = null);

        /// <summary>
        /// Index
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="filterName"> (optional)</param>
        /// <param name="filterCurrency"> (optional)</param>
        /// <param name="filterBankName"> (optional)</param>
        /// <param name="filterBankBranch"> (optional)</param>
        /// <param name="filterAccountType"> (optional)</param>
        /// <param name="filterIban"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id, balance, balance_in_trl* (optional, default to -id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <returns>Task of ApiResponse (InlineResponse200)</returns>
        System.Threading.Tasks.Task<ApiResponse<InlineResponse200>> ListAccountsAsyncWithHttpInfo (int? companyId, string filterName = null, string filterCurrency = null, string filterBankName = null, string filterBankBranch = null, string filterAccountType = null, string filterIban = null, string sort = null, int? pageNumber = null, int? pageSize = null);
        /// <summary>
        /// Show
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <returns>Task of InlineResponse201</returns>
        System.Threading.Tasks.Task<InlineResponse201> ShowAccountAsync (int? companyId, int? id);

        /// <summary>
        /// Show
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <returns>Task of ApiResponse (InlineResponse201)</returns>
        System.Threading.Tasks.Task<ApiResponse<InlineResponse201>> ShowAccountAsyncWithHttpInfo (int? companyId, int? id);
        /// <summary>
        /// Edit
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <param name="accountForm"></param>
        /// <returns>Task of InlineResponse201</returns>
        System.Threading.Tasks.Task<InlineResponse201> UpdateAccountAsync (int? companyId, int? id, AccountForm1 accountForm);

        /// <summary>
        /// Edit
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <param name="accountForm"></param>
        /// <returns>Task of ApiResponse (InlineResponse201)</returns>
        System.Threading.Tasks.Task<ApiResponse<InlineResponse201>> UpdateAccountAsyncWithHttpInfo (int? companyId, int? id, AccountForm1 accountForm);
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public partial class AccountsApi : IAccountsApi
    {
        private ParasutClient.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountsApi"/> class.
        /// </summary>
        /// <returns></returns>
        public AccountsApi(String basePath)
        {
            this.Configuration = new ParasutClient.Client.Configuration { BasePath = basePath };

            ExceptionFactory = ParasutClient.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountsApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public AccountsApi(ParasutClient.Client.Configuration configuration = null)
        {
            if (configuration == null) // use the default one in Configuration
                this.Configuration = ParasutClient.Client.Configuration.Default;
            else
                this.Configuration = configuration;

            ExceptionFactory = ParasutClient.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        public String GetBasePath()
        {
            return this.Configuration.ApiClient.RestClient.BaseUrl.ToString();
        }

        /// <summary>
        /// Sets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        [Obsolete("SetBasePath is deprecated, please do 'Configuration.ApiClient = new ApiClient(\"http://new-path\")' instead.")]
        public void SetBasePath(String basePath)
        {
            // do nothing
        }

        /// <summary>
        /// Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public ParasutClient.Client.Configuration Configuration {get; set;}

        /// <summary>
        /// Provides a factory method hook for the creation of exceptions.
        /// </summary>
        public ParasutClient.Client.ExceptionFactory ExceptionFactory
        {
            get
            {
                if (_exceptionFactory != null && _exceptionFactory.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.");
                }
                return _exceptionFactory;
            }
            set { _exceptionFactory = value; }
        }

        /// <summary>
        /// Gets the default header.
        /// </summary>
        /// <returns>Dictionary of HTTP header</returns>
        [Obsolete("DefaultHeader is deprecated, please use Configuration.DefaultHeader instead.")]
        public IDictionary<String, String> DefaultHeader()
        {
            return new ReadOnlyDictionary<string, string>(this.Configuration.DefaultHeader);
        }

        /// <summary>
        /// Add default header.
        /// </summary>
        /// <param name="key">Header field name.</param>
        /// <param name="value">Header field value.</param>
        /// <returns></returns>
        [Obsolete("AddDefaultHeader is deprecated, please use Configuration.AddDefaultHeader instead.")]
        public void AddDefaultHeader(string key, string value)
        {
            this.Configuration.AddDefaultHeader(key, value);
        }

        /// <summary>
        /// Create 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="accountForm"></param>
        /// <returns>InlineResponse201</returns>
        public InlineResponse201 CreateAccount (int? companyId, AccountForm accountForm)
        {
             ApiResponse<InlineResponse201> localVarResponse = CreateAccountWithHttpInfo(companyId, accountForm);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="accountForm"></param>
        /// <returns>ApiResponse of InlineResponse201</returns>
        public ApiResponse< InlineResponse201 > CreateAccountWithHttpInfo (int? companyId, AccountForm accountForm)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling AccountsApi->CreateAccount");
            // verify the required parameter 'accountForm' is set
            if (accountForm == null)
                throw new ApiException(400, "Missing required parameter 'accountForm' when calling AccountsApi->CreateAccount");

            var localVarPath = "/{company_id}/accounts";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (companyId != null) localVarPathParams.Add("company_id", this.Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (accountForm != null && accountForm.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(accountForm); // http body (model) parameter
            }
            else
            {
                localVarPostBody = accountForm; // byte array
            }

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + this.Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateAccount", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse201>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse201) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse201)));
        }

        /// <summary>
        /// Create 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="accountForm"></param>
        /// <returns>Task of InlineResponse201</returns>
        public async System.Threading.Tasks.Task<InlineResponse201> CreateAccountAsync (int? companyId, AccountForm accountForm)
        {
             ApiResponse<InlineResponse201> localVarResponse = await CreateAccountAsyncWithHttpInfo(companyId, accountForm);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="accountForm"></param>
        /// <returns>Task of ApiResponse (InlineResponse201)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<InlineResponse201>> CreateAccountAsyncWithHttpInfo (int? companyId, AccountForm accountForm)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling AccountsApi->CreateAccount");
            // verify the required parameter 'accountForm' is set
            if (accountForm == null)
                throw new ApiException(400, "Missing required parameter 'accountForm' when calling AccountsApi->CreateAccount");

            var localVarPath = "/{company_id}/accounts";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (companyId != null) localVarPathParams.Add("company_id", this.Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (accountForm != null && accountForm.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(accountForm); // http body (model) parameter
            }
            else
            {
                localVarPostBody = accountForm; // byte array
            }

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + this.Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateAccount", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse201>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse201) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse201)));
        }

        /// <summary>
        /// Credit Transaction 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kasa/Banka Hesabı ID</param>
        /// <param name="transactionForm"></param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account, payments* (optional)</param>
        /// <returns>InlineResponse2011</returns>
        public InlineResponse2011 CreateCreditTransaction (int? companyId, int? id, TransactionForm1 transactionForm, string include = null)
        {
             ApiResponse<InlineResponse2011> localVarResponse = CreateCreditTransactionWithHttpInfo(companyId, id, transactionForm, include);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Credit Transaction 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kasa/Banka Hesabı ID</param>
        /// <param name="transactionForm"></param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account, payments* (optional)</param>
        /// <returns>ApiResponse of InlineResponse2011</returns>
        public ApiResponse< InlineResponse2011 > CreateCreditTransactionWithHttpInfo (int? companyId, int? id, TransactionForm1 transactionForm, string include = null)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling AccountsApi->CreateCreditTransaction");
            // verify the required parameter 'id' is set
            if (id == null)
                throw new ApiException(400, "Missing required parameter 'id' when calling AccountsApi->CreateCreditTransaction");
            // verify the required parameter 'transactionForm' is set
            if (transactionForm == null)
                throw new ApiException(400, "Missing required parameter 'transactionForm' when calling AccountsApi->CreateCreditTransaction");

            var localVarPath = "/{company_id}/accounts/{id}/credit_transactions";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (companyId != null) localVarPathParams.Add("company_id", this.Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (id != null) localVarPathParams.Add("id", this.Configuration.ApiClient.ParameterToString(id)); // path parameter
            if (include != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "include", include)); // query parameter
            if (transactionForm != null && transactionForm.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(transactionForm); // http body (model) parameter
            }
            else
            {
                localVarPostBody = transactionForm; // byte array
            }

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + this.Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateCreditTransaction", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse2011>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse2011) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse2011)));
        }

        /// <summary>
        /// Credit Transaction 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kasa/Banka Hesabı ID</param>
        /// <param name="transactionForm"></param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account, payments* (optional)</param>
        /// <returns>Task of InlineResponse2011</returns>
        public async System.Threading.Tasks.Task<InlineResponse2011> CreateCreditTransactionAsync (int? companyId, int? id, TransactionForm1 transactionForm, string include = null)
        {
             ApiResponse<InlineResponse2011> localVarResponse = await CreateCreditTransactionAsyncWithHttpInfo(companyId, id, transactionForm, include);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Credit Transaction 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kasa/Banka Hesabı ID</param>
        /// <param name="transactionForm"></param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account, payments* (optional)</param>
        /// <returns>Task of ApiResponse (InlineResponse2011)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<InlineResponse2011>> CreateCreditTransactionAsyncWithHttpInfo (int? companyId, int? id, TransactionForm1 transactionForm, string include = null)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling AccountsApi->CreateCreditTransaction");
            // verify the required parameter 'id' is set
            if (id == null)
                throw new ApiException(400, "Missing required parameter 'id' when calling AccountsApi->CreateCreditTransaction");
            // verify the required parameter 'transactionForm' is set
            if (transactionForm == null)
                throw new ApiException(400, "Missing required parameter 'transactionForm' when calling AccountsApi->CreateCreditTransaction");

            var localVarPath = "/{company_id}/accounts/{id}/credit_transactions";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (companyId != null) localVarPathParams.Add("company_id", this.Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (id != null) localVarPathParams.Add("id", this.Configuration.ApiClient.ParameterToString(id)); // path parameter
            if (include != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "include", include)); // query parameter
            if (transactionForm != null && transactionForm.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(transactionForm); // http body (model) parameter
            }
            else
            {
                localVarPostBody = transactionForm; // byte array
            }

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + this.Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateCreditTransaction", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse2011>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse2011) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse2011)));
        }

        /// <summary>
        /// Debit Transaction 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kasa/Banka Hesabı ID</param>
        /// <param name="transactionForm"></param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account, payments* (optional)</param>
        /// <returns>InlineResponse2011</returns>
        public InlineResponse2011 CreateDebitTransaction (int? companyId, int? id, TransactionForm transactionForm, string include = null)
        {
             ApiResponse<InlineResponse2011> localVarResponse = CreateDebitTransactionWithHttpInfo(companyId, id, transactionForm, include);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Debit Transaction 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kasa/Banka Hesabı ID</param>
        /// <param name="transactionForm"></param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account, payments* (optional)</param>
        /// <returns>ApiResponse of InlineResponse2011</returns>
        public ApiResponse< InlineResponse2011 > CreateDebitTransactionWithHttpInfo (int? companyId, int? id, TransactionForm transactionForm, string include = null)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling AccountsApi->CreateDebitTransaction");
            // verify the required parameter 'id' is set
            if (id == null)
                throw new ApiException(400, "Missing required parameter 'id' when calling AccountsApi->CreateDebitTransaction");
            // verify the required parameter 'transactionForm' is set
            if (transactionForm == null)
                throw new ApiException(400, "Missing required parameter 'transactionForm' when calling AccountsApi->CreateDebitTransaction");

            var localVarPath = "/{company_id}/accounts/{id}/debit_transactions";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (companyId != null) localVarPathParams.Add("company_id", this.Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (id != null) localVarPathParams.Add("id", this.Configuration.ApiClient.ParameterToString(id)); // path parameter
            if (include != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "include", include)); // query parameter
            if (transactionForm != null && transactionForm.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(transactionForm); // http body (model) parameter
            }
            else
            {
                localVarPostBody = transactionForm; // byte array
            }

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + this.Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateDebitTransaction", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse2011>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse2011) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse2011)));
        }

        /// <summary>
        /// Debit Transaction 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kasa/Banka Hesabı ID</param>
        /// <param name="transactionForm"></param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account, payments* (optional)</param>
        /// <returns>Task of InlineResponse2011</returns>
        public async System.Threading.Tasks.Task<InlineResponse2011> CreateDebitTransactionAsync (int? companyId, int? id, TransactionForm transactionForm, string include = null)
        {
             ApiResponse<InlineResponse2011> localVarResponse = await CreateDebitTransactionAsyncWithHttpInfo(companyId, id, transactionForm, include);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Debit Transaction 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Kasa/Banka Hesabı ID</param>
        /// <param name="transactionForm"></param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account, payments* (optional)</param>
        /// <returns>Task of ApiResponse (InlineResponse2011)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<InlineResponse2011>> CreateDebitTransactionAsyncWithHttpInfo (int? companyId, int? id, TransactionForm transactionForm, string include = null)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling AccountsApi->CreateDebitTransaction");
            // verify the required parameter 'id' is set
            if (id == null)
                throw new ApiException(400, "Missing required parameter 'id' when calling AccountsApi->CreateDebitTransaction");
            // verify the required parameter 'transactionForm' is set
            if (transactionForm == null)
                throw new ApiException(400, "Missing required parameter 'transactionForm' when calling AccountsApi->CreateDebitTransaction");

            var localVarPath = "/{company_id}/accounts/{id}/debit_transactions";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (companyId != null) localVarPathParams.Add("company_id", this.Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (id != null) localVarPathParams.Add("id", this.Configuration.ApiClient.ParameterToString(id)); // path parameter
            if (include != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "include", include)); // query parameter
            if (transactionForm != null && transactionForm.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(transactionForm); // http body (model) parameter
            }
            else
            {
                localVarPostBody = transactionForm; // byte array
            }

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + this.Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateDebitTransaction", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse2011>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse2011) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse2011)));
        }

        /// <summary>
        /// Delete 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <returns>Object</returns>
        public Object DeleteAccount (int? companyId, int? id)
        {
             ApiResponse<Object> localVarResponse = DeleteAccountWithHttpInfo(companyId, id);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <returns>ApiResponse of Object</returns>
        public ApiResponse< Object > DeleteAccountWithHttpInfo (int? companyId, int? id)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling AccountsApi->DeleteAccount");
            // verify the required parameter 'id' is set
            if (id == null)
                throw new ApiException(400, "Missing required parameter 'id' when calling AccountsApi->DeleteAccount");

            var localVarPath = "/{company_id}/accounts/{id}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (companyId != null) localVarPathParams.Add("company_id", this.Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (id != null) localVarPathParams.Add("id", this.Configuration.ApiClient.ParameterToString(id)); // path parameter

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + this.Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteAccount", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<Object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (Object) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(Object)));
        }

        /// <summary>
        /// Delete 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <returns>Task of Object</returns>
        public async System.Threading.Tasks.Task<Object> DeleteAccountAsync (int? companyId, int? id)
        {
             ApiResponse<Object> localVarResponse = await DeleteAccountAsyncWithHttpInfo(companyId, id);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <returns>Task of ApiResponse (Object)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<Object>> DeleteAccountAsyncWithHttpInfo (int? companyId, int? id)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling AccountsApi->DeleteAccount");
            // verify the required parameter 'id' is set
            if (id == null)
                throw new ApiException(400, "Missing required parameter 'id' when calling AccountsApi->DeleteAccount");

            var localVarPath = "/{company_id}/accounts/{id}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (companyId != null) localVarPathParams.Add("company_id", this.Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (id != null) localVarPathParams.Add("id", this.Configuration.ApiClient.ParameterToString(id)); // path parameter

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + this.Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteAccount", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<Object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (Object) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(Object)));
        }

        /// <summary>
        /// Transactions 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <param name="filterDate"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id* (optional, default to id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account* (optional)</param>
        /// <returns>InlineResponse2001</returns>
        public InlineResponse2001 ListAccountTransactions (int? companyId, int? id, string filterDate = null, string sort = null, int? pageNumber = null, int? pageSize = null, string include = null)
        {
             ApiResponse<InlineResponse2001> localVarResponse = ListAccountTransactionsWithHttpInfo(companyId, id, filterDate, sort, pageNumber, pageSize, include);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Transactions 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <param name="filterDate"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id* (optional, default to id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account* (optional)</param>
        /// <returns>ApiResponse of InlineResponse2001</returns>
        public ApiResponse< InlineResponse2001 > ListAccountTransactionsWithHttpInfo (int? companyId, int? id, string filterDate = null, string sort = null, int? pageNumber = null, int? pageSize = null, string include = null)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling AccountsApi->ListAccountTransactions");
            // verify the required parameter 'id' is set
            if (id == null)
                throw new ApiException(400, "Missing required parameter 'id' when calling AccountsApi->ListAccountTransactions");

            var localVarPath = "/{company_id}/accounts/{id}/transactions";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (companyId != null) localVarPathParams.Add("company_id", this.Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (id != null) localVarPathParams.Add("id", this.Configuration.ApiClient.ParameterToString(id)); // path parameter
            if (filterDate != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "filter[date]", filterDate)); // query parameter
            if (sort != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "sort", sort)); // query parameter
            if (pageNumber != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "page[number]", pageNumber)); // query parameter
            if (pageSize != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "page[size]", pageSize)); // query parameter
            if (include != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "include", include)); // query parameter

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + this.Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ListAccountTransactions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse2001>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse2001) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse2001)));
        }

        /// <summary>
        /// Transactions 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <param name="filterDate"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id* (optional, default to id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account* (optional)</param>
        /// <returns>Task of InlineResponse2001</returns>
        public async System.Threading.Tasks.Task<InlineResponse2001> ListAccountTransactionsAsync (int? companyId, int? id, string filterDate = null, string sort = null, int? pageNumber = null, int? pageSize = null, string include = null)
        {
             ApiResponse<InlineResponse2001> localVarResponse = await ListAccountTransactionsAsyncWithHttpInfo(companyId, id, filterDate, sort, pageNumber, pageSize, include);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Transactions 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <param name="filterDate"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id* (optional, default to id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <param name="include">Response ile birlikte geri dönmesini istediğiniz ilişkiler - *Available: debit_account, credit_account* (optional)</param>
        /// <returns>Task of ApiResponse (InlineResponse2001)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<InlineResponse2001>> ListAccountTransactionsAsyncWithHttpInfo (int? companyId, int? id, string filterDate = null, string sort = null, int? pageNumber = null, int? pageSize = null, string include = null)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling AccountsApi->ListAccountTransactions");
            // verify the required parameter 'id' is set
            if (id == null)
                throw new ApiException(400, "Missing required parameter 'id' when calling AccountsApi->ListAccountTransactions");

            var localVarPath = "/{company_id}/accounts/{id}/transactions";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (companyId != null) localVarPathParams.Add("company_id", this.Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (id != null) localVarPathParams.Add("id", this.Configuration.ApiClient.ParameterToString(id)); // path parameter
            if (filterDate != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "filter[date]", filterDate)); // query parameter
            if (sort != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "sort", sort)); // query parameter
            if (pageNumber != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "page[number]", pageNumber)); // query parameter
            if (pageSize != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "page[size]", pageSize)); // query parameter
            if (include != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "include", include)); // query parameter

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + this.Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ListAccountTransactions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse2001>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse2001) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse2001)));
        }

        /// <summary>
        /// Index 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="filterName"> (optional)</param>
        /// <param name="filterCurrency"> (optional)</param>
        /// <param name="filterBankName"> (optional)</param>
        /// <param name="filterBankBranch"> (optional)</param>
        /// <param name="filterAccountType"> (optional)</param>
        /// <param name="filterIban"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id, balance, balance_in_trl* (optional, default to -id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <returns>InlineResponse200</returns>
        public InlineResponse200 ListAccounts (int? companyId, string filterName = null, string filterCurrency = null, string filterBankName = null, string filterBankBranch = null, string filterAccountType = null, string filterIban = null, string sort = null, int? pageNumber = null, int? pageSize = null)
        {
             ApiResponse<InlineResponse200> localVarResponse = ListAccountsWithHttpInfo(companyId, filterName, filterCurrency, filterBankName, filterBankBranch, filterAccountType, filterIban, sort, pageNumber, pageSize);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Index 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="filterName"> (optional)</param>
        /// <param name="filterCurrency"> (optional)</param>
        /// <param name="filterBankName"> (optional)</param>
        /// <param name="filterBankBranch"> (optional)</param>
        /// <param name="filterAccountType"> (optional)</param>
        /// <param name="filterIban"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id, balance, balance_in_trl* (optional, default to -id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <returns>ApiResponse of InlineResponse200</returns>
        public ApiResponse< InlineResponse200 > ListAccountsWithHttpInfo (int? companyId, string filterName = null, string filterCurrency = null, string filterBankName = null, string filterBankBranch = null, string filterAccountType = null, string filterIban = null, string sort = null, int? pageNumber = null, int? pageSize = null)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling AccountsApi->ListAccounts");

            var localVarPath = "/{company_id}/accounts";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (companyId != null) localVarPathParams.Add("company_id", this.Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (filterName != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "filter[name]", filterName)); // query parameter
            if (filterCurrency != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "filter[currency]", filterCurrency)); // query parameter
            if (filterBankName != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "filter[bank_name]", filterBankName)); // query parameter
            if (filterBankBranch != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "filter[bank_branch]", filterBankBranch)); // query parameter
            if (filterAccountType != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "filter[account_type]", filterAccountType)); // query parameter
            if (filterIban != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "filter[iban]", filterIban)); // query parameter
            if (sort != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "sort", sort)); // query parameter
            if (pageNumber != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "page[number]", pageNumber)); // query parameter
            if (pageSize != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "page[size]", pageSize)); // query parameter

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + this.Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ListAccounts", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse200>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse200) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse200)));
        }

        /// <summary>
        /// Index 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="filterName"> (optional)</param>
        /// <param name="filterCurrency"> (optional)</param>
        /// <param name="filterBankName"> (optional)</param>
        /// <param name="filterBankBranch"> (optional)</param>
        /// <param name="filterAccountType"> (optional)</param>
        /// <param name="filterIban"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id, balance, balance_in_trl* (optional, default to -id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <returns>Task of InlineResponse200</returns>
        public async System.Threading.Tasks.Task<InlineResponse200> ListAccountsAsync (int? companyId, string filterName = null, string filterCurrency = null, string filterBankName = null, string filterBankBranch = null, string filterAccountType = null, string filterIban = null, string sort = null, int? pageNumber = null, int? pageSize = null)
        {
             ApiResponse<InlineResponse200> localVarResponse = await ListAccountsAsyncWithHttpInfo(companyId, filterName, filterCurrency, filterBankName, filterBankBranch, filterAccountType, filterIban, sort, pageNumber, pageSize);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Index 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="filterName"> (optional)</param>
        /// <param name="filterCurrency"> (optional)</param>
        /// <param name="filterBankName"> (optional)</param>
        /// <param name="filterBankBranch"> (optional)</param>
        /// <param name="filterAccountType"> (optional)</param>
        /// <param name="filterIban"> (optional)</param>
        /// <param name="sort">Sortable parameters - *Available: id, balance, balance_in_trl* (optional, default to -id)</param>
        /// <param name="pageNumber">Page Number (optional, default to 1)</param>
        /// <param name="pageSize">Page Size (optional, default to 15)</param>
        /// <returns>Task of ApiResponse (InlineResponse200)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<InlineResponse200>> ListAccountsAsyncWithHttpInfo (int? companyId, string filterName = null, string filterCurrency = null, string filterBankName = null, string filterBankBranch = null, string filterAccountType = null, string filterIban = null, string sort = null, int? pageNumber = null, int? pageSize = null)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling AccountsApi->ListAccounts");

            var localVarPath = "/{company_id}/accounts";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (companyId != null) localVarPathParams.Add("company_id", this.Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (filterName != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "filter[name]", filterName)); // query parameter
            if (filterCurrency != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "filter[currency]", filterCurrency)); // query parameter
            if (filterBankName != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "filter[bank_name]", filterBankName)); // query parameter
            if (filterBankBranch != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "filter[bank_branch]", filterBankBranch)); // query parameter
            if (filterAccountType != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "filter[account_type]", filterAccountType)); // query parameter
            if (filterIban != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "filter[iban]", filterIban)); // query parameter
            if (sort != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "sort", sort)); // query parameter
            if (pageNumber != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "page[number]", pageNumber)); // query parameter
            if (pageSize != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "page[size]", pageSize)); // query parameter

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + this.Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ListAccounts", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse200>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse200) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse200)));
        }

        /// <summary>
        /// Show 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <returns>InlineResponse201</returns>
        public InlineResponse201 ShowAccount (int? companyId, int? id)
        {
             ApiResponse<InlineResponse201> localVarResponse = ShowAccountWithHttpInfo(companyId, id);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Show 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <returns>ApiResponse of InlineResponse201</returns>
        public ApiResponse< InlineResponse201 > ShowAccountWithHttpInfo (int? companyId, int? id)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling AccountsApi->ShowAccount");
            // verify the required parameter 'id' is set
            if (id == null)
                throw new ApiException(400, "Missing required parameter 'id' when calling AccountsApi->ShowAccount");

            var localVarPath = "/{company_id}/accounts/{id}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (companyId != null) localVarPathParams.Add("company_id", this.Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (id != null) localVarPathParams.Add("id", this.Configuration.ApiClient.ParameterToString(id)); // path parameter

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + this.Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ShowAccount", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse201>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse201) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse201)));
        }

        /// <summary>
        /// Show 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <returns>Task of InlineResponse201</returns>
        public async System.Threading.Tasks.Task<InlineResponse201> ShowAccountAsync (int? companyId, int? id)
        {
             ApiResponse<InlineResponse201> localVarResponse = await ShowAccountAsyncWithHttpInfo(companyId, id);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Show 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <returns>Task of ApiResponse (InlineResponse201)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<InlineResponse201>> ShowAccountAsyncWithHttpInfo (int? companyId, int? id)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling AccountsApi->ShowAccount");
            // verify the required parameter 'id' is set
            if (id == null)
                throw new ApiException(400, "Missing required parameter 'id' when calling AccountsApi->ShowAccount");

            var localVarPath = "/{company_id}/accounts/{id}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (companyId != null) localVarPathParams.Add("company_id", this.Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (id != null) localVarPathParams.Add("id", this.Configuration.ApiClient.ParameterToString(id)); // path parameter

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + this.Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ShowAccount", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse201>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse201) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse201)));
        }

        /// <summary>
        /// Edit 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <param name="accountForm"></param>
        /// <returns>InlineResponse201</returns>
        public InlineResponse201 UpdateAccount (int? companyId, int? id, AccountForm1 accountForm)
        {
             ApiResponse<InlineResponse201> localVarResponse = UpdateAccountWithHttpInfo(companyId, id, accountForm);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Edit 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <param name="accountForm"></param>
        /// <returns>ApiResponse of InlineResponse201</returns>
        public ApiResponse< InlineResponse201 > UpdateAccountWithHttpInfo (int? companyId, int? id, AccountForm1 accountForm)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling AccountsApi->UpdateAccount");
            // verify the required parameter 'id' is set
            if (id == null)
                throw new ApiException(400, "Missing required parameter 'id' when calling AccountsApi->UpdateAccount");
            // verify the required parameter 'accountForm' is set
            if (accountForm == null)
                throw new ApiException(400, "Missing required parameter 'accountForm' when calling AccountsApi->UpdateAccount");

            var localVarPath = "/{company_id}/accounts/{id}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (companyId != null) localVarPathParams.Add("company_id", this.Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (id != null) localVarPathParams.Add("id", this.Configuration.ApiClient.ParameterToString(id)); // path parameter
            if (accountForm != null && accountForm.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(accountForm); // http body (model) parameter
            }
            else
            {
                localVarPostBody = accountForm; // byte array
            }

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + this.Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateAccount", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse201>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse201) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse201)));
        }

        /// <summary>
        /// Edit 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <param name="accountForm"></param>
        /// <returns>Task of InlineResponse201</returns>
        public async System.Threading.Tasks.Task<InlineResponse201> UpdateAccountAsync (int? companyId, int? id, AccountForm1 accountForm)
        {
             ApiResponse<InlineResponse201> localVarResponse = await UpdateAccountAsyncWithHttpInfo(companyId, id, accountForm);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Edit 
        /// </summary>
        /// <exception cref="ParasutClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="companyId">Firma ID</param>
        /// <param name="id">Hesap ID</param>
        /// <param name="accountForm"></param>
        /// <returns>Task of ApiResponse (InlineResponse201)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<InlineResponse201>> UpdateAccountAsyncWithHttpInfo (int? companyId, int? id, AccountForm1 accountForm)
        {
            // verify the required parameter 'companyId' is set
            if (companyId == null)
                throw new ApiException(400, "Missing required parameter 'companyId' when calling AccountsApi->UpdateAccount");
            // verify the required parameter 'id' is set
            if (id == null)
                throw new ApiException(400, "Missing required parameter 'id' when calling AccountsApi->UpdateAccount");
            // verify the required parameter 'accountForm' is set
            if (accountForm == null)
                throw new ApiException(400, "Missing required parameter 'accountForm' when calling AccountsApi->UpdateAccount");

            var localVarPath = "/{company_id}/accounts/{id}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/vnd.api+json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (companyId != null) localVarPathParams.Add("company_id", this.Configuration.ApiClient.ParameterToString(companyId)); // path parameter
            if (id != null) localVarPathParams.Add("id", this.Configuration.ApiClient.ParameterToString(id)); // path parameter
            if (accountForm != null && accountForm.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(accountForm); // http body (model) parameter
            }
            else
            {
                localVarPostBody = accountForm; // byte array
            }

            // authentication (parasut_auth) required
            // oauth required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarHeaderParams["Authorization"] = "Bearer " + this.Configuration.AccessToken;
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateAccount", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<InlineResponse201>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (InlineResponse201) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(InlineResponse201)));
        }

    }
}
