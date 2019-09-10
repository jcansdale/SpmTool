using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using System.Text;

namespace SpmTool.WebApplication
{
    public partial class Convert : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void uploadButton_Click(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                string fileName = null;
                byte[] dx8SpmBytes = null;
                byte[] dx9SpmBytes = null;
                var messageWriter = new StringWriter();

                string contact = contactTextBox.Text;
                string generator = targetGenerator.SelectedItem.Value;

                try
                {
                    statusLabel.Text = ""; // Clear any previous messages.

                    if(!fileUpload.HasFile)
                    {
                        statusLabel.Text = "Please select a SPM file";
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(contact) || !contact.Contains('@'))
                    {
                        statusLabel.Text = "I might need to contact you regarding the conversion process. Please leave a contact email address. I won't spam you! ;)";
                        return;
                    }

                    var postedFile = fileUpload.PostedFile;
                    fileName = Path.GetFileName(postedFile.FileName);
                    var removeIndex = removeIndexCheckBox.Checked;

                    if (fileName.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var dx8MemoryStream = new MemoryStream();
                        postedFile.InputStream.CopyTo(dx8MemoryStream);
                        dx8SpmBytes = dx8MemoryStream.ToArray();

                        var dx9MemoryStream = new MemoryStream();
                        convertZip(generator, removeIndex, new MemoryStream(dx8SpmBytes), dx9MemoryStream, messageWriter);
                        dx9SpmBytes = dx9MemoryStream.ToArray();
                    }
                    else if (fileName.EndsWith(".spm", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var reader = new StreamReader(postedFile.InputStream);
                        string dx8Spm = reader.ReadToEnd();
                        dx8SpmBytes = Encoding.ASCII.GetBytes(dx8Spm);

                        string dx9Spm = convertSpm(dx8Spm, generator, removeIndex, fileName, messageWriter);
                        if (dx9Spm == null)
                        {
                            sendEmail(contact, "Couldn't convert: " + fileName, messageWriter.ToString(), fileName, dx8SpmBytes, dx9SpmBytes, generator);
                            statusLabel.Text = messageWriter.ToString();
                            return;
                        }

                        dx9SpmBytes = Encoding.ASCII.GetBytes(dx9Spm);
                    }
                    else
                    {
                        statusLabel.Text = "Please select a SPM or ZIP file for conversion.";
                        return;
                    }
                }
                catch (Exception ex)
                {
                    messageWriter.WriteLine(ex);
                    sendEmail(contact, "Error converting: " + fileName, messageWriter.ToString(), fileName, dx8SpmBytes, dx9SpmBytes, generator);

                    statusLabel.Text = "The file could not be converted: " + ex.Message;
                    return;
                }

                sendEmail(contact, "Converted: " + fileName, messageWriter.ToString(), fileName, dx8SpmBytes, dx9SpmBytes, generator);

                Response.AddHeader("Content-disposition", "attachment; filename=" + generator + "_" + fileName);
                Response.ContentType = "application/octet-stream";
                Response.OutputStream.Write(dx9SpmBytes, 0, dx9SpmBytes.Length);
                Response.End();
            }
        }

        static void convertZip(string targetGenerator, bool removeIndex, Stream inputStream, Stream outputStream, StringWriter messageWriter)
        {
            var zipOutputStream = new ZipOutputStream(outputStream);
            var zipFile = new ZipFile(inputStream);
            foreach (ZipEntry entry in zipFile)
            {
                if (entry.IsDirectory) continue;

                string path = entry.Name;
                if (!path.EndsWith(".spm", StringComparison.InvariantCultureIgnoreCase))
                {
                    messageWriter.WriteLine(path + " is not a SPM file");
                    continue;
                }

                var dx8Reader = new StreamReader(zipFile.GetInputStream(entry));
                string sourceSpm = dx8Reader.ReadToEnd();

                string convertedSpm = convertSpm(sourceSpm, targetGenerator, removeIndex, path, messageWriter);
                if (convertedSpm == null)
                {
                    continue;
                }

                if (targetGenerator == "DX8" || targetGenerator == "DX7S")
                {
                    string fileName = Path.GetFileName(path);
                    fileName = SpmUtilities.GetDX8Filename(fileName);
                    string dir = Path.GetDirectoryName(path);
                    path = Path.Combine(dir, fileName);
                }

                var newEntry = new ZipEntry(path);
                zipOutputStream.PutNextEntry(newEntry);
                var dx9SpmBytes = Encoding.ASCII.GetBytes(convertedSpm); 
                zipOutputStream.Write(dx9SpmBytes, 0, dx9SpmBytes.Length);
                zipOutputStream.CloseEntry();
            }

            var logEntry = new ZipEntry("conversion.txt");
            zipOutputStream.PutNextEntry(logEntry);
            var logBytes = Encoding.ASCII.GetBytes(messageWriter.ToString());
            zipOutputStream.Write(logBytes, 0, logBytes.Length);
            zipOutputStream.CloseEntry();

            zipOutputStream.Finish();
        }

