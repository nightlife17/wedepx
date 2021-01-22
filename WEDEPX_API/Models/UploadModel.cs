using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace WEDEPX_API.Models
{
    public class UploadModel
    {
        public long EMP_CODE { get; set; }
        public Nullable<System.DateTime> BIRTH_DAY { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string NICK_NAME { get; set; }
        public HttpPostedFileBase PHOTO_PATH { get; set; }
    }
}