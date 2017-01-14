using System;
using System.Runtime.Serialization;

namespace ChatLibrary
{
    [DataContract(Namespace = "http://videogamelab.co.uk/ChatLibrary")]
    [KnownType(typeof(Status))]
    [KnownType(typeof(Gender))]
    public class Member : NotifycationObject, IEquatable<Member>
    {
        public static readonly Member Empty = null;

        public enum Status
        {
            [EnumMember]
            Active,     // Green

            [EnumMember]
            Away,       // Yellow

            [EnumMember]
            Busy        // Red
        }

        public enum Gender
        {
            [EnumMember]
            Male,

            [EnumMember]
            Female
        }

        [DataMember]
        public String Username
        {
            get { return Get(() => Username); }
            set { Set(() => Username, value); }
        }

        [DataMember]
        public Gender Orientation
        {
            get { return Get(() => Orientation); }
            set { Set(() => Orientation, value); }
        }

        [DataMember]
        public DateTime Birthday
        {
            get { return Get(() => Birthday); }
            set { Set(() => Birthday, value); }
        }

        [DataMember]
        public Status Available
        {
            get { return Get(() => Available); }
            set { Set(() => Available, value); }
        }

        [DataMember]
        public Boolean Admin
        {
            get { return Get(() => Admin); }
            set { Set(() => Admin, value); }
        }

        public Member() 
            : this (String.Empty, Gender.Male,
                    DateTime.Today, Status.Active, false) { }

        public Member(Member other)
            : this (other.Username, other.Orientation,
                    other.Birthday, other.Available, other.Admin) { }

        public Member(String username, Gender orientation,
            DateTime birthday, Status available, Boolean admin) : base()
        {
            Username = username;
            
            Orientation = orientation;

            Birthday = birthday;

            Available = available;

            Admin = admin;
        }

        public override bool Equals(object other)
        {
            return (this as IEquatable<Member>).Equals(other as Member);
        }

        public override int GetHashCode()
        {
            return Username.GetHashCode();
        }

        bool IEquatable<Member>.Equals(Member other)
        {
            return other != null && Username == other.Username;
        }
    }
}
