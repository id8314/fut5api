using System;
namespace fut5.Data
{
    public class Jogo
    {   
        public string Clube { get; set; } = "";
        public DateTimeOffset DataJogo { get; set; } = DateTime.Today;
        public DateTimeOffset DataInscricoes { get; set; } = DateTime.Today;
        public DateTimeOffset DataInscricoesFim { get; set; } = DateTime.Today;
        public string Campo { get; set; } = "interior";
        public Jogo() { }
        
        public Jogo(String clube, String dataJogo, String dataInscricoes, String campo = "interior")
        {
            string[] data1;
            string[] data2;

            if (dataJogo.Contains("T"))
            {
                data1 = dataJogo.Split("T")[0].Split("-");     // "2021-12-20T00:00:00+00:00"
                data2 = dataInscricoes.Split("T")[0].Split("-");
            }
            else
            {
                data1 = dataJogo.Split(" ")[0].Split("-");     // "2021-12-09 00:00:00+00:00"
                data2 = dataInscricoes.Split(" ")[0].Split("-");
            }

            this.Clube = clube;
            this.Campo = campo;

            this.DataJogo = new DateTimeOffset(
                year : int.Parse(data1[0]),
                month : int.Parse(data1[1]),
                day : int.Parse(data1[2]),
                hour:0,minute:0,second:0, new TimeSpan(0)
                );

            this.DataInscricoes = new DateTimeOffset(
                year : int.Parse(data2[0]),
                month : int.Parse(data2[1]),
                day : int.Parse(data2[2]),
                hour:0,minute:0,second:0, new TimeSpan(0)
                );

            this.DataInscricoesFim = this.DataJogo.AddMinutes(17 * 60 + 30);

        }
    }
}