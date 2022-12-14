using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fintech.Dominio.Entidades
{
    public class ContaCorrente
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public string DigitoVerificador { get; set; }
        public decimal Saldo { get; set; }
        public Agencia Agencia { get; set; }
        public Cliente Cliente { get; set; }

        public void EfetuarOperacao(decimal valor, TipoOperacao tipoOperacao)
        {
            switch (tipoOperacao)
            {
                case TipoOperacao.Deposito:
                    Saldo += valor;
                    break;
                case TipoOperacao.Saque:
                    Saldo -= valor;
                    break;
                
            }
        }
    }
}
