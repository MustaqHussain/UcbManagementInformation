using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using UcbManagementInformation.Web.Models;
namespace UcbManagementInformation.Web
{
    /// <summary>
    /// Summary description for UploadFile
    /// </summary>
    public class UploadFileHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.QueryString.AllKeys.Contains("filename")
                && context.Request.QueryString.AllKeys.Contains("uniquefilename") && context.Request.QueryString.AllKeys.Contains("chunk"))   
            {
                // Write to the Upload Folder     
                System.IO.FileInfo fi = new System.IO.FileInfo(@"C:\MyUploads\"
                    + context.Request.QueryString["uniquefilename"]);    
                Stream fs;       
                try      
                {
                    UploadFile LoadingFile = UploadContainer.Instance.MasterList.FirstOrDefault(x => x.Name == context.Request.QueryString["uniquefilename"] + context.Request.QueryString["chunk"]);
                    if (LoadingFile == null)
                    {
                        UploadFile newFile = new UploadFile() { Name = context.Request.QueryString["uniquefilename"] + context.Request.QueryString["chunk"], File = new MemoryStream() };
                        UploadContainer.Instance.MasterList.Add(newFile);
                        fs = newFile.File;
                        
                    }
                    else
                    {
                        fs = LoadingFile.File;
                    }
                    //if (fi.Exists)     
                    //{             
                    //    fs = new System.IO.FileStream(fi.FullName    
                    //        , System.IO.FileMode.Append);      
                    //}       
                    //else      
                    //{         
                    //    fs = new System.IO.FileStream(fi.FullName    
                    //        , System.IO.FileMode.OpenOrCreate      
                    //        , System.IO.FileAccess.Write);     
                    //}            
                    // Insert the data into the file     
                    //byte[] b = new byte[context.Request.InputStream.Length];  
                    //context.Request.InputStream.Read(b, 0, b.Length);
                    byte[] buffer = new byte[4096];
                    int read =0;
                    while ((read = context.Request.InputStream.Read(buffer,0,buffer.Length)) > 0)
                    {
                        fs.Write(buffer, 0, read);
                    }

                    //fs.Write(b, 0, b.Length);          
                    // Check if this is the last data packet.     
                    // If so do additional processing as needed...   
                    if (context.Request.QueryString.AllKeys.Contains("completed") && context.Request.QueryString["completed"]=="1")   
                    {
                        fs.Position = 0;

                        
                        FileStream fs1;
                        
                        if (fi.Exists)     
                        {             
                            fs1 = new System.IO.FileStream(fi.FullName    
                                , System.IO.FileMode.Append);      
                        }       
                        else      
                        {         
                            fs1 = new System.IO.FileStream(fi.FullName    
                                , System.IO.FileMode.OpenOrCreate      
                                , System.IO.FileAccess.Write);     
                        }


                        //decompress
                        
                        ZipFile zf = new ZipFile(fs);
                        foreach (ZipEntry zipEntry in zf)
                        {
                            if (!zipEntry.IsFile)
                            {
                                continue;			// Ignore directories
                            }
                            String entryFileName = zipEntry.Name;
                            
                            byte[] buffer2 = new byte[4096];		// 4K is optimum
                            Stream zipStream = zf.GetInputStream(zipEntry);

                            // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                            // of the file, but does not waste memory.
                            // The "using" will close the stream even if an exception occurs.
                            using (fs1)
                            {
                                StreamUtils.Copy(zipStream, fs1, buffer2);
                            }
                            
                        }
                        
                        fs.Close();
                            

                        UploadContainer.Instance.MasterList.Remove(LoadingFile);
                        if (context.Request.QueryString["finished"] == "1")
                        {
                            // Additional process as required by your application:     
                            // Send as attachment                
                            // or Send to Database etc
                        }
                    }    
                }  
                catch(Exception e)
                { 
                    throw e;
                }
            } 
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}