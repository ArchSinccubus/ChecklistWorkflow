﻿using System;
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
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for New_file_Dialoge.xaml
    /// </summary>
    public partial class New_file_Dialoge : Window
    {
        public New_file_Dialoge()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (textBox.Text != "")
            {
                ((MainWindow)Application.Current.MainWindow).createSave(textBox.Text);
                this.Close();
            }

        }

    }
}
