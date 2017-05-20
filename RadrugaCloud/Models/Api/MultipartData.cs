using Core.CommonModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RadrugaCloud.Models.Api
{
    public class MultipartData
    {
        public FileData FileData { get; set; }

        public string TextData { get; set; }

        public MultipartDataError ErrorType { get; set; }
    }

    public enum MultipartDataError
    {
        NoError = 0,
        TextDataEmpty = 1,
        FileDataEmpty = 2
    }
}