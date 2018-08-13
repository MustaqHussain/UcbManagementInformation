using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UcbManagementInformation.Server.DataAccess;
using System.Xml;

namespace UcbManagementInformation.Server.RDL2003Engine
{
    public class Chart : IRdlRenderable,IDataModelAware
    {
        string _chartType;
        DataItem _categoryDataItem;
        string _datasetName;
        string _seriesType;
        List<ChartSeries> _seriesItems = new List<ChartSeries>();
        int _sortOrder;
        string _categoryName;
        string _title;
        public DataModel CurrentDataModel { get; set; }
        public Chart(string chartType, DataItem categoryDataItem,string datasetName,string seriesType,List<DataItem> seriesDataItems,int sortOrder,string title,DataModel currentDataModel)
        {
            CurrentDataModel = currentDataModel;
            _title = title;
            _chartType=chartType;
            _categoryDataItem=categoryDataItem;
            _datasetName=datasetName;
            _seriesType=seriesType;
            _sortOrder = sortOrder;
            string categoryTableName = CurrentDataModel.DataTables.Single(x=>x.Code ==_categoryDataItem.DataTableCode).Name;// DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(_categoryDataItem.DataTableCode).Name;
            _categoryName=categoryTableName + _categoryDataItem.Name;
            
            foreach (DataItem currentItem in seriesDataItems)
            {
                ChartSeries newSeries = new ChartSeries(currentItem, seriesType,_categoryName,CurrentDataModel);
                _seriesItems.Add(newSeries);
            }

            
            
        }

