using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ChatLibrary
{
    [DataContract(Namespace = "http://videogamelab.co.uk/ChatLibrary")]
    public class GroupChat : Chat
    {
        public static readonly Int32 GROUP_MIN_SIZE = 3;
        public static readonly Int32 GROUP_MAX_SIZE = 25;

        [DataMember]
        public ObservableCollection<Member> MemberList { get; set; }

        [DataMember]
        public Int32 MinAge
        {
            get { return Get(() => MinAge); }
            set { Set(() => MinAge, value); }
        }

        [DataMember]
        public Int32 MaxSize
        {
            get { return Get(() => MaxSize); }
            set { Set(() => MaxSize, value); }
        }

        public GroupChat() 
            : this (String.Empty, Member.Empty, Status.Online, 0, 0, null, null)
        { }
        
        public GroupChat(GroupChat other)
            : this (other.Name, other.Admin, other.Active,
                    other.MinAge, other.MaxSize,
                    other.MemberList, null)
        { }

        public GroupChat(String name, Member admin, Status active,
            Int32 minAge, Int32 maxSize, 
            ObservableCollection<Member> memberList,
            ObservableCollection<Message> messageList)
            : base (name, admin, active, messageList)
        {
            MinAge = minAge;

            MaxSize = maxSize;

            if (memberList != null)
            {
                MemberList = memberList;
            }
            else
            {
                MemberList = new ObservableCollection<Member>();
            }
        }

        public bool AddMember(Member member)
        {
            if (MemberList.Count >= MaxSize)
            {
                return false;
            }

            if (DateTime.Today.Year - member.Birthday.Year < MinAge)
            {
                return false;
            }

            if (MemberList.Contains(member))
            {
                return false;
            }

            MemberList.Add(member);

            return true;
        }

        public bool HasMember(Member member)
        {
            return MemberList.ToList().Exists(m => m.Equals(member));
        }

        public Member FindMember(String username)
        {
            return MemberList.ToList().Find(m => m.Username.Equals(username));
        }

        public override Chat Clone()
        {
            return new GroupChat(this);
        }
    }
}
