using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;


namespace UcbManagementInformation.Web.Models
{
    ///// <summary>
    ///// ReportGroup contains report groups and permissions.
    ///// </summary>
    //public class ReportGroupForUser 
    //{
    //    public ReportGroupForUser()
    //    {
    //        //
    //        //  Add constructor logic here
    //        //
    //    }
    //    #region Public Fields (Data Store) Definitions
    //    [Key]
    //    public Guid Code
    //    {
    //        get
    //        {
    //            return _code;

    //        }
    //        set
    //        {
    //            _code = value;
    //        }
    //    }

    //    /// <summary>
    //    /// Public property for getting and setting the The Name of the ReportGroup
    //    /// </summary>
    //    public string Name
    //    {
    //        get
    //        {
    //            return _name;

    //        }
    //        set
    //        {
    //            _name = value;
    //        }
    //    }

    //    /// <summary>
    //    /// Public property for getting and setting the The code of the ReportGroup in which this report group resides.
    //    /// </summary>
    //    public Guid? ParentCode
    //    {
    //        get
    //        {
    //            return _parentCode;

    //        }
    //        set
    //        {
    //            _parentCode = value;
    //        }
    //    }

    //    /// <summary>
    //    /// Public property for getting and setting the The path to this ReportGroup.
    //    /// </summary>
    //    public string ParentPath
    //    {
    //        get
    //        {
    //            return _parentPath;

    //        }
    //        set
    //        {
    //            _parentPath = value;
    //        }
    //    }

    //    /// <summary>
    //    /// Public property for getting and setting the The path to this report group.
    //    /// </summary>
    //    public string PathName
    //    {
    //        get
    //        {
    //            return _pathName;

    //        }
    //        set
    //        {
    //            _pathName = value;
    //        }
    //    }

    //    /// <summary>
    //    /// Public property for getting and setting the Access Level for the report group.
    //    /// </summary>
    //    public ReportGroupAccessLevelType AccessLevel
    //    {
    //        get
    //        {
    //            return _accessLevel;

    //        }
    //        set
    //        {
    //            _accessLevel = value;
    //        }
    //    }

    //    public byte[] RowIdentifier
    //    {
    //        get;
    //        set;
    //    }
    //    #endregion
    //    #region Private Fields (Data Store) Definitions

    //    /// <summary>
    //    /// Private field that contains The Unique Code for the ReportGroup
    //    /// </summary>
    //    private Guid _code;

    //    /// <summary>
    //    /// Private field that contains The Name of the ReportGroup
    //    /// </summary>
    //    private string _name;

    //    /// <summary>
    //    /// Private field that contains The code of the ReportGroup in which this report group resides.
    //    /// </summary>
    //    private Guid? _parentCode;

    //    /// <summary>
    //    /// Private field that contains The path to this ReportGroup.
    //    /// </summary>
    //    private string _parentPath;

    //    /// <summary>
    //    /// Private field that contains The path to this report group.
    //    /// </summary>
    //    private string _pathName;

    //    /// <summary>
    //    /// Private field that contains the user Access Level.
    //    /// </summary>
    //    private ReportGroupAccessLevelType _accessLevel;

        
    //    #endregion

    //}
    public enum ReportGroupAccessLevelType
    {
        None,
        ReadOnly,
        Update,
        Delete,
        Admin
    }
}