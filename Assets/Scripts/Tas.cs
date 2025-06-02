using UnityEngine;

public enum TasRengi
{
    Kirmizi,
    Sari,
    Siyah,
    Mavi,
}
public enum TasTipi
{
    Sayi,
    SahteOkey,
}
// Unity edit�r�nde Assets > Create men�s�nden yeni Tas nesneleri olu�turabilmek i�in:
[CreateAssetMenu(fileName = "Yeni Tas", menuName = "101 Okey/Tas Tanim")]

public class Tas : ScriptableObject
{
    [Header("Tas Bilgileri")]
    public TasRengi renk;
    public int sayi;
    public TasTipi tip = TasTipi.Sayi;
    [Header("Gorsel")]
    public Sprite tasGorseli;

    public override string ToString()
    {
        if (tip == TasTipi.SahteOkey)
        {
            return "Sahte Okey";
        }
        return renk.ToString() + " " + sayi;
    }
}
