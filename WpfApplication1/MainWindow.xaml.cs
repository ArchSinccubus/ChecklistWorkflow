using Microsoft.Win32;
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
        public double windowWidth;
        public List<Step> CurrentList;

        public string SaveName;

        public bool PracticeMode;

        public MainWindow()
        {
            SaveName = "New Project";
            PracticeMode = false;
            InitializeComponent();
        }


        #region Save Handling
        public void createSave(string projectName)
        {
            SaveSave(SaveName);
            List<Step> steps = GenerateStepListFromXMLFile("Resources", "Workflow");
            SaveName = projectName;
            mainList.Items.Clear();
            OrganizeList(steps);
            this.Title = (projectName != "") ? projectName : "New Project";
        }

        public void SaveSave(string projectName)
        {
            if (!PracticeMode)
            {
                if (SaveName != null && SaveName != "")
                {
                    List<Step> Steps = GenerateStepListFromVisualTree();

                    XmlDocument xmlDoc = new XmlDocument();
                    XmlNode rootNode = xmlDoc.CreateElement("Save");
                    XmlAttribute attribute = xmlDoc.CreateAttribute("name");
                    attribute.Value = projectName;
                    rootNode.Attributes.Append(attribute);
                    xmlDoc.AppendChild(rootNode);

                    foreach (var item in Steps)
                    {
                        XmlNode TitleNode = xmlDoc.CreateElement("Title");
                        XmlAttribute StageAtt = xmlDoc.CreateAttribute("Stage");
                        StageAtt.Value = item.Stage.ToString();
                        XmlAttribute NameAtt = xmlDoc.CreateAttribute("stepName");
                        NameAtt.Value = item.Title;
                        XmlAttribute CheckedAtt = xmlDoc.CreateAttribute("Checked");
                        CheckedAtt.Value = item.Checked.ToString();
                        TitleNode.Attributes.Append(StageAtt);
                        TitleNode.Attributes.Append(NameAtt);
                        TitleNode.Attributes.Append(CheckedAtt);

                        XmlNode DescriptionNode = xmlDoc.CreateElement("Description");
                        XmlAttribute DataAtt = xmlDoc.CreateAttribute("Data");
                        DataAtt.Value = item.Description;
                        DescriptionNode.Attributes.Append(DataAtt);

                        TitleNode.AppendChild(DescriptionNode);

                        XmlNode LinksNode = xmlDoc.CreateElement("Links");

                        XmlNode TipsNode = xmlDoc.CreateElement("Tips");
                        XmlAttribute TipsAtt = xmlDoc.CreateAttribute("text");
                        TipsAtt.Value = item.Tips;
                        TipsNode.Attributes.Append(TipsAtt);
                        LinksNode.AppendChild(TipsNode);

                        foreach (var link in item.links)
                        {
                            XmlNode UrlNode = xmlDoc.CreateElement("Link");
                            XmlAttribute UrlAtt = xmlDoc.CreateAttribute("ref");
                            UrlAtt.Value = link;
                            UrlNode.Attributes.Append(UrlAtt);
                            LinksNode.AppendChild(UrlNode);
                        }

                        TitleNode.AppendChild(LinksNode);

                        rootNode.AppendChild(TitleNode);
                    }

                    xmlDoc.Save("Projects/" + projectName + ".xml");
                }
                            
            }

        }

        private void loadSave(string projectName)
        {
            SaveSave(SaveName);
            List<Step> steps = GenerateStepListFromXMLFile("Projects", projectName);
            mainList.Items.Clear();
            SaveName = projectName;
            OrganizeList(steps);
            updateChecked();
            this.Title = projectName;
        }
        #endregion       

        public void updateChecked()
        {
            bool check = true;
            foreach (var item in mainList.Items)
            {
                if (item is StepItem)
                {
                    (item as StepItem).lastStepCompleted = check;
                    check = (bool)(item as StepItem).Checked.IsChecked;
                }

            }
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

                        step.convertLinksback();

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

        private List<Step> GenerateStepListFromVisualTree()
        {
            List<Step> steps = new List<Step>();

            for (int i = 0; i < mainList.Items.Count; i++)
            {
                var item = mainList.Items[i];
                if (item is StepItem)
                {
                    Step s = (item as StepItem).ControlToObject();
                    s.LastChecked = (i>0) ? steps[i/2 - 1].Checked : true;
                    steps.Add(s);

                }
            }

            return steps;

        }

        private void OrganizeList(List<Step> steps)
        {
            foreach (var item in steps)
            {
                StepItem Control = new StepItem();
                Control.generateData(item);
                Border border = new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(0, 1, 0, 1), Height = 1f };
                border.Width = this.Width - 50;

                mainList.Items.Add(Control);
                mainList.Items.Add(border);
            }
        }

        #region Events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            createSave("");
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            New_file_Dialoge win2 = new New_file_Dialoge();
            win2.Show();
        }
        #endregion

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if ((SaveName != null && SaveName != ""))
            {
                SaveSave(SaveName);
            }
            else
                saveAs();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            SaveSave(SaveName);
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Project files (*.xml)|*.xml";
            if (openFileDialog.ShowDialog() == true)
            {
                string name = openFileDialog.SafeFileName.Split('.')[0];
                loadSave(name);
                SaveName = name;
            }


        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            //updateChecked();
            SaveSave(SaveName);
            List<Step> steps = GenerateStepListFromXMLFile("Resources", "Practice");
            CurrentList = GenerateStepListFromVisualTree();
            mainList.Items.Clear();
            PracticeMode = true;         
            OrganizeList(steps);
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            mainList.Items.Clear();
            PracticeMode = false;
            OrganizeList(CurrentList);
            updateChecked();
            SaveSave(SaveName);
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            saveAs();
        }

        private void saveAs()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Project files (*.xml)|*.xml";
            dialog.FilterIndex = 2;
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == true)
            {
                string name = dialog.SafeFileName.Split('.')[0];
                SaveSave(name);
                SaveName = name;
            }
        }
    }
}
