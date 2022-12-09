using System;
namespace fut5.Data
{
    public class Clube{
        public string Nome { get; set; } = "";

        public Clube() { }
        public Clube(String nome)
        {
            this.Nome = nome;
        }
    }
}