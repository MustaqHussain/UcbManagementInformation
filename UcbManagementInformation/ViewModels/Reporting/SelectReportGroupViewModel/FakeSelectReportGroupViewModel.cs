using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using UcbManagementInformation.MVVM;
using UcbManagementInformation.Commanding;
using System.Collections.ObjectModel;
using UcbManagementInformation.Models;
using UcbManagementInformation.Web.Models;

namespace UcbManagementInformation.ViewModels
{
    public class FakeSelectReportGroupViewModel : ViewModel,ISelectReportGroupViewModel
    {

        #region Private fields
        private static bool? dialogResult;
        private static RelayCommand okCommand;
        private static string reportName;
        private static ObservableCollection<ReportGroupFolder> rootGroups;
        private static ReportGroupFolder selectedReportGroup;
        #endregion

        #region Public properties
        public bool? DialogResult
        {
            get
            {
                return dialogResult;
            }
            set
            {
                dialogResult = value;
            }
        }

        public RelayCommand OKCommand
        {
            get
            {
                return okCommand;
            }
            set
            {
                okCommand = value;
            }
        }

        public string ReportName
        {
            get
            {
                return reportName;
            }
            set
            {
                reportName = value;
            }
        }

        public ObservableCollection<ReportGroupFolder> RootGroups
        {
            get
            {
                return rootGroups;
            }
            set
            {
                rootGroups = value;
            }
        }

        public ReportGroupFolder SelectedReportGroup
        {
            get
            {
                return selectedReportGroup;
            }
            set
            {
                selectedReportGroup = value;
            }
        }
        #endregion

        static FakeSelectReportGroupViewModel()
        {
            reportName = "TestReportName";
            rootGroups = new ObservableCollection<ReportGroupFolder>()
            {


                new ReportGroupFolder
                {
                    Name="Group1",
                    AccessLevel=ReportGroupAccessLevelType.Update,
                    Color="Green",
                    Children = new ObservableCollection<ReportGroupFolder>()
                    {
                        new ReportGroupFolder
                        {
                            Name="Group1a",
                            AccessLevel=ReportGroupAccessLevelType.Update,
                            Color="Green"
                        },
                        new ReportGroupFolder
                        {
                            Name="Group1b",
                            AccessLevel=ReportGroupAccessLevelType.Update,
                            Color="Green"
                        },
                        new ReportGroupFolder
                        {
                            Name="Group1c",
                            AccessLevel=ReportGroupAccessLevelType.Update,
                            Color="Green"
                        }
                    }

                },
                new ReportGroupFolder
                {
                    Name="Group2",
                    Color="Yellow"

                },
                new ReportGroupFolder
                {
                    Name="Group3",
                    AccessLevel=ReportGroupAccessLevelType.Update,
                    Color="Green"

                },
                new ReportGroupFolder
                {
                    Name="Group4",
                    AccessLevel=ReportGroupAccessLevelType.Update,
                    Color="Green"

                },
                new ReportGroupFolder
                {
                    Name="Group5",
                    AccessLevel=ReportGroupAccessLevelType.Update,
                    Color="Green"

                },
                new ReportGroupFolder
                {
                    Name="Group6",
                    AccessLevel=ReportGroupAccessLevelType.Update,
                    Color="Green"

                },
                     new ReportGroupFolder
                {
                    Name="Group5",
                    AccessLevel=ReportGroupAccessLevelType.Update,
                    Color="Green"

                },
                     new ReportGroupFolder
                {
                    Name="Group5",
                    AccessLevel=ReportGroupAccessLevelType.Update,
                    Color="Green"

                },
                     new ReportGroupFolder
                {
                    Name="Group5",
                    AccessLevel=ReportGroupAccessLevelType.Update,
                    Color="Green"

                },
                     new ReportGroupFolder
                {
                    Name="Group5",
                    AccessLevel=ReportGroupAccessLevelType.Update,
                    Color="Green"

                },
                     new ReportGroupFolder
                {
                    Name="Group5",
                    AccessLevel=ReportGroupAccessLevelType.Update,
                    Color="Green"

                },
                     new ReportGroupFolder
                {
                    Name="Group5",
                    AccessLevel=ReportGroupAccessLevelType.Update,
                    Color="Green"

                },
                
            };
        }
    }
}
