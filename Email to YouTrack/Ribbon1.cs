using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outlook = Microsoft.Office.Interop.Outlook;
using Microsoft.Office.Tools.Ribbon;
using System.Windows.Forms;

namespace Email_to_YouTrack
{

    public partial class Ribbon1
    {
        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private async void Button1_Click(object sender, RibbonControlEventArgs e)
        {
            var explorer = Globals.ThisAddIn.Application.ActiveExplorer();
            var selectedFolder = explorer.CurrentFolder;
            var itemMessage = "";
            try
            {
                if (explorer.Selection.Count > 0)
                {
                    Object selObject = explorer.Selection[1];
                    if (selObject is Outlook.MailItem)
                    {
                        Outlook.MailItem mailItem =
                            (selObject as Outlook.MailItem);
                        await Globals.ThisAddIn.FromMail(mailItem);
                    }
                    else if (selObject is Outlook.ContactItem)
                    {
                        Outlook.ContactItem contactItem =
                            (selObject as Outlook.ContactItem);
                        itemMessage = "The item is a contact." +
                            " The full name is " + contactItem.Subject + ".";
                    }
                    else if (selObject is Outlook.AppointmentItem)
                    {
                        Outlook.AppointmentItem apptItem =
                            (selObject as Outlook.AppointmentItem);
                        itemMessage = "The item is an appointment." +
                            " The subject is " + apptItem.Subject + ".";
                    }
                    else if (selObject is Outlook.TaskItem)
                    {
                        Outlook.TaskItem taskItem =
                            (selObject as Outlook.TaskItem);
                        itemMessage = "The item is a task. The body is "
                            + taskItem.Body + ".";
                    }
                    else if (selObject is Outlook.MeetingItem)
                    {
                        Outlook.MeetingItem meetingItem =
                            (selObject as Outlook.MeetingItem);
                        itemMessage = "The item is a meeting item. " +
                             "The subject is " + meetingItem.Subject + ".";
                    }
                }
                if (!string.IsNullOrEmpty(itemMessage))
                {
                    MessageBox.Show(itemMessage);
                }
            }
            catch (Exception ex) //when (!Env.Debugging)
            {
                MessageBox.Show("An error occured sending to YouTrack.\n" + ex.ToString(), "Email to YouTrack error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Button2_Click(object sender, RibbonControlEventArgs e)
        {
            var form = new SettingsForm();
            form.Show();

        }
    }
    public static class Env
    {
#if DEBUG
        public static readonly bool Debugging = true;
#else
        public static readonly bool Debugging = false;
#endif
    }
}
