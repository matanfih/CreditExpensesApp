using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CreditExpensesApp
{
    public class credit
    {
        //public credit(int nameIndex, string nameIndexString, int sum, string sumCellName)
        public credit(int nameIndex, int sum, int StartRow)
        {
            this.nameIndex = nameIndex;
            //this.nameIndexString = nameIndexString;
            this.sum = sum;
            //this.sumCellName = sumCellName;
            this.StartRow = StartRow;
            namesToExclude = new List<string>();
            namesToExclude.Add("לאצריך");
        }
        public List<string> namesToExclude;
        public int nameIndex;
        //public string nameIndexString;//what is written in the cell  ...
        public int sum;
        public int StartRow;
        //public string sumCellName;
    }
    public class ExcelTemplate
    {
        string md5Sig;
        int BNameZeroBasedIndex;
        int SumZeroBasedIndex;
        int StartRowZeroBasedIndex;
    }
    public class Configuration
    {
        public string LastFolderDialogPath;
        public string LastFileDialogPath;
        public Dictionary<string, string> B2Category = new Dictionary<string, string>();
        public Dictionary<string, credit> ExTemplates = new Dictionary<string, credit>();
    }

    class ConfigurationManger
    {

        private Configuration config = new Configuration();
        public void readConfigFile()
        {
            if (System.IO.File.Exists(System.IO.Directory.GetCurrentDirectory() + @"\Configuration2Json"))
            {
                var json = System.IO.File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Configuration2Json");
                config = JsonConvert.DeserializeObject<Configuration>(json);
            }
        }
        public void saveConfigFile(object conf=null)
        {
            string json;
            if (conf == null)//debug ?
            {
                //((Configuration)config).B2Category.Add("kaka", "bathroom");
                //((Configuration)config).B2Category.Add("pipi", "livingroom");
                //Save(config, System.IO.Directory.GetCurrentDirectory() + "\\Config.xml", typeof(Config));
                json = JsonConvert.SerializeObject(config);

            }
            json = JsonConvert.SerializeObject(conf);

            System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + @"\Configuration2Json", json);

            Configuration confi2 = JsonConvert.DeserializeObject<Configuration>(json);    
        }

        /*private void Save(Object file, String path, Type type)
        {
            
            // Create a new Serializer
            XmlSerializer serializer = new XmlSerializer(type);

            // Create a new StreamWriter
            System.IO.TextWriter writer = new System.IO.StreamWriter(path);

            // Serialize the file
            serializer.Serialize(writer, file);

            // Close the writer
            writer.Close();
        }

        private object Read(String path, Type type)
        {
            // Create a new serializer
            XmlSerializer serializer = new XmlSerializer(type);

            // Create a StreamReader
            System.IO.TextReader reader = new System.IO.StreamReader(path);

            // Deserialize the file
            Object file;
            file = (Object)serializer.Deserialize(reader);

            // Close the reader
            reader.Close();

            // Return the object
            return file;
        }*/


    }
}
