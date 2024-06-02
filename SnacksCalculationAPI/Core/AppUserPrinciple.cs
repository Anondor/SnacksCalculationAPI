using System.Reflection;
using System.Security.Principal;

namespace SnacksCalculationAPI.Core
{
    public class AppUserPrinciple
    {
        public AppUserPrinciple(string userName)
        {
            Identity = new GenericIdentity(userName);
            Name = userName;
        }
        public AppUserPrinciple(string userName, string type)
        {
            Identity = new GenericIdentity(userName, type);
            Name = userName;
        }
        public IIdentity Identity { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
       
        public string Email { get; set; }
        public string Avatar { get; set; }
        public int ActiveRoleId { get; set; }
        public string ActiveRoleName { get; set; }
       

        public List<int> RoleIdList { get; set; } = new List<int>();
        public string RoleIds
        {
            get => string.Join(",", RoleIdList.Select(s => (int)s));
            set
            {
                RoleIdList = !string.IsNullOrWhiteSpace(value) && value.Contains(",")
                    ? value.Split(',').Select(s => (int)(Convert.ToInt32(s.Trim()))).ToList()
                    : !string.IsNullOrWhiteSpace(value) && !value.Contains(",")
                        ? new List<int> { (int)(Convert.ToInt32(value)) }
                    : new List<int> { };
            }
        }

        public string UserAgentInfo { get; set; }
        public int NodeId { get; set; }

        public Dictionary<string, string> GetByName()
        {
            var result = new Dictionary<string, string>
            {
                {nameof(Id),Id.ToString() },
                {nameof(Name),Name??"" },
                {nameof(Phone),Phone??"" },
                {nameof(Email),Email??"" },
                {nameof(ActiveRoleId),ActiveRoleId.ToString() },
                {nameof(RoleIds),RoleIds??"" },
                {nameof(UserAgentInfo),UserAgentInfo??"" },
                {nameof(NodeId),NodeId.ToString()},
                {nameof(ActiveRoleName),ActiveRoleName??""},

            };
            return result;
        }

    }
}
