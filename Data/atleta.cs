using System;
namespace fut5.Data
{
    public class Atleta{
        public string Email { get; set; } = "";
        public string Nome { get; set; } = "";
        public string Pass { get; set; } = "";
        public bool Ativo { get; set; } = false;
        public bool IsAdmin { get; set; } = false;

        public Atleta() { }
        public Atleta(String nome, String email, String pass, bool ativo, bool admin)
        {
            this.Email = email;
            this.Nome = nome;
            this.Pass = pass;
            this.Ativo = ativo;
            this.IsAdmin = admin;
        }
    }
}