using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percheron.API.Model
{
    public class User
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }
        public string BroadcasterType { get; set; }
        public string Description { get; set; }
        public string ProfileImageUrl { get; set; }
        public string OfflineImageUrl { get; set; }
        public int ViewCount { get; set; }
        public string Email { get; set; }
    }

    public class UserResponse
    {
        public List<User> data;
    }
}
