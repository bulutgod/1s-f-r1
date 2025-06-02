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
        if (oyuncular.Count == 0) return;
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
    }
}
