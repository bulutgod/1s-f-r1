using System.Collections.Generic;
using System.Linq;

public static class OkeyRulesEngine
{
    public static bool GecerliPerMi(List<Tas> potansiyelPer, Tas okeyTasiGercek)
    {
        if (potansiyelPer == null || potansiyelPer.Count < 3 || potansiyelPer.Count > 4)
            return false;
        List<Tas> sayisalTaslar = new List<Tas>();
        int wildcardSayisi = 0;

        if (okeyTasiGercek == null)
            return false;

        foreach (Tas tas in potansiyelPer)
        {
            if (tas == null) return false;
            if (tas.tip == TasTipi.SahteOkey)
                sayisalTaslar.Add(okeyTasiGercek);
            else if (tas == okeyTasiGercek)
                wildcardSayisi++;
            else if (tas.tip == TasTipi.Sayi)
                sayisalTaslar.Add(tas);
            else
                return false;
        }
        if (sayisalTaslar.Count == 0)
            return wildcardSayisi >= 3 && wildcardSayisi <= 4 && sayisalTaslar.Count == 0;

        int referansSayi = sayisalTaslar[0].sayi;
        foreach (Tas tas in sayisalTaslar)
            if (tas.sayi != referansSayi)
                return false;

        List<TasRengi> kullanilanRenkler = new List<TasRengi>();
        foreach (Tas tas in sayisalTaslar)
        {
            if (kullanilanRenkler.Contains(tas.renk))
                return false;
            kullanilanRenkler.Add(tas.renk);
        }
        return true;
    }

    public static bool GecerliSeriMi(List<Tas> potansiyelSeri, Tas okeyTasiGercek)
    {
        if (potansiyelSeri == null || potansiyelSeri.Count < 3)
            return false;
        if (okeyTasiGercek == null || okeyTasiGercek.tip != TasTipi.Sayi)
        {
            List<Tas> siraliSafTaslar = new List<Tas>(potansiyelSeri);
            foreach (Tas tst in siraliSafTaslar)
                if (tst == null || tst.tip != TasTipi.Sayi)
                    return false;
            if (siraliSafTaslar.Count == 0 && potansiyelSeri.Count >= 3) return false;
            if (siraliSafTaslar.Count == 0 && potansiyelSeri.Count < 3) return false;
            if (siraliSafTaslar.Count > 0)
            {
                TasRengi r = siraliSafTaslar[0].renk;
                if (!siraliSafTaslar.All(t => t.renk == r))
                    return false;
                siraliSafTaslar.Sort((a, b) => a.sayi.CompareTo(b.sayi));
                for (int i = 0; i < siraliSafTaslar.Count - 1; i++)
                    if (siraliSafTaslar[i + 1].sayi != siraliSafTaslar[i].sayi + 1)
                        return false;
                return true;
            }
            return false;
        }

        List<Tas> islenecekTaslar = new List<Tas>();
        int wildcardAdedi = 0;

        foreach (Tas tas in potansiyelSeri)
        {
            if (tas == null) return false;
            if (tas.tip == TasTipi.SahteOkey)
                islenecekTaslar.Add(okeyTasiGercek);
            else if (tas == okeyTasiGercek)
                wildcardAdedi++;
            else if (tas.tip == TasTipi.Sayi)
                islenecekTaslar.Add(tas);
            else
                return false;
        }
        if (islenecekTaslar.Count == 0)
            return wildcardAdedi >= 3 && wildcardAdedi == potansiyelSeri.Count;

        TasRengi seriRengi = islenecekTaslar[0].renk;
        foreach (Tas tas in islenecekTaslar)
            if (tas.renk != seriRengi)
                return false;

        islenecekTaslar.Sort((a, b) => a.sayi.CompareTo(b.sayi));
        List<int> benzersizSayilar = new List<int>();
        if (islenecekTaslar.Count > 0)
        {
            benzersizSayilar.Add(islenecekTaslar[0].sayi);
            for (int i = 1; i < islenecekTaslar.Count; i++)
            {
                if (islenecekTaslar[i].sayi > islenecekTaslar[i - 1].sayi)
                    benzersizSayilar.Add(islenecekTaslar[i].sayi);
                else if (islenecekTaslar[i].sayi == islenecekTaslar[i - 1].sayi)
                    return false;
            }
        }
        if (benzersizSayilar.Count == 0)
            return wildcardAdedi >= 3 && wildcardAdedi == potansiyelSeri.Count;

        int minSayi = benzersizSayilar[0];
        int maxSayi = benzersizSayilar[benzersizSayilar.Count - 1];
        int teorikSeriUzunluguBoylesiSayilarla = maxSayi - minSayi + 1;
        int gerekenWildcardSayisi = teorikSeriUzunluguBoylesiSayilarla - benzersizSayilar.Count;
        if (gerekenWildcardSayisi < 0) { gerekenWildcardSayisi = 0; }
        if (wildcardAdedi >= gerekenWildcardSayisi)
        {
            int olusanTamSeriUzunlugu = benzersizSayilar.Count + gerekenWildcardSayisi;
            if (olusanTamSeriUzunlugu == potansiyelSeri.Count)
                return true;
            else
                return false;
        }
        else
        {
            return false;
        }
    }

