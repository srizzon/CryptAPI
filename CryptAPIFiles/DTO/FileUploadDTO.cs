using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptAPI.DTO
{
    public class FileUploadDTO
    {
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public byte[] Arquivo { get; set; }
    }
}