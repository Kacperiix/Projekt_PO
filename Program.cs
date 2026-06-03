using System;
using System.Collections.Generic;
using System.IO;

namespace Projekt_PO
{
    public enum StatusZamowienia
    {
        Nowe,
        WPrzygotowaniu,
        Gotowe,
        Zrealizowane
    }
    public interface IStrategiaWyceny
    {
        double ObliczCene(List<PozycjaMenu> pozycje);
    }
    public class CennikStandardowy : IStrategiaWyceny
{
    public double ObliczCene(List<PozycjaMenu> pozycje)
    {
        double suma = 0;
        foreach (var pozycja in pozycje)
        {
            suma += pozycja.CenaBazowa;
        }
        return suma;
    }
}
public class CennikZestawow : IStrategiaWyceny
{
    public double ObliczCene(List<PozycjaMenu> pozycje)
    {
        double suma = 0;
        bool maDanie = false;
        bool maNapoj = false;

        foreach (var pozycja in pozycje)
        {
            suma += pozycja.CenaBazowa;
            
            if (pozycja is Danie)
            {
                maDanie = true;
            }
            else if (pozycja is Napoj)
            {
                maNapoj = true;
            }
        }
        if (maDanie && maNapoj)
        {
            return suma * 0.8;
        }
        return suma;
    }
}
    public abstract class PozycjaMenu
    {
        private string nazwa;
        private double cenaBazowa;

        public string Nazwa
        {
            get {return nazwa; }
        }

        public double CenaBazowa
        {
            get { return cenaBazowa; }
        }

        public PozycjaMenu(string nazwa, double cena)
        {
            this.nazwa = nazwa;
            this.cenaBazowa = cena;
        }

        public abstract void Przygotuj();
    }

    public class Danie : PozycjaMenu
    {
        public bool CzyPikantne { get; set; }
        public Danie(string nazwa, double cena, bool czyPikantne) : base(nazwa, cena)
        {
            CzyPikantne = czyPikantne;
        }

        public override void Przygotuj()
        {
            Console.WriteLine($"Przygotowuję danie w kuchni: {Nazwa}. Czy pikantne: {CzyPikantne}");
        }
    }

    public class Napoj : PozycjaMenu
    {
        public int PojemnoscWMl { get; set; }
        public Napoj(string nazwa, double cena, int pojemnosc) : base(nazwa, cena)
        {
            PojemnoscWMl = pojemnosc;
        }
        public override void Przygotuj()
        {
            Console.WriteLine($"Nalewam napój na barze: {Nazwa}. Pojemność: {PojemnoscWMl} ml");
        }
    }
    public class WyjatekZamowienia : Exception
    {
        public WyjatekZamowienia(string message) : base(message)
        {
        }
    }

    public class WyjatekPustegoZamowienia : WyjatekZamowienia
    {
        public WyjatekPustegoZamowienia(string message) : base(message)
        {
        }
    }

    public class Zamowienie
    {
        private List<PozycjaMenu> pozycje;
        private StatusZamowienia status;
        private IStrategiaWyceny strategiaWyceny;

        public Zamowienie(IStrategiaWyceny strategia)
        {
            pozycje = new List<PozycjaMenu>();
            status = StatusZamowienia.Nowe;
            strategiaWyceny = strategia;
        }

        public void DodajPozycje(PozycjaMenu pozycja)
        {
            throw new NotImplementedException();
        }

        public double ObliczSume()
        {
            throw new NotImplementedException();
        }

        public void ZmienStatus(StatusZamowienia nowyStatus)
        {
            throw new NotImplementedException();
        }

        public static Zamowienie operator +(Zamowienie zamowienie, PozycjaMenu pozycja)
        {
            throw new NotImplementedException();
        }
    }
    public class ZarzadcaRestauracji
    {
        private List<PozycjaMenu> dostepneMenu;
        private List<Zamowienie> historiaZamowien;

        public List<PozycjaMenu> ListaDostepnegoMenu
        {
            get { return dostepneMenu; }
        }        

        public ZarzadcaRestauracji()
        {
            dostepneMenu = new List<PozycjaMenu>();
            historiaZamowien = new List<Zamowienie>();
        }

        public void DodajDoMenu(PozycjaMenu pozycja)
        {
            dostepneMenu.Add(pozycja);
            Console.WriteLine($"Pomyślnie dodano '{pozycja.Nazwa}' do menu restauracji.");
        }

        public void ModyfikujPozycje(string staraNazwa, PozycjaMenu nowaPozycja)
        {
            PozycjaMenu znalezionaPozycja = WyszukajWMenu(staraNazwa);
            if (znalezionaPozycja != null)
            {
                int indeks = dostepneMenu.IndexOf(znalezionaPozycja);
                dostepneMenu[indeks] = nowaPozycja;
                Console.WriteLine($"Pomyślnie zaktualizowano pozycję '{staraNazwa}' na '{nowaPozycja.Nazwa}'.");
            }
            else
            {
                Console.WriteLine($"Błąd: Nie znaleziono pozycji o nazwie '{staraNazwa}' w menu.");
            }
        }

