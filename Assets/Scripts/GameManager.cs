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
            Debug.LogError("GameManager: UIManager atanmam��! L�tfen Inspector �zerinden atay�n.");
            return;
        }
        OyunuBaslat();
    }

    public void OyunuBaslat()
    {
        Debug.Log("Oyun Ba�lat�l�yor...");

        oyuncular.Clear();
        atilanTaslarYigini.Clear();
        oynananElSayisiBuTurda = 0;
        aktifOyuncuIndex = 0;

        for (int i = 0; i < oyuncuSayisi; i++)
        {
            Oyuncu yeniOyuncu = new Oyuncu("Oyuncu " + (i + 1), i == 0, 1000);
            oyuncular.Add(yeniOyuncu);
        }
        Debug.Log(oyuncuSayisi + " adet oyuncu olu�turuldu.");

        Debug.Log("Ta�lar da��t�l�yor...");
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
                    Debug.LogError("HATA: Deste bitti ancak hala ta� da��t�lmaya �al���l�yor! Oyuncu: " + oyuncular[i].oyuncuAdi);
                    break;
                }
            }
        }

        Debug.Log("Ta� da��t�m� tamamland�. Oyuncu elleri (Konsol):");
        foreach (Oyuncu oyuncu in oyuncular)
        {
            Debug.Log(oyuncu.oyuncuAdi + " elindeki ta� say�s�: " + oyuncu.eli.Count);
             
        }

        GostergeVeOkeyBelirle();

        if (desteManager != null)
        {
            
            Debug.Log("Destede kalan ta� say�s� (G�sterge sonras�): " + desteManager.KalanTasSayisi());
        }

        if (uiManager != null && oyuncular.Count > 0)
        {
            uiManager.Oyuncu1EliniGoster(oyuncular[0]);
        }

        // Tur sim�lasyonunu ba�lat
        oynananElSayisiBuTurda = 0;
        aktifOyuncuIndex = 0;

        Debug.Log("Oyun kurulumu tamamland�. �lk tur ba�l�yor...");

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
            Debug.LogError("Oyuncu bulunamad�, oyun ba�lat�lam�yor!");
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
                Debug.LogError("G�STERGE BEL�RLENEMED�: G�sterge �ekmek i�in destede hi� ta� kalmad�!");
                this.gostergeTasi = null;
                this.okeyTasi = null;
                return;
            }

            potansiyelGosterge = desteManager.TasCek();
            denemeSayaci++;

            if (potansiyelGosterge == null && desteManager.KalanTasSayisi() > 0)
            {
                Debug.LogWarning("Potansiyel g�sterge null geldi ama destede hala ta� var. Tekrar deneniyor. Deneme: " + denemeSayaci);
                continue;
            }

            if (potansiyelGosterge != null && potansiyelGosterge.tip == TasTipi.SahteOkey)
            {
                Debug.Log("G�sterge i�in Sahte Okey �ekildi (" + potansiyelGosterge.ToString() + "). Bu ta� ge�erli g�sterge de�il, yeni g�sterge �ekilecek. Deneme: " + denemeSayaci);
                potansiyelGosterge = null;
            }

            if (denemeSayaci > MAKSIMUM_GOSTERGE_DENEMESI)
            {
                Debug.LogError("G�STERGE BEL�RLENEMED�: Maksimum deneme say�s�na ula��ld� (" + MAKSIMUM_GOSTERGE_DENEMESI + "). Destede uygun g�sterge bulunamad�.");
                this.gostergeTasi = null;
                this.okeyTasi = null;
                return;
            }

        } while (potansiyelGosterge == null || potansiyelGosterge.tip == TasTipi.SahteOkey);

        this.gostergeTasi = potansiyelGosterge;

        if (this.gostergeTasi == null)
        {
            Debug.LogError("G�STERGE TA�I HALA NULL! OKEY BEL�RLENEMEYECEK.");
            this.okeyTasi = null;
            return;
        }

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

    public void OyuncuCiftAcmakIstedi(Oyuncu oyuncu)
    {
        if (oyuncu == null) return;

        if (oyuncu.CiftAcilabilirMi())
        {
            oyuncu.ciftAcilmisMi = true;
            Debug.Log(oyuncu.oyuncuAdi + " �ift a�t�!");
        }
        else
        {
            Debug.Log(oyuncu.oyuncuAdi + " �ift a�may� denedi ama yeterli �ifti yok.");
        }
    }

    void IlkOyuncuTurunuOyna()
    {
        if (oyuncular.Count == 0 || aktifOyuncuIndex >= oyuncular.Count || !oyuncular[aktifOyuncuIndex].ilkOyuncuMu)
        {
            Debug.LogError("IlkOyuncuTurunuOyna �a�r�ld� ama ko�ullar uygun de�il.");
            return;
        }

        Oyuncu ilkOyuncu = oyuncular[aktifOyuncuIndex];
        Debug.Log("---- " + ilkOyuncu.oyuncuAdi + " (�lk Tur) Oynuyor ----");

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

                    Debug.Log(ilkOyuncu.oyuncuAdi + " �u ta�� att�: " + atilanTas.ToString() + " (elinden index: " + atilacakIndex + ")");

                    if (atilanTaslarYigini.Count > 0)
                        Debug.Log("Yandaki Son Ta�: " + atilanTaslarYigini[atilanTaslarYigini.Count - 1].ToString());

                    if (aktifOyuncuIndex == 0 && uiManager != null)
                    {
                        uiManager.Oyuncu1EliniGoster(ilkOyuncu);
                    }
                }
                else
                {
                   
                    Debug.LogError(ilkOyuncu.oyuncuAdi + " elinden null bir ta� atmaya �al��t�! Index: " + atilacakIndex + ". Bu beklenmedik bir durum, TasVeritabani veya TasaEkle kontrol edilmeli.");
                    
                    ilkOyuncu.eli.RemoveAt(atilacakIndex);
                }
            }
            else
            {
                Debug.LogError(ilkOyuncu.oyuncuAdi + " i�in ilk turda at�lacak ge�erli bir ta� index'i bulunamad�! El Say�s�: " + ilkOyuncu.eli.Count);
            }
        }
        else
        {
            Debug.LogWarning(ilkOyuncu.oyuncuAdi + " ilk oyuncu olmas�na ra�men 22 (veya >0) ta�� yok! El say�s�: " + ilkOyuncu.eli.Count);
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
                Debug.Log(siradakiOyuncu.oyuncuAdi + " yandan �u ta�� ald�: " + sonYereAtilanTas.ToString());
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
                    Debug.Log(siradakiOyuncu.oyuncuAdi + " yerden �u ta�� �ekti: " + cekilenTas.ToString());
                }
                else
                {
                    Debug.LogWarning(siradakiOyuncu.oyuncuAdi + " yerden null bir ta� �ekti (deste bitti ve sonras�nda �ekmeye �al��t�).");
                }
            }
            else
            {
                Debug.LogWarning(siradakiOyuncu.oyuncuAdi + " yerden ta� �ekemedi, destede ta� kalmam��!");
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
                    Debug.Log(siradakiOyuncu.oyuncuAdi + " �u ta�� att�: " + atilanTasNormalTur.ToString() + " (elinden index: " + atilacakIndexNormalTur + ")");
                    if (atilanTaslarYigini.Count > 0)
                        Debug.Log("Yandaki Son Ta�: " + atilanTaslarYigini[atilanTaslarYigini.Count - 1].ToString());
                }
                else
                {
                    Debug.LogError(siradakiOyuncu.oyuncuAdi + " null bir ta� atmaya �al��t� (Normal Tur)! At�lacak Index: " + atilacakIndexNormalTur);
                }
            }
            else
            {
                Debug.LogError(siradakiOyuncu.oyuncuAdi + " i�in at�lacak ge�erli bir ta� index'i bulunamad�! El Say�s�: " + siradakiOyuncu.eli.Count);
            }
        }
        else
        {
            Debug.LogWarning(siradakiOyuncu.oyuncuAdi + " elinde atacak ta�� kalmad�!");
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
            Debug.Log("---- B�R TAM TUR B�TT� (Her oyuncu birer el oynad�) ----");
            return;
        }
        SiradakiOyuncuyaGec();
    }

    void SiradakiOyuncuyaGec()
    {
        aktifOyuncuIndex = (aktifOyuncuIndex + 1) % oyuncuSayisi;
        Debug.Log(">>>> S�ra �imdi " + oyuncular[aktifOyuncuIndex].oyuncuAdi + " oyuncusunda. <<<<");

        if (atilanTaslarYigini.Count > 0)
        {
            Debug.Log(oyuncular[aktifOyuncuIndex].oyuncuAdi + ", yandaki son ta�� g�r�yor: " + atilanTaslarYigini[atilanTaslarYigini.Count - 1].ToString());
        }
        else
        {
            Debug.Log(oyuncular[aktifOyuncuIndex].oyuncuAdi + ", yanda hen�z at�lm�� ta� yok.");
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
            Debug.LogError("SiraKimdeyseOAtar: Ge�erli aktif oyuncu yok!");
            return false;
        }

        Oyuncu oynayanOyuncu = oyuncular[aktifOyuncuIndex];

        
        if (oynayanOyuncu.eli.Contains(atilanTas))
        {
            oynayanOyuncu.eli.Remove(atilanTas);
            atilanTaslarYigini.Add(atilanTas);

            Debug.Log(oynayanOyuncu.oyuncuAdi + " UI �zerinden s�r�kleyerek �u ta�� att�: " + atilanTas.ToString());

            if (atilanTaslarYigini.Count > 0)
                Debug.Log("Yandaki Son Ta�: " + atilanTaslarYigini[atilanTaslarYigini.Count - 1].ToString());

           
            if (aktifOyuncuIndex == 0 && uiManager != null) 
            {
                uiManager.Oyuncu1EliniGoster(oynayanOyuncu);
            }
           

           
            SonrakiOyuncununHamlesiniBaslat(); 
            return true; 
        }
        else
        {
            Debug.LogError(oynayanOyuncu.oyuncuAdi + " elinde olmayan bir ta�� (" + atilanTas.ToString() + ") atmaya �al��t�!");
            
            if (aktifOyuncuIndex == 0 && uiManager != null)
            {
                uiManager.Oyuncu1EliniGoster(oynayanOyuncu);
            }
            return false; 
        }
    }
} 