using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WpfApplication1
{
    public class Step
    { 
        public int Stage;
        public bool Checked, LastChecked;
        public string Title, Description, Tips;
        public List<string> links;

        public Step()
        {
            links = new List<string>();
        }

        public Step(int stage, string title, string desc, string tips, List<string> Links)
        {
            this.Stage = stage;
            this.Checked = this.LastChecked = false;
            this.Title = title;
            this.Description = desc;
            this.Tips = tips;
            this.links = Links;
            convertLinks();
        }

        public Step(int stage, string title, string desc, string tips, List<string> Links,bool check, bool lastChecked)
        {
            this.Stage = stage;
            this.Checked = check;
            this.LastChecked = lastChecked;
            this.Title = title;
            this.Description = desc;
            this.Tips = tips;
            this.links = Links;
            convertLinks();
        }

        public void convertLinks()
        {
            for (int i = 0; i < links.Count; i++)
            {
                if (links[i].ToLower().Contains("youtube"))
                {
                    links[i] = links[i].Replace("&", "!");
                }
            }
        }

        public void convertLinksback()
        {
            for (int i = 0; i < links.Count; i++)
            {
                if (links[i].ToLower().Contains("youtube"))
                {
                    links[i] = links[i].Replace("!", "&");
                }
            }
        }
    }
}
