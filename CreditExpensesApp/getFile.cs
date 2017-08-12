using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CreditExpensesApp
{
    class getFile : IgetFilesToParse
    {
        RichTextBox RTB;
        private OpenFileDialog FD;
        public getFile(OpenFileDialog FD , RichTextBox RTB)
        {
            this.FD = FD;
            this.RTB = RTB;
        }

        public List<string> getFiles()
        {
            if (System.IO.Directory.Exists(ConfigurationManger.getCMInstance().getConfig().LastFolderDialogPath))
                    FD.InitialDirectory = ConfigurationManger.getCMInstance().getConfig().LastFolderDialogPath;            
            var res = FD.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                ConfigurationManger.getCMInstance().getConfig().LastFolderDialogPath = System.IO.Directory.GetParent(FD.FileName).ToString();
                List<string> retlist = new List<string>();
                var file = FD.FileName;
                retlist.Add(file);
                return retlist;
            }
            else
            {
                RTB.BackColor = System.Drawing.Color.Red;
                RTB.AppendText("bad file :( choose another one\n");
                return new List<string>();

            }
            //throw new NotImplementedException();
        }
    }
}