        public PozycjaMenu WyszukajWMenu(string nazwa)
        {
            foreach(PozycjaMenu pozycja in dostepneMenu)
            {
                if (pozycja.Nazwa.ToLower() == nazwa.ToLower())
                {
                    return pozycja;
                }
            }
            return null;
        }

        public void ZlozZamowienie(Zamowienie zamowienie)
        {
            historiaZamowien.Add(zamowienie);
            Console.WriteLine($"Pomyślnie złożono i zapisano zamówienie. Całkowita liczba zamówień w sysstemie: {historiaZamowien.Count}");
        }

        public void ZapiszStanDoPliku(string sciezka)
        {
            using (StreamWriter writer = new StreamWriter(sciezka))
            {
                foreach(PozycjaMenu pozycja in dostepneMenu)
                {
                    if (pozycja is Danie danie)
                    {
                        writer.WriteLine($"Danie|{danie.Nazwa}|{danie.CenaBazowa}|{danie.CzyPikantne}");
                    }
                    else if (pozycja is Napoj napoj)
                    {
                        writer.WriteLine($"Napoj|{napoj.Nazwa}|{napoj.CenaBazowa}|{napoj.PojemnoscWMl}");
                    }
                }
            }
            Console.WriteLine("Pomyślnie zapisano aktualne menu do pliku.");
        }

        public void WczytajStanZPliku(string sciezka)
        {
            if (!File.Exists(sciezka))
            {
                Console.WriteLine("Plik zapisu nie istnieje.");
                return;
            }

            dostepneMenu.Clear();

            using (StreamReader reader = new StreamReader(sciezka))
            {
                string linia;
                
                while ((linia = reader.ReadLine()) != null)
                {
                    string[] czesci = linia.Split('|');
                    if (czesci[0] == "Danie")
                    {
                        string nazwa = czesci[1];
                        double cena = double.Parse(czesci[2]);
                        bool czyPikantne = bool.Parse(czesci[3]);

                        Danie wczytaneDanie = new Danie(nazwa, cena, czyPikantne);
                        dostepneMenu.Add(wczytaneDanie);
                    }
                    else if (czesci[0] == "Napoj")
                    {
                        string nazwa = czesci[1];
                        double cena = double.Parse(czesci[2]);
                        int pojemnosc = int.Parse(czesci[3]);

                        Napoj wczytanyNapoj = new Napoj(nazwa, cena, pojemnosc);
                        dostepneMenu.Add(wczytanyNapoj);
                    }
                }
            }
                Console.WriteLine("Pomyślnie wczytano stan restauracji z pliku.");
        }
    }
    
    public class Program
    {
        public static void Main(string[] args)
        {
            ZarzadcaRestauracji zarzadca = new ZarzadcaRestauracji();
            bool dziala = true;
            while(dziala)
            {
                Console.WriteLine("\n========================================");
                Console.WriteLine("=== SYSTEM ZARZĄDZANIA RESTAURACJĄ ===");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Pokaż całe dostępne menu");
                Console.WriteLine("2. Dodaj nową pozycję do menu");
                Console.WriteLine("3. Wyszukaj pozycję w menu");
                Console.WriteLine("4. Modyfikuj istniejącą pozycję w menu");
                Console.WriteLine("5. Złóż nowe zamówienie (Koszyk)");
                Console.WriteLine("6. Zapisz stan restauracji do pliku");
                Console.WriteLine("7. Wczytaj stan restauracji z pliku");
                Console.WriteLine("0. Zakończ program");
                Console.WriteLine("========================================");
                Console.Write("Wybierz opcję (0-7): ");         

                string wybor = Console.ReadLine();
                Console.WriteLine();

                switch(wybor)
                {
                    case "1":
                        Console.WriteLine("--- AKTUALNE MENU RESTAURACJI ---");
                        if(zarzadca.ListaDostepnegoMenu.Count == 0)
                        {
                            Console.WriteLine("Menu jest obecnie puste.");
                        }
                        else
                        {
                            foreach (PozycjaMenu pozycja in zarzadca.ListaDostepnegoMenu)
                            {
                                Console.WriteLine($"- {pozycja.Nazwa} | Cena: {pozycja.CenaBazowa} zł");
                            }
                        }
                        break;
                    case "2":
                        Console.WriteLine("");
                        break;
                    case "3":
                        Console.WriteLine("");
                        break;
                    case "4":
                        Console.WriteLine("");
                        break;
                    case "5":
                        Console.WriteLine("");
                        break;
                    case "6":
                        Console.WriteLine("");
                        break;
                    case "7":
                        Console.WriteLine("");
                        break;
                    case "0":
                        dziala = false;
                        Console.WriteLine("Dziękujemy za skorzystanie z systemu. Zamykanie...");
                        break;
                    default:
                        Console.WriteLine("Błąd: Niepoprawny wybór! Wpisz cyfrę od 0 do 7.");
                        break;                   
                }       
            }
        }
    }
}
