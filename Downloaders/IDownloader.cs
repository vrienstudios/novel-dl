using KobeiD.Models;
using MSHTML;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace KobeiD.Downloaders
{
    public class IDownloader
    {
        public WebClient webClient;
        public MSHTML.IHTMLDocument2 page;

        public System.Collections.IEnumerator pageEnumerator;

        public MetaData mdata;
        public Uri url;

        public IDownloader(string url)
        {
            this.url = new Uri(url);
            webClient = new WebClient();
            GenHeaders();
            string html = webClient.DownloadString(url);
            LoadPage(html);
            html = null;
        }

        public void GenHeaders()
        {
            webClient.Headers.Add("accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            webClient.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 10.6; Windows NT 6.1; Trident/5.0; InfoPath.2; SLCC1; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 2.0.50727) 3gpp-gba UNTRUSTED/1.0");
            webClient.Headers.Add("referer", url.Host);
            webClient.Headers.Add("DNT", "1");
            webClient.Headers.Add("Upgrade-Insecure-Requests", "1");
        }

        public static WebHeaderCollection GenHeaders(string url)
        {
            WebHeaderCollection Headers = new WebHeaderCollection();
            Headers.Add("accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 10.6; Windows NT 6.1; Trident/5.0; InfoPath.2; SLCC1; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 2.0.50727) 3gpp-gba UNTRUSTED/1.0");
            Headers.Add("referer", url);
            Headers.Add("DNT", "1");
            Headers.Add("Upgrade-Insecure-Requests", "1");
            return Headers;
        }

        public void MovePage(string url)
        {
            webClient.Headers.Clear();
            GenHeaders();
            LoadPage(webClient.DownloadString(url));
        }

        private void LoadPage(string html)
        {
            page = (MSHTML.IHTMLDocument2)new HTMLDocument();
            page.designMode = "On";
            page.write(html);
            page.close();
            pageEnumerator = page.all.GetEnumerator();
            GC.Collect();
        }
    }
}
