using PuppeteerSharp;
using PuppeteerSharp.Media;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEDEPX_DB.Dao;
using WEDEPX_DB.Models;

namespace WEDEPX
{
    public class WedepxService : DaoDb
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly WEDEPXEntities _db;
        private string pathViewHtml = System.Configuration.ConfigurationManager.AppSettings["PathView"]; // "C://WEDEPX//Views//Views";
        private string outputImage = System.Configuration.ConfigurationManager.AppSettings["PathGenImage"]; //"C://WEDEPX//GenImage";
        private string Image = System.Configuration.ConfigurationManager.AppSettings["PathImage"]; //"C://WEDEPX//GenImage";
        public WedepxService()
        {
            _db = GetConnection();

        }

        public async Task CallWedepxServiceAsync()
        {

            try
            {
                var date = DateTime.Now.Date;
                var listBd = _db.bd_emp.Where(x => x.BIRTH_DAY == date).ToList();
                if (listBd.Count() > 0)
                {

                    string folder = Path.Combine(pathViewHtml, "E_card.cshtml");
                    string razorText = File.ReadAllText(folder);


                    foreach (var data in listBd)
                    {
                        ModelDetail detail = new ModelDetail
                        {
                            BIRTH_DAY = ((DateTime)data.BIRTH_DAY).ToString("d.MM.yyyy"),
                            FULLNAME = data.FIRST_NAME + " " + data.LAST_NAME,
                            PHOTO_PATH_TO = getBase64FromFile(Path.Combine(Image, data.PHOTO_PATH)),
                            NICK_NAME = data.NICK_NAME,
                            BGIMG = getBase64FromFile(Path.Combine(Image, "bgECard.jpg")),
                        };

                        string folderImage = Path.Combine(outputImage, data.PHOTO_PATH + ".jpg");

                        var result = Engine.Razor.RunCompile(razorText, "templateKey", null, detail);



                        await CovertHtmlToImage(result, folderImage);


                    }
                }

            }
            catch (Exception ex)
            {
            }

        }



        public async Task<FileModel> GetBytefromImageAsync(int EMP_CODE)
        {
            byte[] by = new byte[0];
            var data = _db.bd_emp.Where(x => x.EMP_CODE == EMP_CODE).FirstOrDefault();
            var file = new FileModel();
            if (data != null)
            {

                string folder = Path.Combine(pathViewHtml, "E_card.cshtml");
                string razorText = File.ReadAllText(folder);

                DateTime dt = (DateTime)data.BIRTH_DAY;

                ModelDetail detail = new ModelDetail
                {
                    BIRTH_DAY = ((DateTime)data.BIRTH_DAY).ToString("d.MM.yyyy"),
                    FULLNAME = data.FIRST_NAME + " " + data.LAST_NAME,
                    PHOTO_PATH_TO = getBase64FromFile(Path.Combine(Image, data.PHOTO_PATH)),
                    NICK_NAME = data.NICK_NAME,
                    BGIMG = getBase64FromFile(Path.Combine(Image, "bgECard.jpg")),
                };

                var result = Engine.Razor.RunCompile(razorText, "templateKey", null, detail);

                by = await CovertHtmlToImageAsync(result);
                file.FileName = data.NICK_NAME;
                file.File = by;
            }
            return file;

        }
        public void SaveFile(MemoryStream stream, string FileName)
        {

            string PathFile = Path.Combine(Image, FileName);
            System.IO.File.WriteAllBytes(PathFile, stream.ToArray());

        }
        public void DeleteFile(string file)
        {
            string PathFile = Path.Combine(Image, file);
            System.IO.File.Delete(PathFile);
        }
        public async Task CovertHtmlToImage(string html, string ouputpdf)
        {
            try
            {
                var options = new LaunchOptions
                {
                    Headless = true,
                    ExecutablePath = "C://Program Files (x86)//Google//Chrome//Application//chrome.exe"
                    //  ExecutablePath = HttpContext.Server.MapPath($"~//Chrome//Chrome-bin/chrome.exe"),

                };

                using (var browser = await Puppeteer.LaunchAsync(options))
                using (var page = await browser.NewPageAsync())
                {


                    await page.SetContentAsync(html, new NavigationOptions { WaitUntil = new[] { WaitUntilNavigation.Networkidle0 } });

                    var result = await page.GetContentAsync();
                    var imOptions = new ScreenshotOptions
                    {
                        Type = ScreenshotType.Jpeg,
                        Quality = 100,
                        FullPage = true,
                    };
                    await page.ScreenshotAsync(ouputpdf, imOptions);
                }
            }
            catch (Exception ex)
            {

            }
        }
        public async Task<byte[]> CovertHtmlToImageAsync(string html)
        {
            byte[] by = new byte[0];
            var options = new LaunchOptions
            {
                Headless = true,
                ExecutablePath = "C://Program Files (x86)//Google//Chrome//Application//chrome.exe"
                //  ExecutablePath = HttpContext.Server.MapPath($"~//Chrome//Chrome-bin/chrome.exe"),

            };

            using (var browser = await Puppeteer.LaunchAsync(options))
            using (var page = await browser.NewPageAsync())
            {


                await page.SetContentAsync(html, new NavigationOptions { WaitUntil = new[] { WaitUntilNavigation.Networkidle0 } });

                var result = await page.GetContentAsync();
                var imOptions = new ScreenshotOptions
                {
                    Type = ScreenshotType.Jpeg,
                    Quality = 100,
                    FullPage = true,
                };
                by = await page.ScreenshotDataAsync(imOptions);

            }
            return by;

        }

        public string getBase64FromFile(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            BinaryReader br = new BinaryReader(fs);

            byte[] btContent = br.ReadBytes((int)fs.Length);
            string contentBase64 = string.Empty;
            br.Close();
            fs.Close();

            contentBase64 = System.Convert.ToBase64String(btContent);

            return contentBase64;
        }
    }

    public class ModelDetail
    {
        public string NAME_TH { get; set; }
        public string FULLNAME { get; set; }
        public string SURNAME_TH { get; set; }
        public string NICK_NAME { get; set; }
        public string PHOTO_PATH_TO { get; set; }
        public string E_MAIL { get; set; }
        public string BIRTH_DAY { get; set; }
        public string AttFiles { get; set; }

        public string DISPLAYNAME { get; set; }
        public string BGIMG { get; set; }
    }
}