        public void Render2010(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Chart");
            xmlWriter.WriteAttributeString("Name", "Chart"+_sortOrder.ToString());
            xmlWriter.WriteStartElement("ChartCategoryHierarchy");
            xmlWriter.WriteStartElement("ChartMembers");
            xmlWriter.WriteStartElement("ChartMember");
            xmlWriter.WriteStartElement("Group");
            xmlWriter.WriteAttributeString("Name", "Chart"+_sortOrder.ToString()+"_CategoryGroup");
            xmlWriter.WriteStartElement("GroupExpressions");
            string categoryTableName = CurrentDataModel.DataTables.Single(x => x.Code == _categoryDataItem.DataTableCode).Name; // DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(_categoryDataItem.DataTableCode).Name;
            RdlRender.AddLine(xmlWriter,"GroupExpression","=Fields!"+_categoryName+".Value");
            xmlWriter.WriteEndElement();//GroupExpressions
            xmlWriter.WriteEndElement();//Group
            xmlWriter.WriteStartElement("SortExpressions");
            xmlWriter.WriteStartElement("SortExpression");
            RdlRender.AddLine(xmlWriter,"Value","=Fields!"+categoryTableName + _categoryDataItem.Name+".Value");
            xmlWriter.WriteEndElement();//SortExpression
            xmlWriter.WriteEndElement();//SortExpressions
            RdlRender.AddLine(xmlWriter,"Label","=Fields!"+categoryTableName + _categoryDataItem.Name+".Value");
            
            xmlWriter.WriteEndElement();//ChartMember
            xmlWriter.WriteEndElement();//ChartMembers
            xmlWriter.WriteEndElement();//ChartCategoryHierarchy
  
            xmlWriter.WriteStartElement("ChartSeriesHierarchy");
            xmlWriter.WriteStartElement("ChartMembers");
            
            foreach(ChartSeries seriesItem in _seriesItems)
            {
                xmlWriter.WriteStartElement("ChartMember");
                RdlRender.AddLine(xmlWriter,"Label",seriesItem.SeriesDataItem.Caption);
                xmlWriter.WriteEndElement();//ChartMember
            }

            xmlWriter.WriteEndElement();//ChartMembers
            xmlWriter.WriteEndElement();//ChartSeriesHierarchy
 
            xmlWriter.WriteStartElement("ChartData");
            xmlWriter.WriteStartElement("ChartSeriesCollection");

            foreach(ChartSeries seriesItem in _seriesItems)
            {
                seriesItem.Render2010(xmlWriter);
            }

            xmlWriter.WriteEndElement();//ChartSeriesCollection
            xmlWriter.WriteEndElement();//ChartData

            xmlWriter.WriteStartElement("ChartAreas");
            xmlWriter.WriteStartElement("ChartArea");
            xmlWriter.WriteAttributeString("Name", "Default");
            xmlWriter.WriteStartElement("ChartCategoryAxes");
            xmlWriter.WriteStartElement("ChartAxis");
            xmlWriter.WriteAttributeString("Name", _categoryName);
            
            xmlWriter.WriteStartElement("Style");
            RdlRender.AddLine(xmlWriter,"FontSize","8pt");
            xmlWriter.WriteEndElement();//Style
            xmlWriter.WriteStartElement("ChartAxisTitle");
            RdlRender.AddLine(xmlWriter,"Caption",_categoryDataItem.Caption);
            xmlWriter.WriteStartElement("Style");
            RdlRender.AddLine(xmlWriter,"FontSize","8pt");
            xmlWriter.WriteEndElement();//Style
            
            xmlWriter.WriteEndElement();//ChartAxisTitle
            RdlRender.AddLine(xmlWriter,"Interval","1");
            RdlRender.AddLine(xmlWriter,"IntervalType","Default");
            xmlWriter.WriteStartElement("ChartMajorGridLines");
            RdlRender.AddLine(xmlWriter,"Enabled","False");
            xmlWriter.WriteStartElement("Style");
            xmlWriter.WriteStartElement("Border");
            RdlRender.AddLine(xmlWriter,"Color","Gainsboro");
            
            xmlWriter.WriteEndElement();//Border
            xmlWriter.WriteEndElement();//Style
            
            xmlWriter.WriteEndElement();//ChartMajorGridLines
            
            xmlWriter.WriteStartElement("ChartMinorGridLines");
            RdlRender.AddLine(xmlWriter,"Enabled","False");
            xmlWriter.WriteStartElement("Style");
            xmlWriter.WriteStartElement("Border");
            RdlRender.AddLine(xmlWriter,"Color","Gainsboro");
            RdlRender.AddLine(xmlWriter,"Style","Dotted");
            
            xmlWriter.WriteEndElement();//Border
            xmlWriter.WriteEndElement();//Style
            
            xmlWriter.WriteEndElement();//ChartMinorGridLines

            xmlWriter.WriteStartElement("ChartMajorTickMarks");
            RdlRender.AddLine(xmlWriter,"IntervalType","Auto");
            xmlWriter.WriteEndElement();//ChartMajorTickMarks
            xmlWriter.WriteStartElement("ChartMinorTickMarks");
            RdlRender.AddLine(xmlWriter,"Length","0.5");
            xmlWriter.WriteEndElement();//ChartMinorTickMarks
            RdlRender.AddLine(xmlWriter,"CrossAt","NaN");
            RdlRender.AddLine(xmlWriter,"Minimum","NaN");
            RdlRender.AddLine(xmlWriter,"Maximum","NaN");
            RdlRender.AddLine(xmlWriter,"Angle","-90");
            RdlRender.AddLine(xmlWriter,"LabelsAutoFitDisabled","true");
            xmlWriter.WriteStartElement("ChartAxisScaleBreak");
            RdlRender.AddLine(xmlWriter,"Style","");
            xmlWriter.WriteEndElement();//ChartAxisScaleBreak
            xmlWriter.WriteEndElement();//ChartAxis
            
            xmlWriter.WriteEndElement();//ChartCategoryAxes
            xmlWriter.WriteStartElement("ChartValueAxes");
            foreach(ChartSeries currentSeries in _seriesItems)
            {
                currentSeries.RenderAxes2010(xmlWriter);
            }
            xmlWriter.WriteEndElement();//ChartValueAxes
            
            xmlWriter.WriteStartElement("ChartThreeDProperties");
            RdlRender.AddLine(xmlWriter,"Enabled","true");
            RdlRender.AddLine(xmlWriter,"ProjectionMode","Perspective");
            RdlRender.AddLine(xmlWriter,"Rotation","7");
            RdlRender.AddLine(xmlWriter,"Inclination","15");
            RdlRender.AddLine(xmlWriter,"DepthRatio","200");
            RdlRender.AddLine(xmlWriter,"WallThickness","2");
            RdlRender.AddLine(xmlWriter,"Clustered","true");
            
            xmlWriter.WriteEndElement();//ChartThreeDProperties
            
            xmlWriter.WriteStartElement("Style");
            RdlRender.AddLine(xmlWriter,"BackgroundColor","White");
            RdlRender.AddLine(xmlWriter,"BackgroundGradientType","None");
            RdlRender.AddLine(xmlWriter,"BackgroundGradientEndColor","#00ffffff");
            RdlRender.AddLine(xmlWriter,"BackgroundHatchType","None");
            RdlRender.AddLine(xmlWriter,"ShadowOffset","0pt");
            
            xmlWriter.WriteEndElement();//Style
            xmlWriter.WriteEndElement();//ChartArea
            xmlWriter.WriteEndElement();//ChartAreas

            xmlWriter.WriteStartElement("ChartLegends");
            xmlWriter.WriteStartElement("ChartLegend");
            xmlWriter.WriteAttributeString("Name", "Default");
            xmlWriter.WriteStartElement("Style");
            RdlRender.AddLine(xmlWriter,"BackgroundColor","#ffffff");
            RdlRender.AddLine(xmlWriter,"BackgroundGradientType","None");
            RdlRender.AddLine(xmlWriter,"FontSize","8pt");
            xmlWriter.WriteEndElement();//Style
            xmlWriter.WriteStartElement("ChartLegendTitle");
            xmlWriter.WriteElementString("Caption", null);
            
            //RdlRender.AddLine(xmlWriter,"Caption","");
             xmlWriter.WriteStartElement("Style");
            RdlRender.AddLine(xmlWriter,"FontSize","8pt");
            RdlRender.AddLine(xmlWriter,"FontWeight","Bold");
            RdlRender.AddLine(xmlWriter,"TextAlign","Center");
            xmlWriter.WriteEndElement();//Style
            xmlWriter.WriteEndElement();//ChartLegendTitle
            RdlRender.AddLine(xmlWriter,"HeaderSeparatorColor","#cccccc");
            RdlRender.AddLine(xmlWriter,"ColumnSeparatorColor","#cccccc");
            
            xmlWriter.WriteEndElement();//ChartLegend
            xmlWriter.WriteEndElement();//ChartLegends
           
            xmlWriter.WriteStartElement("ChartTitles");
            xmlWriter.WriteStartElement("ChartTitle");
            xmlWriter.WriteAttributeString("Name", "Default");
            RdlRender.AddLine(xmlWriter,"Caption",_title);
             xmlWriter.WriteStartElement("Style");
            RdlRender.AddLine(xmlWriter,"BackgroundGradientType","None");
            RdlRender.AddLine(xmlWriter,"FontWeight","Bold");
            RdlRender.AddLine(xmlWriter,"TextAlign","General");
            RdlRender.AddLine(xmlWriter,"VerticalAlign","Top");
            RdlRender.AddLine(xmlWriter, "Color", "#eaf5f7");
            xmlWriter.WriteEndElement();//Style
            
            xmlWriter.WriteEndElement();//ChartTitle
            xmlWriter.WriteEndElement();//ChartTitles
           
            RdlRender.AddLine(xmlWriter,"Palette","BrightPastel");
            xmlWriter.WriteStartElement("ChartBorderSkin");
            RdlRender.AddLine(xmlWriter,"ChartBorderSkinType","FrameTitle1");
             xmlWriter.WriteStartElement("Style");
            RdlRender.AddLine(xmlWriter,"BackgroundColor","#005db3");
            RdlRender.AddLine(xmlWriter,"BackgroundGradientType","None");
            RdlRender.AddLine(xmlWriter,"Color","#eaf5f7");
            xmlWriter.WriteEndElement();//Style
            xmlWriter.WriteEndElement();//ChartBorderSkin
            xmlWriter.WriteStartElement("ChartNoDataMessage");
            xmlWriter.WriteAttributeString("Name", "NoDataMessage");
            RdlRender.AddLine(xmlWriter,"Caption","No Data Available");
             xmlWriter.WriteStartElement("Style");
            RdlRender.AddLine(xmlWriter,"BackgroundGradientType","None");
            RdlRender.AddLine(xmlWriter,"TextAlign","General");
            RdlRender.AddLine(xmlWriter,"VerticalAlign","Top");
            xmlWriter.WriteEndElement();//Style
            
            xmlWriter.WriteEndElement();//ChartNoDataMessage
            RdlRender.AddLine(xmlWriter,"DataSetName",_datasetName);
             xmlWriter.WriteStartElement("PageBreak");
            RdlRender.AddLine(xmlWriter,"BreakLocation","Start");
            xmlWriter.WriteEndElement();//PageBreak
            RdlRender.AddLine(xmlWriter,"Top",(1.73825 + (8 * _sortOrder)).ToString() + "in");// To make charts go below each other "1.73825in");
            RdlRender.AddLine(xmlWriter,"Left","0.5in");
            RdlRender.AddLine(xmlWriter,"Height","7.46875in");
            RdlRender.AddLine(xmlWriter,"Width","7.2492in");
            RdlRender.AddLine(xmlWriter,"ZIndex","1");
             xmlWriter.WriteStartElement("Style");
             xmlWriter.WriteStartElement("Border");
            RdlRender.AddLine(xmlWriter,"Color","LightGrey");
            RdlRender.AddLine(xmlWriter,"Style","Solid");
            xmlWriter.WriteEndElement();//Border
            RdlRender.AddLine(xmlWriter,"BackgroundColor","#ffffff");
            RdlRender.AddLine(xmlWriter,"BackgroundGradientType","None");
            RdlRender.AddLine(xmlWriter,"BackgroundGradientEndColor","#ffffff");
            xmlWriter.WriteEndElement();//Style
            
            xmlWriter.WriteEndElement();//Chart

        }
    }
}
