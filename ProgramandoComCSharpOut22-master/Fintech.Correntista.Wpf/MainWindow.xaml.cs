using Fintech.Dominio.Entidades;
using Fintech.Repositorios.SistemaArquivos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fintech.Correntista.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<Cliente> clientes = new();
        private Cliente clienteSelecionado;
        private readonly MovimentoRepositorio movimentoRepositorio = new (Properties.Settings.Default.CaminhoArquivoMovimento);

        public MainWindow()
        {
            InitializeComponent();
            PopularControles();
        }

        private void PopularControles()
        {
            sexoComboBox.Items.Add(Sexo.Feminino);
            sexoComboBox.Items.Add(Sexo.Masculino);
            sexoComboBox.Items.Add(Sexo.Outro);

            clienteDataGrid.ItemsSource = clientes;

            tipoContaComboBox.Items.Add(TipoConta.ContaCorrente);
            tipoContaComboBox.Items.Add(TipoConta.ContaEspecial);
            tipoContaComboBox.Items.Add(TipoConta.Poupanca);

            var banco1 = new Banco();
            banco1.Nome = "Banco Um";
            banco1.Numero = 206;

            bancoComboBox.Items.Add(banco1);
            bancoComboBox.Items.Add(new Banco { Nome = "Banco Dois", Numero = 211 });

            operacaoComboBox.Items.Add(TipoOperacao.Deposito);
            operacaoComboBox.Items.Add(TipoOperacao.Saque);
        }

        private void incluirClienteButton_Click(object sender, RoutedEventArgs e)
        {
            var cliente = new Cliente();
            cliente.Cpf = cpfTextBox.Text;
            cliente.DataNascimento = Convert.ToDateTime(dataNascimentoTextBox.Text);
            cliente.Nome = nomeTextBox.Text;
            cliente.Sexo = (Sexo)sexoComboBox.SelectedItem;

            var endereco = new Endereco();
            endereco.Cep = cepTextBox.Text;
            endereco.Cidade = cidadeTextBox.Text;
            endereco.Logradouro = logradouroTextBox.Text;
            endereco.Numero = numeroLogradouroTextBox.Text;

            cliente.EnderecoResidencial = endereco;

            clientes.Add(cliente);

            MessageBox.Show("Cliente cadastrado com sucesso.");
            LimparControlesCliente();
            clienteDataGrid.Items.Refresh();
            pesquisaClienteTabItem.Focus();
        }

        private void LimparControlesCliente()
        {
            cpfTextBox.Clear();
            nomeTextBox.Text = "";
            dataNascimentoTextBox.Text = null;
            sexoComboBox.SelectedIndex = -1;
            logradouroTextBox.Text = string.Empty;
            numeroLogradouroTextBox.Clear();
            cidadeTextBox.Clear();
            cepTextBox.Clear();
        }

        private void SelecionarClienteButtonClick(object sender, RoutedEventArgs e)
        {
            SelecionarCliente(sender);

            clienteTextBox.Text = $"{clienteSelecionado.Nome} - {clienteSelecionado.Cpf}";

            contasTabItem.Focus();
        }

        private void SelecionarCliente(object sender)
        {
            var botaoClicado = (Button)sender;

            clienteSelecionado = (Cliente)botaoClicado.DataContext;
        }

        private void tipoContaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tipoContaComboBox.SelectedItem == null)
            {
                return;
            }
            
            var tipoConta = (TipoConta)tipoContaComboBox.SelectedItem;

            if (tipoConta == TipoConta.ContaEspecial)
            {
                limiteDockPanel.Visibility = Visibility.Visible;
                limiteTextBox.Focus();
            }
            else
            {
                limiteDockPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void incluirContaButton_Click(object sender, RoutedEventArgs e)
        {
            Conta conta = null;

            var agencia = new Agencia();
            agencia.Banco = (Banco)bancoComboBox.SelectedItem;
            agencia.Numero = Convert.ToInt32(numeroAgenciaTextBox.Text);
            agencia.DigitoVerificador = Convert.ToInt32(dvAgenciaTextBox.Text);

            var numero = Convert.ToInt32(numeroContaTextBox.Text);
            var digitoVerificador = dvContaTextBox.Text;

            switch ((TipoConta)tipoContaComboBox.SelectedItem)
            {
                case TipoConta.ContaCorrente:
                    conta = new ContaCorrente(agencia, numero, digitoVerificador);
                    break;
                case TipoConta.ContaEspecial:
                    var limite = Convert.ToDecimal(limiteTextBox.Text);
                    conta = new ContaEspecial(agencia, numero, digitoVerificador, limite);
                    break;
                case TipoConta.Poupanca:
                    conta = new Poupanca(agencia, numero, digitoVerificador);
                    break;
            }

            //clienteSelecionado.Contas = new List<Conta>();

            clienteSelecionado.Contas.Add(conta);

            MessageBox.Show("Conta adicionada com sucesso.");
            LimparControlesConta();
            clienteDataGrid.Items.Refresh();
            clientesTabItem.Focus();
        }

        private void LimparControlesConta()
        {
            clienteTextBox.Clear();
            bancoComboBox.SelectedIndex = -1;
            numeroAgenciaTextBox.Clear();
            dvAgenciaTextBox.Clear();
            numeroContaTextBox.Clear();
            dvContaTextBox.Clear();
            tipoContaComboBox.SelectedIndex = -1;
            limiteTextBox.Clear();
        }

        private void SelecionarContaButtonClick(object sender, RoutedEventArgs e)
        {
            SelecionarCliente(sender);

            contaTextBox.Text = $"{clienteSelecionado.Nome} - {clienteSelecionado.Cpf}";

            contaComboBox.ItemsSource = clienteSelecionado.Contas;
            contaComboBox.Items.Refresh();

            LimparControlesOperacoes();

            operacoesTabItem.Focus();
        }

        private void LimparControlesOperacoes()
        {
            contaComboBox.SelectedIndex = -1;
            operacaoComboBox.SelectedIndex = -1;
            valorTextBox.Clear();
            movimentacaoDataGrid.ItemsSource = null;
            saldoTextBox.Clear();
        }

        private async /*Task*/ void contaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contaComboBox.SelectedItem == null) return;

            mainSpinner.Visibility = Visibility.Visible;

            var conta = (Conta)contaComboBox.SelectedItem;

            conta.Movimentos = await movimentoRepositorio.SelecionarAsync(conta.Agencia.Numero, conta.Numero);

            movimentacaoDataGrid.ItemsSource = conta.Movimentos;
            saldoTextBox.Text = conta.Saldo.ToString("C");

            mainSpinner.Visibility = Visibility.Hidden;
        }

        private void incluirOperacaoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var conta = (Conta)contaComboBox.SelectedItem;
                var operacao = (TipoOperacao)operacaoComboBox.SelectedItem;
                var valor = Convert.ToDecimal(valorTextBox.Text);

                var movimento = conta.EfetuarOperacao(valor, operacao);

                movimentoRepositorio.Inserir(movimento);

                movimentacaoDataGrid.ItemsSource = conta.Movimentos;
                movimentacaoDataGrid.Items.Refresh();

                saldoTextBox.Text = conta.Saldo.ToString("C");
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"O arquivo {ex.FileName} não foi encontrado.");
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show($"O diretório {Properties.Settings.Default.CaminhoArquivoMovimento} não foi encontrado.");
            }
            catch (SaldoInsuficienteException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception excecao)
            {
                MessageBox.Show("Eita! Algo deu errado e em breve teremos uma solução.");
                //Logar(excecao); // log4Net
            }
            finally
            {
                // É sempre executado, mesmo que haja um return no código.
            }
        }
    }
}