using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace pdfToClass
{
    public class RestOCR
    {
        public static string ProcessDocument(string user_name, string license_code, string file_path)
        {
            string ocrResult = "";
            string ocrURL = @"http://www.ocrwebservice.com/restservices/processDocument?language=danish&gettext=true";

            byte[] uploadData = GetUploadedFile(file_path);

            HttpWebRequest request = CreateHttpRequest(ocrURL, user_name, license_code, "POST");
            request.ContentLength = uploadData.Length;

            //  Send request
            using (Stream post = request.GetRequestStream())
            {
                post.Write(uploadData, 0, (int)uploadData.Length);
            }

            try
            {
                //  Get response
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Parse JSON response
                    string strJSON = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    OCRResponseData ocrResponse = JsonConvert.DeserializeObject<OCRResponseData>(strJSON);

                    ocrResult = PrintOCRData(ocrResponse);

                    // Download output converted file
                    if (!string.IsNullOrEmpty(ocrResponse.OutputFileUrl))
                    {
                        HttpWebRequest request_get = (HttpWebRequest)WebRequest.Create(ocrResponse.OutputFileUrl);
                        request_get.Method = "GET";

                        using (HttpWebResponse result = request_get.GetResponse() as HttpWebResponse)
                        {
                            DownloadConvertedFile(result, "C:\\converted_file.doc");
                        }
                    }
                }
            }
            catch (WebException wex)
            {
                Console.WriteLine(string.Format("OCR API Error. HTTPCode:{0}", ((HttpWebResponse)wex.Response).StatusCode));
            }
            return ocrResult;
        }

        /// <summary>
        /// Print OCRWebService.com account information
        /// </summary>
        /// <param name="user_name"></param>
        /// <param name="license_code"></param>
        private static void PrintAccountInformation(string user_name, string license_code)
        {
            try
            {
                string address_get = @"http://www.ocrwebservice.com/restservices/getAccountInformation";

                HttpWebRequest request_get = CreateHttpRequest(address_get, user_name, license_code, "GET");

                using (HttpWebResponse response = request_get.GetResponse() as HttpWebResponse)
                {
                    string strJSON = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    OCRResponseAccountInfo ocrResponse = JsonConvert.DeserializeObject<OCRResponseAccountInfo>(strJSON);

                    Console.WriteLine(string.Format("Available pages:{0}", ocrResponse.AvailablePages));
                    Console.WriteLine(string.Format("Max pages:{0}", ocrResponse.MaxPages));
                    Console.WriteLine(string.Format("Expiration date:{0}", ocrResponse.ExpirationDate));
                    Console.WriteLine(string.Format("Last processing time:{0}", ocrResponse.LastProcessingTime));
                }

            }
            catch (WebException wex)
            {
                Console.WriteLine(string.Format("OCR API Error. HTTPCode:{0}", ((HttpWebResponse)wex.Response).StatusCode));
            }
        }

        private static byte[] GetUploadedFile(string file_name)
        {
            FileStream streamContent = new FileStream(file_name, FileMode.Open, FileAccess.Read);
            byte[] inData = new byte[streamContent.Length];
            streamContent.Read(inData, 0, (int)streamContent.Length);
            return inData;
        }

        private static HttpWebRequest CreateHttpRequest(string address_url, string user_name, string license_code, string http_method)
        {
            Uri address = new Uri(address_url);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);

            byte[] authBytes = Encoding.UTF8.GetBytes(string.Format("{0}:{1}", user_name, license_code).ToCharArray());
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(authBytes);
            request.Method = http_method;
            request.Timeout = 600000;

            // Specify Response format to JSON or XML (application/json or application/xml)
            request.ContentType = "application/json";

            return request;
        }

        private static string PrintOCRData(OCRResponseData ocrResponse)
        {
            string result = "";
            // Available pages
            //Console.WriteLine("Available pages: " + ocrResponse.AvailablePages);

            // Extracted text. For zonal OCR: OCRText[z][p]    z - zone, p - pages
            for (int zone = 0; zone < ocrResponse.OCRText.Count; zone++)
            {
                for (int page = 0; page < ocrResponse.OCRText[zone].Count; page++)
                {
                    //Console.WriteLine(string.Format("Extracted text from page №{0}, zone №{1} :{2}", page, zone, ocrResponse.OCRText[zone][page]));
                    result += ocrResponse.OCRText[zone][page];
                }
            }

            return result;
        }

        private static void DownloadConvertedFile(HttpWebResponse result, string file_name)
        {
            using (Stream response_stream = result.GetResponseStream())
            {
                using (Stream output_stream = File.OpenWrite(file_name))
                {
                    response_stream.CopyTo(output_stream);
                }
            }
        }
    }

    /// <summary>
    /// User account information class
    /// </summary>
    internal class OCRResponseAccountInfo
    {
        /// <summary>
        /// Available pages
        /// </summary>
        public int AvailablePages { get; set; }

        /// <summary>
        /// Max pages
        /// </summary>
        public int MaxPages { get; set; }

        /// <summary>
        /// Expiration Date
        /// </summary>
        public string ExpirationDate { get; set; }

        /// <summary>
        /// Last processing date
        /// </summary>
        public string LastProcessingTime { get; set; }

        /// <summary>
        /// Subscription plan
        /// </summary>
        public string SubcriptionPlan { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// OCR Response data
    /// </summary>
    internal class OCRResponseData
    {
        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Available pages
        /// </summary>
        public int AvailablePages { get; set; }

        /// <summary>
        /// OCRed text
        /// </summary>
        public List<List<string>> OCRText { get; set; }

        /// <summary>
        /// Output file1 URL
        /// </summary>
        public string OutputFileUrl { get; set; }

        /// <summary>
        /// Output file2 URL
        /// </summary>
        public string OutputFileUrl2 { get; set; }

        /// <summary>
        /// Output file3 URL
        /// </summary>
        public string OutputFileUrl3 { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public List<List<string>> Reserved { get; set; }

        /// <summary>
        /// OCRWords
        /// </summary>
        public List<List<OCRWSWord>> OCRWords { get; set; }

        /// <summary>
        /// Task description
        /// </summary>
        public string TaskDescription { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public OCRResponseData()
        {
            OCRText = new List<List<string>>();
            Reserved = new List<List<string>>();
            OCRWords = new List<List<OCRWSWord>>();
        }
    }

    internal class OCRWSWord
    {
        public int Top;
        public int Left;
        public int Height;
        public int Width;
        public string OCRWord;
    }
}
