using UnityEngine;
using System.Collections.Generic;
using System.Linq; 

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

    [Header("UI Yonetimi")]
    public UIManager uiManager;

    void Start()
    {
        if (desteManager == null)
        {
            Debug.LogError("GameManager: DesteManager atanmamis!");
            return;
        }
        if (uiManager == null)
        {
            Debug.LogError("GameManager: UIManager atanmamýþ! Lütfen Inspector üzerinden atayýn.");
            return;
        }
        OyunuBaslat();
    }

    public void OyunuBaslat()
    {
        Debug.Log("Oyun Baþlatýlýyor...");

        oyuncular.Clear();
        atilanTaslarYigini.Clear();
        oynananElSayisiBuTurda = 0;
        aktifOyuncuIndex = 0;

        for (int i = 0; i < oyuncuSayisi; i++)
        {
            Oyuncu yeniOyuncu = new Oyuncu("Oyuncu " + (i + 1), i == 0, 1000);
            oyuncular.Add(yeniOyuncu);
        }
        Debug.Log(oyuncuSayisi + " adet oyuncu oluþturuldu.");

        Debug.Log("Taþlar daðýtýlýyor...");
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
                    Debug.LogError("HATA: Deste bitti ancak hala taþ daðýtýlmaya çalýþýlýyor! Oyuncu: " + oyuncular[i].oyuncuAdi);
                    break;
                }
            }
        }

        Debug.Log("Taþ daðýtýmý tamamlandý. Oyuncu elleri (Konsol):");
        foreach (Oyuncu oyuncu in oyuncular)
        {
            Debug.Log(oyuncu.oyuncuAdi + " elindeki taþ sayýsý: " + oyuncu.eli.Count);
             
        }

        GostergeVeOkeyBelirle();

        if (desteManager != null)
        {
            
            Debug.Log("Destede kalan taþ sayýsý (Gösterge sonrasý): " + desteManager.KalanTasSayisi());
        }

        if (uiManager != null && oyuncular.Count > 0)
        {
            uiManager.Oyuncu1EliniGoster(oyuncular[0]);
        }

        // Tur simülasyonunu baþlat
        oynananElSayisiBuTurda = 0;
        aktifOyuncuIndex = 0;

        Debug.Log("Oyun kurulumu tamamlandý. Ýlk tur baþlýyor...");

        if (oyuncular.Count > 0 && oyuncular[aktifOyuncuIndex].ilkOyuncuMu)
        {
            IlkOyuncuTurunuOyna();
        }
        else if (oyuncular.Count > 0)
        {
            NormalOyuncuTurunuOyna();
        }
        else
        {
            Debug.LogError("Oyuncu bulunamadý, oyun baþlatýlamýyor!");
        }
    }

    void GostergeVeOkeyBelirle()
    {
        Tas potansiyelGosterge = null;
        int denemeSayaci = 0;
        const int MAKSIMUM_GOSTERGE_DENEMESI = 110;

        do
        {
            if (desteManager.KalanTasSayisi() == 0)
            {
                Debug.LogError("GÖSTERGE BELÝRLENEMEDÝ: Gösterge çekmek için destede hiç taþ kalmadý!");
                this.gostergeTasi = null;
                this.okeyTasi = null;
                return;
            }

            potansiyelGosterge = desteManager.TasCek();
            denemeSayaci++;

            if (potansiyelGosterge == null && desteManager.KalanTasSayisi() > 0)
            {
                Debug.LogWarning("Potansiyel gösterge null geldi ama destede hala taþ var. Tekrar deneniyor. Deneme: " + denemeSayaci);
                continue;
            }

            if (potansiyelGosterge != null && potansiyelGosterge.tip == TasTipi.SahteOkey)
            {
                Debug.Log("Gösterge için Sahte Okey çekildi (" + potansiyelGosterge.ToString() + "). Bu taþ geçerli gösterge deðil, yeni gösterge çekilecek. Deneme: " + denemeSayaci);
                potansiyelGosterge = null;
            }

            if (denemeSayaci > MAKSIMUM_GOSTERGE_DENEMESI)
            {
                Debug.LogError("GÖSTERGE BELÝRLENEMEDÝ: Maksimum deneme sayýsýna ulaþýldý (" + MAKSIMUM_GOSTERGE_DENEMESI + "). Destede uygun gösterge bulunamadý.");
                this.gostergeTasi = null;
                this.okeyTasi = null;
                return;
            }

        } while (potansiyelGosterge == null || potansiyelGosterge.tip == TasTipi.SahteOkey);

        this.gostergeTasi = potansiyelGosterge;

        if (this.gostergeTasi == null)
        {
            Debug.LogError("GÖSTERGE TAÞI HALA NULL! OKEY BELÝRLENEMEYECEK.");
            this.okeyTasi = null;
            return;
        }

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

    public void OyuncuCiftAcmakIstedi(Oyuncu oyuncu)
    {
        if (oyuncu == null) return;

        if (oyuncu.CiftAcilabilirMi())
        {
            oyuncu.ciftAcilmisMi = true;
            Debug.Log(oyuncu.oyuncuAdi + " çift açtý!");
        }
        else
        {
            Debug.Log(oyuncu.oyuncuAdi + " çift açmayý denedi ama yeterli çifti yok.");
        }
    }

    void IlkOyuncuTurunuOyna()
    {
        if (oyuncular.Count == 0 || aktifOyuncuIndex >= oyuncular.Count || !oyuncular[aktifOyuncuIndex].ilkOyuncuMu)
        {
            Debug.LogError("IlkOyuncuTurunuOyna çaðrýldý ama koþullar uygun deðil.");
            return;
        }

        Oyuncu ilkOyuncu = oyuncular[aktifOyuncuIndex];
        Debug.Log("---- " + ilkOyuncu.oyuncuAdi + " (Ýlk Tur) Oynuyor ----");

        if (ilkOyuncu.eli.Count == 22) 
        {
            int atilacakIndex = -1; 

            if (ilkOyuncu.eli.Count == 1) 
            {
                atilacakIndex = 0;
            }
            else if (ilkOyuncu.eli.Count > 1) 
            {
                Tas sonTas = ilkOyuncu.eli[ilkOyuncu.eli.Count - 1];
                bool sonTasDegerliMi = (this.okeyTasi != null && sonTas == this.okeyTasi) || (sonTas.tip == TasTipi.SahteOkey);

                if (sonTasDegerliMi)
                {
                    bool degersizTasBulundu = false;
                    for (int k = 0; k < ilkOyuncu.eli.Count; k++)
                    {
                        Tas potansiyelTas = ilkOyuncu.eli[k];
                        
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
                        atilacakIndex = ilkOyuncu.eli.Count - 1;
                    }
                }
                else 
                {
                    atilacakIndex = ilkOyuncu.eli.Count - 1;
                }
            }

            
            if (atilacakIndex != -1 && atilacakIndex < ilkOyuncu.eli.Count)
            {
                Tas atilanTas = ilkOyuncu.eli[atilacakIndex]; 

                if (atilanTas != null) 
                {
                    ilkOyuncu.eli.RemoveAt(atilacakIndex); 
                    atilanTaslarYigini.Add(atilanTas);     

                    Debug.Log(ilkOyuncu.oyuncuAdi + " þu taþý attý: " + atilanTas.ToString() + " (elinden index: " + atilacakIndex + ")");

                    if (atilanTaslarYigini.Count > 0)
                        Debug.Log("Yandaki Son Taþ: " + atilanTaslarYigini[atilanTaslarYigini.Count - 1].ToString());

                    if (aktifOyuncuIndex == 0 && uiManager != null)
                    {
                        uiManager.Oyuncu1EliniGoster(ilkOyuncu);
                    }
                }
                else
                {
                   
                    Debug.LogError(ilkOyuncu.oyuncuAdi + " elinden null bir taþ atmaya çalýþtý! Index: " + atilacakIndex + ". Bu beklenmedik bir durum, TasVeritabani veya TasaEkle kontrol edilmeli.");
                    
                    ilkOyuncu.eli.RemoveAt(atilacakIndex);
                }
            }
            else
            {
                Debug.LogError(ilkOyuncu.oyuncuAdi + " için ilk turda atýlacak geçerli bir taþ index'i bulunamadý! El Sayýsý: " + ilkOyuncu.eli.Count);
            }
        }
        else
        {
            Debug.LogWarning(ilkOyuncu.oyuncuAdi + " ilk oyuncu olmasýna raðmen 22 (veya >0) taþý yok! El sayýsý: " + ilkOyuncu.eli.Count);
        }

        SonrakiOyuncununHamlesiniBaslat();
    }


    void NormalOyuncuTurunuOyna()
    {
        if (oyuncular.Count == 0 || aktifOyuncuIndex >= oyuncular.Count) return;

        Oyuncu siradakiOyuncu = oyuncular[aktifOyuncuIndex];
        Debug.Log("---- " + siradakiOyuncu.oyuncuAdi + " Oynuyor ----");

        bool yandanTasAldi = false;
        Tas sonYereAtilanTas = null;

        if (atilanTaslarYigini.Count > 0)
        {
            sonYereAtilanTas = atilanTaslarYigini[atilanTaslarYigini.Count - 1];
            if (OyuncuYandanAlmakIsterMi(siradakiOyuncu, sonYereAtilanTas))
            {
                siradakiOyuncu.TasaEkle(sonYereAtilanTas);
                atilanTaslarYigini.RemoveAt(atilanTaslarYigini.Count - 1);
                yandanTasAldi = true;
                Debug.Log(siradakiOyuncu.oyuncuAdi + " yandan þu taþý aldý: " + sonYereAtilanTas.ToString());
            }
        }

        if (!yandanTasAldi)
        {
            if (desteManager.KalanTasSayisi() > 0)
            {
                Tas cekilenTas = desteManager.TasCek();
                
                siradakiOyuncu.TasaEkle(cekilenTas);
                if (cekilenTas != null)
                { 
                    Debug.Log(siradakiOyuncu.oyuncuAdi + " yerden þu taþý çekti: " + cekilenTas.ToString());
                }
                else
                {
                    Debug.LogWarning(siradakiOyuncu.oyuncuAdi + " yerden null bir taþ çekti (deste bitti ve sonrasýnda çekmeye çalýþtý).");
                }
            }
            else
            {
                Debug.LogWarning(siradakiOyuncu.oyuncuAdi + " yerden taþ çekemedi, destede taþ kalmamýþ!");
            }
        }

        if (aktifOyuncuIndex == 0 && uiManager != null)
        {
            uiManager.Oyuncu1EliniGoster(siradakiOyuncu);
        }

      
        Tas atilanTasNormalTur = null; 
        if (siradakiOyuncu.eli.Count > 0)
        {
            int atilacakIndexNormalTur = -1; 

            if (siradakiOyuncu.eli.Count == 1)
            {
                atilacakIndexNormalTur = 0;
            }
            else
            {
                Tas sonTas = siradakiOyuncu.eli[siradakiOyuncu.eli.Count - 1];
                bool sonTasDegerliMi = (this.okeyTasi != null && sonTas == this.okeyTasi) || (sonTas.tip == TasTipi.SahteOkey);
                if (sonTasDegerliMi)
                {
                    bool degersizTasBulundu = false;
                    for (int k = 0; k < siradakiOyuncu.eli.Count; k++)
                    {
                        Tas potansiyelTas = siradakiOyuncu.eli[k];
                        bool potansiyelTasDegerliMi = (this.okeyTasi != null && potansiyelTas == this.okeyTasi) || (potansiyelTas.tip == TasTipi.SahteOkey);
                        if (!potansiyelTasDegerliMi)
                        {
                            atilacakIndexNormalTur = k;
                            degersizTasBulundu = true;
                            break;
                        }
                    }
                    if (!degersizTasBulundu)
                    {
                        atilacakIndexNormalTur = siradakiOyuncu.eli.Count - 1;
                    }
                }
                else
                {
                    atilacakIndexNormalTur = siradakiOyuncu.eli.Count - 1;
                }
            }

            if (atilacakIndexNormalTur != -1 && atilacakIndexNormalTur < siradakiOyuncu.eli.Count)
            {
                atilanTasNormalTur = siradakiOyuncu.eli[atilacakIndexNormalTur];
                siradakiOyuncu.eli.RemoveAt(atilacakIndexNormalTur); 
               
                if (atilanTasNormalTur != null)
                {
                    atilanTaslarYigini.Add(atilanTasNormalTur);
                    Debug.Log(siradakiOyuncu.oyuncuAdi + " þu taþý attý: " + atilanTasNormalTur.ToString() + " (elinden index: " + atilacakIndexNormalTur + ")");
                    if (atilanTaslarYigini.Count > 0)
                        Debug.Log("Yandaki Son Taþ: " + atilanTaslarYigini[atilanTaslarYigini.Count - 1].ToString());
                }
                else
                {
                    Debug.LogError(siradakiOyuncu.oyuncuAdi + " null bir taþ atmaya çalýþtý (Normal Tur)! Atýlacak Index: " + atilacakIndexNormalTur);
                }
            }
            else
            {
                Debug.LogError(siradakiOyuncu.oyuncuAdi + " için atýlacak geçerli bir taþ index'i bulunamadý! El Sayýsý: " + siradakiOyuncu.eli.Count);
            }
        }
        else
        {
            Debug.LogWarning(siradakiOyuncu.oyuncuAdi + " elinde atacak taþý kalmadý!");
        }

        if (aktifOyuncuIndex == 0 && uiManager != null)
        {
            uiManager.Oyuncu1EliniGoster(siradakiOyuncu);
        }
        SonrakiOyuncununHamlesiniBaslat();
    }


    void SonrakiOyuncununHamlesiniBaslat()
    {
        oynananElSayisiBuTurda++;
        if (oynananElSayisiBuTurda >= oyuncuSayisi)
        {
            Debug.Log("---- BÝR TAM TUR BÝTTÝ (Her oyuncu birer el oynadý) ----");
            return;
        }
        SiradakiOyuncuyaGec();
    }

    void SiradakiOyuncuyaGec()
    {
        aktifOyuncuIndex = (aktifOyuncuIndex + 1) % oyuncuSayisi;
        Debug.Log(">>>> Sýra þimdi " + oyuncular[aktifOyuncuIndex].oyuncuAdi + " oyuncusunda. <<<<");

        if (atilanTaslarYigini.Count > 0)
        {
            Debug.Log(oyuncular[aktifOyuncuIndex].oyuncuAdi + ", yandaki son taþý görüyor: " + atilanTaslarYigini[atilanTaslarYigini.Count - 1].ToString());
        }
        else
        {
            Debug.Log(oyuncular[aktifOyuncuIndex].oyuncuAdi + ", yanda henüz atýlmýþ taþ yok.");
        }
        NormalOyuncuTurunuOyna();
    }

    bool OyuncuYandanAlmakIsterMi(Oyuncu oyuncu, Tas yandanAlinabilecekTas)
    {
        if (yandanAlinabilecekTas == null) return false;

        if (this.okeyTasi != null && yandanAlinabilecekTas == this.okeyTasi)
        {
            return true;
        }
        if (yandanAlinabilecekTas.tip == TasTipi.SahteOkey)
        {
            return true;
        }
        if (yandanAlinabilecekTas.tip == TasTipi.Sayi)
        {
            foreach (Tas eldekiTas in oyuncu.eli)
            {
                if (eldekiTas.tip == TasTipi.Sayi && eldekiTas.sayi == yandanAlinabilecekTas.sayi)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool SiraKimdeyseOAtar(Tas atilanTas)
    {
        if (oyuncular.Count == 0 || aktifOyuncuIndex >= oyuncular.Count)
        {
            Debug.LogError("SiraKimdeyseOAtar: Geçerli aktif oyuncu yok!");
            return false;
        }

        Oyuncu oynayanOyuncu = oyuncular[aktifOyuncuIndex];

        
        if (oynayanOyuncu.eli.Contains(atilanTas))
        {
            oynayanOyuncu.eli.Remove(atilanTas);
            atilanTaslarYigini.Add(atilanTas);

            Debug.Log(oynayanOyuncu.oyuncuAdi + " UI üzerinden sürükleyerek þu taþý attý: " + atilanTas.ToString());

            if (atilanTaslarYigini.Count > 0)
                Debug.Log("Yandaki Son Taþ: " + atilanTaslarYigini[atilanTaslarYigini.Count - 1].ToString());

           
            if (aktifOyuncuIndex == 0 && uiManager != null) 
            {
                uiManager.Oyuncu1EliniGoster(oynayanOyuncu);
            }
           

           
            SonrakiOyuncununHamlesiniBaslat(); 
            return true; 
        }
        else
        {
            Debug.LogError(oynayanOyuncu.oyuncuAdi + " elinde olmayan bir taþý (" + atilanTas.ToString() + ") atmaya çalýþtý!");
            
            if (aktifOyuncuIndex == 0 && uiManager != null)
            {
                uiManager.Oyuncu1EliniGoster(oynayanOyuncu);
            }
            return false; 
        }
    }
} 