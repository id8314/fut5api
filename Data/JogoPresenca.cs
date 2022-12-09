using System;
namespace fut5.Data
{
    public class JogoPresenca
    {
        public string Nome { get; set; } = "";
        public string Email { get; set; } = "";
        public string Clube { get; set; } = "";
        public int Resposta { get; set; } = 0;
        public DateTimeOffset DataJogo { get; set; } = DateTime.Today;

        public JogoPresenca() { }
        public JogoPresenca(String nome, String email, String clube, int resposta, DateTimeOffset dataJogo)
        {
            this.Nome = nome;
            this.Email = email;
            this.Clube = clube;
            this.Resposta = resposta;
            this.DataJogo = dataJogo;
        }
    }
}