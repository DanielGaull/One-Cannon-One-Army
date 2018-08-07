using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OneCannonOneArmy
{
    public static class Error
    {
        public static event System.Action OkClicked;

        public static void Handle(Exception error)
        {
            Sound.PlaySounds = false;
            if (MessageBox.Show(ErrorMessage(error), "There's been an error.", MessageBoxButtons.OK, MessageBoxIcon.Error) ==
                (DialogResult.OK | DialogResult.None))
            {
                OkClicked?.Invoke();
            }
        }

        private static string ErrorMessage(Exception e)
        {
            return string.Format("Message: {0}\nData: {1}\nSource: {2}\nTarget Site: {3}\n\nStack Trace: {4}\n\n" + 
                "Please contact your provider with this information.", e.Message, e.Data, e.Source, e.TargetSite, 
                e.StackTrace);
        }
    }
}
