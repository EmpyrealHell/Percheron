using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Percheron.Interfaces.Chat
{
    public struct IrcMessage
    {
        private static readonly Regex messageParser = new Regex(@"^(?:(?<Tags>@(?:(?:[^ ]+\/)?[a-zA-Z0-9\-]+=[^\0; \r\n]*)(?:;(?:[^ ]+\/)?[a-zA-Z0-9\-]+=[^\0; \r\n]*)*) +)?(?:(?:\:(?<ServerNick>[^ !@]+))(?:\!(?<User>[^ @]+))?(?:@(?<Host>[^ ]+))? +)?(?<Command>(?:[a-zA-Z]+)|(?:[0-9]{3}))(?<Params>(?: +[^ \0\r\n\:][^ \0\r\n]*)*(?: +\:[^\0\r\n]*)?)$");
        private static readonly Regex firstParamParser = new Regex(@"");


        public string ServerNick { get; private set; }
        public string User { get; private set; }
        public string Host { get; private set; }
        public string Command { get; private set; }
        public IEnumerable<string> Middles { get; private set; }
        public string Trailing { get; private set; }
        public string Params
        {
            get
            {
                var middles = string.Join(" ", this.Middles);
                if (middles.Length > 0)
                    middles = " " + middles;
                return  middles + (string.IsNullOrEmpty(this.Trailing) ? "" : " :" + this.Trailing);
            }
        }
        public IDictionary<string, string> Tags { get; private set; }
        public string TagList
        {
            get
            {
                if (this.Tags != null && this.Tags.Count() > 0)
                    return "@" + string.Join(";", this.Tags.Select(x => x.Key + "=" + x.Value)) + " ";
                return "";
            }
        }

        public IrcMessage(string rawText)
        {
            var match = messageParser.Match(rawText);
            if (!match.Success)
            {
                throw new ArgumentException("Specified text is not a valid IRC command: " + rawText);
            }
            this.ServerNick = match.Groups["ServerNick"]?.Value;
            this.User = match.Groups["User"]?.Value;
            this.Host = match.Groups["Host"]?.Value;
            this.Command = match.Groups["Command"]?.Value;
            var messageParams = match.Groups["Params"]?.Value;
            if (!string.IsNullOrWhiteSpace(messageParams))
            {
                var trailingStart = messageParams.IndexOf(':');
                if (trailingStart >= 0)
                {
                    var middles = messageParams.Substring(0, trailingStart).Trim();
                    if (!string.IsNullOrWhiteSpace(middles))
                    {
                        this.Middles = middles.Split(' ');
                    }
                    else
                    {
                        this.Middles = new string[0];
                    }
                    this.Trailing = messageParams.Substring(trailingStart + 1);
                }
                else
                {
                    this.Middles = messageParams.Trim().Split(' ');
                    this.Trailing = null;
                }
            }
            else
            {
                this.Middles = null;
                this.Trailing = null;
            }
            var tagList = match.Groups["Tags"]?.Value;
            if (!string.IsNullOrWhiteSpace(tagList))
            {
                tagList = tagList.Trim().Substring(1);
                this.Tags = tagList.Split(';').Select(x => x.Split('=')).ToDictionary(key => key[0], value => value[1]);
            }
            else
            {
                this.Tags = null;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.TagList);
            if (!string.IsNullOrEmpty(this.ServerNick))
            {
                sb.Append(":" + this.ServerNick);
            }
            if (!string.IsNullOrEmpty(this.User))
            {
                sb.Append("!" + this.User);
            }
            if (!string.IsNullOrEmpty(this.Host))
            {
                sb.Append("@" + this.Host);
            }
            if (sb.Length > 0)
            {
                sb.Append(" ");
            }
            sb.Append(this.Command);
            sb.Append(this.Params);
            return sb.ToString();
        }
    }
}
