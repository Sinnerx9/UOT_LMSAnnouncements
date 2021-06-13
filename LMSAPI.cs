using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;
using Newtonsoft.Json;
namespace LMSAnnouncements
{
    class LMSAPI
    {
        private string COURSES_API { get; set; } = "https://lms.uotechnology.edu.iq/api/v1/users/self/favorites/courses?include[]=term&exclude[]=enrollments&sort=nickname";
        private string ANNOUNCEMENTS_API { get; set; } = "https://lms.uotechnology.edu.iq/api/v1/courses/";
        private string LOGIN_API { get; set; } = "https://lms.uotechnology.edu.iq/login/canvas";
        private string INIT_API { get; set; } = "https://lms.uotechnology.edu.iq/login/canvas";
        private CookieContainer cookies { get; set; } = new CookieContainer();
        private string authenticity_token { get; set; } = "";
        public LMSAPI()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.INIT_API);
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36 Edg/90.0.818.66";
            request.CookieContainer = this.cookies;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var src = new StreamReader(response.GetResponseStream()).ReadToEnd();
            this.authenticity_token = Regex.Match(src, "name=\"authenticity_token\".*?value=\"(.*?)\"").Groups[1].Value;
            if (String.IsNullOrWhiteSpace(this.authenticity_token))
            {
                throw new Exception("Couldn't Find Authenticity Token");
            }
        }
        public bool Login(string email, string password)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string postdata = ($"utf8=%E2%9C%93&authenticity_token={HttpUtility.UrlEncode(this.authenticity_token)}&redirect_to_ssl=1&pseudonym_session%5Bunique_id%5D={email.Replace("@", "%40")}&pseudonym_session%5Bpassword%5D={password}&pseudonym_session%5Bremember_me%5D=0&pseudonym_session%5Bremember_me%5D=1");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.LOGIN_API);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36 Edg/90.0.818.66";
                request.CookieContainer = this.cookies;
                var stream = request.GetRequestStream();
                byte[] payload = Encoding.UTF8.GetBytes(postdata);
                request.ContentLength = payload.Length;
                stream.Write(payload, 0, payload.Length);
                stream.Flush();
                stream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return response.ResponseUri.Query.ToString().Contains("?login_success=1");
            }
            catch
            {
                return false;
            }
        }

        public Course[] FetchCourse()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.COURSES_API);
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36 Edg/90.0.818.66";
            request.CookieContainer = this.cookies;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var src = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return JsonConvert.DeserializeObject<Course[]>(src);
        }

        public Announcement[] FetchAnnouncements(int id)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{this.ANNOUNCEMENTS_API}{id}/discussion_topics?only_announcements=true&include[]=sections_user_count&include[]=sections");
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36 Edg/90.0.818.66";
            request.CookieContainer = this.cookies;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var src = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return JsonConvert.DeserializeObject<Announcement[]>(src);
        }

    }
}
