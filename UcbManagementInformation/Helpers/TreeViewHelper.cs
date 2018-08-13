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
using UcbManagementInformation.Web.Models;
using System.Collections.Generic;
using UcbManagementInformation.Models;
using System.Collections.ObjectModel;
using System.Linq;
using UcbManagementInformation.Server.DataAccess;

namespace UcbManagementInformation.Helpers
{
    public class TreeViewHelper
    {
        public static void AssembleTreeView(IList<ReportGroupFolder> rootGroups,
                  ReportGroupAccessLevelType requiredAccessLevel, List<ReportGroup> ReportGroupForUserList, bool DisplayTempGroup)
        { AssembleTreeView(rootGroups, requiredAccessLevel, ReportGroupForUserList, DisplayTempGroup, Guid.Empty); }
        /// <summary>
        /// Create a TreeView that contains nodes populated from ReportGroup table
        /// </summary>
        /// <param name="rootTreeNodes">A TreeNodeCollection to be populated with 
        /// report group nodes.</param>
        /// <param name="requiredAccessLevel">Required access level for this TreeView.</param>
        /// <param name="ReportGroupForUserList">List of ReportGroupForUser objects
        /// to use in populating the TreeView.</param>
        /// <param name="DisplayTempGroup">Indicates whether the Temp Folder should be displayed in the tree view.</param>
        public static void AssembleTreeView(IList<ReportGroupFolder> rootGroups,
            ReportGroupAccessLevelType requiredAccessLevel, List<ReportGroup> ReportGroupForUserList, bool DisplayTempGroup,Guid initialExpandedGroup)
        {
            ReportGroupForUserList = ReportGroupForUserList.OrderBy(x => x.ParentPath).ToList();
            foreach (ReportGroup CurrentReportGroupForUser in ReportGroupForUserList)
            {
                //Only display temp folder if true.
                if (CurrentReportGroupForUser.ParentPath == null)
                {
                    //Do not display temp group unless explicitly requested
                    if (CurrentReportGroupForUser.Name == "Temp" && DisplayTempGroup == false)
                        continue;

                    //add a root tree node
                    ReportGroupFolder NodeToAdd = new ReportGroupFolder();
                    NodeToAdd.Code = CurrentReportGroupForUser.Code;
                    NodeToAdd.Name = CurrentReportGroupForUser.Name;
                    NodeToAdd.FullPath = CurrentReportGroupForUser.Name + "/"; 
                    NodeToAdd.Parent = null;
                    if (CurrentReportGroupForUser.Name == "Temp")
                    {
                        NodeToAdd.IsTemp = true;
                    }
                    NodeToAdd.AccessLevel = CurrentReportGroupForUser.AccessLevel;

                    //Temp folder must be Yellow.
                    if (CurrentReportGroupForUser.AccessLevel >= requiredAccessLevel
                        || CurrentReportGroupForUser.Name == "Temp")
                    {
                        // Required access level is less than or equal to user
                        // access level, so show access granted folder
                        NodeToAdd.Color = "Yellow";
                    }
                    else
                    {
                        // Required access level must be greater than user access level, so
                        // image shows access denied folder
                        NodeToAdd.Color = "Gray";// ImageIndex = 2;
                        
                    }

                    rootGroups.Add(NodeToAdd);
                }
                else
                {
                    //add a root tree node with all the child tree nodes
                    PopulateTreeViewRecursive(rootGroups, CurrentReportGroupForUser,
                        requiredAccessLevel,initialExpandedGroup);
                }
            }
        }

        private static bool PopulateTreeViewRecursive(IList<ReportGroupFolder> currentReportGroupChildren,
            ReportGroup currentReportGroupForUser, ReportGroupAccessLevelType requiredAccessLevel, Guid initialExpandedGroup)
        {
            bool IsFoundNode = false;

            foreach (ReportGroupFolder currentReportGroup in currentReportGroupChildren)
            {
                if (currentReportGroup.FullPath == currentReportGroupForUser.ParentPath)
                {
                    ReportGroupFolder NodeToAdd = new ReportGroupFolder();

                    /* TIR0003 DMH - Access ia Parent Group has already been established.
                    // If the user does not have an access level set it is inherited from its parent
                    if (currentReportGroupForUser.AccessLevel == ReportGroupAccessLevelType.None)
                    {
                        currentReportGroupForUser.AccessLevel = 
                            ((ReportGroupForUser)currentTreeNode.Tag).AccessLevel;
                    }
                    */

                    //Expand all tree nodes down to the initially selected one.
                    if (currentReportGroupForUser.Code == initialExpandedGroup)
                    {
                        NodeToAdd.IsExpanded = false;
                        ReportGroupFolder ParentReportGroup = currentReportGroup;
                        while (ParentReportGroup.Parent != null)
                        {
                            ParentReportGroup.IsExpanded = true;
                            ParentReportGroup = ParentReportGroup.Parent;

                        }
                        ParentReportGroup.IsExpanded = true;
                    }
                        

                    if (currentReportGroupForUser.AccessLevel < requiredAccessLevel)
                    {
                        //set folder icon to grey for folders that the users do not have access to
                        NodeToAdd.Color = "Gray";// SelectedImageIndex = 2;
                        
                    }


                    if (currentReportGroupForUser.AccessLevel >= requiredAccessLevel)
                    {
                        // DBS Code Review Issue : 17/03/2005 - Need to change image colour
                        //DBS 31/03/2005 image changed to yellow
                        NodeToAdd.Color = "Yellow";
                        //go back up tree to set image colour to red if access not set
                        ReportGroupFolder ParentReportGroup = currentReportGroup;
                        //04/07/05 LL - TIR0178 - Root Parent Folder needs to be set to red image colour 
                        //if not already yellow
                        if (ParentReportGroup.Parent == null)
                        {
                            if (ParentReportGroup.AccessLevel < requiredAccessLevel)
                            {
                                ParentReportGroup.Color = "Red";
                            }
                        }

                        //TIR0004 Set Parent group to Red only if not already yellow.
                        while (ParentReportGroup.Parent != null)
                        {
                            if (ParentReportGroup.AccessLevel < requiredAccessLevel)
                            {

                                ParentReportGroup.Color = "Red";
                            }
                            ParentReportGroup = ParentReportGroup.Parent;

                        }


                    }
                    //add node
                    NodeToAdd.Name = currentReportGroupForUser.Name;
                    NodeToAdd.FullPath = currentReportGroup.FullPath   + currentReportGroupForUser.Name + "/";
                    NodeToAdd.Code = currentReportGroupForUser.Code;
                    NodeToAdd.IsTemp = (currentReportGroupForUser.Name == "Temp");
                    NodeToAdd.AccessLevel = currentReportGroupForUser.AccessLevel;
                    NodeToAdd.Parent = currentReportGroup;
                    currentReportGroup.Children.Add(NodeToAdd);
                    IsFoundNode = true;
                }
                else
                    IsFoundNode = PopulateTreeViewRecursive(currentReportGroup.Children,
                        currentReportGroupForUser, requiredAccessLevel,initialExpandedGroup);
            }
            return IsFoundNode;
        }

