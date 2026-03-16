// Para isolar o css aplicado no report tive de isolar num Iframe pois aplicava na restante página
window.BoldReports = {
    RenderViewerInIframe: function (iframeID, reportViewerOptions) {
        //console.log("Inicializando Report Viewer dentro do iframe...");
        //console.log('ReportServiveUrl',reportViewerOptions.reportServiceUrl);
        //console.log("ReportPath", reportViewerOptions.reportPath);
        //console.log("Parameters", reportViewerOptions.idtratamento);

        let iframe = document.getElementById(iframeID);
        if (!iframe) {
            console.error(`Elemento com ID '${iframeID}' não encontrado.`);
            return;
        }

        let iframeDoc = iframe.contentDocument || iframe.contentWindow.document;
        iframeDoc.open();
        iframeDoc.write(`
            <html>
            <head>
                <link href="https://cdn.boldreports.com/7.1.9/content/v2.0/tailwind-light/bold.report-viewer.min.css" rel="stylesheet" /> 

	            <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
	            <script src="https://cdn.boldreports.com/7.1.9/scripts/v2.0/common/bold.reports.common.min.js"></script>
	            <script src="https://cdn.boldreports.com/7.1.9/scripts/v2.0/common/bold.reports.widgets.min.js"></script>
	            <script src="https://cdn.boldreports.com/7.1.9/scripts/v2.0/bold.report-viewer.min.js"></script>
            </head>
            <body>
                <div id="boldReportViewer" style="width:100%; height:100%;"></div>
                <script>
                    document.addEventListener("DOMContentLoaded", function () {
                        $("#boldReportViewer").boldReportViewer({
                            reportPath: "${reportViewerOptions.reportPath}",
                            reportServiceUrl: "${reportViewerOptions.reportServiceUrl}",
                            parameters: [{ Name: "idTratamento", Values: [${reportViewerOptions.idTratamento}] }] 
                        });
                    });
                </script>
            </body>
            </html>
        `);
        iframeDoc.close();
        console.log("Report Viewer carregado no iframe.");
    }
};


//---------------------------------- Versão sem IFrame aplica e remove css quando entra e sai do tab Relatório ------------------------------
//window.BoldReports = {
//    RenderViewer: function (elementID, reportViewerOptions) {
//        let link = document.createElement("link");
//        link.rel = "stylesheet";
//        link.href = "https://cdn.boldreports.com/7.1.9/content/v2.0/tailwind-light/bold.report-viewer.min.css"; // URL correta dos estilos
//        link.id = "boldReportsStyle";

//        let container = document.getElementById(elementID);
//        if (!document.getElementById("boldReportsStyle")) {
//            container.appendChild(link);
//        }

//        $("#" + elementID).boldReportViewer({
//            reportPath: reportViewerOptions.reportPath,
//            reportServiceUrl: reportViewerOptions.reportServiceUrl
//        });
//    }
//};

//window.BoldReports.RemoveViewer = function () {
//    let style = document.getElementById("boldReportsStyle");
//    if (style) {
//        style.remove();
//    }
//};



