using CryptAPI.DTO;
using CryptAPI.Models;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace CryptAPI.Controllers
{
    public class EnviarArquivosController : ApiController
    {
        /*
         * Método destinado ao envio dos arquivos.
         * Para enviar um arquivo corretamente é necessário enviar um objeto do tipo FileUploadDTO
         */
        [HttpPost]
        // POST: api/EnviarArquivos
        public JsonResult Post(FileUploadDTO file)
        {
            try
            {
                ArquivosModel arquivosModel = new ArquivosModel();
                arquivosModel.SalvarArquivo(file);

                return new JsonResult() { error = false, message = "Arquivo salvo com sucesso!", status = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new JsonResult() { error = true, message = ex.Message, status = HttpStatusCode.InternalServerError };
            }
        }

        [HttpGet]
        // GET: api/TesteEnvio
        public JsonResult TesteEnvio()
        {
            try
            {
                ArquivosModel arquivosModel = new ArquivosModel();
                DirectoryInfo directory = new DirectoryInfo(@"C:\Users\Rizzon\Desktop\Faculdade\teste");
                FileInfo[] Files = directory.GetFiles();

                foreach (FileInfo file in Files)
                {
                    var newFile = new FileUploadDTO();

                    newFile.Nome = file.Name;
                    newFile.Tipo = file.Extension;

                    var bytes = File.ReadAllBytes(file.FullName);

                    newFile.Arquivo = bytes;

                    Task.Run(() => arquivosModel.Upload(newFile));
                }

                return new JsonResult() { error = false, message = "Arquivo enviado com sucesso!", status = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new JsonResult() { error = true, message = ex.Message, status = HttpStatusCode.InternalServerError };
            }
        }
    }
}
