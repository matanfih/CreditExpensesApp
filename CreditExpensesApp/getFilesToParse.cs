using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace CreditExpensesApp
{
    interface IgetFilesToParse
    {
        List<string> getFiles();
    }

    class getFilesFromFolder : IgetFilesToParse
    {
        FolderBrowserDialog FBD;
        RichTextBox RTB;
        public getFilesFromFolder(FolderBrowserDialog FBD , RichTextBox RTB)
        {
            this.FBD = FBD;
            this.RTB = RTB;
        }
        List<string> IgetFilesToParse.getFiles()
        {
            if (System.IO.Directory.Exists(ConfigurationManger.getCMInstance().getConfig().LastFolderDialogPath))
                FBD.SelectedPath = ConfigurationManger.getCMInstance().getConfig().LastFolderDialogPath;            
                if (FBD.ShowDialog() == DialogResult.OK)
                {
                    var folderPath = FBD.SelectedPath;
                    ConfigurationManger.getCMInstance().getConfig().LastFolderDialogPath = folderPath;
                    //writeConfigFile(FolderRootStart + "=" + folderBrowserDialog1.SelectedPath);

                    RTB.AppendText("folder found! moving to parse it");
                    RTB.BackColor = System.Drawing.Color.Green;
                    var files = System.IO.Directory.GetFiles(folderPath).ToList();
                    return files;
                }
                else
                {
                    RTB.BackColor = System.Drawing.Color.Red;
                    RTB.AppendText("bad folder :( choose another one\n");
                    return new List<string>();
                }
        }
    }


}
