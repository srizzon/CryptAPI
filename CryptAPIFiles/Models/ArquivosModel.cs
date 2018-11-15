using CryptAPI.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace CryptAPI.Models
{
    public class ArquivosModel
    {
        public ArquivosModel()
        {

        }

        /*
         * Salva o arquivo no diretorio criado
         */
        public string SalvarArquivo(FileUploadDTO file)
        {
            var caminho = VerificaDiretorio();

            // use Path.Combine to combine 2 strings to a path
            File.WriteAllBytes(Path.Combine(caminho, file.Nome), file.Arquivo);

            return "Salvo com sucesso!";
        }

        /*
         * Verifica se o diretório existe e retorna o caminho,
         * Se não existir, ele cria um novo e retorna o caminho do mesmo
        */
        private string VerificaDiretorio()
        {
            string systemPath = Path.GetPathRoot(Environment.SystemDirectory);

            var path = systemPath + "Crypt\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public async Task Upload(FileUploadDTO file)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://187.111.18.94:54321");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var result = await client.PostAsJsonAsync("/CryptAPI/api/EnviarArquivos/Post", file);
                string resultContent = await result.Content.ReadAsStringAsync();
            }
        }
    }
}