    public static int ElinPuaniniHesapla(List<Tas> oyuncuEli, Tas okeyTasiGercek)
    {
        if (oyuncuEli == null || oyuncuEli.Count == 0)
            return 0;
        if (okeyTasiGercek == null)
            return 0;

        List<Tas> kalanTaslar = new List<Tas>(oyuncuEli);
        int toplamPuan = 0;

        List<Tas> mevcutTaslar = new List<Tas>(oyuncuEli);
        bool degisiklikYapildi = true;

        while (degisiklikYapildi)
        {
            degisiklikYapildi = false;
            for (int i = 0; i < mevcutTaslar.Count; i++)
            {
                for (int j = i + 1; j < mevcutTaslar.Count; j++)
                {
                    for (int k = j + 1; k < mevcutTaslar.Count; k++)
                    {
                        for (int l = k + 1; l < mevcutTaslar.Count; l++)
                        {
                            List<Tas> potansiyelGrup = new List<Tas> { mevcutTaslar[i], mevcutTaslar[j], mevcutTaslar[k], mevcutTaslar[l] };
                            if (GecerliPerMi(potansiyelGrup, okeyTasiGercek) || GecerliSeriMi(potansiyelGrup, okeyTasiGercek))
                            {
                                foreach (Tas tas in potansiyelGrup)
                                {
                                    toplamPuan += tas.sayi;
                                    if (tas.tip == TasTipi.SahteOkey || tas == okeyTasiGercek)
                                    {
                                        toplamPuan += okeyTasiGercek.sayi;
                                    }
                                }
                                mevcutTaslar.Remove(potansiyelGrup[0]);
                                mevcutTaslar.Remove(potansiyelGrup[1]);
                                mevcutTaslar.Remove(potansiyelGrup[2]);
                                mevcutTaslar.Remove(potansiyelGrup[3]);
                                degisiklikYapildi = true;
                                goto NextIteration;
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < mevcutTaslar.Count; i++)
            {
                for (int j = i + 1; j < mevcutTaslar.Count; j++)
                {
                    for (int k = j + 1; k < mevcutTaslar.Count; k++)
                    {
                        List<Tas> potansiyelGrup = new List<Tas> { mevcutTaslar[i], mevcutTaslar[j], mevcutTaslar[k] };
                        if (GecerliPerMi(potansiyelGrup, okeyTasiGercek) || GecerliSeriMi(potansiyelGrup, okeyTasiGercek))
                        {
                            foreach (Tas tas in potansiyelGrup)
                            {
                                toplamPuan += tas.sayi;
                                if (tas.tip == TasTipi.SahteOkey || tas == okeyTasiGercek)
                                {
                                    toplamPuan += okeyTasiGercek.sayi;
                                }
                            }
                            mevcutTaslar.Remove(potansiyelGrup[0]);
                            mevcutTaslar.Remove(potansiyelGrup[1]);
                            mevcutTaslar.Remove(potansiyelGrup[2]);
                            degisiklikYapildi = true;
                            goto NextIteration;
                        }
                    }
                }
            }
        NextIteration:;
        }
        return toplamPuan;
    }
}