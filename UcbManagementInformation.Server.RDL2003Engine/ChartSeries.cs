using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UcbManagementInformation.Server.DataAccess;
using System.Xml;

namespace UcbManagementInformation.Server.RDL2003Engine
{
    public class ChartSeries : IRdlRenderable,IDataModelAware
    {
        string _seriesType;
        string _seriesName;
        string _categoryName;
        public DataItem SeriesDataItem { get; set; }
        public DataModel CurrentDataModel { get; set; }
        public ChartSeries(DataItem seriesDataItem, string seriesType, string categoryName,DataModel currentDataModel)
        {
            CurrentDataModel = currentDataModel;
            _categoryName = categoryName;
            SeriesDataItem = seriesDataItem;
            _seriesType = seriesType;
            string seriesTableName = CurrentDataModel.DataTables.Single(x => x.Code == seriesDataItem.DataTableCode).Name;//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(seriesDataItem.DataTableCode).Name;
            _seriesName = seriesTableName + seriesDataItem.Name;
        }

        public void Render2010(XmlWriter xmlWriter)
        { 
            xmlWriter.WriteStartElement("ChartSeries");
            xmlWriter.WriteAttributeString("Name",SeriesDataItem.Name);
            xmlWriter.WriteStartElement("ChartDataPoints");
            xmlWriter.WriteStartElement("ChartDataPoint");
            xmlWriter.WriteStartElement("ChartDataPointValues");
            
            RdlRender.AddLine(xmlWriter,"X","=Fields!" + _categoryName+ ".Value");
            RdlRender.AddLine(xmlWriter, "Y", "=Sum(Fields!" + _seriesName + ".Value)");
            

            xmlWriter.WriteEndElement();//ChartDataPointValues
  
            xmlWriter.WriteStartElement("ChartDataLabel");
            RdlRender.AddLine(xmlWriter,"Style","");
            xmlWriter.WriteEndElement();//ChartDataLabel

            RdlRender.AddLine(xmlWriter, "ToolTip", "=FormatNumber(Sum(Fields!" + _seriesName + ".Value),0,true,false,true)");
            
            xmlWriter.WriteStartElement("Style");
            RdlRender.AddLine(xmlWriter,"BackgroundGradientType","None");
            xmlWriter.WriteEndElement();//Style
            xmlWriter.WriteStartElement("ChartMarker");
            RdlRender.AddLine(xmlWriter,"Style","");
            xmlWriter.WriteEndElement();//ChartMarker
            RdlRender.AddLine(xmlWriter,"DataElementOutput","Output");
            xmlWriter.WriteEndElement();//ChartDataPoint
            xmlWriter.WriteEndElement();//ChartDataPoints
            
            xmlWriter.WriteStartElement("Style");
            RdlRender.AddLine(xmlWriter,"ShadowOffset","0pt");
            xmlWriter.WriteEndElement();//Style
            
            xmlWriter.WriteStartElement("ChartEmptyPoints");
            RdlRender.AddLine(xmlWriter,"Style","");
            xmlWriter.WriteStartElement("ChartMarker");
            RdlRender.AddLine(xmlWriter,"Style","");
            xmlWriter.WriteEndElement();//ChartMarker
             xmlWriter.WriteStartElement("ChartDataLabel");
            RdlRender.AddLine(xmlWriter,"Style","");
            xmlWriter.WriteEndElement();//ChartDataLabel
            xmlWriter.WriteEndElement();//ChartEmptyPoints
            RdlRender.AddLine(xmlWriter, "ValueAxisName", _seriesName);
            RdlRender.AddLine(xmlWriter,"CategoryAxisName",_categoryName);
             xmlWriter.WriteStartElement("ChartSmartLabel");
            RdlRender.AddLine(xmlWriter,"CalloutLineColor","Black");
            RdlRender.AddLine(xmlWriter,"MinMovingDistance","0pt");
            
            xmlWriter.WriteEndElement();//ChartSmartLabel
            xmlWriter.WriteEndElement();//ChartSeries

        }
        public void RenderAxes2010(XmlWriter xmlWriter)
        { 
            xmlWriter.WriteStartElement("ChartAxis");
            xmlWriter.WriteAttributeString("Name", _seriesName);
            
            xmlWriter.WriteStartElement("Style");
            RdlRender.AddLine(xmlWriter,"Format","#,0;(#,0)");
            RdlRender.AddLine(xmlWriter,"FontSize","8pt");
            xmlWriter.WriteEndElement();//Style
            xmlWriter.WriteStartElement("ChartAxisTitle");
            RdlRender.AddLine(xmlWriter,"Caption","Units");
            xmlWriter.WriteStartElement("Style");
            RdlRender.AddLine(xmlWriter,"FontSize","8pt");
            xmlWriter.WriteEndElement();//Style
            
            xmlWriter.WriteEndElement();//ChartAxisTitle
            //RdlRender.AddLine(xmlWriter,"Interval","1");
            RdlRender.AddLine(xmlWriter,"IntervalType","Default");
            //xmlWriter.WriteStartElement("ChartMajorGridLines");
            //RdlRender.AddLine(xmlWriter,"Enabled","False");
            //xmlWriter.WriteStartElement("Style");
            //xmlWriter.WriteStartElement("Border");
            //RdlRender.AddLine(xmlWriter,"Color","Gainsboro");
            
            //xmlWriter.WriteEndElement();//Border
            //xmlWriter.WriteEndElement();//Style
            
            //xmlWriter.WriteEndElement();//ChartMajorGridLines
            
            xmlWriter.WriteStartElement("ChartMinorGridLines");
            RdlRender.AddLine(xmlWriter,"Enabled","False");
            xmlWriter.WriteStartElement("Style");
            xmlWriter.WriteStartElement("Border");
            RdlRender.AddLine(xmlWriter,"Color","Gainsboro");
            RdlRender.AddLine(xmlWriter,"Style","Dotted");
            
            xmlWriter.WriteEndElement();//Border
            xmlWriter.WriteEndElement();//Style
            
            xmlWriter.WriteEndElement();//ChartMinorGridLines

            //xmlWriter.WriteStartElement("ChartMajorTickMarks");
            //RdlRender.AddLine(xmlWriter,"IntervalType","Auto");
            //xmlWriter.WriteEndElement();//ChartMajorTickMarks
            xmlWriter.WriteStartElement("ChartMinorTickMarks");
            RdlRender.AddLine(xmlWriter,"Length","0.5");
            RdlRender.AddLine(xmlWriter,"IntervalType","Auto");
            xmlWriter.WriteEndElement();//ChartMinorTickMarks
            RdlRender.AddLine(xmlWriter,"CrossAt","NaN");
            RdlRender.AddLine(xmlWriter,"Interlaced","true");
            RdlRender.AddLine(xmlWriter,"Minimum","NaN");
            RdlRender.AddLine(xmlWriter,"Maximum","NaN");
            xmlWriter.WriteStartElement("ChartAxisScaleBreak");
            RdlRender.AddLine(xmlWriter,"Style","");
            xmlWriter.WriteEndElement();//ChartAxisScaleBreak
            xmlWriter.WriteEndElement();//ChartAxis
        }
    }
}
