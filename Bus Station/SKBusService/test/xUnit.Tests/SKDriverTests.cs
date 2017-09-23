using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SKBusService.Controllers;
using SKBusService.Models;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using SKClassLibrary;
using Microsoft.AspNetCore.Mvc;

namespace xUnit.Tests
{

    // Use region here, make cleanup method, single tone

    /// <summary>
    /// Class to test driver data
    /// </summary>
    public class SKDriverTests
    {
        #region Fileds to test Driver model
        protected BusServiceContext _context = BusServiceContext_Singleton.Context();//new BusServiceContext(CreateOptions());
        protected Driver driver;
        protected Random random = new Random();
        protected Boolean recordAccepted = true;
        protected string validationResults = "";
        #endregion

        #region Utilites for testing
        /// <summary>
        /// To initialise ideal model for driver
        /// </summary>
        public void Initialise()
        {
            recordAccepted = true;
            validationResults = "";

            driver = new Driver();
            driver.City = "Kitchener";
            driver.DateHired = new DateTime(1987, 06, 23);
            driver.FirstName = "Soochang";
            driver.LastName = "Kim";
            driver.FullName = driver.LastName + ", " + driver.FirstName;
            driver.HomePhone = "647-833-9916";
            driver.WorkPhone = "010-899-9903";
            driver.PostalCode = "A1B 2C3";
            driver.ProvinceCode = "ON";
            driver.Street = "72 Pinncale Drive";
        }

        /// <summary>
        /// To validate given driver object
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns>Boolean result of the validation</returns>
        public Boolean RunValidate(out string errorMessage)
        {
            Boolean recordPassedValidate = true;
            errorMessage = "";

            List<ValidationResult> ValidationResults = driver.Validate(new ValidationContext(driver)).ToList();
            foreach (ValidationResult x in ValidationResults)
            {
                if (x != ValidationResult.Success)
                {
                    recordPassedValidate = false;
                    errorMessage += $"<<<{x.ErrorMessage}>>>";
                }
            }
            return recordPassedValidate;
        }

        public void Clean()
        {
            _context.Entry(driver).State = EntityState.Detached;
        }

        /// <summary>
        /// To run validation method
        /// </summary>
        [Fact]
        public void VenchmarkDriverObject_ShouldPass()
        {
            Initialise();
            recordAccepted = RunValidate(out validationResults);
            Assert.True(recordAccepted, $"{validationResults}");
        }
        #endregion

        #region Potal code validtion part
        /// <summary>
        /// No seperator with lower case
        /// </summary>        
        [Fact]
        public void PostalCodeNoSpaceWithLowerCase_ShouldPassEdit()
        {
            //arrange
            Initialise();
            driver.PostalCode = "a1b2c3";
            //act
            recordAccepted = RunValidate(out validationResults);
            //assert
            Assert.True(recordAccepted, validationResults);
        }

        /// <summary>
        /// Space seperatoer with lower case
        /// </summary>
        [Fact]
        public void PostalCodeSpaceWithLowerCase_ShouldPassEdit()
        {
            //arrange
            Initialise();
            driver.PostalCode = "a1b 2c3";
            //act
            recordAccepted = RunValidate(out validationResults);
            //assert
            Assert.True(recordAccepted, validationResults);
        }

        /// <summary>
        /// Dash seperator with lower case
        /// </summary>
        [Fact]
        public void PostalCodeDashWithLowerCase_ShouldPassEdit()
        {
            //arrange
            Initialise();
            driver.PostalCode = "a1b-2c3";
            //act
            recordAccepted = RunValidate(out validationResults);
            //assert
            Assert.True(recordAccepted, validationResults);
        }

        /// <summary>
        /// No seperator with Upper case
        /// </summary>        
        [Fact]
        public void PostalCodeNoSpaceWithUpperCase_ShouldPassEdit()
        {
            //arrange
            Initialise();
            driver.PostalCode = "A1B2C3";
            //act
            recordAccepted = RunValidate(out validationResults);
            //assert
            Assert.True(recordAccepted, validationResults);
        }

        /// <summary>
        /// Space seperatoer with lower letters
        /// </summary>
        [Fact]
        public void PostalCodeSpaceWithUpperCase_ShouldPassEdit()
        {
            //arrange
            Initialise();
            driver.PostalCode = "A1B 2C3";
            //act
            recordAccepted = RunValidate(out validationResults);
            //assert
            Assert.True(recordAccepted, validationResults);
        }

        /// <summary>
        /// Dash seperator with lower letters
        /// </summary>
        [Fact]
        public void PostalCodeDashWithUpperCase_ShouldPassEdit()
        {
            //arrange
            Initialise();
            driver.PostalCode = "A1B-2C3";
            //act
            recordAccepted = RunValidate(out validationResults);
            //assert
            Assert.True(recordAccepted, validationResults);
        }



        /// <summary>
        /// Theory to test multiple invalid inputs
        /// </summary>
        /// <param name="invalidInput">invalid format of postal code</param>
        [Theory]
        [InlineData("D1A 2C3")]
        [InlineData("F1A 2C3")]
        [InlineData("I1A 2C3")]
        [InlineData("O1A 2C3")]
        [InlineData("Q1A 2C3")]
        [InlineData("U1A 2C3")]
        [InlineData("Z1A 2C3")]
        [InlineData("A1D 2B2")]
        [InlineData("A1F 2B2")]
        [InlineData("A1I 2B2")]
        [InlineData("A1O 2B2")]
        [InlineData("A1Q 2B2")]
        [InlineData("A1B 2D3")]
        [InlineData("A1B 2F3")]
        [InlineData("A1B 2I3")]
        [InlineData("A1B 2O3")]
        [InlineData("A1B 2Q3")]
        public void PostalCodeInvalidLetters_ShouldNotPassEdit(string invalidInput)
        {
            // arrange
            Initialise();
            driver.PostalCode = invalidInput;

            // act

            PostalCodeValidationAttribute tester = new PostalCodeValidationAttribute();
            recordAccepted = tester.IsValid(invalidInput);

            // assert
            Assert.False(recordAccepted, validationResults);
        }
        #endregion

