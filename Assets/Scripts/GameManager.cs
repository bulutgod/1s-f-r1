using UnityEngine;
using System.Collections.Generic;
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
            Debug.Log("Destede kalan tas sayisi: " + desteManager.kalanTasSayisi());
        }
        oynananElSayisiBuTurda = 0;
        aktifOyuncuIndex = 0;
        if(oyuncular.Count > 0 && oyuncular[aktifOyuncuIndex].ilkOyuncuMu)
        {
            IlkOyuncuTurunuOyna();
        }
        else if (oyuncular.Count > 0 )
        {
            NormalOyuncuTurunuOyna();
        }
    }
    void GostergeVeOkeyBelirle()
    {
        if (desteManager.kalanTasSayisi() == 0)
        {
            Debug.Log("Gosterge cekmek icin destede tas kalmadi!");
            return;
        }
        gostergeTasi = desteManager.TasCek();
        if (gostergeTasi == null)
        {
            Debug.Log("Gosterge tasi cekilemedi null geldi");
            return;
        }

        Debug.Log("Gosterge tasi: " + gostergeTasi.ToString());
        if (gostergeTasi.tip == TasTipi.SahteOkey)
        {
            okeyTasi = gostergeTasi;
        }
        else
        {
            TasRengi okeyRengi = gostergeTasi.renk;
            int okeyNumarasi;
            if (gostergeTasi.sayi == 13)
            {
                okeyNumarasi = 1;
            }
            else
            {
                okeyNumarasi = gostergeTasi.sayi + 1;
            }
            if (desteManager.tasVeritabani != null)
            {
                bool okeyBulundu = false;
                foreach (Tas potansiyelOkey in desteManager.tasVeritabani.tumTaslar)
                {
                    if (potansiyelOkey.tip == TasTipi.Sayi && potansiyelOkey.renk == okeyRengi && potansiyelOkey.sayi == okeyNumarasi)
                    {
                        okeyTasi = potansiyelOkey;
                        okeyBulundu = true;
                        break;
                    }
                }
                if (okeyBulundu)
                {
                    Debug.Log("Okey tasi: " + okeyTasi.ToString());
                }
                else
                {
                    Debug.LogError("Belirlenen okey tasi (" + okeyRengi + " " + okeyNumarasi + ") TasVeritabani'nda bulunamadi!");
                }
            }
            else
            {
                Debug.LogError("Okey tasini bulmak icin TasVeritabaný referansý eksik");
            }
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

        if(atilanTaslarYigini.Count > 0)
        {
            sonYereAtilanTas = atilanTaslarYigini[atilanTaslarYigini.Count-1];
            if(OyuncuYandanAlmakÝsterMi(siradakiOyuncu,sonYereAtilanTas))
            {
                siradakiOyuncu.TasaEkle(sonYereAtilanTas);
                atilanTaslarYigini.RemoveAt(atilanTaslarYigini.Count -1);
                yandanTasAldi = true;
                Debug.Log(siradakiOyuncu.oyuncuAdi + "yandan su tasi aldi: " + sonYereAtilanTas.ToString());
            }
        }

        if(!yandanTasAldi)
        {
            if(desteManager.kalanTasSayisi() > 0)
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
            if(siradakiOyuncu.eli.Count == 1)
            {
                atilacakIndex = 0;
            }
            else
            {
                Tas sonTas = siradakiOyuncu.eli[siradakiOyuncu.eli.Count - 1];
                bool sonTasDegerliMi = (this.okeyTasi != null && sonTas == this.okeyTasi) || (sonTas.tip == TasTipi.SahteOkey); 
                if (sonTasDegerliMi )
                {
                    bool degersizTasBulundu = false;
                    for (int k = 0; k < siradakiOyuncu.eli.Count -1; k++)
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
                    if(!degersizTasBulundu)
                    {
                        atilacakIndex = siradakiOyuncu.eli.Count - 1;
                    }
                }
                else
                {
                    atilacakIndex = siradakiOyuncu.eli.Count - 1;
                }


            }
           if(atilacakIndex != -1 && atilacakIndex < siradakiOyuncu.eli.Count)
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

        if(atilanTaslarYigini.Count > 0)
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
