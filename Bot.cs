using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LMSAnnouncements
{
    class Bot
    {
        private LMSAPI p_api { get; set; }
        private string p_token { get; set; }
        private long p_userid { get; set; }
        private bool p_debugmode { get; set; } = false;
        private long p_debuguserid { get; set; } = 0;
        public int CheckTime = 60000;
        private List<int> temp { get; set; }
        private DateTime dealy { get; set; }
        public Bot(LMSAPI api, string token, long userid, bool debugmode = false, long debug_userid = 0)
        {
            this.p_api = api;
            this.p_token = token;
            this.p_userid = userid;
            this.p_debugmode = debugmode;
            this.p_debuguserid = debug_userid;
            this.temp = new List<int>();
        }
        public void Start()
        {
            this.dealy = DateTime.Now;
            if (this.p_debugmode && this.p_debuguserid != 0)
            {
                SendMessage(this.p_token, this.p_debuguserid, "Bot Started :D");
                Console.WriteLine("Bot Started :D");
            }
            new Task(() => Reset()).Start();
            new Task(() =>
            {
                var courses = p_api.FetchCourse();
                while (true)
                {
                    for (int i = 0; i < courses.Length; i++)
                    {
                        try
                        {
                            var anno = p_api.FetchAnnouncements(courses[i].id);
                            var tmessage = "";
                            for (int j = 0; j < anno.Length; j++)
                            {

                                if (anno[j].posted_at.ToString("M/d/yyyy") == DateTime.Today.ToString("M/d/yyyy"))
                                {
                                    if (this.temp.Contains(anno[j].id))
                                        continue;
                                    tmessage += ($"Title : {courses[i].name} - {anno[j].title}{Environment.NewLine}");
                                    tmessage += ($"Dr.{anno[j].user_name} Says : {Environment.NewLine}");
                                    tmessage += ($"{ExtractText(anno[j].message).Replace("&nbsp;", "")}{Environment.NewLine}");
                                    tmessage += ($"Message Sent At : {anno[j].posted_at}{Environment.NewLine}");
                                    this.temp.Add(anno[j].id);
                                }

                            }
                            if (!String.IsNullOrWhiteSpace(tmessage))
                            {
                                SendMessage(this.p_token, this.p_userid, tmessage);
                            }


                        }
                        catch (Exception ex)
                        {
                            if (this.p_debugmode && this.p_debuguserid != 0)
                            {
                                SendMessage(this.p_token, this.p_debuguserid, ex.Message);
                            }
                        }
                    }
                    if (this.p_debugmode && this.p_debuguserid != 0)
                    {
                        Console.WriteLine("Check For New Announcement Completed...");
                        SendMessage(this.p_token, this.p_debuguserid, "Check For New Announcement Completed...");
                    }
                    Thread.Sleep(this.CheckTime);
                }
            }).Start(); ;

        }

        private void Reset()
        {
            while (true)
            {
                this.temp.Clear();

                var interval = (int)(DateTime.Now.AddDays(1) - this.dealy).TotalMilliseconds;
                if (this.p_debugmode && this.p_debuguserid != 0)
                {
                    Console.WriteLine($"Anno List Cleared...{Environment.NewLine}Next Clear in : {(TimeSpan.FromMilliseconds(interval)).ToString(@"d\:hh\:mm\:ss")}");
                    SendMessage(this.p_token, this.p_debuguserid, $"Anno List Cleared...{Environment.NewLine}Next Clear in : {(TimeSpan.FromMilliseconds(interval)).ToString(@"d\:hh\:mm\:ss")}");
                }
                Thread.Sleep(interval);
                this.dealy = DateTime.Now;
            }

        }
        public string ExtractText(string html)
        {
            if (html == null)
            {
                throw new ArgumentNullException("html");
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var chunks = new List<string>();

            foreach (var item in doc.DocumentNode.DescendantNodesAndSelf())
            {
                if (item.NodeType == HtmlNodeType.Text)
                {
                    if (item.InnerText.Trim() != "")
                    {
                        chunks.Add(item.InnerText.Trim());
                        HtmlAttribute att = item.Attributes["href"];
                        if (att != null)
                            chunks.Add(att.Value);
                    }
                }
            }
            return String.Join(" ", chunks);
        }

        private void SendMessage(string token, long userid, string message)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            new WebClient().DownloadString($"https://api.telegram.org/bot{token}/sendmessage?chat_id={userid}&text={message}");
        }
    }
}
