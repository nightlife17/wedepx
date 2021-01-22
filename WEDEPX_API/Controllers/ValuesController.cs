using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WEDEPX;
using WEDEPX_API.Lib;
using WEDEPX_API.Models;
using WEDEPX_DB.Dao;
using WEDEPX_DB.Models;

namespace WEDEPX_API.Controllers
{
    // [EnableCors(origins: "https://localhost:44373/", headers: "*", methods: "*")]
    public class ValuesController : ApiController
    {
        private readonly WedepxDbHelper dbHelper;
        private readonly WedepxService service;


        public ValuesController()
        {
            dbHelper = new WedepxDbHelper();
            service = new WedepxService();
        }
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet]
        public HttpResponseMessage GetValues()
        {
            try
            {
                var listEmp = dbHelper.Get();
                return aReturnData(ToDataTable<bd_emp>(listEmp));
            }
            catch (Exception ex)
            {
                return aReturnError(ex.Message);
            }

        }

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<HttpResponseMessage> Save()
        {
            try
            {

                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }
                var provider = await Request.Content.ReadAsMultipartAsync<InMemoryMultipartFormDataStreamProvider>(new InMemoryMultipartFormDataStreamProvider());
                //access form data  
                NameValueCollection formData = provider.FormData;
                //access files  
                IList<HttpContent> files = provider.Files;

                HttpContent file1 = files[0];
                var thisFileName = file1.Headers.ContentDisposition.FileName.Trim('\"');
                var stream = new MemoryStream();
                await file1.CopyToAsync(stream);
                service.SaveFile(stream, thisFileName);

                CultureInfo provide = CultureInfo.InvariantCulture;
                var sa = new bd_emp();

                //sa.EMP_CODE = Convert.ToInt64(formData["EMP_CODE"]);
                //  var date = formData["BIRTH_DAY"];
                sa.BIRTH_DAY = DateTime.ParseExact(formData["BIRTH_DAY"], "yyyy-MM-dd", provide);
                sa.FIRST_NAME = formData["FIRST_NAME"];
                sa.LAST_NAME = formData["LAST_NAME"];
                sa.NICK_NAME = formData["NICK_NAME"];
                sa.PHOTO_PATH = thisFileName;

                dbHelper.Save(sa);
                // dbHelper.Delete(EMP_CODE);
                return aReturnData("Success");
            }
            catch (Exception ex)
            {
                return aReturnError(ex.Message);
            }

        }

        [HttpGet]
        public HttpResponseMessage Delete(int EMP_CODE)
        {
            try
            {
                var del = dbHelper.Delete(EMP_CODE);
                service.DeleteFile(del.PHOTO_PATH);
                return aReturnData("Success");
            }
            catch (Exception ex)
            {
                return aReturnError(ex.Message);
            }

        }
        [HttpGet]
        public async System.Threading.Tasks.Task<HttpResponseMessage> Generate(int EMP_CODE)
        {
            try
            {

                var file = await service.GetBytefromImageAsync(Convert.ToInt32(EMP_CODE));

                return aReturnFileData(file.File, file.FileName);


            }
            catch (Exception ex)
            {
                return aReturnError(ex.Message);

            }
        }

        private static HttpResponseMessage aReturnData(DataTable tmp_dtt)
        {
            if (tmp_dtt.Rows.Count == 0)
            {
                return new HttpResponseMessage()
                {
                    Content = new JsonContent(new
                    {
                        status = "data not found!!!",
                    })
                };
            }
            else
            {
                return new HttpResponseMessage()
                {
                    Content = new JsonContent(new
                    {
                        status = "ok",
                        data = tmp_dtt,
                    })
                };
            }
        }
        private static HttpResponseMessage aReturnData(string tmp_str)
        {
            return new HttpResponseMessage()
            {
                Content = new JsonContent(new
                {
                    status = "ok",
                    data = tmp_str
                })
            };
        }
        private static HttpResponseMessage aReturnError(string tmp_ex)
        {
            return new HttpResponseMessage()
            {
                Content = new JsonContent(new
                {
                    status = "error",
                    data = tmp_ex,
                })
            };
        }
        private static HttpResponseMessage aReturnFileData(byte[] file, string FileName)
        {
            if (file.Length > 0)
            {
                var stream = new MemoryStream(file);
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(stream.ToArray()),
                };

                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = FileName + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".jpg", ///"genImage.jpg"
                };

                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                return result;
            }
            else
            {
                return new HttpResponseMessage()
                {
                    Content = new JsonContent(new
                    {
                        status = "data not found!!!",
                    })
                };
            }
        }
        private static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

    }
}