        private static string convertSpm(string sourceSpm, string targetGenerator, bool removeIndex, string path, StringWriter messageWriter)
        {
            try
            {
                if (SpmUtilities.IsCorrupt(sourceSpm))
                {
                    messageWriter.WriteLine(path + " appears to be corrupt. Try exporting the model again or using a different SD card before converting.");
                    return null;
                }
/*
                bool isAirplaneOrHelicopter = SpmUtilities.IsAirplane(sourceSpm) || SpmUtilities.IsHelicopter(sourceSpm);
                if (!isAirplaneOrHelicopter)
                {
                    messageWriter.WriteLine(path + " is not an Airplane/Helicopter (Sailplanes are not currently supported)");
                    return null;
                }
*/
                string sourceGenerator = SpmUtilities.GetGenerator(sourceSpm);

                string convertedSpm;
                switch (targetGenerator)
                {
                    case "DX8":
                        {
                            // HACK: If source model is DX7s convert to DX9 first.
                            if (sourceGenerator == "DX7S")
                            {
                                sourceGenerator = "DX9";
                                sourceSpm = SpmConvert.DX8To(sourceSpm, generator: sourceGenerator);
                            }

                            if (sourceGenerator != "DX8G2" && sourceGenerator != "DX9" && sourceGenerator != "DX18")
                            {
                                messageWriter.WriteLine(path + " is not a DX9/DX18 model file");
                                return null;
                            }

                            string fileName = Path.GetFileName(path);
                            convertedSpm = SpmConvert.DX9To(sourceSpm);
                        }
                        break;
                    case "DX9":
                    case "DX18":
                        {
                            if (sourceGenerator != "DX8" && sourceGenerator != "DX7S")
                            {
                                messageWriter.WriteLine(path + " is not a DX8/DX7s model file");
                                return null;
                            }

                            string masterVolume = targetGenerator == "DX9" ? "20" : null;   // If targeting DX9 set volume to a mellow 20%.
                            string fileName = Path.GetFileName(path);
                            int modelNumber = SpmUtilities.GetModelNumberFromFilename(fileName);
                            if (!removeIndex && modelNumber != -1)
                            {
                                string modelName = modelNumber + ": " + SpmUtilities.GetModelName(sourceSpm);
                                convertedSpm = SpmConvert.DX8To(sourceSpm, generator: targetGenerator, modelName: modelName, masterVolume: masterVolume);
                            }
                            else
                            {
                                convertedSpm = SpmConvert.DX8To(sourceSpm, generator: targetGenerator, masterVolume: masterVolume);
                            }
                        }
                        break;
                    default:
                        messageWriter.WriteLine("Unknown target Tx: " + targetGenerator);
                        return null;
                }

                messageWriter.WriteLine(path + " converted!");
                return convertedSpm;
            }
            catch (Exception e)
            {
                messageWriter.WriteLine(path + " failed to convert: " + e.Message);
                return null;
            }
        }

        private void sendEmail(string contact, string subject, string body, string fileName, byte[] dx8SpmBytes, byte[] dx9SpmBytes, string targetGenerator)
        {
            string toEmailAddress = "jcansdale+spm@gmail.com";
            MailMessage message = new MailMessage(FROM_EMAIL_ADDRESS, toEmailAddress);
            message.Subject = subject;
            message.Body = "Contact: " + contact + "\n\n" + body;
            if (dx8SpmBytes != null)
            {
                var dx8Attachment = new Attachment(new MemoryStream(dx8SpmBytes), fileName);
                message.Attachments.Add(dx8Attachment);
            }
            if (dx9SpmBytes != null)
            {
                var dx9Attachment = new Attachment(new MemoryStream(dx9SpmBytes), targetGenerator + "_" + fileName);
                message.Attachments.Add(dx9Attachment);
            }
            sendEmail(message);
        }

        const string FROM_EMAIL_ADDRESS = "SPM Converter <spm@mutantdesign.co.uk>";

        static void sendEmail(MailMessage message)
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("spm@mutantdesign.co.uk", "E3*WVqa%2");
            client.Send(message);
        }
    }
}
