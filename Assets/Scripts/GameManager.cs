using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Loading;
public class GameManager : MonoBehaviour
{
    [Header("Baglantilar")]
    public DesteManager desteManager;

    [Header("Oyun Ayarlari")]
    public int oyuncuSayisi = 4;
    public List<Oyuncu> oyuncular = new List<Oyuncu>();
    [Header("Oyun Durumu")]
    public Tas gostergeTasi;
    public Tas okeyTasi;
    private int aktifOyuncuIndex = 0;
    public List<Tas> atilanTaslarYigini = new List<Tas>();
    private int oynananElSayisiBuTurda = 0;

    void Start()
    {
        if (desteManager == null)
        {
            Debug.LogError("GameManager: DesteManager atanmamis");
            return;
        }
        OyunuBaslat();

    }
    void OyunuBaslat()
    {
        Debug.Log("Oyun Baslatiliyor");
        oyuncular.Clear();
        for (int i = 0; i < oyuncuSayisi; i++)
        {
            Oyuncu yenioyuncu = new Oyuncu("Oyuncu" + (i + 1), i == 0);
            oyuncular.Add(yenioyuncu);
        }
        Debug.Log(oyuncuSayisi + "Adet oyuncu olusturuldu");

        Debug.Log("Taslar dagitiliyor");
        for (int i = 0; i < oyuncular.Count; i++)
        {
            int dagitilacakTasSayisi = oyuncular[i].ilkOyuncuMu ? 22 : 21;
            for (int j = 0; j < dagitilacakTasSayisi; j++)
            {
                Tas cekilenTas = desteManager.TasCek();
                if (cekilenTas != null)
                {
                    oyuncular[i].TasaEkle(cekilenTas);
                }
                else
                {
                    Debug.LogError("tas bitti hala tas dagilitilmaya calisiliyor");
                    break;
                }
            }
        }
        Debug.Log("Tas dagitimi tamamlandi. Oyuncu elleri: ");
        foreach (Oyuncu oyuncu in oyuncular)
        {
            oyuncu.EliniGoster();
        }
        GostergeVeOkeyBelirle();
        if (desteManager != null)
        {
            Debug.Log("Destede kalan tas sayisi: " + desteManager.KalanTasSayisi());
        }

        Debug.Log("--- Per Kontrol Testleri Baþlýyor (Jokerli) ---");
        Debug.Log("Bu el için belirlenen Okey Taþý: " + (this.okeyTasi != null ? this.okeyTasi.ToString() : "BELÝRLENEMEDÝ"));


        if (desteManager != null && desteManager.TasVeritabani != null && desteManager.TasVeritabani.tumTaslar != null && desteManager.TasVeritabani.tumTaslar.Count > 10)
        {

            Tas kirmizi7 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Kirmizi && t.sayi == 7 && t.tip == TasTipi.Sayi);
            Tas mavi7 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Mavi && t.sayi == 7 && t.tip == TasTipi.Sayi);
            Tas siyah7 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Siyah && t.sayi == 7 && t.tip == TasTipi.Sayi);
            Tas sari7 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Sari && t.sayi == 7 && t.tip == TasTipi.Sayi);
            Tas kirmizi8 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Kirmizi && t.sayi == 8 && t.tip == TasTipi.Sayi);
            Tas sahteOkeyTest = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.tip == TasTipi.SahteOkey); // Bir Sahte Okey alalým


            if (kirmizi7 == null || mavi7 == null || siyah7 == null || sari7 == null || kirmizi8 == null)
            {
                Debug.LogError("Per testleri için gerekli temel sayýlý taþlar TasVeritabani'nda bulunamadý!");
            }
            else
            {

                List<Tas> gecerliPer3 = new List<Tas> { kirmizi7, mavi7, siyah7 };
                Debug.Log("Test 1 [Geçerli Saf Per (3'lü - K7,M7,S7)]: " + OkeyRulesEngine.GecerliPerMi(gecerliPer3, this.okeyTasi) + " (Beklenen: True)");

                List<Tas> gecerliPer4 = new List<Tas> { kirmizi7, mavi7, siyah7, sari7 };
                Debug.Log("Test 2 [Geçerli Saf Per (4'lü - K7,M7,S7,Sa7)]: " + OkeyRulesEngine.GecerliPerMi(gecerliPer4, this.okeyTasi) + " (Beklenen: True)");

                List<Tas> gecersizPerAzTas = new List<Tas> { kirmizi7, mavi7 };
                Debug.Log("Test 3 [Geçersiz Per (Az Taþ - K7,M7)]: " + OkeyRulesEngine.GecerliPerMi(gecersizPerAzTas, this.okeyTasi) + " (Beklenen: False)");

                List<Tas> gecersizPerAyniRenk = new List<Tas> { kirmizi7, mavi7, kirmizi7 };
                Debug.Log("Test 5 [Geçersiz Per (Ayný Renk Tekrarý - K7,M7,K7)]: " + OkeyRulesEngine.GecerliPerMi(gecersizPerAyniRenk, this.okeyTasi) + " (Beklenen: False)");

                List<Tas> gecersizPerFarkliSayi = new List<Tas> { kirmizi7, mavi7, kirmizi8 };
                Debug.Log("Test 6 [Geçersiz Per (Farklý Sayýlar - K7,M7,K8)]: " + OkeyRulesEngine.GecerliPerMi(gecersizPerFarkliSayi, this.okeyTasi) + " (Beklenen: False)");


                if (sahteOkeyTest != null)
                {
                    List<Tas> per1SahteOkeyli = new List<Tas> { kirmizi7, mavi7, sahteOkeyTest };
                    Debug.Log("Test 7 [Geçerli Per (K7,M7,SO) - 1 SahteOkeyli]: " + OkeyRulesEngine.GecerliPerMi(per1SahteOkeyli, this.okeyTasi) + " (Beklenen: True)");


                    if (this.okeyTasi != null && this.okeyTasi.tip == TasTipi.Sayi && this.okeyTasi != kirmizi7 && this.okeyTasi != mavi7)
                    {
                        List<Tas> per1GercekOkeyli = new List<Tas> { kirmizi7, mavi7, this.okeyTasi };
                        Debug.Log("Test 8 [Geçerli Per (K7,M7,Okey) - 1 Gerçek Okeyli]: " + OkeyRulesEngine.GecerliPerMi(per1GercekOkeyli, this.okeyTasi) + " (Beklenen: True)");
                    }
                    else if (this.okeyTasi != null && this.okeyTasi.tip == TasTipi.SahteOkey)
                    {
                        List<Tas> per1GercekOkeyli = new List<Tas> { kirmizi7, mavi7, this.okeyTasi };
                        Debug.Log("Test 8 [Geçerli Per (K7,M7,Okey<-SahteOkey) - Okey=SahteOkey]: " + OkeyRulesEngine.GecerliPerMi(per1GercekOkeyli, this.okeyTasi) + " (Beklenen: True)");
                    }
                    else
                    {
                        Debug.LogWarning("Test 8 atlandý: Okey taþý test için uygun deðil (K7, M7 veya null olabilir) veya Sahte Okey deðil.");
                    }



                    if (this.okeyTasi != null && sahteOkeyTest != this.okeyTasi)
                    {
                        List<Tas> per2Jokerli = new List<Tas> { kirmizi7, sahteOkeyTest, this.okeyTasi };
                        Debug.Log("Test 9 [Geçersiz Per (K7,SO,Okey) - 2 Jokerli]: " + OkeyRulesEngine.GecerliPerMi(per2Jokerli, this.okeyTasi) + " (Beklenen: False)");
                    }
                    else
                    {
                        Debug.LogWarning("Test 9 atlandý: Okey taþý ve SahteOkey ayný olabilir veya biri null.");
                    }


                    List<Tas> per4SahteOkeyli = new List<Tas> { kirmizi7, mavi7, siyah7, sahteOkeyTest };
                    Debug.Log("Test 10 [Geçerli Per (K7,M7,S7,SO) - 1 SahteOkeyli]: " + OkeyRulesEngine.GecerliPerMi(per4SahteOkeyli, this.okeyTasi) + " (Beklenen: True)");

                }
                else
                {
                    Debug.LogWarning("Jokerli per testlerinin bazýlarý atlandý: TasVeritabani'nda SahteOkey bulunamadý.");
                }
            }
        }
        else
        {
            Debug.LogError("Per kontrol testleri için TasVeritabani düzgün yüklenemedi veya yeterli sayýda/çeþitte taþ yok.");
        }
        Debug.Log("--- Per Kontrol Testleri Bitti (Jokerli) ---");

        Debug.Log("--- Seri Kontrol Testleri Baþlýyor (Jokerli) ---");
        Debug.Log("Bu el için belirlenen Okey Taþý (Seri Testi): " + (this.okeyTasi != null ? this.okeyTasi.ToString() : "BELÝRLENEMEDÝ"));

        if (desteManager != null && desteManager.TasVeritabani != null && desteManager.TasVeritabani.tumTaslar != null && this.okeyTasi != null && this.okeyTasi.tip == TasTipi.Sayi) // Okey taþý düzgün belirlenmiþ olmalý
        {
            // Test senaryolarý için örnek taþlar (bazýlarýný per testinden tekrar kullanabiliriz veya yenilerini tanýmlayabiliriz)
            Tas k5 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Kirmizi && t.sayi == 5 && t.tip == TasTipi.Sayi);
            Tas k6 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Kirmizi && t.sayi == 6 && t.tip == TasTipi.Sayi);
            Tas k7 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Kirmizi && t.sayi == 7 && t.tip == TasTipi.Sayi);
            Tas k8 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Kirmizi && t.sayi == 8 && t.tip == TasTipi.Sayi);

            Tas m10 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Mavi && t.sayi == 10 && t.tip == TasTipi.Sayi);
            Tas m11 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Mavi && t.sayi == 11 && t.tip == TasTipi.Sayi);
            Tas m12 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Mavi && t.sayi == 12 && t.tip == TasTipi.Sayi);

            Tas sahteOkey1 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.tip == TasTipi.SahteOkey);
            // Ýkinci bir Sahte Okey bulalým (eðer varsa ve ilk bulduðumuzdan farklýysa)
            Tas sahteOkey2 = desteManager.TasVeritabani.tumTaslar.LastOrDefault(t => t != null && t.tip == TasTipi.SahteOkey);
            if (sahteOkey1 == sahteOkey2 && desteManager.TasVeritabani.tumTaslar.Count(t => t != null && t.tip == TasTipi.SahteOkey) < 2)
            {
                // Eðer sadece bir Sahte Okey tanýmý varsa veya ikisi aynýysa, ikinciyi null yapalým ki testler karýþmasýn.
                // Pratikte oyuncunun elinde iki farklý Sahte Okey olabilir. Test için farklý referanslar önemli.
                // Bu test kurgusu için, eðer veritabanýnda iki farklý SO yoksa, bazý testler doðru çalýþmayabilir.
                // Þimdilik tek SO ile idare edelim, testleri ona göre yorumlarýz.
                sahteOkey2 = null;
            }


            if (k5 == null || k6 == null || k7 == null || m10 == null || m11 == null || m12 == null || sahteOkey1 == null)
            {
                Debug.LogError("Jokerli Seri testleri için gerekli temel taþlar TasVeritabani'nda bulunamadý!");
            }
            else
            {
                // Test S9: Geçerli Seri - 1 Gerçek Okey ile (K5, Okey, K7)
                // Varsayým: this.okeyTasi, K6'nýn yerine geçebilecek bir wildcard.
                List<Tas> seriOkeyli1 = new List<Tas> { k5, this.okeyTasi, k7 };
                Debug.Log("Test S9 [Geçerli Seri (K5, Okey, K7)]: " + OkeyRulesEngine.GecerliSeriMi(seriOkeyli1, this.okeyTasi) + " (Beklenen: True)");

                // Test S10: Geçerli Seri - 1 Sahte Okey ile (M10, SahteOkey, M12)
                // Sahte Okey, this.okeyTasi'ný temsil edecek. this.okeyTasi'nýn Mavi 11 olduðunu varsayalým.
                // Bu testin sonucu, this.okeyTasi'nýn ne olduðuna çok baðlý.
                // Eðer this.okeyTasi Mavi 11 ise True döner. Deðilse False döner.
                // Þimdilik genel bir test olarak býrakalým, sonucunu o anki okeyTasi'na göre yorumlarýz.
                List<Tas> seriSahteOkeyli1 = new List<Tas> { m10, sahteOkey1, m12 };
                Debug.Log("Test S10 [Geçerli Seri (M10, SO, M12) - SO, Okey'i temsil eder]: " + OkeyRulesEngine.GecerliSeriMi(seriSahteOkeyli1, this.okeyTasi) + " (Sonuç Okey'e baðlý)");

                // Test S11: Geçerli Seri - Tamamen Gerçek Okeylerden (Okey, Okey, Okey)
                List<Tas> seriFullOkey = new List<Tas> { this.okeyTasi, this.okeyTasi, this.okeyTasi };
                Debug.Log("Test S11 [Geçerli Seri (Okey,Okey,Okey)]: " + OkeyRulesEngine.GecerliSeriMi(seriFullOkey, this.okeyTasi) + " (Beklenen: True)");

                // Test S12: Geçerli Seri - Sahte Okeyler Okey'i temsil ediyor (SO, SO, SO) -> Okey, Okey, Okey
                // Bu test için en az 2 farklý Sahte Okey'e (veya ayný SO'dan 3 tane) ihtiyacýmýz var.
                // Þimdilik bir tane SO ve iki okeyTasi ile deneyelim, eðer okeyTasi SO deðilse:
                if (sahteOkey1 != this.okeyTasi && sahteOkey2 != null && sahteOkey2 != this.okeyTasi && sahteOkey1 != sahteOkey2) // Farklý 2 SO ve bir Okey
                { // Bu test mantýðý biraz karýþýk, çünkü Sahte Okeyler okeyTasiGercek'e dönüþüyor.
                  // Eðer potansiyelSeri = {SO1, SO2, GercekOkey} ise ve GercekOkey = K8 ise,
                  // islenecekTaslar = {K8, K8}, wildcardAdedi = 1 olur.
                  // benzersizSayilar = {K8}. min=8, max=8. teorikUzunluk=1. gerekenWildcard=0.
                  // wildcardAdedi (1) >= gerekenWildcard (0) -> True.
                  // (benzersizSayilar.Count (1) + wildcardAdedi (1)) == potansiyelSeri.Count (3) -> 2 != 3 -> False. Bu test böyle False çýkar.
                  // Eðer {SO, SO, SO} ise ve Okey K8 ise -> islenecekTaslar={K8,K8,K8}, wildcardAdedi=0.
                  // benzersizSayilar={K8}. teorikUzunluk=1. gerekenWildcard=0.
                  // (1+0) == 3 && 1 == 3 -> False. Bu da False çýkar.
                  // Bu nedenle {SO,SO,SO} veya {Okey,Okey,Okey} testleri yukarýdaki Test S11 gibi daha basit olmalý.
                  // Sahte okeylerin okeyi temsil ettiði durumlar için ayrý test yazmak daha iyi.
                }

                // Test S13: Geçersiz Seri - Sahte Okey yanlýþ yerde kullanýlýyor
                // Örn: Okey Kýrmýzý 8. Seri: Mavi 10, Sahte Okey (Kýrmýzý 8'i temsil eder), Mavi 12. Renkler uyuþmaz.
                List<Tas> seriSahteOkeyYanlisRenk = new List<Tas> { m10, sahteOkey1, m12 }; // sahteOkey1'in rengi Mavi deðilse (Kýrmýzý 8 ise) False olmalý.
                                                                                            // Bu test S10 ile ayný ama beklentiyi farklý yorumlayabiliriz.
                                                                                            // Eðer this.okeyTasi.renk != TasRengi.Mavi ise bu False olmalý.
                Debug.Log("Test S13 [Geçersiz Seri (M10, SO(Kýrmýzý ise), M12) - Renk Uyuþmazlýðý]: " + OkeyRulesEngine.GecerliSeriMi(seriSahteOkeyYanlisRenk, this.okeyTasi) + " (Sonuç Okey'e baðlý)");

                // Test S14: Geçersiz Seri - Ýki farklý sayý arasýnda çok fazla boþluk var, yeterli joker yok
                // Okey Kýrmýzý 6. Seri: Kýrmýzý 5, Okey (K6), Kýrmýzý 8 -> Normalde K5,K6,K7,K8 olmalý. Sadece K7 eksik.
                // Ama biz {K5, Okey, K8} veriyoruz. islenecekTaslar={K5,K8}, wildcard=1.
                // benzersiz={5,8}. min=5,max=8. teorik=4. gerekenWildcard=4-2=2. Eldeki wildcard=1. 1<2 -> False. Doðru.
                List<Tas> seriCokBosluk = new List<Tas> { k5, this.okeyTasi, k8 };
                Debug.Log("Test S14 [Geçersiz Seri (K5, Okey, K8) - Yetersiz Joker]: " + OkeyRulesEngine.GecerliSeriMi(seriCokBosluk, this.okeyTasi) + " (Beklenen: False)");
            }
        }
        else
        {
            Debug.LogError("Jokerli Seri kontrol testleri için TasVeritabani veya OkeyTasi düzgün yüklenemedi.");
        }
        Debug.Log("--- Seri Kontrol Testleri Bitti (Jokerli) ---");
        oynananElSayisiBuTurda = 0;
        aktifOyuncuIndex = 0;
        if (oyuncular.Count > 0 && oyuncular[aktifOyuncuIndex].ilkOyuncuMu)
        {
            IlkOyuncuTurunuOyna();
        }
        else if (oyuncular.Count > 0)
        {
            NormalOyuncuTurunuOyna();
        }

        void GostergeVeOkeyBelirle()
        {
            Tas potansiyelGosterge = null;
            int denemeSayaci = 0;
            const int MAKSIMUM_GOSTERGE_DENEMESI = 100;

            do
            {
                if (desteManager.KalanTasSayisi() == 0)
                {
                    Debug.Log("Gosterge Belirlenemedi: Gosterge cekmek icin destede tas yok");
                    this.gostergeTasi = null;
                    this.okeyTasi = null;
                    return;
                }
                potansiyelGosterge = desteManager.TasCek();
                denemeSayaci++;

                if (potansiyelGosterge == null && desteManager.KalanTasSayisi() > 0)
                {
                    Debug.LogWarning("Potansiyel gosterge null geldi ama destede hala tas var. Tekrar Deneniyor. Deneme : " + denemeSayaci);
                    continue;
                }
                if (potansiyelGosterge != null && potansiyelGosterge.tip == TasTipi.SahteOkey)
                {
                    Debug.Log("Gösterge için Sahte Okey çekildi (" + potansiyelGosterge.ToString() + "). Bu taþ geçerli gösterge deðil, kenara ayrýlýyor ve yeni gösterge çekilecek. Deneme: " + denemeSayaci);
                    potansiyelGosterge = null;
                }
                if (denemeSayaci > MAKSIMUM_GOSTERGE_DENEMESI)
                {
                    Debug.LogError("Gosterge Belirlenemedi : Maksimum Deneme Sayisina ulasildi");
                    this.gostergeTasi = null;
                    this.okeyTasi = null;
                    return;
                }
            } while (potansiyelGosterge == null || potansiyelGosterge.tip == TasTipi.SahteOkey);
            this.gostergeTasi = potansiyelGosterge;

            if (this.gostergeTasi == null) // Bu noktaya gelinmemeli ama son bir kontrol.
            {
                Debug.LogError("GÖSTERGE TAÞI HALA NULL! OKEY BELÝRLENEMEYECEK.");
                this.okeyTasi = null;
                return;
            }

            Debug.Log("Gösterge Taþý: " + this.gostergeTasi.ToString());

            Debug.Log("Gösterge Taþý: " + this.gostergeTasi.ToString());

            TasRengi okeyRengi = this.gostergeTasi.renk;
            int okeyNumarasi;

            if (this.gostergeTasi.sayi == 13)
            {
                okeyNumarasi = 1;
            }
            else
            {
                okeyNumarasi = this.gostergeTasi.sayi + 1;
            }

            this.okeyTasi = null;

            if (desteManager.TasVeritabani != null)
            {
                this.okeyTasi = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t =>
                    t != null &&
                    t.tip == TasTipi.Sayi &&
                    t.renk == okeyRengi &&
                    t.sayi == okeyNumarasi);

                if (this.okeyTasi != null)
                {
                    Debug.Log("Okey Taþý: " + this.okeyTasi.ToString());
                }
                else
                {
                    Debug.LogError("Belirlenen okey taþý (" + okeyRengi + " " + okeyNumarasi + ") TasVeritabani'nda bulunamadý!");
                }
            }
            else
            {
                Debug.LogError("Okey taþýný bulmak için TasVeritabani referansý eksik!");
            }
        }
        void IlkOyuncuTurunuOyna()
        {
            if (oyuncular.Count == 0 || aktifOyuncuIndex >= oyuncular.Count) return;
            Oyuncu ilkOyuncu = oyuncular[aktifOyuncuIndex];
            Debug.Log("----" + ilkOyuncu.oyuncuAdi + "(Ýlk Tur) Oynuyor ----");

            if (ilkOyuncu.eli.Count == 22)
            {
                Tas atilanTas = ilkOyuncu.eli[ilkOyuncu.eli.Count - 1];
                ilkOyuncu.eli.RemoveAt(ilkOyuncu.eli.Count - 1);
                atilanTaslarYigini.Add(atilanTas);

                Debug.Log(ilkOyuncu.oyuncuAdi + "su tasi atti: " + atilanTas.ToString());
                ilkOyuncu.EliniGoster();
                if (atilanTaslarYigini.Count > 0)
                {
                    Debug.Log("Ortadaki son tas: " + atilanTaslarYigini[atilanTaslarYigini.Count - 1].ToString());
                }
            }
            else
            {
                Debug.LogWarning(ilkOyuncu.oyuncuAdi + "Ýlk oyuncu olmasina ragmen 22 tasi yok el sayisi: " + ilkOyuncu.eli.Count);

            }
            SonrakiOyuncununHamlesiniBaslat();
        }
        void NormalOyuncuTurunuOyna()
        {
            if (oyuncular.Count == 0 || aktifOyuncuIndex >= oyuncular.Count) return;
            Oyuncu siradakiOyuncu = oyuncular[aktifOyuncuIndex];
            Debug.Log("----" + siradakiOyuncu.oyuncuAdi + "Oynuyor ----");

            bool yandanTasAldi = false;
            Tas sonYereAtilanTas = null;

            if (atilanTaslarYigini.Count > 0)
            {
                sonYereAtilanTas = atilanTaslarYigini[atilanTaslarYigini.Count - 1];
                if (OyuncuYandanAlmakÝsterMi(siradakiOyuncu, sonYereAtilanTas))
                {
                    siradakiOyuncu.TasaEkle(sonYereAtilanTas);
                    atilanTaslarYigini.RemoveAt(atilanTaslarYigini.Count - 1);
                    yandanTasAldi = true;
                    Debug.Log(siradakiOyuncu.oyuncuAdi + "yandan su tasi aldi: " + sonYereAtilanTas.ToString());
                }
            }

            if (!yandanTasAldi)
            {
                if (desteManager.KalanTasSayisi() > 0)
                {
                    Tas cekilenTas = desteManager.TasCek();
                    siradakiOyuncu.TasaEkle(cekilenTas);
                    Debug.Log(siradakiOyuncu.oyuncuAdi + "yerden su tasi cekti : " + cekilenTas.ToString());
                }
                else
                {
                    Debug.Log(siradakiOyuncu.oyuncuAdi + "yerden tas cekemedi yerde tas yok");
                }
            }
            siradakiOyuncu.EliniGoster();
            if (siradakiOyuncu.eli.Count > 0)
            {
                Tas atilacakTas;
                int atilacakIndex = -1;
                if (siradakiOyuncu.eli.Count == 1)
                {
                    atilacakIndex = 0;
                }
                else
                {
                    Tas sonTas = siradakiOyuncu.eli[siradakiOyuncu.eli.Count - 1];
                    bool sonTasDegerliMi = (this.okeyTasi != null && sonTas == this.okeyTasi) || (sonTas.tip == TasTipi.SahteOkey);
                    if (sonTasDegerliMi)
                    {
                        bool degersizTasBulundu = false;
                        for (int k = 0; k < siradakiOyuncu.eli.Count - 1; k++)
                        {
                            Tas potansiyelTas = siradakiOyuncu.eli[k];
                            bool potansiyelTasDegerliMi = (this.okeyTasi != null && potansiyelTas == this.okeyTasi) || (potansiyelTas.tip == TasTipi.SahteOkey);
                            if (!potansiyelTasDegerliMi)
                            {
                                atilacakIndex = k;
                                degersizTasBulundu = true;
                                break;

                            }
                        }
                        if (!degersizTasBulundu)
                        {
                            atilacakIndex = siradakiOyuncu.eli.Count - 1;
                        }
                    }
                    else
                    {
                        atilacakIndex = siradakiOyuncu.eli.Count - 1;
                    }


                }
                if (atilacakIndex != -1 && atilacakIndex < siradakiOyuncu.eli.Count)
                {
                    atilacakTas = siradakiOyuncu.eli[atilacakIndex];
                    siradakiOyuncu.eli.RemoveAt(siradakiOyuncu.eli.Count - 1);
                    atilanTaslarYigini.Add(atilacakTas);

                    Debug.Log(siradakiOyuncu.oyuncuAdi + "Su tasi atti: " + atilacakTas.ToString());
                    siradakiOyuncu.EliniGoster();
                    if (atilanTaslarYigini.Count > 0)
                    {
                        Debug.Log("Ortadaki Son Tas: " + atilanTaslarYigini[atilanTaslarYigini.Count - 1].ToString());

                    }
                }
            }
            else
            {
                Debug.LogWarning(siradakiOyuncu.oyuncuAdi + "elinde atacak tas kalmadi!");

            }
            SonrakiOyuncununHamlesiniBaslat();
        }

        void SonrakiOyuncununHamlesiniBaslat()
        {
            oynananElSayisiBuTurda++;


            if (oynananElSayisiBuTurda >= oyuncuSayisi)
            {
                Debug.Log("---- BÝR TAM TUR BÝTTÝ (HER OYUNCU BIR EL OYNADI) ----");

                return;
            }
            SiradakiOyuncuyaGec();
        }
        void SiradakiOyuncuyaGec()
        {
            aktifOyuncuIndex = (aktifOyuncuIndex + 1) % oyuncuSayisi;
            Debug.Log(">>>>>Sira simdi " + oyuncular[aktifOyuncuIndex].oyuncuAdi + "oyuncusunda<<<<");

            if (atilanTaslarYigini.Count > 0)
            {
                Debug.Log(oyuncular[aktifOyuncuIndex].oyuncuAdi + ", ortadaki son taþý goruyor: " + atilanTaslarYigini[atilanTaslarYigini.Count - 1].ToString());
            }
            else
            {
                Debug.Log(oyuncular[aktifOyuncuIndex].oyuncuAdi + ", ortada henuz atilmis tas yok");
            }

            NormalOyuncuTurunuOyna();

        }
        bool OyuncuYandanAlmakÝsterMi(Oyuncu oyuncu, Tas yandanAlinabilecekTas)
        {
            if (yandanAlinabilecekTas == null) return false;
            Debug.Log("[KARAR] " + oyuncu.oyuncuAdi + " oyuncusu, yandan alýnabilecek " + yandanAlinabilecekTas.ToString() + " taþý için karar veriyor...");

            if (this.okeyTasi != null && yandanAlinabilecekTas == this.okeyTasi)
            {
                Debug.Log("[KARAR] " + oyuncu.oyuncuAdi + ", " + yandanAlinabilecekTas.ToString() + " taþýný YANDAN ALIYOR (BU ELÝN OKEY TAÞI!).");
                return true;
            }

            if (yandanAlinabilecekTas.tip == TasTipi.SahteOkey)
            {
                Debug.Log("[KARAR] " + oyuncu.oyuncuAdi + ", " + yandanAlinabilecekTas.ToString() + " taþýný YANDAN ALIYOR (BU BÝR SAHTE OKEY!).");
                return true;
            }

            if (yandanAlinabilecekTas.tip == TasTipi.Sayi)
            {
                foreach (Tas eldekiTas in oyuncu.eli)
                {
                    if (eldekiTas.tip == TasTipi.Sayi && eldekiTas.sayi == yandanAlinabilecekTas.sayi)
                    {
                        Debug.Log("[KARAR] " + oyuncu.oyuncuAdi + ", " + yandanAlinabilecekTas.ToString() + " taþýný YANDAN ALIYOR (eldeki " + eldekiTas.ToString() + " ile ayný sayýya sahip).");
                        return true;
                    }
                }
            }
            Debug.Log("[KARAR] " + oyuncu.oyuncuAdi + ", " + yandanAlinabilecekTas.ToString() + " taþýný yandan ALMIYOR (yerden çekecek).");
            return false;
        }

    }
}
