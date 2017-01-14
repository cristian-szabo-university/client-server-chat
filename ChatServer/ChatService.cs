using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using MySql.Data.MySqlClient;
using ChatLibrary;

namespace ChatServer
{
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple, 
        InstanceContextMode = InstanceContextMode.Single)]
    internal class ChatService : IChatContract
    {
        private Database _Database;

        private Dictionary<String, IChatCallback> _ClientMap;

        private GlobalChat _GlobalChat;

        private List<GroupChat> _GroupChatList;

        private List<PrivateChat> _PrivateChatList;

        public ChatService(Database db)
        {
            _Database = db;

            _ClientMap = new Dictionary<String, IChatCallback>();

            _GroupChatList = new List<GroupChat>();

            _PrivateChatList = new List<PrivateChat>();
            
            _GlobalChat = new GlobalChat()
            {
                Admin = new Member()
                {
                    Username = "Administrator"
                }
            };
        }

        public Member Connect()
        {
            Member result = _GlobalChat.FindMember(CurrentUser);

            if (result != null)
            {
                throw new FaultException("Member has already been connected.", new FaultCode("Client"));
            }

            result = new Member();
            result.Username = CurrentUser;
             
            using (MySqlCommand cmd = _Database.CreateCommand("SELECT Birthday, Gender, Admin FROM Members WHERE Username=@username;"))
            {
                cmd.Parameters.AddWithValue("@username", CurrentUser);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Member.Gender g;
                        if (Enum.TryParse<Member.Gender>(reader.GetString("Gender"), out g))
                        {
                            result.Orientation = g;
                        }
                       
                        result.Birthday = reader.GetDateTime("Birthday");

                        result.Admin = reader.GetBoolean("Admin");
                    }
                }
            }

            Message msg = new Message()
            {
                Text = "Member " + result.Username + " has joined the chat!",
                Deliver = DateTime.Now,
                Position = Message.Alignment.Centre
            };

            if (_GlobalChat.AddMember(result))
            {
                _ClientMap.Add(CurrentUser, CurrentCallback);

                foreach (Member client in _GlobalChat.MemberList)
                {
                    _ClientMap[client.Username].NotifyMemberJoinChat(_GlobalChat.Name, result);

                    _ClientMap[client.Username].NotifyMessageChat(_GlobalChat.Name, msg);
                }
            }

            return result;
        }

        public void Disconnect()
        {
            if (!_ClientMap.ContainsKey(CurrentUser))
            {
                return;
            }

            Member result = _GlobalChat.FindMember(CurrentUser);

            if (result == null)
            {
                return;
            }

            _GlobalChat.MemberList.Remove(result);

            Message msg = new Message()
            {
                Text = "Member " + result.Username + " has left the chat!",
                Deliver = DateTime.Now,
                Position = Message.Alignment.Centre
            };

            foreach (Member client in _GlobalChat.MemberList)
            {
                _ClientMap[client.Username].NotifyMemberLeaveChat(_GlobalChat.Name, result);

                _ClientMap[client.Username].NotifyMessageChat(_GlobalChat.Name, msg);
            }

            for (int i = _GroupChatList.Count - 1; i >= 0; i--)
            {
                GroupChat chat = _GroupChatList[i];

                if (!chat.HasMember(result))
                {
                    continue;
                }

                chat.MemberList.Remove(result);

                foreach (Member client in chat.MemberList)
                {
                    _ClientMap[client.Username].NotifyMemberLeaveChat(chat.Name, result);

                    _ClientMap[client.Username].NotifyMessageChat(chat.Name, msg);
                }

                if (chat.Admin.Equals(result))
                {
                    _GroupChatList.RemoveAt(i);
                }
            }

            for (int i = _PrivateChatList.Count - 1; i >= 0; i--)
            {
                PrivateChat chat = _PrivateChatList[i];

                if (!chat.Admin.Equals(result) && !chat.Person.Equals(result))
                {
                    continue;
                }

                if (chat.Person.Equals(result))
                {
                    _ClientMap[chat.Admin.Username].NotifyMemberLeaveChat(chat.Name, result);

                    _ClientMap[chat.Admin.Username].NotifyMessageChat(chat.Name, msg);
                }
                else if (chat.Admin.Equals(result))
                {
                    _ClientMap[chat.Person.Username].NotifyMemberLeaveChat(chat.Name, result);

                    _ClientMap[chat.Person.Username].NotifyMessageChat(chat.Name, msg);
                }

                _PrivateChatList.RemoveAt(i);
            }

            _ClientMap.Remove(CurrentUser);
        }

        public void UpdateMemberList()
        {
            if (!_ClientMap.ContainsKey(CurrentUser))
            {
                throw new FaultException("Invalid operation requested for the service.", new FaultCode("Client"));
            }

            foreach(Member client in _GlobalChat.MemberList)
            {
                _ClientMap[CurrentUser].NotifyMemberJoinChat(_GlobalChat.Name, client);
            }
        }

        public void UpdateGroupChatList()
        {
            if (!_ClientMap.ContainsKey(CurrentUser))
            {
                throw new FaultException("Invalid operation requested for the service.", new FaultCode("Client"));
            }

            foreach (GroupChat chat in _GroupChatList)
            {
                _ClientMap[CurrentUser].NotifyGroupChatOpen(chat);
            }
        }

        public void ChangeStatus(Member.Status available)
        {
            if (!_ClientMap.ContainsKey(CurrentUser))
            {
                return;
            }

            Member result = _GlobalChat.FindMember(CurrentUser);

            if (result == null)
            {
                return;
            }

            result.Available = available;

            foreach (Member client in _GlobalChat.MemberList)
            {
                _ClientMap[client.Username].NotifyMemberStatus(result.Username, available);
            }
        }

        public GroupChat CreateGroupChat(String chatName, Int32 minAge, Int32 maxSize)
        {
            if (!_ClientMap.ContainsKey(CurrentUser))
            {
                return null;
            }

            // Group name should be unique
            if (_GroupChatList.Exists(g => g.Name.Equals(chatName)))
            {
                return null;
            }

            Member result = _GlobalChat.FindMember(chatName);

            // Not allowed to create a group using a member's name
            if (result != null)
            {
                return null;
            }

            result = _GlobalChat.FindMember(CurrentUser);

            // Member who creates the group should be logged in
            if (result == null)
            {
                return null;
            }

            GroupChat chat = new GroupChat()
            {
                Name = chatName,
                Admin = result,
                MinAge = minAge,
                MaxSize = maxSize
            };

            chat.AddMember(chat.Admin);

            _GroupChatList.Add(chat);
            
            // Notify the rest of the members about this new group chat
            foreach (Member client in _GlobalChat.MemberList)
            {
                _ClientMap[client.Username].NotifyGroupChatOpen(chat);
            }

            return chat;
        }
        
        public void JoinGroupChat(String chatName)
        {
            if (!_ClientMap.ContainsKey(CurrentUser))
            {
                return;
            }

            GroupChat chat = _GroupChatList.Find(g => g.Name.Equals(chatName));

            if (chat == null)
            {
                return;
            }

            Member result = _GlobalChat.FindMember(CurrentUser);

            if (result == null)
            {
                return;
            }

            if (chat.HasMember(result))
            {
                return;
            }

            if (!_ClientMap[chat.Admin.Username].JoinGroupChatRequest(result))
            {
                return;
            }

            if (!chat.AddMember(result))
            {
                return;
            }

            Message msg = new Message()
            {
                Text = "Member " + result.Username + " has joined the chat!",
                Deliver = DateTime.Now,
                Position = Message.Alignment.Centre
            };

            // Notify the rest of the members of this group
            foreach (Member client in _GlobalChat.MemberList)
            {
                _ClientMap[client.Username].NotifyMemberJoinChat(chat.Name, result);

                if (chat.HasMember(client))
                {
                    _ClientMap[client.Username].NotifyMessageChat(chat.Name, msg);
                }
            }

            _ClientMap[result.Username].JoinGroupChatResponse(new GroupChat(chat));
        }

        public void KickOutMember(String memberName, String chatName)
        {
            if (!_ClientMap.ContainsKey(CurrentUser))
            {
                return;
            }

            Member result = _GlobalChat.FindMember(CurrentUser);

            if (result == null)
            {
                return;
            }

            Chat chat = null;

            if (_GlobalChat.Name.Equals(chatName))
            {
                chat = _GlobalChat;
            }
            else
            {
                chat = _GroupChatList.Find(g => g.Name.Equals(chatName));
            }

            if (chat == null)
            {
                return;
            }

            List<Member> memberList = null;

            if (_GlobalChat.Name.Equals(chatName))
            {
                GlobalChat globalChat = chat as GlobalChat;

                if (result.Admin != true)
                {
                    return;
                }

                memberList = globalChat.MemberList.ToList();
            }
            else
            {
                GroupChat groupChat = chat as GroupChat;

                if (!chat.Admin.Equals(result))
                {
                    return;
                }

                memberList = groupChat.MemberList.ToList();
            }

            Member other = null;

            if (_GlobalChat.Name.Equals(chatName))
            {
                GlobalChat globalChat = chat as GlobalChat;

                other = globalChat.FindMember(memberName);
            }
            else
            {
                GroupChat groupChat = chat as GroupChat;

                other = groupChat.FindMember(memberName);
            }

            if (other == null)
            {
                return;
            }

            Message msg = new Message()
            {
                Text = "Member " + other.Username + " was kicked out from chat!",
                Deliver = DateTime.Now,
                Position = Message.Alignment.Centre
            };

            // Notify the rest of the members of this group
            foreach (Member client in memberList)
            {
                _ClientMap[client.Username].NotifyMemberLeaveChat(chat.Name, other);

                if (memberList.Find(m => client.Username.Equals(m.Username)) != null)
                {
                    _ClientMap[client.Username].NotifyMessageChat(chat.Name, msg);
                }
            }

            if (!_GlobalChat.Name.Equals(chatName))
            {
                GroupChat groupChat = chat as GroupChat;

                groupChat.MemberList.Remove(other);
            }
        }

        public void LeaveGroupChat(String chatName)
        {
            if (!_ClientMap.ContainsKey(CurrentUser))
            {
                return;
            }

            GroupChat chat = _GroupChatList.Find(g => g.Name.Equals(chatName));

            if (chat == null)
            {
                return;
            }

            Member result = _GlobalChat.FindMember(CurrentUser);

            if (result == null)
            {
                return;
            }

            if (chat.Admin.Equals(result))
            {
                _GroupChatList.Remove(chat);

                foreach (Member client in _GlobalChat.MemberList)
                {
                    _ClientMap[client.Username].NotifyMemberLeaveChat(chat.Name, result);
                }
            }

            Message msg = new Message()
            {
                Text = "Member " + result.Username + " has left the chat!",
                Deliver = DateTime.Now,
                Position = Message.Alignment.Centre
            };

            foreach (Member client in _GlobalChat.MemberList)
            {
                _ClientMap[client.Username].NotifyMemberLeaveChat(chat.Name, result);

                if (chat.HasMember(client))
                {
                    _ClientMap[client.Username].NotifyMessageChat(chat.Name, msg);
                }
            }

            chat.MemberList.Remove(result);
        }

        public PrivateChat InvitePrivateChat(String personName)
        {
            if (!_ClientMap.ContainsKey(CurrentUser))
            {
                return null;
            }

            if (_PrivateChatList.Exists(g => g.Name.Equals(personName)))
            {
                return null;
            }

            Member result = _GlobalChat.FindMember(CurrentUser);

            if (result == null)
            {
                return null;
            }

            Member other = _GlobalChat.FindMember(personName);

            if (other == null)
            {
                return null;
            }

            if (!_ClientMap[other.Username].InvitePrivateChatRequest(result))
            {
                return null;
            }

            PrivateChat chat = new PrivateChat()
            {
                Name = result.Username + " & " + other.Username,
                Admin = result,
                Person = other
            };

            _PrivateChatList.Add(chat);

            _ClientMap[other.Username].InvitePrivateChatResponse(chat);

            return chat;
        }

        public void LeavePrivateChat(String chatName)
        {
            if (!_ClientMap.ContainsKey(CurrentUser))
            {
                return;
            }

            PrivateChat chat = _PrivateChatList.Find(g => g.Name.Equals(chatName));

            if (chat == null)
            {
                return;
            }

            Member result = _GlobalChat.FindMember(CurrentUser);

            if (result == null)
            {
                return;
            }

            _ClientMap[chat.Person.Username].NotifyMemberLeaveChat(chat.Name, result);
            _ClientMap[chat.Admin.Username].NotifyMemberLeaveChat(chat.Name, result);

            Message msg = new Message()
            {
                Text = "Member " + result.Username + " has left the chat!",
                Deliver = DateTime.Now,
                Position = Message.Alignment.Centre
            };

            if (chat.Person.Equals(result))
            {
                _ClientMap[chat.Admin.Username].NotifyMessageChat(chat.Name, msg);
            }
            else if (chat.Admin.Equals(result))
            {
                _ClientMap[chat.Person.Username].NotifyMessageChat(chat.Name, msg);
            }

            _PrivateChatList.Remove(chat);
        }

        public void MessageChat(string chatName, string text)
        {
            if (!_ClientMap.ContainsKey(CurrentUser))
            {
                return;
            }

            Member result = _GlobalChat.FindMember(CurrentUser);

            if (result == null)
            {
                return;
            }

            List<Member> memberList = new List<Member>();
            Chat chat = null;

            if (_GlobalChat.Name.Equals(chatName))
            {
                chat = _GlobalChat;
                memberList = _GlobalChat.MemberList.ToList();
            }
            else
            {
                chat = _GroupChatList.Find(g => g.Name.Equals(chatName));

                if (chat == null)
                {
                    chat = _PrivateChatList.Find(g => g.Name.Equals(chatName));
                }

                if (chat is GroupChat)
                {
                    GroupChat groupChat = chat as GroupChat;

                    memberList = groupChat.MemberList.ToList();
                }
                else if (chat is PrivateChat)
                {
                    PrivateChat privateChat = chat as PrivateChat;

                    memberList.Add(privateChat.Person);
                    memberList.Add(privateChat.Admin);
                }
            }

            if (chat == null)
            {
                return;
            }

            Message msg = new Message()
            {
                Text = result.Username + ": " + text,
                Deliver = DateTime.Now,
                Position = Message.Alignment.Left
            };

            chat.MessageList.Add(msg);

            foreach (Member client in memberList)
            {
                if (client.Equals(result) || client.Available == Member.Status.Busy)
                {
                    continue;
                }

                _ClientMap[client.Username].NotifyMessageChat(chat.Name, msg);
            }

            msg = new Message()
            {
                Text = "You: " + text,
                Deliver = DateTime.Now,
                Position = Message.Alignment.Right
            };

            _ClientMap[result.Username].NotifyMessageChat(chat.Name, msg);
        }

        public void UpdateMessageListFrom(DateTime deliverTime)
        {
            if (!_ClientMap.ContainsKey(CurrentUser))
            {
                return;
            }

            Member result = _GlobalChat.FindMember(CurrentUser);

            if (result == null)
            {
                return;
            }

            deliverTime = deliverTime.AddSeconds(10);

            foreach (Message msg in _GlobalChat.MessageList)
            {
                if (msg.Deliver > deliverTime)
                {
                    _ClientMap[result.Username].NotifyMessageChat(_GlobalChat.Name, msg);
                }
            }

            foreach (GroupChat chat in _GroupChatList)
            {
                if (!chat.HasMember(result))
                {
                    continue;
                }

                foreach (Message msg in chat.MessageList)
                {
                    if (msg.Deliver > deliverTime)
                    {
                        _ClientMap[result.Username].NotifyMessageChat(chat.Name, msg);
                    }
                }
            }

            foreach (PrivateChat chat in _PrivateChatList)
            {
                if (!chat.Admin.Equals(result) && !chat.Person.Equals(result))
                {
                    continue;
                }

                foreach (Message msg in chat.MessageList)
                {
                    if (msg.Deliver > deliverTime)
                    {
                        _ClientMap[result.Username].NotifyMessageChat(chat.Name, msg);
                    }
                }
            }
        }

        private IChatCallback CurrentCallback
        {
            get
            {
                return OperationContext.Current.GetCallbackChannel<IChatCallback>();
            }
        }

        private String CurrentUser
        {
            get
            {
                return ServiceSecurityContext.Current.PrimaryIdentity.Name;
            }
        }
    }
}
