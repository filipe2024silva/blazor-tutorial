using System.Runtime.InteropServices;
using System.Text.Json;
using BoldReports.Data.WebData;
using BoldReports.Web;
using BoldReports.Web.ReportViewer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace f10.pulsar.mes.Controllers
{
    [Route("api/[controller]/[action]/{id?}")]
    public class ReportViewerController : ControllerBase, IReportController
    {
        // Report viewer requires a memory cache to store the information of consecutive client requests and
        // the rendered report viewer in the server.
        private IMemoryCache _cache;

        // IWebHostEnvironment used with sample to get the application data from wwwroot.
        private IWebHostEnvironment _hostingEnvironment;
        private IReportDataService _reportDataService;

        private int _idTratamento = -1;


        public ReportViewerController(IMemoryCache memoryCache, IWebHostEnvironment hostingEnvironment, IReportDataService reportDataService)
        {
            _cache = memoryCache;
            _hostingEnvironment = hostingEnvironment;
            _reportDataService = reportDataService;
        }

        //Get action for getting resources from the report
        [ActionName("GetResource")]
        [AcceptVerbs("GET")]
        // Method will be called from Report Viewer client to get the image src for Image report item.
        public object GetResource(ReportResource resource)
        {
            return ReportHelper.GetResource(resource, this, _cache);
        }

        // Method will be called to initialize the report information to load the report with ReportHelper for processing.
        [NonAction]
        public async void OnInitReportOptions(ReportViewerOptions reportOption)
        {
            try
            {
                string basePath = _hostingEnvironment.WebRootPath;
                //reportOption.ReportModel.ProcessingMode = BoldReports.Web.ReportViewer.ProcessingMode.Local;
                // Here, we have loaded the sales-order-detail.rdl report from application the folder wwwroot\Resources. sales-order-detail.rdl should be there in
                //wwwroot\Resources application folder.

                string pathCompleted = string.Empty;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    pathCompleted = $"{basePath}\\Resources\\{reportOption.ReportModel.ReportPath}";
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    pathCompleted = $"{basePath}/Resources/{reportOption.ReportModel.ReportPath}";
                }

                FileStream inputStream = new FileStream(pathCompleted, FileMode.Open, FileAccess.Read);

                MemoryStream reportStream = new MemoryStream();
                inputStream.CopyTo(reportStream);
                reportStream.Position = 0;
                inputStream.Close();
                reportOption.ReportModel.Stream = reportStream;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        // Method will be called when report is loaded internally to start the layout process with ReportHelper.
        [NonAction]
        public void OnReportLoaded(ReportViewerOptions reportOption)
        {
            try
            {
                //não dar erro por faltar dados nos datasets passa a vazio
                Dictionary<string, List<RetificadoresValues>> stringVazia = new();

                List<DataSourceInfo> datasources = ReportHelper.GetDataSources(_jsonResult, this, _cache);

                var jsonResult = _reportDataService.GetReportInfo(_idTratamento);

                string jsonCabecalho = JsonConvert.SerializeObject(jsonResult.Result.Item1);
                string jsonRoteiro = JsonConvert.SerializeObject(jsonResult.Result.Item2);

                var jsonRetificadoresValues = jsonResult.Result.Item3;

                var credentialsList = new List<DataSourceCredentials>();

                //A ideia foi deixar dinâmico para  tratar as várias tinas com retificadores 
                //é construido o dicionário do onde a key é o nome dos datasources do report e o value os valores 
                foreach (DataSourceInfo item in datasources)
                {
                    var model = new FileDataModel
                    {
                        DataMode = "inline"
                    };

                    item.DataProvider = "JSON";

                    if (item.DataSourceName == "cabecalho")
                    {
                        model.Data = jsonCabecalho;
                    }
                    else if (item.DataSourceName == "roteiro")
                    {
                        model.Data = jsonRoteiro;
                    }
                    else if (jsonRetificadoresValues.Keys.Any(key => key.Contains(item.DataSourceName, StringComparison.OrdinalIgnoreCase))) //verifica se existe algum o datasource do report com o mesmo nome da key dos dicionários
                    {
                        var matchingKey = jsonRetificadoresValues.Keys.FirstOrDefault(key => key.Contains(item.DataSourceName, StringComparison.OrdinalIgnoreCase));

                        if (matchingKey != null && jsonRetificadoresValues.TryGetValue(matchingKey, out var existingEntry))
                        {
                            model.Data = JsonConvert.SerializeObject(existingEntry);
                        }
                        else
                        {
                            model.Data = JsonConvert.SerializeObject(stringVazia);
                        }
                    }
                    else
                    {
                        model.Data = JsonConvert.SerializeObject(stringVazia);
                    }

                    var credentials = new DataSourceCredentials
                    {
                        Name = item.DataSourceName,
                        UserId = null,
                        Password = null,
                        ConnectionString = JsonConvert.SerializeObject(model),
                        IntegratedSecurity = false
                    };

                    credentialsList.Add(credentials);
                }

                reportOption.ReportModel.DataSourceCredentials = credentialsList;

            }
            catch (HttpRequestException exHttp)
            {
                Console.WriteLine(exHttp.Message);
            }

        }

        [HttpPost]
        public object PostFormReportAction()
        {
            return ReportHelper.ProcessReport(null, this, _cache);
        }

        public Dictionary<string, object> _jsonResult;

        // Post action to process the report from server based json parameters and send the result back to the client.
        [HttpPost]
        public object PostReportAction([FromBody] Dictionary<string, object> jsonArray)
        {
            _jsonResult = jsonArray;
            //aqui temos de pegar nos parametros que vem do body, o Report exige que venho como array daí ter de ser tratado desta maneira
            if (jsonArray.TryGetValue("parameters", out var parameters) && parameters is JsonElement jsonElement)
            {
                _idTratamento = jsonElement.EnumerateArray()
                    .FirstOrDefault(p => p.GetProperty("Name").GetString() == "idTratamento")
                    .GetProperty("Values") is JsonElement valuesElement
                    ? valuesElement.ValueKind == JsonValueKind.Array ? valuesElement[0].GetInt32() : valuesElement.GetInt32()
                    : 0;
            }

            return ReportHelper.ProcessReport(jsonArray, this, _cache);
        }

    }
}

