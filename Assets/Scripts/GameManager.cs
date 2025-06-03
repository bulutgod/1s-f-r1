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

        Debug.Log("--- Per Kontrol Testleri Ba�l�yor (Jokerli) ---");
        Debug.Log("Bu el i�in belirlenen Okey Ta��: " + (this.okeyTasi != null ? this.okeyTasi.ToString() : "BEL�RLENEMED�"));


        if (desteManager != null && desteManager.TasVeritabani != null && desteManager.TasVeritabani.tumTaslar != null && desteManager.TasVeritabani.tumTaslar.Count > 10)
        {

            Tas kirmizi7 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Kirmizi && t.sayi == 7 && t.tip == TasTipi.Sayi);
            Tas mavi7 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Mavi && t.sayi == 7 && t.tip == TasTipi.Sayi);
            Tas siyah7 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Siyah && t.sayi == 7 && t.tip == TasTipi.Sayi);
            Tas sari7 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Sari && t.sayi == 7 && t.tip == TasTipi.Sayi);
            Tas kirmizi8 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Kirmizi && t.sayi == 8 && t.tip == TasTipi.Sayi);
            Tas sahteOkeyTest = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.tip == TasTipi.SahteOkey); // Bir Sahte Okey alal�m


            if (kirmizi7 == null || mavi7 == null || siyah7 == null || sari7 == null || kirmizi8 == null)
            {
                Debug.LogError("Per testleri i�in gerekli temel say�l� ta�lar TasVeritabani'nda bulunamad�!");
            }
            else
            {

                List<Tas> gecerliPer3 = new List<Tas> { kirmizi7, mavi7, siyah7 };
                Debug.Log("Test 1 [Ge�erli Saf Per (3'l� - K7,M7,S7)]: " + OkeyRulesEngine.GecerliPerMi(gecerliPer3, this.okeyTasi) + " (Beklenen: True)");

                List<Tas> gecerliPer4 = new List<Tas> { kirmizi7, mavi7, siyah7, sari7 };
                Debug.Log("Test 2 [Ge�erli Saf Per (4'l� - K7,M7,S7,Sa7)]: " + OkeyRulesEngine.GecerliPerMi(gecerliPer4, this.okeyTasi) + " (Beklenen: True)");

                List<Tas> gecersizPerAzTas = new List<Tas> { kirmizi7, mavi7 };
                Debug.Log("Test 3 [Ge�ersiz Per (Az Ta� - K7,M7)]: " + OkeyRulesEngine.GecerliPerMi(gecersizPerAzTas, this.okeyTasi) + " (Beklenen: False)");

                List<Tas> gecersizPerAyniRenk = new List<Tas> { kirmizi7, mavi7, kirmizi7 };
                Debug.Log("Test 5 [Ge�ersiz Per (Ayn� Renk Tekrar� - K7,M7,K7)]: " + OkeyRulesEngine.GecerliPerMi(gecersizPerAyniRenk, this.okeyTasi) + " (Beklenen: False)");

                List<Tas> gecersizPerFarkliSayi = new List<Tas> { kirmizi7, mavi7, kirmizi8 };
                Debug.Log("Test 6 [Ge�ersiz Per (Farkl� Say�lar - K7,M7,K8)]: " + OkeyRulesEngine.GecerliPerMi(gecersizPerFarkliSayi, this.okeyTasi) + " (Beklenen: False)");


                if (sahteOkeyTest != null)
                {
                    List<Tas> per1SahteOkeyli = new List<Tas> { kirmizi7, mavi7, sahteOkeyTest };
                    Debug.Log("Test 7 [Ge�erli Per (K7,M7,SO) - 1 SahteOkeyli]: " + OkeyRulesEngine.GecerliPerMi(per1SahteOkeyli, this.okeyTasi) + " (Beklenen: True)");


                    if (this.okeyTasi != null && this.okeyTasi.tip == TasTipi.Sayi && this.okeyTasi != kirmizi7 && this.okeyTasi != mavi7)
                    {
                        List<Tas> per1GercekOkeyli = new List<Tas> { kirmizi7, mavi7, this.okeyTasi };
                        Debug.Log("Test 8 [Ge�erli Per (K7,M7,Okey) - 1 Ger�ek Okeyli]: " + OkeyRulesEngine.GecerliPerMi(per1GercekOkeyli, this.okeyTasi) + " (Beklenen: True)");
                    }
                    else if (this.okeyTasi != null && this.okeyTasi.tip == TasTipi.SahteOkey)
                    {
                        List<Tas> per1GercekOkeyli = new List<Tas> { kirmizi7, mavi7, this.okeyTasi };
                        Debug.Log("Test 8 [Ge�erli Per (K7,M7,Okey<-SahteOkey) - Okey=SahteOkey]: " + OkeyRulesEngine.GecerliPerMi(per1GercekOkeyli, this.okeyTasi) + " (Beklenen: True)");
                    }
                    else
                    {
                        Debug.LogWarning("Test 8 atland�: Okey ta�� test i�in uygun de�il (K7, M7 veya null olabilir) veya Sahte Okey de�il.");
                    }



                    if (this.okeyTasi != null && sahteOkeyTest != this.okeyTasi)
                    {
                        List<Tas> per2Jokerli = new List<Tas> { kirmizi7, sahteOkeyTest, this.okeyTasi };
                        Debug.Log("Test 9 [Ge�ersiz Per (K7,SO,Okey) - 2 Jokerli]: " + OkeyRulesEngine.GecerliPerMi(per2Jokerli, this.okeyTasi) + " (Beklenen: False)");
                    }
                    else
                    {
                        Debug.LogWarning("Test 9 atland�: Okey ta�� ve SahteOkey ayn� olabilir veya biri null.");
                    }


                    List<Tas> per4SahteOkeyli = new List<Tas> { kirmizi7, mavi7, siyah7, sahteOkeyTest };
                    Debug.Log("Test 10 [Ge�erli Per (K7,M7,S7,SO) - 1 SahteOkeyli]: " + OkeyRulesEngine.GecerliPerMi(per4SahteOkeyli, this.okeyTasi) + " (Beklenen: True)");

                }
                else
                {
                    Debug.LogWarning("Jokerli per testlerinin baz�lar� atland�: TasVeritabani'nda SahteOkey bulunamad�.");
                }
            }
        }
        else
        {
            Debug.LogError("Per kontrol testleri i�in TasVeritabani d�zg�n y�klenemedi veya yeterli say�da/�e�itte ta� yok.");
        }
        Debug.Log("--- Per Kontrol Testleri Bitti (Jokerli) ---");

        Debug.Log("--- Seri Kontrol Testleri Ba�l�yor (Jokerli) ---");
        Debug.Log("Bu el i�in belirlenen Okey Ta�� (Seri Testi): " + (this.okeyTasi != null ? this.okeyTasi.ToString() : "BEL�RLENEMED�"));

        if (desteManager != null && desteManager.TasVeritabani != null && desteManager.TasVeritabani.tumTaslar != null && this.okeyTasi != null && this.okeyTasi.tip == TasTipi.Sayi) // Okey ta�� d�zg�n belirlenmi� olmal�
        {
            // Test senaryolar� i�in �rnek ta�lar (baz�lar�n� per testinden tekrar kullanabiliriz veya yenilerini tan�mlayabiliriz)
            Tas k5 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Kirmizi && t.sayi == 5 && t.tip == TasTipi.Sayi);
            Tas k6 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Kirmizi && t.sayi == 6 && t.tip == TasTipi.Sayi);
            Tas k7 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Kirmizi && t.sayi == 7 && t.tip == TasTipi.Sayi);
            Tas k8 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Kirmizi && t.sayi == 8 && t.tip == TasTipi.Sayi);

            Tas m10 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Mavi && t.sayi == 10 && t.tip == TasTipi.Sayi);
            Tas m11 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Mavi && t.sayi == 11 && t.tip == TasTipi.Sayi);
            Tas m12 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.renk == TasRengi.Mavi && t.sayi == 12 && t.tip == TasTipi.Sayi);

            Tas sahteOkey1 = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t => t != null && t.tip == TasTipi.SahteOkey);
            // �kinci bir Sahte Okey bulal�m (e�er varsa ve ilk buldu�umuzdan farkl�ysa)
            Tas sahteOkey2 = desteManager.TasVeritabani.tumTaslar.LastOrDefault(t => t != null && t.tip == TasTipi.SahteOkey);
            if (sahteOkey1 == sahteOkey2 && desteManager.TasVeritabani.tumTaslar.Count(t => t != null && t.tip == TasTipi.SahteOkey) < 2)
            {
                // E�er sadece bir Sahte Okey tan�m� varsa veya ikisi ayn�ysa, ikinciyi null yapal�m ki testler kar��mas�n.
                // Pratikte oyuncunun elinde iki farkl� Sahte Okey olabilir. Test i�in farkl� referanslar �nemli.
                // Bu test kurgusu i�in, e�er veritaban�nda iki farkl� SO yoksa, baz� testler do�ru �al��mayabilir.
                // �imdilik tek SO ile idare edelim, testleri ona g�re yorumlar�z.
                sahteOkey2 = null;
            }


            if (k5 == null || k6 == null || k7 == null || m10 == null || m11 == null || m12 == null || sahteOkey1 == null)
            {
                Debug.LogError("Jokerli Seri testleri i�in gerekli temel ta�lar TasVeritabani'nda bulunamad�!");
            }
            else
            {
                // Test S9: Ge�erli Seri - 1 Ger�ek Okey ile (K5, Okey, K7)
                // Varsay�m: this.okeyTasi, K6'n�n yerine ge�ebilecek bir wildcard.
                List<Tas> seriOkeyli1 = new List<Tas> { k5, this.okeyTasi, k7 };
                Debug.Log("Test S9 [Ge�erli Seri (K5, Okey, K7)]: " + OkeyRulesEngine.GecerliSeriMi(seriOkeyli1, this.okeyTasi) + " (Beklenen: True)");

                // Test S10: Ge�erli Seri - 1 Sahte Okey ile (M10, SahteOkey, M12)
                // Sahte Okey, this.okeyTasi'n� temsil edecek. this.okeyTasi'n�n Mavi 11 oldu�unu varsayal�m.
                // Bu testin sonucu, this.okeyTasi'n�n ne oldu�una �ok ba�l�.
                // E�er this.okeyTasi Mavi 11 ise True d�ner. De�ilse False d�ner.
                // �imdilik genel bir test olarak b�rakal�m, sonucunu o anki okeyTasi'na g�re yorumlar�z.
                List<Tas> seriSahteOkeyli1 = new List<Tas> { m10, sahteOkey1, m12 };
                Debug.Log("Test S10 [Ge�erli Seri (M10, SO, M12) - SO, Okey'i temsil eder]: " + OkeyRulesEngine.GecerliSeriMi(seriSahteOkeyli1, this.okeyTasi) + " (Sonu� Okey'e ba�l�)");

                // Test S11: Ge�erli Seri - Tamamen Ger�ek Okeylerden (Okey, Okey, Okey)
                List<Tas> seriFullOkey = new List<Tas> { this.okeyTasi, this.okeyTasi, this.okeyTasi };
                Debug.Log("Test S11 [Ge�erli Seri (Okey,Okey,Okey)]: " + OkeyRulesEngine.GecerliSeriMi(seriFullOkey, this.okeyTasi) + " (Beklenen: True)");

                // Test S12: Ge�erli Seri - Sahte Okeyler Okey'i temsil ediyor (SO, SO, SO) -> Okey, Okey, Okey
                // Bu test i�in en az 2 farkl� Sahte Okey'e (veya ayn� SO'dan 3 tane) ihtiyac�m�z var.
                // �imdilik bir tane SO ve iki okeyTasi ile deneyelim, e�er okeyTasi SO de�ilse:
                if (sahteOkey1 != this.okeyTasi && sahteOkey2 != null && sahteOkey2 != this.okeyTasi && sahteOkey1 != sahteOkey2) // Farkl� 2 SO ve bir Okey
                { // Bu test mant��� biraz kar���k, ��nk� Sahte Okeyler okeyTasiGercek'e d�n���yor.
                  // E�er potansiyelSeri = {SO1, SO2, GercekOkey} ise ve GercekOkey = K8 ise,
                  // islenecekTaslar = {K8, K8}, wildcardAdedi = 1 olur.
                  // benzersizSayilar = {K8}. min=8, max=8. teorikUzunluk=1. gerekenWildcard=0.
                  // wildcardAdedi (1) >= gerekenWildcard (0) -> True.
                  // (benzersizSayilar.Count (1) + wildcardAdedi (1)) == potansiyelSeri.Count (3) -> 2 != 3 -> False. Bu test b�yle False ��kar.
                  // E�er {SO, SO, SO} ise ve Okey K8 ise -> islenecekTaslar={K8,K8,K8}, wildcardAdedi=0.
                  // benzersizSayilar={K8}. teorikUzunluk=1. gerekenWildcard=0.
                  // (1+0) == 3 && 1 == 3 -> False. Bu da False ��kar.
                  // Bu nedenle {SO,SO,SO} veya {Okey,Okey,Okey} testleri yukar�daki Test S11 gibi daha basit olmal�.
                  // Sahte okeylerin okeyi temsil etti�i durumlar i�in ayr� test yazmak daha iyi.
                }

                // Test S13: Ge�ersiz Seri - Sahte Okey yanl�� yerde kullan�l�yor
                // �rn: Okey K�rm�z� 8. Seri: Mavi 10, Sahte Okey (K�rm�z� 8'i temsil eder), Mavi 12. Renkler uyu�maz.
                List<Tas> seriSahteOkeyYanlisRenk = new List<Tas> { m10, sahteOkey1, m12 }; // sahteOkey1'in rengi Mavi de�ilse (K�rm�z� 8 ise) False olmal�.
                                                                                            // Bu test S10 ile ayn� ama beklentiyi farkl� yorumlayabiliriz.
                                                                                            // E�er this.okeyTasi.renk != TasRengi.Mavi ise bu False olmal�.
                Debug.Log("Test S13 [Ge�ersiz Seri (M10, SO(K�rm�z� ise), M12) - Renk Uyu�mazl���]: " + OkeyRulesEngine.GecerliSeriMi(seriSahteOkeyYanlisRenk, this.okeyTasi) + " (Sonu� Okey'e ba�l�)");

                // Test S14: Ge�ersiz Seri - �ki farkl� say� aras�nda �ok fazla bo�luk var, yeterli joker yok
                // Okey K�rm�z� 6. Seri: K�rm�z� 5, Okey (K6), K�rm�z� 8 -> Normalde K5,K6,K7,K8 olmal�. Sadece K7 eksik.
                // Ama biz {K5, Okey, K8} veriyoruz. islenecekTaslar={K5,K8}, wildcard=1.
                // benzersiz={5,8}. min=5,max=8. teorik=4. gerekenWildcard=4-2=2. Eldeki wildcard=1. 1<2 -> False. Do�ru.
                List<Tas> seriCokBosluk = new List<Tas> { k5, this.okeyTasi, k8 };
                Debug.Log("Test S14 [Ge�ersiz Seri (K5, Okey, K8) - Yetersiz Joker]: " + OkeyRulesEngine.GecerliSeriMi(seriCokBosluk, this.okeyTasi) + " (Beklenen: False)");
            }
        }
        else
        {
            Debug.LogError("Jokerli Seri kontrol testleri i�in TasVeritabani veya OkeyTasi d�zg�n y�klenemedi.");
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
                    Debug.Log("G�sterge i�in Sahte Okey �ekildi (" + potansiyelGosterge.ToString() + "). Bu ta� ge�erli g�sterge de�il, kenara ayr�l�yor ve yeni g�sterge �ekilecek. Deneme: " + denemeSayaci);
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
                Debug.LogError("G�STERGE TA�I HALA NULL! OKEY BEL�RLENEMEYECEK.");
                this.okeyTasi = null;
                return;
            }

            Debug.Log("G�sterge Ta��: " + this.gostergeTasi.ToString());

            Debug.Log("G�sterge Ta��: " + this.gostergeTasi.ToString());

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
                    Debug.Log("Okey Ta��: " + this.okeyTasi.ToString());
                }
                else
                {
                    Debug.LogError("Belirlenen okey ta�� (" + okeyRengi + " " + okeyNumarasi + ") TasVeritabani'nda bulunamad�!");
                }
            }
            else
            {
                Debug.LogError("Okey ta��n� bulmak i�in TasVeritabani referans� eksik!");
            }
        }
        void IlkOyuncuTurunuOyna()
        {
            if (oyuncular.Count == 0 || aktifOyuncuIndex >= oyuncular.Count) return;
            Oyuncu ilkOyuncu = oyuncular[aktifOyuncuIndex];
            Debug.Log("----" + ilkOyuncu.oyuncuAdi + "(�lk Tur) Oynuyor ----");

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
                Debug.LogWarning(ilkOyuncu.oyuncuAdi + "�lk oyuncu olmasina ragmen 22 tasi yok el sayisi: " + ilkOyuncu.eli.Count);

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
                if (OyuncuYandanAlmak�sterMi(siradakiOyuncu, sonYereAtilanTas))
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
                Debug.Log("---- B�R TAM TUR B�TT� (HER OYUNCU BIR EL OYNADI) ----");

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
                Debug.Log(oyuncular[aktifOyuncuIndex].oyuncuAdi + ", ortadaki son ta�� goruyor: " + atilanTaslarYigini[atilanTaslarYigini.Count - 1].ToString());
            }
            else
            {
                Debug.Log(oyuncular[aktifOyuncuIndex].oyuncuAdi + ", ortada henuz atilmis tas yok");
            }

            NormalOyuncuTurunuOyna();

        }
        bool OyuncuYandanAlmak�sterMi(Oyuncu oyuncu, Tas yandanAlinabilecekTas)
        {
            if (yandanAlinabilecekTas == null) return false;
            Debug.Log("[KARAR] " + oyuncu.oyuncuAdi + " oyuncusu, yandan al�nabilecek " + yandanAlinabilecekTas.ToString() + " ta�� i�in karar veriyor...");

            if (this.okeyTasi != null && yandanAlinabilecekTas == this.okeyTasi)
            {
                Debug.Log("[KARAR] " + oyuncu.oyuncuAdi + ", " + yandanAlinabilecekTas.ToString() + " ta��n� YANDAN ALIYOR (BU EL�N OKEY TA�I!).");
                return true;
            }

            if (yandanAlinabilecekTas.tip == TasTipi.SahteOkey)
            {
                Debug.Log("[KARAR] " + oyuncu.oyuncuAdi + ", " + yandanAlinabilecekTas.ToString() + " ta��n� YANDAN ALIYOR (BU B�R SAHTE OKEY!).");
                return true;
            }

            if (yandanAlinabilecekTas.tip == TasTipi.Sayi)
            {
                foreach (Tas eldekiTas in oyuncu.eli)
                {
                    if (eldekiTas.tip == TasTipi.Sayi && eldekiTas.sayi == yandanAlinabilecekTas.sayi)
                    {
                        Debug.Log("[KARAR] " + oyuncu.oyuncuAdi + ", " + yandanAlinabilecekTas.ToString() + " ta��n� YANDAN ALIYOR (eldeki " + eldekiTas.ToString() + " ile ayn� say�ya sahip).");
                        return true;
                    }
                }
            }
            Debug.Log("[KARAR] " + oyuncu.oyuncuAdi + ", " + yandanAlinabilecekTas.ToString() + " ta��n� yandan ALMIYOR (yerden �ekecek).");
            return false;
        }

    }
}
