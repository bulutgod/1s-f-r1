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

    void Start()
    {
        if (desteManager == null)
        {
            Debug.LogError("GameManager: DesteManager atanmamis");
            return;
        }
        // Multiplayer'da oyun ba�latma kullan�c�dan tetiklenmeli.
    }

    // Oyun ba�latma, ta� da��t�m�, hamle y�netimi gibi gerekli fonksiyonlar burada olmal�.
    // Test, otomatik demo, sabit ve �rnek test bloklar� kald�r�ld�.
    // Multiplayer�da oyuncu hamleleri UI veya network mesajlar�yla tetiklenmelidir.

    // Oda/oyun sonu, �ip i�lemleri, ceza gibi ek mekanikler buraya eklenmeli.
    public void OyuncuCiftAcmakIstedi(Oyuncu oyuncu)
    {
        if (oyuncu.CiftAcilabilirMi())
        {
            oyuncu.ciftAcilmisMi = true;
            // UI�da bilgilendirme: ��ift a�t�n, puan 2x olacak�
        }
        else
        {
            // UI�da hata: �5 �iftin yok!�
        }
    }
}