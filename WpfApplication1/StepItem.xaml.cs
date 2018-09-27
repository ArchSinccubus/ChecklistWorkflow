using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for StepItem.xaml
    /// </summary>
    public partial class StepItem : UserControl
    {
        Step baseStep;
        public int Stage;

        public bool lastStepCompleted
        {

            get { return (bool)GetValue(_isFlagSetProperty); }

            set { SetValue(_isFlagSetProperty, value); }

        }

        public static readonly DependencyProperty _isFlagSetProperty =
            DependencyProperty.Register("lastStepCompleted", typeof(bool), typeof(StepItem), new FrameworkPropertyMetadata(false));


        public StepItem()
        {
            lastStepCompleted = false;
            this.Width = Application.Current.MainWindow.Width - 50;
            InitializeComponent();
        }

        public void addLinkRef(TextBlock hyper, int number)
        {
            LinkPanel.Children.Insert(number, hyper);
        }

        public Step ControlToObject()
        {
            return baseStep;
        }

        public void generateData(Step step)
        {
            this.baseStep = step;

            this.Label.Content = step.Title;
            this.Description.Text = step.Description;
            this.Checked.IsChecked = step.Checked;
            this.lastStepCompleted = step.LastChecked;
            this.Tips.Text = step.Tips;

            string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\Workflow.xml");
            //string[] files = File.ReadAllLines(path);


            foreach (var item in step.links)
            {
                TextBlock tex = new TextBlock();
                Hyperlink hyper = new Hyperlink();
                Uri uri = new Uri(item);
                hyper.NavigateUri = uri;
                hyper.Inlines.Add(item);
                hyper.RequestNavigate += Hyper_RequestNavigate;
                tex.Inlines.Add(hyper);
                this.addLinkRef(tex, 2);
            }

            this.Checked.Checked += Checked_Checked;
        }

        private void Hyper_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            string s = ((Hyperlink)e.Source).NavigateUri.ToString();
            System.Diagnostics.Process.Start(((Hyperlink)e.Source).NavigateUri.ToString());
        }

        private void Checked_Checked(object sender, RoutedEventArgs e)
        {
            if (baseStep.LastChecked)
            {
                baseStep.Checked = (bool)Checked.IsChecked;
                ((MainWindow)Application.Current.MainWindow).SaveSave(((MainWindow)Application.Current.MainWindow).SaveName);
            }

            ((MainWindow)Application.Current.MainWindow).updateChecked();
        }


    }
}
