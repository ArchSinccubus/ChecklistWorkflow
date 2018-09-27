using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for StepItem.xaml
    /// </summary>
    public partial class StepItem : UserControl
    {
        Step baseStep;
        public int Stage;
        public bool Editing
        {

            get { return (bool)GetValue(_isEditingProperty); }

            set { SetValue(_isEditingProperty, value); }

        }


        public bool lastStepCompleted
        {

            get { return (bool)GetValue(_isFlagSetProperty); }

            set { SetValue(_isFlagSetProperty, value); }

        }

        public static readonly DependencyProperty _isFlagSetProperty =
            DependencyProperty.Register("lastStepCompleted", typeof(bool), typeof(StepItem), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty _isEditingProperty =
            DependencyProperty.Register("Editing", typeof(bool), typeof(StepItem), new FrameworkPropertyMetadata(false));

        public StepItem()
        {
            Editing = false;
            lastStepCompleted = false;
            this.Width = Application.Current.MainWindow.Width - 50;
            InitializeComponent();
        }

        public void addLinkRef(Grid hyper, int number)
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
            this.Tips.Document.Blocks.Clear();
            this.Tips.Document.Blocks.Add(new Paragraph(new Run(step.Tips)));

            this.Tips.TextChanged += Tips_TextChanged;

            string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\Workflow.xml");
            //string[] files = File.ReadAllLines(path);


            for (int i = 0; i < step.links.Count; i++)
            {
                addLink(step.links[i], i);
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

                baseStep.Checked = (bool)Checked.IsChecked;
                ((MainWindow)Application.Current.MainWindow).SaveSave(((MainWindow)Application.Current.MainWindow).SaveName);

            ((MainWindow)Application.Current.MainWindow).updateChecked();
        }

        private void addLink(string item, int num)
        {
            Grid linkGrid = new Grid();
            linkGrid.Width = 200;
            linkGrid.Name = "grid" + num;
            TextBlock tex = new TextBlock();
            Hyperlink hyper = new Hyperlink();
            Uri uri = new Uri(item);
            hyper.NavigateUri = uri;
            hyper.Inlines.Add(item);
            hyper.RequestNavigate += Hyper_RequestNavigate;
            tex.Inlines.Add(hyper);
            if (!baseStep.links.Contains(item))
            {
                baseStep.links.Add(item);
            }
            tex.HorizontalAlignment = HorizontalAlignment.Left;

            Button delButton = DeepCopy<Button>(RemoveLinkButton);
            BindingExpression bindingExpression = TipsList.GetBindingExpression(UIElement.VisibilityProperty);
            Binding parentBinding = bindingExpression.ParentBinding;
            delButton.SetBinding(UIElement.VisibilityProperty, parentBinding);
            delButton.Click += RemoveLinkButton_Click;
            //delButton.BindingGroup = TipsList.BindingGroup;

            linkGrid.Children.Add(tex);
            linkGrid.Children.Add(delButton);

            this.addLinkRef(linkGrid, 2);
        }

        private void AddLinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (NewlinkTextBox.Text.ToLower().Contains("http") && !baseStep.links.Contains(NewlinkTextBox.Text))
            {
                string newLink = NewlinkTextBox.Text;
                addLink(newLink, baseStep.links.Count);
                NewlinkTextBox.Text = "";
            }

        }

        private void RemoveLinkButton_Click(object sender, RoutedEventArgs e)
        {
            Grid grid = ((Button)e.Source).Parent as Grid;
            string ToRemove = ((grid.Children[0] as TextBlock).Inlines.FirstInline as Hyperlink).NavigateUri.ToString();
            baseStep.links.Remove(ToRemove);
            LinkPanel.Children.Remove(grid);
        }

        private void EditModeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Editing)
            {
                ((MainWindow)Application.Current.MainWindow).SaveSave(((MainWindow)Application.Current.MainWindow).SaveName);
                Editing = true;
            }
            else
            {
                Editing = false;
                ((MainWindow)Application.Current.MainWindow).SaveSave(((MainWindow)Application.Current.MainWindow).SaveName);
            }
        }

        private void Tips_TextChanged(object sender, TextChangedEventArgs e)
        {
            baseStep.Tips = new TextRange(Tips.Document.ContentStart, Tips.Document.ContentEnd).Text;
        }

        public T DeepCopy<T>(T element)
        {
            var xaml = XamlWriter.Save(element);
            var xamlString = new StringReader(xaml);
            var xmlTextReader = new XmlTextReader(xamlString);
            var deepCopyObject = (T)XamlReader.Load(xmlTextReader);
            return deepCopyObject;
        }

    }
}
