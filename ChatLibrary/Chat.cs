using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ChatLibrary
{
    [DataContract(Namespace = "http://videogamelab.co.uk/ChatLibrary")]
    [KnownType(typeof(GlobalChat))]
    [KnownType(typeof(GroupChat))]
    [KnownType(typeof(PrivateChat))]
    [KnownType(typeof(Status))]
    [KnownType(typeof(Member))]
    public class Chat : NotifycationObject, IEquatable<Chat>
    {
        public static readonly Chat Empty = null;

        public static readonly Int32 MEMBER_MAX_GROUPS = 3;

        public enum Status
        {
            [EnumMember]
            Online,

            [EnumMember]
            Offline
        }

        [DataMember]
        public String Name
        {
            get { return Get(() => Name); }
            set { Set(() => Name, value); }
        }

        [DataMember]
        public Member Admin
        {
            get { return Get(() => Admin); }
            set { Set(() => Admin, value); }
        }

        [DataMember]
        public Status Active
        {
            get { return Get(() => Active); }
            set { Set(() => Active, value); }
        }

        [DataMember]
        public ObservableCollection<Message> MessageList { get; set; }

        public Chat() 
            : this (String.Empty, Member.Empty, Status.Online, null)
        { }

        public Chat(Chat other)
            : this (other.Name, other.Admin, other.Active, null)
        { }

        public Chat(String name, Member admin, Status active, ObservableCollection<Message> messageList)
        {
            Name = name;

            Admin = admin;

            Active = active;

            if (messageList != null)
            {
                MessageList = messageList;
            }
            else
            {
                MessageList = new ObservableCollection<Message>();
            }
        }

        public override bool Equals(object other)
        {
            return (this as IEquatable<Chat>).Equals(other as Chat);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        bool IEquatable<Chat>.Equals(Chat other)
        {
            return other != null && Name == other.Name;
        }

        public virtual Chat Clone()
        {
            return new Chat(this);
        }
    }
}
