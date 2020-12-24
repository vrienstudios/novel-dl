﻿using KobeiD.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KobeiD
{
    public enum MediaType
    {
        xhtml,
        image,
        ncx,
        css
    }

    public enum MetaType
    {
        dc,
        meta,
    }

    public static class shorts
    {
        public static Dictionary<string, string> RemoveList = new Dictionary<string, string>();

        public static string mediaTypes(MediaType mt)
        {
            switch (mt)
            {
                case MediaType.image:
                    return "image/jpeg";
                case MediaType.xhtml:
                    return "application/xhtml+xml";
                case MediaType.ncx:
                    return "application/x-dtbncx+xml";
                case MediaType.css:
                    return "text/css";
            }
            return null;
        }

        public static List<Item> ToItems(this List<Page> pages)
        {
            List<Item> items = new List<Item>();
            foreach (Page pg in pages)
                items.Add(new Item(pg.id, pg.hrefTo, MediaType.xhtml));
            return items;
        }

        /// <summary>
        /// Enumerates over all characters in the given string and replaces special chars, <, >, and & with escaped chars.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string MakeTextXHTMLReady(string text)
        {
            char[] chars = text.ToCharArray();
            StringBuilder sb = new StringBuilder();
            for(int idx = 0; idx < text.Length; idx++)
                switch(text[idx])
                {
                    case '<':
                        sb.Append("&lt;");
                        break;
                    case '>':
                        sb.Append("&gt;");
                        break;
                    case '&':
                        sb.Append("&amp;");
                        break;
                    default:
                        sb.Append(text[idx]);
                        continue;
                }
            return sb.ToString();
        }
    }

    class Epub
    {
        public string Title, author;
        public string workingDirectory, OEBPSDIR;
        public string mimeType = "application/epub+zip";
        public string METAINF = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><container version = \"1.0\" xmlns=\"urn:oasis:names:tc:opendocument:xmlns:container\"><rootfiles><rootfile full-path=\"OEBPS/content.opf\" media-type=\"application/oebps-package+xml\"/></rootfiles></container>";
        public string creditFactory = "<?xml version='1.0' encoding='utf-8'?><html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\"><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"/><meta name=\"calibre:cover\" content=\"false\"/><title>Tribute</title><style type=\"text/css\" title=\"override_css\">@page {padding: 0pt; margin:0pt}\nbody { text-align: center; padding:0pt; margin: 0pt; }</style></head><body><div><svg xmlns = \"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" version=\"1.1\" width=\"100%\" height=\"100%\" viewBox=\"0 0 741 1186\" preserveAspectRatio=\"none\"><image width = \"741\" height=\"1186\" xlink:href=\"../cover.jpeg\"/></svg></div>";
        public string stylesheet = @"div.svg_outer {
   display: block;
   margin-bottom: 0;
   margin-left: 0;
   margin-right: 0;
   margin-top: 0;
   padding-bottom: 0;
   padding-left: 0;
   padding-right: 0;
   padding-top: 0;
   text-align: left;
}
    div.svg_inner {
   display: block;
   text-align: center;
}
h1, h2
{
    text - align: center;
    page -break-before: always;
    margin - bottom: 10 %;
    margin - top: 10 %;
}
h3, h4, h5, h6
{
    text - align: center;
    margin - bottom: 15 %;
    margin - top: 10 %;
}
ol, ul
{
    padding - left: 8 %;
}
body
{
margin: 2 %;
}
p
{
    overflow - wrap: break-word;
}
dd, dt, dl
{
padding: 0;
margin: 0;
}
img
{
display: block;
    min - height: 1em;
    max - height: 100 %;
    max - width: 100 %;
    padding - bottom: 0;
    padding - left: 0;
    padding - right: 0;
    padding - top: 0;
    margin - left: auto;
    margin - right: auto;
    margin - bottom: 2 %;
    margin - top: 2 %;
}
img.inline {
display: inline;
    min - height: 1em;
    margin - bottom: 0;
    margin - top: 0;
}
.thumbcaption
{
display: block;
    font - size: 0.9em;
    padding - right: 5 %;
    padding - left: 5 %;
}
hr
{
color: black;
    background - color: black;
height: 2px;
}
a: link {
    text - decoration: none;
color: #0B0080;
}
a: visited {
    text - decoration: none;
}
a: hover {
    text - decoration: underline;
}
a: active {
    text - decoration: underline;
}
table
{
width: 90 %;
    border - collapse: collapse;
}
table, th, td
{
border: 1px solid black;
}
";

        public string xhtmlCover = "<?xml version='1.0' encoding='utf-8'?><html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\"><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"/><meta name=\"calibre:cover\" content=\"true\"/><title>Cover</title><style type=\"text/css\" title=\"override_css\">@page {padding: 0pt; margin:0pt}\nbody { text-align: center; padding:0pt; margin: 0pt; }</style></head><body><div><svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" version=\"1.1\" width=\"100%\" height=\"100%\" viewBox=\"0 0 741 1186\" preserveAspectRatio=\"none\"><image width=\"741\" height=\"1186\" xlink:href=\"cover.jpeg\"/></svg></div></body></html>";
        public NCX ToC;
        public OPFPackage OPF;

        List<Page> pages;
        List<Image> images;

        public Epub(string title, string author = null, Image image = null, Uri toWork = null)
        {
            Title = title; this.author = author;

            workingDirectory = $"{Directory.GetCurrentDirectory()}\\Epubs\\{title}";
            OEBPSDIR = workingDirectory + "\\OEBPS";

            Directory.CreateDirectory(workingDirectory);

            File.CreateText(workingDirectory + "\\mimetype").Close();
            File.AppendAllText(workingDirectory + "\\mimetype", mimeType);

            Directory.CreateDirectory(OEBPSDIR + "\\Text");
            Directory.CreateDirectory(workingDirectory + "\\META-INF");
            Directory.CreateDirectory(OEBPSDIR + "\\Styles");
            File.CreateText(workingDirectory + "\\META-INF\\container.xml").Close();
            File.AppendAllText(workingDirectory + "\\META-INF\\container.xml", METAINF);

            if (image != null)
                using (BinaryWriter bw = new BinaryWriter(new FileStream(OEBPSDIR + "\\cover.jpeg", FileMode.OpenOrCreate)))
                    bw.Write(image.bytes, 0, image.bytes.Length);

            creditFactory += $"<p>Link to source: <a href=\"{(toWork != null ? toWork.ToString() : "null")}\">{(toWork != null ? toWork.ToString() : "null")}</a></p><p>Work is by: {author}, go support them!</p><p>Converted to Epub by Chay#3670</p></body></html>";
            pages = new List<Page>();
            AddPage(new Page() { id = "titlepage", Text = creditFactory });
        }

        public void AddPage(Page page)
        {
            page.id.Replace(" ", "_");
            page.FileName = $"{pages.Count}_{page.id}.xhtml";
            page.hrefTo = $"Text/{pages.Count}_{page.id}.xhtml";
            File.CreateText($"{OEBPSDIR}\\Text\\{page.FileName}").Close();
            File.AppendAllText($"{OEBPSDIR}\\Text\\{page.FileName}", page.Text);
            pages.Add(page);
        }

        public void CreateEpub()
        {
            //OPF FILE
            OPF = new OPFPackage();
            OPF.metaData = new OPFMetaData(Title, author, "Chay#3670", "null", "2020-01-01");
            OPF.manifest = new Manifest();
            OPF.manifest.items = pages.ToItems();
            OPF.manifest.items.Add(new Item("cover", "cover.jpeg", MediaType.image));
            OPF.manifest.items.Add(new Item("css", "Styles/stylesheet.css", MediaType.css));
            OPF.manifest.items.Add(new Item("ncx", "toc.ncx", MediaType.ncx));
            OPF.spine = new Spine(OPF.manifest.items);

            //TOC
            ToC = new NCX();
            ToC.header = new TOCHeader();
            ToC.header.AddMeta("VrienCo", "dtb:uid");
            ToC.header.AddMeta("1", "dtb:depth");
            ToC.header.AddMeta("0", "dtb:totalPageCount");
            ToC.header.AddMeta("0", "dtb:maxPageNumber");
            
            ToC.title = new DocTitle(Title);
            ToC.map = new NavMap();

            for(int idx = 0; idx < pages.Count; idx++)
                ToC.map.Points.Add(new NavPoint() { text = pages[idx].id, id = $"navPoint-{idx}", playOrder = idx.ToString(), source = pages[idx].hrefTo });

            File.Create(OEBPSDIR + "\\content.opf").Close();
            File.AppendAllText(OEBPSDIR + "\\content.opf", OPF.ToString());

            File.Create(OEBPSDIR + "\\toc.ncx").Close();
            File.AppendAllText(OEBPSDIR + "\\toc.ncx", ToC.GenerateTOCNCXFile());

            
            File.Create(OEBPSDIR + "\\Styles\\stylesheet.css").Close();
            File.AppendAllText(OEBPSDIR + "\\Styles\\stylesheet.css", stylesheet);

            File.Create(OEBPSDIR + "\\Styles\\stylesheet.css").Close();
            File.AppendAllText(OEBPSDIR + "\\cover.xhtml", xhtmlCover);
        }
    }
    
    class OPFPackage
    {
        public OPFMetaData metaData;
        public Manifest manifest;
        public Spine spine;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<package xmlns=\"http://www.idpf.org/2007/opf\" version=\"2.0\">");
            sb.AppendLine(metaData.ToString());
            sb.AppendLine(manifest.ToString());
            sb.AppendLine(spine.ToString());
            sb.AppendLine("<guide><reference type=\"cover\" title=\"cover\" href=\"cover.xhtml\"/></guide>");
            sb.AppendLine("</package>");
            return sb.ToString();
        }
    }

    class Spine
    {
        List<Item> items;
        public Spine(List<Item> items)
        {
            this.items = items;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<spine toc=\"ncx\">");
            foreach (Item item in this.items)
                sb.AppendLine($"<itemref idref=\"{item.id}\"/>");
            sb.AppendLine("</spine>");
            return sb.ToString();
        }
    }

    class OPFMetaData
    {
        List<Meta> metadata;

        public OPFMetaData(string title, string author, string bookid, string cover, string moddate)
        {
            Meta Title = new Meta($">{title}", "title", MetaType.dc);
            Meta Language = new Meta(">en_US", "language", MetaType.dc);
            Meta Author = new Meta($"opf:role=\"auth\" opf:file-as=\"{author}\">{author}", "creator", MetaType.dc);
            Meta Identifier = new Meta($"id=\"BookID\" opf:scheme=\"URI\">{bookid}", "identifier", MetaType.dc);
            Meta pub = new Meta(">Chay#3670", "publisher", MetaType.dc);
            Meta _cover = new Meta("cover", "cover");
            Meta creator = new Meta("1.0f", "VrienV");
            Meta date = new Meta($"xmlns:opf=\"http://www.idpf.org/2007/opf\" opf:event=\"modification\">{ DateTime.Now }", "date", MetaType.dc);

            metadata = new List<Meta>();

            metadata.AddRange(new Meta[] { Title, Language, Author, Identifier, _cover, creator });
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<metadata xmlns:opf=\"http://www.idpf.org/2007/opf\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\">");
            foreach (Meta meta in metadata)
                sb.AppendLine(meta.ToString());
            sb.AppendLine("</metadata>");
            return sb.ToString();
        }
    }
    class Manifest
    {
        public List<Item> items;

        public Manifest() =>
            items = new List<Item>();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<manifest>");
            foreach (Item it in items)
                sb.Append(it.ToString());
            sb.AppendLine("</manifest>");
            return sb.ToString();
        }
    }

    public class Item
    {
        public string id, href;
        MediaType mediaType;

        public Item(string id, string href, MediaType mediaType)
        {
            this.id = id; this.href = href; this.mediaType = mediaType;
        }

        public override string ToString()
            => $"<item id=\"{id}\" href=\"{href}\" media-type=\"{shorts.mediaTypes(mediaType)}\"/>";
    }

    /// <summary>
    /// JPG only please.
    /// </summary>
    class Image
    {
        public string Name;
        // Location is set when exporting to epub
        public string location;
        public Byte[] bytes;

        public static Image LoadImageFromFile(string name, string location)
            => new Image { Name = name, bytes = File.ReadAllBytes(location)};

    }

    public class Page
    {
        public string id;
        public string Text;
        public string FileName;
        public string hrefTo;

        public static Page AutoGenerate(string pageText, string title)
        {
            pageText = shorts.MakeTextXHTMLReady(pageText);
            foreach (KeyValuePair<string, string> str in shorts.RemoveList)
                pageText = pageText.Replace(str.Key, str.Value);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<!DOCTYPE html PUBLIC \" -//W3C//DTD XHTML 1.1//EN\"\n\"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">\n");
            sb.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">\n<head><title></title><link href=\"../Styles/stylesheet.css\" type=\"text/css\" rel=\"stylesheet\"/></head>");
            sb.AppendLine($"<body>\n<h1 class=\"entry-title\">{title}</h1><p></p>");
            string[] st = pageText.Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.None);
            foreach(string str in st)
                sb.AppendLine($"<p>{str}</p>");
            sb.AppendLine("</body></html>");
            return new Page() { id = title, Text = sb.ToString(), FileName = title };
        }
    }

    class NCX
    {
        StringBuilder sb = new StringBuilder();
        public TOCHeader header;
        public DocTitle title;
        public NavMap map;

        public NCX()
        {
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<!DOCTYPE ncx PUBLIC \" -//NISO//DTD ncx 2005-1//EN\"\n\"http://www.daisy.org/z3986/2005/ncx-2005-1.dtd\"><ncx version = \"2005-1\" xmlns = \"http://www.daisy.org/z3986/2005/ncx/\" >");
        }

        public string GenerateTOCNCXFile()
        {
            sb.AppendLine(header.ToString());
            sb.AppendLine(title.ToString());
            sb.AppendLine(map.ToString());
            sb.AppendLine("</ncx>");
            return sb.ToString();
        }
    }

    class TOCHeader
    {
        List<Meta> metaContent;

        public TOCHeader()
        {
            metaContent = new List<Meta>();
        }

        public void AddMeta(Meta metacontent)
            => metaContent.Add(metacontent);

        public void AddMeta(string a, string b)
            => metaContent.Add(new Meta(a, b));

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<head>");
            foreach(Meta meta in metaContent)
                sb.AppendLine(meta.ToString());
            sb.AppendLine("</head>");
            return sb.ToString();
        }
    }

    class DocTitle
    {
        string docName;
        public DocTitle(string name)
            => docName = name;
        public override string ToString()
            => $"<docTitle><text>{docName}</text></docTitle>";

        public override bool Equals(object obj)
            => docName == obj;
    }

    class NavMap
    {
        public List<NavPoint> Points;
        public NavMap() => this.Points = new List<NavPoint>();
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<navMap>");
            foreach(NavPoint point in Points)
                sb.AppendLine($"<navPoint id=\"navPoint-{point.id}\" playOrder=\"{point.playOrder}\"><navLabel><text>{point.text}</text></navLabel><content src=\"{point.source}\"/></navPoint>");
            sb.AppendLine("</navMap>");
            return sb.ToString();
        }
    }

    class NavPoint
    {
        public string id, playOrder;
        public string text, source;
    }

    /// <summary>
    /// If metaType DC content is the other variables of the data, e.x content = "name=\"coolio\""
    /// </summary>
    class Meta
    {
        MetaType metaType;
        string metaHeader;
        public Meta(string content, string name, MetaType mt = MetaType.meta)
        {
            metaType = mt;
            if (mt == MetaType.meta)
                metaHeader = $"<meta content=\"{content}\" name=\"{name}\"/>";
            else
                metaHeader = $"<dc:{name} {content}</dc:{name}>";
        }

        public override string ToString() => metaHeader;
    }
}
