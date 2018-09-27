using System;
using System.Collections.Generic;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int stepSize = 5;
        public double windowWidth;
        public List<Step> CurrentList;

        public MainWindow()
        {
            CurrentList = new List<Step>();
            GenerateStepListFromXMLFile("Resources", "Workflow");
            InitializeComponent();
        }

        private StepItem GenerateStep(Step step)
        {
            //StepItem step = new StepItem();

            StepItem stepControl = new StepItem();

            stepControl.Label.Content = step.Title;
            stepControl.Description.Text = step.Description;
            stepControl.Checked.IsChecked = step.Checked;
            stepControl.lastStepCompleted = step.LastChecked;
            stepControl.Tips.Text = step.Tips;

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
                stepControl.addLinkRef(tex, 2); 
            }

            return stepControl;
        }

        private List<Step> GenerateStepListFromXMLFile(string Directory, string Name)
        {
            List<Step> steps = new List<Step>();

            string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Directory + @"\" + Name + ".xml");

            using (XmlReader reader = XmlReader.Create(path))
            {
                while (reader.ReadToFollowing("Title"))
                {
                    Step step = new Step();
                    // Only detect start elements.
                    if (reader.IsStartElement())
                    {
                        //string s = reader.GetAttribute("name");
                        // Get element name and switch on it.

                        //reader.ReadToDescendant("Title");

                        int stage = int.Parse(reader["Stage"]);
                        step.Stage = stage;

                        string stepName = reader["stepName"];
                        step.Title = stepName;
                        bool Check = reader["Checked"] == "True";
                        step.Checked = Check;


                        reader.ReadToDescendant("Description");
                        string DescData = reader["Data"];
                        step.Description = DescData;

                        reader.ReadToNextSibling("Links");


                        reader.ReadToDescendant("Tips");
                        string TipsText = reader["text"];
                        step.Tips = TipsText;
                        //step.Description.Text = DescData;

                        while (reader.ReadToNextSibling("Link"))
                        {
                            string link = reader["ref"];
                            step.links.Add(link);
                        }

                    }
                    if (steps.Count > 0)
                    {
                        step.LastChecked = steps.Last<Step>().Checked;
                    }
                    else
                        step.LastChecked = true;

                    steps.Add(step);
                }
            }

            return steps;

        }

        public void createSave(string projectName)
        {
            string Projectpath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Projects");
            string TemplatePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\Workflow.xml");
            List<StepItem> steps = new List<StepItem>();
            XmlDocument doc = new XmlDocument();
            doc.Load(TemplatePath);

            // Get and display all the book titles.
            XmlElement root = doc.DocumentElement;
            int elemList = root.GetElementsByTagName("Title").Count;

            for (int i = 0; i < elemList; i++)
            {
                StepItem item = new StepItem();
                item = GenerateStep(i);
                item.Stage = i;
                if (i == 0)
                {
                    item.lastStepCompleted = true;
                }
                steps.Add(item);
            }

            using (XmlWriter writer = XmlWriter.Create(Projectpath + @"\" + projectName + ".xml"))
            {

                writer.WriteStartElement("Save");
                writer.WriteAttributeString("name", projectName);
                foreach (var item in steps)
                {
                    writer.WriteElementString("Title","");
                    //writer.WriteAttributeString("Stage", item.Stage.ToString());
                    //writer.WriteAttributeString("stepName", item.Label.Content.ToString());
                    //writer.WriteAttributeString("Checked", item.Checked.IsChecked.ToString());

                    
                    writer.WriteElementString("author", "Mahesh Chand");
                    writer.WriteElementString("publisher", "Addison-Wesley");
                    writer.WriteElementString("price", "64.95");
                }
                writer.WriteEndElement();
                writer.Flush();
            }

        }

        private void OrganizeList(List<Step> steps)
        {
            foreach (var item in steps)
            {
                StepItem Control = GenerateStep(item);
                Border border = new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(0, 1, 0, 1), Height = 1f };
                border.SetBinding(Border.WidthProperty, new Binding() { ElementName = "Main_Window", Path = new PropertyPath("ActualWidth") });

                mainList.Items.Add(Control);
                mainList.Items.Add(border);
            }
        }

        private void SaveSave(string projectName, List<StepItem> steps)
        {
            string Projectpath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Projects");

            XmlDocument ProjectDoc = new XmlDocument();

            ProjectDoc.Load(Projectpath + @"\" + projectName + ".xml");

            foreach (var item in steps)
            {

            }

        }

        private void loadSave(string projectName)
        {
            GenerateStepListFromXMLFile("Projects", projectName);
        }

        private void Hyper_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            string s = ((Hyperlink)e.Source).NavigateUri.ToString();
            System.Diagnostics.Process.Start(((Hyperlink)e.Source).NavigateUri.ToString());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {   
            
        }



        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            New_file_Dialoge win2 = new New_file_Dialoge();
            win2.Show();
        }
    }
}
