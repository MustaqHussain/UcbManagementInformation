//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UcbManagementInformation.Server.DataAccess;

namespace UcbManagementInformation.Server.UnitTest.TestEntityBuilder
{
    public static partial class InputFileErrorBuilder
    {
        #region Create Method
        public static InputFileError Create()
        {
            return new InputFileError
            {
    				Code = Guid.NewGuid(),
    				ErrorMessage = "test ErrorMessage",
    				InputFileHistoryCode = Guid.NewGuid(),
    				RecordKey = "test RecordKey",
    				Category = "test Category",
    				ErrorLevel = 100,
    				RecordDateTime = DateTime.Now,
    				RowIdentifier = null
            };
        }

        #endregion
    
        #region With Methods
       	public static InputFileError WithCode(this InputFileError inputFileError, Guid code)
        {
            inputFileError.Code = code;
            return inputFileError;
        }
       	public static InputFileError WithErrorMessage(this InputFileError inputFileError, String errorMessage)
        {
            inputFileError.ErrorMessage = errorMessage;
            return inputFileError;
        }
       	public static InputFileError WithInputFileHistoryCode(this InputFileError inputFileError, Guid inputFileHistoryCode)
        {
            inputFileError.InputFileHistoryCode = inputFileHistoryCode;
            return inputFileError;
        }
       	public static InputFileError WithRecordKey(this InputFileError inputFileError, String recordKey)
        {
            inputFileError.RecordKey = recordKey;
            return inputFileError;
        }
       	public static InputFileError WithCategory(this InputFileError inputFileError, String category)
        {
            inputFileError.Category = category;
            return inputFileError;
        }
       	public static InputFileError WithErrorLevel(this InputFileError inputFileError, Int32 errorLevel)
        {
            inputFileError.ErrorLevel = errorLevel;
            return inputFileError;
        }
       	public static InputFileError WithRecordDateTime(this InputFileError inputFileError, DateTime recordDateTime)
        {
            inputFileError.RecordDateTime = recordDateTime;
            return inputFileError;
        }
       	public static InputFileError WithInputFileHistory(this InputFileError inputFileError, InputFileHistory inputFileHistory)
        {
            inputFileError.InputFileHistory = inputFileHistory;
            return inputFileError;
        }
    

        #endregion
    }
}
