#region usings

using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;

#endregion

namespace Heath.Lister.About
{
    internal sealed class ManifestAppInfo
    {
        private const string VersionLabel = "VERSION";
        private const string ProductIdLabel = "PRODUCTID";
        private const string TitleLabel = "TITLE";
        private const string GenreLabel = "GENRE";
        private const string DescriptionLabel = "DESCRIPTION";
        private const string AuthorLabel = "AUTHOR";
        private const string PublisherLabel = "PUBLISHER";

        private static Dictionary<string, string> _attributes;

        static ManifestAppInfo()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDesignTimeData();
            }
            else
            {
                LoadData();
            }
        }

        private static Dictionary<string, string> Attributes
        {
            get { return _attributes; }
        }

        public string Version
        {
            get { return Attributes.ContainsKey(VersionLabel) ? Attributes[VersionLabel] : null; }
        }

        public string ProductId
        {
            get { return Attributes.ContainsKey(ProductIdLabel) ? Attributes[ProductIdLabel] : null; }
        }

        public string Title
        {
            get { return Attributes.ContainsKey(TitleLabel) ? Attributes[TitleLabel] : null; }
        }

        public string Genre
        {
            get { return Attributes.ContainsKey(GenreLabel) ? Attributes[GenreLabel] : null; }
        }

        public string Description
        {
            get { return Attributes.ContainsKey(DescriptionLabel) ? Attributes[DescriptionLabel] : null; }
        }

        public string Author
        {
            get { return Attributes.ContainsKey(AuthorLabel) ? Attributes[AuthorLabel] : null; }
        }

        public string Publisher
        {
            get { return Attributes.ContainsKey(PublisherLabel) ? Attributes[PublisherLabel] : null; }
        }

        private static void LoadDesignTimeData()
        {
            _attributes = new Dictionary<string, string>
            {
                { VersionLabel, "1.2.3.4" },
                { ProductIdLabel, "{CF68A1E0-578C-4A7C-9278-6AC10F51EAE1}" },
                { TitleLabel, "My Title" },
                { GenreLabel, "apps.normal" },
                { DescriptionLabel, "Some really long sample description that exceeds the available width of the screen to test whether wrapping works correctly :-)." },
                { AuthorLabel, "Mr. Author" },
                { PublisherLabel, "My Publisher" }
            };
        }

        private static void LoadData()
        {
            _attributes = new Dictionary<string, string>();

            var appManifestXml = XDocument.Load("WMAppManifest.xml");

            using (var xmlReader = appManifestXml.CreateReader(ReaderOptions.None))
            {
                xmlReader.ReadToDescendant("App");
                xmlReader.MoveToFirstAttribute();

                while (xmlReader.MoveToNextAttribute())
                {
                    _attributes.Add(xmlReader.Name.ToUpper(), xmlReader.Value);
                }
            }
        }
    }
}