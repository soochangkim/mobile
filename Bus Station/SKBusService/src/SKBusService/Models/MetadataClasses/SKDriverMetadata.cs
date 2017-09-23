using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using SKClassLibrary;
using System.Text.RegularExpressions;

namespace SKBusService.Models
{
    [ModelMetadataType(typeof(SKDriverMetadata))]

    //Self validation class
    public partial class Driver : IValidatableObject
    {
        /// <summary>
        /// To instanciate CustomValidator class
        /// </summary>
        SKValidations customValidator = new SKValidations();


        //Use context here to check duplicated id key

        /// <summary>
        /// Validation method to verify given driver data
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationResult.Success;
            var postalcodValidator = new PostalCodeValidationAttribute();

            if(customValidator.isEmpty(FirstName))
            {
                yield return new ValidationResult($"First name Cannot be null", new[] { nameof(FirstName) });
            }

            if(customValidator.isEmpty(LastName))
            {
                yield return new ValidationResult($"Last name Cannot be null", new[] { nameof(LastName) });
            }


            if(!customValidator.isEmpty(FirstName) && !customValidator.isEmpty(LastName))
            {
                FirstName = customValidator.Capitalise(FirstName);
                LastName = customValidator.Capitalise(LastName);
                FullName = LastName + ", " + FirstName;
            }

            if(customValidator.isEmpty(HomePhone) || !(customValidator.countIntger(HomePhone) == 10))
            {
                yield return new ValidationResult($"Home phone number {HomePhone} is not valid! must be 10 digit number", new[] { nameof(HomePhone) });
            }
            else
            {
                HomePhone = customValidator.convertToPhoneNumber(HomePhone, new int[3] { 3, 3, 4 });
            }

            if(!customValidator.isEmpty(WorkPhone) && !(customValidator.countIntger(WorkPhone) == 10))
            {
                yield return new ValidationResult($"Work phone number {WorkPhone} is not valid! must be 10 digit number", new[] { nameof(WorkPhone) });
            }
            else
            {
                WorkPhone = customValidator.convertToPhoneNumber(WorkPhone, new int[3] { 3, 3, 4 });
            }

            var dateNotFutureValidator = new DateNotInFutureAttribute();
            if(customValidator.isEmpty(DateHired) || !dateNotFutureValidator.IsValid(DateHired))
            {
                yield return new ValidationResult($"Hired date '{DateHired}' can not be the future!", new[] { nameof(DateHired) });
            }

            if(!customValidator.isEmpty(ProvinceCode))
            {
                ProvinceCode = ProvinceCode.ToUpper();
            }

            if(customValidator.isEmpty(PostalCode))
            {
                yield return new ValidationResult("Postal Code cannot null", new[] { nameof(PostalCode) });
            }
            else if (!postalcodValidator.IsValid(PostalCode, new ValidationContext(PostalCode)))
            {
                yield return new ValidationResult($"Postal code '{PostalCode}' is not a valid Canadin postal code", new[] { nameof(PostalCode) });
            }
            else
            {
                if (postalcodValidator.IsSanta(PostalCode, new ValidationContext(PostalCode)))
                {
                    FirstName = "SANTA";
                    LastName = "CLAUS";
                    FullName = LastName + ", " + FirstName;
                    City = "NORTH POLE";
                    DateHired = new DateTime(1773,12,24);
                }
                PostalCode = postalcodValidator.formatPostalCode(PostalCode);
            }

            if(!customValidator.isEmpty(Street))
            {
                Street = customValidator.Capitalise(Street);
            }

            if(!customValidator.isEmpty(City))
            {
                City = customValidator.Capitalise(City);
            }
        }
    }

    /// <summary>
    /// Partial class for driver model
    /// </summary>
    public class SKDriverMetadata
    {
        public int DriverId { get; set; }

        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }

        public string FullName { get; set; }

        [Display(Name = "Home Phone")]
        [Required]
        public string HomePhone { get; set; }

        [Display(Name = "Work Phone")]
        public string WorkPhone { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        [Display(Name = "Postal Code")]
        [Required]
        public string PostalCode { get; set; }

        [Display(Name = "Province Code")]
        [Remote("provinceCodeCheck","Remote")]
        public string ProvinceCode { get; set; }

        [Display(Name = "Date Hired")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd MMM yyyy}")]
        [Required]
        public DateTime? DateHired { get; set; }
    }
}
