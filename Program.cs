using System;
using System.Collections.Generic;

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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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

        public ZarzadcaRestauracji()
        {
            dostepneMenu = new List<PozycjaMenu>();
            historiaZamowien = new List<Zamowienie>();
        }

        public void DodajDoMenu(PozycjaMenu pozycja)
        {
            throw new NotImplementedException();
        }

        public void ModyfikujPozycje(string staraNazwa, PozycjaMenu nowaPozycja)
        {
            throw new NotImplementedException();
        }

        public PozycjaMenu WyszukajWMenu(string nazwa)
        {
            throw new NotImplementedException();
        }

        public void ZlozZamowienie(Zamowienie zamowienie)
        {
            throw new NotImplementedException();
        }

        public void ZapiszStanDoPliku(string sciezka)
        {
            throw new NotImplementedException();
        }

        public void WczytajStanZPliku(string sciezka)
        {
            throw new NotImplementedException();
        }
    }
}