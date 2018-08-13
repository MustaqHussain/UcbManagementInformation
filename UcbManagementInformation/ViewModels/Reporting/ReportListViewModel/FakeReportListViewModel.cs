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
using System.Collections.ObjectModel;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Models;
using UcbManagementInformation.Web.Models;

namespace UcbManagementInformation.ViewModels
{
    public class FakeReportListViewModel : ViewModel,IReportListViewModel
    {
        private static ObservableCollection<Report> reports;
        private static ObservableCollection<ReportGroupFolder> rootGroups;

        public ObservableCollection<Report> Reports
        {
            get { return reports; }
            set { reports = value; }
        }

        public ObservableCollection<ReportGroupFolder> RootGroups
        {
            get { return rootGroups; }
            set { rootGroups = value; }
        }
        static FakeReportListViewModel()
        {
            reports = new ObservableCollection<Report>
            {
                new Report
                {
                    Name="Report1",
                    IsStandard=true,
                    Description = "Test Report",
                    CreationDate=DateTime.Now,
                    CreationUserID="TestUser1",
                    ModifiedDate=DateTime.Now
                },
                new Report
                {
                    Name="Report2",
                    IsStandard=true,
                    Description = "Test Report",
                    CreationDate=DateTime.Now,
                    CreationUserID="TestUser1",
                    ModifiedDate=DateTime.Now
                },
                new Report
                {
                    Name="Report3",
                    IsStandard=true,
                    Description = "Test Report",
                    CreationDate=DateTime.Now,
                    CreationUserID="TestUser3",
                    ModifiedDate=DateTime.Now
                },
                new Report
                {
                    Name="Report4",
                    IsStandard=true,
                    Description = "Test Report",
                    CreationDate=DateTime.Now,
                    CreationUserID="TestUser2",
                    ModifiedDate=DateTime.Now
                },
                new Report
                {
                    Name="Report5",
                    IsStandard=true,
                    Description = "Test Report",
                    CreationDate=DateTime.Now,
                    CreationUserID="TestUser5",
                    ModifiedDate=DateTime.Now
                },
                new Report
                {
                    Name="Report6",
                    IsStandard=true,
                    Description = "Test Report",
                    CreationDate=DateTime.Now,
                    CreationUserID="TestUser1",
                    ModifiedDate=DateTime.Now
                },
                new Report
                {
                    Name="Report7",
                    IsStandard=true,
                    Description = "Test Report",
                    CreationDate=DateTime.Now,
                    CreationUserID="TestUser6",
                    ModifiedDate=DateTime.Now
                },
                new Report
                {
                    Name="Report8",
                    IsStandard=true,
                    Description = "Test Report",
                    CreationDate=DateTime.Now,
                    CreationUserID="TestUser1",
                    ModifiedDate=DateTime.Now
                },
                new Report
                {
                    Name="Report9",
                    IsStandard=true,
                    Description = "Test Report",
                    CreationDate=DateTime.Now,
                    CreationUserID="TestUser8",
                    ModifiedDate=DateTime.Now
                },
                new Report
                {
                    Name="Report10",
                    IsStandard=true,
                    Description = "Test Report",
                    CreationDate=DateTime.Now,
                    CreationUserID="TestUser1",
                    ModifiedDate=DateTime.Now
                }
            };
        

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
                   
            };
        }

        public Commanding.RelayCommand EditReportCommand
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsRetreivingReportGroups
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsRetreivingReports
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Commanding.RelayCommand LoadedCommand
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Helpers.INavigationService NavigationService
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Report SelectedReport
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ReportGroupFolder SelectedReportGroup
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Commanding.RelayCommand ViewReportCommand
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
