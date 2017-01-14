using System;
using System.Runtime.Serialization;

namespace ChatLibrary
{
    [DataContract(Namespace = "http://videogamelab.co.uk/ChatLibrary")]
    [KnownType(typeof(Alignment))]
    public class Message : NotifycationObject, IEquatable<Message>
    {
        public static readonly Message Empty = null;
        
        public enum Alignment
        {
            [EnumMember]
            Left,

            [EnumMember]
            Centre,

            [EnumMember]
            Right,
        }

        [DataMember]
        public String Text
        {
            get { return Get(() => Text); }
            set { Set(() => Text, value); }
        }

        [DataMember]
        public DateTime Deliver
        {
            get { return Get(() => Deliver); }
            set { Set(() => Deliver, value); }
        }

        [DataMember]
        public Alignment Position
        {
            get { return Get(() => Position); }
            set { Set(() => Position, value); }
        }

        public Message() 
            : this(String.Empty, DateTime.Now, Alignment.Centre)
        { }

        public Message(Message other)
            : this (other.Text, other.Deliver, other.Position)
        { }

        public Message(String text, DateTime deliver, Alignment position)
        {
            Text = text;

            Deliver = deliver;

            Position = position;
        }

        public override bool Equals(object other)
        {
            return (this as IEquatable<Message>).Equals(other as Message);
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode() ^ Deliver.GetHashCode();
        }

        bool IEquatable<Message>.Equals(Message other)
        {
            return other != null && Text == other.Text && Deliver == other.Deliver;
        }
    }
}
