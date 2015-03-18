using System;
using System.Collections.Generic;

namespace NShapeCreator.Message
{

    public enum MessageID
    {
        GDIShapeUpdated,
        SaveFiles,
        FormClosed,
        LogError,
        LoadFromGDIFile,
        SelectedPathIndex
    }

    public class MessageEventArgs : EventArgs
    {
        private Dictionary<MessageID, Object> _messages;
        public Dictionary<MessageID, Object> Messages
        {
            get
            {
                if (_messages == null)
                {
                    _messages = new Dictionary<MessageID, Object>();
                }
                return _messages;
            }
        }
    }

    public abstract class Messenger
    {
        public event EventHandler<MessageEventArgs> MessageSubscription;
        protected virtual void SendMessage(MessageEventArgs e)
        {
            if (MessageSubscription != null)
            {
                MessageSubscription(this, e);
            }
        }

        public void SendErrorMessage(string error)
        {
            MessageEventArgs e = new MessageEventArgs();
            e.Messages[MessageID.LogError] = error;
            SendMessage(e);
        }
    }

}