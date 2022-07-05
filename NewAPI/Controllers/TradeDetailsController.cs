using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NewAPI.Models;
using System.Web;
using System.IO;
using NewAPI.DTOs;
using System.Net.Http.Headers;
using System.Web.Hosting;
namespace NewAPI.Controllers
{
    [RoutePrefix("API/TradeDetails")]  
    public class TradeDetailsController : ApiController
    {
        [HttpPost]  
        [Route("AddTradeDetails")]
        public IHttpActionResult AddFile()
        {
            string result = "";
            try
            {
                var objEntity = new crudEntities();
                var objFile = new Trade();
                string testPlanName = null;
                string syllabusfileName = null;
                var httpRequest = HttpContext.Current.Request;
                var syllabusFile = httpRequest.Files["SyllabusFileUpload"];
                var testPlanFile = httpRequest.Files["TestPlanFileUpload"];
                objFile.TradeName = httpRequest.Form["TradeName"];
                objFile.TradeLevel = httpRequest.Form["TradeLevel"];
                objFile.Languages = httpRequest.Form["Languages"];
                objFile.DevelopmentOfficer = httpRequest.Form["DevelopmentOfficer"];
                objFile.SyllabusName = httpRequest.Form["SyllabusName"];
                objFile.Manager = httpRequest.Form["Manager"];
                
                if (string.IsNullOrWhiteSpace(objFile.TradeName))
                {
                    result = "Pleae select a trade";
                    return Ok(result);
                }
                if (string.IsNullOrWhiteSpace(objFile.TradeName))
                {
                    result = "Pleae select a level";
                    return Ok(result);
                }
                if (string.IsNullOrWhiteSpace(objFile.TradeLevel))
                {
                    result = "Pleae select a TradeLevel";
                    return Ok(result);
                }
                
                if (string.IsNullOrWhiteSpace(objFile.Languages))
                {
                    result = "Pleae select a Language";
                    return Ok(result);
                }
                if (string.IsNullOrWhiteSpace(objFile.DevelopmentOfficer))
                {
                    result = "Pleae select a DevelopmentOfficer";
                    return Ok(result);
                }
                if (string.IsNullOrWhiteSpace(objFile.Manager))
                {
                    result = "Pleae select a Manager";
                    return Ok(result);
                }
                if (syllabusFile == null)
                {
                    result = "Pleae select a syllabus";
                    return Ok(result);
                }
                if (testPlanFile == null)
                {
                    result = "Pleae select a testPlan";
                    return Ok(result);
                }
                if (string.IsNullOrEmpty(httpRequest.Form["ActiveDate"]))
                {
                    result = "Pleae select a active date";
                    return Ok(result);
                }
                objFile.ActiveDate = DateTime.Parse(httpRequest.Form["ActiveDate"]);

                if (testPlanFile != null)
                {
                    testPlanName = new String(Path.GetFileNameWithoutExtension(testPlanFile.FileName).Take(10).ToArray()).Replace(" ", "-");
                    testPlanName = testPlanName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(testPlanFile.FileName);
                    var filePath = HttpContext.Current.Server.MapPath("~/FileUpload/" + testPlanName);
                    testPlanFile.SaveAs(filePath);
                }
                if (syllabusFile != null)
                {
                    syllabusfileName = new String(Path.GetFileNameWithoutExtension(syllabusFile.FileName).Take(10).ToArray()).Replace(" ", "-");
                    syllabusfileName = syllabusfileName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(syllabusFile.FileName);
                    var filePath = HttpContext.Current.Server.MapPath("~/FileUpload/" + syllabusfileName);
                    syllabusFile.SaveAs(filePath);
                }
                objFile.TestPlanFilePath = testPlanName;
                objFile.SyllabusFilePath = syllabusfileName;
                objFile.SyllabusName = syllabusfileName;
                objEntity.Trades.Add(objFile);
                int i = objEntity.SaveChanges();
                if (i > 0)
                {
                    result = "File saved sucessfully";
                }
                else
                {
                    result = "File uploaded faild";
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(result);
        }
        [HttpGet]
        [Route("GetDetails")]
        public IHttpActionResult GetFile()
        {
            var url = HttpContext.Current.Request.Url;
            IEnumerable<TradeDTO> lstFile = new List<TradeDTO>();
            try
            {
                crudEntities objEntity = new crudEntities();
                lstFile = objEntity.Trades.Select(a => new TradeDTO
                {
                    ID = a.ID,
                    TradeName = a.TradeName,
                    TradeLevel = a.TradeLevel,
                    ActiveDate = a.ActiveDate,
                    DevelopmentOfficer = a.DevelopmentOfficer,
                    Manager = a.Manager,
                    Languages = a.Languages,
                    SyllabusName = a.SyllabusName,
                    SyllabusFile = url.Scheme + "://" + url.Host + ":" + url.Port + "/FileUpload/" + a.SyllabusFilePath,
                    TestPlanFile = url.Scheme + "://" + url.Host + ":" + url.Port + "/FileUpload/" + a.TestPlanFilePath,
                    SyllabusFilePath = a.SyllabusFilePath,
                    TestPlanFilePath = a.TestPlanFilePath
                }).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(lstFile);
        }

        [HttpGet]
        [Route("GetFile")]
        //download file api  
        public HttpResponseMessage GetFile(string pdfFile)
        {
            
            //Create HTTP Response.  
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            var pdfLocation = HostingEnvironment.MapPath("~/FileUpload/" + pdfFile);

            try
            {
              var stream = new MemoryStream(File.ReadAllBytes(pdfLocation));
              stream.Position = 0;
              response.Content = new StreamContent(stream);
              response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
              response.Content.Headers.ContentDisposition.FileName = pdfFile + ".pdf";
              response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
              response.Content.Headers.ContentLength = stream.Length;
            }catch(Exception e)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                
            }
            return response;
        }

        [HttpDelete]
        [Route("Delete")]
        //download file api  
        public string Delete(int  ID)
        {
            //Create HTTP Response.  
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            //Set the File Path.  
            var objEntity = new crudEntities();
            var trade = objEntity.Trades.FirstOrDefault(t => t.ID == ID);
            if (trade == null)
            {
                return "No data found to delete!";
            }
            objEntity.Trades.Remove(trade);
            objEntity.SaveChanges();
            return "Delete successful";
        } 
    }
}
