using System;
using Renci.SshNet;
using System.IO;
using System.Xml;

namespace ModificarNombreXML
{
    internal class ejecuta
    {
        static void Main(string[] args)
        {
            string host = "tu.servidor.sftp";
            int puerto = 22;
            string usuario = "tu.usuario";
            string contraseña = "tu.contraseña";
            string directorioArchivos = "/archivos"; // Ruta de la carpeta "archivos"

            using (var clienteSftp = new SftpClient(host, puerto, usuario, contraseña))
            {
                try
                {
                    clienteSftp.Connect();
                    Console.WriteLine("Conexión SFTP establecida correctamente.");

                    var archivos = clienteSftp.ListDirectory(directorioArchivos);
                    foreach (var archivo in archivos)
                    {
                        if (!archivo.IsDirectory && archivo.Name.EndsWith(".xml"))
                        {
                            // Leer el contenido del archivo XML
                            using (var stream = clienteSftp.OpenRead(Path.Combine(directorioArchivos, archivo.Name)))
                            {
                                XmlDocument xmlDoc = new XmlDocument();
                                xmlDoc.Load(stream);

                                XmlNodeList refNodes = xmlDoc.SelectNodes("//REF");

                                foreach (XmlNode refNode in refNodes)
                                {
                                    XmlNode ref1Node = refNode.SelectSingleNode("REF_1");
                                    XmlNode ref2Node = refNode.SelectSingleNode("REF_2");

                                    string ref1Value = ref1Node.InnerText;
                                    string ref2Value = ref2Node.InnerText;

                                    if (ref1Value == "TN" && ref2Value != null)
                                    {
                                        Console.WriteLine($"Archivo leído: {archivo.Name}, Nombre nuevo: {ref2Value}");
                                        // Renombrar el archivo
                                        clienteSftp.RenameFile(Path.Combine(directorioArchivos, archivo.Name), Path.Combine(directorioArchivos, ref2Value + ".xml"));

                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al conectar o procesar archivos: {ex.Message}");
                }
                finally
                {
                    clienteSftp.Disconnect();
                }
            }
        }
    }
}
