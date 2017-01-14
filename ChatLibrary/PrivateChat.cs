using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ChatLibrary
{
    [DataContract(Namespace = "http://videogamelab.co.uk/ChatLibrary")]
    public class PrivateChat : Chat
    {
        [DataMember]
        public Member Person
        {
            get { return Get(() => Person); }
            set { Set(() => Person, value); }
        }

        public PrivateChat() 
            : this(String.Empty, Member.Empty, Status.Online, Member.Empty, null)
        { }

        public PrivateChat(PrivateChat other)
            : this (other.Name, other.Admin, other.Active, other.Person, null)
        { }

        public PrivateChat(String name, Member admin, Status active, Member person,
            ObservableCollection<Message> messageList)
            : base(name, admin, active, messageList)
        {
            Person = person;
        }

        public override Chat Clone()
        {
            return new PrivateChat(this);
        }
    }
}
