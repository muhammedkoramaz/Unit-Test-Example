using JobApplicationLibrary;
using JobApplicationLibrary.Models;
using NUnit.Framework;
using Moq;
using JobApplicationLibrary.Services;
using FluentAssertions;
using System;

namespace JobApplicationLibraryUnitTest
{
    public class ApplicationEvaluateUnitTest
    {
        //Ýsimlendirme olarak ( UnitOfWork_Condition_ExpectedResult) formatý tavsiye edilir.
        [Test]
        public void Application_WithUnderAge_TransferredToAutoRejected()
        {
            //Arrange
            var evaluator = new ApplicationEvaluator(null);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 17 
                }

            };
            //Action
            var appResult = evaluator.Evaluate(form);
            //Assert
            Assert.AreEqual(appResult, ApplicationResult.AutoRejected);

        }

        [Test]
        public void Application_WithNoTechStack_TransferredToAutoRejected()
        {
            //Arrange

            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i => i.isValid(It.IsAny<string>())).Returns(true);
            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 19},
                TechStackList = new System.Collections.Generic.List<string>() { "" },
                OfficeLocation = "KAYSERI"
            };
            //Action
            var appResult = evaluator.Evaluate(form);
            //Assert
            Assert.AreEqual(ApplicationResult.AutoRejected, appResult);

        }

        [Test]
        public void Application_WithTechStack75P_TransferredToAutoAccepted()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i => i.isValid(It.IsAny<string>())).Returns(true);
            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 19 },
                TechStackList = new System.Collections.Generic.List<string>() { "C#", "RabbitMQ", "Microservice", "Visual Studio" },
                YearsOfExperience = 16,
                OfficeLocation = "KAYSERI"

            };
            //Action
            var appResult = evaluator.Evaluate(form);
            //Assert
            Assert.AreEqual(appResult, ApplicationResult.AutoAccepted);

        }

        [Test]
        public void Application_WithValideIdentityNumber_TransferredToHR()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i => i.isValid(It.IsAny<string>())).Returns(false);
            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 19 },
                OfficeLocation = "KAYSERI"
            };
            //Action
            var appResult = evaluator.Evaluate(form);
            //Assert
            Assert.AreEqual(ApplicationResult.TransferredToHR, appResult);

        }
        [Test]
        public void Application_WithOfficeLocation_TransferredToCTO()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 19 },
                OfficeLocation = "Ankara"
            };
            //Action
            var appResult = evaluator.Evaluate(form);
            //Assert
            //Assert.AreEqual(ApplicationResult.TransferredToCTO, appResult);
            appResult.Should().Be(ApplicationResult.TransferredToCTO); //Fluent Assertion
        }

        [Test]

        public void Application_WithOver50_ValidationModeToDetailed()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.SetupProperty(i => i.ValidationMode);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 51 },
                OfficeLocation = "Ankara"
            };
            //Action
            var appResult = evaluator.Evaluate(form);
            //Assert
            //  Assert.AreEqual(ValidationMode.Detailed, mockValidator.Object.ValidationMode);
            mockValidator.Object.ValidationMode.Should().Be(ValidationMode.Detailed); //Fluent Assertion
        }

        [Test]

        public void Application_WithNullControlApplicatant_ThrowsArgumentNullException()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication();
            //Action
            Action appResultAction = () => evaluator.Evaluate(form);
            //Assert
            appResultAction.Should().Throw<ArgumentNullException>();    
        }

    }
}