using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using ThirdParty;

namespace CreditExpensesApp
{
 
    public partial class Form1 : Form
    {
        string folderPath = string.Empty;
        string currentdir;
        
        string config = "config.txt";
        string missingValues = "missingValues.txt";
        string result = "results.txt";

        string dic = "dictionary.txt";
        //Dictionary<string, string> dictionary;

        bool firstround = true;

        //const string FolderRootStart = "FolderRootStart";
        //string FolderRootStartReadFromConfig; 
        

        

        Dictionary<string, creditResult> CategoryResultDic = new Dictionary<string, creditResult>();
        public Form1()
        {
            InitializeComponent();
            currentdir = System.IO.Directory.GetCurrentDirectory();
        }
        private bool init()
        {
            if (firstround)//try creating missing files , should be done only once
            {
                string missingfile = currentdir + @"\" + config;
                config = missingfile;
                List<string> missl = new List<string>();
                if (!System.IO.File.Exists(missingfile))
                {
                    TextBox1.BackColor = Color.Yellow;
                    TextBox1.AppendText(string.Format("config file is missing !! [{0}]\n creating new one", currentdir + @"\" + config));
                    missl.Add(missingfile);
                }
                missingfile = currentdir + @"\" + missingValues;
                missingValues = missingfile;
                if (!System.IO.File.Exists(missingfile))
                {
                    TextBox1.AppendText(string.Format("missing values file is missing !! [{0}]\n creating new one", currentdir + @"\" + config));
                    missl.Add(missingfile);
                }
                missingfile = currentdir + @"\" + dic;
                dic = missingfile;
                if (!System.IO.File.Exists(missingfile))
                {
                    TextBox1.AppendText(string.Format("dictionary file is missing !! [{0}]\n creating new one", currentdir + @"\" + config));
                    missl.Add(missingfile);
                }

                foreach (var item in missl)
                {
                    System.IO.File.Create(item);
                }
                firstround = false;
            }
            /////////////////////////////////read config file /////////////////////////////////////////////////
            //ReadConfigFile();
            ///////////////////////////////////////////////////////////////////////////////////////////////////
            if (!System.IO.File.Exists(missingValues))
            {
                System.IO.File.Create(missingValues);
            }

            ConfigurationManger.getCMInstance().readConfigFile();

            /*read missing vlaues if there unanswered categories break else add them to dictionary and clear file !!!*/

            /*if (System.IO.File.ReadAllText(missingValues).
                    Replace('\t',' ').Replace('\r',' ').
                        Replace('\n',' ').Trim().Length == 0){*/
            
            List<BwithoutCategory> BwoC = new List<BwithoutCategory>();
            
            ConfigurationManger.getCMInstance().getConfig().BussinessWithoutCategory.ForEach(x=> {
                if (string.IsNullOrEmpty( x.category)) BwoC.Add(x);
                else/* adding filled categories to B2Category dictionary */
                {
                    ConfigurationManger.getCMInstance().getConfig().B2Category.Add(x.name, x.category);
                }
            });

            if (BwoC.Count == 0)
            {
                /*no empty categories !! we can now remove the list */
                ConfigurationManger.getCMInstance().getConfig().BussinessWithoutCategory.Clear();
                result = currentdir + @"\" + result;
                return true;
            }
            else
            {
                TextBox1.BackColor = Color.Red;
                TextBox1.AppendText("missing category are not empty is not empty !!!");
                TextBox1.AppendText("1) go over it and fill missing categories!!");
                TextBox1.AppendText("2) give High5 to yourself");
                TextBox1.AppendText("3) try again");
                return false;
            }
            
        }

        enum FILES_TO_WRITE
        {
            CONFIG=0,
            MISSING,
            RESULT
        }

        private void writeFile(FILES_TO_WRITE file,string input)
        {
            switch (file)
            {
                //case FILES_TO_WRITE.CONFIG:
                //    writeConfigFile(input);
                //    break;
                case FILES_TO_WRITE.MISSING:
                    writeMissingFile(input);
                    break;
                default:
                    break;
            }
        }
        private void writeFile(FILES_TO_WRITE file,creditResult res)
        {
            System.IO.File.AppendAllText(result, "************************************************************\n");
            System.IO.File.AppendAllText(result, string.Format("category:\n{0}\n\nsum:\n\n {1}\n\n", res.category,res.sum));
            
            foreach (var r in res.relatedRows)
            {
                string row = "";
                r.Reverse();
                foreach (var w in r)
                {
                    row += w + "\t";    
                }
                System.IO.File.AppendAllText(result, row + "\n\n");    
            }   
        }
        private void ReadConfigFile()
        {
            if (!System.IO.File.Exists(config))
                System.IO.File.AppendAllText(config, "");
            var configrows = System.IO.File.ReadAllLines(config);
            Config.instance().initConfig(configrows.ToList(),config);
            Config.instance().parseConfig();



        }
        /*private void writeConfigFile(string input)
        {
            if (System.IO.File.Exists(config))
            {
                System.IO.File.AppendAllLines(config,input.Split('\n'));
            }
        }*/
        private void writeMissingFile(string input)
        {
            if (System.IO.File.Exists(missingValues))
            {
                System.IO.File.AppendAllText(missingValues,input +"=" + "????" + "\n\r");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            TextBox1.Clear();
            TextBox1.BackColor = Color.White;
            //check integritry 
            if (init() == false)
                return;

            SetConfBackToStart();

            IgetFilesToParse FileSeekerInterface = new getFilesFromFolder(folderBrowserDialog1, TextBox1);

            var files = FileSeekerInterface.getFiles();

            //get all files in chosen folder

            handleFiles(files);

        }

        private void handleFiles(List<string> files)
        {
            List<string> excels = new List<string>();

            //fill up dictionary
            /*dictionary = new Dictionary<string, string>();
            foreach (var r in System.IO.File.ReadAllLines(dic))
            {
                if (!string.IsNullOrEmpty(r))
                {
                    var words = r.Split('=').ToList();

                    if (words.Count == 2 && !dictionary.ContainsKey(words[0]))
                    {
                        dictionary.Add(words[0], words[1]);
                    }
                }
            }*/

            //get all excels 
            string exten;
            foreach (var f in files)
            {
                exten = System.IO.Path.GetExtension(f);
                if (exten == ".xlsx" || exten == ".xls")
                    excels.Add(f);
            }
                
            if (excels.Count == 0)
            {
                TextBox1.AppendText("no excel files found in folder !!\n");
                TextBox1.BackColor = Color.Green;
                return;
            }



            //iterate over excels;
            foreach (var ex in excels)
            {
                var excel = Read_From_Excel.getExcelFile(ex);
                TextBox1.BackColor = Color.Green;
                TextBox1.AppendText(string.Format("reading excel:[{0}]\n", ex));

                //calc MD5 for excel template ////////
                string template = "";
                foreach (var w in excel[0])
                {
                    template += w;
                }
                var MD5 = Read_From_Excel.CalculateMD5Hash(template);
                //////////////////////////////////////
                var configuration = ConfigurationManger.getCMInstance().getConfig();
                credit cval;
                //if (!Config.instance().templateDic.TryGetValue(MD5, out cval))
               
                if (!configuration.ExTemplates.TryGetValue(MD5, out cval))
                {/*temp solution !!*/
                    
                    configuration.ExTemplates.Add(MD5, new credit(-1, -1, -1));
                    //writeConfigFile(Exceltemplate + "=" + MD5 + "," + "?????" + "\n");

                    TextBox1.BackColor = Color.Red;
                    TextBox1.AppendText(string.Format("missing template in config file[{0}] !!!\n", config));
                    TextBox1.AppendText("better call Matan Integration Inc. [052-7758661]\n");
                    return;
                }
                else//template found
                {
                    List<string> r;
                    //var excelRanged = excel.GetRange(cval.StartRow, excel.Count - 1);
                    for (int i = cval.StartRow; i < excel.Count; i++)
                    //foreach (var r in excel.GetRange(1,excel.Count-1))
                    {
                        r = excel[i];
                        if (r.Count < cval.sum || r.Count < cval.nameIndex)
                            continue;
                        string category;
                        if (!configuration.B2Category.TryGetValue(r[cval.nameIndex], out category))
                        {
                            configuration.BussinessWithoutCategory.Add(new BwithoutCategory( r[cval.nameIndex]));
                            //writeFile(FILES_TO_WRITE.MISSING, r[cval.nameIndex]);
                            //TextBox1.BackColor = Color.Yellow;
                            TextBox1.AppendText(string.Format("missing category for [{0}] !!!\n\n", r[cval.nameIndex]));
                        }
                        else//value found in dectionary
                        {
                            if (cval.namesToExclude.Contains(category))
                            {
                                continue;//leave current iteration , exclude filter triggered 
                            }
                            if (!CategoryResultDic.ContainsKey(category))
                            {
                                CategoryResultDic.Add(category, new creditResult());
                            }
                            double dSum = Convert.ToDouble(r[cval.sum]);
                            if (dSum > 0)
                            {
                                CategoryResultDic[category].category = category;
                                CategoryResultDic[category].relatedRows.Add(r);
                                CategoryResultDic[category].sum += dSum;
                            }
                            


                        }
                    }
                    TextBox1.BackColor = Color.WhiteSmoke;


                    TextBox1.AppendText("parsing finished\n");

                    System.IO.File.WriteAllText(result, "");
                    foreach (var pair in CategoryResultDic)
                    {
                        TextBox1.AppendText(string.Format("Category\n{0}\n", pair.Value.category));
                        TextBox1.AppendText(string.Format("final sum\n{0}\n", pair.Value.sum));
                        writeFile(FILES_TO_WRITE.RESULT, pair.Value);
                    }
                }
            }
        }

        private void SetConfBackToStart()
        {
            result = "results.txt";
            CategoryResultDic.Clear();
            /*erase missing entries*/
        }

        private void ChooseFile_Click(object sender, EventArgs e)
        {
            TextBox1.Clear();
            TextBox1.BackColor = Color.White;
            //check integritry 
            if (init() == false)
                return;

            SetConfBackToStart();

            IgetFilesToParse FileSeekerInterface = new getFile(openFileDialog1, TextBox1);

            var files = FileSeekerInterface.getFiles();
            
            //get all files in chosen folder
            handleFiles(files);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Config.instance().writeConfigFile();
            if (ConfigurationManger.getCMInstance().IsConfigFull)
                ConfigurationManger.getCMInstance().saveConfigFile(ConfigurationManger.getCMInstance().getConfig());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            init();
            
            /*if (dictionary.Count != 0)
            {
                Configuration confi = new Configuration();
                confi.ExTemplates = Config.instance().templateDic;
                //confi.ExTemplates
                confi.B2Category = dictionary;
                ConfigurationManger.getCMInstance().saveConfigFile(confi);
            }else*/
            {
                ConfigurationManger.getCMInstance().readConfigFile();
                ConfigurationManger.getCMInstance().saveConfigFile(ConfigurationManger.getCMInstance().getConfig());
            }

        }

    }
    public class creditResult
    {
        public creditResult()
        {
            relatedRows = new List<List<string>>();
            sum = 0;
        }
        public string category;
        public List<List<string>> relatedRows;
        public double sum;

    }
    public class Config
    {
        private static Config inst;
        private Config()
        {

        }
        public static Config instance()
        {
            if (inst == null) inst = new Config();
            return inst;
        }
        private const string Exceltemplate = "Exceltemplate";
        //private const string StartDialogFolder = "StartDialogFolder";
        //public string startDialogFolder;
        List<string> execlExtensions;
        public Dictionary<string, credit> templateDic;
        List<string> rows;
        private string configPath;
        public void initConfig(List<string> rows,string configPath)
        {
            this.configPath = configPath;
            this.rows = rows;
            execlExtensions = new List<string>();
            templateDic = new Dictionary<string, credit>();
        }
        public void parseConfig()
        {
            foreach (var r in rows)
            {
                List<string> w = r.Split('=').ToList();

                /*if (w[0] == StartDialogFolder)
                {
                    startDialogFolder = w[1];
                }
                else */
                if (w[0] == Exceltemplate)
                {
                    var t = w[1].Split(',');
                    var c = t[1].Split('|');
                    if (c.Length == 3)
                        templateDic.Add(t[0], new credit(Convert.ToInt32(c[0]), Convert.ToInt32(c[1]), Convert.ToInt32(c[2])));
                }
            }
        }
        public void writeConfigFile()
        {
            if (templateDic == null)
                return;
                   
            foreach (var pair in templateDic)
            {
                if (pair.Value.nameIndex == -1)
                {
                    execlExtensions.Add(Exceltemplate + "=" + pair.Key + "," + "nameIndex_zeroBasedIndex|sum|startRow_zeroBasedIndex" + "\n");
                }else
                {
                    execlExtensions.Add(Exceltemplate + "=" + pair.Key + "," + pair.Value.nameIndex +  "|" + pair.Value.sum +"|"+pair.Value.StartRow + "\n");
                }
            }
            //Exceltemplate + "=" + MD5 + "," + "?????" + "\n"
            System.IO.File.WriteAllLines(configPath, execlExtensions);
           // System.IO.File.WriteAllText(configPath, startDialogFolder + "\n");
        }
    }
 
}
