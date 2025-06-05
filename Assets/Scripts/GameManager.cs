using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("Ba�lant�lar")]
    public DesteManager desteManager;
    public UIManager uiManager;

    [Header("Oyun Ayarlar�")]
    public int oyuncuSayisi = 4;
    public List<Oyuncu> oyuncular = new List<Oyuncu>();

    [Header("Oyun Durumu")]
    public Tas gostergeTasi;
    public Tas okeyTasi; 

    private int aktifOyuncuIndex = 0;
    public List<Tas> atilanTaslarYigini = new List<Tas>();
    private int oynananElSayisiBuTurda = 0;

    
    public int AktifOyuncuIndex
    {
        get { return aktifOyuncuIndex; }
    }

    void Start()
    {
        
        if (desteManager == null)
        {
            Debug.LogError("GameManager: DesteManager atanmam��! L�tfen Inspector �zerinden atay�n.");
            enabled = false; 
            return;
        }
        if (uiManager == null)
        {
            Debug.LogError("GameManager: UIManager atanmam��! L�tfen Inspector �zerinden atay�n.");
            enabled = false;
            return;
        }

        OyunuBaslat();
    }

    public void OyunuBaslat()
    {
        Debug.Log("---------------- OYUN BA�LATILIYOR ----------------");

        oyuncular.Clear();
        atilanTaslarYigini.Clear();
        oynananElSayisiBuTurda = 0;
        aktifOyuncuIndex = 0;
        gostergeTasi = null;
        okeyTasi = null;

       
        desteManager.DesteOlusturVeKaristir();
        Debug.Log("Deste olu�turuldu ve kar��t�r�ld�. Toplam ta�: " + desteManager.KalanTasSayisi());

        
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
                    return;
                }
            }
        }

        Debug.Log("Ta� da��t�m� tamamland�.");
        foreach (Oyuncu oyuncu in oyuncular)
        {
            Debug.Log(oyuncu.oyuncuAdi + " elindeki ta� say�s�: " + oyuncu.eli.Count);
            
        }

        GostergeVeOkeyBelirle();
        if (this.okeyTasi == null) 
        {
            Debug.LogError("Okey ta�� belirlenemedi�i i�in oyun ba�lat�lam�yor.");
            return;
        }


        Debug.Log("Destede kalan ta� say�s� (G�sterge sonras�): " + desteManager.KalanTasSayisi());

        GuncelOyuncununEliniUIDaGoster();

        Debug.Log("Oyun kurulumu tamamland�. Oyuncu " + (oyuncular[aktifOyuncuIndex].oyuncuAdi) + "'in hamlesi bekleniyor...");
        
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
                Debug.LogWarning("Potansiyel g�sterge null geldi ama destede hala ta� var. Tekrar deneniyor.");
                continue;
            }

            if (potansiyelGosterge != null && potansiyelGosterge.tip == TasTipi.SahteOkey)
            {
                Debug.Log("G�sterge i�in Sahte Okey �ekildi (" + potansiyelGosterge.ToString() + "). Bu ta� ge�erli g�sterge de�il, yeni g�sterge �ekilecek.");
                potansiyelGosterge = null;
            }

            if (denemeSayaci > MAKSIMUM_GOSTERGE_DENEMESI)
            {
                Debug.LogError("G�STERGE BEL�RLENEMED�: Maksimum deneme say�s�na ula��ld�. Destede uygun g�sterge bulunamad�.");
                this.gostergeTasi = null;
                this.okeyTasi = null;
                return;
            }
        } while (potansiyelGosterge == null || potansiyelGosterge.tip == TasTipi.SahteOkey);

        this.gostergeTasi = potansiyelGosterge;

        if (this.gostergeTasi == null)
        {
            Debug.LogError("G�STERGE TA�I BEL�RLENEMED� (SON KONTROL)!");
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

    
    public bool SiraKimdeyseOAtar(Tas atilanTas)
    {
        if (oyuncular.Count == 0 || aktifOyuncuIndex >= oyuncular.Count)
        {
            Debug.LogError("SiraKimdeyseOAtar: Ge�erli aktif oyuncu yok!");
            return false;
        }

        Oyuncu oynayanOyuncu = oyuncular[aktifOyuncuIndex];

        if (atilanTas == null)
        {
            Debug.LogError(oynayanOyuncu.oyuncuAdi + " null bir ta� atmaya �al��t�!");
            GuncelOyuncununEliniUIDaGoster();
            return false;
        }

        
        if (oynayanOyuncu.eli.Contains(atilanTas))
        {
            

            oynayanOyuncu.eli.Remove(atilanTas);
            atilanTaslarYigini.Add(atilanTas);

            Debug.Log(oynayanOyuncu.oyuncuAdi + " UI �zerinden s�r�kleyerek �u ta�� att�: " + atilanTas.ToString());

            if (atilanTaslarYigini.Count > 0)
                Debug.Log("Yandaki Son Ta� (At�lan): " + atilanTaslarYigini[atilanTaslarYigini.Count - 1].ToString());

            GuncelOyuncununEliniUIDaGoster();

            

            SonrakiOyuncununHamlesiniBaslat();
            return true;
        }
        else
        {
            Debug.LogError(oynayanOyuncu.oyuncuAdi + " elinde olmayan bir ta�� (" + atilanTas.ToString() + ") atmaya �al��t�! El kontrol ediliyor.");
            
            GuncelOyuncununEliniUIDaGoster();
            return false;
        }
    }

    void SonrakiOyuncununHamlesiniBaslat()
    {
        oynananElSayisiBuTurda++;
        
        
        if (oynananElSayisiBuTurda >= oyuncuSayisi && aktifOyuncuIndex == oyuncuSayisi - 1) 
        {
            Debug.Log("---- B�R TAM TUR B�TT� (Herkes birer el oynad�) ----");
            oynananElSayisiBuTurda = 0; 
            
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

        
        if (aktifOyuncuIndex == 0)
        {
            Debug.Log(oyuncular[aktifOyuncuIndex].oyuncuAdi + " hamlesini UI �zerinden yapacak.");
            GuncelOyuncununEliniUIDaGoster();
        }
        else 
        {
            NormalOyuncuTurunuOyna(); 
        }
    }

    
    void NormalOyuncuTurunuOyna()
    {
        if (oyuncular.Count == 0 || aktifOyuncuIndex >= oyuncular.Count) return;

        Oyuncu siradakiOyuncu = oyuncular[aktifOyuncuIndex];
        Debug.Log("---- " + siradakiOyuncu.oyuncuAdi + " (Otomatik BOT Hamlesi) Oynuyor ----");

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
                Debug.Log(siradakiOyuncu.oyuncuAdi + " (BOT) yandan �u ta�� ald�: " + sonYereAtilanTas.ToString());
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
                    Debug.Log(siradakiOyuncu.oyuncuAdi + " (BOT) yerden �u ta�� �ekti: " + cekilenTas.ToString());
                }
                else
                { 
                    Debug.LogWarning(siradakiOyuncu.oyuncuAdi + " (BOT) yerden null bir ta� �ekti (deste bitti).");
                }
            }
            else
            {
                Debug.LogWarning(siradakiOyuncu.oyuncuAdi + " (BOT) yerden ta� �ekemedi, destede ta� kalmam��!");
                
            }
        }

        

       
        if (siradakiOyuncu.eli.Count > 0)
        {
            int atilacakIndex = 0; 
            
            if (siradakiOyuncu.eli.Count > 1)
            {
                Tas sonTas = siradakiOyuncu.eli[siradakiOyuncu.eli.Count - 1]; 
                bool sonTasDegerli = (this.okeyTasi != null && sonTas == this.okeyTasi) || sonTas.tip == TasTipi.SahteOkey;

                if (sonTasDegerli)
                { 
                    bool degersizBulundu = false;
                    for (int k = 0; k < siradakiOyuncu.eli.Count; k++)
                    {
                        Tas potansiyel = siradakiOyuncu.eli[k];
                        if (!((this.okeyTasi != null && potansiyel == this.okeyTasi) || potansiyel.tip == TasTipi.SahteOkey))
                        {
                            atilacakIndex = k;
                            degersizBulundu = true;
                            break;
                        }
                    }
                    if (!degersizBulundu) atilacakIndex = siradakiOyuncu.eli.Count - 1; 
                }
                else
                {
                    atilacakIndex = siradakiOyuncu.eli.Count - 1; 
                }
            }

            Tas atilanTas = siradakiOyuncu.eli[atilacakIndex];
            if (atilanTas != null)
            {
                siradakiOyuncu.eli.RemoveAt(atilacakIndex);
                atilanTaslarYigini.Add(atilanTas);
                Debug.Log(siradakiOyuncu.oyuncuAdi + " (BOT) �u ta�� att�: " + atilanTas.ToString());
                if (atilanTaslarYigini.Count > 0)
                    Debug.Log("Yandaki Son Ta�: " + atilanTaslarYigini[atilanTaslarYigini.Count - 1].ToString());
            }
            else
            {
                Debug.LogError(siradakiOyuncu.oyuncuAdi + " (BOT) null ta� atmaya �al��t�! Index: " + atilacakIndex);
                siradakiOyuncu.eli.RemoveAt(atilacakIndex);
            }
        }
        else
        {
            Debug.LogWarning(siradakiOyuncu.oyuncuAdi + " (BOT) elinde atacak ta�� kalmad�!");
        }

        
        SonrakiOyuncununHamlesiniBaslat();
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

    
    void GuncelOyuncununEliniUIDaGoster()
    {
        if (uiManager != null && oyuncular.Count > aktifOyuncuIndex)
        {
            
            if (aktifOyuncuIndex == 0)
            {
                uiManager.Oyuncu1EliniGoster(oyuncular[aktifOyuncuIndex]);
            }
            
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
}