        #region Rquire filed validation part

        [Fact]
        public void FirstNameNull_ShouldNotPassEdit()
        {
            // arrange
            Initialise();
            driver.FirstName = null;

            // act
            recordAccepted = RunValidate(out validationResults);

            // assert
            Assert.False(recordAccepted, validationResults);
        }

        [Fact]
        public void LastNameNull_ShouldNotPassEdit()
        {
            // arrange
            Initialise();
            driver.LastName = null;

            // act
            recordAccepted = RunValidate(out validationResults);

            // assert
            Assert.False(recordAccepted, validationResults);
        }


        [Fact]
        public void HomePhoneNull_ShouldNotPassEdit()
        {
            // arrange
            Initialise();
            driver.HomePhone = null;

            // act
            recordAccepted = RunValidate(out validationResults);

            // assert
            Assert.False(recordAccepted, validationResults);
        }


        [Fact]
        public void PostalCodeNull_ShouldNotPassEdit()
        {
            // arrange
            Initialise();
            driver.PostalCode = null;

            // act
            recordAccepted = RunValidate(out validationResults);

            // assert
            Assert.False(recordAccepted, validationResults);
        }


        [Fact]
        public void DateHiredNull_ShouldNotPassEdit()
        {
            // arrange
            Initialise();
            driver.DateHired = null;

            // act
            recordAccepted = RunValidate(out validationResults);

            // assert
            Assert.False(recordAccepted, validationResults);
        }

        [Fact]
        public void FirstNameOnlyWhiteSpace_ShouldNotPassEdit()
        {
            // arrange
            Initialise();
            driver.FirstName = null;

            // act
            recordAccepted = RunValidate(out validationResults);

            // assert
            Assert.False(recordAccepted, validationResults);
        }

        [Fact]
        public void LastNameOnlyWhiteSpace_ShouldNotPassEdit()
        {
            // arrange
            Initialise();
            driver.LastName = null;

            // act
            recordAccepted = RunValidate(out validationResults);

            // assert
            Assert.False(recordAccepted, validationResults);
        }



        #endregion

        #region controller exception handling
        [Fact]
        public async void CreateController_ShouldCatchDbException()
        {
            Initialise();
            driver.ProvinceCode = "green";
            SKDriverController dc = new SKDriverController(_context);
            try
            {
                await dc.Create(driver);
            }
            catch (Exception)
            {
                Assert.True(false, "Faild to catch exception");
            }
        }

        [Fact]
        public async void CreateController_PutExceptionIntoModelState()
        {
            Initialise();
            driver.ProvinceCode = "green";
            SKDriverController controller = new SKDriverController(_context);
            // act            
            var result = await controller.Create(driver);
            // assert
            Assert.NotNull(result);
            Assert.IsType(typeof(ViewResult), result);
            ViewResult viewResult = (ViewResult)result;
            Assert.NotEmpty(viewResult.ViewData.ModelState.Keys);
            Assert.Equal("", viewResult.ViewData.ModelState.Keys.First());
        }

        [Fact]
        public async void EditController_ShouldCatchDbException()
        {
            Initialise();
            driver.ProvinceCode = "green";
            SKDriverController dc = new SKDriverController(_context);
            try
            {
                await dc.Edit(driver.DriverId, driver);
            }
            catch (Exception)
            {
                Assert.True(false, "Faild to catch exception");
            }
        }

        [Fact]
        public async void DeleteController_ShouldCatchDbException()
        {
            Initialise();
            driver.DriverId = 122;
            SKDriverController dc = new SKDriverController(_context);
            try
            {
                await dc.DeleteConfirmed(driver.DriverId);
            }
            catch (Exception)
            {
                Assert.True(false, "Faild to catch exception");
            }
        }


        #endregion

        #region Formatting after validation

        [Fact]
        public void PostalCodeFormat_ShouldPass()
        {
            Initialise();
            driver.PostalCode = "a1b2c3";
            recordAccepted = RunValidate(out validationResults);
            Assert.Equal("A1B 2C3", driver.PostalCode);
        }

        [Fact]
        public void HomePhoneFormat_ShouldPass()
        {
            Initialise();
            driver.HomePhone = "6aaa4xxxx7....833,,mtymjrth99dsffs16";
            recordAccepted = RunValidate(out validationResults);
            Assert.Equal("647-833-9916", driver.HomePhone);
        }

        [Fact]
        public void WorkPhoneFormat_ShouldPass()
        {
            Initialise();
            driver.HomePhone = "6aaa4xxxx7....833,,mtymjrth99dsffs16";
            recordAccepted = RunValidate(out validationResults);
            Assert.Equal("647-833-9916", driver.HomePhone);
        }

        [Fact]
        public void StreetAddressCaptalise_ShouldPass()
        {
            Initialise();
            driver.Street = "72 pinnacle drive";
            recordAccepted = RunValidate(out validationResults);
            Assert.Equal("72 Pinnacle Drive", driver.Street);
        }

        [Fact]
        public void HiredDateFuture_ShouldNotPass()
        {
            Initialise();
            driver.DateHired = new DateTime(2020, 10, 10);

            recordAccepted = RunValidate(out validationResults);

            Assert.False(recordAccepted, validationResults);
        }

        #endregion
    }
}
