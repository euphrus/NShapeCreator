using System;
using System.Windows.Forms;
using NShapeCreator.Message;

namespace NShapeCreator.Message
{
    public class MessageForm : Form
    {
        public event EventHandler<MessageEventArgs> Messages;

        protected void SendMessage(MessageEventArgs e)
        {
            if (Messages != null)
            {
                Messages(this, e);
            }
        }

        public void SendErrorMessage(string error)
        {
            MessageEventArgs e = new MessageEventArgs();
            e.Messages[MessageID.LogError] = error;
            SendMessage(e);
        }

        public virtual void HandleMessage(Object sender, MessageEventArgs e)
        {
        }
    }
}