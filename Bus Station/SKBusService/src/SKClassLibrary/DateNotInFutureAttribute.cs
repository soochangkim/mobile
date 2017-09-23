/*
 *  DateNotInFutureAttribute.cs
 *  Assignment 4
 *  Created By:
 *      Soochang Kim, 7227663
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SKClassLibrary
{
    public class DateNotInFutureAttribute : ValidationAttribute
    {
        SKValidations customValidator = new SKValidations();
        /// <summary>
        /// Method to validate date field
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            if (!customValidator.isEmpty(value) && DateTime.Parse(value.ToString()) > DateTime.Now)
            {
                return new ValidationResult($"{validationContext.DisplayName} can not be future");
            }
                
            return ValidationResult.Success;
        }
        
        /// <summary>
        /// To check if the given value is not in future
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if the given date is not in future</returns>
        public override bool IsValid(object value)
        {
            if (!customValidator.isEmpty(value) && DateTime.Parse(value.ToString()) > DateTime.Now)
            {
                return false;
            }
            return true;
        }
    }
}
