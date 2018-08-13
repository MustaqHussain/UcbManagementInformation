/*----------------------------------------------------------------------
  Name: ObjectUtility

  Description: Contains methods for manipulating Custom Complex Types
             
  History:
  --------
  28 Feb 2005   1.00    DH  Genesis. 
  09 Mar 2005   1.01    LL  Added IsValidFileName static method
  15 Apr 2005   1.02    LL  Added TransferLikeNamedFieldsToProperties
  21 Apr 2005   1.03    LL  Added an overloaded method for TransferLikeNamedProperties
                            with three parameters
  12 May 2005	1.04	LL	Moved IsValidFileName to DataUtility class	
  14 Jul 2005	1.05	LL	Removed CreateDate & CreateUserID from TransferLikeNamedProperties				
  ----------------------------------------------------------------------*/
using System;
using System.Collections;
using System.Reflection;

namespace UcbManagementInformation.Server.Utilities
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class ObjectUtility
	{
		private ObjectUtility()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region static methods
        /// <summary>
        /// Transfers all property values from the inputObject to a new instance of the outputType
        /// using the PropertyName as the common link.
        /// </summary>
        /// <param name="inputObject">The object containing values</param>
        /// <param name="outputype">The System.Type for the object to create.</param>
        /// <returns></returns>
        public static object TransferLikeNamedProperties(object inputObject,System.Type outputType)
        {
            return TransferLikeNamedProperties(inputObject,outputType, null);
        }

        /// <summary>
		/// Transfers all property values from the inputObject to a new instance of the outputType
		/// using the PropertyName as the common link.
		/// </summary>
		/// <param name="inputObject">The object containing values</param>
		/// <param name="outputype">The System.Type for the object to be transferred to.</param>
        /// <param name="outputObject">The object assigned with the value of the input object</param>
        /// <returns></returns>
		public static object TransferLikeNamedProperties(object inputObject,System.Type outputType, object outputObject)
		{
            //Determine if a new object is returned or a reference object returned
            bool IsNewOutputObject = (outputObject==null?true:false);

			//Get the Type of the Input Object
			Type InputType = inputObject.GetType();
			
			//Retreive all the properties for the input object
			PropertyInfo[] InputObjectPropertyInfoArray = InputType.GetProperties();
			
            //create output object if it's not passed in
            if (IsNewOutputObject == true)
            {
                //Retreive the default Constructor for the Output object. This should be
                //the parameterless one.
                ConstructorInfo OutputObjectConstructorInfo = outputType.GetConstructor(System.Type.EmptyTypes);
			
                //Invoke the constructor to instance the output object
                outputObject = OutputObjectConstructorInfo.Invoke(null);
            }

            //Loop through the properties on the input object and set the like named
			//property to the value of the property on the input object
			foreach (PropertyInfo CurrentPropertyInfo in InputObjectPropertyInfoArray)
			{
                if(IsNewOutputObject == false && 
                    (CurrentPropertyInfo.Name.Equals("Code") |
                    CurrentPropertyInfo.Name.Equals("RowIdentifier")))
                {
                    //for a referenced output object, do not assign code and rowidentifier column
                }
                else
                {
                    object PropertyValue = CurrentPropertyInfo.GetValue(inputObject,null);
                    PropertyInfo OutputPropertInfo = outputType.GetProperty(CurrentPropertyInfo.Name);
				
                    //Only set the value if the type on the input property is the same as the
                    //property on the output object
				
                    if (OutputPropertInfo != null)
                    {
                        if (CurrentPropertyInfo.PropertyType == OutputPropertInfo.PropertyType)
                        {
                            OutputPropertInfo.SetValue(outputObject,PropertyValue,null);
                        }
                    }
                }
			}
			return outputObject;
		}

        /// <summary>
        /// Transfers all property values from the inputObject to a new instance of the outputType
        /// using the PropertyName as the common link.
        /// </summary>
        /// <param name="inputObject">The object containing values</param>
        /// <param name="outputype">The System.Type for the object to create.</param>
        /// <returns></returns>
        public static object TransferLikeNamedPropertiesToFields(object inputObject,System.Type outputType)
        {
            //Get the Type of the Input Object
            Type InputType = inputObject.GetType();
			
            //Retreive all the properties for the input object
            PropertyInfo[] InputObjectPropertyInfoArray = InputType.GetProperties();
			
            //Retreive the default Constructor for the Output object. This should be
            //the parameterless one.
            ConstructorInfo OutputObjectConstructorInfo = outputType.GetConstructor(System.Type.EmptyTypes);
			
            //Invoke the constructor to instance the output object
            object OutputObject = OutputObjectConstructorInfo.Invoke(null);
			
            //Loop through the properties on the input object and set the like named
            //field to the value of the property on the input object
            foreach (PropertyInfo CurrentPropertyInfo in InputObjectPropertyInfoArray)
            {
                object PropertyValue = CurrentPropertyInfo.GetValue(inputObject,null);
                PropertyInfo OutputFieldInfo = OutputObject.GetType().GetProperty(CurrentPropertyInfo.Name);
				
                //Only set the value if the type on the input property is the same as the
                //field on the output object
				if (OutputFieldInfo != null)
				{
                    /*
                    if (CurrentPropertyInfo.PropertyType.ToString() == "System.Collections.ArrayList")
                    {
                        //get property values of arraylist
                        ArrayList PropertyValueArrayList = (ArrayList)PropertyValue;

                        //loop through field list to find the field type same as property type
                        //and populate the field of that type.
                        FieldInfo[] OutputObjectFieldInfoArray = outputType.GetFields();
                        foreach (FieldInfo CurrentFieldInfo in OutputObjectFieldInfoArray)
                        {
                            if (CurrentFieldInfo.FieldType == PropertyValueArrayList[0].GetType())
                            {
                                ArrayList FieldValueArrayList = new ArrayList();
                                foreach(object CurrentPropertyValue in PropertyValueArrayList)
                                {
                                    FieldValueArrayList.Add(ObjectUtility.TransferLikeNamedProperties(
                                        CurrentPropertyValue, CurrentFieldInfo.FieldType));
                                }
                                OutputObject = FieldValueArrayList;
                                break;
                            }
                        }
                    }
                    else
                    {*/
                        if (CurrentPropertyInfo.PropertyType == OutputFieldInfo.PropertyType)
                        {
                            OutputFieldInfo.SetValue(OutputObject,PropertyValue,null);
                        }
                    //} 
				}
            }
            return OutputObject;
        }

        /// <summary>
        /// Transfers all field values from the inputObject to a new instance of the outputType
        /// using the FieldInfo as the common link.
        /// </summary>
        /// <param name="inputObject">The object containing values</param>
        /// <param name="outputype">The System.Type for the object to create.</param>
        /// <returns>The transferred output object</returns>
        public static object TransferLikeNamedFieldsToProperties(object inputObject,System.Type outputType)
        {
            //Get the Type of the Input Object
            Type InputType = inputObject.GetType();
			
            //Retreive all the fields for the input object
            PropertyInfo[] InputObjectFieldInfoArray = InputType.GetProperties();

            //Retreive the default Constructor for the Output object. This should be
            //the parameterless one.
            ConstructorInfo OutputObjectConstructorInfo = outputType.GetConstructor(System.Type.EmptyTypes);
			
            //Invoke the constructor to instance the output object
            object OutputObject = OutputObjectConstructorInfo.Invoke(null);
			
            //Loop through the properties on the input object and set the like named
            //field to the value of the property on the input object
            foreach (PropertyInfo CurrentPropertyInfo in InputObjectFieldInfoArray)
            {
                object FieldValue = CurrentPropertyInfo.GetValue(inputObject,null);
                PropertyInfo OutputPropertyInfo = OutputObject.GetType().GetProperty(CurrentPropertyInfo.Name);
				
                //Only set the value if the type on the input field is the same as the
                //property on the output object
                if (OutputPropertyInfo != null)
                {
                    if (CurrentPropertyInfo.PropertyType == OutputPropertyInfo.PropertyType)
                    {
                        OutputPropertyInfo.SetValue(OutputObject,FieldValue,null);
                    }
                }
            }
            return OutputObject;
        }

		#endregion
	}
}
