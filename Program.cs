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
            string czyPikantneTekst = CzyPikantne ? "tak" : "nie";
            Console.WriteLine($"Przygotowuję danie w kuchni: {Nazwa}. Czy pikantne: {czyPikantneTekst}");
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

        public List<PozycjaMenu> Pozycje
        {
            get { return pozycje; }
        }

        public Zamowienie(IStrategiaWyceny strategia)
        {
            pozycje = new List<PozycjaMenu>();
            status = StatusZamowienia.Nowe;
            strategiaWyceny = strategia;
        }

        public void DodajPozycje(PozycjaMenu pozycja)
        {
            pozycje.Add(pozycja);
            Console.WriteLine($"Dodano '{pozycja.Nazwa}' do zamówienia.");
        }

        public double ObliczSume()
        {
            if (pozycje.Count == 0)
            {
                throw new WyjatekPustegoZamowienia("Nie można obliczyć sumy. Zamówienie jest puste!");
            }
            return strategiaWyceny.ObliczCene(pozycje);
        }

        public void ZmienStatus(StatusZamowienia nowyStatus)
        {
            status = nowyStatus;
            Console.WriteLine($"Zmieniono status zamówienia na: {status}");
        }

        public static Zamowienie operator +(Zamowienie zamowienie, PozycjaMenu pozycja)
        {
            zamowienie.DodajPozycje(pozycja);
            return zamowienie;
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
            Console.WriteLine($"Pomyślnie złożono i zapisano zamówienie. Całkowita liczba zamówień w systemie: {historiaZamowien.Count}");
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
            string domyslnyPlik = "menu.txt";

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
                                string typ = pozycja is Danie ? "Danie" : "Napój";
                                Console.WriteLine($"- [{typ}] {pozycja.Nazwa} | Cena: {Math.Round(pozycja.CenaBazowa, 2)} zł");
                            }
                        }
                        break;

                    case "2":
                        Console.WriteLine("--- DODAWANIE DO MENU ---");
                        Console.WriteLine("Wybierz typ: 1 - Danie, 2 - Napój");
                        string typWyb = Console.ReadLine();
                        Console.Write("Podaj nazwę: ");
                        string nazwa = Console.ReadLine();
                        Console.Write("Podaj cenę bazową: ");
                        if (!double.TryParse(Console.ReadLine(), out double cena)) cena = 0;

                        if (typWyb == "1")
                        {
                            Console.Write("Czy danie jest pikantne? (tak/nie): ");
                            bool pikantne = Console.ReadLine().ToLower() == "tak";
                            zarzadca.DodajDoMenu(new Danie(nazwa, cena, pikantne));
                        }
                        else if (typWyb == "2")
                        {
                            Console.Write("Podaj pojemność (w ml): ");
                            if (!int.TryParse(Console.ReadLine(), out int poj)) poj = 0;
                            zarzadca.DodajDoMenu(new Napoj(nazwa, cena, poj));
                        }
                        else
                        {
                            Console.WriteLine("Nieznany typ pozycji.");
                        }
                        break;

                    case "3":
                        Console.WriteLine("--- WYSZUKIWANIE W MENU ---");
                        Console.Write("Podaj nazwę szukanej pozycji: ");
                        string szukanaNazwa = Console.ReadLine();
                        PozycjaMenu znaleziona = zarzadca.WyszukajWMenu(szukanaNazwa);
                        if (znaleziona != null)
                        {
                            Console.WriteLine($"Znaleziono: {znaleziona.Nazwa} za {Math.Round(znaleziona.CenaBazowa, 2)} zł.");
                        }
                        else
                        {
                            Console.WriteLine("Nie znaleziono takiej pozycji w menu.");
                        }
                        break;

                    case "4":
                        Console.WriteLine("--- MODYFIKOWANIE POZYCJI ---");
                        Console.Write("Podaj nazwę pozycji, którą chcesz zmodyfikować: ");
                        string staraNazwa = Console.ReadLine();
                        PozycjaMenu doZMIANY = zarzadca.WyszukajWMenu(staraNazwa);

                        if (doZMIANY != null)
                        {
                            Console.Write("Podaj nową nazwę: ");
                            string nowaNazwa = Console.ReadLine();
                            Console.Write("Podaj nową cenę: ");
                            if (!double.TryParse(Console.ReadLine(), out double nowaCena)) nowaCena = doZMIANY.CenaBazowa;

                            if (doZMIANY is Danie stareDanie)
                            {
                                Console.Write("Czy nowe danie jest pikantne? (tak/nie): ");
                                bool nowepika = Console.ReadLine().ToLower() == "tak";
                                zarzadca.ModyfikujPozycje(staraNazwa, new Danie(nowaNazwa, nowaCena, nowepika));
                            }
                            else if (doZMIANY is Napoj staryNapoj)
                            {
                                Console.Write("Podaj nową pojemność (w ml): ");
                                if (!int.TryParse(Console.ReadLine(), out int nowaPoj)) nowaPoj = staryNapoj.PojemnoscWMl;
                                zarzadca.ModyfikujPozycje(staraNazwa, new Napoj(nowaNazwa, nowaCena, nowaPoj));
                            }
                        }
                        else
                        {
                            Console.WriteLine("Pozycja o podanej nazwie nie istnieje.");
                        }
                        break;

                    case "5":
                        Console.WriteLine("--- NOWE ZAMÓWIENIE ---");
                        Console.WriteLine("Wybierz strategię wyceny: 1 - Standardowa, 2 - Zestawy (zniżka 20% przy napoju i daniu)");
                        string stratWyb = Console.ReadLine();

                        IStrategiaWyceny strategia = stratWyb == "2" ? new CennikZestawow() : new CennikStandardowy();
                        Zamowienie noweZamowienie = new Zamowienie(strategia);

                        bool kompletowanie = true;
                        while (kompletowanie)
                        {
                            Console.Write("Wpisz nazwę pozycji do dodania (lub 'gotowe' aby zakończyć): ");
                            string nazwaPoz = Console.ReadLine();
                            if (nazwaPoz.ToLower() == "gotowe")
                            {
                                kompletowanie = false;
                                break;
                            }

                            PozycjaMenu p = zarzadca.WyszukajWMenu(nazwaPoz);
                            if (p != null)
                            {
                                noweZamowienie += p;
                            }
                            else
                            {
                                Console.WriteLine("Brak takiej pozycji w menu!");
                            }
                        }

                        try
                        {
                            double suma = noweZamowienie.ObliczSume();
                            
                            double sumaPrzedZnizka = 0;
                            foreach (PozycjaMenu poz in noweZamowienie.Pozycje)
                            {
                                sumaPrzedZnizka += poz.CenaBazowa;
                            }

                            string nazwaStrategii = stratWyb == "2" ? "Cennik zestawów (promocyjny)" : "Cennik standardowy";

                            Console.WriteLine("\n--- REALIZACJA ZAMÓWIENIA ---");
                            noweZamowienie.ZmienStatus(StatusZamowienia.WPrzygotowaniu);

                            foreach (PozycjaMenu poz in noweZamowienie.Pozycje)
                            {
                                poz.Przygotuj();
                            }

                            noweZamowienie.ZmienStatus(StatusZamowienia.Gotowe);
                            noweZamowienie.ZmienStatus(StatusZamowienia.Zrealizowane);

                            zarzadca.ZlozZamowienie(noweZamowienie);

                            Console.WriteLine("\n--- PODSUMOWANIE ZAMÓWIENIA ---");
                            Console.WriteLine($"Strategia wyceny: {nazwaStrategii}");
                            Console.WriteLine("Zawartość koszyka:");
                            foreach (PozycjaMenu poz in noweZamowienie.Pozycje)
                            {
                                string typ = poz is Danie ? "Danie" : "Napój";
                                Console.WriteLine($"- [{typ}] {poz.Nazwa} ({Math.Round(poz.CenaBazowa, 2)} zł)");
                            }
                            Console.WriteLine($"Cena przed zniżką: {Math.Round(sumaPrzedZnizka, 2)} zł");
                            Console.WriteLine($"Cena po zniżce: {Math.Round(suma, 2)} zł");
                            Console.WriteLine("-------------------------------");
                        }
                        catch (WyjatekPustegoZamowienia ex)
                        {
                            Console.WriteLine($"\nBŁĄD: {ex.Message}");
                            Console.WriteLine("Zamówienie zostało anulowane.");
                        }
                        break;

                    case "6":
                        Console.WriteLine("--- ZAPIS DO PLIKU ---");
                        zarzadca.ZapiszStanDoPliku(domyslnyPlik);
                        break;

                    case "7":
                        Console.WriteLine("--- ODCZYT Z PLIKU ---");
                        zarzadca.WczytajStanZPliku(domyslnyPlik);
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