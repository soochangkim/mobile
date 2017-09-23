/*
 *  PostalCodeValidationAttribute.cs
 *  Assignment 4
 *  Created By:
 *      Soochang Kim, 7227663
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SKClassLibrary
{
    public class PostalCodeValidationAttribute : ValidationAttribute
    {
        protected SKValidations customValidator = new SKValidations();
        protected Regex regexPostal = new Regex(@"^[a-ceghj-nprstvxy]\d[a-ceghj-nprstv-z][- ]?\d[a-ceghj-nprstv-z]\d$", RegexOptions.IgnoreCase);
        protected Regex santa = new Regex(@"h0h[ -.]?0h0", RegexOptions.IgnoreCase);


        /// <summary>
        /// To validation Candian postal code
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(!customValidator.isEmpty(value))
            {
                if (!(value.ToString().Trim() == "") && !regexPostal.IsMatch(value.ToString()))
                {
                    if (validationContext != null)
                    {
                        return new ValidationResult($"{validationContext.DisplayName} must be valid Candian postal code! (ex)A1B-C2D");
                    }
                    else
                    {
                        return new ValidationResult($"{value} must be valid Candian postal code! (ex)A1B-C2D");
                    }
                }
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// To validate postal code
        /// </summary>
        /// <param name="postalCode"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public bool IsValid(string postalCode, ValidationContext validationContext)
        {
            return IsValid((object)postalCode, validationContext) == ValidationResult.Success ? true : false;
        }

        
        /// <summary>
        /// To check if the potal code is for santa
        /// </summary>
        /// <param name="postalCode"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public bool IsSanta(string postalCode, ValidationContext validationContext)
        {
            return santa.IsMatch(postalCode) ? true : false;
        }

        /// <summary>
        /// To formatting postalcode 
        /// </summary>
        /// <param name="postalCode"></param>
        /// <returns></returns>
        public string formatPostalCode(string postalCode)
        {
            postalCode = postalCode.ToUpper();
            if (postalCode.Length != 7)
            {
                postalCode = postalCode.Insert(3, " ");
            }
            else
            {
                postalCode = postalCode.Replace('-', ' ');
            }
            return postalCode;
        }
    }
}
