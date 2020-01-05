using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percheron.API.Model
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ExpiresIn { get; set; }
        public IEnumerable<string> Scope { get; set; }
        public string TokenType { get; set; }
    }
}
