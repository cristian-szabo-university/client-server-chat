using System.Linq;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System;

namespace ChatLibrary
{
    [DataContract(Namespace = "http://videogamelab.co.uk/ChatLibrary")]
    public class GlobalChat : Chat
    {
        [DataMember]
        public ObservableCollection<Member> MemberList { get; set; }

        public GlobalChat()
            : this ("GlobalChat", Member.Empty, Status.Online, null, null)
        { }
        
        public GlobalChat(GlobalChat other)
            : this (other.Name, other.Admin, other.Active,
                    other.MemberList, null)
        { }

        public GlobalChat(String name, Member admin, Status active,
            ObservableCollection<Member> memberList,
            ObservableCollection<Message> messageList)
            : base (name, admin, active, messageList)
        {
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
            if (MemberList.Contains(member))
            {
                return false;
            }

            MemberList.Add(member);

            return true;
        }

        public Member FindMember(string username)
        {
            return MemberList.ToList().Find(m => m.Username.Equals(username));
        }
        
        public override Chat Clone()
        {
            return new GlobalChat(this);
        }
    }
}
