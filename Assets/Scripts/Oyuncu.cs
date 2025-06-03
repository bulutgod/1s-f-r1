using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Oyuncu
{
    public string oyuncuAdi;
    public List<Tas> eli = new List<Tas>();
    public bool ilkOyuncuMu = false;
    public int cipMiktari = 0;
    public bool ciftAcilmisMi = false;

    public Oyuncu(string ad, bool ilkMi = false, int baslangicCip = 0)
    {
        oyuncuAdi = ad;
        eli = new List<Tas>();
        ilkOyuncuMu = ilkMi;
        cipMiktari = baslangicCip;
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
    public bool CiftAcilabilirMi()
    {
        var el = this.eli;
        var ciftler = el.GroupBy(t => t.sayi)
                        .Where(g => g.Count() == 2)
                        .Count();
        return ciftler == 5;
    }
}