        public static void UpdateList(IList<ReportGroupFolder> oldList,IList<ReportGroup> newList)
        {
            //Recursive update 
            foreach (ReportGroupFolder currentFolder in oldList)
            {
                var FoundInNewList = (from x in newList
                                     where x.Code == currentFolder.Code
                                     select x).FirstOrDefault();
                if (FoundInNewList != null)
                {
                    MarshalMembers(currentFolder,FoundInNewList); 
                }
            }
            //var results = oldList.Where(
            //            entity => !newList.Contains(entity)).ToList();


            //results.ForEach(entity => oldList.Remove(entity));

            //var results2 = newList.Where(entity => !oldList.Contains(entity)).ToList();
            //results2.ForEach(entity => oldList.Add(entity));

            //var results3 = newList.Where(entity => oldList.Contains(entity)).ToList();
            //results3.ForEach(entity => MarshalMembers(entity, oldList));
        }
        public static void MarshalMembers(ReportGroupFolder updatedFolder, ReportGroup newValues)
        {
            updatedFolder.AccessLevel = newValues.AccessLevel;
            //updatedFolder.Color = newValues.Color;
            updatedFolder.FullPath = newValues.PathName;
            updatedFolder.IsTemp = (newValues.Name == "Temp");
            updatedFolder.Name = newValues.Name;
            updatedFolder.RowIdentifier = newValues.RowIdentifier;
        }
        public static IEnumerable<ReportGroup> GetAccessLevels(string userID, IEnumerable<ReportGroup> reportGroupsList)
        {
            foreach (ReportGroup CurrentReportGroup in reportGroupsList)
            {
                CurrentReportGroup.AccessLevel = GetAccessLevel(userID, CurrentReportGroup.PathName, reportGroupsList);
            }
            return reportGroupsList;
        }
        private static ReportGroupAccessLevelType GetAccessLevel(
            string userID, string reportGroupPathName,IEnumerable<ReportGroup> reportGroupsList)
        {
            ReportGroupAccessLevelType MyAccessLevel = ReportGroupAccessLevelType.None;

            // To obtain the UserReportGroup, we need the UserCode and ReportGroupCode
            ReportGroup CurrentReportGroup = reportGroupsList.First(x => x.PathName == reportGroupPathName);
            //MCUser ThisUser = mcUserRepository.Find(x => x.Name == userID).First();

            UserReportGroup CurrentUserReportGroup = CurrentReportGroup.UserReportGroups.FirstOrDefault();
//                userReportGroupRepository.Find(x => x.ReportGroupCode == CurrentReportGroup.Code && x.UserCode == ThisUser.Code).FirstOrDefault();

            /* In getting the access level, we try firstly to obtain it from the
            * specified report group. If it has none, we must then check the parent
            * of this report group for its access level. If the parent has an access
            * level, we return that, otherwise we check this folder's parent. We
            * continue like this, until a level is found or the top of the tree
            * is reached without finding one.
            */

            // This denotes whether the top of the tree has been reached in event of searching.
            bool IsTreeTop = false;

            // This denotes if a UserReportGroup for the ReportGroup has been found.
            bool IsFound = false;

            while (!IsTreeTop && !IsFound)
            {
                // If a UserReportGroup is found, we can stop searching.
                if (CurrentUserReportGroup != null)
                {
                    MyAccessLevel = (ReportGroupAccessLevelType)CurrentUserReportGroup.AccessLevel;
                    IsFound = true;
                }
                else
                {
                    // We must look to its parent for an access level.
                    if (CurrentReportGroup.ParentPath != null)
                    {
                        string ParentPath = CurrentReportGroup.ParentPath;
                        CurrentReportGroup = reportGroupsList.FirstOrDefault(x => x.PathName == ParentPath);
                        if (CurrentReportGroup == null)
                        {
                            IsTreeTop = true;
                        }
                        else
                        {
                            CurrentUserReportGroup = CurrentReportGroup.UserReportGroups.FirstOrDefault();
                            //userReportGroupRepoitory.Find(x => x.ReportGroupCode == CurrentReportGroup.Code && x.UserCode == ThisUser.Code).FirstOrDefault();
                        }
                    }
                    else // The child is a root folder, so has no parent.
                    {
                        // Hence, we must stop searching.
                        IsTreeTop = true;
                    }
                }
            }

            return MyAccessLevel;
        }
    }
}
