using Fintech.Dominio.Entidades;
using System;
using System.Collections.Generic;
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
        }

        private void incluirClienteButton_Click(object sender, RoutedEventArgs e)
        {
            var cliente = new Cliente();
            cliente.Cpf = cpfTexBox.Text;
            cliente.DataNascimento = Convert.ToDateTime(dataNascimentoTexBox.Text);
            cliente.Nome = nomeTexBox.Text;
            cliente.Sexo = (Sexo)sexoComboBox.SelectedItem;

            var endereco = new Endereco();
            endereco.Cep = cepTexBox.Text;
            endereco.Cidade = cidadeTexBox.Text;
            endereco.Logradouro = LogradouroTexBox.Text;
            endereco.Numero = numeroLogradouroTexBox.Text;

            cliente.EnderecoResidencial = endereco;

        }
    }
}
