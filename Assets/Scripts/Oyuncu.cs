using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Oyuncu
{
    public string oyuncuAdi;
    public List<Tas> eli = new List<Tas>();
    public bool ilkOyuncuMu = false;

    public Oyuncu(string ad, bool ilkMi = false)
    {
        oyuncuAdi = ad;
        eli = new List<Tas>();
        ilkOyuncuMu = ilkMi;
    }

    public void EliniTemizle()
    {
        eli.Clear();
    }

    public void TasaEkle(Tas tas)
    {
        if (tas != null)
        {
            eli.Add(tas);
        }
    }

    public void EliniGoster()
    {
        Debug.Log("---- " + oyuncuAdi + " Eli (" + eli.Count + " taþ) ----");
        if (eli.Count == 0)
        {
            Debug.Log("(Boþ el)");
        }
        else
        {
            foreach (Tas tasInEli in eli) // 'tas' zaten Tas.cs'de kullanýlýyor, karýþýklýk olmasýn diye 'tasInEli' yaptým.
            {
                if (tasInEli != null)
                {
                    Debug.Log(tasInEli.ToString());
                }
                else
                {
                    Debug.Log("(Null Taþ Referansý!)");
                }
            }
        }
        Debug.Log("--------------------------");
    }
}