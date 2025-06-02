using UnityEngine;
using System.Collections.Generic; 
using System.Linq; 

public class DesteManager : MonoBehaviour
{
    [Header("Veri Kaynaðý")]
    public TasVeritabani tasVeritabani; 

    private List<Tas> aktifDeste = new List<Tas>(); 

    void Start()
    {
        if (tasVeritabani == null)
        {
            Debug.LogError("DesteManager: TasVeritabani atanmamýþ!");
            return;
        }
        DesteOlusturVeKaristir();
        TestDeste();
    }

    public void DesteOlusturVeKaristir()
    {
        aktifDeste.Clear(); 

        foreach (Tas tasSO in tasVeritabani.tumTaslar)
        {
            aktifDeste.Add(tasSO);
        }

        
        System.Random rng = new System.Random();
        int n = aktifDeste.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Tas deger = aktifDeste[k];
            aktifDeste[k] = aktifDeste[n];
            aktifDeste[n] = deger;
        }

        Debug.Log("Deste oluþturuldu ve karýþtýrýldý. Toplam taþ: " + aktifDeste.Count);
    }

    public Tas TasCek()
    {
        if (aktifDeste.Count == 0)
        {
            Debug.LogWarning("Destede çekilecek taþ kalmadý!");
            return null; 
        }

        
        Tas cekilenTas = aktifDeste[aktifDeste.Count - 1];
        aktifDeste.RemoveAt(aktifDeste.Count - 1); 

        return cekilenTas;
    }

    void TestDeste()
    {
        if (aktifDeste.Count > 0)
        {
            Debug.Log("Destenin en üstündeki ilk 5 taþ (karýþtýrýldýktan sonra):");
            for (int i = 0; i < Mathf.Min(5, aktifDeste.Count); i++)
            {
                
                Debug.Log((i + 1) + ". Çekilecek Taþ: " + aktifDeste[aktifDeste.Count - 1 - i].ToString());
            }
        }
    }
}
