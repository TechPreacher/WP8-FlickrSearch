using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace WP8_FlickrSearch
{
    class FlickrUriMapper : UriMapperBase
    {
        private static string targetPageName = "DetailsPage.xaml";
        private static string protocolTemplate = "/Protocol?encodedLaunchUri=";
        private static int protocolTemplateLength = protocolTemplate.Length;
        private string tempUri;

        private static string fileTemplate = "/FileTypeAssociation?fileToken=";

        public override Uri MapUri(Uri uri)
        {
            tempUri = uri.ToString();

            if (tempUri.Contains("/FileTypeAssociation"))
            {
                tempUri = HttpUtility.UrlDecode(tempUri);

                if (tempUri.Contains("fileToken"))
                {
                    return GetFileMappedUri(tempUri);
                }
            }

            if (tempUri.Contains("/Protocol"))
            {
                tempUri = HttpUtility.UrlDecode(tempUri);

                if (tempUri.Contains("flickr:"))
                {
                    return GetMappedUri(tempUri);
                }
            }
            return uri;
        }

        private Uri GetFileMappedUri(string uri)
        {
            string fileToken = "";

            // Extract parameter values from URI.
            if (uri.IndexOf(fileTemplate) > -1)
            {
                int groupIdLen = uri.IndexOf("=", 0);
                fileToken = uri.Substring(groupIdLen + 1);
            }

            string NewURI = String.Format("/{0}?ID={1}&HandleFile", targetPageName, fileToken);

            return new Uri(NewURI, UriKind.Relative);
        }

        private Uri GetMappedUri(string uri)
        {
            string operation = "";
            string groupUID = "";

            // Extract parameter values from URI.
            if (uri.IndexOf(protocolTemplate) > -1)
            {
                int operationLen = uri.IndexOf("?", protocolTemplateLength);
                int groupIdLen = uri.IndexOf("=", operationLen + 1);

                operation = uri.Substring(protocolTemplateLength, operationLen - protocolTemplateLength);
                groupUID = uri.Substring(groupIdLen + 1);
            }

            string NewURI = String.Format("/{0}?ID={1}&HandleUri", targetPageName, groupUID);

            return new Uri(NewURI, UriKind.Relative);
        }
    }
}
